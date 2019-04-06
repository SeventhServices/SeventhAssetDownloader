using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using T7s_Enc_Decoder;
using static System.IO.File;

namespace T7s_Asset_Downloader
{

    public static class EnumerableExtender
    {
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                var elementValue = keySelector(element);
                if (seenKeys.Add(elementValue))
                {
                    yield return element;
                }
            }
        }
    }
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

        public JObject ResultJsonObject; 

        public List<FileUrl> FileUrls = new List<FileUrl>();

        public List<DownloadConfing> DownloadConfings = new List<DownloadConfing>();

        private void OnGetComplete()
        {
            System.Windows.Forms.MessageBox.Show(@"获取完成", @"Notice", System.Windows.Forms.MessageBoxButtons.OK,
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

        public void AddFileUrl(JsonParse jsonParse, string name, string url, URL_TYPE uRL_TYPE)
        {
            jsonParse.FileUrls.Add(new FileUrl
            {
                Name = name,
                Url = url,
                URL_TYPE = uRL_TYPE
            });
        }

        public void DeleteFileUrl(JsonParse jsonParse, string name)
        {
            var fileUrls = jsonParse.FileUrls.SingleOrDefault(u => u.Name == name);
            jsonParse.FileUrls.Remove(fileUrls);
        }


        #region Save Data Encrypt Async

        public async void UpdateUrlIndex(JsonParse jsonParse, bool encrypt)
        {
            if (!Directory.Exists(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev))
            {
                Directory.CreateDirectory(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev);
            }
            if (!Directory.Exists(Define.LocalPath + @"\Asset\Index\Temp"))
            {
                Directory.CreateDirectory(Define.LocalPath + @"\Asset\Index\Temp");
            }
            if ((await ParseResultJsonAsync(Define.GetUpdatePath())) == 1)
            {
                return;
            }

            var baseFileUrl = (GetModify())[1].ToString();
            var tempFileUrl = baseFileUrl.Split('/');

            var downloadPath = new StringBuilder();
            for (var j = 0; j < 2; j++)
            {
                downloadPath.Append(tempFileUrl[j]);
                downloadPath.Append("/");
            }
            
            jsonParse.DownloadConfings.Add(new DownloadConfing
            {
                Revision = ResultJsonObject["updateResource"]["revision"].ToString(),
                DownloadDomain = ResultJsonObject["updateResource"]["downloadConfig"]["domain"].ToString(),
                DownloadPath = downloadPath.ToString(),
                NewDownloadSize = ResultJsonObject["updateResource"]["downloadSize"].ToString(),
                OneByOneDownloadPath = ResultJsonObject["updateResource"]["downloadConfig"]["oneByOneDownloadPath"].ToString(),
                SubDomain = ResultJsonObject["updateResource"]["downloadConfig"]["subDomain"].ToString(),
                ImageRev = ResultJsonObject["updateResource"]["imageRev"].ToString(),
                ImagePath = ResultJsonObject["updateResource"]["imagePath"].ToString()
            });
            var downloadConfing = JsonConvert.SerializeObject(jsonParse.DownloadConfings);
            using (var fileStream = OpenWrite(Define.LocalPath + @"\Asset\Index\Temp" + @"\Confing.json"))
            {
                var fileBytes = Crypt.Encrypt(Encoding.UTF8.GetBytes(downloadConfing), true);
                fileStream.Write(fileBytes, 0, fileBytes.Length);
                fileStream.Close();
            }

            Define.JsonParse.DownloadConfings.Clear();
            Define.JsonParse.LoadConfing(Define.LocalPath + @"\Asset\Index\Temp" + @"\Confing.json", encrypt);
            Define._ini_Coning();
            DownloadConfings.Clear();
            Directory.CreateDirectory(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev);
            Copy(Define.LocalPath + @"\Asset\Index\Temp" + @"\Confing.json",
                Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev + @"\Confing.json", true);
            Copy(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev + @"\Confing.json",
                Define.GetConfingPath(), true);

            ParseModify(false,true,jsonParse);
            ParseModify(true,true,jsonParse);

            foreach (var deleteFiles in GetModify(false,true))
            {
                DeleteFileUrl(jsonParse,deleteFiles.ToString());
            }

            var urlIndex = JsonConvert.SerializeObject(jsonParse.FileUrls.OrderBy((FileUrl e) => e.Name, StringComparer.Ordinal).Distinct(f => f.Name));
            using (var fileStream = OpenWrite(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev + @"\Index.json"))
            {
                var fileBytes = Crypt.Encrypt(Encoding.UTF8.GetBytes(urlIndex), false, true);
                fileStream.Write(fileBytes, 0, fileBytes.Length);
                fileStream.Close();
            }

            FileUrls.Clear();
            Define.JsonParse.FileUrls.Clear();
            Copy(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev + @"\Index.json",
                Define.GetIndexPath(), true);
            Define.JsonParse.LoadUrlIndex(Define.GetIndexPath(), encrypt);
            Define.IsGetNewComplete = true;
        }

        public async Task<UPDATE_STATUS> SaveDlConfing(Task<string> data, bool encrypt)
        {
            if (!Directory.Exists(Define.LocalPath + @"\Asset\Index\Temp"))
            {
                Directory.CreateDirectory(Define.LocalPath + @"\Asset\Index\Temp");
            }

            ResultJsonObject = null;

            if ((await ParseResultJsonAsync(data)) == 1)
            {
                return UPDATE_STATUS.Error;
            }

            var testRev = GetRevOrError();
            if (testRev.Substring(0,1) == "e")
            {
                return UPDATE_STATUS.Error;
            }
            else
            {
                if (Convert.ToInt16(Define.LastRev) >= Convert.ToInt16(testRev))
                {
                    return UPDATE_STATUS.NoNecessary;
                }
            }

            var baseFileUrl = (GetModify())[1].ToString();
            var tempFileUrl = baseFileUrl.Split('/');

            var downloadPath = new StringBuilder();
            for (var j = 0; j < 2; j++)
            {
                downloadPath.Append(tempFileUrl[j]);
                downloadPath.Append("/");
            }

            DownloadConfings.Add(new DownloadConfing
            {
                Revision = ResultJsonObject["updateResource"]["revision"].ToString(),
                DownloadDomain = ResultJsonObject["updateResource"]["downloadConfig"]["domain"].ToString(),
                DownloadPath = downloadPath.ToString(),
                NewDownloadSize = ResultJsonObject["updateResource"]["downloadSize"].ToString(),
                OneByOneDownloadPath = ResultJsonObject["updateResource"]["downloadConfig"]["oneByOneDownloadPath"].ToString(),
                SubDomain = ResultJsonObject["updateResource"]["downloadConfig"]["subDomain"].ToString(),
                ImageRev = ResultJsonObject["updateResource"]["imageRev"].ToString(),
                ImagePath = ResultJsonObject["updateResource"]["imagePath"].ToString()
            });
            var downloadConfing = JsonConvert.SerializeObject(DownloadConfings);
            using (var fileStream = OpenWrite(Define.LocalPath + @"\Asset\Index\Temp" + @"\Confing.json"))
            {
                var fileBytes = Crypt.Encrypt(Encoding.UTF8.GetBytes(downloadConfing), true, true);
                fileStream.Write(fileBytes, 0, fileBytes.Length);
                fileStream.Close();
            }

            Define.JsonParse.DownloadConfings.Clear();
            Define.JsonParse.LoadConfing(Define.LocalPath + @"\Asset\Index\Temp" + @"\Confing.json", encrypt);
            Define._ini_Coning();
            DownloadConfings.Clear();
            Directory.CreateDirectory(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev);
            Copy(Define.LocalPath + @"\Asset\Index\Temp" + @"\Confing.json",
                Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev + @"\Confing.json", true);
            Copy(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev + @"\Confing.json",
                Define.GetConfingPath(), true);

            return UPDATE_STATUS.Ok;
        }

        public async void SaveUrlIndex(Task<string> data, bool encrypt)
        {
            if (!Directory.Exists(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev))
            {
                Directory.CreateDirectory(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev);
            }

            FileUrls.Clear();

            ResultJsonObject = null;

            if ((await ParseResultJsonAsync(data)) == 1)
            {
                return;
            }

            GetError();

            ParseModify();
            ParseModify(true);

            var urlIndex =
                JsonConvert.SerializeObject(FileUrls.OrderBy((FileUrl e) => e.Name, StringComparer.Ordinal).Distinct());
            using (FileStream fileStream =
                OpenWrite(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev + @"\Index.json"))
            {
                byte[] fileBytes = Crypt.Encrypt<Byte[]>(Encoding.UTF8.GetBytes(urlIndex), false, true);
                fileStream.Write(fileBytes, 0, fileBytes.Length);
                fileStream.Close();
            }

            FileUrls.Clear();
            Define.JsonParse.FileUrls.Clear();
            Copy(Define.LocalPath + @"\Asset\Index\" + "r" + Define.NowRev + @"\Index.json",
                Define.GetIndexPath(), true);
            Define.JsonParse.LoadUrlIndex(Define.GetIndexPath(), encrypt);
            Define.IsGetNewComplete = true;
            //OnGetComplete();
        }

        #endregion

        #region Load Data Encrypt

        public void LoadUrlIndex(string indexPath, bool encrypt)
        {
            byte[] fileBytes = Crypt.Decrypt<Byte[]>(ReadAllBytes(indexPath), false, true);
            string fileText = Encoding.UTF8.GetString(fileBytes);
            FileUrls = DeserializeJsonToList(fileText);
        }

        public void LoadConfing(string confingPath, bool encrypt)
        {
            byte[] fileBytes = Crypt.Decrypt<Byte[]>(ReadAllBytes(confingPath),true, true);
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

        #endregion

        #region Read Json String Async

        public async Task<UPDATE_STATUS> TestUpdateStatusAsync()
        {
            ResultJsonObject = null;

            if (( await ParseResultJsonAsync(Define.GetUpdatePath()) ) == 1)
            {
                return UPDATE_STATUS.Error;
            }
            var testRev = GetRevOrError();
            if (testRev.Substring(0, 1) == "e")
            {
                return UPDATE_STATUS.Error;
            }

            return Convert.ToInt16(Define.NowRev) >= Convert.ToInt16(testRev) 
                ? UPDATE_STATUS.NoNecessary 
                : UPDATE_STATUS.Ok;
        }



        public string GetRevOrError()
        {
            if (ResultJsonObject.Property("error") != null)
            {
                return $"error:{ResultJsonObject["error"]["errorCode"]}";
            }
            var rev = ResultJsonObject["rev"];
            return rev != null ? rev.ToString() :"error";
        }

        public void GetError()
        {
            if (ResultJsonObject.Property("error") != null)
            {
                MessageBox.Show( $"error:{ResultJsonObject["error"]["errorCode"]}");
            }
        }


        public void ParseModify( bool oneByOneModify = false, bool update = false,
            JsonParse jsonParse = null)
        {
            var urlList = GetModify( oneByOneModify );

            foreach (var url in urlList)
            {
                var baseFileUrl = url.ToString();
                var tempFileUrl = baseFileUrl.Split('/');

                var fileUrl = new StringBuilder();
                for (var j = 2; j < tempFileUrl.Length; j++)
                {
                    fileUrl.Append(tempFileUrl[j]);
                    if (j != tempFileUrl.Length - 1)
                    {
                        fileUrl.Append("/");
                    }
                }

                if (update)
                {
                    AddFileUrl(jsonParse, tempFileUrl.Last(), fileUrl.ToString(),
                        !oneByOneModify ? URL_TYPE.Modify : URL_TYPE.oneByOneModify);
                }
                else
                {
                    AddFileUrl(tempFileUrl.Last(), fileUrl.ToString(),
                        !oneByOneModify ? URL_TYPE.Modify : URL_TYPE.oneByOneModify);
                }
            }
        }

        /// <summary>
        /// 读取Url
        /// </summary>
        public JArray GetModify(bool oneByOneModify = false , bool delete = false)
        {
            if (delete)
            {
                return (JArray) ResultJsonObject["updateResource"]["deleteList"];
            }
            if (!oneByOneModify)
            {
                return (JArray) ResultJsonObject["updateResource"]["modifyList"];
            }
            else
            {
                return (JArray) ResultJsonObject["updateResource"]["oneByOneModifyList"];
            }
        }

        /// <summary>
        /// 异步解析ResultJson
        /// </summary>
        public async Task<int> ParseResultJsonAsync(Task<string> data)
        {
            var jsonData = await data;
            if (jsonData.Substring(0, 1) == "e")
            {
                return 1;
            }
            using (var stringReader = new StringReader(jsonData))
            {
                using (var reader = new JsonTextReader(stringReader))
                {
                    ResultJsonObject = await JToken.ReadFromAsync(reader) as JObject;
                }
            }

            return 0;
        }
        public async Task<int> ParseResultJsonAsync(string path)
        {
            using (var file = OpenText(path))
            {
                using (var reader = new JsonTextReader(file))
                {
                    ResultJsonObject = await JToken.ReadFromAsync(reader) as JObject;
                }
            }

            return 0;
        }

        #endregion

        #region Now UnUsed

        #region Save Data Async

        public async void SaveDlConfing(Task<string> data)
        {
            if (!Directory.Exists(Define.LocalPath + @"\Asset\Index\Temp"))
            {
                Directory.CreateDirectory(Define.LocalPath + @"\Asset\Index\Temp");
            }

            ResultJsonObject = null;

            await ParseResultJsonAsync(data);

            var baseFileUrl = (GetModify())[1].ToString();
            string[] tempFileUrl = baseFileUrl.Split('/');

            StringBuilder downloadPath = new StringBuilder();
            for (int j = 0; j < 2; j++)
            {
                downloadPath.Append(tempFileUrl[j]);
                downloadPath.Append("/");
            }

            DownloadConfings.Add(new DownloadConfing
            {
                Revision = ResultJsonObject["updateResource"]["revision"].ToString(),
                DownloadDomain = ResultJsonObject["updateResource"]["downloadConfig"]["domain"].ToString(),
                DownloadPath = downloadPath.ToString(),
                NewDownloadSize = ResultJsonObject["updateResource"]["downloadSize"].ToString(),
                OneByOneDownloadPath = ResultJsonObject["updateResource"]["downloadConfig"]["oneByOneDownloadPath"].ToString(),
                SubDomain = ResultJsonObject["updateResource"]["downloadConfig"]["subDomain"].ToString(),
                ImageRev = ResultJsonObject["updateResource"]["imageRev"].ToString(),
                ImagePath = ResultJsonObject["updateResource"]["imagePath"].ToString()
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

            ResultJsonObject = null;

            await ParseResultJsonAsync(data);

            ParseModify();
            ParseModify(true);

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

        #endregion

        #region Save Data

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

        #endregion

        #region Read Json String

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

            using (System.IO.StreamReader file = OpenText(jsonfile))
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

        #endregion

        #region Load Data

        public void LoadUrlIndex(string indexPath)
        {
            using (var file = OpenText(indexPath))
            {
                FileUrls = DeserializeJsonToList(file.ReadToEnd());
            }
        }

        public void LoadConfing(string confingPath)
        {
            using (System.IO.StreamReader file = OpenText(confingPath))
            {
                DownloadConfings = DeserializeJsonToList(file.ReadToEnd(), true);
            }
        }

        #endregion

        #endregion
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

    public enum UPDATE_STATUS
    {

        Error,

        NoNecessary,

        Ok
    }
}
