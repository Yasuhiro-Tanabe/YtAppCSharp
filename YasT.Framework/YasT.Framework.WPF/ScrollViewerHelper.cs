using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace YasT.Framework.WPF
{
    /// <summary>
    /// ScrollViewer 用の添付プロパティ
    /// </summary>
    public static class ScrollViewerHelper
    {
        #region AutoScrollToBottom プロパティ
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
        /// <see cref="TextBlock"/> や <see cref="TextBox"/>、<see cref="ScrollViewer"/> に添付する。
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
            if(d is ScrollViewer s)
            {
                if((bool)e.NewValue)
                {
                    s.ScrollChanged += OnScrollChanged;
                }
                else
                {
                    s.ScrollChanged -= OnScrollChanged;
                }
            }
            else if(d is TextBoxBase b)
            {
                if((bool)e.NewValue)
                {
                    b.TextChanged += OnTextChanged;
                }
                else
                {
                    b.TextChanged -= OnTextChanged;
                }
            }
        }
        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender == null) { return; }
            if(sender is TextBoxBase b)
            {
                b.ScrollToEnd();
            }
        }
        private static void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if(sender == null) { return; }
            if(sender is ScrollViewer s)
            {
                if(e.ExtentHeightChange < -1.0 || 1.0 < e.ExtentHeightChange)
                {
                    s.ScrollToBottom();
                }
            }
        }
        #endregion
    }
}
