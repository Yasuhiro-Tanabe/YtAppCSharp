namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// Reload ボタンやLoadedイベントハンドラを持つビューモデルに実装すべきインタフェース
    /// </summary>
    public interface IReloadable
    {
        /// <summary>
        /// ビューモデルの持つプロパティを更新する
        /// </summary>
        public void UpdateProperties();
    }
}
