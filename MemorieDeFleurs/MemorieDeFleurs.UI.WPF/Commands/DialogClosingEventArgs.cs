using System;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    public enum DialogClosingStatus
    {
        OK,
        CANCEL
    }

    public class DialogClosingEventArgs : EventArgs
    {
        public DialogClosingStatus Status { get; private set; }
        public NotificationObject DialogViewModel { get; private set; }

        public DialogClosingEventArgs(DialogClosingStatus status, NotificationObject viewModel)
        {
            Status = status;
            DialogViewModel = viewModel;
        }
    }
}
