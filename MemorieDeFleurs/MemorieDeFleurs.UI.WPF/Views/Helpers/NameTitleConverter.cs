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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as string) + " 様";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("敬称を除去する変換は使わないはず");
        }
    }
}
