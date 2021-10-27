using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class FixPartsListCommand : CommandBase
    {
        public FixPartsListCommand() : base(typeof(BouquetDetailViewModel), FixPartsList) { }

        private static void FixPartsList(object parameter) => (parameter as BouquetDetailViewModel).FixPartsList();
    }
}
