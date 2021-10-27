﻿using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenBouquetDetailViewCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if (parameter is MainWindowViiewModel)
            {
                var vm = parameter as MainWindowViiewModel;
                vm.OpenTabItem(new BouquetDetailViewModel());
            }
            else if(parameter is BouquetSummaryViewModel)
            {
                (parameter as BouquetSummaryViewModel).OpenDetailView();
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}