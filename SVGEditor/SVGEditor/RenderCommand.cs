using System.Windows;

using YasT.Framework.WPF;

namespace SVGEditor
{
    internal class RenderCommand : CommandBase<MainWindowViewModel>
    {
        protected override void Execute(MainWindowViewModel vm)
        {
            if(vm != null)
            {
                var code = (App.Current.MainWindow as MainWindow)?.Editor?.Text;
                if(!string.IsNullOrEmpty(code))
                {
                    vm.SvgImage = SVGEditorModel.Instance.RenderToImage(code);
                }
            }
        }
    }
}
