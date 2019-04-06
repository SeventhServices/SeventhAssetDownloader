using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T7s_Asset_Downloader.Extensions
{
    class Default : IGetUrl
    {
        private readonly string _domin = "https://d2kvktrbzlzxwg.cloudfront.net/";

        public string GetUrl(string fileName)
        {
            string UrlPath = Define.JsonParse.FileUrls.Where(p => p.Name == fileName).Select(p => p.Url).ToArray()[0];
            return Define.DownloadPath + UrlPath;
        }
    }
}
