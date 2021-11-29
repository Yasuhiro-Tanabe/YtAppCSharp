using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    public class OpenListViewCommand<VM> : CommandBase where VM : ListViewModelBase, new()
    {
        public OpenListViewCommand() : base(typeof(MainWindowViiewModel), OpenTabItem) {}

        private static void OpenTabItem(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new VM());
    }
}
