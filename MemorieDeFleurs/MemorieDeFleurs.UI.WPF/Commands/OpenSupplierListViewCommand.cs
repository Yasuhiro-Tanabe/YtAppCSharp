using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenSupplierListViewCommand : CommandBase
    {
        public OpenSupplierListViewCommand() : base(typeof(MainWindowViiewModel), Open) { }

        private static void Open(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new SupplierListViewModel());
    }
}
