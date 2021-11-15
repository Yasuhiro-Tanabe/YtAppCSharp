using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ReloadDetailCommand : CommandBase
    {
        public ReloadDetailCommand() : base()
        {
            AddAction(typeof(DetailViewModelBase), UpdateDetailView);
            AddAction(typeof(DialogViewModel), UpdateDialog);
        }

        public static void UpdateDetailView(object parameter) => (parameter as DetailViewModelBase).Update();
        private static void UpdateDialog(object parameter) => (parameter as DialogViewModel).UpdateDialogParameter();
    }
}
