using System.Windows;

namespace SVGEditor
{
    internal class RenderCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            var vm = parameter as MainWindowViewModel;
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
