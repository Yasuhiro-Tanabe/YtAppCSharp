using MemorieDeFleurs.UI.WPF.Commands;

using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class MainWindowViiewModel : ViewModelBase
    {
        public string WindowTitle { get; } = "Memorie de Fleurs 花卉受発注支援システム 管理画面";

        #region コマンド
        public ICommand Exit { get; } = new ExitCommand();
        #endregion // コマンド
    }
}
