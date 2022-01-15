using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IReloadable"/> 実装に必要なプロパティ更新コマンド
    /// </summary>
    public class ReloadCommand : CommandBase<IReloadable>
    {
        /// <inheritdoc/>
        protected override void Execute(IReloadable parameter) => parameter.UpdateProperties();
    }
}
