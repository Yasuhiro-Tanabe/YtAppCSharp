namespace SVGEditor
{
    internal class SaveToFileCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            (parameter as MainWindowViewModel).SaveToCurrentFile();
        }
    }
}
