using MemorieDeFleurs.UI.WPF.ViewModels;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ChangeInspectionDateCommand : CommandBase<OrderToSupplierInspectionListViewModel>
    {
        protected override void Execute(OrderToSupplierInspectionListViewModel parameter) => parameter.ReloadOrders();
    }
}
