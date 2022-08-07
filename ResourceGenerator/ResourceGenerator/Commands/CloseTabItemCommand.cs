using ResourceGenerator.ViewModels;

using YasT.Framework.WPF;

namespace ResourceGenerator.Commands
{
    /// <summary>
    /// タブを閉じるコマンド。
    /// </summary>
    public class CloseTabItemCommand : CommandBase<TabItemControlViewModel>
    {
        /// <summary>
        /// vm で指定されたビューモデルに対応するタブを閉じる。
        /// </summary>
        /// <param name="vm"></param>
        protected override void Execute(TabItemControlViewModel vm)
        {
        }
    }
}
