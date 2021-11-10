using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OrderCommand : CommandBase
    {
        public OrderCommand() : base()
        {
            AddAction(typeof(OrderToSupplierDetailViewModel), OrderToSupplier);
            AddAction(typeof(OrderFromCustomerDetailViewModel), OrderFromCustomer);
        }


        private static void OrderToSupplier(object parameter) => (parameter as OrderToSupplierDetailViewModel).OrderMe();
        private static void OrderFromCustomer(object parameter) => (parameter as OrderFromCustomerDetailViewModel).OrderMe();
    }
}
