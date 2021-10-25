﻿using MemorieDeFleurs.UI.WPF.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ReloadListCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if(parameter is BouquetPartsListViewModel)
            {
                (parameter as BouquetPartsListViewModel).LoadBouquetParts();
            }
            else if(parameter is BouquetListViewModel)
            {
                (parameter as BouquetListViewModel).LoadBouquets();
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
