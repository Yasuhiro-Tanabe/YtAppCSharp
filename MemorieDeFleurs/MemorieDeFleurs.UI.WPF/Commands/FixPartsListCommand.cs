using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class FixPartsListCommand : CommandBase
    {
        public FixPartsListCommand() : base()
        {
            AddAction(typeof(BouquetDetailViewModel), FixBouquetPartsList);
            AddAction(typeof(SupplierDetailViewModel), FixSupplierPartsList);
            AddAction(typeof(OrderToSupplierDetailViewModel), null);
        }

        private static void FixBouquetPartsList(object parameter) => (parameter as BouquetDetailViewModel).FixPartsList();
        private static void FixSupplierPartsList(object parameter) => (parameter as SupplierDetailViewModel).FixSupplierParts();

        private static void FixOrderPartsList(object parameter) => (parameter as OrderToSupplierDetailViewModel).FixOrderParts();
    }
}
