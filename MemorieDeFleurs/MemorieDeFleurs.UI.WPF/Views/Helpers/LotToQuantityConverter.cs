using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.ViewModels;

using System;
using System.Globalization;
using System.Windows.Data;

namespace MemorieDeFleurs.UI.WPF.Views.Helpers
{
    [ValueConversion(typeof(PartsListItemViewModel), typeof(string))]
    internal class LotToQuantityConverter : IValueConverter
    {
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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("逆変換は使用しないはず");
        }
    }
}
