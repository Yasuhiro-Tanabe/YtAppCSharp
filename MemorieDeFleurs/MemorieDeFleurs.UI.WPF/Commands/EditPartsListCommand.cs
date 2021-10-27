using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class EditPartsListCommand : CommandBase
    {
        public EditPartsListCommand() : base(typeof(BouquetDetailViewModel), EditPartsList) { }

        private static void EditPartsList(object parameter) => (parameter as BouquetDetailViewModel).EditPartsList();
    }
}
