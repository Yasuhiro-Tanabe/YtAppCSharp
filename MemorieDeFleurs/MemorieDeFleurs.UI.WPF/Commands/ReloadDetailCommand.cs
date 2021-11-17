using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ReloadDetailCommand : CommandBase
    {
        public ReloadDetailCommand() : base()
        {
            AddAction(typeof(IReloadable), UpdateDetailView);
            //AddAction(typeof(DialogViewModel), UpdateDialog);
        }

        private static void UpdateDetailView(object parameter) => (parameter as IReloadable).UpdateProperties();
        //private static void UpdateDialog(object parameter) => (parameter as DialogViewModel).UpdateDialogParameter();
    }
}
