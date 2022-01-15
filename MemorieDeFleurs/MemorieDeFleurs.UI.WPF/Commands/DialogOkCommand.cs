using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;

using System;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class DialogOkCommand : CommandBase<DialogWindow>
    {
        public event EventHandler DialogClosing;

        protected override void Execute(DialogWindow dialog)
        {
            (dialog.DataContext as DialogViewModel).DialogUser.DialogOK();
            DialogClosing?.Invoke(this, null);
            dialog.Close();
        }
    }
}
