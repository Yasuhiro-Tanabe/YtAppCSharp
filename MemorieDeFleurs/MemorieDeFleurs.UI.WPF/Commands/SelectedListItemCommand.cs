using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class SelectedListItemCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            LogUtil.DEBUGLOG_BeginMethod(parameter == null ? "parameter=null" : $"parameter={parameter.GetType().Name}");
            var args = parameter as SelectionChangedEventArgs;

            if (args.RemovedItems.Count > 0)
            {
                foreach(var item in args.RemovedItems)
                {
                    if(item is BouquetPartsSummaryViewModel)
                    {
                        var vm = item as BouquetPartsSummaryViewModel;
                        vm.HideCommandButtons();
                    }
                    else if(item is BouquetSummaryViewModel)
                    {
                        var vm = item as BouquetSummaryViewModel;
                        vm.HideCommandButtons();
                    }
                }
            }

            if (args.AddedItems.Count > 0)
            {
                foreach(var item in args.AddedItems)
                {
                    if(item is BouquetPartsSummaryViewModel)
                    {
                        var vm = item as BouquetPartsSummaryViewModel;
                        vm.ShowCommandButtons();
                        LogUtil.Debug($"{vm.PartsCode} selected.");
                    }
                    else if(item is BouquetSummaryViewModel)
                    {
                        var vm = item as BouquetSummaryViewModel;
                        vm.ShowCommandButtons();
                        LogUtil.Debug($"{vm.BouquetCode} selected.");
                    }
                }
            }
            LogUtil.DEBUGLOG_EndMethod();
        }
    }
}
