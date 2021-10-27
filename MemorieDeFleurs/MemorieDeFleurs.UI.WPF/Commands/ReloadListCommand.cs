using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ReloadListCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if(parameter is BouquetPartsListViewModel)
            {
                (parameter as BouquetPartsListViewModel).LoadBouquetParts();
            }
            else if(parameter is BouquetListViewModel)
            {
                (parameter as BouquetListViewModel).LoadBouquets();
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
