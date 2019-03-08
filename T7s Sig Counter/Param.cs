using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace T7s_Sig_Counter
{
    public class Param
    {
        public string Key { set; get; }
        public string Value { set; get; }
    }

    public static class Counter
    {
        public static List<Param> Params = AddCommonParams();


        public static string SortParam()
        {
            return string.Join("&", (from e in Params.OrderBy((Param e) => e.Key, StringComparer.Ordinal)
                                     select $"{e.Key}={e.Value}").ToArray());
        }

        private static List<Param> AddCommonParams()
        {
            List<Param> Params = new List<Param>
            {
                new Param
                {
                    Key = "ver",
                    Value = "5.13.0"
                },
                new Param
                {
                    Key = "rev",
                    Value = "287"
                },
                new Param
                {
                    Key = "ts",
                    Value = "1550571259"
                },
                new Param
                {
                    Key = "os",
                    Value = "android"
                },
                new Param
                {
                    Key = "blt",
                    Value = "130"
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
                    Value = "790180"
                }
            };
            return Params;
        }

        public static string GetParam (string id)
        {
            Params.Add(new Param
            {
                Key = "sig",
                Value = GetSignature(id)
            });

            return string.Join("&", (from e in Params select $"{e.Key}={e.Value}").ToArray());
        }

        public static string GetSignature ( string id )
        {
            string uuid = SaveData.Decrypt(id);
            string sigKey = "0249E2D0-739D-47E7-9TOK-YO7THSISTERS&" + uuid;
            string data = "login?" + SortParam();

            return Signature.EscapeRfc3986(Signature.MakeSignature(sigKey,Uri.UnescapeDataString(data)));
        }

    }

}
