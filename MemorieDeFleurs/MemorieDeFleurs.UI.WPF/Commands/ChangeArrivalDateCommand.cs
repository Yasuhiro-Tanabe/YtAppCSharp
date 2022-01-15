using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IOrderable"/> 実装に必要な、お届け日・入荷予定日変更コマンド
    /// </summary>
    public class ChangeArrivalDateCommand : CommandBase<IOrderable>
    {
        /// <inheritdoc/>
        protected override void Execute(IOrderable parameter) => parameter.ChangeMyArrivalDate();
    }
}
