using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ReloadListCommand : CommandBase
    {
        public ReloadListCommand() : base()
        {
            AddAction(typeof(BouquetPartsListViewModel), ReloadBouquetPartsList);
            AddAction(typeof(BouquetListViewModel), ReloadBouquetList);
        }   

        private static void ReloadBouquetPartsList(object parameter) => (parameter as BouquetPartsListViewModel).LoadBouquetParts();
        private static void ReloadBouquetList(object parameter) => (parameter as BouquetListViewModel).LoadBouquets();
    }
}
