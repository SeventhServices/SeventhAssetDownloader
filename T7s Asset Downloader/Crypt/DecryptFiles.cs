using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace T7s_Enc_Decoder
{
    public static class DecryptFiles
    {
        public static void DecryptFile(string FilePath)
        {
            byte[] FileBytes;

            switch (Save.GetFileType(FilePath))
            {
                case ENC_TYPE.JPGorPNG :
                    using (FileStream fileStream = File.OpenWrite(Save.GetSavePath(FilePath)))
                    {
                        FileBytes = Crypt.Decrypt<Byte[]>(System.IO.File.ReadAllBytes(FilePath));
                        fileStream.Write(FileBytes, 0, FileBytes.Length);
                        fileStream.Close();
                    }
                    break;
                case ENC_TYPE.TXTorSQLorJSON :
                    using (StreamWriter streamWriter = new StreamWriter(Save.GetSavePath(FilePath)))
                    {
                        FileBytes = Crypt.Decrypt<Byte[]>(System.IO.File.ReadAllBytes(FilePath), true);
                        string FileText = Encoding.UTF8.GetString(FileBytes);
                        streamWriter.Write(FileText);
                        streamWriter.Close();
                    }
                    break;
                case ENC_TYPE.BIN :
                    using (StreamWriter streamWriter = new StreamWriter(Save.GetSavePath(FilePath)))
                    {
                        FileBytes = Crypt.Decrypt<Byte[]>(System.IO.File.ReadAllBytes(FilePath));
                        string FileText = Encoding.UTF8.GetString(FileBytes);
                        streamWriter.Write(FileText);
                        streamWriter.Close();
                    }
                    break;
                case ENC_TYPE.ERROR:
                    System.Windows.Forms.MessageBox.Show("无法识别");
                    break;

            }
        }


    }

    public static class Save
    {
        public static string GetSavePath(string FilePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(FilePath) + "\\Deconde Files"))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath) + "\\Deconde Files");
            }
            string SavePath = Path.GetDirectoryName(FilePath) + "\\Deconde Files\\" + Path.GetFileNameWithoutExtension(FilePath);
            return SavePath;
        }

        public static ENC_TYPE GetFileType(string FilePath)
        {
            string[] FileType = FilePath.Split('.');
            string Type = FileType[FileType.Length - 2];
            if (Equals(Type, "txt") | Equals(Type, "sql" )| Equals(Type, "json"))
            {
                return ENC_TYPE.TXTorSQLorJSON;
            }
            else if (Equals(Type, "png") | Equals(Type, "jpg"))
            {
                return ENC_TYPE.JPGorPNG;
            }
            else if (Equals(Type, "bin" ))
            {
                return ENC_TYPE.BIN;
            }
            else
            {
                return ENC_TYPE.ERROR;
            }

        }


    }

    /// <summary>
    /// 定义加密文件类型
    /// </summary>
    public enum ENC_TYPE
    {
        /// <summary>
        /// 文本或数据文件
        /// </summary>
        TXTorSQLorJSON,
        /// <summary>
        /// 图片文件（JPG or PNG）
        /// </summary>
        JPGorPNG,
        /// <summary>
        /// 其他文件（BIN）
        /// </summary>
        BIN,
        /// <summary>
        /// 无法识别
        /// </summary>
        ERROR,
    }
}
