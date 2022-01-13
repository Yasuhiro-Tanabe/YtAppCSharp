using MemorieDeFleurs.UI.WPF.Views.Helpers;

using System.Globalization;

using YasT.Framework.Logging;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// 画面表示言語切替コマンド
    /// </summary>
    public class ChangeCurrentCultureCommand : CommandBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ChangeCurrentCultureCommand() : base(typeof(string), ChangeCurrentCulture) { }

        private static void ChangeCurrentCulture(object parameter)
        {
            TextResourceManager.Instance.UpdateCultureInfo(new CultureInfo(parameter as string));
            LogUtil.Info($"Lanuage chaned: {parameter as string}");
        }
    }
}
