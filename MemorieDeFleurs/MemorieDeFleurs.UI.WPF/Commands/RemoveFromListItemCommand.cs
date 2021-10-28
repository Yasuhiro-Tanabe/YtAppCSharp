using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class RemoveFromListItemCommand : CommandBase
    {
        public RemoveFromListItemCommand() : base()
        {
            AddAction(typeof(BouquetDetailViewModel), RemoveFromBouquetPatsList);
            AddAction(typeof(SupplierDetailViewModel), RemoveFromSupplierPartsList);
        }

        private static void RemoveFromBouquetPatsList(object parameter) => (parameter as BouquetDetailViewModel).RemoveFromPartsList();
        private static void RemoveFromSupplierPartsList(object parameter) => (parameter as SupplierDetailViewModel).RemoveFromSupplingParts();
    }
}
