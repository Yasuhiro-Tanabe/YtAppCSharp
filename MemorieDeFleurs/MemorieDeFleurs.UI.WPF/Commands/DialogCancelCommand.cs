using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class DialogCancelCommand : CommandBase
    {
        public DialogCancelCommand() : base(typeof(DialogWindow), Cancel) { }

        private static void Cancel(object parameter)
        {
            var dialog = parameter as DialogWindow;
            if(dialog != null)
            {
                (dialog.DataContext as DialogViewModel).ViewModel.DialogCancel();
                dialog.Close();
            }
        }
    }
}
