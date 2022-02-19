using MemorieDeFleurs.UI.WPF.ViewModels;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ChangeShippingAddressCommand : CommandBase<OrderFromCustomerDetailViewModel>
    {
        protected override void Execute(OrderFromCustomerDetailViewModel parameter) => parameter.LoadShippingAddresses();
    }
}
