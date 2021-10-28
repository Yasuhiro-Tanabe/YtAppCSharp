using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenSupplierDetailViewCommand : CommandBase
    {
        public OpenSupplierDetailViewCommand() : base()
        {
            AddAction(typeof(MainWindowViiewModel), OpenTabItem);
            AddAction(typeof(SupplierSummaryViewModel), OpenSupplierDetailView);
        }

        private static void OpenTabItem(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new SupplierDetailViewModel());
        private static void OpenSupplierDetailView(object parameter) => (parameter as SupplierSummaryViewModel).OpenDetailView();
    }
}
