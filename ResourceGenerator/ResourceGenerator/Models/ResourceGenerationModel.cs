using ClosedXML.Excel;

using ResourceGenerator.Resources;
using ResourceGenerator.Templates;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using YasT.Framework.Logging;
using YasT.Framework.WPF;

namespace ResourceGenerator.Models
{
    /// <summary>
    /// リソース管理モデル。
    /// </summary>
    public class ResourceGenerationModel : NotificationObject
    {
        #region イベント
        /// <summary>
        /// 既存のサンプルコードが更新されたときに発行されるイベント。
        /// </summary>
        public event EventHandler<KeyValuePair<string, string>>? CodeUpdated;
        /// <summary>
        /// 新しいサンプルコードが追加されたときに発行されるイベント。
        /// </summary>
        public event EventHandler<KeyValuePair<string, string>>? CodeGenerated;
        #endregion イベント
        #region プロパティ
        /// <summary>
        /// 出力先フォルダ名。
        /// </summary>
        public string OutputPath
        {
            get { return Properties[ResourceFinder.FindText("OutputFolderPropertyName")]; }
            set { Properties[ResourceFinder.FindText("OutputFolderPropertyName")] = value; }
        }
        /// <summary>
        /// サンプルクラスの名前空間。
        /// </summary>
        public string Namespace
        {
            get { return Properties[ResourceFinder.FindText("NameSpecePropertyName")]; }
            set { Properties[ResourceFinder.FindText("NameSpecePropertyName")] = value; }
        }
        private ResourceProperties Properties { get; } = new ResourceProperties();
        /// <summary>
        /// リソースファイル中のカルチャー情報
        /// </summary>
        public IList<CultureInfo> Cultures { get; } = new List<CultureInfo>();
        private IList<string> ResourceSheets { get; } = new List<string>();
        private IDictionary<string, string> SampleCodes { get; } = new Dictionary<string, string>();
        #endregion プロパティ

        /// <summary>
        /// リソース管理ファイルを開く。
        /// </summary>
        /// <param name="path">リソース管理ファイルのパス。</param>
        public void OpenResourceManagementModel(string path)
        {
            if(!File.Exists(path))
            {
                throw new ApplicationException(ResourceFinder.FindText("CannotOpenResourceManagementFile", path));
            }

            using (var book = new XLWorkbook(path))
            {
                try
                {
                    Properties.LoadFrom(book);
                    CollectCultureInfo();
                    CollectResourceSheets(book);
                }
                catch (ApplicationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ResourceFinder.FindText("FileMayNotBeResourceManagementFile", path, ex.Message), ex);
                }
            }

            LogUtil.Info($"File '{path}' was opened.");
        }
        private void CollectCultureInfo()
        {
            Cultures.Clear();

            var iMax = Properties.NumberOfCultures;
            for(int i = 0; i < iMax; i++)
            {
                Cultures.Add(Properties.Cultures[i]);
            }
        }
        private void CollectResourceSheets(XLWorkbook book)
        {
            ResourceSheets.Clear();
            LogUtil.Debug($"{book.Worksheets.Count} sheets in the file.");
            foreach(var sheet in book.Worksheets.Where(s => s.Name != Properties.PropertySheetName))
            {
                LogUtil.Debug($"Sheet='{sheet.Name}'");
                ResourceSheets.Add(sheet.Name);
            }
        }

        /// <summary>
        /// リソース管理ファイルに現在保持しているプロパティを保存する。
        /// </summary>
        /// <param name="path">リソース管理ファイルのパス。</param>
        public void SaveResourceProperties(string path)
        {
            if (!File.Exists(path))
            {
                throw new ApplicationException(ResourceFinder.FindText("CannotOpenResourceManagementFile", path));
            }

            try
            {
                using (var book = new XLWorkbook(path))
                {
                    Properties.SaveTo(book);
                    book.Save();
                }
            }
            catch (ApplicationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ResourceFinder.FindText("FileMayNotBeResourceManagementFile", path), ex);
            }

