using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.Windows.Input;

using YasT.Framework.Logging;
using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.ViewModels.Bases
{
    /// <summary>
    /// 一覧画面内(タブ要素画面)の一覧表示で使用する各要素ビューモデルの共通ベースクラス
    /// </summary>
    public class ListItemViewModelBase : NotificationObject
    {
        /// <summary>
        /// 選択中の要素に関する詳細画面を表示しようとしていることを通知する
        /// </summary>
        public event EventHandler DetailViewOpening;

        /// <summary>
        /// 選択中の要素をDBから削除しようとしていることを通知する
        /// </summary>
        public event EventHandler SelectedItemRemoving;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="openDetailView">詳細画面表示コマンド：このクラスのサブクラス毎に異なるためコンストラクタで渡す</param>
        protected ListItemViewModelBase(ICommand openDetailView) : base()
        {
            Detail = openDetailView;
        }

        #region プロパティ
        /// <summary>
        /// リスト内の要素を特定するためのキー文字列
        /// 
        /// 今のところログ専用
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }
        private string _key;

        /// <summary>
        /// ボタンを表示する/しているかどうか
        /// </summary>
        public bool IsActionVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }
        private bool _isVisible = false;
        #endregion // プロパティ

        #region コマンド
        /// <summary>
        /// 削除ボタン押下時に実行するコマンド
        /// </summary>
        public ICommand Remove { get; } = new DeleteFromDatabase();

        /// <summary>
        /// 詳細ボタン押下時に実行するコマンド
        /// </summary>
        public ICommand Detail { get; private set; }
        #endregion // コマンド

        /// <summary>
        /// <see cref="Remove"/> コマンド実行時の処理
        /// </summary>
        public void RemoveMe()
        {
            LogUtil.DEBUGLOG_MethodCalled();
            SelectedItemRemoving?.Invoke(this, null);
            RaisePropertyChanged(nameof(RemoveMe));
        }

        /// <summary>
        /// <see cref="Detail"/> コマンド実行時の処理
        /// </summary>
        public void OpenDetailView()
        {
            LogUtil.DEBUGLOG_MethodCalled();
            DetailViewOpening?.Invoke(this, null);
        }

        /// <summary>
        /// 要素の選択状態を初期化する
        /// </summary>
        /// <param name="key">要素ビューモデルに対応する詳細ビューモデルのキー情報</param>
        protected void Update(string key)
        {
            Key = key;
            IsActionVisible = false;
        }
    }
}
