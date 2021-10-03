using Microsoft.Win32;

namespace SVGEditor
{
    internal class OpenFileCommand : CommandBase
    {
        protected override void Execute()
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "SVG File(*.svg)|*.svg",
                CheckPathExists = true,
            };

            var result = dialog.ShowDialog();
            if(result.HasValue && result.Value)
            {
                ViewModel.SvgFileName = dialog.FileName;
                ViewModel.SourceCode = Model.LoadXmlFile(ViewModel.SvgFileName);
            }
        }
    }
}
