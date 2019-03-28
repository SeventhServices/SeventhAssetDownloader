using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T7s_Enc_Decoder;
using System.Threading;

namespace T7s_Asset_Downloader
{
    public class JsonParse
    {
        public class FileUrl
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public URL_TYPE URL_TYPE { get; set; }
        }

        public class DownloadConfing
        {
            public string Revision { get; set; }
            public string DownloadDomain { get; set; }
            public string DownloadPath { get; set; }
            public string NewDownloadSize { get; set; }
            public string OneByOneDownloadPath { get; set; }
            public string SubDomain { get; set; }
            public string ImageRev { get; set; }
            public string ImageDomain { get; set; }
            public string ImagePath { get; set; }
        }

        public List<FileUrl> FileUrls = new List<FileUrl>();
        public List<DownloadConfing> DownloadConfings = new List<DownloadConfing>();

        private void OnGetComplete()
        {
            System.Windows.Forms.MessageBox.Show("获取完成", "Notice", System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Information, System.Windows.Forms.MessageBoxDefaultButton.Button1);
        }

        public void AddFileUrl(string name, string url, URL_TYPE uRL_TYPE)
        {
            FileUrls.Add(new FileUrl
            {
                Name = name,
                Url = url,
                URL_TYPE = uRL_TYPE
            });
        }

        #region

