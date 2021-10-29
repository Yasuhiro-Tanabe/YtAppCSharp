using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System.Windows.Controls;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class SelectionChangedEventCommand : CommandBase
    {
        public SelectionChangedEventCommand() : base(typeof(SelectionChangedEventArgs), ChangeCommandButtonVisibility) { }
        private static void ChangeCommandButtonVisibility(object parameter)
        {
            LogUtil.DEBUGLOG_BeginMethod(parameter == null ? "parameter=null" : $"parameter={parameter.GetType().Name}");
            var args = parameter as SelectionChangedEventArgs;

            if (args.RemovedItems.Count > 0)
            {
                foreach (var item in args.RemovedItems)
                {
                    if (item is BouquetPartsSummaryViewModel)
                    {
                        var vm = item as BouquetPartsSummaryViewModel;
                        vm.HideCommandButtons();
                    }
                    else if (item is BouquetSummaryViewModel)
                    {
                        var vm = item as BouquetSummaryViewModel;
                        vm.HideCommandButtons();
                    }
                    else if(item is ListItemViewModelBase)
                    {
                        (item as ListItemViewModelBase).HideCommandButtons();
                    }
                }
            }

            if (args.AddedItems.Count > 0)
            {
                foreach (var item in args.AddedItems)
                {
                    if (item is BouquetPartsSummaryViewModel)
                    {
                        var vm = item as BouquetPartsSummaryViewModel;
                        vm.ShowCommandButtons();
                        LogUtil.Debug($"{vm.PartsCode} selected.");
                    }
                    else if (item is BouquetSummaryViewModel)
                    {
                        var vm = item as BouquetSummaryViewModel;
                        vm.ShowCommandButtons();
                        LogUtil.Debug($"{vm.BouquetCode} selected.");
                    }
                    else if(item is ListItemViewModelBase)
                    {
                        var vm = item as ListItemViewModelBase;
                        vm.ShowCommandButtons();
                        LogUtil.Debug($"[{vm.GetType().Name}] {vm.Key} selected.");
                    }
                }
            }
            LogUtil.DEBUGLOG_EndMethod();
        }
    }
}
