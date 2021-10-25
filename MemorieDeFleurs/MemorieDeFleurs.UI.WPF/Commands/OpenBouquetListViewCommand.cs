using MemorieDeFleurs.UI.WPF.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenBouquetListViewCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            if(parameter is MainWindowViiewModel)
            {
                var vm = parameter as MainWindowViiewModel;
                vm.OpenTabItem(new BouquetListViewModel());
            }
            else
            {
                base.Execute(parameter);
            }
        }
    }
}
