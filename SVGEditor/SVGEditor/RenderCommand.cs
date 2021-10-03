using System.Windows;

namespace SVGEditor
{
    internal class RenderCommand : CommandBase
    {
        protected override void Execute()
        {
            MessageBox.Show($"parameter={ViewModel?.GetType().Name}", $"{this.GetType().Name}.Execute()", MessageBoxButton.OK);
        }
    }
}
