using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ReloadDetailCommand : CommandBase
    {
        public ReloadDetailCommand() : base()
        {
            AddAction(typeof(BouquetPartsDetailViewModel), UpdateBouquetPartsDetailView);
            AddAction(typeof(BouquetDetailViewModel), UpdateBouquetDetailView);
            AddAction(typeof(SupplierDetailViewModel), UpdateSupplierDetailView);
        }

        private static void UpdateBouquetPartsDetailView(object parameter) => (parameter as BouquetPartsDetailViewModel).Update();
        private static void UpdateBouquetDetailView(object parameter) => (parameter as BouquetDetailViewModel).Update();
        private static void UpdateSupplierDetailView(object parameter) => (parameter as SupplierDetailViewModel).Update();
    }
}
