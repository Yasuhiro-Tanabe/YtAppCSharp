using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenBouquetListViewCommand : CommandBase
    {
        public OpenBouquetListViewCommand() : base(typeof(MainWindowViiewModel), OpenTabItem) { }

        private static void OpenTabItem(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new BouquetListViewModel());
    }
}
