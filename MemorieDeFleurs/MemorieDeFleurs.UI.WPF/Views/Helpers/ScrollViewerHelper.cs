using System.Windows;
using System.Windows.Controls;

namespace MemorieDeFleurs.UI.WPF.Views.Helpers
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
            if(d is ScrollViewer && (bool)e.NewValue)
            {
                (d as ScrollViewer).ScrollToBottom();
            }
        }
        #endregion
    }
}
