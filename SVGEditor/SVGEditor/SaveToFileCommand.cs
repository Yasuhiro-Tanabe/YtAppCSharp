namespace SVGEditor
{
    internal class SaveToFileCommand : CommandBase
    {
        public override void Execute(object parameter)
        {
            SVGEditorModel.Instance.Save((parameter as MainWindowViewModel).SvgFileName);
        }
    }
}
