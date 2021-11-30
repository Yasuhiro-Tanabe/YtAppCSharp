using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// DialogWindow 内に表示するビューモデルが実装すべきインタフェース
    /// 
    /// ダイアログ(<see cref="DialogWindow"/>) 自体は別のビューモデル (<see cref="DialogViewModel"/>) を DataContext として保持し、
    /// このインタフェースを実装したビューモデルはダイアログ内にある ContentControl の表示内容となる。
    /// </summary>
    internal interface IDialogViewModel
    {
        /// <summary>
        /// ダイアログパラメータを設定する
        /// </summary>
        /// <param name="param">ダイアログパラメータ
        /// 
        /// 登録した値はダイアログのビューモデル (<see cref="DialogViewModel"/> のプロパティを更新するために使われる。</param>
        public void FillDialogParameters(DialogParameter param);

        /// <summary>
        /// ダイアログの OK ボタンが押されたときの処理を行う
        /// </summary>
        public void DialogOK();

        /// <summary>
        /// ダイアログの CANCEL ボタンが押されたときの処理を行う
        /// </summary>
        public void DialogCancel();
    }
}
