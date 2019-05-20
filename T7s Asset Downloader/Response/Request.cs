using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace T7s_Asset_Downloader.Response
{
    internal class MakeRequest
    {
        #region Http

        private static HttpClient _getClient;
        private static HttpClient _singleGetClient;

        private static HttpClient _postClient;
        //private static ManualResetEvent ManualResetEvent = new ManualResetEvent(true);

        public void HttpClientTest(ProgressMessageHandler progressMessageHandler)
        {
            _getClient = new HttpClient(progressMessageHandler)
            {
                BaseAddress = new Uri(Define.Domin),
                Timeout = new TimeSpan(0, 0, 10)
            };

            Task.Run(() =>
            {
                _getClient.SendAsync(new HttpRequestMessage
                {
                    Method = new HttpMethod("POST"),
                    RequestUri = new Uri(Define.BaseUrl + Define.GetApiName(Define.APINAME_TYPE.inspection))
                });
            });
        }

        public void _ini_GetClient()
        {
            _singleGetClient = new HttpClient();
        }

        public void _ini_PostClient()
        {
            _postClient = new HttpClient
            {
                BaseAddress = new Uri(Define.BaseUrl),
                Timeout = new TimeSpan(0, 10, 0)
            };

            _postClient.DefaultRequestHeaders.Add("Expect", "100-continue");
            _postClient.DefaultRequestHeaders.Add("X-Unity-Version", "2018.2.6f1");
            _postClient.DefaultRequestHeaders.Add("UserAgent",
                "Dalvik/2.1.0 (Linux; U; Android 5.1.1; xiaomi 8 Build/LMY49I)");
            _postClient.DefaultRequestHeaders.Add("Host", "api.t7s.jp");
            _postClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            _postClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
        }

        public async Task<string> MakeSingleGetRequest(string getUrl, string savePath, string fileName)
        {
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);

            try
            {
                var response = await _singleGetClient.GetAsync(getUrl);

                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = response.Content.ReadAsByteArrayAsync().Result;

                    using (var fileStream = File.OpenWrite(savePath + fileName))
                    {
                        await fileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                        fileStream.Close();
                    }
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                }

                return fileName;
            }
            catch (Exception e)
            {
                Console.WriteLine($@"ERROR:{fileName}");
                throw;
            }
        }


        public async Task<string> MakeGetRequest(string getUrl, string savePath, string fileName)
        {
            try
            {
                var response = await _getClient.GetAsync(getUrl);

                if (response.IsSuccessStatusCode)
                {
                    var fileBytes = response.Content.ReadAsByteArrayAsync().Result;

                    using (var fileStream = File.OpenWrite(savePath + fileName))
                    {
                        await fileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
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
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<int> MakeUpdatePostRequest(string id, string apiName)
        {
            try
            {
                var Params = new MakeParams();
                Params.AddParam("userRev", Define.UserRev);
                Params.AddCommonParams();
                Params.AddParam("pid", SaveData.Decrypt(Define.encPid));
                Params.AddSignatureParam(id, apiName);
                var httpContent = new StringContent(Params.GetParam())
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                        {
                            CharSet = "utf-8"
                        }
                    }
                };

                var response = await _postClient.PostAsync(Define.GetApiName(Define.APINAME_TYPE.result)
                    , httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using (var streamWriter = new StreamWriter(Define.GetUpdatePath()))
                    {
                        streamWriter.Write(jsonString);
                        streamWriter.Close();
                    }

                    return 0;
                }

                response.EnsureSuccessStatusCode();
                return 1;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        public async Task<string> MakePostRequest(string id, string apiName, bool update = false)
        {
            try
            {
                var makeParams = new MakeParams();
                makeParams.AddParam("userRev", Define.UserRev);
                makeParams.AddCommonParams();
                makeParams.AddParam("pid", SaveData.Decrypt(Define.encPid));
                makeParams.AddSignatureParam(id, apiName);
                var httpContent = new StringContent(makeParams.GetParam())
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                        {
                            CharSet = "utf-8"
                        }
                    }
                };

                var response = await _postClient.PostAsync(Define.GetApiName(Define.APINAME_TYPE.result)
                    , httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    if (!update)
                        return jsonString;
                    using (var streamWriter = new StreamWriter(Define.GetUpdatePath()))
                    {
                        streamWriter.Write(jsonString);
                        streamWriter.Close();
                    }
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                    MessageBox.Show(@"请求超时");
                }

                return "complete";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        public async Task<int> MakeNaturalPostRequest(string apiname, string param)
        {
            try
            {
                var httpContent = new StringContent(param)
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded")
                        {
                            CharSet = "utf-8"
                        }
                    }
                };

                var response = await _postClient.PostAsync(apiname
                    , httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    using (var streamWriter = new StreamWriter(Define.GetExtensionsTempPath()))
                    {
                        streamWriter.Write(jsonString);
                        streamWriter.Close();
                    }
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                    MessageBox.Show(@"请求超时");
                }

                return 0;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                throw;
            }
        }

        public async Task<string> MakePostRequest(string id, string apiName,
            ProgressMessageHandler progressMessageHandler, bool save = false)
        {
            return await Task.Run(async () =>
            {
                var makeParams = new MakeParams();
                makeParams.AddSignatureParam(id, apiName);
                HttpContent httpContent = new StringContent(makeParams.GetParam());
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
                    client.DefaultRequestHeaders.Add("UserAgent",
                        "Dalvik/2.1.0 (Linux; U; Android 5.1.1; xiaomi 8 Build/LMY49I)");
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
        ///     生成GET请求
        /// </summary>
        /// <param name="getUrl">GET地址</param>
        /// <returns></returns>
        public async void RawMkaeGetRequest(string getUrl, string savePath)
        {
            await Task.Run(async () =>
            {
                var request = (HttpWebRequest) WebRequest.Create(getUrl);
                request.Method = "GET";
                request.ContentType = "application/json";
                var response = (HttpWebResponse) request.GetResponse();
                //在这里对接收到的页面内容进行处理
                var responseStream = response.GetResponseStream();

                var FileBytes = new byte[Convert.ToInt32(response.ContentLength)];
                var Size = await responseStream.ReadAsync(FileBytes, 0, FileBytes.Length);
                var NowSize = Size;
                using (var fileStream = new FileStream(savePath, FileMode.Create))
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
        ///     生成Post请求
        /// </summary>
        /// <param name="postUrl">POST地址</param>
        /// <param name="id">用户的id</param>
        /// <param name="apiName">请求的apiName</param>
        /// <returns></returns>
        public async Task<string> RawMakePostRequest(string postUrl, string id, string apiName, bool save = false)
        {
            var makeParams = new MakeParams();
            makeParams.AddSignatureParam(id, apiName);
            var PrarmsBytes = Encoding.UTF8.GetBytes(makeParams.GetParam());
            var request = (HttpWebRequest) WebRequest.Create(new Uri(postUrl));
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

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(PrarmsBytes, 0, PrarmsBytes.Length);
            }

            using (var response = (HttpWebResponse) request.GetResponse())
            {
                using (var streamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    var JsonString = await streamReader.ReadToEndAsync();
                    if (save)
                    {
                        if (!Directory.Exists(Define.LocalPath + @"\Asset\Result"))
                            Directory.CreateDirectory(Define.LocalPath + @"\Asset\Result");
                        using (var streamWriter =
                            new StreamWriter(Define.LocalPath + @"\Asset\Result\" + "Result.json"))
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