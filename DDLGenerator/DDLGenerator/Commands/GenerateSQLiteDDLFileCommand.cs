using YasT.Framework.Logging;
using DDLGenerator.ViewModels;

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using DDLGenerator.Models.Writers;
using DDLGenerator.Models.Parsers;
using System.ComponentModel;

namespace DDLGenerator.Commands
{
    class GenerateSQLiteDDLFileCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#{nameof(CanExecute)}() called. parameter={parameter?.GetType().Name}");

            return IsTableDefinitionFileSelected && IsOutputDDLFileSelected;
        }

        public void Execute(object parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#{nameof(Execute)}() called. parameter={parameter?.GetType().Name}");

            if(parameter is SQLiteDDLViewModel)
            {
                var vm = parameter as SQLiteDDLViewModel;

                Task.Run(() => Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    var generator = new Models.DDLGenerator()
                    {
                        Parser = new TableDefinitionWorksheetParser() { DataDefinitionFileName = vm.TableDefinitionFilePath },
                        Writer = new SQLiteDDLWriter() { OutputFileName = vm.OutputDdlFilePath }
                    };

                    generator.GenerateDDL();
                    LogUtil.Info("ファイル出力完了");
                }));
            }
        }

        private bool IsTableDefinitionFileSelected { get; set; } = false;
        private bool IsOutputDDLFileSelected { get; set; } = false;
        public void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if(sender is SQLiteDDLViewModel)
            {
                var vm = sender as SQLiteDDLViewModel;
                if(args.PropertyName == nameof(SQLiteDDLViewModel.TableDefinitionFilePath))
                {
                    IsTableDefinitionFileSelected = !string.IsNullOrWhiteSpace(vm.TableDefinitionFilePath);
                    CanExecuteChanged?.Invoke(this, null);
                }
                if (args.PropertyName == nameof(SQLiteDDLViewModel.OutputDdlFilePath))
                {
                    IsOutputDDLFileSelected = !string.IsNullOrWhiteSpace(vm.OutputDdlFilePath);
                    CanExecuteChanged?.Invoke(this, null);
                }

            }
        }
    }
}
