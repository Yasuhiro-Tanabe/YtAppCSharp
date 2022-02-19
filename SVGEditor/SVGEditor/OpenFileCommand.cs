using Microsoft.Win32;

using YasT.Framework.WPF;

namespace SVGEditor
{
    internal class OpenFileCommand : CommandBase<MainWindowViewModel>
    {
        protected override void Execute(MainWindowViewModel vm)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "SVG File(*.svg)|*.svg",
                CheckPathExists = true,
            };

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                vm.LoadFile(dialog.FileName);
            }
        }
    }
}
