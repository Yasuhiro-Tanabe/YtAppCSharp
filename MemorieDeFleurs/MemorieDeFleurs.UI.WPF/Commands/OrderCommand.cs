namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IOrderable"/> 実装に必要な発注コマンド
    /// </summary>
    public class OrderCommand : CommandBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OrderCommand() : base(typeof(IOrderable), Order) { }

        private static void Order(object parameter) => (parameter as IOrderable).OrderMe();
    }
}
