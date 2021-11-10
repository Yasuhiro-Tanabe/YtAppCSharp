using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class CancelOrderCommand : CommandBase
    {
        public CancelOrderCommand() : base()
        {
            AddAction(typeof(OrderToSupplierDetailViewModel), CancelOrderToSupplier);
            AddAction(typeof(OrderFromCustomerDetailViewModel), CancelOrderFromCustomer);
        }

        private static void CancelOrderToSupplier(object parameter) => (parameter as OrderToSupplierDetailViewModel).CancelMe();
        private static void CancelOrderFromCustomer(object parameter) => (parameter as OrderFromCustomerDetailViewModel).CancelMe();
    }
}
