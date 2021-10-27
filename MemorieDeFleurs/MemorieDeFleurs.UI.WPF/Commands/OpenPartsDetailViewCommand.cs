using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenPartsDetailViewCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if(parameter is MainWindowViiewModel)
            {
                var vm = parameter as MainWindowViiewModel;
                vm.OpenTabItem(new BouquetPartsDetailViewModel());
            }
            else if(parameter is BouquetPartsSummaryViewModel)
            {
                (parameter as BouquetPartsSummaryViewModel).OpenDetailView();
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
