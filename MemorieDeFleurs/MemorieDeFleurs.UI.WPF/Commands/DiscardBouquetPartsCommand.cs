using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class DiscardBouquetPartsCommand : CommandBase
    {
        public DiscardBouquetPartsCommand() : base(typeof(InventoryViewModel), Discard) { }

        private static void Discard(object parameter) => (parameter as InventoryViewModel).DiscardBouquetParts();
    }
}
