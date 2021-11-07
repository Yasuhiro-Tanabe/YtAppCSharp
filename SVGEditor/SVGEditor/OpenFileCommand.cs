using Microsoft.Win32;

using System.Windows.Input;

namespace SVGEditor
{
    internal class OpenFileCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "SVG File(*.svg)|*.svg",
                CheckPathExists = true,
            };

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                SVGEditorModel.Instance.Load(dialog.FileName);
                (parameter as MainWindowViewModel).SvgFileName = dialog.FileName;
            }
        }
    }
}
