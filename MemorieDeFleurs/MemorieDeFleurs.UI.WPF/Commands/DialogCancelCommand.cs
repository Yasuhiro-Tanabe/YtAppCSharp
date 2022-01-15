using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;

using System;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class DialogCancelCommand : CommandBase<DialogWindow>
    {
        public event EventHandler DialogCloing;

        protected override void Execute(DialogWindow dialog)
        {
            (dialog.DataContext as DialogViewModel).DialogUser.DialogCancel();
            DialogCloing?.Invoke(this, null);
            dialog.Close();
        }
    }
}
