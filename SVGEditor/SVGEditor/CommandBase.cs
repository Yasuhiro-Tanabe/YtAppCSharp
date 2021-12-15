using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace SVGEditor
{
    internal abstract class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        protected void CommandExecutabilityChanged()
        {
            CanExecuteChanged?.Invoke(this, null);
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);
    }
}
