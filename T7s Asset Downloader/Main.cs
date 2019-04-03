using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using T7s_Enc_Decoder;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Threading;
using System.Threading.Tasks.Schedulers;

namespace T7s_Asset_Downloader
{

    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        private void Main_Load(object sender, EventArgs e)
        {
            var request = new MakeRequest();
            request.HttpClientTest(_downloadProcessMessageHandler);
            if (File.Exists(Define.GetIndexPath()))
            {
                Define.jsonParse.LoadUrlIndex(Define.GetIndexPath(), true);
                _ini_listResult();
                button_LoadAllResult.Enabled = true;
                if (File.Exists(Define.GetConfingPath()))
                {
                    Define.jsonParse.LoadConfing(Define.GetConfingPath(), true);
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
        private static readonly CancellationTokenSource CancelSource = new CancellationTokenSource();
        private readonly ProgressMessageHandler _downloadProcessMessageHandler = new ProgressMessageHandler(new HttpClientHandler());
        private readonly ProgressMessageHandler _postProcessMessageHandler = new ProgressMessageHandler(new HttpClientHandler());
        private delegate void SetNotices( string notices , Label label );
        private delegate void SetProgress( int progress );
        private delegate void SetCallBack( object obj );
        private delegate void SetEnable( bool enabled, Button button);
        private delegate void SetVisible(bool visible, Button button);
        private string[] _listResult;
        public bool IsSeveralFiles = true;
        private readonly List<string> _downloadDoneList = new List<string>();
        private readonly MakeRequest _request = new MakeRequest();

        #region UI逻辑



        private void Button_DownloadAllFiles_Click(object sender, EventArgs e)
        {
            _downloadDoneList.Clear();
            if (_listResult.Length > 50)
            {
                if (MessageBox.Show($"请注意，所选文件量为{_listResult.Length}个" + "下载可能会花费较长时间。", "Notices", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)
                    == DialogResult.Cancel)
                {
                    return;
                }
            };
            StartDownload(DOWNLOAD_TYPE.AllFiles);
        }

        private void Button_DownloadCheckFiles_Click(object sender, EventArgs e)
        {
            _downloadDoneList.Clear();
            StartDownload(DOWNLOAD_TYPE.SeletFiles);
        }

        private void Button_OpenDownloadPath_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(Define.GetFileSavePath()))
            {
                Directory.CreateDirectory(Define.GetFileSavePath());
            }
            System.Diagnostics.Process.Start("Explorer.exe", Define.GetFileSavePath());
        }

        private void Button_LoadAllResult_Click(object sender, EventArgs e)
        {
            if (_listResult == null)
            {
                _listResult = Define.GetDefaultNameList();
            }
            ShowlistResult(_listResult, Define.DefaultShowCount, _listResult.Length);
            button_LoadAllResult.Enabled = false;
        }

        private void TextBox_SeachFiles_TextChanged(object sender, EventArgs e)
        {
            listBoxResult.Items.Clear();
            _listResult = Define.GetListResult(textBox_SeachFiles.Text);
            if (!(_listResult.Length > Define.DefaultShowCount))
            {
                ShowlistResult(_listResult, _listResult.Length);
            }
            else
            {
                ShowlistResult(_listResult, Define.DefaultShowCount);
            }
            button_LoadAllResult.Enabled = !button_LoadAllResult.Enabled ? true : true;
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
                        SetNoticesText("Error", downloadNotice);
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
                    updateStatus = await StartPost(true,true);
                });
            }

            await Task.Run(() =>
            {
                while (Define.IsGetNewComplete == false){ }
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

            JsonParse jsonParse = new JsonParse();

            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "选择要打开的文件",
                Filter = "加密索引文件|Index.json",
                RestoreDirectory = true
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                jsonParse.LoadUrlIndex(ofd.FileName, true);
                namesList1 = jsonParse.FileUrls.Select(t => t.Name).ToArray();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    jsonParse.LoadUrlIndex(ofd.FileName, true);
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
            namesList1 = namesList2 = null;
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
                SetNotices call = new SetNotices(SetNoticesText);
                Invoke(call, new object[] { notice, label });
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
                Invoke(call, new object[] { enabled, button });
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
                SetProgress call = new SetProgress(SetProgressInt);
                Invoke(call, new object[] { progress });
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
                SetCallBack call = new SetCallBack(SetlistResult);
                Invoke(call, new object[] { item });
            }
            else
            {
                AddListResult(item);
            }
        }
        #endregion

        /// <summary>
        /// Initialize default the list will show into the listbox.
        /// </summary>
        public void _ini_listResult()
        {
            _listResult = Define.GetDefaultNameList();
            ShowlistResult(Define.GetDefaultNameList(), Define.DefaultShowCount);
        }
        private void ReloadNoticeLabels()
        {
            SetNoticesText((Define.NOW_STAUTUS != NOW_STAUTUS.First) ? "当前版本 : " + "r" + Define.NowRev : "当前版本 : " + ">> 请获取最新版本", label_NowRev);
        }
        private void ReloadProcess(int TotalCount)
        {
            Task.Run(() =>
            {
                int NowProcess = (_downloadDoneList.Count / TotalCount) * 100;
                SetProgressInt(NowProcess);
            }).Wait();
        }

        private async void TestNew()
        {
            _request._ini_PostClient(_postProcessMessageHandler);
            if (Define.NOW_STAUTUS == NOW_STAUTUS.First)
            {
                SetNoticesText(">> 需要获取一次完整索引文件，请点击获取最新版本", downloadNotice);
                return;
            }

            SetNoticesText(">> ... 正在自动检测最新版本 , 请稍等 ",downloadNotice);

            var updateStatus = await Task.Run(async () =>
            {
                Define.Rev = Define.UserRev = Define.NowRev;
                return await Define.jsonParse.TestUpdateStatusAsync(_request.MakePostRequest(
                        Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)));
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
                    if ((MessageBox.Show(" 检测到最新版本 , 请问是否要现在更新 "
                            , "Notice"
                            , MessageBoxButtons.OKCancel)) == DialogResult.OK)
                    {
                        Button_GetNew_Click(null, null);
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


        /// <summary>
        /// Main control download method. 
        /// </summary>
        /// <param name="downloadType">下载类型:少量下载或大量下载</param>
        private async void StartDownload ( DOWNLOAD_TYPE downloadType )
        {
            if (!Directory.Exists(Define.GetFileSavePath()))
            {
                Directory.CreateDirectory(Define.GetFileSavePath());
            }
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
                        MessageBox.Show("未选择要下载的文件，请点击选择某项或多项文件，再开始下载。", "Notice");
                        button_DownloadCancel.Visible = false;
                        return;
                    }
                    var checkedNameList = new string[totalCount];
                    for (int i = 0; i < totalCount; i++)
                    {
                        checkedNameList[i] = listBoxResult.SelectedItems[i].ToString();
                    }
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
                Task[] downloaDTask = new Task[totalCount];
                var cancelToken = CancelSource.Token;
                var scheduler = new LimitedConcurrencyLevelTaskScheduler(Define.MaxDownloadTasks);
                var downloadTaskFactory = new TaskFactory(scheduler);
                if (totalCount <= 15)
                {
                    IsSeveralFiles = true;
                    foreach (var fileName in willDownloadList)
                    {
                        downloaDTask[nowFileIndex - 1] = await downloadTaskFactory.StartNew(async nowFileName =>
                        {
                            nowFileIndex++;
                            await DownloadFiles(fileName, nowFileIndex, totalCount, AUTO_DECRYPT.Auto);
                        }, fileName, cancelToken);
                    }
                }
                else
                {
                    IsSeveralFiles = false;
                    Define.DownloadTaskSleep = (totalCount < 200) ? 100 : (totalCount > 1000) ? 500 : totalCount / 3;

                    foreach (var fileName in willDownloadList)
                    {
                        downloaDTask[nowFileIndex - 1] = await downloadTaskFactory.StartNew(async nowFileName =>
                        {
                            nowFileIndex++;
                            await downloadTaskFactory.StartNewDelayed((nowFileIndex % 25 == 0)
                                ? 500
                                : Define.DownloadTaskSleep);
                            await DownloadFiles(nowFileName.ToString(), nowFileIndex, totalCount, AUTO_DECRYPT.Auto);
                        }, fileName, cancelToken);
                    }
                }

                cancelToken.ThrowIfCancellationRequested();

                //cannot wait in the main thread.
                //Task.WaitAll(downloaDTask);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void OnDownloadDone( int totalCount)
        {
            if (_downloadDoneList.Count != totalCount) return;
            GC.Collect();
            Thread.Sleep(200);
            SetNoticesText("下载完成 >> 共 " + totalCount + " 个文件 ! !", downloadNotice);
            SetButtomVisibled(false,button_DownloadCancel);
        }


        /// <summary>
        /// Main Download method
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileNameIndex"></param>
        /// <param name="totalCount"></param>
        /// <param name="AUTO_DECRYPT"></param>
        /// <returns></returns>
        private async Task DownloadFiles(string fileName, int fileNameIndex, int totalCount, AUTO_DECRYPT AUTO_DECRYPT )
        {
            SetNoticesText("正在下载 ... " + _downloadDoneList.Count + " / " + totalCount, downloadNotice);
            if (IsSeveralFiles)
            {
                _downloadProcessMessageHandler.HttpReceiveProgress += (senders, es) =>
                {
                    var num = es.ProgressPercentage;
                    SetProgressInt(num);
                };
            }
            _downloadDoneList.Add(await _request.MakeGetRequest(Define.GetUrl(fileName), Define.GetFileSavePath(), fileName));
            SetNoticesText("正在下载 ... " + _downloadDoneList.Count + " / " + totalCount, downloadNotice);
            
            if (!IsSeveralFiles)
            {
                ReloadProcess(totalCount);
            }
            
            if (AUTO_DECRYPT == AUTO_DECRYPT.Auto)
            {
                if (Save.GetFileType(fileName) != ENC_TYPE.ERROR)
                {
                    DecryptFiles.DecryptFile(Define.GetFileSavePath() + fileName);
                }
            }

            OnDownloadDone(totalCount);

        }

        /// <summary>
        /// Main post method
        /// </summary>
        /// <param name="index"></param>
        private async Task<UPDATE_STATUS> StartPost(bool index = false , bool update = false)
        {
            SetNoticesText("正在获取新版本数据 ...请稍等..." , downloadNotice);
            JsonParse jsonParse = new JsonParse();
            _postProcessMessageHandler.HttpSendProgress += (senders, es) =>
            {
                int num = es.ProgressPercentage;
                SetProgressInt(num);
            };
            if (!index)
            {
                return await jsonParse.SaveDlConfing(
                _request.MakePostRequest(Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)), true);
            }
            else
            {
                if (update)
                {
                    jsonParse.UpdateUrlIndex(Define.jsonParse,
                        _request.MakePostRequest(Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)), true);
                }
                else
                {
                    jsonParse.SaveUrlIndex(
                    _request.MakePostRequest(Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)), true);
                }

                return UPDATE_STATUS.Ok;
            }
        }

        /// <summary>
        /// Show the list into listbox.
        /// </summary>
        /// <param name="nameList"></param>
        /// <param name="showCount"></param>
        private void ShowlistResult( string[] nameList , int showCount )
        {
            Task.Run(() =>
            {
                for (int i = 0; i < showCount; i++)
                {
                    SetlistResult(nameList[i]);
                }
            });
        }

        /// <summary>
        /// Show the list into listbox.
        /// </summary>
        /// <param name="nameList"></param>
        /// <param name="startIndex"></param>
        /// <param name="showCount"></param>
        private void ShowlistResult(string[] nameList, int startIndex, int showCount)
        {
            Task.Run(() =>
            {
                for (int i = startIndex ; i < showCount; i++)
                {
                    SetlistResult(nameList[i]);
                }
            });
        }

        private void Button_DownloadCancel_Click(object sender, EventArgs e)
        {
            CancelSource.Cancel();
        }


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
