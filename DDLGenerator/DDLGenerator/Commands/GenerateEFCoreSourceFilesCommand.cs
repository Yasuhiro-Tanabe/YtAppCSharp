using YasT.Framework.Logging;
using DDLGenerator.Models.Parsers;
using DDLGenerator.Models.Writers;
using DDLGenerator.ViewModels;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Threading;
using YasT.Framework.WPF;

namespace DDLGenerator.Commands
{
    public class GenerateEFCoreSourceFilesCommand : CommandBase<EFCoreEntityViewModel>
    {
        public GenerateEFCoreSourceFilesCommand() : base(false) { }

        private bool IsTableDefinitionFileSelected
        {
            get { return _tableDefinitonFileSelected; }
            set
            {
                _tableDefinitonFileSelected = value;
                SetExecutability(_tableDefinitonFileSelected && _outputFolderPathSelected && _namespaceDefined);
            }
        }
        private bool _tableDefinitonFileSelected = false;
        private bool IsOutputFolderPathSelected
        {
            get { return _outputFolderPathSelected; }
            set
            {
                _outputFolderPathSelected = value;
                SetExecutability(_tableDefinitonFileSelected && _outputFolderPathSelected && _namespaceDefined);
            }
        }
        private bool _outputFolderPathSelected = false;
        private bool IsNameSpaceDefined
        {
            get { return _namespaceDefined; }
            set
            {
                _namespaceDefined = value;
                SetExecutability(_tableDefinitonFileSelected && _outputFolderPathSelected && _namespaceDefined);
            }
        }
        private bool _namespaceDefined = false;

        public void OnViewModelPropertiesChanged(object sender, PropertyChangedEventArgs args)
        {
            if(sender is EFCoreEntityViewModel)
            {
                var vm = sender as EFCoreEntityViewModel;
                if(args.PropertyName == nameof(EFCoreEntityViewModel.TableDefinitionFilePath))
                {
                    IsTableDefinitionFileSelected = !string.IsNullOrWhiteSpace(vm.TableDefinitionFilePath);
                }
                if (args.PropertyName == nameof(EFCoreEntityViewModel.OutputFolderPath))
                {
                    IsOutputFolderPathSelected = !string.IsNullOrWhiteSpace(vm.OutputFolderPath);
                }
                if (args.PropertyName == nameof(EFCoreEntityViewModel.OutputNameSpace))
                {
                    IsNameSpaceDefined = !string.IsNullOrWhiteSpace(vm.OutputNameSpace);
                }
            }
        }

        protected override void Execute(EFCoreEntityViewModel parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#Execute() called. parameter={parameter?.GetType().Name}");

            Task.Run(() => Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                var generator = new Models.DDLGenerator()
                {
                    Parser = new TableDefinitionWorksheetParser()
                    {
                        DataDefinitionFileName = parameter.TableDefinitionFilePath
                    },
                    Writer = new EFCoreCsEntityWriter()
                    {
                        OutputPath = parameter.OutputFolderPath,
                        NameSpace = parameter.OutputNameSpace,
                        WriterActionMode = EFCoreCsEntityWriter.ActionIfClassFileExists.ForceOverwrite
                    }
                };
                generator.GenerateDDL();
                LogUtil.Info("ファイル出力完了");
            }));
        }
    }
}
