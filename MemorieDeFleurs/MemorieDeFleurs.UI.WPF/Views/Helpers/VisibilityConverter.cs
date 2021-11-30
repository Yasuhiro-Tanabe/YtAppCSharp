using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MemorieDeFleurs.UI.WPF.Views.Helpers
{
    /// <summary>
    /// 真理値を <see cref="UIElement.Visibility"/> の値に変換する、正論理のコンバータ (真のとき表示、偽のとき非表示)
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    internal class VisibilityConverter : IValueConverter
    {
        /// <summary>
        /// 変換する
        /// </summary>
        /// <param name="value">真理値</param>
        /// <param name="targetType">(使用しない)変換後のデータ型</param>
        /// <param name="parameter">(使用しない)変換パラメータ</param>
        /// <param name="culture">(使用しない)実行環境のカルチャー情報</param>
        /// <returns>変換結果：value が真のとき <see cref="Visibility.Visible"/>、偽のとき <see cref="Visibility.Collapsed"/></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 逆変換する
        /// </summary>
        /// <param name="value">現在の <see cref="Visibility"/></param>
        /// <param name="targetType">(使用しない)変換後のデータ型</param>
        /// <param name="parameter">(使用しない)変換パラメータ</param>
        /// <param name="culture">(使用しない)実行環境のカルチャー情報</param>
        /// <returns>変換結果：value が <see cref="Visibility.Visible"/> のとき真、そうでないとき偽</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return false;
            }
            else
            {
                return (Visibility)value == Visibility.Visible;
            }
        }
    }
}
