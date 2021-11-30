using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class InventoryTransitionTableChangedCommand : CommandBase
    {
        public InventoryTransitionTableChangedCommand() : base(typeof(InventoryTransitionTableViewModel), UpdateTable) { }

        private static void UpdateTable(object parameter) => (parameter as InventoryTransitionTableViewModel).UpdateProperties();
    }
}
