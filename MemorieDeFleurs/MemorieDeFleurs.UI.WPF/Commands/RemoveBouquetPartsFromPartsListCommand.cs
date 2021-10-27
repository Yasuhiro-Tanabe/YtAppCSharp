using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class RemoveBouquetPartsFromPartsListCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if(parameter is BouquetDetailViewModel)
            {
                (parameter as BouquetDetailViewModel).RemoveFromPartsList();
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
