using Microsoft.Win32;

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
                (parameter as MainWindowViewModel).LoadFile(dialog.FileName);
            }
        }
    }
}
