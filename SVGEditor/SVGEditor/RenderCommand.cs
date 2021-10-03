using System.Windows;

namespace SVGEditor
{
    internal class RenderCommand : CommandBase
    {
        public void Execute()
        {
            MessageBox.Show($"parameter={ViewModel?.GetType().Name}", $"{this.GetType().Name}.Execute()", MessageBoxButton.OK);
        }
    }
}
