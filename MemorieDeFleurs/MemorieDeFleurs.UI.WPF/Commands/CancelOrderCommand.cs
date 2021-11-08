using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class CancelOrderCommand : CommandBase
    {
        public CancelOrderCommand() : base(typeof(OrderToSupplierDetailViewModel), OrderToSupplier) { }

        private static void OrderToSupplier(object parameter) => (parameter as OrderToSupplierDetailViewModel).CancelMe();
    }
}
