using MemorieDeFleurs.Logging;
using MemorieDeFleurs.Models;
using MemorieDeFleurs.UI.WPF.Model;
using MemorieDeFleurs.UI.WPF.Model.Exceptions;
using MemorieDeFleurs.UI.WPF.ViewModels;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class RegisterCommand : CommandBase
    {
        public override bool CanExecute(object parameter)
        {
            if(parameter is TabItemControlViewModelBase)
            {
                var vm = parameter as TabItemControlViewModelBase;
                return vm.IsDirty;
            }
            else
            {
                return base.CanExecute(parameter);
            }
        }

        public override void Execute(object parameter)
        {
            try
            {
                if (parameter is BouquetPartsDetailViewModel)
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
                }
                else if (parameter is BouquetDetailViewModel)
                {
                    var vm = parameter as BouquetDetailViewModel;
                    vm.Validate();

                    var model = new MemorieDeFleursModel(MemorieDeFleursUIModel.Instance.DbConnection);
                    var builder = model.BouquetModel.GetBouquetBuilder();

                    builder.CodeIs(vm.BouquetCode)
                        .NameIs(vm.BouquetName)
                        .ImageIs(vm.ImageFileName);
                    foreach (var p in vm.PartsList) { builder.Uses(p.Key, p.Value); }

                    vm.Update(builder.Create());
                    vm.IsDirty = false;
                }
                else
                {
                    base.Execute(parameter);
                }
            }
            catch (ValidateFailedException ex)
            {
                MessageBox.Show($"{string.Join("\n", ex.ValidationErrors)}", ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (NotConnectedToDatabaseException ex)
            {
                MessageBox.Show(ex.Message, $"DB未接続", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    LogUtil.Warn($"Exception thrown: {ex.GetType().Name}, {ex.Message}\n{ex.StackTrace}");
                }
                else
                {
                    LogUtil.Warn($"Exception thrown: {ex.GetType().Name}, {ex.Message} => {ex.InnerException.GetType()}, {ex.InnerException.Message}\n{ex.StackTrace}");
                }
                MessageBox.Show(ex.Message, "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public RegisterCommand(NotificationObject vm)
        {
            vm.PropertyChanged += CheckDirtyFlag;
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
