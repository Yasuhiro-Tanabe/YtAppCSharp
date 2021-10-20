using MemorieDeFleurs.UI.WPF.Commands;

using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class MainWindowViiewModel : ViewModelBase
    {
        #region プロパティ
        /// <summary>
        /// ウィンドウタイトル
        /// </summary>
        public string WindowTitle
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _title = "Memorie de Fleurs 花卉受発注支援システム 管理画面";

        /// <summary>
        /// システムメッセージ表示領域に表示する文言
        /// </summary>
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        private string _message;
        #endregion // プロパティ

        #region コマンド
        public ICommand Exit { get; } = new ExitCommand();
        #endregion // コマンド
    }
}
