using System;
using System.Windows.Forms;
using T7s_Asset_Downloader.Extensions;
using T7s_Enc_Decoder;

namespace T7s_Asset_Downloader
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Define.LocalPath = Application.StartupPath;
            Crypt.IdentifyEncVersion("");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //var readEpisode = new ReadEpisodes();
            //readEpisode.StartRead();
            Application.Run(new Main());

        }
    }
}
