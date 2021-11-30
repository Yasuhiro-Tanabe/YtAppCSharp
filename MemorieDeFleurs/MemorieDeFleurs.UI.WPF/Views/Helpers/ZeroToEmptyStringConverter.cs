using System;
using System.Globalization;
using System.Windows.Data;

namespace MemorieDeFleurs.UI.WPF.Views.Helpers
{
    /// <summary>
    /// 数値ゼロをそのまま表示せず空文字に変換するコンバータ
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    public class ZeroToEmptyStringConverter : IValueConverter
    {
        /// <summary>
        /// 変換する
        /// </summary>
        /// <param name="value">数値</param>
        /// <param name="targetType">(使用しない)変換後のデータ型</param>
        /// <param name="parameter">(使用しない)変換パラメータ</param>
        /// <param name="culture">(使用しない)実行環境のカルチャー情報</param>
        /// <returns>数値がゼロだったら空文字 ""、数値が非ゼロのときは数値をそのまま返す</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
            {
                return string.Empty;
            }
            else if((int)value == 0)
            {
                return string.Empty;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// 逆変換は行わない
        /// </summary>
        /// <param name="value">(使用しない)</param>
        /// <param name="targetType">(使用しない)</param>
        /// <param name="parameter">(使用しない)</param>
        /// <param name="culture">(使用しない)</param>
        /// <returns>(戻り値なし)</returns>
        /// <exception cref="NotImplementedException">使用しないはずの逆変換を使用した：ポカヨケ用</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException($"{GetType().Name}.{nameof(ConvertBack)}() : 逆変換は使わないはず");
        }
    }
}
