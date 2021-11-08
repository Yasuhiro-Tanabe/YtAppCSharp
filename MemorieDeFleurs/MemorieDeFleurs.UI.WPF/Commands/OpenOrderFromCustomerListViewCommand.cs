using MemorieDeFleurs.UI.WPF.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenOrderFromCustomerListViewCommand : CommandBase
    {
        public OpenOrderFromCustomerListViewCommand() : base(typeof(MainWindowViiewModel), OpenTabItem) { }

        private static void OpenTabItem(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new OrderFromCustomerListViewModel());
    }
}
