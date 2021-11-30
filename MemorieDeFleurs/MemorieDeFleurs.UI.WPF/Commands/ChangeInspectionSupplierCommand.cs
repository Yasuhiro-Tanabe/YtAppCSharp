using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ChangeInspectionSupplierCommand : CommandBase
    {
        public ChangeInspectionSupplierCommand() : base(typeof(OrderToSupplierInspectionListViewModel), ChangeSupplier) { }
        private static void ChangeSupplier(object parameter) => (parameter as OrderToSupplierInspectionListViewModel).ReloadOrders();
    }
}
