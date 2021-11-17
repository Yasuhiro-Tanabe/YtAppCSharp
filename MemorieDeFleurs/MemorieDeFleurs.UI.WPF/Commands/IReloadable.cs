namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// Reload ボタンやLoadedイベントハンドラを持つビューモデルに実装すべきインタフェース
    /// </summary>
    internal interface IReloadable
    {
        /// <summary>
        /// ビューモデルの持つプロパティを更新する：
        /// データベースから値を取り直しても、単純に RaisePropertyChanged を呼んでもよい。
        /// 
        /// いずれにせよ、ビューの再描画に必要な PropertyChanged イベントを発行すること。
        /// </summary>
        public void UpdateProperties();
    }
}
