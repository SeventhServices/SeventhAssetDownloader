using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T7s_Asset_Downloader
{
    public static class Define
    {
        public static JsonParse jsonParse = new JsonParse();

        public static string LocalPath;
        public static string BaseUrl = "https://api.t7s.jp/";
        public static string Domin = "https://d2kvktrbzlzxwg.cloudfront.net/";
        public static string Id = "353b3932613b34642c346230672e36366165293e3a31312c3a64356060363030613f3764";
        public static string encPid = "36323335313138";
        public static string Ver = "6.0.0";
        public static string Rev = "293";
        public static string Blt = "131";
        public static string UserRev = "293";

        public static string DownloadPath;
        public static string NowRev;
        public static List<string> TEST = new List<string>();

        public static NOW_STAUTUS NOW_STAUTUS;
        public static AUTO_DECRYPT AUTO_DECRYPT = AUTO_DECRYPT.Auto;
        public static int DefaultShowCount = 20;
        public static int MaxDownloadTasks = 5;
        public static int DownloadTaskSleep = 300;

        public static string[] DiifList;

        public static bool isGetNewComplete;
        public static void _ini_Coning()
        {
            DownloadPath = jsonParse.DownloadConfings.Select(p => p.DownloadPath).ToArray()[0];
            NowRev = jsonParse.DownloadConfings.Select(p => p.Revision).ToArray()[0];
        }

        public static string GetUrl ( string fileName )
        {
            string UrlPath = jsonParse.FileUrls.Where(p => p.Name == fileName).Select(p => p.Url).ToArray()[0];
            return DownloadPath + UrlPath;
        }

        public static string[] GetDefaultNameList()
        {
            return jsonParse.FileUrls.Select(t => t.Name).ToArray();
        }

        public static string[] GetListResult(string SearchText)
        {
            return jsonParse.FileUrls.Where(p => p.Name.Contains(SearchText)).Select(p => p.Name).ToArray();
        }

        public static string GetFileSavePath()
        {
            return LocalPath + @"\Asset\Download\" + NowRev + @"\";
        }

        public static string GetIndexPath()
        {
            return LocalPath + @"\Asset\Index\Index.json";
        }

        public static string GetConfingPath()
        {
            return LocalPath + @"\Asset\Index\Confing.json";
        }

        public static string GetAdvanceIndexPath()
        {
            return LocalPath + @"\Asset\Index\Temp\Index.json";
        }

        public static string GetAdvanceConfingPath()
        {
            return LocalPath + @"\Asset\Index\Temp\Confing.json";
        }

        public static string GetFileDownloadUrl( string fileName )
        {
            return jsonParse.DownloadConfings.Select(p => p.DownloadPath).ToString();
        }


        public static string GetApiName (APINAME_TYPE apiName)
        {
            switch (apiName)
            {
                case APINAME_TYPE.login:
                    return "login";
                case APINAME_TYPE.result:
                    return "setup/resource/result";
                case APINAME_TYPE.inspection:
                    return "inspection";
                default:
                    return "inspection";
            }
        }

        /// <summary>
        /// 定义apiName
        /// </summary>
        public enum APINAME_TYPE
        {
            /// <summary>
            /// Login
            /// </summary>
            login,
            /// <summary>
            /// setup/resource/result
            /// </summary>
            result,
            /// <summary>
            /// setup/resource/result
            /// </summary>
            inspection
        }
    }

    /// <summary>
    /// 配置自动解密
    /// </summary>
    public enum AUTO_DECRYPT
    {
        /// <summary>
        /// Auto
        /// </summary>
        Auto,
        /// <summary>
        /// None
        /// </summary>
        None
    }

    /// <summary>
    /// 当前程序状态
    /// </summary>
    public enum NOW_STAUTUS
    {
        /// <summary>
        /// Normal
        /// </summary>
        Normal,
        /// <summary>
        /// First Open
        /// </summary>
        First,
        /// <summary>
        /// None Index.json
        /// </summary>
        NoneIndex,
        /// <summary>
        /// None Coning.json
        /// </summary>
        NoneConing
    }

    /// <summary>
    /// 下载模式
    /// </summary>
    public enum DOWNLOAD_TYPE
    {
        /// <summary>
        /// All Files
        /// </summary>
        AllFiles,
        /// <summary>
        /// Selet Files
        /// </summary>
        SeletFiles,
    }
}
