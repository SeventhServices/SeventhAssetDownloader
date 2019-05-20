using System.Linq;
using T7s_Asset_Downloader.Crypt;
using T7s_Asset_Downloader.Response;

namespace T7s_Asset_Downloader.Extensions
{
    internal interface IGetUrl
    {
        string Donin { get; }
        string GetUrl(string fileName);
    }

    internal interface IGetData
    {
        string Donin { get; }
        string GetUrl(int id);
        string GetFileNme(int id);

        void SaveFile(int id, string savePath);
    }

    internal class Datas : IGetUrl
    {
        public string Donin => "https://d2kvktrbzlzxwg.cloudfront.net/";

        public string GetUrl(string fileName)
        {
            var UrlPath = Define.JsonParse.FileUrls.Where(p => p.Name == fileName).Select(p => p.Url).ToArray()[0];
            return Define.DownloadPath + UrlPath;
        }
    }

    internal class GetCard : IGetData
    {
        private readonly MakeRequest _makeRequest;

        public GetCard(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
            _makeRequest._ini_GetClient();
        }

        public string Donin => "https://api.t7s.jp/resource/images/card/l/";

        public string GetUrl(int id)
        {
            return Donin + GetFileNme(id);
        }

        public string GetFileNme(int id)
        {
            return Crypt.Crypt.ConvertFileName("card_l_" + id.ToString("D5") + ".jpg.enc", EncVersion.Ver1,
                EncVersion.Ver2);
        }

        public async void SaveFile(int id, string savePath)
        {
            await _makeRequest.MakeSingleGetRequest(GetUrl(id), savePath, GetFileNme(id));
        }

        public async void SaveFileAndDecrypt(int id, string savePath)
        {
            if (await _makeRequest.MakeSingleGetRequest(GetUrl(id), savePath, GetFileNme(id)) != null)
                DecryptFiles.DecryptFile(savePath + GetFileNme(id), Crypt.EncVersion.Ver2);
        }
    }

    internal class GetMiddleCard : IGetData
    {
        private readonly MakeRequest _makeRequest;

        public GetMiddleCard(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
            _makeRequest._ini_GetClient();
        }

        public string Donin => "https://d2kvktrbzlzxwg.cloudfront.net/revision/";
        
        public string GetUrl(int id)
        {
            return Donin + Define.DownloadPath + GetFileNme(id);
        }

        public string GetFileNme(int id)
        {
            return Crypt.Crypt.ConvertFileName("card_m_" + id.ToString("D5") + ".jpg.enc", EncVersion.Ver1,
                EncVersion.Ver2);
        }

        public async void SaveFile(int id, string savePath)
        {
            if (await _makeRequest.MakeSingleGetRequest(GetUrl(id), savePath, GetFileNme(id)) != null)
                DecryptFiles.DecryptFile(savePath + GetFileNme(id), Crypt.EncVersion.Ver2);
        }

        public async void SaveFileAndDecrypt(int id, string savePath)
        {
            await _makeRequest.MakeSingleGetRequest(GetUrl(id), savePath, GetFileNme(id));
        }
    }
}