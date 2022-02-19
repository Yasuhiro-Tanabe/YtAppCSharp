using YasT.Framework.WPF;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// ダイアログを開く側が実装しなければならないビューモデル
    /// 
    /// ダイアログ内での操作は <see cref="IDialogViewModel"/> が処理するため、
    /// 呼び出し元である <see cref="IDialogCaller"/> は直接知ることができない。
    /// 代替策として <see cref="OpenDialogCommand.DialogClosing"/> イベントを公開しているので、
    /// 必要ならこのイベントを購読すること。
    /// </summary>
    public interface IDialogCaller
    {
        /// <summary>
        /// ダイアログを開く
        /// </summary>
        OpenDialogCommand OpenDialog { get; }
        
        /// <summary>
        /// ダイアログ内で表示するビューモデル
        /// 
        /// ダイアログ内で表示するビューモデルは <see cref="NotificationObject"/> を継承し、
        /// <see cref="IDialogViewModel"/> を実装したクラスでなければならない
        /// </summary>
        public NotificationObject DialogViewModel { get; }
    }
}
