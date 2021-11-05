using MemorieDeFleurs.UI.WPF.ViewModels;

using Microsoft.Win32;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class FindImageSourceFileCommand : CommandBase
    {
        public FindImageSourceFileCommand() : base(typeof(BouquetDetailViewModel), FindImageSourceFile) { }

        private static void FindImageSourceFile(object sender)
        {
            if (sender is BouquetDetailViewModel)
            {
                var dialog = new OpenFileDialog();
                dialog.Title = "画像ファイル選択";
                dialog.Filter = "画像ファイル|*.png;*.gif;.jpg;*.bmp";
                dialog.DefaultExt = "*.png";

                var result = dialog.ShowDialog();
                if (result.HasValue && result.Value)
                {
                    (sender as BouquetDetailViewModel).ImageFileName = dialog.FileName;
                }
            }
        }
    }
}
