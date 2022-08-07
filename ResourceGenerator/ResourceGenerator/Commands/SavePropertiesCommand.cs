using ResourceGenerator.Resources;
using ResourceGenerator.ViewModels;

using YasT.Framework.WPF;

namespace ResourceGenerator.Commands
{
    /// <summary>
    /// 画面に表示されているプロパティをリソース管理ファイルに保存するコマンド。
    /// </summary>
    public class SavePropertiesCommand : CommandBase<SettingControlViewModel>
    {
        /// <summary>
        /// 画面に憑依されているプロパティをリソース管理ファイルに保存する。
        /// </summary>
        /// <param name="vm">コマンドが関連付けられたビューモデル。</param>
        protected override void Execute(SettingControlViewModel vm)
        {
            if(string.IsNullOrWhiteSpace(vm.InputFileName))
            {
                this.PopupError(ResourceFinder.FindText("InputFileNameIsNotFound"));
                return;
            }

            vm.Model.OutputPath = vm.OutputFolderName;
            vm.Model.Namespace = vm.Namespace;
            vm.Model.SaveResourceProperties(vm.InputFileName);
        }
    }
}
