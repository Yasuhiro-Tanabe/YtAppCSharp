using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;

using System;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class DialogOkCommand : CommandBase
    {
        public event EventHandler DialogClosing;

        public DialogOkCommand() : base()
        {
            AddAction(typeof(DialogWindow), Ok);
        }

        private void Ok(object parameter)
        {
            var dialog = parameter as DialogWindow;
            if(dialog != null)
            {
                (dialog.DataContext as DialogViewModel).DialogUser.DialogOK();
                DialogClosing?.Invoke(this, null);
                dialog.Close();
            }
        }
    }
}
