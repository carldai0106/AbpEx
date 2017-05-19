using System;

namespace Abp.Timing
{
    public static class DateTimeExtensions
    {
        public static string ToShortDate(this DateTime? dt, string format = "yyyy-MM-dd")
        {
            return dt.HasValue ? dt.Value.ToString(format) : string.Empty;
        }

        public static string ToShortDate(this DateTime dt, string format = "yyyy-MM-dd")
        {
            return dt.ToString(format);
        }

        public static string ToLongDateTime(this DateTime? dt, string format = "yyyy-MM-dd HH:mm")
        {
            return dt.HasValue ? dt.Value.ToString(format) : string.Empty;
        }

        public static string ToLongDateTime(this DateTime dt, string format = "yyyy-MM-dd HH:mm")
        {
            return dt.ToString(format);
        }

        public static string ToShortTime(this DateTime? dt, string format = "HH:mm")
        {
            return dt.HasValue ? dt.Value.ToString(format) : string.Empty;
        }

        public static string ToShortTime(this DateTime dt, string format = "HH:mm")
        {
            return dt.ToString(format);
        }

        public static string ToExcelShortTime(this string time)
        {
            TimeSpan ts;
            return TimeSpan.TryParse(time, out ts) ? ts.ToString(@"h\:mm") : string.Empty;
        }
    }
}