        public void SaveDlConfing(string path)
        {
            JObject jObject = ParseResultJson(path);

            string baseFileUrl = GetModify(path)[1].ToString();
            string[] tempFileUrl = baseFileUrl.Split('/');

            StringBuilder downloadPath = new StringBuilder();
            for (int j = 0; j < 2; j++)
            {
                downloadPath.Append(tempFileUrl[j]);
                downloadPath.Append("/");
            }

            DownloadConfings.Add(new DownloadConfing
            {
                Revision = jObject["updateResource"]["revision"].ToString(),
                DownloadDomain = jObject["updateResource"]["downloadConfig"]["domain"].ToString(),
                DownloadPath = downloadPath.ToString(),
                NewDownloadSize = jObject["updateResource"]["downloadSize"].ToString(),
                OneByOneDownloadPath = jObject["updateResource"]["downloadConfig"]["oneByOneDownloadPath"].ToString(),
                SubDomain = jObject["updateResource"]["downloadConfig"]["subDomain"].ToString(),
                ImageRev = jObject["updateResource"]["imageRev"].ToString(),
                ImagePath = jObject["updateResource"]["imagePath"].ToString()
            });
            string downloadConfing = JsonConvert.SerializeObject(DownloadConfings);
            using (StreamWriter streamWriter =
                new StreamWriter(Define.LocalPath + @"\Asset\Index\" + "DownloadConfing.json"))
            {
                //byte[] FileBytes = Crypt.Encrypt<Byte[]>(Encoding.UTF8.GetBytes(UrlIndex) ,true);
                //string FileText = Encoding.UTF8.GetString(FileBytes);
                streamWriter.Write(downloadConfing);
                streamWriter.Close();
            }
        }

        public void SaveUrlIndex(string path)
        {
            if (!Directory.Exists(Define.LocalPath + @"\Asset\Index"))
            {
                Directory.CreateDirectory(Define.LocalPath + @"\Asset\Index");
            }

            ParseModify(path);
            ParseModify(path, true);
            var urlIndex =
                JsonConvert.SerializeObject(FileUrls.OrderBy((FileUrl e) => e.Name, StringComparer.Ordinal));
            using (StreamWriter streamWriter = new StreamWriter(Define.LocalPath + @"\Asset\Index\" + "Index.json"))
            {
                //byte[] FileBytes = Crypt.Encrypt<Byte[]>(Encoding.UTF8.GetBytes(UrlIndex) ,true);
                //string FileText = Encoding.UTF8.GetString(FileBytes);
                streamWriter.Write(urlIndex);
                streamWriter.Close();
            }
        }

        public async void SaveDlConfing(Task<string> data)
        {
            if (!Directory.Exists(Define.LocalPath + @"\Asset\Index\Temp"))
            {
                Directory.CreateDirectory(Define.LocalPath + @"\Asset\Index\Temp");
            }

            JObject jObject = await ParseResultJson(data);

            string baseFileUrl = (await GetModify(data))[1].ToString();
            string[] tempFileUrl = baseFileUrl.Split('/');

            StringBuilder downloadPath = new StringBuilder();
            for (int j = 0; j < 2; j++)
            {
                downloadPath.Append(tempFileUrl[j]);
                downloadPath.Append("/");
            }

            DownloadConfings.Add(new DownloadConfing
            {
                Revision = jObject["updateResource"]["revision"].ToString(),
                DownloadDomain = jObject["updateResource"]["downloadConfig"]["domain"].ToString(),
                DownloadPath = downloadPath.ToString(),
                NewDownloadSize = jObject["updateResource"]["downloadSize"].ToString(),
                OneByOneDownloadPath = jObject["updateResource"]["downloadConfig"]["oneByOneDownloadPath"].ToString(),
                SubDomain = jObject["updateResource"]["downloadConfig"]["subDomain"].ToString(),
                ImageRev = jObject["updateResource"]["imageRev"].ToString(),
                ImagePath = jObject["updateResource"]["imagePath"].ToString()
            });
            string downloadConfing = JsonConvert.SerializeObject(DownloadConfings);
            using (StreamWriter streamWriter = new StreamWriter(Define.GetAdvanceConfingPath()))
            {
                streamWriter.Write(downloadConfing);
                streamWriter.Close();
            }

            DownloadConfings.Clear();
            OnGetComplete();
        }

        public async void SaveUrlIndex(Task<string> data)
        {
            if (!Directory.Exists(Define.LocalPath + @"\Asset\Index\Temp"))
            {
                Directory.CreateDirectory(Define.LocalPath + @"\Asset\Index\Temp");
            }

            await ParseModify(data);
            await ParseModify(data, true);
            string urlIndex =
                JsonConvert.SerializeObject(FileUrls.OrderBy((FileUrl e) => e.Name, StringComparer.Ordinal));
            using (StreamWriter streamWriter = new StreamWriter(Define.GetAdvanceIndexPath()))
            {
                streamWriter.Write(urlIndex);
                streamWriter.Close();
            }

            FileUrls.Clear();
            OnGetComplete();
        }

        public async void SaveDlConfing(Task<string> data, bool encrypt)
        {
            if (!Directory.Exists(Define.LocalPath + @"\Asset\Index\Temp"))
            {
                Directory.CreateDirectory(Define.LocalPath + @"\Asset\Index\Temp");
            }

            JObject jObject = await ParseResultJson(data);

            string baseFileUrl = (await GetModify(data))[1].ToString();
            string[] tempFileUrl = baseFileUrl.Split('/');

            StringBuilder downloadPath = new StringBuilder();
            for (int j = 0; j < 2; j++)
            {
                downloadPath.Append(tempFileUrl[j]);
                downloadPath.Append("/");
            }

            DownloadConfings.Add(new DownloadConfing
            {
                Revision = jObject["updateResource"]["revision"].ToString(),
                DownloadDomain = jObject["updateResource"]["downloadConfig"]["domain"].ToString(),
                DownloadPath = downloadPath.ToString(),
                NewDownloadSize = jObject["updateResource"]["downloadSize"].ToString(),
                OneByOneDownloadPath = jObject["updateResource"]["downloadConfig"]["oneByOneDownloadPath"].ToString(),
                SubDomain = jObject["updateResource"]["downloadConfig"]["subDomain"].ToString(),
                ImageRev = jObject["updateResource"]["imageRev"].ToString(),
                ImagePath = jObject["updateResource"]["imagePath"].ToString()
            });
            string downloadConfing = JsonConvert.SerializeObject(DownloadConfings);
            using (FileStream fileStream = File.OpenWrite(Define.LocalPath + @"\Asset\Index\Temp" + @"\Confing.json"))
            {
                byte[] fileBytes = Crypt.Encrypt<Byte[]>(Encoding.UTF8.GetBytes(downloadConfing), true, true);
                fileStream.Write(fileBytes, 0, fileBytes.Length);
                fileStream.Close();
            }

            Define.jsonParse.DownloadConfings.Clear();
            Define.jsonParse.LoadConfing(Define.LocalPath + @"\Asset\Index\Temp" + @"\Confing.json", encrypt);
            Define._ini_Coning();
            DownloadConfings.Clear();
            Directory.CreateDirectory(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev);
            File.Copy(Define.LocalPath + @"\Asset\Index\Temp" + @"\Confing.json",
                Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev + @"\Confing.json", true);
            File.Copy(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev + @"\Confing.json",
                Define.GetConfingPath(), true);
        }

        public async void SaveUrlIndex(Task<string> data, bool encrypt)
        {
            if (!Directory.Exists(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev))
            {
                Directory.CreateDirectory(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev);
            }

            await ParseModify(data);
            await ParseModify(data, true);
            string urlIndex =
                JsonConvert.SerializeObject(FileUrls.OrderBy((FileUrl e) => e.Name, StringComparer.Ordinal));
            using (FileStream fileStream =
                File.OpenWrite(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev + @"\Index.json"))
            {
                byte[] fileBytes = Crypt.Encrypt<Byte[]>(Encoding.UTF8.GetBytes(urlIndex), false, true);
                fileStream.Write(fileBytes, 0, fileBytes.Length);
                fileStream.Close();
            }

            FileUrls.Clear();
            Define.jsonParse.FileUrls.Clear();
            File.Copy(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev + @"\Index.json",
                Define.GetIndexPath(), true);
            Define.jsonParse.LoadUrlIndex(Define.GetIndexPath(), encrypt);
            Define.isGetNewComplete = true;
            OnGetComplete();
        }

        #endregion

        public void LoadUrlIndex(string indexPath)
        {
            using (var file = System.IO.File.OpenText(indexPath))
            {
                FileUrls = DeserializeJsonToList(file.ReadToEnd());
            }
        }

        public void LoadUrlIndex(string indexPath, bool encrypt)
        {
            byte[] fileBytes = Crypt.Decrypt<Byte[]>(System.IO.File.ReadAllBytes(indexPath), false, true);
            string fileText = Encoding.UTF8.GetString(fileBytes);
            FileUrls = DeserializeJsonToList(fileText);
        }

        public void LoadConfing(string confingPath)
        {
            using (System.IO.StreamReader file = System.IO.File.OpenText(confingPath))
            {
                DownloadConfings = DeserializeJsonToList(file.ReadToEnd(), true);
            }
        }

        public void LoadConfing(string confingPath, bool encrypt)
        {
            byte[] fileBytes = Crypt.Decrypt<Byte[]>(System.IO.File.ReadAllBytes(confingPath), true, true);
            string fileText = Encoding.UTF8.GetString(fileBytes);
            DownloadConfings = DeserializeJsonToList(fileText, true);
        }

        public static List<DownloadConfing> DeserializeJsonToList(string json, bool isConfing)
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<DownloadConfing>));
            List<DownloadConfing> list = o as List<DownloadConfing>;
            return list;
        }

        public static List<FileUrl> DeserializeJsonToList(string json)
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<FileUrl>));
            List<FileUrl> list = o as List<FileUrl>;
            return list;
        }

        public void ParseModify(string path, bool oneByOneModify = false)
        {
            int urlNum = GetModify(path, oneByOneModify).Count - 1;
            JArray urlList = GetModify(path, oneByOneModify);
            for (int i = 0; i < urlNum; i++)
            {
                string baseFileUrl = urlList[i].ToString();
                string[] tempFileUrl = baseFileUrl.Split('/');

                StringBuilder fileUrl = new StringBuilder();
                for (int j = 2; j < tempFileUrl.Length; j++)
                {
                    fileUrl.Append(tempFileUrl[j]);
                    if (j != tempFileUrl.Length)
                    {
                        fileUrl.Append("/");
                    }
                }

                if (!oneByOneModify)
                {
                    AddFileUrl(tempFileUrl.Last(), fileUrl.ToString(), URL_TYPE.Modify);
                }
                else
                {
                    AddFileUrl(tempFileUrl.Last(), fileUrl.ToString(), URL_TYPE.oneByOneModify);
                }
            }
        }

        /// <summary>
        /// 解析ResultJson
        /// </summary>
        public JObject ParseResultJson(string path)
        {
            string jsonfile = path;

            using (System.IO.StreamReader file = System.IO.File.OpenText(jsonfile))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    return (JObject) JToken.ReadFrom(reader);
                }
            }
        }

        /// <summary>
        /// 读取Url位置
        /// </summary>
        public JArray GetModify(string path, bool oneByOneModify = false)
        {
            if (!oneByOneModify)
            {
                return (JArray) ParseResultJson(path)["updateResource"]["modifyList"];
            }
            else
            {
                return (JArray) ParseResultJson(path)["updateResource"]["oneByOneModifyList"];
            }
        }


        public async Task ParseModify(Task<string> data, bool oneByOneModify = false)
        {
            JArray urlList = await GetModify(data, oneByOneModify);

            for (var i = 0; i < urlList.Count - 1; i++)
            {
                var baseFileUrl = urlList[i].ToString();
                var tempFileUrl = baseFileUrl.Split('/');

                StringBuilder fileUrl = new StringBuilder();
                for (int j = 2; j < tempFileUrl.Length; j++)
                {
                    fileUrl.Append(tempFileUrl[j]);
                    if (j != tempFileUrl.Length - 1)
                    {
                        fileUrl.Append("/");
                    }
                }

                AddFileUrl(tempFileUrl.Last(), fileUrl.ToString(),
                    !oneByOneModify ? URL_TYPE.Modify : URL_TYPE.oneByOneModify);
            }
        }

        /// <summary>
        /// 异步读取Url
        /// </summary>
        public async Task<JArray> GetModify(Task<string> data, bool oneByOneModify = false)
        {
            JObject jObject = await ParseResultJson(data);
            if (!oneByOneModify)
            {
                return (JArray) jObject["updateResource"]["modifyList"];
            }
            else
            {
                return (JArray) (await ParseResultJson(data))["updateResource"]["oneByOneModifyList"];
            }
        }

        /// <summary>
        /// 异步解析ResultJson
        /// </summary>
        public async Task<JObject> ParseResultJson(Task<string> data)
        {
            string JsonData = await data;

            using (StringReader stringReader = new StringReader(JsonData))
            {
                using (JsonTextReader reader = new JsonTextReader(stringReader))
                {
                    return await JToken.ReadFromAsync(reader) as JObject;
                }
            }
        }
    }

    class JsonParseSync
    {
        public class FileUrl
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public URL_TYPE URL_TYPE { get; set; }
        }

        public class DownloadConfing
        {
            public string Revision { get; set; }
            public string DownloadDomain { get; set; }
            public string DownloadPath { get; set; }
            public string NewDownloadSize { get; set; }
            public string OneByOneDownloadPath { get; set; }
            public string SubDomain { get; set; }
            public string ImageRev { get; set; }
            public string ImageDomain { get; set; }
            public string ImagePath { get; set; }
        }

        public List<FileUrl> FileUrls = new List<FileUrl>();
        public List<DownloadConfing> DownloadConfings = new List<DownloadConfing>();

        public void AddFileUrl(string name, string url, URL_TYPE uRL_TYPE)
        {
            FileUrls.Add(new FileUrl
            {
                Name = name,
                Url = url,
                URL_TYPE = uRL_TYPE
            });
        }
    }


    /// <summary>
    /// 定义URLTYPE
    /// </summary>
    public enum URL_TYPE
    {
        /// <summary>
        /// Modify
        /// </summary>
        Modify,

        /// <summary>
        /// oneByOneModify
        /// </summary>
        oneByOneModify
    }
}
