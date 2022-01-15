using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// メイン画面内にタブ要素として○○一覧画面を開くコマンド
    /// 
    /// ○○の部分は型パラメータで指定したビューモデルによって決まる
    /// </summary>
    /// <typeparam name="VM">タブ要素画面(○○一覧画面)のビューモデル</typeparam>
    public class OpenListViewCommand<VM> : CommandBase<MainWindowViiewModel> where VM : ListViewModelBase, new()
    {
        /// <inheritdoc/>
        protected override void Execute(MainWindowViiewModel mainVM) => mainVM.OpenTabItem(new VM());
    }
}
