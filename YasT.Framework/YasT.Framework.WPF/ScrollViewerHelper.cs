using System.Windows;
using System.Windows.Controls;

namespace YasT.Framework.WPF
{
    /// <summary>
    /// ScrollViewer 用の添付プロパティ
    /// </summary>
    public static class ScrollViewerHelper
    {
        #region AutoScroll プロパティ
        /// <summary>
        /// <see cref="AutoScrollToBottom"/> プロパティの値を取得する
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetAutoScrollToBottom(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToBottom);
        }

        /// <summary>
        /// <see cref="AutoScrollToBottom"/> プロパティの値を変更する
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetAutoScrollToBottom(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToBottom, value);
        }

        /// <summary>
        /// 自動的に最下部までスクロールするかどうか
        /// 
        /// <see cref="TextBlock"/> や <see cref="TextBox"/> を囲う <see cref="ScrollViewer"/> に添付する。
        /// 真のときテキストブロックの内容変更に合わせて随時テキストの末尾が表示されるように移動する
        /// </summary>
        public static readonly DependencyProperty AutoScrollToBottom =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(ScrollViewerHelper), new PropertyMetadata(false, AutoScrollToBottomPropertyChanged));

        /// <summary>
        /// <see cref="AutoScrollToBottom"/> プロパティ変更時に呼び出されるイベントハンドラ
        /// 
        /// <see cref="AutoScrollToBottom"/> プロパティが真だったとき、プロパティの添付されている <see cref="ScrollViewer"/> に対して
        /// <see cref="ScrollViewer.ScrollToBottom()"/> を呼び出す
        /// </summary>
        /// <param name="d">送信元オブジェクト</param>
        /// <param name="e">イベントパラメータ</param>
        private static void AutoScrollToBottomPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is ScrollViewer && (bool)e.NewValue)
            {
                (d as ScrollViewer)?.ScrollToBottom();
            }
        }
        #endregion
    }
}
