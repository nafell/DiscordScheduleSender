using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DiscordScheduleSender.VIews.Converters
{
    public class DateOnlyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateOnly = (DateOnly)value;

            return dateOnly.ToMonthDayJapaneseString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private const char splitChar = '_';

        public string XmlConvert(DateOnly value)
        {
            return $"{value.Year}{splitChar}{value.Month}{splitChar}{value.Day}";
        }

        public DateOnly XmlConvertBack(string dateString)
        {
            var splitted = dateString.Split(splitChar);

            try
            {
                var dateOnly = new DateOnly(int.Parse(splitted[0]), int.Parse(splitted[1]), int.Parse(splitted[2]));

                return dateOnly;
            }
            catch
            {
                return new DateOnly();
            }
        }
    }
}
