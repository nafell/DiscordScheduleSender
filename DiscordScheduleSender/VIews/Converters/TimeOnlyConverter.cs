using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DiscordScheduleSender.VIews.Converters
{
    public class TimeOnlyConverter : IValueConverter
    {
        private const char splitChar = ':';

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeOnly = (TimeOnly)value;

            return Convert(timeOnly);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((string)value);
        }

        public string Convert(TimeOnly? value)
        {
            if (value == null)
                return "";

            return $"{value.Value.Hour}{splitChar}{value.Value.Minute}";
        }

        public TimeOnly? ConvertBack(string timeString)
        {
            var splitted = timeString.Split(splitChar);

            try
            {
                var timeOnly = new TimeOnly(int.Parse(splitted[0]), int.Parse(splitted[1]));

                return timeOnly;
            }
            catch
            {
                return null;
            }
        }
    }
}
