using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using System.ComponentModel;
using System.Linq;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class SaveToDatabaseCommand : CommandBase
    {
        public SaveToDatabaseCommand(NotificationObject vm) : base()
        {
            AddChecker(typeof(TabItemControlViewModelBase), IsDirty);
            AddAction(typeof(BouquetPartsDetailViewModel), SaveToDatabase);
            AddAction(typeof(BouquetDetailViewModel), SaveToDatabase);
            AddAction(typeof(SupplierDetailViewModel), SaveToDatabase);
            AddAction(typeof(CustomerDetailViewModel), SaveToDatabase);

            vm.PropertyChanged += CheckDirtyFlag;
        }

        private static bool IsDirty(object parameter) => (parameter as TabItemControlViewModelBase).IsDirty;
        private static void SaveToDatabase(object parameter) => (parameter as DetailViewModelBase).SaveToDatabase();

        public void CheckDirtyFlag(object sender, PropertyChangedEventArgs args)
        {
            var vm = sender as TabItemControlViewModelBase;
            if (args.PropertyName == nameof(vm.IsDirty))
            {
                RaiseStatusChanged();
            }
        }
    }
}
