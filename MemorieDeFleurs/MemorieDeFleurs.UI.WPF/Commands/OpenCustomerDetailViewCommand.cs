using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenCustomerDetailViewCommand : CommandBase
    {
        public OpenCustomerDetailViewCommand() : base()
        {
            AddAction(typeof(MainWindowViiewModel), OpenTabItem);
            AddAction(typeof(CustomerSummaryViewModel), OpenDetailView);
        }

        private static void OpenTabItem(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new CustomerDetailViewModel());
        private static void OpenDetailView(object parameter) => (parameter as CustomerSummaryViewModel).OpenDetailView();
    }
}
