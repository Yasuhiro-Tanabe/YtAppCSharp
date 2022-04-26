using DDLGenerator.Commands;

using System.Windows.Input;

namespace DDLGenerator.ViewModels
{
    public class SQLiteDDLViewModel : TabItemControlBase
    {
        private string _outputFIle;
        /// <summary>
        /// 生成するデータ定義スクリプトファイル名 (パスを含む)
        /// </summary>
        public string OutputDdlFilePath
        {
            get { return _outputFIle; }
            set { SetProperty(ref _outputFIle, value); }
        }

        /// <summary>
        /// アプリケーション終了コマンド
        /// </summary>
        public ICommand QuitApplication { get; } = new QuitCommand();

        /// <summary>
        /// DDL生成コマンド
        /// </summary>
        public ICommand GenerateDDL { get; } = new GenerateSQLiteDDLFileCommand();

        /// <summary>
        /// 入力となるテーブル定義書ファイル選択ダイアログを開くコマンド
        /// </summary>
        public ICommand SelectInputFile { get; } = new SelectTableDefinitionFileCommand();

        /// <summary>
        /// 出力となるテーブル定義スクリプト選択ダイアログを開くコマンド
        /// </summary>
        public ICommand SelectOutputFile { get; } = new SelectOutputFileCommand();

        public SQLiteDDLViewModel(MainWindowViewModel parent) : base("SQLite", parent)
        {

            PropertyChanged += ((GenerateSQLiteDDLFileCommand)GenerateDDL).OnViewModelPropertyChanged;

            FileGenerationStarted += ((SelectTableDefinitionFileCommand)SelectInputFile).OnGenerationStarted;
            FileGenerationCompleted += ((SelectTableDefinitionFileCommand)SelectInputFile).OnGenerationFinished;
            FileGenerationFailed += ((SelectTableDefinitionFileCommand)SelectInputFile).OnGenerationFinished;

            FileGenerationStarted += ((SelectOutputFileCommand)SelectOutputFile).OnGenerationStarted;
            FileGenerationCompleted += ((SelectOutputFileCommand)SelectOutputFile).OnGenerationFinished;
            FileGenerationFailed += ((SelectOutputFileCommand)SelectOutputFile).OnGenerationFinished;
        }
    }
}
