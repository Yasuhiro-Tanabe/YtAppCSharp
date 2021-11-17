using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ReloadListCommand : CommandBase
    {
        public ReloadListCommand() : base(typeof(IReloadable), LoadItems) { }

        private static void LoadItems(object parameter) => (parameter as IReloadable).UpdateProperties();
    }
}
