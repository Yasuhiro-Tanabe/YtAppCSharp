using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    public class OpenDetailViewCommand<VM> : CommandBase where VM : DetailViewModelBase, new()
    {
        public OpenDetailViewCommand() : base()
        {
            AddAction(typeof(MainWindowViiewModel), OpenTabItem);
            AddAction(typeof(ListItemViewModelBase), OpenDetailView);
        }
        
        private static void OpenTabItem(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new VM());
        private static void OpenDetailView(object parameter) => (parameter as ListItemViewModelBase).OpenDetailView();
    }
}
