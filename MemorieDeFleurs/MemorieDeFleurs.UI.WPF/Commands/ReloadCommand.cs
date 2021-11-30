using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IReloadable"/> 実装に必要なプロパティ更新コマンド
    /// </summary>
    public class ReloadCommand : CommandBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ReloadCommand() : base()
        {
            AddAction(typeof(IReloadable), UpdateDetailView);
        }

        private static void UpdateDetailView(object parameter) => (parameter as IReloadable).UpdateProperties();
    }
}
