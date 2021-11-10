using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ChangeDateCommand : CommandBase
    {
        public ChangeDateCommand() : base()
        {
            AddAction(typeof(OrderToSupplierDetailViewModel), ChangeArrivalDate);
            AddAction(typeof(OrderFromCustomerDetailViewModel), ChangeShippingDate);
        }

        private static void ChangeArrivalDate(object parameter) => (parameter as OrderToSupplierDetailViewModel).ChangeMyArrivalDate();
        private static void ChangeShippingDate(object parameter) => (parameter as OrderFromCustomerDetailViewModel).ChangeMyArrivalDate();
    }
}
