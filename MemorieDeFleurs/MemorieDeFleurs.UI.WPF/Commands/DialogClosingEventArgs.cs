using MemorieDeFleurs.UI.WPF.Views;

using System;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// ダイアログクローズ時の状態：どのボタンが押下されたか、など
    /// </summary>
    public enum DialogClosingStatus
    {
        /// <summary>
        /// OKボタンが押下された
        /// </summary>
        OK,

        /// <summary>
        /// CANCELボタンが押下された
        /// </summary>
        CANCEL
    }

    /// <summary>
    /// <see cref="DialogWindow"/> クローズ時に通知されるイベントのイベント引数
    /// </summary>
    public class DialogClosingEventArgs : EventArgs
    {
        /// <summary>
        /// ダイアログクローズ時の状態
        /// </summary>
        public DialogClosingStatus Status { get; private set; }

        /// <summary>
        /// ダイアログ内で操作したビューモデル
        /// </summary>
        public NotificationObject DialogViewModel { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="status">ダイアログクローズ時の状態</param>
        /// <param name="viewModel">ダイアログ内で操作したビューモデル</param>
        public DialogClosingEventArgs(DialogClosingStatus status, NotificationObject viewModel)
        {
            Status = status;
            DialogViewModel = viewModel;
        }
    }
}
