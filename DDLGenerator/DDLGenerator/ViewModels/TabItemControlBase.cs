using System;
using System.ComponentModel;

using YasT.Framework.WPF;

namespace DDLGenerator.ViewModels
{
    public class TabItemControlBase : NotificationObject
    {
        /// <summary>
        /// ファイル出力が開始されたときに発行されるイベント
        /// </summary>
        public event EventHandler FileGenerationStarted;

        /// <summary>
        /// ファイル出力に成功したとき発行されるイベント
        /// </summary>
        public event EventHandler FileGenerationCompleted;

        /// <summary>
        /// ファイル出力に失敗したときに発行されるイベント
        /// </summary>
        public event EventHandler FileGenerationFailed;

        /// <summary>
        /// タブのヘッダ情報
        /// </summary>
        public string Header { get; private set; } = string.Empty;

        /// <summary>
        /// 親ビューモデル
        /// </summary>
        public MainWindowViewModel Parent { get; private set; }

        /// <summary>
        /// テーブル定義書ファイル名 (パスを含む)
        /// </summary>
        public string TableDefinitionFilePath
        {
            get { return Parent.TableDefinitionFilePath; }
            set { Parent.TableDefinitionFilePath = value; }
        }

        public TabItemControlBase(string header, MainWindowViewModel parent)
        {
            Header = header;
            Parent = parent;

            parent.PropertyChanged += OnTableDefinitionFilePathChanged;
        }

        private void OnTableDefinitionFilePathChanged(object sender, PropertyChangedEventArgs args)
        {
            if(args.PropertyName == nameof(Parent.TableDefinitionFilePath))
            {
                RaisePropertyChanged(nameof(TableDefinitionFilePath));
            }
        }

        /// <summary>
        /// ファイル出力を開始する
        /// 
        /// 具体的には <see cref="FileGenerationStarted"/> イベントを発行する
        /// </summary>
        public void RaiseFileGenerationStarted()
        {
            FileGenerationStarted?.Invoke(this, null);
        }

        /// <summary>
        /// ファイル出力を正常終了する
        /// 
        /// 具体的には <see cref="FileGenerationCompleted"/> イベントを発行する
        /// </summary>
        public void RaiseFileGenerationCompleted()
        {
            FileGenerationCompleted?.Invoke(this, null);
        }

        /// <summary>
        /// ファイル出力を異常終了する
        /// 
        /// 具体的には <see cref="FileGenerationFailed"/> イベントを発行する
        /// </summary>
        public void RaiseFileGenerationFailed()
        {
            FileGenerationFailed?.Invoke(this, null);
        }
    }
}
