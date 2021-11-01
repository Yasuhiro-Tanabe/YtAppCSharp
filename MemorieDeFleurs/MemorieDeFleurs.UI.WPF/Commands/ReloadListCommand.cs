using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ReloadListCommand : CommandBase
    {
        public ReloadListCommand() : base()
        {
            AddAction(typeof(BouquetPartsListViewModel), LoadItems);
            AddAction(typeof(BouquetListViewModel), LoadItems);
            AddAction(typeof(SupplierListViewModel), LoadItems);
            AddAction(typeof(CustomerListViewModel), LoadItems);
        }

        private static void LoadItems(object parameter) => (parameter as ListViewModelBase).LoadItems();
    }
}
