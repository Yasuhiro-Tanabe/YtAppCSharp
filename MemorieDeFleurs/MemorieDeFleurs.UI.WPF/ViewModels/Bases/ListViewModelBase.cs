using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.Windows.Input;

using YasT.Framework.Logging;

namespace MemorieDeFleurs.UI.WPF.ViewModels.Bases
{
    /// <summary>
    /// 一覧画面(タブ要素画面)ビューモデルの共通ベースクラス
    /// </summary>
    public class ListViewModelBase : TabItemControlViewModelBase
    {
        /// <summary>
        /// この一覧画面で選択した要素の詳細画面を開こうとしていることを通知する
        /// </summary>
        public event EventHandler DetailViewOpening;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="header">タブに表示する画面名称</param>
        protected ListViewModelBase(string header) : base(header) { }

        #region コマンド
        /// <summary>
        /// ビュー内の一覧表示コントロールで SelectionChanged イベントが発行されたときに実行するコマンド
        /// </summary>
        public ICommand Selected { get; } = new SelectionChangedEventHandler();
        #endregion // コマンド

        private void OpenDetailView(object sender, EventArgs unused)
        {
            LogUtil.DEBUGLOG_MethodCalled(sender.GetType().Name);
            DetailViewOpening?.Invoke(this, null);
        }

        /// <summary>
        /// 選択中のリスト要素ビューモデルが発行する <see cref="ListItemViewModelBase.SelectedItemRemoving"/> イベントを購読するイベントハンドラ
        /// </summary>
        /// <param name="sender">イベント通知元オブジェクト</param>
        /// <param name="unused">イベントパラメータ</param>
        protected void RemoveSelectedItem(object sender, EventArgs unused)
        {
            LogUtil.DEBUGLOG_MethodCalled(sender.GetType().Name);
            RemoveSelectedItem(sender);
        }

        /// <summary>
        /// 作成したリスト要素ビューモデルのイベント購読を開始する
        /// </summary>
        /// <param name="view">このビューが持つイベントハンドラを登録するリスト要素ビュー</param>
        protected void Subscribe(ListItemViewModelBase view)
        {
            view.DetailViewOpening += OpenDetailView;
            view.SelectedItemRemoving += RemoveSelectedItem;
        }

        /// <summary>
        /// 削除対象リスト要素ビューモデルのは気前にイベント購読を解除する
        /// </summary>
        /// <param name="view">イベントハンドラを登録していたリスト要素ビュー</param>
        protected void Unsubscribe(ListItemViewModelBase view)
        {
            view.DetailViewOpening -= OpenDetailView;
            view.SelectedItemRemoving -= RemoveSelectedItem;
        }

        /// <summary>
        /// 選択されたビューモデルとビューモデルが値を格納しているエンティティを削除する
        /// </summary>
        /// <param name="sender">選択中のリスト要素ビュー： <see cref="ListItemViewModelBase.SelectedItemRemoving"/> イベントの通知元</param>
        /// <exception cref="NotImplementedException">削除処理が実装されていない：ポカヨケ用</exception>
        protected virtual void RemoveSelectedItem(object sender) { throw new NotImplementedException($"{this.GetType().Name}.{nameof(RemoveSelectedItem)}({sender?.GetType()?.Name})"); }

        /// <summary>
        /// 選択されたビューモデルが値を格納しているエンティティの詳細画面を作成しメイン画面に登録する
        /// </summary>
        /// <param name="mainVM">メイン画面：詳細画面のビューモデル登録先</param>
        /// <returns>メソッド内で作成した詳細画面ビューモデル</returns>
        /// <exception cref="NotImplementedException">詳細画面登録処理が実装されていない：ポカヨケ用</exception>
        public virtual DetailViewModelBase OpenDetailTabItem(MainWindowViiewModel mainVM) { throw new NotImplementedException($"{GetType().Name}.{nameof(OpenDetailView)}({mainVM.GetType().Name})"); }
    }
}
