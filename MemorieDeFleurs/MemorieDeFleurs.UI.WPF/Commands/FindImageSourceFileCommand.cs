using MemorieDeFleurs.UI.WPF.ViewModels;

using Microsoft.Win32;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    internal class FindImageSourceFileCommand : CommandBase<BouquetDetailViewModel>
    {
        protected override void Execute(BouquetDetailViewModel parameter)
        {
            var dialog = new OpenFileDialog();
            dialog.Title = "画像ファイル選択";
            dialog.Filter = "画像ファイル|*.png;*.gif;.jpg;*.bmp";
            dialog.DefaultExt = "*.png";

            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                parameter.ImageFileName = dialog.FileName;
            }
        }
    }
}
