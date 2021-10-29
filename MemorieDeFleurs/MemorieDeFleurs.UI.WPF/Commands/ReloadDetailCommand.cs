using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ReloadDetailCommand : CommandBase
    {
        public ReloadDetailCommand() : base()
        {
            AddAction(typeof(BouquetPartsDetailViewModel), UpdateDetailView);
            AddAction(typeof(BouquetDetailViewModel), UpdateDetailView);
            AddAction(typeof(SupplierDetailViewModel), UpdateDetailView);
        }

        private static void UpdateBouquetPartsDetailView(object parameter) => (parameter as BouquetPartsDetailViewModel).Update();
        private static void UpdateBouquetDetailView(object parameter) => (parameter as BouquetDetailViewModel).Update();
        private static void UpdateSupplierDetailView(object parameter) => (parameter as SupplierDetailViewModel).Update();

        public static void UpdateDetailView(object parameter) => (parameter as DetailViewModelBase).Update();
    }
}
