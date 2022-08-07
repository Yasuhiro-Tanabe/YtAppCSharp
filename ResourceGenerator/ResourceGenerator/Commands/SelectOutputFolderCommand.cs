using Microsoft.WindowsAPICodePack.Dialogs;

using ResourceGenerator.Resources;
using ResourceGenerator.ViewModels;

using YasT.Framework.WPF;

namespace ResourceGenerator.Commands
{
    /// <summary>
    /// 出力先フォルダ名を指定するコマンド。
    /// </summary>
    public class SelectOutputFolderCommand : CommandBase<SettingControlViewModel>
    {
        /// <summary>
        /// 出力先フォルダを指定する。
        /// </summary>
        /// <param name="vm">設定タブのビューモデル。</param>
        protected override void Execute(SettingControlViewModel vm)
        {
            var dlg = new CommonOpenFileDialog()
            {
                Title = ResourceFinder.FindText("OutputFolder"),
                IsFolderPicker = true,
            };

            if(dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                vm.OutputFolderName = dlg.FileName;
            }
        }
    }
}
