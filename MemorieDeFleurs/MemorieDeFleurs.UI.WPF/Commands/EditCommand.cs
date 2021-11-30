namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IEditableAndFixable"/> 実装に必要な、追加表示領域を開くコマンド
    /// </summary>
    public class EditCommand : CommandBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EditCommand() : base(typeof(IEditableAndFixable), OpenEditView) { }

        private static void OpenEditView(object parameter) => (parameter as IEditableAndFixable).OpenEditView();
    }
}
