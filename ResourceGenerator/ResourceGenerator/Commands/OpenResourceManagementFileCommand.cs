using Microsoft.WindowsAPICodePack.Dialogs;

using ResourceGenerator.Resources;
using ResourceGenerator.ViewModels;

using YasT.Framework.WPF;

namespace ResourceGenerator.Commands
{
    /// <summary>
    /// リソース管理ファイルを開くコマンド。
    /// </summary>
    public class OpenResourceManagementFileCommand : CommandBase<SettingControlViewModel>
    {
        /// <summary>
        /// リソース管理ファイルを開く。
        /// </summary>
        /// <param name="vm">設定タブのビューモデル。</param>
        protected override void Execute(SettingControlViewModel vm)
        {
            var dlg = new CommonOpenFileDialog()
            {
                EnsureFileExists = true,
                EnsurePathExists = true,
                Title = ResourceFinder.FindText("InputFile"),
                DefaultExtension = ".xlsx",
                Multiselect = false
            };
            dlg.Filters.Add(new CommonFileDialogFilter("Microsoft Excel files", "xlsx"));

            if(dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                vm.InputFileName = dlg.FileName;
                vm.Model.OpenResourceManagementModel(dlg.FileName);
                vm.OutputFolderName = vm.Model.OutputPath;
                vm.Namespace = vm.Model.Namespace;
                foreach(var c in vm.Model.Cultures)
                {
                    vm.DefaultCultures.Add(c);
                }
            }
        }
    }
}
