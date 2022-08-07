using ResourceGenerator.Resources;
using ResourceGenerator.ViewModels;

using YasT.Framework.Logging;
using YasT.Framework.WPF;

namespace ResourceGenerator.Commands
{
    /// <summary>
    /// リソースファイル生成コマンド。
    /// </summary>
    public class GenerateResourceFilesCommand : CommandBase<SettingControlViewModel>
    {
        /// <summary>
        /// リソースファイルを生成する。
        /// </summary>
        /// <param name="vm">コマンドが関連付けられたビューモデル。</param>
        protected override void Execute(SettingControlViewModel vm)
        {
            if(string.IsNullOrWhiteSpace(vm.InputFileName))
            {
                this.PopupError(ResourceFinder.FindText("InputFileNameIsNotFound"));
                return;
            }
            if (string.IsNullOrWhiteSpace(vm.OutputFolderName))
            {
                this.PopupError(ResourceFinder.FindText("OutputFolderNameIsNotFound"));
                return;
            }
            if ((vm.GenerateResourceFinderSampleCode || vm.GenerateResourceManagerSampleCode || vm.GenerateAppXamlResourceSampleCode)
                && string.IsNullOrWhiteSpace(vm.Namespace))
            {
                this.PopupError(ResourceFinder.FindText("NamespaceUndefined"));
                return;
            }

            vm.Model.GenerateResourceFiles(vm.InputFileName, vm.OutputFolderName);
            LogUtil.Info($"Resource files generated from file '{vm.InputFileName}' to under the folder '{vm.OutputFolderName}'");

            if(vm.GenerateResourceFinderSampleCode)
            {
                vm.Model.GenerateResourceFinderSampleCode(vm.Namespace);
            }
            if(vm.GenerateResourceManagerSampleCode)
            {
                vm.Model.GenerateResourceManagerSampleCode(vm.Namespace, vm.GenerateResourceFinderSampleCode);
            }
            if(vm.GenerateAppXamlResourceSampleCode)
            {
                vm.Model.GenerateAppXamlResourceDictionarySampleCode(vm.Namespace, vm.OutputFolderName, vm.SelectedCulture);
            }
        }
    }
}
