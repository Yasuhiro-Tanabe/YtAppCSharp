namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IEditableAndFixable"/> 実装に必要な、追加表示領域を閉じるコマンド
    /// </summary>
    public class FixCommand : CommandBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FixCommand() : base(typeof(IEditableAndFixable), FixEditing) { }

        private static void FixEditing(object parameter) => (parameter as IEditableAndFixable).FixEditing();
    }
}
