﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using System.Windows.Forms;
using T7s_Asset_Downloader.Asset;
using T7s_Asset_Downloader.Crypt;
using T7s_Asset_Downloader.Response;

namespace T7s_Asset_Downloader
{
    public partial class Main : Form
    {
        private static readonly CancellationTokenSource CancelSource = new CancellationTokenSource();
        private readonly List<string> _downloadDoneList = new List<string>();

        private readonly ProgressMessageHandler _downloadProcessMessageHandler =
            new ProgressMessageHandler(new HttpClientHandler());

        private readonly ProgressMessageHandler _postProcessMessageHandler =
            new ProgressMessageHandler(new HttpClientHandler());

        private readonly MakeRequest _request = new MakeRequest();
        private readonly Task _setNewVersion = Define.SetNewVersion();
        private string[] _listResult;
        public bool IsSeveralFiles = true;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            _setNewVersion.Start();
            _request.HttpClientTest(_downloadProcessMessageHandler);
            
            if (File.Exists(Define.GetIndexPath()))
            {
                Define.JsonParse.LoadUrlIndex(Define.GetIndexPath());
                _ini_listResult();
                button_LoadAllResult.Enabled = true;
                if (File.Exists(Define.GetConfingPath()))
                {
                    Define.JsonParse.LoadConfing(Define.GetConfingPath());
                    Define._ini_Coning();
                }
                else
                {
                    Define.NOW_STAUTUS = NOW_STAUTUS.NoneConing;
                    Define.NOW_STAUTUS = NOW_STAUTUS.First;
                }
            }
            else
            {
                button_ReloadAdvance.Enabled = false;

                Define.NOW_STAUTUS = NOW_STAUTUS.NoneIndex;
                Define.NOW_STAUTUS = NOW_STAUTUS.First;
            }

            TestNew();
            ReloadNoticeLabels();
        }

        /// <summary>
        ///     Initialize default the list will show into the listbox.
        /// </summary>
        public void _ini_listResult()
        {
            _listResult = Define.GetDefaultNameList();
            ShowlistResult(Define.GetDefaultNameList(), Define.DefaultShowCount);
        }

        private void ReloadNoticeLabels()
        {
            SetNoticesText(
                Define.NOW_STAUTUS != NOW_STAUTUS.First ? "当前版本 : " + "r" + Define.NowRev : "当前版本 : " + ">> 请获取最新版本",
                label_NowRev);
        }

        private void ReloadProcess(int TotalCount)
        {
            Task.Run(() =>
            {
                var NowProcess = _downloadDoneList.Count / TotalCount * 100;
                SetProgressInt(NowProcess);
            }).Wait();
        }

