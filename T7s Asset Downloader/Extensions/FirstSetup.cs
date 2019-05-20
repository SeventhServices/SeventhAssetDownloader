using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using T7s_Asset_Downloader.Response;

namespace T7s_Asset_Downloader.Extensions
{
    internal class FirstSetup
    {
        private static readonly MakeRequest MakeRequest = new MakeRequest();
        public readonly GetCard GetCard = new GetCard(MakeRequest);
        private JObject _resultJsonObject;

        public List<Account> Accounts = new List<Account>();

        public List<Card> UserCardList = new List<Card>();

        public string DecryptUUID(string encUUID, string iv)
        {
            return SaveData.DecryptByTripleDES("ho8s8Q72r72H5dopYJThoScE", iv, encUUID);
        }

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

        public Account ParseAccount()
        {
            var firstSetupString = _resultJsonObject["firstSetup"];
            var eud = firstSetupString["eud"].ToString();
            var ivs = firstSetupString["ivs"].ToString();
            var tpid = firstSetupString["tpid"].ToString();
            var uuid = DecryptUUID(eud, ivs);
            var account = new Account
            {
                pid = tpid,
                uuid = uuid,
                encPid = SaveData.Encrypt(tpid),
                Id = SaveData.Encrypt(uuid),
                eud = eud,
                ivs = ivs,
                tpid = tpid
            };
            return account;
        }

        public string ParseGachaMain(int index)
        {
            var gachaList = _resultJsonObject["gachaMain"]["gachaList"] as JArray;
            if (gachaList == null) return "0";
            var gachaDetails = gachaList[index]["gachaDetails"];
            var gachaId = gachaDetails[0]["gachaId"].ToString();
            return gachaId;
        }

        public Gacha ParseGachaResult()
        {
            var gachaResult = _resultJsonObject["gachaResult"];
            var getCardList = gachaResult["getCardList"] as JArray;

            var Gacha = new Gacha
            {
                CardList = new List<Card>()
            };
            foreach (var card in getCardList)
                Gacha.CardList.Add(new Card
                {
                    Id = (int) card["cardId"]
                });

            return Gacha;
        }

        private void SetUserCardList(Account account)
        {
            var gachaResult = _resultJsonObject["gachaResult"];
            var userCardList = gachaResult["userCardList"] as JArray;

            account.userCardList = new List<Card>();
            foreach (var card in userCardList)
                account.userCardList.Add(new Card
                    {
                        Id = (int) card["cardId"]
                    }
                );
        }

        private Task DownloadGachaCard(Gacha gacha, string savePath)
        {
            return new Task(() =>
            {
                foreach (var card in gacha.CardList) GetCard.SaveFileAndDecrypt(card.Id, savePath);
            });
        }


        public async void StartFirstSetup()
        {
            MakeRequest._ini_PostClient();
            var accountCount = 0;
            try
            {
                do
                {
                    Define.Rev = "0";
                    var apiName = Define.GetApiName(Define.APINAME_TYPE.first);
                    var makeParams = new MakeParams();
                    makeParams.AddCommonParams();
                    makeParams.AddSignatureParam("", apiName, true);
                    await MakeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());
                    await ParseResultJsonAsync(Define.GetExtensionsTempPath());

                    var account = ParseAccount();

                    apiName = Define.GetApiName(Define.APINAME_TYPE.comp);
                    makeParams.ClearParam();
                    makeParams.AddParam("tpid", account.tpid);
                    makeParams.AddCommonParams();
                    makeParams.AddSignatureParam(account.Id, apiName);
                    await MakeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());

                    apiName = Define.GetApiName(Define.APINAME_TYPE.login);
                    makeParams.ClearParam();
                    makeParams.AddCommonParams();
                    makeParams.AddParam("pid", account.pid);
                    makeParams.AddSignatureParam(account.Id, apiName);
                    await MakeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());

                    apiName = Define.GetApiName(Define.APINAME_TYPE.tutorial_chara);
                    makeParams.ClearParam();
                    makeParams.AddParam("characterId", "4");
                    makeParams.AddCommonParams();
                    makeParams.AddParam("pid", account.pid);
                    makeParams.AddSignatureParam(account.Id, apiName);
                    await MakeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());

