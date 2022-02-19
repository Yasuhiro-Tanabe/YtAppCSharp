using MemorieDeFleurs.UI.WPF.ViewModels;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class DiscardBouquetPartsCommand : CommandBase<InventoryViewModel>
    {
        protected override void Execute(InventoryViewModel parameter) => parameter.DiscardBouquetParts();
    }
}
