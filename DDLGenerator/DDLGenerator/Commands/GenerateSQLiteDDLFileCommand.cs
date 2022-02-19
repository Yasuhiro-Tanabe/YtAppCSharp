using YasT.Framework.Logging;
using DDLGenerator.ViewModels;
using System.Threading.Tasks;
using System.Windows.Threading;
using DDLGenerator.Models.Writers;
using DDLGenerator.Models.Parsers;
using System.ComponentModel;
using YasT.Framework.WPF;

namespace DDLGenerator.Commands
{
    public class GenerateSQLiteDDLFileCommand : CommandBase<SQLiteDDLViewModel>
    {
        public GenerateSQLiteDDLFileCommand() : base(false) { }

        private bool IsTableDefinitionFileSelected
        {
            get { return _tableDefinitionFileSelected; }
            set
            {
                _tableDefinitionFileSelected = value;
                SetExecutability(_tableDefinitionFileSelected && _outputDDLFileSelected);
            }
        }
        private bool _tableDefinitionFileSelected = false;
        private bool IsOutputDDLFileSelected
        {
            get { return _outputDDLFileSelected; }
            set
            {
                _outputDDLFileSelected = value;
                SetExecutability(_tableDefinitionFileSelected && _outputDDLFileSelected);
            }
        }
        private bool _outputDDLFileSelected = false;
        public void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if(sender is SQLiteDDLViewModel)
            {
                var vm = sender as SQLiteDDLViewModel;
                if(args.PropertyName == nameof(SQLiteDDLViewModel.TableDefinitionFilePath))
                {
                    IsTableDefinitionFileSelected = !string.IsNullOrWhiteSpace(vm.TableDefinitionFilePath);
                }
                if (args.PropertyName == nameof(SQLiteDDLViewModel.OutputDdlFilePath))
                {
                    IsOutputDDLFileSelected = !string.IsNullOrWhiteSpace(vm.OutputDdlFilePath);
                }

            }
        }

        protected override void Execute(SQLiteDDLViewModel parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#{nameof(Execute)}() called. parameter={parameter?.GetType().Name}");

            Task.Run(() => Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                var generator = new Models.DDLGenerator()
                {
                    Parser = new TableDefinitionWorksheetParser() { DataDefinitionFileName = parameter.TableDefinitionFilePath },
                    Writer = new SQLiteDDLWriter() { OutputFileName = parameter.OutputDdlFilePath }
                };

                generator.GenerateDDL();
                LogUtil.Info("ファイル出力完了");
            }));
        }
    }
}
