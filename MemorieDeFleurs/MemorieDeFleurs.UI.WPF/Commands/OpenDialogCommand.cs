using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenDialogCommand : CommandBase
    {
        public OpenDialogCommand() : base(typeof(IDialogUser), Open) { }

        private static void Open(object parameter)
        {
            var dialog = new DialogWindow();
            if(dialog.DataContext == null)
            {
                dialog.DataContext = new DialogViewModel();
            }

            (dialog.DataContext as DialogViewModel).ViewModel = parameter as IDialogUser;
            dialog.ShowDialog();
        }
    }
}
