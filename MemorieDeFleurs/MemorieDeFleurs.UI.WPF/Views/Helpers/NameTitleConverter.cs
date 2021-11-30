using System;
using System.Globalization;
using System.Windows.Data;

namespace MemorieDeFleurs.UI.WPF.Views.Helpers
{
    /// <summary>
    /// 人名にに敬称をつけるコンバータ： AAA → AAA 様
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public class NameTitleConverter : IValueConverter
    {
        /// <summary>
        /// 変換する： AAA → AAA 様
        /// </summary>
        /// <param name="value">変換元の名称(AAA)</param>
        /// <param name="targetType">(使用しない)変換後のデータ型</param>
        /// <param name="parameter">(使用しない)変換パラメータ</param>
        /// <param name="culture">(使用しない)実行環境のカルチャー情報</param>
        /// <returns>変換後の文字列 (AAA 様)</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as string) + " 様";
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
            throw new NotImplementedException("敬称を除去する変換は使わないはず");
        }
    }
}
