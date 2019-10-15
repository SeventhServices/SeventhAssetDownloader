using System;
using System.Windows.Forms;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace T7s_Asset_Downloader.Asset
{
    internal static class GetVersion
    {
        private const string Url = "http://1app.pw/app/history/8";

        private static HtmlDocument _htmlDocument;
        private static string SelectHtmlToString(string selectString, string param = " ", bool attributes = false)
        {

            var NewVersionDivSelectString = "//body/div[2]/div[last()]/div[1]";

            if (attributes)
                return _htmlDocument.DocumentNode.SelectSingleNode(
                    NewVersionDivSelectString +
                    selectString).Attributes[param].Value;

            return _htmlDocument.DocumentNode.SelectSingleNode(
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
                var web = new HtmlWeb();
                _htmlDocument = web.Load(Url);
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
                MessageBox.Show($@"检测游戏版本失败! : {e.Message}");
                return new GameVersion
                {
                    Version = Define.Ver,
                    VersionCode = Define.Blt,
                };
            }
        }

        public class GameVersion
        {
            public string Version { get; set; }
            public string VersionCode { get; set; }
            public string DownloadPath { get; set; }
        }
    }
}