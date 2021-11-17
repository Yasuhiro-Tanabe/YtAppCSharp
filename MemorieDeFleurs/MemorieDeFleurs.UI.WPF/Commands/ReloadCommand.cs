using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ReloadCommand : CommandBase
    {
        public ReloadCommand() : base()
        {
            AddAction(typeof(IReloadable), UpdateDetailView);
        }

        private static void UpdateDetailView(object parameter) => (parameter as IReloadable).UpdateProperties();
    }
}
