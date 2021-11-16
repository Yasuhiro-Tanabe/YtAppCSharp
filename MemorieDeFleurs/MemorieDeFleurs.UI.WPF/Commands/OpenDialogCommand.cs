using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenDialogCommand : CommandBase
    {
        public OpenDialogCommand() : base()
        {
            AddAction(typeof(IDialogUser), Open);
            AddAction(typeof(OrderToSupplierSummaryViewModel), OpeOrderToSupplierPrintPreviewDialog);
        }

        private static void Open(object parameter)
        {
            var dialog = new DialogWindow();
            if(dialog.DataContext == null)
            {
                dialog.DataContext = new DialogViewModel();
            }

            (dialog.DataContext as DialogViewModel).ViewModel = parameter as NotificationObject;
            dialog.ShowDialog();
        }

        private static void OpeOrderToSupplierPrintPreviewDialog(object parameter)
        {
            var dialog = new DialogWindow();
            if (dialog.DataContext == null)
            {
                dialog.DataContext = new DialogViewModel();
            }

            var summaryVM = parameter as OrderToSupplierSummaryViewModel;
            var detailVM = new OrderToSupplierDetailViewModel() { OrderNo = summaryVM.OrderNo };
            detailVM.Update();

            (dialog.DataContext as DialogViewModel).ViewModel = detailVM;
            dialog.ShowDialog();
        }
    }
}
