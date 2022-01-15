using Microsoft.Win32;

using YasT.Framework.WPF;

namespace SVGEditor
{
    internal class SaveAsNewFileCommand : CommandBase<MainWindowViewModel>
    {
        protected override void Execute(MainWindowViewModel vm)
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
                vm.SaveToFile(dialog.FileName);
            }
        }
    }
}
