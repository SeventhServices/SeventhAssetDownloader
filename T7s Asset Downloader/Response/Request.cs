using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using T7s_Enc_Decoder;
using T7s_Sig_Counter;
using System.Net.Http.Headers;
using System.Net.Http.Handlers;
using System.Threading;

namespace T7s_Asset_Downloader
{
    internal class MakeRequest
    {
        #region Http
        private static HttpClient GetClient;
        private static HttpClient PostClient;
        //private static ManualResetEvent ManualResetEvent = new ManualResetEvent(true);

        public void HttpClientTest(ProgressMessageHandler progressMessageHandler)
        {
            GetClient = new HttpClient(progressMessageHandler)
            {
                BaseAddress = new Uri(Define.Domin),
                Timeout = new TimeSpan(0, 0, 10)
            };

            Task.Run(() =>
            {
                GetClient.SendAsync(new HttpRequestMessage
                {
                    Method = new HttpMethod("POST"),
                    RequestUri = new Uri(Define.BaseUrl + Define.GetApiName(Define.APINAME_TYPE.inspection))
                });
            });

        }

        public void _ini_PostClient(ProgressMessageHandler progressMessageHandler)
        {
            PostClient = new HttpClient(progressMessageHandler)
            {
                BaseAddress = new Uri(Define.BaseUrl),
                Timeout = new TimeSpan(0, 10, 0)
            };

            PostClient.DefaultRequestHeaders.Add("Expect", "100-continue");
            PostClient.DefaultRequestHeaders.Add("X-Unity-Version", "2018.2.6f1");
            PostClient.DefaultRequestHeaders.Add("UserAgent", "Dalvik/2.1.0 (Linux; U; Android 5.1.1; xiaomi 8 Build/LMY49I)");
            PostClient.DefaultRequestHeaders.Add("Host", "api.t7s.jp");
            PostClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            PostClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");

        }

        public async Task<string> MakeGetRequest(string getUrl, string savePath, string fileName)
        {
            try
            {
                var response = await GetClient.GetAsync(getUrl);

                if (response.IsSuccessStatusCode)
                {
                    byte[] FileBytes = response.Content.ReadAsByteArrayAsync().Result;

                    using (FileStream fileStream = File.OpenWrite(savePath + fileName))
                    {
                        fileStream.Write(FileBytes, 0, FileBytes.Length);
                        fileStream.Close();
                    }
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                    MessageBox.Show("文件不存在");
                }


                return fileName;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }

        }

        public async Task<string> MakePostRequest(string id, string apiName, bool save = false)
        {
            try
            {
                var makeParams = new MakeParams();
                makeParams.AddSignatureParam(id, apiName);
                var httpContent = new StringContent(MakeParams.GetParam())
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                        {
                            CharSet = "utf-8"
                        }
                    }
                };

                var response = PostClient.PostAsync(Define.GetApiName(Define.APINAME_TYPE.result)
                    , httpContent).Result;
                if (!response.IsSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                    MessageBox.Show("请求超时");
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }

        }

        public async Task<string> MakePostRequest(string id, string apiName, ProgressMessageHandler progressMessageHandler, bool save = false)
        {
            return await Task.Run(async () => {
                var makeParams = new MakeParams();
                makeParams.AddSignatureParam(id, apiName);
                HttpContent httpContent = new StringContent(MakeParams.GetParam());
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                {
                    CharSet = "utf-8"
                };

                using (var client = new HttpClient(progressMessageHandler)
                {
                    BaseAddress = new Uri(Define.BaseUrl)
                })
                {
                    client.DefaultRequestHeaders.Add("Expect", "100-continue");
                    client.DefaultRequestHeaders.Add("X-Unity-Version", "2018.2.6f1");
                    client.DefaultRequestHeaders.Add("UserAgent", "Dalvik/2.1.0 (Linux; U; Android 5.1.1; xiaomi 8 Build/LMY49I)");
                    client.DefaultRequestHeaders.Add("Host", "api.t7s.jp");
                    client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");

                    var httpResponse = client.PostAsync(Define.GetApiName(Define.APINAME_TYPE.result)
                        , httpContent).Result;
                    httpResponse.EnsureSuccessStatusCode();
                    //ManualResetEvent.WaitOne(100);
                    return await httpResponse.Content.ReadAsStringAsync();
                }
            });

        }



        #endregion

        #region RawHttp
        /// <summary>
        /// 生成GET请求
        /// </summary>
        /// <param name="getUrl">GET地址</param>
        /// <returns></returns>
        public async void RawMkaeGetRequest(string getUrl, string savePath)
        {
            await Task.Run(async () =>
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(getUrl);
                request.Method = "GET";
                request.ContentType = "application/json";
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //在这里对接收到的页面内容进行处理
                Stream responseStream = response.GetResponseStream();

                byte[] FileBytes = new byte[Convert.ToInt32(response.ContentLength)];
                int Size = await responseStream.ReadAsync(FileBytes, 0, FileBytes.Length);
                int NowSize = Size;
                using (FileStream fileStream = new FileStream(savePath, FileMode.Create))
                {
                    while (Size > 0)
                    {
                        await fileStream.WriteAsync(FileBytes, 0, Size);
                        Size = await responseStream.ReadAsync(FileBytes, 0, FileBytes.Length);
                        NowSize = NowSize + Size;
                    }
                    fileStream.Close();
                }
            });

        }
        /// <summary>
        /// 生成Post请求
        /// </summary>
        /// <param name="postUrl">POST地址</param>
        /// <param name="id">用户的id</param>
        /// <param name="apiName">请求的apiName</param>
        /// <returns></returns>
        public async Task<string> RawMakePostRequest(string postUrl, string id, string apiName, bool save = false)
        {
            MakeParams makeParams = new MakeParams();
            makeParams.AddSignatureParam(id, apiName);
            byte[] PrarmsBytes = Encoding.UTF8.GetBytes(MakeParams.GetParam());
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
            request.Method = "POST";

            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("X-Unity-Version", "2018.2.6f1");
            request.ContentLength = PrarmsBytes.Length;
            request.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 5.1.1; xiaomi 8 Build/LMY49I)";
            request.Host = "api.t7s.jp";
            request.Headers.Add("Accept-Encoding", "gzip");

            //request.Expect = "100-continue";
            //request.Connection = "Keep-Alive";
            //request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate; 

            //if (!request.HaveResponse)
            //{
            //    System.Windows.Forms.MessageBox.Show("网络异常，请重试！", "错误", System.Windows.Forms.MessageBoxButtons.RetryCancel); 
            //}

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(PrarmsBytes, 0, PrarmsBytes.Length);
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    string JsonString = await streamReader.ReadToEndAsync();
                    if (save)
                    {
                        if (!Directory.Exists(Define.LocalPath + @"\Asset\Result"))
                        {
                            Directory.CreateDirectory(Define.LocalPath + @"\Asset\Result");
                        }
                        using (StreamWriter streamWriter = new StreamWriter(Define.LocalPath + @"\Asset\Result\" + "Result.json"))
                        {
                            await streamWriter.WriteAsync(JsonString);
                            streamWriter.Close();
                        }
                    }
                    return JsonString;
                }
            }
        }


        #endregion

    }

   

}

