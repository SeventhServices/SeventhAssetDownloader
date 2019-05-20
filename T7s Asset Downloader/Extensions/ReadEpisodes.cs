using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using T7s_Asset_Downloader.Response;

namespace T7s_Asset_Downloader.Extensions
{
    public class ReadEpisodes
    {
        public static string RankingString;
        private readonly MakeRequest _makeRequest = new MakeRequest();

        private JObject _resultJsonObject;
        public List<Episode> Episodes = new List<Episode>();

        public async Task<int> ParseResultJsonAsync(string path)
        {
            using (var file = File.OpenText(path))
            {
                using (var reader = new JsonTextReader(file))
                {
                    _resultJsonObject = await JToken.ReadFromAsync(reader) as JObject;
                }
            }

            return 0;
        }

        public void ParseEpisodeMain()
        {
            if (!_resultJsonObject.ContainsKey("episodeMain")) return;
            if (!(_resultJsonObject["episodeMain"]["episodeInfo"]["newIds"] is JArray episodes)) return;
            foreach (var episode in episodes)
                Episodes.Add(new Episode
                {
                    EpisodeId = Convert.ToUInt32(episode)
                });
        }

        public async void StartRead()
        {
            await Task.Delay(1000 * 15);


            if (!Directory.Exists(Define.GetExtensionsPath() + @"Temp"))
                Directory.CreateDirectory(Define.GetExtensionsPath() + @"Temp");
            _makeRequest._ini_PostClient();
            Episodes.Clear();
            var useAccount = new EventScore.UseAccount
            {
                pid = "790180",
                id = "34303764313230342c336765612e3666633529676062342c353767673232603f30613234"
            };
            var makeParams = new MakeParams();
            string apiName;
            var count = 0;


            while (count < 40)
            {
                Episodes.Clear();

                Console.WriteLine($@"Now Count:{count}");

                apiName = Define.GetApiName(Define.APINAME_TYPE.episode_main);
                makeParams.ClearParam();
                makeParams.AddCommonParams();
                makeParams.AddParam("pid", useAccount.pid);
                makeParams.AddSignatureParam(useAccount.id, apiName);
                await _makeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());
                await ParseResultJsonAsync(Define.GetExtensionsTempPath());
                ParseEpisodeMain();

                await Task.Delay(1000 * 3);

                apiName = Define.GetApiName(Define.APINAME_TYPE.scenario_result);
                makeParams.ClearParam();
                makeParams.AddParam("scenarioId", Episodes[0].EpisodeId.ToString());
                makeParams.AddCommonParams();
                makeParams.AddParam("pid", useAccount.pid);
                makeParams.AddSignatureParam(useAccount.id, apiName);
                await _makeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());
                await ParseResultJsonAsync(Define.GetExtensionsTempPath());

                Console.WriteLine($@"Read Success! id:{Episodes[0].EpisodeId.ToString()}");

                await Task.Delay(1000 * 3);
                count++;
            }
        }

        public class UseAccount
        {
            public string encPid { get; set; }
            public string id { get; set; }

            public string pid { get; set; }
        }

        public class Episode
        {
            public uint EpisodeId { get; set; }
        }
    }
}