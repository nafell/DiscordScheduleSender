using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordScheduleSender
{
    public static class DateTimeOnlyExtentions
    {
        static CultureInfo Japanese = new CultureInfo("ja-JP");

        public static string ToHourMinuteString(this TimeOnly timeOnly)
        {
            return timeOnly.ToString("HH:mm");
        }

        public static string ToMonthDayJapaneseString(this DateOnly dateOnly)
        {
            return dateOnly.ToString("M/d(ddd)", Japanese);
        }
    }
}
