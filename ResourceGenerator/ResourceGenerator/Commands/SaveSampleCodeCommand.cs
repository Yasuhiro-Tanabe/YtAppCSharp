using Microsoft.WindowsAPICodePack.Dialogs;

using ResourceGenerator.Resources;
using ResourceGenerator.ViewModels;

using System.IO;

using YasT.Framework.Logging;
using YasT.Framework.WPF;

namespace ResourceGenerator.Commands
{
    /// <summary>
    /// サンプルコードをファイル保存するコマンド。
    /// </summary>
    public class SaveSampleCodeCommand : CommandBase<SampleCodeViewModel>
    {
        /// <summary>
        /// サンプルコードをファイル保存する。
        /// </summary>
        /// <param name="vm">コマンドが関連付けられたビューモデル。</param>
        protected override void Execute(SampleCodeViewModel vm)
        {
            var ext = Path.GetExtension(vm.DefaultFileName).Substring(1); // 先頭の "." を除去した文字列
            var dlg = new CommonSaveFileDialog()
            {
                DefaultFileName = vm.DefaultFileName,
                OverwritePrompt = true,
                DefaultExtension = Path.GetExtension(vm.DefaultFileName),
            };
            dlg.Filters.Add(new CommonFileDialogFilter(ext, ResourceFinder.FindText($"Extensions.{ext}")));

            if(CommonFileDialogResult.Ok == dlg.ShowDialog())
            {
                var path = dlg.FileName;
                using (var writer = new StreamWriter(path))
                {
                    writer.WriteLine(vm.Code);
                    writer.Flush();
                }
                LogUtil.Info($"File {path} is written.");
            }
        }
    }
}
