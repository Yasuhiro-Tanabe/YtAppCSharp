using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class RemoveBouquetPartsFromPartsListCommand : CommandBase
    {
        public RemoveBouquetPartsFromPartsListCommand() : base(typeof(BouquetDetailViewModel), RemoveFromPatsList) { }

        private static void RemoveFromPatsList(object parameter) => (parameter as BouquetDetailViewModel).RemoveFromPartsList();
    }
}
