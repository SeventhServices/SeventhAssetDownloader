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

        public async void Format()
        {
            var exportRawPath = _exportPath + @"RawText/";
            var exportForTranslatePath = _exportPath + @"ForTranslateText/";
            if (!Directory.Exists(exportRawPath)) Directory.CreateDirectory(exportRawPath);

            if (!Directory.Exists(exportForTranslatePath)) Directory.CreateDirectory(exportForTranslatePath);


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
                foreach (var filePath in ofd.FileNames)
                {
                    var fileNameVer2 = Crypt.Crypt.ConvertFileName(filePath, EncVersion.Ver2, EncVersion.Ver1);
                    var fileName = Path.GetFileNameWithoutExtension(fileNameVer2);

                    await ParseResultJsonAsync(filePath);

                    var dialogList = _resultJsonObject["Pages"] as JArray;

                    var NowIndex = 0;

                    var exportText = File.CreateText(exportForTranslatePath + fileName + ".txt");
                    var exportRawText = File.CreateText(exportRawPath + "raw_" + fileName + ".txt");


                    exportText.WriteLine("标题 : ");
                    exportText.WriteLine("副标题 : ");
                    exportText.WriteLine("位置 : ");
                    exportText.WriteLine("翻译人 : ");
                    exportText.WriteLine("===============================");
                    exportText.WriteLine($"文件名 : {fileNameVer2}");
                    exportText.WriteLine("文本 :\n");
                    exportRawText.WriteLine($"文件名 : {fileNameVer2}");
                    exportRawText.WriteLine("文本 :\n");

                    foreach (var dialog in dialogList.Where(d => ((JObject) d).ContainsKey("TextArea")))
                    {
                        NowIndex++;

                        var name = dialog["TextArea"]["Name"];
                        var dialogue = dialog["TextArea"]["Dialogue"];
                        var dialogueWithoutSymbols = dialogue.ToString().Replace("\n", "");

                        await exportText.WriteLineAsync($"◆ {NowIndex:D4} ◆ {name} :");
                        await exportText.WriteLineAsync($"◆ {NowIndex:D4} ◆「 {dialogueWithoutSymbols} 」");
                        await exportText.WriteLineAsync($"◇ {NowIndex:D4} ◇ {name} :");
                        await exportText.WriteLineAsync(
                            Regex.Matches(dialogueWithoutSymbols, @"\p{P}").Count == dialogueWithoutSymbols.Length
                                ? $"◇ {NowIndex:D4} ◇「 {dialogueWithoutSymbols} 」"
                                : $"◇ {NowIndex:D4} ◇「 」 ");
                        await exportText.WriteLineAsync(" ");

                        await exportRawText.WriteLineAsync($"{name}");
                        await exportRawText.WriteLineAsync($"「 {dialogueWithoutSymbols} 」");
                        await exportRawText.WriteLineAsync(" ");
                        //exportText.WriteLine($"◆ {NowIndex:D4} ◆:{name}:");
                        //exportText.WriteLine($"◆ {NowIndex:D4} ◆:{dialogue}");
                        //exportText.WriteLine($"◇ {NowIndex:D4} ◇:");
                        //exportText.WriteLine($"◇ {NowIndex:D4} ◇:");
                        //exportText.WriteLine(" ");
                        if (name == null) continue;
                        var nameText = name.ToString();
                        if (!Regex.IsMatch(nameText, @"\p{P}"))
                            nameDirectory.NameTranslates.Add(new NameTranslate
                            {
                                Name = nameText
                            });
                    }

                    exportText.Close();
                    exportRawText.Close();
                }

            nameDirectory.NameTranslates = nameDirectory.NameTranslates.Distinct(n => n.Name).ToList();

            for (var id = 0; id < nameDirectory.NameTranslates.Count; id++)
            {
                nameDirectory.NameTranslates[id].Id = id + 1;
                nameDirectory.NameTranslates[id].Translate = " ";
            }

            var DirectoryText = JsonConvert.SerializeObject(nameDirectory);

            using (var streamWriter = new StreamWriter(_exportPath + "dictionary.json"))
            {
                streamWriter.Write(DirectoryText);
                streamWriter.Close();
            }
        }
    }
}