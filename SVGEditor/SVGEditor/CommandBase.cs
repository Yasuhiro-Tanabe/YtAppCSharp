using System;
using System.Windows.Input;

namespace SVGEditor
{
    internal abstract class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        protected void RaiseEvent()
        {
            CanExecuteChanged?.Invoke(this, null);
        }

        protected MainWindowViewModel ViewModel { get; private set; }
        protected SVGEditorModel Model { get; } = new SVGEditorModel();

        public bool CanExecute(object parameter)
        {
            return CanExecute();
        }

        public void Execute(object parameter)
        {
            ViewModel = parameter as MainWindowViewModel;

            Execute();
        }

        protected virtual bool CanExecute()
        {
            return true;
        }

        protected abstract void Execute();

    }
}
