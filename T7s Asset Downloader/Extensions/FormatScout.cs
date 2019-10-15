using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using T7s_Asset_Downloader.Asset;
using T7s_Asset_Downloader.Crypt;

namespace T7s_Asset_Downloader.Extensions
{
    public class NameDirectory
    {
        public List<NameTranslate> NameTranslates { get; set; }
    }

    public class NameTranslate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Translate { get; set; }
    }


    public class FormatScout
    {
        private readonly string _exportPath = Define.LocalPath + @"/Asset/Extensions/FormatScout/";

        private JObject _resultJsonObject;

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

        public async void Format(bool isLink)
        {
            var exportRawPath = _exportPath + @"RawText/";
            var exportForTranslatePath = _exportPath + @"ForTranslateText/";
            var exportForTranslateWithNewLinePath = _exportPath + @"ForTranslateTextWithNewLine/";
            if (!Directory.Exists(exportRawPath)) Directory.CreateDirectory(exportRawPath);
            if (!Directory.Exists(exportForTranslatePath)) Directory.CreateDirectory(exportForTranslatePath);
            if (!Directory.Exists(exportForTranslateWithNewLinePath)) Directory.CreateDirectory(exportForTranslateWithNewLinePath);

            var ofd = new OpenFileDialog
            {
                Title = @"选择要打开的文件",
                Multiselect = true,
                Filter = @"剧情文件|*.json",
                RestoreDirectory = true
            };

            var nameDirectory = new NameDirectory
            {
                NameTranslates = new List<NameTranslate>()
            };


            if (ofd.ShowDialog() == DialogResult.OK)
            {
                StreamWriter exportText = null,exportRawText = null,exportTextWithNewLine = null;

                foreach (var filePath in ofd.FileNames)
                {
                    var fileNameVer2 = Crypt.Crypt.ConvertFileName(filePath, EncVersion.Ver2, EncVersion.Ver1);
                    var fileName = Path.GetFileNameWithoutExtension(fileNameVer2);

                    await ParseResultJsonAsync(filePath);

                    var dialogList = _resultJsonObject["Pages"] as JArray;



                    if (isLink)
                    {
                        if (exportText == null)
                        {
                            exportText = File.CreateText(exportForTranslatePath + fileName + ".txt");
                            exportRawText = File.CreateText(exportRawPath + "raw_" + fileName + ".txt");
                            exportTextWithNewLine = File.CreateText(exportForTranslateWithNewLinePath + "raw_" + fileName + ".txt");
                        }

                        exportText.WriteLine("===============================");
                        exportText.WriteLine("位置 : Main episode EP4.0~ > EPISODE 4.0 AXiS ");
                        exportText.WriteLine("翻译人 : ");
                        exportText.WriteLine("===============================");
                        exportText.WriteLine($"文件名 : {fileNameVer2}");
                        exportText.WriteLine("文本 :\n");

                        exportTextWithNewLine.WriteLine("===============================");
                        exportTextWithNewLine.WriteLine("位置 : Main episode EP4.0~ > EPISODE 4.0 AXiS ");
                        exportTextWithNewLine.WriteLine("翻译人 : ");
                        exportTextWithNewLine.WriteLine("===============================");
                        exportTextWithNewLine.WriteLine($"文件名 : {fileNameVer2}");
                        exportTextWithNewLine.WriteLine("文本 :\n");

                        exportRawText.WriteLine($"文件名 : {fileNameVer2}");
                        exportRawText.WriteLine("文本 :\n");

                        await StartFormat(dialogList,nameDirectory,exportText,exportRawText,exportTextWithNewLine);

                    }
                    else
                    {
                        exportText = File.CreateText(exportForTranslatePath + fileName + ".txt");
                        exportRawText = File.CreateText(exportRawPath + "raw_" + fileName + ".txt");
                        exportTextWithNewLine = File.CreateText(exportForTranslateWithNewLinePath + "raw_" + fileName + ".txt");

                        var title = (dialogList ?? throw new InvalidOperationException()).SingleOrDefault(d =>
                            ((JObject)d).ContainsKey("SubTitle"))?["SubTitle"]["EpisodeName"];
            
                        exportText.WriteLine("===============================");
                        exportText.WriteLine($"标题 : {title}");
                        exportText.WriteLine("副标题 : ");
                        exportText.WriteLine("位置 : Main episode EP4.0~ > EPISODE 4.0 AXiS ");
                        exportText.WriteLine("翻译人 : ");
                        exportText.WriteLine("===============================");
                        exportText.WriteLine($"文件名 : {fileNameVer2}");
                        exportText.WriteLine("文本 :\n");

                        exportTextWithNewLine.WriteLine("===============================");
                        exportTextWithNewLine.WriteLine($"标题 : {title}");
                        exportTextWithNewLine.WriteLine("副标题 : ");
                        exportTextWithNewLine.WriteLine("位置 : Main episode EP4.0~ > EPISODE 4.0 AXiS ");
                        exportTextWithNewLine.WriteLine("翻译人 : ");
                        exportTextWithNewLine.WriteLine("===============================");
                        exportTextWithNewLine.WriteLine($"文件名 : {fileNameVer2}");
                        exportTextWithNewLine.WriteLine("文本 :\n");

                        exportRawText.WriteLine($"标题 : {title}");
                        exportRawText.WriteLine($"文件名 : {fileNameVer2}");
                        exportRawText.WriteLine("文本 :\n");

                        await StartFormat(dialogList,nameDirectory,exportText,exportRawText,exportTextWithNewLine);

                        exportText.Close();
                        exportRawText.Close();
                        exportTextWithNewLine.Close();
                    }
                }

                if (isLink)
                {
                    exportText?.Close();
                    exportRawText?.Close();
                    exportTextWithNewLine?.Close();
                }
            }


            nameDirectory.NameTranslates = nameDirectory.NameTranslates.Distinct(n => n.Name).ToList();

            for (var id = 0; id < nameDirectory.NameTranslates.Count; id++)
            {
                nameDirectory.NameTranslates[id].Id = id + 1;
                nameDirectory.NameTranslates[id].Translate = " ";
            }

            var directoryText = JsonConvert.SerializeObject(nameDirectory);

            using var streamWriter = new StreamWriter(_exportPath + "dictionary.json");
            streamWriter.Write(directoryText);
            streamWriter.Close();
        }

        private static async Task StartFormat(JArray dialogList , NameDirectory nameDirectory, 
            TextWriter exportText, TextWriter exportRawText, TextWriter exportTextWithNewLine)
        {
            var nowIndex = 0;

            foreach (var dialog in (dialogList).Where(d => ((JObject)d).ContainsKey("TextArea")))
            {
                nowIndex++;

                var name = dialog["TextArea"]["Name"];
                var dialogue = dialog["TextArea"]["Dialogue"];
                var dialogueWithoutSymbols = dialogue.ToString();

                await exportText.WriteLineAsync($"◆ {nowIndex:D4} ◆ {name} :");
                await exportText.WriteLineAsync($"◆ {nowIndex:D4} ◆「 {dialogueWithoutSymbols.Replace("\n", "")} 」");
                await exportText.WriteLineAsync($"◇ {nowIndex:D4} ◇ {name} :");
                await exportText.WriteLineAsync(
                    Regex.Matches(dialogueWithoutSymbols.Replace("\n", ""), @"\p{P}").Count == dialogueWithoutSymbols.Replace("\n", "").Length
                        ? $"◇ {nowIndex:D4} ◇「 {dialogueWithoutSymbols.Replace("\n", "")} 」"
                        : $"◇ {nowIndex:D4} ◇「 」 ");
                await exportText.WriteLineAsync(" ");

                await exportRawText.WriteLineAsync($"{name}");
                await exportRawText.WriteLineAsync($"「 {dialogueWithoutSymbols.Replace("\n", "")} 」");
                await exportRawText.WriteLineAsync(" ");
                //exportText.WriteLine($"◆ {NowIndex:D4} ◆:{name}:");
                //exportText.WriteLine($"◆ {NowIndex:D4} ◆:{dialogue}");
                //exportText.WriteLine($"◇ {NowIndex:D4} ◇:");
                //exportText.WriteLine($"◇ {NowIndex:D4} ◇:");
                //exportText.WriteLine(" ");

                await exportTextWithNewLine.WriteLineAsync($" {nowIndex:D4} : {name} :");
                if (dialogueWithoutSymbols.Length > 0) await exportTextWithNewLine.WriteLineAsync($" 「 {dialogueWithoutSymbols.Substring(0, dialogueWithoutSymbols.Length - 1)} 」");
                await exportTextWithNewLine.WriteLineAsync($" {nowIndex:D4} : {name} :");
                await exportTextWithNewLine.WriteLineAsync(
                    Regex.Matches(dialogueWithoutSymbols.Replace("\n", ""), @"\p{P}").Count == dialogueWithoutSymbols.Replace("\n", "").Length
                        ? $" 「 {dialogueWithoutSymbols.Substring(0, dialogueWithoutSymbols.Length - 1)} 」"
                        : $" 「 」 ");
                await exportTextWithNewLine.WriteLineAsync(" ");


                if (name == null) continue;
                var nameText = name.ToString();
                if (!Regex.IsMatch(nameText, @"\p{P}"))
                    nameDirectory.NameTranslates.Add(new NameTranslate
                    {
                        Name = nameText
                    });
            }
        }
    }
}