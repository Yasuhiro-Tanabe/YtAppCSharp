using System.Windows;

namespace SVGEditor
{
    internal class RenderCommand : CommandBase
    {
        protected override void Execute()
        {
            ViewModel.RefreshImage();
        }
    }
}
