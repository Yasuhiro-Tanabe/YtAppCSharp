using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.ComponentModel;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels.Bases
{
    /// <summary>
    /// メイン画面内に表示するタブ要素画面ビューモデルの共通ベースクラス
    /// </summary>
    public class TabItemControlViewModelBase : NotificationObject
    {
        /// <summary>
        /// タブ要素画面がクローズされようとしていることを通知する
        /// </summary>
        public event EventHandler TabItemControlClosing;

        #region プロパティ
        /// <summary>
        /// タブのヘッダーに表示する文字列
        /// </summary>
        public string Header { get; private set; }

        /// <summary>
        /// データ編集が行われたかどうかを記録する：データ登録必要かどうかの判定フラグ。
        /// </summary>
        public bool IsDirty
        {
            get { return _dirty; }
            set { SetProperty(ref _dirty, value); }
        }
        private bool _dirty;
        #endregion // プロパティ

        #region コマンド
        /// <summary>
        /// 画面(タブ)を閉じる
        /// </summary>
        public ICommand Close { get; } = new CloseTabItemControlCommand();

        /// <summary>
        /// この画面で編集したデータをデータベースに登録する
        /// </summary>
        public ICommand Register { get; private set; }
        #endregion // コマンド

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="header">タブに表示する文字列：各詳細画面の名称</param>
        protected TabItemControlViewModelBase(string header)
        {
            Header = header;
            Register = new SaveToDatabaseCommand(this);
            PropertyChanged += SetDirtyFlag;
        }

        /// <summary>
        /// ビューモデル内のプロパティが更新されたときに通知を受けるイベントハンドラ
        /// </summary>
        /// <param name="sender">イベント送信元：このビューモデル自身</param>
        /// <param name="args">イベントパラメータ：変更されたプロパティの名前</param>
        private void SetDirtyFlag(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IsDirty))
            {
                IsDirty = true;
            }
        }

        /// <summary>
        /// このビューモデルを表示しているタブ要素コントロールを閉じる
        /// 
        /// 実際には <see cref="TabItemControlClosing"/> イベントを発行するだけ
        /// </summary>
        public void CloseControl()
        {
            TabItemControlClosing?.Invoke(this, null);
        }
    }
}
