using System.Windows;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class ExitCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
