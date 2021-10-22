using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    class OpenPartListViewCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if (parameter is MainWindowViiewModel)
            {
                var vm = parameter as MainWindowViiewModel;
                vm.OpenTabItem(new BouquetPartsListViewModel());
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
