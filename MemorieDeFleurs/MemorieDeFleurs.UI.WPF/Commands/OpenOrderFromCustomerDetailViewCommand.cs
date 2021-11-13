using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenOrderFromCustomerDetailViewCommand : CommandBase
    {
        public OpenOrderFromCustomerDetailViewCommand()
        {
            AddAction(typeof(MainWindowViiewModel), OpenTabItem);
            AddAction(typeof(ListItemViewModelBase), OpenDetailView);
        }

        private static void OpenTabItem(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new OrderFromCustomerDetailViewModel());
        private static void OpenDetailView(object parameter) => (parameter as ListItemViewModelBase).OpenDetailView();
    }
}
