using MemorieDeFleurs.UI.WPF.ViewModels;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class OpenSupplierListViewCommand : CommandBase
    {
        public OpenSupplierListViewCommand() : base(typeof(MainWindowViiewModel), Open) { }

        private static void Open(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new SupplierListViewModel());
    }
}
