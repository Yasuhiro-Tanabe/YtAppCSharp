using Microsoft.Win32;

namespace SVGEditor
{
    internal class SaveAsNewFileCommand : CommandBase
    {
        public void Execute()
        {
            var dialog = new SaveFileDialog()
            {
                Filter = "SVGファイル (*.svg)|*.svg|XMLファイル (*.xml)|*.xml|すべてのファイル (*.*)|*.*",
                DefaultExt = ".svg",
                OverwritePrompt = true,
                CheckPathExists = true,
            };

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                ViewModel.SvgFileName = dialog.FileName;
                Model.SaveToFile(ViewModel.SvgFileName, ViewModel.SourceCode);
            }
        }
    }
}
