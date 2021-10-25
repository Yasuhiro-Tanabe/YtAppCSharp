using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class FixPartsListCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if(parameter is BouquetDetailViewModel)
            {
                (parameter as BouquetDetailViewModel).FixPartsList();
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
