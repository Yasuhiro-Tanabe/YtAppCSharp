using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System.Windows.Controls;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class SelectionChangedEventHandler : CommandBase
    {
        public SelectionChangedEventHandler() : base()
        {
            AddAction(typeof(SelectionChangedEventArgs), ChangeCommandButtonVisibility);
            AddAction(typeof(OrderToSupplierListViewModel), ReloadItems);
        }

        private static void ChangeCommandButtonVisibility(object parameter)
        {
            LogUtil.DEBUGLOG_BeginMethod(parameter == null ? "parameter=null" : $"parameter={parameter.GetType().Name}");
            var args = parameter as SelectionChangedEventArgs;

            if (args.RemovedItems.Count > 0)
            {
                foreach (var item in args.RemovedItems)
                {
                    if (item is ListItemViewModelBase)
                    {
                        (item as ListItemViewModelBase).IsActionVisible = false;
                    }
                    if(item is InventorySummaryViewModel)
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
                    if(item is InventorySummaryViewModel)
                    {
                        (item as InventorySummaryViewModel).IsSelected = true;
                    }
                }
            }
            LogUtil.DEBUGLOG_EndMethod();
        }

        private static void ReloadItems(object parameter) => (parameter as OrderToSupplierListViewModel).UpdateProperties();
    }
}
