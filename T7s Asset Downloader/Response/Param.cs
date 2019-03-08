using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using T7s_Asset_Downloader;

namespace T7s_Sig_Counter
{
    public class Param
    {
        public string Key { set; get; }
        public string Value { set; get; }
    }

    public class MakeParams
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public static List<Param> Params = new List<Param>();

        public static string GetUnixTime()
        {
            TimeSpan ts = DateTime.UtcNow - UnixEpoch;
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public static string SortParam()
        {
            return string.Join("&", (from e in Params.OrderBy((Param e) => e.Key, StringComparer.Ordinal)
                                     select $"{e.Key}={e.Value}").ToArray());
        }

        public static List<Param> AddCommonParams()
        {
            List<Param> Params = new List<Param>
            {
                new Param
                {
                    Key = "ver",
                    Value = Define.Ver
                },
                new Param
                {
                    Key = "rev",
                    Value = Define.Rev
                },
                new Param
                {
                    Key = "ts",
                    //Value = "1550571259"
                    Value = GetUnixTime()
                },
                new Param
                {
                    Key = "os",
                    Value = "android"
                },
                new Param
                {
                    Key = "blt",
                    Value = Define.Blt
                },
                new Param
                {
                    Key = "device",
                    Value = "Android"
                },
                new Param
                {
                    Key = "platform",
                    Value = "xiaomi%208"
                },
                new Param
                {
                    Key = "osversion",
                    Value = "5.1.1"
                },
                new Param
                {
                    Key = "jb",
                    Value = "0"
                },
                new Param
                {
                    Key = "pid",
                    //Value = "791080"
                    Value = SaveData.Decrypt(Define.encPid)
                }
            };

            return Params;
        }

        public void AddParam(string key, string value)
        {
            Params.Add(new Param
            {
                Key = key,
                Value = value
            });
        }

        public void AddSignatureParam(string id , string apiName)
        {
            Params = AddCommonParams();
            AddParam("userRev", Define.UserRev);
            Params.Add(new Param
            {
                Key = "sig",
                Value = GetSignature(id , apiName)
            });
        }

        public static string GetParam()
        {
            return string.Join("&", (from e in Params select $"{e.Key}={e.Value}").ToArray());
        }

        public string GetSignature ( string id , string apiName )
        {
            string uuid = SaveData.Decrypt(id);
            string sigKey = "0249E2D0-739D-47E7-9TOK-YO7THSISTERS&" + uuid;
            string data = apiName + "?" + SortParam();

            return Signature.EscapeRfc3986(Signature.MakeSignature(sigKey,Uri.UnescapeDataString(data)));
        }

    }

}
