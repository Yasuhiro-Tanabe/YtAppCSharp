using System.Windows;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ExitCommand : CommandBase<NotificationObject>
    {
        protected override void Execute(NotificationObject parameter) => Application.Current.Shutdown();
    }
}
