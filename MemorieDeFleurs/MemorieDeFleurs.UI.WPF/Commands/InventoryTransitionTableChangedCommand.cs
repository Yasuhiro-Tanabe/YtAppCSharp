using MemorieDeFleurs.UI.WPF.ViewModels;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class InventoryTransitionTableChangedCommand : CommandBase<InventoryTransitionTableViewModel>
    {
        protected override void Execute(InventoryTransitionTableViewModel parameter) => parameter.UpdateProperties();
    }
}
