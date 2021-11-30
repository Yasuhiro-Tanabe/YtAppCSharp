using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.ViewModels.Bases;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// メイン画面内にタブ要素として○○詳細画面を開くコマンド
    /// 
    /// ○○の部分は型パラメータで指定したビューモデルによって決まる
    /// </summary>
    /// <typeparam name="VM">タブ要素画面(○○詳細画面)のビューモデル</typeparam>
    public class OpenDetailViewCommand<VM> : CommandBase where VM : DetailViewModelBase, new()
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OpenDetailViewCommand() : base()
        {
            AddAction(typeof(MainWindowViiewModel), OpenTabItem);
            AddAction(typeof(ListItemViewModelBase), OpenDetailView);
        }
        
        private static void OpenTabItem(object parameter) => (parameter as MainWindowViiewModel).OpenTabItem(new VM());
        private static void OpenDetailView(object parameter) => (parameter as ListItemViewModelBase).OpenDetailView();
    }
}
