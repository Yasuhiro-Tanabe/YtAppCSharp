using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class DeleteFromDatabase : CommandBase
    {
        public DeleteFromDatabase() : base()
        {
            AddAction(typeof(BouquetPartsSummaryViewModel), RemoveBouquetParts);
            AddAction(typeof(BouquetSummaryViewModel), RemoveBouquet);
        }

        private static void RemoveBouquetParts(object parameter) => (parameter as BouquetPartsSummaryViewModel).RemoveMe();
        private static void RemoveBouquet(object parameter) => (parameter as BouquetSummaryViewModel).RemoveMe();
    }
}
