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
                vm.AddViewModel(new BouquetPartsDetailViewModel());
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
