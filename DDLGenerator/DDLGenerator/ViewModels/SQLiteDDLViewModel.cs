using DDLGenerator.Commands;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DDLGenerator.ViewModels
{
    public class SQLiteDDLViewModel : INotifyPropertyChanged
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
        public ICommand QuitApplication { get; private set; } = new QuitCommand();

        /// <summary>
        /// DDL生成コマンド
        /// </summary>
        public ICommand GenerateDDL { get; private set; }

        /// <summary>
        /// 入力となるテーブル定義書ファイル選択ダイアログを開くコマンド
        /// </summary>
        public ICommand SelectInputFile { get; private set; }

        /// <summary>
        /// 出力となるテーブル定義スクリプト選択ダイアログを開くコマンド
        /// </summary>
        public ICommand SelectOutputFile { get; private set; }

        public SQLiteDDLViewModel()
        {
            SelectInputFile = new SelectTableDefinitionFileCommand(this);
            SelectOutputFile = new SelectOutputFileCommand(this);
            GenerateDDL = new GenerateDDLCommand(this);

        }
    }
}
