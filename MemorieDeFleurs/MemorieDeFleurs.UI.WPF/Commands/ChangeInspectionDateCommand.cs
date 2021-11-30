using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ChangeInspectionDateCommand : CommandBase
    {
        public ChangeInspectionDateCommand() : base(typeof(OrderToSupplierInspectionListViewModel), Reload) { }

        private static void Reload(object parameter) => (parameter as OrderToSupplierInspectionListViewModel).ReloadOrders();
    }
}
