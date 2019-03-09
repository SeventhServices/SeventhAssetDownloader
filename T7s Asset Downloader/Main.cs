using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using T7s_Enc_Decoder;
using T7s_Sig_Counter;
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
                Define.NOW_STAUTUS = NOW_STAUTUS.NoneIndex;
                Define.NOW_STAUTUS = NOW_STAUTUS.First;
            }
            ReloadNoticeLabels();
            MakeRequest Request = new MakeRequest();
            Request.HttpClientTest(ProcessMessageHander);
        }
        private ProgressMessageHandler ProcessMessageHander = new ProgressMessageHandler(new HttpClientHandler());
        private delegate void SetNotices( string notices , Label label );
        private delegate void SetProgress( int progress );
        private delegate void SetCallBack( object obj );
        private delegate void SetEnabled( bool enabled, Button button);
        private string[] ListResult;
        private string[] ErrorList;
        
        private bool isSevealFiles = true;
        private MakeRequest Request = new MakeRequest();
        
        private List<string> DownloadDoneList = new  List<string>();

        private void Button_DownloadAllFiles_Click(object sender, EventArgs e)
        {
            DownloadDoneList.Clear();
            if (ListResult.Length > 200)
            {
                if (MessageBox.Show($"请注意，所选文件量为{ListResult.Length}个" + "下载将会花费较长时间。", "Notices", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)
                    == DialogResult.Cancel)
                {
                    return;
                }
            };
            StartDownload(DOWNLOAD_TYPE.AllFiles);
        }

        private void Button_DownloadCheckFiles_Click(object sender, EventArgs e)
        {
            DownloadDoneList.Clear();
            StartDownload(DOWNLOAD_TYPE.SeletFiles);
        }
        private async void StartDownload ( DOWNLOAD_TYPE DOWNLOAD_TYPE )
        {
            if (!Directory.Exists(Define.GetFileSavePath()))
            {
                Directory.CreateDirectory(Define.GetFileSavePath());
            }
            SetProgressInt(0);

            string[] WillDownloadList;
            int TotalCount;

            switch (DOWNLOAD_TYPE)
            {
                case DOWNLOAD_TYPE.AllFiles:
                    TotalCount = ListResult.Length;
                    WillDownloadList = ListResult;
                    break;
                case DOWNLOAD_TYPE.SeletFiles:
                    TotalCount = listBoxResult.SelectedItems.Count;
                    if (TotalCount < 1)
                    {
                        MessageBox.Show("未选择要下载的文件，请点击选择某项或多项文件，再开始下载。", "Notice");
                        return;
                    }
                    var CheckedNameList = new string[TotalCount];
                    for (int i = 0; i < TotalCount; i++)
                    {
                        CheckedNameList[i] = listBoxResult.SelectedItems[i].ToString();
                    }
                    WillDownloadList = CheckedNameList;
                    break;
                default:
                    TotalCount = ListResult.Length;
                    WillDownloadList = ListResult;
                    break;
            }

            int NowFileIndex = 0;
            try
            {
                var scheduler = new LimitedConcurrencyLevelTaskScheduler(Define.MaxDownloadTasks);
                TaskFactory DownloadTaskFactory = new TaskFactory(scheduler);
                SetNoticesText("正在下载 ... " + "共" + TotalCount + "个文件", downloadNotice);
                if (TotalCount <= 15)
                {
                    foreach (var fileName in WillDownloadList)
                    {
                        await DownloadTaskFactory.StartNew((Func<object, Task>)(async NowFileName =>
                        {
                            NowFileIndex++;
                            await DownloadFiles(fileName, NowFileIndex, TotalCount, AUTO_DECRYPT.Auto );
                        }), fileName);
                    }
                    await Task.Run(() =>
                    {
                        SetNoticesText("正在下载 ... " + DownloadDoneList.Count + " / " + TotalCount, downloadNotice);
                        while (!(DownloadDoneList.Count == TotalCount)) { };
                        Thread.Sleep(100);
                        GC.Collect();
                    });
                }
                else
                {
                    isSevealFiles = false;
                    Define.DownloadTaskSleep = (TotalCount < 200) ? 0 : TotalCount / 3;
                    foreach (var fileName in WillDownloadList)
                    {
                        await DownloadTaskFactory.StartNew((Func<object, Task>)(async NowFileName =>
                        {
                            NowFileIndex++;
                            await DownloadFiles(fileName, NowFileIndex, TotalCount, AUTO_DECRYPT.Auto);
                            Thread.Sleep((NowFileIndex % 50 == 0) ? 1000 : Define.DownloadTaskSleep);
                        }), fileName);
                    }
                    await Task.Run(() =>
                    {
                        SetNoticesText("正在下载 ... " + DownloadDoneList.Count + " / " + TotalCount, downloadNotice);
                        while (!(DownloadDoneList.Count == TotalCount)) { };
                        Thread.Sleep(100);
                        GC.Collect();
                    });
                }
            }
            finally
            {
                SetNoticesText("下载完成 >> 共 " + TotalCount + " 个文件 ! !", downloadNotice);
                var ErrorList = WillDownloadList.Except(DownloadDoneList.ToArray());
            }

}
        private async Task DownloadFiles(string fileName, int fileNameIndex, int totalCount, AUTO_DECRYPT AUTO_DECRYPT )
        {
            SetNoticesText("正在下载 ... " + DownloadDoneList.Count + " / " + totalCount, downloadNotice);
            if (isSevealFiles)
            {
                ProcessMessageHander.HttpReceiveProgress += (senders, es) =>
                {
                    int num = es.ProgressPercentage;
                    SetProgressInt(num);
                };
            }
            DownloadDoneList.Add(await Request.MkaeGetRequest(Define.GetUrl(fileName), Define.GetFileSavePath(), fileName));
            ReloadProcess(totalCount);
            if (AUTO_DECRYPT == AUTO_DECRYPT.Auto)
            {
                if (Save.GetFileType(fileName) != ENC_TYPE.ERROR)
                {
                    DecryptFiles.DecryptFile(Define.GetFileSavePath() + fileName);
                }
            }
        }
        private void StartPost(bool index = false)
        {
            SetNoticesText("正在获取新版本数据 ...请稍等..." , downloadNotice);
            JsonParse jsonParse = new JsonParse();
            ProcessMessageHander.HttpReceiveProgress += (senders, es) =>
            {
                int num = es.ProgressPercentage;
                SetProgressInt(num);
            };
            if (!index)
            {
                jsonParse.SaveDLConfing(
                Request.MakePostRequest(Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)), true);
            }
            else
            { 
                jsonParse.SaveUrlIndex(
                Request.MakePostRequest(Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)), true);
            }

        }

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
        private void SetNoticesText ( string notice ,Label label)
        {
            if (label.InvokeRequired)
            {
                SetNotices call = new SetNotices(SetNoticesText);
                Invoke(call, new object[] { notice , label });
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
                SetEnabled call = new SetEnabled(SetButtomEnabled);
                Invoke(call, new object[] { enabled, button });
            }
            else
            {
                button.Enabled = enabled;
            }
        }
        private void SetProgressInt (int progress)
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


        public void _ini_listResult()
        {
            ListResult = Define.GetDefaultNameList();
            ShowlistResult(Define.GetDefaultNameList(), Define.DefaultShowCount);
        }

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

        private void ShowlistResult(string[] nameList, int startCount, int showCount)
        {
            Task.Run(() =>
            {
                for (int i = startCount ; i < showCount; i++)
                {
                    SetlistResult(nameList[i]);
                }
            });
        }
        public void AddListResult( Object item )
        {
            listBoxResult.Items.Add( item );
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
            if (ListResult == null)
            {
                ListResult = Define.GetDefaultNameList();
            }
            ShowlistResult(ListResult, Define.DefaultShowCount, ListResult.Length);
            button_LoadAllResult.Enabled = false;
        }

        private void TextBox_SeachFiles_TextChanged(object sender, EventArgs e)
        {
            listBoxResult.Items.Clear();
            ListResult = Define.GetListResult(textBox_SeachFiles.Text);
            if (!(ListResult.Length > Define.DefaultShowCount))
            {
                ShowlistResult(ListResult, ListResult.Length);
            }
            else
            {
                ShowlistResult(ListResult, Define.DefaultShowCount);
            }
            button_LoadAllResult.Enabled = !button_LoadAllResult.Enabled ? true : true;
        }

        private void ReloadNoticeLabels()
        {
            SetNoticesText((Define.NOW_STAUTUS != NOW_STAUTUS.First) ? "当前版本 : " + "r" + Define.NowRev : "当前版本 : " + ">> 请获取最新版本", label_NowRev);
        }

        private void Button_ShowAdvance_Click(object sender, EventArgs e)
        {
            Advance Advance = new Advance();
            Advance.Show();
        }

        private void Button_GetNew_Click(object sender, EventArgs e)
        {
            Define.isGetNewComplete = false;
            button_ReloadAdvance.Enabled = false;
            button_GetNew.Enabled = false;
            try
            {
                Define.Rev = Define.UserRev = (Define.NOW_STAUTUS == NOW_STAUTUS.First) ? (Convert.ToInt32(Define.NowRev) + 296).ToString() : (Convert.ToInt32(Define.NowRev) - 3).ToString();
                StartPost();

                Define.Rev = Define.UserRev = "001";
                StartPost(true);

            }
            finally
            {
                Task.Run( () =>
                {
                    while (Define.isGetNewComplete == false) { };
                    Define.NOW_STAUTUS = NOW_STAUTUS.Normal;
                    SetNoticesText(">> 就绪 ...", downloadNotice);
                    ReloadNoticeLabels();
                    SetButtomEnabled(true, button_GetNew);
                    SetButtomEnabled(true, button_ReloadAdvance);
                    _ini_listResult();
                });
            }

        }

        private void Button_ReloadAdvance(object sender, EventArgs e)
        {
            listBoxResult.Items.Clear();
            Define._ini_Coning();
            ReloadNoticeLabels();
            _ini_listResult();
        }


        private void ReloadProcess ( int TotalCount )
        {
            Task.Run(() =>
            {
                int NowProcess = (DownloadDoneList.Count / TotalCount) * 100;
                SetNoticesText("正在下载 ... " + DownloadDoneList.Count + " / " + TotalCount, downloadNotice);
                if (!isSevealFiles)
                {
                    SetProgressInt(NowProcess);
                };
            });
        }
           
    }
}
