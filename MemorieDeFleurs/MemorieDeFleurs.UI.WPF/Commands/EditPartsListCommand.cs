using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class EditPartsListCommand : CommandBase
    {
        public EditPartsListCommand() : base()
        {
            AddAction(typeof(BouquetDetailViewModel), EditBouquetPartsList);
            AddAction(typeof(SupplierDetailViewModel), EditSupplierPartsList);
        }

        private static void EditBouquetPartsList(object parameter) => (parameter as BouquetDetailViewModel).EditPartsList();
        private static void EditSupplierPartsList(object parameter) => (parameter as SupplierDetailViewModel).EditSupplierParts();
    }
}
