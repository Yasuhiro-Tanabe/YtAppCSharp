using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class FixPartsListCommand : CommandBase
    {
        public FixPartsListCommand() : base()
        {
            AddAction(typeof(BouquetDetailViewModel), FixBouquetPartsList);
            AddAction(typeof(SupplierDetailViewModel), FixSupplierPartsList);
        }

        private static void FixBouquetPartsList(object parameter) => (parameter as BouquetDetailViewModel).FixPartsList();
        private static void FixSupplierPartsList(object parameter) => (parameter as SupplierDetailViewModel).FixSupplierParts();
    }
}
