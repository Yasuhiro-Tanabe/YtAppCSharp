using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class RemoveFromListItemCommand : CommandBase
    {
        public RemoveFromListItemCommand() : base(typeof(BouquetDetailViewModel), RemoveFromPatsList) { }

        private static void RemoveFromPatsList(object parameter) => (parameter as BouquetDetailViewModel).RemoveFromPartsList();
    }
}