            LogUtil.Info($"Properties were saved to file '{path}'.");
        }

        /// <summary>
        /// リソースファイルを生成する。
        /// </summary>
        /// <param name="inFile">入力となるリソース管理ファイル。</param>
        /// <param name="rootPath">出力先となるルートフォルダ名。</param>
        public void GenerateResourceFiles(string inFile, string rootPath)
        {
            using(var book = new XLWorkbook(inFile))
            {
                GenerateResourceFiles(book, rootPath);
            }
        }
        private int FindCell(IXLWorksheet sheet, CultureInfo cul)
        {
            for(int i = 1; ; i++)
            {
                var cname = sheet.Cell(1, i).Value.ToString();
                if(cul.Name == cname) { return i; }
            }
            throw new ApplicationException(ResourceFinder.FindText("NoSuchCultureName", cul.Name, sheet.Name));
        }
        private void GenerateResourceFiles(XLWorkbook book, string root)
        {

            foreach(var shName in ResourceSheets)
            {
                LogUtil.Debug($"Sheet '{shName}'");
                var sheet = book.Worksheet(shName);
                foreach(var cul in Cultures)
                {
                    LogUtil.Debug($"Culture '{cul.Name}");
                    var c = FindCell(sheet, cul);

                    var tmpl = new ResourceXamlTemplate();
                    tmpl.Culture = cul;

                    for (int r = 2; ; r++)
                    {
                        LogUtil.Debug($"R{r}C{c}");
                        var key = sheet.Cell(r, 1).Value.ToString();
                        var value = sheet.Cell(r, c).Value.ToString();
                        if (string.IsNullOrWhiteSpace(key)) { break; }
                        tmpl.Properties.Add(key, value);
                    }

                    var outFolder = Path.Combine(root, cul.Name);
                    var outFile = Path.Combine(outFolder, $"{shName}.xaml");
                    if (!Directory.Exists(Path.Combine(root, cul.Name)))
                    {
                        Directory.CreateDirectory(outFolder);
                    }

                    using (var w = new StreamWriter(outFile))
                    {
                        w.Write(tmpl.TransformText());
                        w.Flush();
                        LogUtil.Info($"Resource XAML file '{outFile}' generated.");
                    }
                }
            }
        }

        /// <summary>
        /// ResourceFinder のサンプルコードを生成する。
        /// 生成されたサンプルコードとそのデフォルトファイル名(ResourceFinder.cs) が
        /// <see cref="CodeGenerated"/> または <see cref="CodeUpdated"/> イベントとしてイベント監視している各オブジェクトに通知される。
        /// </summary>
        /// <param name="ns">サンプルコードの名前空間。</param>
        public void GenerateResourceFinderSampleCode(string ns)
        {
            var tmpl = new ResourceFinderSourceCodeTemplate();
            tmpl.Namespace = ns;
            var kv = KeyValuePair.Create(ResourceFinder.FindText("ResourceFinderClassFileName"), tmpl.TransformText());
            AddOrReplaceSampleCode(kv);
        }

        private void AddOrReplaceSampleCode(KeyValuePair<string, string> kv)
        {
            if (SampleCodes.ContainsKey(kv.Key))
            {
                SampleCodes[kv.Key] = kv.Value;
                CodeUpdated?.Invoke(this, kv);
            }
            else
            {
                SampleCodes.Add(kv);
                CodeGenerated?.Invoke(this, kv);
            }
        }

        /// <summary>
        /// ResourceManager クラスのサンプルコードを生成する。
        /// 生成されたサンプルコードとそのデフォルトファイル名(ResourceManager.cs) が
        /// <see cref="CodeGenerated"/> または <see cref="CodeUpdated"/> イベントとしてイベント監視している各オブジェクトに通知される。
        /// </summary>
        /// <param name="ns">サンプルコードの名前空間。</param>
        /// <param name="genFinder">ResourceFinder クラスのサンプルコードを生成するかどうか。
        /// 生成するかどうかで、クラスのコメントが一部変わってくる。</param>
        public void GenerateResourceManagerSampleCode(string ns, bool genFinder)
        {
            var tmpl = new ResourceManagerSourceCodeTemplate();
            tmpl.Namespace = ns;
            foreach(var c in Cultures) { tmpl.Cultures.Add(c); }
            foreach(var s in ResourceSheets) { tmpl.ResourceFiles.Add(s); }

            var kv = KeyValuePair.Create(ResourceFinder.FindText("ResourceManagerClassFileName"), tmpl.TransformText());
            AddOrReplaceSampleCode(kv);
        }

        /// <summary>
        /// App.xaml 内に記述する ResourceDictionary のサンプルコードを生成する。
        /// </summary>
        /// <param name="ns">名前空間</param>
        /// <param name="path">出力先フォルダのパス。</param>
        /// <param name="culture">サンプルコードに書き込むリソースファイルのカルチャー情報。</param>
        public void GenerateAppXamlResourceDictionarySampleCode(string ns, string path, CultureInfo culture)
        {
            var tmpl = new AppXamlResourceCodeTemplate();
            tmpl.Namespace = ns.Split(".").First();
            tmpl.DefaultCulture = culture;
            tmpl.ResourceRootPath = Path.GetDirectoryName(path)?.Split(Path.DirectorySeparatorChar).LastOrDefault() ?? string.Empty;
            foreach(var f in ResourceSheets) { tmpl.ResourceFiles.Add(f); }

            var kv = KeyValuePair.Create(ResourceFinder.FindText("AppXamlFileName"), tmpl.TransformText());
            AddOrReplaceSampleCode(kv);
        }
    }
}
