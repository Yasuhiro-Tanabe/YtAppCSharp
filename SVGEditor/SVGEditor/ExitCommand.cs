using System.Windows;

namespace SVGEditor
{
    internal class ExitCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
