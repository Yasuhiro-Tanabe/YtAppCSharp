using MemorieDeFleurs.UI.WPF.ViewModels;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ShippingOrdersCommand : CommandBase<ProcessingInstructionViewModel>
    {
        protected override void Execute(ProcessingInstructionViewModel parameter) => parameter.ShipBouquets();
    }
}
