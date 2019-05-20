using System;
using System.Collections.Generic;
using System.Linq;

namespace T7s_Asset_Downloader.Response
{
    public class Param
    {
        public string Key { set; get; }
        public string Value { set; get; }
    }

    public class MakeParams
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        public List<Param> Params = new List<Param>();

        public string GetUnixTime()
        {
            var ts = DateTime.UtcNow - UnixEpoch;
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public string SortParam()
        {
            return string.Join("&", (from e in Params.OrderBy(e => e.Key, StringComparer.Ordinal)
                select $"{e.Key}={e.Value}").ToArray());
        }

        public void AddCommonParams()
        {
            var CommonParams = new List<Param>
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
                }
            };

            Params.AddRange(CommonParams);
        }

        public void AddParam(string key, string value)
        {
            Params.Add(new Param
            {
                Key = key,
                Value = value
            });
        }

        public void ClearParam()
        {
            Params.Clear();
        }

        public void AddSignatureParam(string id, string apiName, bool isFirst = false)
        {
            AddParam("sig", GetSignature(id, apiName, isFirst));
        }

        public string GetParam()
        {
            return string.Join("&", (from e in Params select $"{e.Key}={e.Value}").ToArray());
        }

        public string GetSignature(string id, string apiName, bool isFirst = false)
        {
            string sigKey;
            if (isFirst)
            {
                sigKey = "0249E2D0-739D-47E7-9TOK-YO7THSISTERS";
            }
            else
            {
                var uuid = SaveData.Decrypt(id);
                sigKey = "0249E2D0-739D-47E7-9TOK-YO7THSISTERS&" + uuid;
            }

            var data = apiName + "?" + SortParam();

            return Signature.EscapeRfc3986(Signature.MakeSignature(sigKey, Uri.UnescapeDataString(data)));
        }
    }
}