using MemorieDeFleurs.UI.WPF.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ReloadDetailCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if(parameter is BouquetPartsDetailViewModel)
            {
                (parameter as BouquetPartsDetailViewModel).Update();
            }
            else if(parameter is BouquetDetailViewModel)
            {
                (parameter as BouquetDetailViewModel).Update();
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
