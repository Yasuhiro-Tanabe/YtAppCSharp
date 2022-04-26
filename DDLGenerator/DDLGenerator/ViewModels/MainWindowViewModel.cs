using YasT.Framework.Logging;

using System.ComponentModel;
using YasT.Framework.WPF;
using System.Collections.ObjectModel;

namespace DDLGenerator.ViewModels
{
    /// <summary>
    /// DDLGeneratorのビューモデル
    /// </summary>
    public class MainWindowViewModel : NotificationObject
    {
        private string _title = "データ定義ファイルジェネレータ";
        /// <summary>
        /// ウィンドウタイトルバーに表示する文字列
        /// </summary>
        public string WindowTitle
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _tableDefinitionFile;
        /// <summary>
        /// テーブル定義書ファイル名 (パスを含む)
        /// </summary>
        public string TableDefinitionFilePath
        {
            get { return _tableDefinitionFile; }
            set { SetProperty(ref _tableDefinitionFile, value); }
        }

        /// <summary>
        /// 画面出力するログ文言
        /// </summary>
        public string Log { get { return LogUtil.Appender?.Notification; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel()
        {
            LogUtil.Appender.PropertyChanged += OnLogRefreshed;

            TabItems.Add(new SQLiteDDLViewModel(this));
            TabItems.Add(new EFCoreEntityViewModel(this));

            LogUtil.Debug($"MainWindowViewModel Generated.");
        }

        public void OnLogRefreshed(object sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName.Equals(nameof(LogUtil.Appender.Notification)))
            {
                RaisePropertyChanged(nameof(Log));
            }
        }

        /// <summary>
        /// 画面に表示するタブのビューモデル
        /// </summary>
        public ObservableCollection<NotificationObject> TabItems { get; set; } = new ObservableCollection<NotificationObject>();
    }
}
