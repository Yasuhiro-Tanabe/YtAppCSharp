using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;
using MemorieDeFleurs.UI.WPF.Views.Helpers;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// 在庫推移表印刷コマンド
    /// </summary>
    public class PrintInventoryTransitionTableCommand : PrintCommand<InventoryTransitionTableViewModel>
    {
        /// <inheritdoc/>
        protected override void Execute(InventoryTransitionTableViewModel parameter)
        {
            UserControlPrinter.PrintDocument<InventoryTransitionTableViewModel, InventoryTransitionTableControl>(parameter);
        }
    }
}
