namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IOrderable"/> 実装に必要な、お届け日・入荷予定日変更コマンド
    /// </summary>
    public class ChangeArrivalDateCommand : CommandBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChangeArrivalDateCommand() : base(typeof(IOrderable), ChangeArrivalDate) { }

        private static void ChangeArrivalDate(object parameter) => (parameter as IOrderable).ChangeMyArrivalDate();
    }
}
