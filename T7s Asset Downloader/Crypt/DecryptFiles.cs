using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace T7s_Asset_Downloader.Crypt
{
    public static class DecryptFiles
    {
        public static void DecryptFile(string filePath, EncVersion encVersion)
        {
            byte[] fileBytes;

            switch (Save.GetFileType(filePath))
            {
                case ENC_TYPE.JPGorPNG:
                    using (var fileStream = File.OpenWrite(Save.GetSavePath(filePath)))
                    {
                        fileBytes = Crypt.Decrypt(File.ReadAllBytes(filePath), false, encVersion);
                        fileStream.Write(fileBytes, 0, fileBytes.Length);
                        fileStream.Close();
                    }

                    break;
                case ENC_TYPE.TXTorSQLorJSON:
                    using (var streamWriter = new StreamWriter(Save.GetSavePath(filePath)))
                    {
                        fileBytes = Crypt.Decrypt(File.ReadAllBytes(filePath), true, encVersion);
                        var fileText = Encoding.UTF8.GetString(fileBytes).Replace("\r", "");
                        streamWriter.Write(fileText);
                        streamWriter.Close();
                    }

                    break;
                case ENC_TYPE.BIN:
                    using (var streamWriter = new StreamWriter(Save.GetSavePath(filePath)))
                    {
                        fileBytes = Crypt.Decrypt(File.ReadAllBytes(filePath), false, encVersion);
                        var fileText = Encoding.UTF8.GetString(fileBytes);
                        streamWriter.Write(fileText);
                        streamWriter.Close();
                    }

                    break;
                case ENC_TYPE.UNKONWN:
                    using (var fileStream = File.OpenWrite(Save.GetSavePath(filePath)))
                    {
                        fileBytes = Crypt.Decrypt(File.ReadAllBytes(filePath), false, encVersion);
                        fileStream.Write(fileBytes, 0, fileBytes.Length);
                        fileStream.Close();
                    }

                    break;
                case ENC_TYPE.ERROR:
                    MessageBox.Show(@"无法识别");
                    break;

                case ENC_TYPE.ACB:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void DecryptForDownloader(string path)
        {
            var fileBytes = Crypt.Decrypt(File.ReadAllBytes(path), true);
            using (var streamWriter = new StreamWriter(Save.GetSavePathForDownloader(path)))
            {
                var fileText = Encoding.UTF8.GetString(fileBytes);
                streamWriter.Write(fileText);
                streamWriter.Close();
            }
        }
    }

    public static class Save
    {
        public static string GetSavePathForDownloader(string FilePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(FilePath) + "\\Deconde Files"))
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath) + "\\Deconde Files");
            var SavePath = Path.GetDirectoryName(FilePath) + "\\Deconde Files\\" + Path.GetFileName(FilePath);
            return SavePath;
        }

        public static string GetSavePath(string FilePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(FilePath) + "\\Deconde Files"))
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath) + "\\Deconde Files");
            var SavePath = Path.GetDirectoryName(FilePath) + "\\Deconde Files\\" +
                           Path.GetFileNameWithoutExtension(FilePath);
            return SavePath;
        }

        public static ENC_TYPE GetFileType(string FilePath)
        {
            var FileType = FilePath.Split('.');
            var Type = FileType[FileType.Length - 2];
            if (Equals(Type, "txt") | Equals(Type, "sql") | Equals(Type, "json")) return ENC_TYPE.TXTorSQLorJSON;

            if (Equals(Type, "png") | Equals(Type, "jpg")) return ENC_TYPE.JPGorPNG;

            if (Equals(Type, "bin"))
            {
                return ENC_TYPE.BIN;
            }
            //if (Equals(FileType.Last(), "acb"))
            //{

            //}
            if (Equals(FileType.Last(), "enc")) return ENC_TYPE.UNKONWN;
            return ENC_TYPE.ERROR;
        }
    }

    /// <summary>
    ///     定义加密文件类型
    /// </summary>
    public enum ENC_TYPE
    {
        /// <summary>
        ///     文本或数据文件
        /// </summary>
        TXTorSQLorJSON,

        /// <summary>
        ///     图片文件（JPG or PNG）
        /// </summary>
        JPGorPNG,

        /// <summary>
        ///     其他文件（BIN）
        /// </summary>
        BIN,

        /// <summary>
        ///     音频文件（ACB）
        /// </summary>
        ACB,

        /// <summary>
        ///     未知加密
        /// </summary>
        UNKONWN,

        /// <summary>
        ///     无法识别
        /// </summary>
        ERROR
    }
}