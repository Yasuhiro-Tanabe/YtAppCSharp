using DDLGenerator.Commands;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace DDLGenerator.ViewModels
{
    /// <summary>
    /// DDLGeneratorのビューモデル
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティを更新しイベントを発行する
        /// </summary>
        /// <typeparam name="T">プロパティの型</typeparam>
        /// <param name="variable">プロパティの値を格納する変数名</param>
        /// <param name="value">プロパティの値</param>
        /// <param name="caller">プロパティ名、省略可。省略した場合は呼び出し元 setter をもつプロパティ名。</param>
        private void SetProperty<T>(ref T variable, T value, [CallerMemberName] string caller = "")
        {
            variable = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(caller));
        }

        private string _title = "データ定義ファイルジェネレータ";
        /// <summary>
        /// ウィンドウタイトルバーに表示する文字列
        /// </summary>
        public string WindowTitle
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private string _tableDefinitionFile;
        /// <summary>
        /// テーブル定義書ファイル名 (パスを含む)
        /// </summary>
        public string TableDefinitionFilePath
        {
            get { return _tableDefinitionFile; }
            set { SetProperty(ref _tableDefinitionFile, value); }
        }

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
        public ICommand QuitCommand { get; private set; } = new QuitCommand();

    }
}
