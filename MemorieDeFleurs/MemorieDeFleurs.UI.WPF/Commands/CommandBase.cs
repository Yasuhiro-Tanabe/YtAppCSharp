using System;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public virtual void Execute(object parameter)
        {
            throw new NotImplementedException();
        }

        protected void RaiseStatusChanged()
        {
            CanExecuteChanged?.Invoke(this, null);
        }

    }
}