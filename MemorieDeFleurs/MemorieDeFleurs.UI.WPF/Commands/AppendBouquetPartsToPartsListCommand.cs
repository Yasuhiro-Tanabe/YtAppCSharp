using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class AppendBouquetPartsToPartsListCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if (parameter is BouquetDetailViewModel)
            {
                (parameter as BouquetDetailViewModel).AppendToPartsList();
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
