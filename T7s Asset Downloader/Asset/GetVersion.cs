using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace T7s_Asset_Downloader.Asset
{
    internal static class GetVersion
    {
        public class GameVersion
        {
            public string Version { get; set; }
            public string VersionCode { get; set; }
            public string DownloadPath { get; set; }
        };

        private const string Url = "http://1app.pw/app/history/8";

        private static string SelectHtmlToString(string selectString ,string param = " ", bool attributes = false)
        {
            var web = new HtmlWeb();
            var htmlDoc = web.Load(Url);
            var NewVersionDivSelectString = "//body/div[2]/div[last()]/div[1]";

            if (attributes)
            {
                return htmlDoc.DocumentNode.SelectSingleNode(
                    NewVersionDivSelectString +
                    selectString).Attributes[param].Value;
            }

            return  htmlDoc.DocumentNode.SelectSingleNode(
                    NewVersionDivSelectString +
                    selectString).InnerText
                .Replace("\t", "")
                .Replace(" ", "")
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace(param, "");

        }

        public static GameVersion GetNewVersion()
        {
            try
            {
                var tempVersion = SelectHtmlToString("/div[1]/div[1]/a/span");
                return new GameVersion
                {
                    Version = tempVersion,
                    VersionCode = SelectHtmlToString("/div[1]/div[1]/a", tempVersion).Substring(1),
                    DownloadPath = Url + SelectHtmlToString("/div[2]/div/a", "href", true)
                };
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }



    }
}
