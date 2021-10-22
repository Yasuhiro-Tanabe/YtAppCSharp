using MemorieDeFleurs.UI.WPF.ViewModels;

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
            var vm = parameter as BouquetPartsListViewModel;
            if(vm != null)
            {
                vm.LoadBouquetParts();
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
