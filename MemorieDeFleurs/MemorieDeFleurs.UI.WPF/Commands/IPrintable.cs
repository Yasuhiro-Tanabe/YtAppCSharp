using MemorieDeFleurs.UI.WPF.Views.Helpers;

using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// 印刷対象のビューモデルが実装すべきインタフェース
    /// 
    /// 印刷処理 <see cref="UserControlPrinter.PrintDocument{ViewModel, View}(ViewModel)"/> に渡すビューモデルは、
    /// <see cref="NotificationObject"/> を継承しこのインタフェースと <see cref="IReloadable"/> インタフェースを実装しなければならない。
    /// </summary>
    public interface IPrintable
    {
        /// <summary>
        /// 印刷する
        /// </summary>
        public ICommand Print { get; }

        /// <summary>
        /// 印刷前にビューモデルが保持している情報が印刷可能であることを検証する
        /// </summary>
        public void ValidateBeforePrinting();
    }
}
