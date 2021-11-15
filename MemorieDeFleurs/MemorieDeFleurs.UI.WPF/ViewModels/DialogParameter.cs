namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    /// <summary>
    /// ダイアログ (DialogWindow) を開くために内部の UserControl から取得する必要のあるパラメータ
    /// </summary>
    public class DialogParameter
    {
        /// <summary>
        /// ダイアログのタイトルとして表示する文字列
        /// </summary>
        public string DialogTitle { get; set; }

        /// <summary>
        /// ダイアログの Ok ボタンに表示するコンテンツ
        /// </summary>
        public object OkContent { get; set; }

        /// <summary>
        /// ダイアログの Cancel ボタンに表示するコンテンツ
        /// </summary>
        public object CancelContent { get; set; }
    }
}
