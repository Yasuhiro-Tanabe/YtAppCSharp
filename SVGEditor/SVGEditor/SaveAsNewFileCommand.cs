using Microsoft.Win32;

namespace SVGEditor
{
    internal class SaveAsNewFileCommand : CommandBase
    {
        public override void Execute(object parameter)
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
                (parameter as MainWindowViewModel).SaveToFile(dialog.FileName);
            }
        }
    }
}
