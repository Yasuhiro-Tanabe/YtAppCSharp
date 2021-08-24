using System.Windows;
using System.Windows.Controls;

namespace DDLGenerator.Views
{
    /// <summary>
    /// ScrollViewer 用の添付プロパティ
    /// </summary>
    public static class ScrollViewerHelper
    {

        #region AutoScroll プロパティ
        public static bool GetAutoScrollToBottom(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToBottom);
        }

        public static void SetAutoScrollToBottom(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToBottom, value);
        }

        public static readonly DependencyProperty AutoScrollToBottom =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(ScrollViewerHelper), new PropertyMetadata(false, AutoScrollToBottomPropertyChanged));

        private static void AutoScrollToBottomPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sv = d as ScrollViewer;

            if (sv != null && (bool)e.NewValue)
            {
                sv.ScrollToBottom();
            }
        }
        #endregion
    }
}
