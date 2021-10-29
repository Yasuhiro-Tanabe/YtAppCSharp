using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.ViewModels;

using System.ComponentModel;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class SaveToDatabaseCommand : CommandBase
    {
        public SaveToDatabaseCommand(NotificationObject vm) : base()
        {
            AddChecker(typeof(TabItemControlViewModelBase), IsDirty);
            AddAction(typeof(BouquetPartsDetailViewModel), CreateBouquetParts);
            AddAction(typeof(BouquetDetailViewModel), CreateBouquet);

            vm.PropertyChanged += CheckDirtyFlag;
        }

        private static bool IsDirty(object parameter) => (parameter as TabItemControlViewModelBase).IsDirty;
        
        private static void CreateBouquetParts(object parameter)
        {
            var vm = parameter as BouquetPartsDetailViewModel;
            vm.Validate();

            var model = new MemorieDeFleursModel(MemorieDeFleursUIModel.Instance.DbConnection);
            var builder = model.BouquetModel.GetBouquetPartBuilder();

            builder.PartCodeIs(vm.PartsCode)
                .PartNameIs(vm.PartsName)
                .QauntityParLotIs(vm.QuantitiesParLot)
                .LeadTimeIs(vm.LeadTime)
                .ExpiryDateIs(vm.ExpiryDate);

            vm.Update(builder.Create());
            vm.IsDirty = false;
            LogUtil.Info($"Bouquet parts {vm.PartsCode} is saved.");
        }

        private static void CreateBouquet(object parameter)
        {
            var vm = parameter as BouquetDetailViewModel;
            vm.Validate();

            var model = new MemorieDeFleursModel(MemorieDeFleursUIModel.Instance.DbConnection);
            var builder = model.BouquetModel.GetBouquetBuilder();

            builder.CodeIs(vm.BouquetCode)
                .NameIs(vm.BouquetName)
                .ImageIs(vm.ImageFileName);
            foreach (var p in vm.SelectedPartListItem) { builder.Uses(p.PartsCode, p.Quantity); }

            vm.Update(builder.Create());
            vm.IsDirty = false;
            LogUtil.Info($"Bouquet {vm.BouquetCode} is saved.");
        }


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
