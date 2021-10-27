using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class RemoveListItemCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if(parameter is BouquetPartsSummaryViewModel)
            {
                (parameter as BouquetPartsSummaryViewModel).RemoveMe();
            }
            else if(parameter is BouquetSummaryViewModel)
            {
                (parameter as BouquetSummaryViewModel).RemoveMe();
            }
        }
    }
}
