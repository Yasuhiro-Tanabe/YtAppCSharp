using System.Windows;
using System.Windows.Input;

namespace YasT.Framework.WPF
{
    /// <summary>
    /// コマンドからの <see cref="MessageBox"/> 呼出を簡略化するための拡張メソッド。
    /// </summary>
    public static class MessageBoxExtension
    {
        /// <summary>
        /// 警告ダイアログを表示する。
        /// </summary>
        /// <param name="cmd">拡張対象のコマンドオブジェクト。</param>
        /// <param name="msg">ダイアログに表示するメッセージ。</param>
        /// <param name="title">(省略可)ダイアログタイトル。省略時は "Warning (呼び出し元コマンドクラス名)" がダイアログタイトルとして表示される。</param>
        /// <param name="image">(通常は省略)ダイアログに表示するイメージアイコン。省略時は Windows の一般的な警告アイコンが表示される。</param>
        public static void PopupWarning(this ICommand cmd, string msg, string title = "", MessageBoxImage image = MessageBoxImage.Warning)
        {
            if(string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show(msg, $"Warning ({cmd.GetType().Name})", MessageBoxButton.OK, image);
            }
            else
            {
                MessageBox.Show(msg, title, MessageBoxButton.OK, image);
            }
        }

        /// <summary>
        /// エラーダイアログを表示する。
        /// </summary>
        /// <param name="cmd">拡張対象のコマンドオブジェクト。</param>
        /// <param name="msg">ダイアログに表示するメッセージ。</param>
        /// <param name="title">(省略可)ダイアログタイトル。省略時は "ERROR (呼び出し元コマンドクラス名)" がダイアログタイトルとして表示される。</param>
        /// <param name="image">(通常は省略)ダイアログに表示するイメージアイコン。省略時は Windows の一般的なエラーアイコンが表示される。</param>
        public static void PopupError(this ICommand cmd, string msg, string title = "", MessageBoxImage image = MessageBoxImage.Error)
        {
            if(string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show(msg, $"ERROR ({cmd.GetType().Name})", MessageBoxButton.OK, image);
            }
            else
            {
                MessageBox.Show(msg, title, MessageBoxButton.OK, image);
            }
        }

        /// <summary>
        /// Yes/No/Cancel 形式の問い合わせダイアログを表示する。
        /// </summary>
        /// <param name="cmd">拡張対象のコマンドオブジェクト。</param>
        /// <param name="msg">ダイアログに表示するメッセージ。</param>
        /// <param name="title">(省略可)ダイアログタイトル。省略時は "Query (呼び出し元コマンドクラス名)" がダイアログタイトルとして表示される。</param>
        /// <param name="image">(通常は省略)ダイアログに表示するイメージアイコン。省略時はアイコンなしでダイアログが表示される。</param>
        /// <returns>ダイアログでユーザーが押したボタンに該当する値：<para><list type="bullet">
        /// <item><see cref="MessageBoxResult.Yes"/> ⇒ Yes ボタンが押された。</item>
        /// <item><see cref="MessageBoxResult.No"/> ⇒ No ボタンが押された。</item>
        /// <item><see cref="MessageBoxResult.Cancel"/> ⇒ Cancel ボタンが押された。</item>
        /// </list></para></returns>
        public static MessageBoxResult PopupYesNoCancel(this ICommand cmd, string msg, string title = "", MessageBoxImage image = MessageBoxImage.None)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return MessageBox.Show(msg, $"Query ({cmd.GetType().Name})", MessageBoxButton.YesNoCancel, image);
            }
            else
            {
                return MessageBox.Show(msg, title, MessageBoxButton.YesNoCancel, image);
            }
        }

        /// <summary>
        /// OK/Cancel 形式の問い合わせダイアログを表示する。
        /// </summary>
        /// <param name="cmd">拡張対象のコマンドオブジェクト。</param>
        /// <param name="msg">ダイアログに表示するメッセージ。</param>
        /// <param name="title">(省略可)ダイアログタイトル。省略時は "Query (呼び出し元コマンドクラス名)" がダイアログタイトルとして表示される。</param>
        /// <param name="image">(通常は省略)ダイアログに表示するイメージアイコン。省略時はアイコンなしでダイアログが表示される。</param>
        /// <returns>ユーザーがダイアログのOKボタンを押したとき真、Cancelボタンを押したとき偽。</returns>
        public static bool PopupOkCancel(this ICommand cmd, string msg, string title = "", MessageBoxImage image = MessageBoxImage.None)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return MessageBox.Show(msg, $"Query ({cmd.GetType().Name})", MessageBoxButton.OKCancel, image) == MessageBoxResult.OK;
            }
            else
            {
                return MessageBox.Show(msg, title, MessageBoxButton.OKCancel, image) == MessageBoxResult.OK;
            }
        }
    }
}
