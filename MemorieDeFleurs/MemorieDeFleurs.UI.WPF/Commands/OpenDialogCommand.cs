using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenDialogCommand : CommandBase
    {
        public OpenDialogCommand() : base(typeof(IDialogCaller), Open) { }

        private static void Open(object parameter)
        {
            var dialog = new DialogWindow();
            dialog.DataContext = new DialogViewModel() { ViewModel = (parameter as IDialogCaller).DialogViewModel };
            dialog.ShowDialog();
        }
    }
}
