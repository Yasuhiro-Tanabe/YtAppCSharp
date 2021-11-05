using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenCustomerListViewCommand : CommandBase
    {
        public OpenCustomerListViewCommand() : base(typeof(MainWindowViiewModel), OpenTabItem) { }

        private static void OpenTabItem(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new CustomerListViewModel());
    }
}
