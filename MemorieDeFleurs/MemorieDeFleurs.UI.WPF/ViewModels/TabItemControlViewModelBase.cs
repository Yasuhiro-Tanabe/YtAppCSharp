using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.ComponentModel;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class TabItemControlViewModelBase : NotificationObject
    {
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

        protected TabItemControlViewModelBase(string header)
        {
            Header = header;
            Register = new RegisterCommand(this);
            PropertyChanged += SetDirtyFlag;
        }

        private void SetDirtyFlag(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(IsDirty))
            {
                IsDirty = true;
            }
        }

        public void CloseControl()
        {
            TabItemControlClosing?.Invoke(this, null);
        }
    }
}
