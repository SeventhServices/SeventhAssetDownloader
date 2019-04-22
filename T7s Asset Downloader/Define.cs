using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T7s_Asset_Downloader.Asset;
using UnityEngine.SocialPlatforms;

namespace T7s_Asset_Downloader
{
    public static class Define
    {
        public static JsonParse JsonParse = new JsonParse();

        public static string LocalPath { get; set; }
        public static string ApkDownloadPath { get; set; }

        public static string BaseUrl = "https://api.t7s.jp/";
        public static string Domin = "https://d2kvktrbzlzxwg.cloudfront.net/";
        public static string Id = "353b3932613b34642c346230672e36366165293e3a31312c3a64356060363030613f3764";
        public static string encPid = "36323335313138";
        public static string Ver = "6.2.2";
        public static string Rev = "310";
        public static string Blt = "139";
        public static string UserRev = "297";
        

        public static string DownloadPath;
        public static string NowRev = "1";
        public static string LastRev;
        public static string[] DiifList;

        public static NOW_STAUTUS NOW_STAUTUS;
        public static AUTO_DECRYPT AUTO_DECRYPT = AUTO_DECRYPT.Auto;
        public static int DefaultShowCount = 20;
        public static int MaxDownloadTasks = 5;
        public static int DownloadTaskSleep = 300;
        public static bool IsGetNewComplete;

        public static Task SetNewVersion()
        {
            return new Task(() =>
            {
                var newVersion = GetVersion.GetNewVersion();
                if (Convert.ToInt16(newVersion.VersionCode) <= Convert.ToInt16(Blt)) return;
                Ver = newVersion.Version;
                Blt = newVersion.VersionCode;
                ApkDownloadPath = newVersion.DownloadPath;
            });
        }



        public static void _ini_Coning()
        {
            DownloadPath = JsonParse.DownloadConfings.Select(p => p.DownloadPath).ToArray().Last();
            Rev = NowRev = JsonParse.DownloadConfings.Select(p => p.Revision).ToArray().Last();
        }

        public static string GetUrl ( string fileName )
        {
            string urlPath = JsonParse.FileUrls.Where(p => p.Name == fileName).Select(p => p.Url).ToArray()[0];
            return DownloadPath + urlPath;
        }

        public static string[] GetDefaultNameList()
        {
            return JsonParse.FileUrls.Select(t => t.Name).ToArray();
        }

        public static string[] GetListResult(string searchText)
        {
            return JsonParse.FileUrls.Where(p => p.Name.Contains(searchText)).Select(p => p.Name).ToArray();
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
        public static string GetUpdatePath()
        {
            return LocalPath + @"\Asset\Index\Temp\Update.json";
        }

        public static string GetTempPath()
        {
            return LocalPath + @"\Asset\Index\Temp\Temp.json";
        }

        public static string GetExtensionsSavePath()
        {
            return LocalPath + @"\Asset\Extensions\Download\";
        }

        public static string GetAdvanceConfingPath()
        {
            return LocalPath + @"\Asset\Index\Temp\Confing.json";
        }

        public static string GetFileDownloadUrl( string fileName )
        {
            return JsonParse.DownloadConfings.Select(p => p.DownloadPath).ToString();
        }

        public static string GetExtensionsPath()
        {
            return LocalPath + @"\Asset\Extensions\";
        }

        public static string GetExtensionsTempPath()
        {
            return LocalPath + @"\Asset\Extensions\Temp\Temp.json";
        }

        /// <summary>
        /// (临时)获取API完整名
        /// </summary>
        /// <param name="apiName">API类型</param>
        /// <returns></returns>
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
                case APINAME_TYPE.first:
                    return "setup/first";
                case APINAME_TYPE.comp:
                    return "setup/comp";
                case APINAME_TYPE.gacha_main:
                    return "gacha/main";
                case APINAME_TYPE.gacha_result:
                    return "gacha/result";
                case APINAME_TYPE.tutorial_chara:
                    return "tutorial/chara";
                case APINAME_TYPE.tutorial_name:
                    return "tutorial/name";
                case APINAME_TYPE.tutorial_type:
                    return "tutorial/type";
                case APINAME_TYPE.tutorial_end:
                    return "tutorial/end";
                case APINAME_TYPE.tutorial_questionnaire:
                    return "tutorial/questionnaire";
                case APINAME_TYPE.tutorial_beginner:
                    return "tutorial/beginner";

                case APINAME_TYPE.event_ranking_user:
                    return "event/ranking/user";

                case APINAME_TYPE.episode_main:
                    return "episode/main";
                case APINAME_TYPE.scenario_result:
                    return "scenario/result";

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
            inspection,
            first,
            comp,
            gacha_main,
            gacha_result,
            tutorial_chara,
            tutorial_name,
            tutorial_type,
            tutorial_end,
            tutorial_questionnaire,
            tutorial_beginner,

            event_ranking_user,

            episode_main,
            scenario_result
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
