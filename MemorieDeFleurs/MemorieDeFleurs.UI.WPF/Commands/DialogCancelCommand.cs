using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;

using System;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class DialogCancelCommand : CommandBase
    {
        public event EventHandler DialogCloing;

        public DialogCancelCommand() : base()
        {
            AddAction(typeof(DialogWindow), Cancel);
        }

        private void Cancel(object parameter)
        {
            var dialog = parameter as DialogWindow;
            if(dialog != null)
            {
                (dialog.DataContext as DialogViewModel).DialogUser.DialogCancel();
                DialogCloing?.Invoke(this, null);
                dialog.Close();
            }
        }
    }
}
