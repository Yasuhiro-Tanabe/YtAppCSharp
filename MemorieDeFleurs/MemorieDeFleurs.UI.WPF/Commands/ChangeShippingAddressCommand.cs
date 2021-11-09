using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.ViewModels;

using System;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ChangeShippingAddressCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if(parameter == null)
            {
                LogUtil.Debug("parameter is null");
            }
            else if (parameter is OrderFromCustomerDetailViewModel)
            {
                var vm = parameter as OrderFromCustomerDetailViewModel;
                if (vm.SelectedCustomer != null)
                {
                    vm.LoadShippingAddresses(vm.SelectedCustomer.CustomerID);
                    vm.SelectedShippingAddress = null;
                }
                else
                {
                    LogUtil.Debug("SelectedCustomer is null.");
                }
            }
            else
            {
                throw new NotImplementedException($"{GetType().Name}.{nameof(Execute)}({parameter.GetType().Name})");
            }
        }

        private void RaiseEvent()
        {
            CanExecuteChanged?.Invoke(this, null);
        }
    }
}
