using YasT.Framework.Logging;
using System.Windows;
using DDLGenerator.ViewModels;
using YasT.Framework.WPF;

namespace DDLGenerator.Commands
{
    /// <summary>
    /// 終了コマンド：アプリケーションを終了する
    /// </summary>
    public class QuitCommand : CommandBase<TabItemControlBase>
    {
        protected override void Execute(TabItemControlBase parameter)
        {
            LogUtil.Debug($"{this.GetType().Name}#Execute() called. parameter={parameter?.GetType().Name}");
            Application.Current.Shutdown();
        }
    }
}
