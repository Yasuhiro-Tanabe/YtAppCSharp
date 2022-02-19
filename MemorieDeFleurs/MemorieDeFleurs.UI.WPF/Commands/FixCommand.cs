using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IEditableAndFixable"/> 実装に必要な、追加表示領域を閉じるコマンド
    /// </summary>
    public class FixCommand : CommandBase<IEditableAndFixable>
    {
        /// <inheritdoc/>
        protected override void Execute(IEditableAndFixable parameter) => parameter.FixEditing();
    }
}
