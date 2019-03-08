using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace T7s_Sig_Counter
{
    public static class Signature
    {
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (T item in self)
            {
                action(item);
            }
        }

        private static readonly char[] EncodeChars = new char[5]
{
        '!',
        '*',
        '\'',
        '(',
        ')'
};
        public static string EscapeRfc3986(string data)
        {
            StringBuilder sb = new StringBuilder();
            int num;
            for (int i = 0; i < data.Length; i += num)
            {
                num = Math.Min(data.Length - i, 32766);
                sb.Append(Uri.EscapeDataString(data.Substring(i, num)));
            }
            EncodeChars.ForEach(
                delegate (char c)
                {
                    sb.Replace(c.ToString(), Uri.HexEscape(c));
                });
            return sb.ToString();
        }

        public static string MakeSignature(string key, string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(key);
            byte[] bytes2 = Encoding.UTF8.GetBytes(EscapeRfc3986(data));

            using (HMACSHA1 hMACSHA = new HMACSHA1(bytes))
            {
                byte[] inArray = hMACSHA.ComputeHash(bytes2);
                return Uri.EscapeDataString(Convert.ToBase64String(inArray));
            }
        }

    }

    public static class SaveData
    {
        public static byte[] ConvertHexStringToByte(string hex)
        {
            int length = hex.Length;
            byte[] array = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                array[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return array;
        }

        public static string DecryptByTripleDES(string key, string iv, string encryptedText)
        {
            using (TripleDES tripleDES = TripleDES.Create())
            {
                tripleDES.Key = Encoding.ASCII.GetBytes(key);
                tripleDES.IV = ConvertHexStringToByte(iv);
                tripleDES.Mode = CipherMode.CBC;
                tripleDES.Padding = PaddingMode.Zeros;
                using (ICryptoTransform cryptoTransform = tripleDES.CreateDecryptor())
                {
                    byte[] array = ConvertHexStringToByte(encryptedText);
                    byte[] array2 = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
                    string @string = Encoding.ASCII.GetString(array2, 0, array2.Length);
                    return new string((from c in @string
                                       where !char.IsControl(c)
                                       select c).ToArray());
                }
            }
        }

        public static readonly byte[] Rand = new byte[36]
        {
        5,
        3,
        1,
        7,
        3,
        3,
        1,
        0,
        1,
        3,
        4,
        6,
        2,
        3,
        2,
        2,
        2,
        3,
        4,
        6,
        3,
        4,
        7,
        1,
        2,
        5,
        4,
        1,
        6,
        3,
        6,
        7,
        0,
        7,
        1,
        7
        };


        public static int Min(int a, int b)
        {
            return (a >= b) ? b : a;
        }

        public static string Decrypt(string str)
        {

            byte[] array = new byte[str.Length / 2];
            for (int i = 0; i < SaveData.Min(array.Length, Rand.Length); i++)
            {
                byte b = Convert.ToByte(str.Substring(i * 2, 2), 16);
                array[i] = (byte)(b ^ Rand[i]);
            }
            return Encoding.ASCII.GetString(array);
        }

        public static string Encrypt(string str)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < SaveData.Min(str.Length, Rand.Length); i++)
            {
                stringBuilder.Append(Convert.ToString(str[i] ^ Rand[i], 16));
            }
            return stringBuilder.ToString();
            

        }
        
    }


}
