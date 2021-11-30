using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MemorieDeFleurs.UI.WPF.Views.Helpers
{
    /// <summary>
    /// 真理値を <see cref="UIElement.Visibility"/> の値に変換する、逆論理のコンバータ (真のとき非表示、偽のとき表示)
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class NegativeVisivilityConverter : IValueConverter
    {
        /// <summary>
        /// 変換する
        /// </summary>
        /// <param name="value">真理値</param>
        /// <param name="targetType">(使用しない)変換後のデータ型</param>
        /// <param name="parameter">(使用しない)変換パラメータ</param>
        /// <param name="culture">(使用しない)実行環境のカルチャー情報</param>
        /// <returns>変換結果：value が真のとき <see cref="Visibility.Collapsed"/>、偽のとき <see cref="Visibility.Visible"/></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return (bool)value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        /// <summary>
        /// 逆変換する
        /// </summary>
        /// <param name="value">現在の <see cref="Visibility"/></param>
        /// <param name="targetType">(使用しない)変換後のデータ型</param>
        /// <param name="parameter">(使用しない)変換パラメータ</param>
        /// <param name="culture">(使用しない)実行環境のカルチャー情報</param>
        /// <returns>変換結果：value が <see cref="Visibility.Visible"/> のとき偽、そうでないとき真</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                return (Visibility)value != Visibility.Visible;
            }
        }
    }
}
