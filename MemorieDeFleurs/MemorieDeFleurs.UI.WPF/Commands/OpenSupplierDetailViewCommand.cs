using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenSupplierDetailViewCommand : CommandBase
    {
        public OpenSupplierDetailViewCommand() : base()
        {
            AddAction(typeof(MainWindowViiewModel), OpenTabItem);
            AddAction(typeof(ListItemViewModelBase), OpenSupplierDetailView);
        }

        private static void OpenTabItem(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new SupplierDetailViewModel());
        private static void OpenSupplierDetailView(object parameter) => (parameter as ListItemViewModelBase).OpenDetailView();
    }
}
