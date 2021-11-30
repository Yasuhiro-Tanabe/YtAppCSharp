using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;
using MemorieDeFleurs.UI.WPF.Views.Helpers;

using System.Windows.Controls;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IPrintable"/> 実装に必要な印刷コマンド
    /// </summary>
    public class PrintCommand : CommandBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PrintCommand() : base()
        {
            AddAction(typeof(ProcessingInstructionViewModel), Print<ProcessingInstructionViewModel, ProcessingInstructionControl>);
            AddAction(typeof(OrderToSupplierDetailViewModel), Print<OrderToSupplierDetailViewModel, OrderSheetToSupplier>);
            AddAction(typeof(InventoryTransitionTableViewModel), Print<InventoryTransitionTableViewModel, InventoryTransitionTableControl>);
        }

        private static void Print<VM, V>(object parameter) where VM : NotificationObject, IPrintable, IReloadable where V : UserControl, new()
            => UserControlPrinter.PrintDocument<VM, V>(parameter as VM);
    }
}
