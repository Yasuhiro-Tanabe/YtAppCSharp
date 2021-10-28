using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class AddToListItemCommand : CommandBase
    {
        public AddToListItemCommand() : base()
        {
            AddAction(typeof(BouquetDetailViewModel), AppendToBouquetPartsList);
            AddAction(typeof(SupplierDetailViewModel), AppendToSupplierPartsList);
        }

        private static void AppendToBouquetPartsList(object parameter) => (parameter as BouquetDetailViewModel).AppendToPartsList();
        private static void AppendToSupplierPartsList(object parameter) => (parameter as SupplierDetailViewModel).AppnedToSupplingParts();
    }
}