        private async void TestNew()
        {
            try
            {
                SetNoticesText(">> ... 正在查询游戏最新版本信息", downloadNotice);
                            await _setNewVersion;

            _request._ini_PostClient();
            if (Define.NOW_STAUTUS == NOW_STAUTUS.First)
                if (MessageBox.Show(@" 需要获取一次完整索引文件，可能下载较长时间，是否现在下载 "
                        , @"Notice"
                        , MessageBoxButtons.OKCancel) != DialogResult.OK)
                {
                    SetNoticesText(">> 需要获取一次完整索引文件，请点击获取最新版本", downloadNotice);
                    return;
                }

            SetNoticesText(">> ... 正在自动检测数据最新版本 , 请稍等 ", downloadNotice);

            var updateStatus = await Task.Run(async () =>
            {
                Define.Rev = Define.UserRev = Define.NowRev;
                if (await _request.MakeUpdatePostRequest(Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)) == 0)
                    return await Define.JsonParse.TestUpdateStatusAsync();
                return UPDATE_STATUS.Error;
            });

            switch (updateStatus)
            {
                case UPDATE_STATUS.Error:
                    SetNoticesText(">> Error : 游戏服务器可能在维护中，请稍后重试", downloadNotice);
                    return;
                case UPDATE_STATUS.NoNecessary:
                    SetNoticesText(">> 已经是最新版本 ", downloadNotice);
                    return;
                case UPDATE_STATUS.Ok:
                    if (MessageBox.Show(@" 检测到最新版本 , 请问是否要现在更新 "
                            , @"Notice"
                            , MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        Define.JsonParse.UpdateUrlIndex(Define.JsonParse, true);
                        await Task.Run(() =>
                        {
                            while (Define.IsGetNewComplete == false)
                            {
                            }

                            Define.NOW_STAUTUS = NOW_STAUTUS.Normal;
                            SetNoticesText(">> 就绪 ...", downloadNotice);
                            ReloadNoticeLabels();
                            SetButtomEnabled(true, button_GetNew);
                            SetButtomEnabled(true, button_ReloadAdvance);
                            _ini_listResult();
                        });
                    }
                    else
                    {
                        SetNoticesText(">> 就绪 , 有新版本可以更新 ", downloadNotice);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            }
            catch (Exception e)
            {
                MessageBox.Show($@"请检测网络 ： {e.Message}");
            }
            

        }


        /// <summary>
        ///     Main control download method.
        /// </summary>
        /// <param name="downloadType">下载类型:少量下载或大量下载</param>
        private async void StartDownload(DOWNLOAD_TYPE downloadType)
        {
            if (!Directory.Exists(Define.GetFileSavePath())) Directory.CreateDirectory(Define.GetFileSavePath());
            //Initialize the UI about download
            SetProgressInt(0);
            button_DownloadCancel.Visible = true;

            string[] willDownloadList;
            int totalCount;
            var nowFileIndex = 0;

            switch (downloadType)
            {
                case DOWNLOAD_TYPE.AllFiles:
                    totalCount = _listResult.Length;
                    willDownloadList = _listResult;
                    break;
                case DOWNLOAD_TYPE.SeletFiles:
                    totalCount = listBoxResult.SelectedItems.Count;
                    if (totalCount < 1)
                    {
                        MessageBox.Show(@"未选择要下载的文件，请点击选择某项或多项文件，再开始下载。", "Notice");
                        button_DownloadCancel.Visible = false;
                        return;
                    }

                    var checkedNameList = new string[totalCount];
                    for (var i = 0; i < totalCount; i++) checkedNameList[i] = listBoxResult.SelectedItems[i].ToString();
                    willDownloadList = checkedNameList;
                    break;
                default:
                    totalCount = _listResult.Length;
                    willDownloadList = _listResult;
                    break;
            }

            //Start Downlaod
            try
            {
                var cancelToken = CancelSource.Token;
                var scheduler = new LimitedConcurrencyLevelTaskScheduler(Define.MaxDownloadTasks);
                var downloadTaskFactory = new TaskFactory(scheduler);
                if (totalCount <= 15)
                {
                    IsSeveralFiles = true;
                    foreach (var fileName in willDownloadList)
                        await downloadTaskFactory.StartNew(async nowFileName =>
                        {
                            nowFileIndex++;
                            await DownloadFiles(fileName, nowFileIndex, totalCount, AUTO_DECRYPT.Auto);
                        }, fileName);
                }
                else
                {
                    IsSeveralFiles = false;
                    Define.DownloadTaskSleep = totalCount < 200 ? 100 : totalCount > 1000 ? 500 : totalCount / 3;

                    foreach (var fileName in willDownloadList)
                        await downloadTaskFactory.StartNew(async nowFileName =>
                        {
                            nowFileIndex++;
                            Thread.Sleep(nowFileIndex % 25 == 0
                                ? 500
                                : Define.DownloadTaskSleep);
                            await DownloadFiles(nowFileName.ToString(), nowFileIndex, totalCount, AUTO_DECRYPT.Auto);
                        }, fileName, cancelToken);
                }

                cancelToken.ThrowIfCancellationRequested();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void OnDownloadDone(int totalCount)
        {
            if (_downloadDoneList.Count != totalCount) return;
            GC.Collect();
            Thread.Sleep(200);
            SetNoticesText("下载完成 >> 共 " + totalCount + " 个文件 ! !", downloadNotice);
            SetButtomVisibled(false, button_DownloadCancel);
        }


        /// <summary>
        ///     Main Download method
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileNameIndex"></param>
        /// <param name="totalCount"></param>
        /// <param name="AUTO_DECRYPT"></param>
        /// <returns></returns>
        private async Task DownloadFiles(string fileName, int fileNameIndex, int totalCount, AUTO_DECRYPT AUTO_DECRYPT)
        {
            SetNoticesText("正在下载 ... " + _downloadDoneList.Count + " / " + totalCount, downloadNotice);
            if (IsSeveralFiles)
                _downloadProcessMessageHandler.HttpReceiveProgress += (senders, es) =>
                {
                    var num = es.ProgressPercentage;
                    SetProgressInt(num);
                };
            _downloadDoneList.Add(await _request.MakeGetRequest(Define.GetUrl(fileName), Define.GetFileSavePath(),
                fileName));
            SetNoticesText("正在下载 ... " + _downloadDoneList.Count + " / " + totalCount, downloadNotice);

            if (!IsSeveralFiles) ReloadProcess(totalCount);

            if (AUTO_DECRYPT == AUTO_DECRYPT.Auto)
                if (Save.GetFileType(fileName) != ENC_TYPE.ERROR)
                    DecryptFiles.DecryptFile(Define.GetFileSavePath() + fileName,
                        Crypt.Crypt.IdentifyEncVersion(fileName));

            OnDownloadDone(totalCount);
        }

        /// <summary>
        ///     Main post method
        /// </summary>
        /// <param name="index"></param>
        /// <param name="update"></param>
        private async Task<UPDATE_STATUS> StartPost(bool index = false, bool update = false)
        {
            SetNoticesText("正在获取新版本数据 ...请稍等...", downloadNotice);
            var jsonParse = new JsonParse();
            _postProcessMessageHandler.HttpSendProgress += (senders, es) =>
            {
                var num = es.ProgressPercentage;
                SetProgressInt(num);
            };
            if (!index)
                return await jsonParse.SaveDlConfing(
                    _request.MakePostRequest(Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)), true);

            if (update)
            {
                if (await _request.MakeUpdatePostRequest(Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)) == 0)
                    Define.JsonParse.UpdateUrlIndex(Define.JsonParse, true);
            }
            else
            {
                jsonParse.SaveUrlIndex(
                    _request.MakePostRequest(Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)), true);
            }

            return UPDATE_STATUS.Ok;
        }

        /// <summary>
        ///     Show the list into listbox.
        /// </summary>
        /// <param name="nameList"></param>
        /// <param name="showCount"></param>
        private void ShowlistResult(string[] nameList, int showCount)
        {
            Task.Run(() =>
            {
                for (var i = 0; i < showCount; i++) SetlistResult(nameList[i]);
            });
        }

        /// <summary>
        ///     Show the list into listbox.
        /// </summary>
        /// <param name="nameList"></param>
        /// <param name="startIndex"></param>
        /// <param name="showCount"></param>
        private void ShowlistResult(string[] nameList, int startIndex, int showCount)
        {
            Task.Run(() =>
            {
                for (var i = startIndex; i < showCount; i++) SetlistResult(nameList[i]);
            });
        }

        private void Button_DownloadCancel_Click(object sender, EventArgs e)
        {
            CancelSource.Cancel();
        }

        private delegate void SetNotices(string notices, Label label);

        private delegate void SetProgress(int progress);

        private delegate void SetCallBack(object obj);

        private delegate void SetEnable(bool enabled, Button button);

        private delegate void SetVisible(bool visible, Button button);

        #region UI逻辑

        private void Button_DownloadAllFiles_Click(object sender, EventArgs e)
        {
            _downloadDoneList.Clear();
            if (_listResult.Length > 50)
                if (MessageBox.Show($@"请注意，所选文件量为{_listResult.Length}个" + @"下载可能会花费较长时间。", @"Notices",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)
                    == DialogResult.Cancel)
                    return;
            ;
            StartDownload(DOWNLOAD_TYPE.AllFiles);
        }

        private void Button_DownloadCheckFiles_Click(object sender, EventArgs e)
        {
            _downloadDoneList.Clear();
            StartDownload(DOWNLOAD_TYPE.SeletFiles);
        }

        private void Button_OpenDownloadPath_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Define.GetFileSavePath())) Directory.CreateDirectory(Define.GetFileSavePath());
            Process.Start("Explorer.exe", Define.GetFileSavePath());
        }

        private void Button_LoadAllResult_Click(object sender, EventArgs e)
        {
            if (_listResult == null) _listResult = Define.GetDefaultNameList();
            ShowlistResult(_listResult, Define.DefaultShowCount, _listResult.Length);
            button_LoadAllResult.Enabled = false;
        }

        private async void TextBox_SearchFiles_TextChanged(object sender, EventArgs e)
        {
            await Task.Delay(1);
            listBoxResult.Items.Clear();
            _listResult = Define.GetListResult(textBox_SeachFiles.Text);
            ShowlistResult(_listResult,
                !(_listResult.Length > Define.DefaultShowCount) ? _listResult.Length : Define.DefaultShowCount);
            button_LoadAllResult.Enabled = true;
        }

        private void Button_ShowAdvance_Click(object sender, EventArgs e)
        {
            var advance = new Advance();
            advance.Show();
        }

        private async void Button_GetNew_Click(object sender, EventArgs e)
        {
            var updateStatus = UPDATE_STATUS.Ok;

            Define.IsGetNewComplete = false;
            button_ReloadAdvance.Enabled = false;
            button_GetNew.Enabled = false;

            if (Define.NOW_STAUTUS == NOW_STAUTUS.First)
            {
                await Task.Run(async () =>
                {
                    Define.Rev = Define.UserRev = (Convert.ToInt32(Define.NowRev) + 300).ToString();
                    updateStatus = await StartPost();
                });

                switch (updateStatus)
                {
                    case UPDATE_STATUS.Error:
                        SetNoticesText(">> Error : 游戏服务器可能在维护中，请稍后重试", downloadNotice);
                        SetButtomEnabled(true, button_GetNew);
                        SetButtomEnabled(true, button_ReloadAdvance);
                        return;
                    case UPDATE_STATUS.Ok:
                        break;
                    case UPDATE_STATUS.NoNecessary:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                await Task.Run(async () =>
                {
                    Define.Rev = Define.UserRev = "001";
                    updateStatus = await StartPost(true);
                });
            }
            else
            {
                await Task.Run(async () =>
                {
                    Define.LastRev = Define.NowRev;
                    Define.Rev = Define.UserRev = (Convert.ToInt32(Define.NowRev) - 3).ToString();
                    updateStatus = await StartPost();
                });

                switch (updateStatus)
                {
                    case UPDATE_STATUS.Error:
                        SetNoticesText(">> Error : 游戏服务器可能在维护中，请稍后重试", downloadNotice);
                        SetButtomEnabled(true, button_GetNew);
                        SetButtomEnabled(true, button_ReloadAdvance);
                        return;
                    case UPDATE_STATUS.NoNecessary:
                        SetNoticesText("已经是最新版本", downloadNotice);
                        SetButtomEnabled(true, button_GetNew);
                        SetButtomEnabled(true, button_ReloadAdvance);
                        return;
                    case UPDATE_STATUS.Ok:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                await Task.Run(async () =>
                {
                    Define.Rev = Define.UserRev = Define.LastRev;
                    updateStatus = await StartPost(true, true);
                });
            }

            await Task.Run(() =>
            {
                while (Define.IsGetNewComplete == false)
                {
                }

                Define.NOW_STAUTUS = NOW_STAUTUS.Normal;
                SetNoticesText(">> 就绪 ...", downloadNotice);
                ReloadNoticeLabels();
                SetButtomEnabled(true, button_GetNew);
                SetButtomEnabled(true, button_ReloadAdvance);
                _ini_listResult();
            });
        }

        private void Button_ReloadAdvance(object sender, EventArgs e)
        {
            listBoxResult.Items.Clear();
            Define._ini_Coning();
            ReloadNoticeLabels();
            _ini_listResult();
        }

        private void Button_About_Click(object sender, EventArgs e)
        {
        }

        private void Button_GetDiffList_Click(object sender, EventArgs e)
        {
            string[] namesList1 = null, namesList2 = null;

            var jsonParse = new JsonParse();

            var ofd = new OpenFileDialog
            {
                Title = "选择要打开的文件",
                Filter = "加密索引文件|Index.json",
                RestoreDirectory = true
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                jsonParse.LoadUrlIndex(ofd.FileName);
                namesList1 = jsonParse.FileUrls.Select(t => t.Name).ToArray();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    jsonParse.LoadUrlIndex(ofd.FileName);
                    namesList2 = jsonParse.FileUrls.Select(t => t.Name).ToArray();
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }

            Define.DiifList = namesList1.Except(namesList2).ToArray();
            _listResult = Define.DiifList;
            listBoxResult.Items.Clear();
            ShowlistResult(Define.DiifList, Define.DiifList.Length);
        }

        #endregion

        #region 委托修改UI

        public void AddListResult(object item)
        {
            listBoxResult.Items.Add(item);
        }

        private void SetNoticesText(string notice, Label label)
        {
            if (label.InvokeRequired)
            {
                SetNotices call = SetNoticesText;
                Invoke(call, notice, label);
            }
            else
            {
                label.Text = notice;
            }
        }

        private void SetButtomEnabled(bool enabled, Button button)
        {
            if (button.InvokeRequired)
            {
                var call = new SetEnable(SetButtomEnabled);
                Invoke(call, enabled, button);
            }
            else
            {
                button.Enabled = enabled;
            }
        }

        private void SetButtomVisibled(bool visibled, Button button)
        {
            if (button.InvokeRequired)
            {
                var call = new SetVisible(SetButtomVisibled);
                Invoke(call, visibled, button);
            }
            else
            {
                button.Visible = visibled;
            }
        }

        private void SetProgressInt(int progress)
        {
            if (downloadProgressBar.InvokeRequired)
            {
                var call = new SetProgress(SetProgressInt);
                Invoke(call, progress);
            }
            else
            {
                downloadProgressBar.Value = progress;
            }
        }

        private void SetlistResult(object item)
        {
            if (listBoxResult.InvokeRequired)
            {
                var call = new SetCallBack(SetlistResult);
                Invoke(call, item);
            }
            else
            {
                AddListResult(item);
            }
        }

        #endregion


        //test

        //private async void DownloadFiles(string fileName, int fileNameIndex, int totalCount, TaskScheduler scheduler, AUTO_DECRYPT AUTO_DECRYPT)
        //{
        //    Action<object> download = async (NowfileName) =>
        //    {
        //        string TempNowfileName = NowfileName.ToString();
        //        SetNoticesText("正在下载 ... " + TempNowfileName + fileNameIndex + "/" + totalCount, downloadNotice);
        //        processMessageHander.HttpReceiveProgress += (senders, es) =>
        //        {
        //            int num = es.ProgressPercentage;
        //            SetProgressInt(num);
        //        };
        //        await new MakeRequest().MkaeGetRequest(Define.GetUrl(TempNowfileName), Define.GetFileSavePath(), TempNowfileName);
        //        if (AUTO_DECRYPT == AUTO_DECRYPT.Auto)
        //        {
        //            if (Save.GetFileType(TempNowfileName) != ENC_TYPE.ERROR)
        //            {
        //                DecryptFiles.DecryptFile(Define.GetFileSavePath() + TempNowfileName);
        //            }
        //        }
        //        DownloadDomeList.Add(NowfileName.ToString());
        //    };
        //    await Task.Factory.StartNew(download, fileName, CancellationToken.None, TaskCreationOptions.None, scheduler).ContinueWith((t, obj) =>
        //    {
        //        if (t.Status != TaskStatus.RanToCompletion)
        //        {
        //            DownloadFiles(fileName, fileNameIndex, totalCount, scheduler, Define.AUTO_DECRYPT);
        //        }
        //    }, fileName);
        //}
    }
}