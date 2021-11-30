namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IAppendableRemovable"/> で実装必要な、コレクション間のアイテム移動コマンド
    /// 
    /// このコマンドがコレクションアイテムを移動する方向を正として、
    /// <see cref="RemoveFromListCommand"/> は逆方向のコレクションアイテム移動を行う
    /// </summary>
    public class AppendToListCommand : CommandBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AppendToListCommand() : base(typeof(IAppendableRemovable), AddToList) { }

        private static void AddToList(object parameter) => (parameter as IAppendableRemovable).AppendToList();
    }
}
