namespace SVGEditor
{
    internal class SaveToFileCommand : CommandBase
    {
        protected override void Execute()
        {
            if (string.IsNullOrWhiteSpace(ViewModel.SvgFileName))
            {
                ViewModel.SaveAs.Execute(ViewModel);
            }
            else
            {
                Model.SaveToFile(ViewModel.SvgFileName, ViewModel.SourceCode);
            }
        }
    }
}
