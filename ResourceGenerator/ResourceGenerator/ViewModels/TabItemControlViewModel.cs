using ResourceGenerator.Commands;
using ResourceGenerator.Models;

using YasT.Framework.WPF;

namespace ResourceGenerator.ViewModels
{
    /// <summary>
    /// 各タブの共通クラス。
    /// </summary>
    public class TabItemControlViewModel : NotificationObject
    {
        #region プロパティ
        /// <summary>
        /// タブヘッダに表示する文字列。
        /// </summary>
        public string Header
        {
            get { return _header ?? string.Empty; }
            protected set { SetProperty(ref _header, value); }
        }
        private string? _header;
        /// <summary>
        /// このタブアイテムをクローズできるかどうか：設定タブは閉じられないようにする。
        /// </summary>
        public bool CanClose { get; private set; }
        /// <summary>
        /// リソース管理モデル。
        /// </summary>
        public ResourceGenerationModel Model { get; private set; }
        #endregion プロパティ
        #region コマンド
        /// <summary>
        /// タブを閉じる。
        /// </summary>
        public CommandBase<TabItemControlViewModel> Close { get; private set; } = new CloseTabItemCommand();
        #endregion コマンド

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="model">呼び出し元で生成したリソース管理モデル。</param>
        /// <param name="header">タブヘッダ表示文字列。</param>
        /// <param name="canclose">このタブアイテムをクローズできるかどうか。
        /// 通常タブアイテムはクローズできるため引数省略可。
        /// クローズできないようにするときだけ true を渡す。</param>
        protected TabItemControlViewModel(ResourceGenerationModel model, string header, bool canclose = true)
        {
            Header = header;
            Model = model;
            CanClose = canclose;
        }

    }
}
