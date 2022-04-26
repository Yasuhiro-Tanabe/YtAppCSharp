using DDLGenerator.Commands;

using System;
using System.Windows.Input;

namespace DDLGenerator.ViewModels
{
    public class EFCoreEntityViewModel : TabItemControlBase
    {
        /// <summary>
        /// ソースファイル出力先
        /// </summary>
        private string _outputFolder;
        public string OutputFolderPath
        {
            get { return _outputFolder; }
            set { SetProperty(ref _outputFolder, value); }
        }

        private string _nameSpace;
        public string OutputNameSpace
        {
            get { return _nameSpace; }
            set { SetProperty(ref _nameSpace, value);  }
        }

        /// <summary>
        /// アプリケーション終了コマンド
        /// </summary>
        public ICommand QuitApplication { get; } = new QuitCommand();

        /// <summary>
        /// 入力となるテーブル定義書ファイル選択ダイアログを開くコマンド
        /// </summary>
        public ICommand SelectInputFile { get; } = new SelectTableDefinitionFileCommand();

        /// <summary>
        /// エンティティクラスのソースファイル出力先フォルダ選択ダイアログを開くコマンド
        /// </summary>
        public ICommand SelectOutputFolder { get; } = new SelectOutputFolderCommand();

        /// <summary>
        /// Entity Framework のソースコード出力コマンド
        /// </summary>
        public ICommand GenerateEFSourceCode { get; } = new GenerateEFCoreSourceFilesCommand();

        public EFCoreEntityViewModel(MainWindowViewModel parent) : base("EFCore", parent)
        {
            PropertyChanged += ((GenerateEFCoreSourceFilesCommand)GenerateEFSourceCode).OnViewModelPropertiesChanged;

            FileGenerationStarted +=((SelectTableDefinitionFileCommand)SelectInputFile).OnGenerationStarted;
            FileGenerationCompleted += ((SelectTableDefinitionFileCommand)SelectInputFile).OnGenerationFinished;
            FileGenerationFailed += ((SelectTableDefinitionFileCommand)SelectInputFile).OnGenerationFinished;

            FileGenerationStarted +=((SelectOutputFolderCommand)SelectOutputFolder).OnGenerationStarted;
            FileGenerationCompleted += ((SelectOutputFolderCommand)SelectOutputFolder).OnGenerationFinished;
            FileGenerationFailed += ((SelectOutputFolderCommand)SelectOutputFolder).OnGenerationFinished;

        }
    }
}
