using DDLGenerator.Commands;
using DDLGenerator.Models.Logging;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace DDLGenerator.ViewModels
{
    /// <summary>
    /// DDLGeneratorのビューモデル
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティを更新しイベントを発行する
        /// </summary>
        /// <typeparam name="T">プロパティの型</typeparam>
        /// <param name="variable">プロパティの値を格納する変数名</param>
        /// <param name="value">プロパティの値</param>
        /// <param name="caller">プロパティ名、省略可。省略した場合は呼び出し元 setter をもつプロパティ名。</param>
        private void SetProperty<T>(ref T variable, T value, [CallerMemberName] string caller = "")
        {
            variable = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Log)));
            }
        }

    }
}
