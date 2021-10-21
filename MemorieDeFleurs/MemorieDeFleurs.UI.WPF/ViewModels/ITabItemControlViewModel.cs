using MemorieDeFleurs.UI.WPF.Commands;

using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public interface ITabItemControlViewModel
    {
        #region プロパティ
        /// <summary>
        /// タブのヘッダーに表示する文字列
        /// </summary>
        public string Header { get; }

        /// <summary>
        /// 親ビューモデル
        /// </summary>
        public MainWindowViiewModel ParentViewModel { get; set; }

        /// <summary>
        /// データ編集が行われたかどうかを記録する：データ登録必要かどうかの判定フラグ。
        /// </summary>
        public bool IsDirty { get; }
        #endregion // プロパティ

        #region コマンド
        /// <summary>
        /// 画面(タブ)を閉じる
        /// </summary>
        public ICommand Close { get; }

        /// <summary>
        /// この画面で編集したデータをデータベースに登録する
        /// </summary>
        public ICommand Register { get; }
        #endregion // コマンド
    }
}
