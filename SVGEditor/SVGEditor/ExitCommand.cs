using System.Windows;

using YasT.Framework.WPF;

namespace SVGEditor
{
    internal class ExitCommand : CommandBase<MainWindowViewModel>
    {
        protected override void Execute(MainWindowViewModel unUsed)
        {
            Application.Current.Shutdown();
        }
    }
}
