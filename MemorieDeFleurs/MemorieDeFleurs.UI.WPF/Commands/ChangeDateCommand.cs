using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ChangeDateCommand : CommandBase
    {
        public ChangeDateCommand() : base(typeof(OrderToSupplierDetailViewModel), ChangeArrivalDate) { }

        private static void ChangeArrivalDate(object parameter) => (parameter as OrderToSupplierDetailViewModel).ChangeMyArrivalDate();
    }
}
