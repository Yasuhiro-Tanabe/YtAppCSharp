using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using System;

namespace MemorieDeFleurs.Models.Converters
{
    internal class DateTimeConverter : ValueConverter<DateTime, int>
    {
        public static int DateTimeToDate(DateTime datetime)
        {
            return datetime.Year * 10000 + datetime.Month * 100 + datetime.Day;
        }

        public static DateTime DateToDateTime(int date)
        {
            return DateTime.Parse($"{date / 10000:0000}-{(date % 10000) / 100}-{date % 100}");
        }

        public DateTimeConverter() : base(d => DateTimeToDate(d), i => DateToDateTime(i))
        {
        }
    }
}
