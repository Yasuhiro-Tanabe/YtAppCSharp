using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MemorieDeFleurs.UI.WPF.Views.Helpers
{
    [ValueConversion(typeof(Visibility), typeof(Visibility))]
    public class NegativeVisivilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Visibility)
            {
                return (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
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
