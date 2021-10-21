using MemorieDeFleurs.Models;
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
            if(parameter is BouquetPartsDetailViewModel)
            {
                try
                {
                    var vm = parameter as BouquetPartsDetailViewModel;
                    var model = new MemorieDeFleursModel(null);
                    var builder = model.BouquetModel.GetBouquetPartBuilder();

                    vm.Validate();
                    builder.PartCodeIs(vm.PartsCode)
                        .PartNameIs(vm.PartsName)
                        .QauntityParLotIs(vm.QuantitiesParLot)
                        .LeadTimeIs(vm.LeadTime)
                        .ExpiryDateIs(vm.ExpiryDate);
                    vm.Update(builder.Create());
                    vm.IsDirty = false;

                }
                catch (ValidateFailedException ex)
                {
                    MessageBox.Show($"{string.Join("\n", ex.ValidationErrors)}", ex.Message, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "システムエラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                base.Execute(parameter);
            }
        }

        public RegisterCommand(ViewModelBase vm)
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
