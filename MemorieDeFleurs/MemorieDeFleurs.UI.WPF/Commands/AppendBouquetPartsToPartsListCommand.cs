using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class AppendBouquetPartsToPartsListCommand : CommandBase
    {
        public AppendBouquetPartsToPartsListCommand() : base(typeof(BouquetDetailViewModel), AppendToPartsList) { }

        private static void AppendToPartsList(object parameter) => (parameter as BouquetDetailViewModel).AppendToPartsList();
    }
}
