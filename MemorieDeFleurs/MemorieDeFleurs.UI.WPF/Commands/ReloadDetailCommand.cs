using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ReloadDetailCommand : CommandBase
    {
        public ReloadDetailCommand() : base()
        {
            AddAction(typeof(DetailViewModelBase), UpdateDetailView);
        }

        public static void UpdateDetailView(object parameter) => (parameter as DetailViewModelBase).Update();
    }
}
