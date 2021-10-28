using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class DeleteFromDatabase : CommandBase
    {
        public DeleteFromDatabase() : base()
        {
            AddAction(typeof(BouquetPartsSummaryViewModel), RemoveBouquetParts);
            AddAction(typeof(BouquetSummaryViewModel), RemoveBouquet);
            AddAction(typeof(SupplierSummaryViewModel), RemoveSupplier);
        }

        private static void RemoveBouquetParts(object parameter) => (parameter as BouquetPartsSummaryViewModel).RemoveMe();
        private static void RemoveBouquet(object parameter) => (parameter as BouquetSummaryViewModel).RemoveMe();
        private static void RemoveSupplier(object parameter) => (parameter as SupplierSummaryViewModel).RemoveMe();
    }
}
