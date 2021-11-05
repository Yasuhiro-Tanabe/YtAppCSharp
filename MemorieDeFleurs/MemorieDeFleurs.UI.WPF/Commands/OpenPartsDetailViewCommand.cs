using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenPartsDetailViewCommand : CommandBase
    {
        public OpenPartsDetailViewCommand() : base()
        {
            AddAction(typeof(MainWindowViiewModel), OpenTabItem);
            AddAction(typeof(BouquetPartsSummaryViewModel), OpenDetailView);
        }

        private static void OpenTabItem(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new BouquetPartsDetailViewModel());
        private static void OpenDetailView(object parameter) => (parameter as BouquetPartsSummaryViewModel).OpenDetailView();
    }
}
