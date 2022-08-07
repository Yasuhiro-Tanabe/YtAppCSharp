using Microsoft.WindowsAPICodePack.Dialogs;

using ResourceGenerator.Resources;
using ResourceGenerator.ViewModels;

using System;
using System.IO;

using YasT.Framework.Logging;
using YasT.Framework.WPF;

namespace ResourceGenerator.Commands
{
    /// <summary>
    /// 新規リソース管理ファイルを開くコマンド。
    /// </summary>
    public class NewResourceManagementFileCommand : CommandBase<SettingControlViewModel>
    {
        /// <summary>
        /// テンプレートファイルを使って新規リソース管理ファイルを開く。
        /// </summary>
        /// <param name="vm">設定タブのビューモデル。</param>
        /// <exception cref="NotImplementedException"></exception>
        protected override void Execute(SettingControlViewModel vm)
        {
            var dlg = new CommonSaveFileDialog()
            {
                Title = ResourceFinder.FindText("InputFile"),
                DefaultFileName = ResourceFinder.FindText("DefaultResourceManagementFileName"),
                DefaultExtension = ResourceFinder.FindText("xlsx"),
            };

            if(dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var newFileName = dlg.FileAsShellObject.ParsingName;
                if (File.Exists(newFileName))
                {
                    if(!this.PopupOkCancel(ResourceFinder.FindText("QueryFileExists", newFileName)))
                    {
                        return;
                    }
                }

                try
                {
                    File.Copy(ResourceFinder.FindText("DefaultResourceManagementFilePath"), newFileName);
                    LogUtil.Info($"File {newFileName} is created.");
                }
                catch(Exception ex)
                {
                    throw new ApplicationException(ResourceFinder.FindText("CannotCreateNewResourceManagementFile", newFileName, ex.Message), ex);
                }
                vm.InputFileName = newFileName;
                vm.Model.OpenResourceManagementModel(vm.InputFileName);
                vm.OutputFolderName = vm.Model.OutputPath;
                vm.Namespace = vm.Model.Namespace;
            }
        }
    }
}
