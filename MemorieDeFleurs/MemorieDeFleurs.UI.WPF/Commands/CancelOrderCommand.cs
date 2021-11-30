namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IOrderable"/> 実装に必要な発注キャンセルコマンド
    /// </summary>
    public class CancelOrderCommand : CommandBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CancelOrderCommand() : base(typeof(IOrderable), CancelOrder) { }

        private static void CancelOrder(object parameter) => (parameter as IOrderable).CancelMe();
    }
}
