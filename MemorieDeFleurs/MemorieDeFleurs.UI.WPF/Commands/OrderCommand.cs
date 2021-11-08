using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OrderCommand : CommandBase
    {
        public OrderCommand() : base(typeof(OrderToSupplierDetailViewModel), OrderToSupplier) { }

        private static void OrderToSupplier(object parameter) => (parameter as OrderToSupplierDetailViewModel).OrderMe();
    }
}
