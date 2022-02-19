using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IAppendableRemovable"/> で実装必要な、コレクション間のアイテム移動コマンド
    /// 
    /// <see cref="AppendToListCommand"/> とは逆方向にコレクションアイテムを移動する
    /// </summary>
    public class RemoveFromListCommand : CommandBase<IAppendableRemovable>
    {
        /// <inheritdoc/>
        protected override void Execute(IAppendableRemovable parameter) => parameter.RemoveFromList();
    }
}
