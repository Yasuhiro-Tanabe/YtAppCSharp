using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;
using MemorieDeFleurs.UI.WPF.Views.Helpers;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// 仕入先発注書印刷コマンド
    /// </summary>
    public class PrintOrderToSupplierCommand : PrintCommand<OrderToSupplierDetailViewModel>
    {
        /// <inheritdoc/>
        protected override void Execute(OrderToSupplierDetailViewModel parameter)
        {
            UserControlPrinter.PrintDocument<OrderToSupplierDetailViewModel, OrderSheetToSupplier>(parameter);
        }
    }
}
