using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ChangeShippingAddressCommand : CommandBase
    {
        public ChangeShippingAddressCommand() : base(typeof(OrderFromCustomerDetailViewModel), ChangeShippingAddress) { }
        private static void ChangeShippingAddress(object parameter) => (parameter as OrderFromCustomerDetailViewModel).LoadShippingAddresses();
    }
}
