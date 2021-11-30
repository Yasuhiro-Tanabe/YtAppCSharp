using System;
using System.Globalization;
using System.Windows.Data;

namespace MemorieDeFleurs.UI.WPF.Views.Helpers
{
    /// <summary>
    /// 商品のリードタイムから最短お届け日 (当日起点) を計算・表示するコンバータ
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    public class MinimumDeliveryDateConverter : IValueConverter
    {
        /// <summary>
        /// リードタイム→最短お届け日の変換を行う
        /// </summary>
        /// <param name="value">リードタイム</param>
        /// <param name="targetType">(使用しない)変換後のデータ型</param>
        /// <param name="parameter">(使用しない)変換パラメータ</param>
        /// <param name="culture">(使用しない)実行環境のカルチャー情報</param>
        /// <returns>最短お届け日 (<see cref="DateTime.Today"/> + value 日)</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int)
            {
                return DateTime.Today.AddDays((int)value).ToString("yyyy/MM/dd");
            }
            else
            {
                throw new NotImplementedException($"{GetType().Name}.{nameof(Convert)}({value.ToString()}, {targetType.Name}, {parameter?.ToString()}, {culture.Name})");
            }
        }

        /// <summary>
        /// 逆変換は行わない
        /// </summary>
        /// <param name="value">(使用しない)</param>
        /// <param name="targetType">(使用しない)</param>
        /// <param name="parameter">(使用しない)</param>
        /// <param name="culture">(使用しない)</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">使用しないはずの逆変換を使用した：ポカヨケ用</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException($"{GetType().Name}.{nameof(ConvertBack)}({value.ToString()}, {targetType.Name}, {parameter?.ToString()}, {culture.Name})");
        }
    }
}