                    apiName = Define.GetApiName(Define.APINAME_TYPE.tutorial_name);
                    makeParams.ClearParam();
                    makeParams.AddParam("userName", "777wiki");
                    makeParams.AddCommonParams();
                    makeParams.AddParam("pid", account.pid);
                    makeParams.AddSignatureParam(account.Id, apiName);
                    await MakeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());

                    apiName = Define.GetApiName(Define.APINAME_TYPE.tutorial_type);
                    makeParams.ClearParam();
                    makeParams.AddParam("userTypeId", "1");
                    makeParams.AddCommonParams();
                    makeParams.AddParam("pid", account.pid);
                    makeParams.AddSignatureParam(account.Id, apiName);
                    await MakeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());

                    apiName = Define.GetApiName(Define.APINAME_TYPE.gacha_main);
                    makeParams.ClearParam();
                    makeParams.AddCommonParams();
                    makeParams.AddParam("pid", account.pid);
                    makeParams.AddSignatureParam(account.Id, apiName);
                    await MakeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());
                    await ParseResultJsonAsync(Define.GetExtensionsTempPath());

                    apiName = Define.GetApiName(Define.APINAME_TYPE.gacha_result);
                    makeParams.ClearParam();
                    makeParams.AddParam("gachaId", ParseGachaMain(0));
                    makeParams.AddParam("continueNum", "1");
                    makeParams.AddParam("paymentType", "4");
                    makeParams.AddParam("sellRarity", "0");
                    makeParams.AddParam("removeTarget", "0");
                    makeParams.AddParam("eventType", "0");
                    makeParams.AddCommonParams();
                    makeParams.AddParam("pid", account.pid);
                    makeParams.AddSignatureParam(account.Id, apiName);
                    await MakeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());
                    await ParseResultJsonAsync(Define.GetExtensionsTempPath());

                    account.Gachas = new List<Gacha> {ParseGachaResult()};

                    DownloadGachaCard(account.Gachas[0], $@"{Define.GetExtensionsSavePath()}{account.pid}\").Start();

                    apiName = Define.GetApiName(Define.APINAME_TYPE.tutorial_end);
                    makeParams.ClearParam();
                    makeParams.AddCommonParams();
                    makeParams.AddParam("pid", account.pid);
                    makeParams.AddSignatureParam(account.Id, apiName);
                    await MakeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());

                    apiName = Define.GetApiName(Define.APINAME_TYPE.tutorial_questionnaire);
                    makeParams.ClearParam();
                    makeParams.AddParam("answers%5B%5D", "0");
                    makeParams.AddParam("answers%5B%5D", "2");
                    makeParams.AddParam("answers%5B%5D", "1");
                    makeParams.AddParam("answers%5B%5D", "11");
                    makeParams.AddCommonParams();
                    makeParams.AddParam("pid", account.pid);
                    makeParams.AddSignatureParam(account.Id, apiName);
                    await MakeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());

                    apiName = Define.GetApiName(Define.APINAME_TYPE.tutorial_beginner);
                    makeParams.ClearParam();
                    makeParams.AddCommonParams();
                    makeParams.AddParam("pid", account.pid);
                    makeParams.AddSignatureParam(account.Id, apiName);
                    await MakeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());

                    apiName = Define.GetApiName(Define.APINAME_TYPE.gacha_main);
                    makeParams.ClearParam();
                    makeParams.AddCommonParams();
                    makeParams.AddParam("pid", account.pid);
                    makeParams.AddSignatureParam(account.Id, apiName);
                    await MakeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());
                    await ParseResultJsonAsync(Define.GetExtensionsTempPath());

                    var gachaId1 = ParseGachaMain(2);
                    var gachaId2 = ParseGachaMain(3);

                    apiName = Define.GetApiName(Define.APINAME_TYPE.gacha_result);
                    makeParams.ClearParam();
                    makeParams.AddParam("gachaId",gachaId1 );
                    makeParams.AddParam("continueNum", "1");
                    makeParams.AddParam("paymentType", "1");
                    makeParams.AddParam("sellRarity", "0");
                    makeParams.AddParam("removeTarget", "0");
                    makeParams.AddParam("eventType", "0");
                    makeParams.AddCommonParams();
                    makeParams.AddParam("pid", account.pid);
                    makeParams.AddSignatureParam(account.Id, apiName);
                    await MakeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());
                    await ParseResultJsonAsync(Define.GetExtensionsTempPath());

                    account.Gachas.Add(ParseGachaResult());

                    apiName = Define.GetApiName(Define.APINAME_TYPE.gacha_result);
                    makeParams.ClearParam();
                    makeParams.AddParam("gachaId", gachaId2);
                    makeParams.AddParam("continueNum", "1");
                    makeParams.AddParam("paymentType", "1");
                    makeParams.AddParam("sellRarity", "0");
                    makeParams.AddParam("removeTarget", "0");
                    makeParams.AddParam("eventType", "0");
                    makeParams.AddCommonParams();
                    makeParams.AddParam("pid", account.pid);
                    makeParams.AddSignatureParam(account.Id, apiName);
                    await MakeRequest.MakeNaturalPostRequest(apiName, makeParams.GetParam());
                    await ParseResultJsonAsync(Define.GetExtensionsTempPath());

                    account.Gachas.Add(ParseGachaResult());

                    SetUserCardList(account);

                    DownloadGachaCard(account.Gachas[1], Define.GetExtensionsSavePath() + account.pid + @"\").Start();

                    Accounts.Add(account);

                    Thread.Sleep(60000);
                    accountCount++;
                } while (accountCount < 5);
            }
            finally
            {
                var downloadConfing = JsonConvert.SerializeObject(Accounts);
                using (var fileStream = File.OpenWrite(Define.GetExtensionsSavePath() + @"Accounts.json"))
                {
                    var fileBytes = Encoding.UTF8.GetBytes(downloadConfing);
                    fileStream.Write(fileBytes, 0, fileBytes.Length);
                    fileStream.Close();
                }
            }
        }

        public class Card
        {
            public int Id;
        }

        public class Gacha
        {
            public List<Card> CardList { get; set; }
        }

        public class Account
        {
            public string pid { get; set; }
            public string uuid { get; set; }
            public string Id { get; set; }
            public string encPid { get; set; }
            public string eud { get; set; }
            public string ivs { get; set; }
            public string tpid { get; set; }
            public List<Gacha> Gachas { get; set; }
            public List<Card> userCardList { get; set; }
        }
    }
}