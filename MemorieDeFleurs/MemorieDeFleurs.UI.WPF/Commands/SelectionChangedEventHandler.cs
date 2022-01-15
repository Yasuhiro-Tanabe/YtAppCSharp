using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System.Windows.Controls;

using YasT.Framework.Logging;
using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class SelectionChangedEventHandler : CommandBase<SelectionChangedEventArgs>
    {
        protected override void Execute(SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count > 0)
            {
                foreach (var item in args.RemovedItems)
                {
                    if (item is ListItemViewModelBase)
                    {
                        (item as ListItemViewModelBase).IsActionVisible = false;
                    }
                    if (item is InventorySummaryViewModel)
                    {
                        (item as InventorySummaryViewModel).IsSelected = false;
                    }
                }
            }

            if (args.AddedItems.Count > 0)
            {
                foreach (var item in args.AddedItems)
                {
                    if (item is ListItemViewModelBase)
                    {
                        var vm = item as ListItemViewModelBase;
                        vm.IsActionVisible = true;
                        LogUtil.Debug($"[{vm.GetType().Name}] {vm.Key} selected.");
                    }
                    if (item is InventorySummaryViewModel)
                    {
                        (item as InventorySummaryViewModel).IsSelected = true;
                    }
                }
            }
        }
    }
}
