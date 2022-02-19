using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IEditableAndFixable"/> 実装に必要な、追加表示領域を開くコマンド
    /// </summary>
    public class EditCommand : CommandBase<IEditableAndFixable>
    {
        /// <inheritdoc/>
        protected override void Execute(IEditableAndFixable parameter) => parameter.OpenEditView();
    }
}
