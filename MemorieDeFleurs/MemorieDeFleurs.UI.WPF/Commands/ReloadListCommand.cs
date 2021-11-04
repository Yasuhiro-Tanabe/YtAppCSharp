using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ReloadListCommand : CommandBase
    {
        public ReloadListCommand() : base(typeof(ListViewModelBase), LoadItems) { }

        private static void LoadItems(object parameter) => (parameter as ListViewModelBase).LoadItems();
    }
}
