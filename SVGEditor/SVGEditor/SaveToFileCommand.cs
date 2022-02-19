using YasT.Framework.WPF;

namespace SVGEditor
{
    internal class SaveToFileCommand : CommandBase<MainWindowViewModel>
    {
        protected override void Execute(MainWindowViewModel vm)
        {
            vm.SaveToCurrentFile();
        }
    }
}
