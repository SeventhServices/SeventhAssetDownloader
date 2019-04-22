using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T7s_Enc_Decoder;

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
    class Datas : IGetUrl
    {
        public string Donin => "https://d2kvktrbzlzxwg.cloudfront.net/";

        public string GetUrl(string fileName)
        {
            string UrlPath = Define.JsonParse.FileUrls.Where(p => p.Name == fileName).Select(p => p.Url).ToArray()[0];
            return Define.DownloadPath + UrlPath;
        }
    }

    class GetCard : IGetData
    {
        private readonly MakeRequest _makeRequest;
        public GetCard( MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
            _makeRequest._ini_GetClient();
        }
        public string Donin => "https://api.t7s.jp/resource/images/card/l/";
        public string GetUrl(int id)
        {
            return Donin + "card_l_" + id.ToString("D5") + ".jpg.enc";
        }

        public string GetFileNme(int id)
        {
            return "card_m_" + id.ToString("D5") + ".jpg.enc";
        }

        public async void SaveFileAndDecrypt(int id, string savePath)
        {
            await _makeRequest.MakeSingleGetRequest(GetUrl(id),savePath,GetFileNme(id));
            DecryptFiles.DecryptFile(savePath + GetFileNme(id), EncVersion.Ver1);
        }

        public async void SaveFile(int id, string savePath)
        {
            await _makeRequest.MakeSingleGetRequest(GetUrl(id), savePath, GetFileNme(id));
        }
    }

    class GetMiddleCard : IGetData
    {
        private readonly MakeRequest _makeRequest;
        public GetMiddleCard(MakeRequest makeRequest)
        {
            _makeRequest = makeRequest;
            _makeRequest._ini_GetClient();
        }
        public string Donin => "https://api.t7s.jp/resource/images/card/l/";
        public string GetUrl(int id)
        {
            return Donin + Define.DownloadPath + "card_l_" + id.ToString("D5") + ".jpg.enc";
        }

        public string GetFileNme(int id)
        {
            return "card_m_" + id.ToString("D5") + ".jpg.enc";
        }

        public async void SaveFileAndDecrypt(int id, string savePath)
        {
            await _makeRequest.MakeSingleGetRequest(GetUrl(id), savePath, GetFileNme(id));
        }

        public async void SaveFile(int id, string savePath)
        {
            await _makeRequest.MakeSingleGetRequest(GetUrl(id), savePath, GetFileNme(id));
            DecryptFiles.DecryptFile(savePath + GetFileNme(id), EncVersion.Ver1);
        }
    }
}
