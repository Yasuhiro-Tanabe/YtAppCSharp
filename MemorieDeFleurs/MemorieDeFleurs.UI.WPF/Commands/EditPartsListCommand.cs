using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class EditPartsListCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if (parameter is BouquetDetailViewModel)
            {
                (parameter as BouquetDetailViewModel).EditPartsList();
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
