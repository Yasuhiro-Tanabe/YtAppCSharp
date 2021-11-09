using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class EditPartsListCommand : CommandBase
    {
        public EditPartsListCommand() : base()
        {
            AddAction(typeof(BouquetDetailViewModel), EditBouquetPartsList);
            AddAction(typeof(SupplierDetailViewModel), EditSupplierPartsList);
            AddAction(typeof(OrderToSupplierDetailViewModel), EditOrderPartsList);
            AddAction(typeof(OrderFromCustomerDetailViewModel), SelectFromList);
        }

        private static void EditBouquetPartsList(object parameter) => (parameter as BouquetDetailViewModel).EditPartsList();
        private static void EditSupplierPartsList(object parameter) => (parameter as SupplierDetailViewModel).EditSupplierParts();

        private static void EditOrderPartsList(object parameter) => (parameter as OrderToSupplierDetailViewModel).EditOrderParts();
        private static void SelectFromList(object parameter) => (parameter as OrderFromCustomerDetailViewModel).OpenShippingAddressList();
    }
}
