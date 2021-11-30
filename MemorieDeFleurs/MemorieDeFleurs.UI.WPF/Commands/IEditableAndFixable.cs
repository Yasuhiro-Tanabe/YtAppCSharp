using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// ビューモデル内の一部を「追加編集領域」として普段は隠しておき、
    /// 必要に応じてユーザーが表示/非表示を切り替える処理を行う際に実装するインタフェース。
    /// 
    /// 
    /// </summary>
    internal interface IEditableAndFixable
    {
        /// <summary>
        /// 追加表示領域を開く。
        /// 具体的には <see cref="IsEditing"/> を true に変更し <see cref="OpenEditView"/> を呼び出す。
        /// </summary>
        public ICommand Edit { get; }

        /// <summary>
        /// 追加表示領域を隠す。
        /// 具体的には <see cref="IsEditing"/> を false に変更し <see cref="FixEditing"/> を呼び出す。
        /// </summary>
        public ICommand Fix { get; }

        /// <summary>
        /// 追加表示領域を表示するかどうか
        /// </summary>
        public bool IsEditing { get; }

        /// <summary>
        /// 追加表示領域を開くときに呼び出される。
        /// </summary>
        public void OpenEditView();

        /// <summary>
        /// 追加表示領域を閉じるときに呼び出される。
        /// </summary>
        public void FixEditing();
    }
}
