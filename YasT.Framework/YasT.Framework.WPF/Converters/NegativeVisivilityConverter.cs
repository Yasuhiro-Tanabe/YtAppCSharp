using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace YasT.Framework.WPF.Converters
{
    /// <summary>
    /// 真理値→画面表示状態コンバータ(負論理)
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility), ParameterType = typeof(string))]
    public class NegativeVisivilityConverter : IValueConverter
    {
        /// <summary>
        /// 真理値を WPF UI コンポーネントの画面表示状態に変換する：真のときパラメータで指定された非表示状態 (<see cref="Visibility.Hidden"/> または <see cref="Visibility.Collapsed"/>)、
        /// 偽のとき表示状態(<see cref="Visibility.Visible"/>)。
        /// </summary>
        /// <param name="value">入力値(真理値)。</param>
        /// <param name="targetType">出力データ型(<see cref="Visibility"/>)。</param>
        /// <param name="parameter">非表示状態の指定：
        ///   <list type="bullet">
        ///     <item>"Hidden" のときは value = <b>true</b> だったら <see cref="Visibility.Hidden"/>。</item>
        ///     <item>省略時および "Hidden" 以外のときは value = <b>true</b> だったら <see cref="Visibility.Collapsed"/>。</item>
        ///   </list>
        /// </param>
        /// <param name="culture">(使用しない)</param>
        /// <returns>コンポーネントの表示状態。</returns>
        /// <exception cref="NullReferenceException">value が入力されていない。</exception>
        /// <exception cref="InvalidCastException">value が真理値ではない。</exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new NullReferenceException(nameof(value));
            }
            else if (value is bool)
            {
                if ((bool)value)
                {
                    var str = parameter as string;
                    if (!string.IsNullOrWhiteSpace(str) && str == "Hidden")
                    {
                        return Visibility.Hidden;
                    }
                    else
                    {
                        return Visibility.Collapsed;
                    }
                }
                else
                {
                    return Visibility.Visible;
                }
            }
            else
            {
                throw new InvalidCastException($"{nameof(value)} is expected bool, but {value.GetType().Name}.");
            }
        }

        /// <summary>
        /// WPF UI コンポーネントの表示状態を真理値に変換する：<see cref="Convert(object, Type, object, CultureInfo)"/> の逆変換。
        /// </summary>
        /// <param name="value">入力値(<see cref="Visibility"/>)。</param>
        /// <param name="targetType">出力データ型(bool)。</param>
        /// <param name="parameter">(使用しない)</param>
        /// <param name="culture">(使用しない)</param>
        /// <returns>真理値。</returns>
        /// <exception cref="NullReferenceException">value が入力されていない。</exception>
        /// <exception cref="InvalidCastException">value が <see cref="Visibility"/> ではない。</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                throw new NullReferenceException(nameof(value));
            }
            else if (value is Visibility)
            {
                return (Visibility)value != Visibility.Visible;
            }
            else
            {
                throw new InvalidCastException($"{nameof(value)} is expected Visivility, but {value.GetType().Name}.");
            }
        }
    }
}
