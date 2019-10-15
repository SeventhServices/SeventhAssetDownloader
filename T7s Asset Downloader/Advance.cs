using System;
using System.Windows.Forms;
using T7s_Asset_Downloader.Asset;
using T7s_Asset_Downloader.Extensions;
using T7s_Asset_Downloader.Response;

namespace T7s_Asset_Downloader
{
    public partial class Advance : Form
    {
        public Advance()
        {
            InitializeComponent();
        }
        private readonly GetCard _getCard = new GetCard(new MakeRequest());
        private void Button_GetAllIndex_Click(object sender, EventArgs e)
        {
            Define.Rev = Define.UserRev = "001";
            new JsonParse().SaveUrlIndex(
                new MakeRequest().MakePostRequest(Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)));
        }

        private void Button_GetConfing_Click(object sender, EventArgs e)
        {
            Define.Rev = Define.UserRev = Define.NOW_STAUTUS == NOW_STAUTUS.First
                ? (Convert.ToInt32(Define.NowRev) + 296).ToString()
                : (Convert.ToInt32(Define.NowRev) - 3).ToString();
            new JsonParse().SaveDlConfing(
                new MakeRequest().MakePostRequest(Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)));
        }

        private void Button_LoadAllIndex_Click(object sender, EventArgs e)
        {
            Define.JsonParse.FileUrls.Clear();
            Define.JsonParse.LoadUrlIndex(Define.GetAdvanceIndexPath());
        }


        private void Button_LoadConfing_Click(object sender, EventArgs e)
        {
            Define.JsonParse.DownloadConfings.Clear();
            Define.JsonParse.LoadConfing(Define.GetAdvanceConfingPath());
            Define._ini_Coning();
        }

        private void Button_LoadToNewIndex_Click(object sender, EventArgs e)
        {
            Define.JsonParse.FileUrls.Clear();
            Define.JsonParse.LoadUrlIndex(Define.GetAdvanceIndexPath());
        }

        private void Button_GetToNewIndex_Click(object sender, EventArgs e)
        {
            Define.Rev = Define.UserRev = textBox_InputRev.Text;
            new JsonParse().SaveUrlIndex(
                new MakeRequest().MakePostRequest(Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)));
        }

        private void Advance_Load(object sender, EventArgs e)
        {
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                var cardIdFrom =int.Parse(TextBox_CardFrom.Text);
                var cardIdTo = int.Parse(TextBox_CardTo.Text);
                for (var cardId = cardIdFrom; cardId < cardIdTo; cardId++)
                {
                    _getCard.SaveFileAndDecrypt(cardId, Define.GetExtensionsSavePath());
                }

                MessageBox.Show(@"下载完成!");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }


        }

        private void Button_FormatScout_Click(object sender, EventArgs e)
        {
            var formatScout = new FormatScout();
            if (CheckBox_IsLink.Checked)
            {
                formatScout.Format(true);
            }
            else
            {
                formatScout.Format(false);
            }
        }
    }

    partial class Main
    {
        private void Button3_Click(object sender, EventArgs e)
        {
            listBoxResult.Items.Clear();
            ShowlistResult(Define.DiifList, Define.DiifList.Length);
        }
    }
}