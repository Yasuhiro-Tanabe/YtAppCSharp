using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IOrderable"/> 実装に必要な発注コマンド
    /// </summary>
    public class OrderCommand : CommandBase<IOrderable>
    {
        /// <inheritdoc/>
        protected override void Execute(IOrderable parameter) => parameter.OrderMe();
    }
}
