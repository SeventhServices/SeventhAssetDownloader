using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace T7s_Asset_Downloader
{
    public partial class Advance : Form 
    {
        public Advance()
        {
            InitializeComponent();
        }

        private void Button_GetAllIndex_Click(object sender, EventArgs e)
        {
            Define.Rev = Define.UserRev = "001";
            new JsonParse().SaveUrlIndex(
                new MakeRequest().MakePostRequest(Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)));
        }

        private void Button_GetConfing_Click(object sender, EventArgs e)
        {
            Define.Rev = Define.UserRev = (Define.NOW_STAUTUS == NOW_STAUTUS.First) ? (Convert.ToInt32(Define.NowRev) + 296).ToString() : (Convert.ToInt32(Define.NowRev) - 3).ToString();
            new JsonParse().SaveDlConfing(
                new MakeRequest().MakePostRequest(Define.Id, Define.GetApiName(Define.APINAME_TYPE.result)));
        }

        private void Button_LoadAllIndex_Click(object sender, EventArgs e)
        {
            Define.jsonParse.FileUrls.Clear();
            Define.jsonParse.LoadUrlIndex(Define.GetAdvanceIndexPath());
        }


        private void Button_LoadConfing_Click(object sender, EventArgs e)
        {
            Define.jsonParse.DownloadConfings.Clear();
            Define.jsonParse.LoadConfing(Define.GetAdvanceConfingPath());
            Define._ini_Coning();

        }

        private void Button_LoadToNewIndex_Click(object sender, EventArgs e)
        {
            Define.jsonParse.FileUrls.Clear();
            Define.jsonParse.LoadUrlIndex(Define.GetAdvanceIndexPath());
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

    }

    partial class Main :Form
    {
        private void Button3_Click(object sender, EventArgs e)
        {
            listBoxResult.Items.Clear();
            ShowlistResult(Define.DiifList, Define.DiifList.Length);
        }




    }
}
