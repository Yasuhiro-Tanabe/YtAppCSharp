using System.Windows;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ExitCommand : CommandBase
    {
        public ExitCommand() : base(typeof(NotificationObject), Shutdown) { }

        private static void Shutdown(object unused) => Application.Current.Shutdown();
    }
}
