using System;
using System.Globalization;
using System.Windows.Data;

namespace MemorieDeFleurs.UI.WPF.Views.Helpers
{
    [ValueConversion(typeof(int), typeof(string))]
    public class MinimumDeliveryDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int)
            {
                return DateTime.Now.AddDays((int)value).ToString("yyyy/MM/dd");
            }
            else
            {
                throw new NotImplementedException($"{GetType().Name}.{nameof(Convert)}({value.ToString()}, {targetType.Name}, {parameter?.ToString()}, {culture.Name})");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException($"{GetType().Name}.{nameof(ConvertBack)}({value.ToString()}, {targetType.Name}, {parameter?.ToString()}, {culture.Name})");
        }
    }
}
