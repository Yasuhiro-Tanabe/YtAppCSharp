﻿using MemorieDeFleurs.UI.WPF.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class RemoveBouquetPartsFromPartsListCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if(parameter is BouquetDetailViewModel)
            {
                (parameter as BouquetDetailViewModel).RemoveFromPartsList();
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
