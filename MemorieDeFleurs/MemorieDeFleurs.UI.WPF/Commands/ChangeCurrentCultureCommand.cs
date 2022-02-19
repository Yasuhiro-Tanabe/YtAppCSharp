using MemorieDeFleurs.UI.WPF.Views.Helpers;

using System.Globalization;

using YasT.Framework.Logging;
using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// 画面表示言語切替コマンド
    /// </summary>
    public class ChangeCurrentCultureCommand : CommandBase<string>
    {
        /// <inheritdoc/>
        protected override void Execute(string parameter)
        {
            TextResourceManager.Instance.UpdateCultureInfo(new CultureInfo(parameter));
            LogUtil.Info($"Lanuage chaned: {parameter}");
        }
    }
}
