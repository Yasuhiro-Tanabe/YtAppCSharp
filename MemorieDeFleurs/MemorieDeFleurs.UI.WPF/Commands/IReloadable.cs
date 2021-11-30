using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// Reload ボタンやLoadedイベントハンドラを持つビューモデルに実装すべきインタフェース
    /// </summary>
    public interface IReloadable
    {
        /// <summary>
        /// ビューモデルの内容を洗い替えする
        /// </summary>
        public ReloadCommand Reload { get; }

        /// <summary>
        /// ビューモデルの持つプロパティを更新する
        /// </summary>
        public void UpdateProperties();
    }
}
