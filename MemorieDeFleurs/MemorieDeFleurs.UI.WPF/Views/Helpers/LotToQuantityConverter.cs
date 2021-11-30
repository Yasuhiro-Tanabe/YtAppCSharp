using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.ViewModels;

using System;
using System.Globalization;
using System.Windows.Data;

namespace MemorieDeFleurs.UI.WPF.Views.Helpers
{
    /// <summary>
    /// 単品情報を元に購入ロット数を本数に変換して表示するコンバータ
    /// </summary>
    [ValueConversion(typeof(PartsListItemViewModel), typeof(string))]
    internal class LotToQuantityConverter : IValueConverter
    {
        /// <summary>
        /// 変換する
        /// </summary>
        /// <param name="value">単品情報を格納した <see cref="PartsListItemViewModel"/></param>
        /// <param name="targetType">(使用しない)変換後のデータ型</param>
        /// <param name="parameter">(省略可)出力される数値につける文言：例えば"本"を指定すると出力文字が "50" ではなく "50本" に変わる。</param>
        /// <param name="culture">(使用しない)実行環境のカルチャー情報</param>
        /// <returns>単品購入ロット数 × 購入単位数 の値</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            LogUtil.DEBUGLOG_MethodCalled($"{value}, {targetType.Name}, {parameter}, {culture.Name}");

            var part = value as PartsListItemViewModel;
            if(part == null) { throw new InvalidOperationException($"変換ルール外：変換元={value?.GetType()?.Name}"); }


            if(parameter == null)
            {
                return (part.Quantity * part.QuantityPerLot).ToString();
            }
            else
            {
                return $"{part.Quantity * part.QuantityPerLot} {parameter}";
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
            throw new NotImplementedException("逆変換は使用しないはず");
        }
    }
}
