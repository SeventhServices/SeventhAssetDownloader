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

namespace T7s_Asset_Downloader
{
    public partial class Main_Test : Form
    {
        public Main_Test()
        {
            InitializeComponent();

            //StreamReader JsonStream =  MakeRequest.MakePostRequest(Define.BaseUrl + Define.GetApiName(Define.APINAME_TYPE.result), Define.Id, Define.GetApiName(Define.APINAME_TYPE.result));
            //new JsonParse().SaveDLConfing(@"C:\Users\Sagilio\Desktop\18.json");
            //new JsonParse().LoadUrlIndex(@"C:\Users\Sagilio\source\repos\NanasGadgets\T7s Asset Downloader\bin\Debug\Asset\Index\Index.json");
            //new MakeRequest().MkaeGetRequest();
            if (File.Exists(Define.GetIndexPath()))
            {
                Define.jsonParse.LoadUrlIndex( Define.GetIndexPath() );
            }
            if (File.Exists(Define.GetConfingPath()))
            {
                Define.jsonParse.LoadConfing(Define.GetConfingPath());
            }
            
            
        }

        private void Button_GetAllIndex_Click(object sender, EventArgs e)
        {
            Define.Rev = "001";
            Define.UserRev = "001";
            var a = new MakeRequest().RawMakePostRequest(Define.BaseUrl + Define.GetApiName(Define.APINAME_TYPE.result), Define.Id, Define.GetApiName(Define.APINAME_TYPE.result));
            new JsonParse().SaveUrlIndex(a);
        }

        private void Button_LoadAllIndex_Click(object sender, EventArgs e)
        {

        }
    }
}
