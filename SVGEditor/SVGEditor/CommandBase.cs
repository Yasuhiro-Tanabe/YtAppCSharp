using System;
using System.Windows.Input;

namespace SVGEditor
{
    internal class CommandBase : ICommand
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
            return CanExecuteImpl();
        }

        public void Execute(object parameter)
        {
            ViewModel = parameter as MainWindowViewModel;

            ExecuteImpl();
        }

        protected virtual bool CanExecuteImpl()
        {
            return true;
        }

        protected virtual void ExecuteImpl()
        {

        }

    }
}
