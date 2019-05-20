using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using T7s_Asset_Downloader.Response;

namespace T7s_Asset_Downloader.Extensions
{
    public class EventScore
    {
        private readonly MakeRequest _makeRequest = new MakeRequest();

        private JObject _resultJsonObject;
        public List<FriendRanking> FriendRankings = new List<FriendRanking>();
        public List<Ranking> Rankings = new List<Ranking>();
        public string RankingString;

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

        public void ParseRanking()
        {
            if (!_resultJsonObject.ContainsKey("eventRankingUser")) return;
            if (!(_resultJsonObject["eventRankingUser"]["borderList"] is JArray borderList)) return;
            foreach (var border in borderList)
                Rankings.Add(new Ranking
                {
                    Score = Convert.ToUInt32(border["border"]),
                    Rank = Convert.ToUInt32(border["borderRank"])
                });
        }

        public void ParseFriendRanking()
        {
            if (!_resultJsonObject.ContainsKey("eventRankingUser")) return;
            if (!(_resultJsonObject["eventRankingUser"]["userRanking"]["eventParticipants"] is JArray friendBorderList)
            ) return;
            var id = 0;
            foreach (var border in friendBorderList)
            {
                FriendRankings.Add(new FriendRanking
                {
                    Id = id,
                    Name = border["userName"].ToString(),
                    Score = Convert.ToUInt32(border["score"]),
                    Rank = Convert.ToUInt32(border["rank"])
                });
                id++;
            }
        }

        public async void GetEventScore()
        {
            if (!Directory.Exists(Define.GetExtensionsPath() + @"Temp"))
                Directory.CreateDirectory(Define.GetExtensionsPath() + @"Temp");
            _makeRequest._ini_PostClient();
            Rankings.Clear();
            var useAccount = new UseAccount
            {
                pid = SaveData.Decrypt(Define.encPid),
                id = Define.Id
            };

            var apiName = Define.GetApiName(Define.APINAME_TYPE.event_ranking_user);
            var makeParams = new MakeParams();

            makeParams.AddParam("eventType", "10");
            makeParams.AddParam("rankingType", "0");
            makeParams.AddParam("musicId", "0");
            makeParams.AddParam("difficulty", "0");
            makeParams.AddParam("maxRank", "1");
            makeParams.AddParam("pickupUserId", "0");
            makeParams.AddParam("characterId", "0");
            makeParams.AddParam("friend", "1");

            makeParams.AddCommonParams();
            makeParams.AddParam("pid", useAccount.pid);


            makeParams.AddSignatureParam(useAccount.id, apiName);
            await _makeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());
            await ParseResultJsonAsync(Define.GetExtensionsTempPath());

            ParseRanking();

            RankingString = string.Join("\n", (from ranking in Rankings
                select $"{ranking.Rank}位 : {ranking.Score}").ToArray());
        }

        public class UseAccount
        {
            public string encPid { get; set; }
            public string id { get; set; }
            public string pid { get; set; }
        }

        public class Ranking
        {
            public uint Rank { get; set; }

            public uint Score { get; set; }
        }

        public class FriendRanking
        {
            public int Id { get; set; }
            public int GroupRank { get; set; }
            public string Name { get; set; }
            public uint Rank { get; set; }

            public uint Score { get; set; }
        }
    }
}