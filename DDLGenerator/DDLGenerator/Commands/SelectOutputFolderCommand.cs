using YasT.Framework.Logging;
using DDLGenerator.ViewModels;

using Microsoft.WindowsAPICodePack.Dialogs;

using System;
using System.Windows.Input;
using YasT.Framework.WPF;

namespace DDLGenerator.Commands
{
    public class SelectOutputFolderCommand : CommandBase<EFCoreEntityViewModel>
    {
        public void OnGenerationStarted(object sender, EventArgs unused) => ToUnexecutable();

        public void OnGenerationFinished(object sender, EventArgs unused) => ToExecutable();

        protected override void Execute(EFCoreEntityViewModel parameter)
        {
            if (CommonOpenFileDialog.IsPlatformSupported)
            {
                using (var dialog = new CommonOpenFileDialog() { IsFolderPicker = true, Title = "出力先フォルダの選択" })
                {
                    var result = dialog.ShowDialog();
                    if (result == CommonFileDialogResult.Ok)
                    {
                        LogUtil.Info("出力先フォルダ：" + dialog.FileName);

                        LogUtil.Debug($"EFCoreEntityViewModel.OutputFolderPath = '{dialog.FileName}'");
                        parameter.OutputFolderPath = dialog.FileName;
                    }
                }
            }
        }
    }
}
