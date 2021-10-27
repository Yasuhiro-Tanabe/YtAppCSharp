using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenBouquetListViewCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if(parameter is MainWindowViiewModel)
            {
                var vm = parameter as MainWindowViiewModel;
                vm.OpenTabItem(new BouquetListViewModel());
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
