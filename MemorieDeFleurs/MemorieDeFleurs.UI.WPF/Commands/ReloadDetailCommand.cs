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
            AddAction(typeof(CustomerDetailViewModel), UpdateDetailView);
        }

        public static void UpdateDetailView(object parameter) => (parameter as DetailViewModelBase).Update();
    }
}
