namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IAppendableRemovable"/> で実装必要な、コレクション間のアイテム移動コマンド
    /// 
    /// <see cref="AppendToListCommand"/> とは逆方向にコレクションアイテムを移動する
    /// </summary>
    public class RemoveFromListCommand : CommandBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RemoveFromListCommand() : base(typeof(IAppendableRemovable), RemoveFromList) { }

        private static void RemoveFromList(object parameter) => (parameter as IAppendableRemovable).RemoveFromList();
    }
}
