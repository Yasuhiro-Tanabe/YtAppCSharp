using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;
using MemorieDeFleurs.UI.WPF.Views.Helpers;

using System.Windows.Controls;

using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// <see cref="IPrintable"/> 実装に必要な印刷コマンドのベースクラス
    /// </summary>
    public abstract class PrintCommand : CommandBase<IPrintable> 
    {
    }

    /// <summary>
    /// <see cref="IPrintable"/> 実装に必要な印刷コマンドのベースクラス
    /// </summary>
    public abstract class PrintCommand<T> : PrintCommand where T : NotificationObject, IPrintable, IReloadable
    {
        /// <inheritdoc/>
        /// <remarks>
        /// サブクラスではこのメソッドのオーバーライドを許可しない。
        /// 代わりに <see cref="Execute(T)"/> を実装させる。
        /// </remarks>
        protected override sealed void Execute(IPrintable parameter)
        {
            Execute(parameter as T);
        }

        /// <summary>
        /// 印刷処理を実行する。各サブクラスは、このメソッドを実装しなければならない。
        /// </summary>
        /// <param name="parameter">コマンドパラメータ</param>
        protected abstract void Execute(T parameter);
    }
}
