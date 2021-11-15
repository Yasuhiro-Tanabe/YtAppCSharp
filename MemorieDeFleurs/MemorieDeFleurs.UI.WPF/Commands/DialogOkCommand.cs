using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class DialogOkCommand : CommandBase
    {
        public DialogOkCommand() : base(typeof(DialogWindow), Ok) { }

        private static void Ok(object parameter)
        {
            var dialog = parameter as DialogWindow;
            if(dialog != null)
            {
                (dialog.DataContext as DialogViewModel).ViewModel.DialogOK();
                dialog.Close();
            }
        }
    }
}
