using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenBouquetDetailViewCommand : CommandBase
    {
        public OpenBouquetDetailViewCommand() : base()
        {
            AddAction(typeof(MainWindowViiewModel), OpenTabItem);
            AddAction(typeof(BouquetSummaryViewModel), OpenBouquetDetailView);
        }

        private static void OpenTabItem(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new BouquetDetailViewModel());

        private static void OpenBouquetDetailView(object parameter) => (parameter as BouquetSummaryViewModel).OpenDetailView();
    }
}
