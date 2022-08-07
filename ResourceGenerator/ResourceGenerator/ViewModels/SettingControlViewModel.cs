using ResourceGenerator.Commands;
using ResourceGenerator.Models;
using ResourceGenerator.Resources;

using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace ResourceGenerator.ViewModels
{
    /// <summary>
    /// プロパティ設定画面のビューモデル。
    /// </summary>
    public class SettingControlViewModel : TabItemControlViewModel
    {
        #region プロパティ
        /// <summary>
        /// リソース管理ファイル名。
        /// </summary>
        public string InputFileName
        {
            get { return _input ?? string.Empty; }
            set { SetProperty(ref _input, value); }
        }
        private string? _input;
        /// <summary>
        /// 出力先フォルダ名。
        /// </summary>
        public string OutputFolderName
        {
            get { return _output ?? string.Empty; }
            set { SetProperty(ref _output, value); }
        }
        private string? _output;
        /// <summary>
        /// ResourceFinder クラスのサンプルコードを生成するかどうか。
        /// </summary>
        public bool GenerateResourceFinderSampleCode
        {
            get { return _genFinder; }
            set { SetProperty(ref _genFinder, value); }
        }
        private bool _genFinder = false;
        /// <summary>
        /// サンプルコードの名前空間。
        /// </summary>
        public string Namespace
        {
            get { return _ns ?? string.Empty; }
            set { SetProperty(ref _ns, value); }
        }
        private string? _ns;
        /// <summary>
        /// ResourceManager クラスのサンプルコードを生成するかどうか。
        /// </summary>
        public bool GenerateResourceManagerSampleCode
        {
            get { return _genManager; }
            set { SetProperty(ref _genManager, value); }
        }
        private bool _genManager = false;
        /// <summary>
        /// App.xaml に記載する ResourceDictionary のサンプルコードを生成するかどうか。
        /// </summary>
        public bool GenerateAppXamlResourceSampleCode
        {
            get { return _genAppRes; }
            set { SetProperty(ref _genAppRes, value); }
        }
        private bool _genAppRes = false;
        /// <summary>
        /// App.xaml に記載する ResourceDictionary のカルチャー。
        /// </summary>
        public ObservableCollection<CultureInfo> DefaultCultures { get; } = new ObservableCollection<CultureInfo>();
        /// <summary>
        /// <see cref="DefaultCultures"/> から選択したカルチャー。
        /// </summary>
        public CultureInfo SelectedCulture
        {
            get { return _culture ?? CultureInfo.InvariantCulture; }
            set { SetProperty(ref _culture, value); }
        }
        private CultureInfo? _culture;
        #endregion プロパティ
        #region コマンド
        /// <summary>
        /// リソース管理ファイルを開くコマンド。
        /// </summary>
        public ICommand Open { get; } = new OpenResourceManagementFileCommand();
        /// <summary>
        /// 出力先フォルダを選択するコマンド。
        /// </summary>
        public ICommand Select { get; } = new SelectOutputFolderCommand();
        /// <summary>
        /// 新規リソースファイルを開くコマンド。
        /// </summary>
        public ICommand New { get; } = new NewResourceManagementFileCommand();
        /// <summary>
        /// プロパティをリソース管理ファイルに保存するコマンド。
        /// </summary>
        public ICommand Save { get; } = new SavePropertiesCommand();
        /// <summary>
        /// リソース管理ファイルの内容をリソースファイルに書き出すコマンド。
        /// </summary>
        public ICommand Generate { get; } = new GenerateResourceFilesCommand();
        #endregion コマンド

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="model">呼び出し元で生成したリソース管理モデル。</param>
        public SettingControlViewModel(ResourceGenerationModel model) : base(model, ResourceFinder.FindText("Tab.Setting"), false)
        {
        }

        /// <summary>
        /// XAML エディタ用コンストラクタ。
        /// </summary>
        internal SettingControlViewModel() : base(new ResourceGenerationModel(), "", false)
        {
        }
    }
}
