namespace SVGEditor
{
    internal class SaveToFileCommand : CommandBase
    {
        public void Execute()
        {
            if(string.IsNullOrWhiteSpace(ViewModel.SvgFileName))
            {
                ViewModel.SaveAs.Execute();
            }
            else
            {
                Model.SaveToFile(ViewModel.SvgFileName, ViewModel.SourceCode);
            }
        }
    }
}
