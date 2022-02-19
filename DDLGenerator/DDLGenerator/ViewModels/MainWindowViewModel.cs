using YasT.Framework.Logging;

using System.ComponentModel;
using YasT.Framework.WPF;

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


        /// <summary>
        /// 画面出力するログ文言
        /// </summary>
        public string Log { get { return LogUtil.Appender?.Notification; } }

        public MainWindowViewModel()
        {
            LogUtil.Appender.PropertyChanged += OnLogRefreshed;

            LogUtil.Debug($"MainWindowViewModel Generated.");
        }

        public void OnLogRefreshed(object sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName.Equals(nameof(LogUtil.Appender.Notification)))
            {
                RaisePropertyChanged(nameof(Log));
            }
        }

    }
}
