using System;

namespace Abp.Timing
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Truncates the specified round ticks.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="roundTicks">
        /// eg : 5/26/2017 09:32:43
        /// <para/>TimeSpan.TicksPerMillisecond = 10000 => 5/26/2017 09:32:43;
        /// <para/>TimeSpan.TicksPerSecond = 10000000 => 5/26/2017 09:32:43;
        /// <para/>TimeSpan.TicksPerMinute = 600000000 => 5/26/2017 09:32:00;
        /// <para/>TimeSpan.TicksPerHour = 36000000000 => 5/26/2017 09:00:00;
        /// <para/>TimeSpan.TicksPerDay = 864000000000 => 5/26/2017 00:00:00;        
        /// </param>
        /// <returns></returns>
        public static DateTime? Truncate(this DateTime? date, long roundTicks)
        {
            if (!date.HasValue)
                return date;

            return new DateTime(date.Value.Ticks - date.Value.Ticks % roundTicks);
        }

        /// <summary>
        /// To the short date.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string ToShortDate(this DateTime? dt, string format = "yyyy-MM-dd")
        {
            return dt.HasValue ? dt.Value.ToString(format) : string.Empty;
        }

        /// <summary>
        /// To the short date.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string ToShortDate(this DateTime dt, string format = "yyyy-MM-dd")
        {
            return dt.ToString(format);
        }

        /// <summary>
        /// To the long date time.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string ToLongDateTime(this DateTime? dt, string format = "yyyy-MM-dd HH:mm")
        {
            return dt.HasValue ? dt.Value.ToString(format) : string.Empty;
        }

        /// <summary>
        /// To the long date time.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string ToLongDateTime(this DateTime dt, string format = "yyyy-MM-dd HH:mm")
        {
            return dt.ToString(format);
        }

        /// <summary>
        /// To the short time.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string ToShortTime(this DateTime? dt, string format = "HH:mm")
        {
            return dt.HasValue ? dt.Value.ToString(format) : string.Empty;
        }

        /// <summary>
        /// To the short time.
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="format">The format.</param>
        /// <returns></returns>
        public static string ToShortTime(this DateTime dt, string format = "HH:mm")
        {
            return dt.ToString(format);
        }      
    }
}
