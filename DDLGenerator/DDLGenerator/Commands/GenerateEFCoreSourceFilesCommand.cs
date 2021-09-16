using DDLGenerator.Models.Logging;
using DDLGenerator.Models.Parsers;
using DDLGenerator.Models.Writers;
using DDLGenerator.ViewModels;

using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

namespace DDLGenerator.Commands
{
    public class GenerateEFCoreSourceFilesCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return IsTableDefinitionFileSelected && IsOutputFolderPathSelected && IsNameSpaceDefined;
        }

        public void Execute(object parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#Execute() called. parameter={parameter?.GetType().Name}");

            Task.Run(() => Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                if (parameter is EFCoreEntityViewModel)
                {
                    var vm = parameter as EFCoreEntityViewModel;
                    var generator = new Models.DDLGenerator()
                    {
                        Parser = new TableDefinitionWorksheetParser()
                        {
                            DataDefinitionFileName = vm.TableDefinitionFilePath
                        },
                        Writer = new EFCoreCsEntityWriter()
                        {
                            OutputPath = vm.OutputFolderPath,
                            NameSpace = vm.OutputNameSpace,
                            WriterActionMode = EFCoreCsEntityWriter.ActionIfClassFileExists.ForceOverwrite
                        }
                    };
                    generator.GenerateDDL();
                    LogUtil.Info("ファイル出力完了");
                }
                else
                {
                    LogUtil.Warn($"Unexpected View: {parameter.GetType().Name}");
                }
            }));
        }

        private bool IsTableDefinitionFileSelected { get; set; } = false;
        private bool IsOutputFolderPathSelected { get; set; } = false;
        private bool IsNameSpaceDefined { get; set; } = false;

        public void OnViewModelPropertiesChanged(object sender, PropertyChangedEventArgs args)
        {
            if(sender is EFCoreEntityViewModel)
            {
                var vm = sender as EFCoreEntityViewModel;
                if(args.PropertyName == nameof(EFCoreEntityViewModel.TableDefinitionFilePath))
                {
                    IsTableDefinitionFileSelected = !string.IsNullOrWhiteSpace(vm.TableDefinitionFilePath);
                    CanExecuteChanged?.Invoke(this, null);
                }
                if (args.PropertyName == nameof(EFCoreEntityViewModel.OutputFolderPath))
                {
                    IsOutputFolderPathSelected = !string.IsNullOrWhiteSpace(vm.OutputFolderPath);
                    CanExecuteChanged?.Invoke(this, null);
                }
                if (args.PropertyName == nameof(EFCoreEntityViewModel.OutputNameSpace))
                {
                    IsNameSpaceDefined = !string.IsNullOrWhiteSpace(vm.OutputNameSpace);
                    CanExecuteChanged?.Invoke(this, null);
                }
            }
        }
    }
}
