using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenOrderToSupplierDetailViewCommand : CommandBase
    {
        public OpenOrderToSupplierDetailViewCommand() : base()
        {
            AddAction(typeof(MainWindowViiewModel), OpenTabItem);
            AddAction(typeof(OrderToSupplierSummaryViewModel), OpenDetailView);
        }

        private static void OpenTabItem(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new OrderToSupplierDetailViewModel());
        private static void OpenDetailView(object parameter) => (parameter as ListItemViewModelBase).OpenDetailView();
    }
}
