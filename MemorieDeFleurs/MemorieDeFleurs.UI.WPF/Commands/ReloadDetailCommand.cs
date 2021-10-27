using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ReloadDetailCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if(parameter is BouquetPartsDetailViewModel)
            {
                (parameter as BouquetPartsDetailViewModel).Update();
            }
            else if(parameter is BouquetDetailViewModel)
            {
                (parameter as BouquetDetailViewModel).Update();
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
