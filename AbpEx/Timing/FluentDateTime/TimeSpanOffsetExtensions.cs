﻿//Source code from https://github.com/FluentDateTime/FluentDateTime

using System;

namespace Abp.Timing.FluentDateTime
{
    /// <summary>
	/// Static class containing Fluent <see cref="TimeSpan"/> extension methods.
	/// </summary>
	public static class TimeSpanOffsetExtensions
    {

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from current date (<see cref="DateTime.Now"/>) and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTimeOffset Ago(this TimeSpan from)
        {
            return from.Before(DateTimeOffset.Now);
        }

        /// <summary>
        /// Subtracts given <see cref="FluentTimeSpan"/> from current date (<see cref="DateTimeOffset.Now"/>) and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTimeOffset Ago(this FluentTimeSpan from)
        {
            return from.Before(DateTimeOffset.Now);
        }


        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTimeOffset Ago(this TimeSpan from, DateTimeOffset originalValue)
        {
            return from.Before(originalValue);
        }

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTimeOffset Ago(this FluentTimeSpan from, DateTimeOffset originalValue)
        {
            return from.Before(originalValue);
        }

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTimeOffset Before(this TimeSpan from, DateTimeOffset originalValue)
        {
            return originalValue - from;
        }

        /// <summary>
        /// Subtracts given <see cref="TimeSpan"/> from <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the past.
        /// </summary>
        public static DateTimeOffset Before(this FluentTimeSpan from, DateTimeOffset originalValue)
        {
            return originalValue.AddMonths(-from.Months).AddYears(-from.Years).Add(-from.TimeSpan);
        }


        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to current <see cref="DateTime.Now"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public static DateTimeOffset FromNow(this TimeSpan from)
        {
            return from.From(DateTimeOffset.Now);
        }

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to current <see cref="DateTime.Now"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public static DateTimeOffset FromNow(this FluentTimeSpan from)
        {
            return from.From(DateTimeOffset.Now);
        }

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public static DateTimeOffset From(this TimeSpan from, DateTimeOffset originalValue)
        {
            return originalValue + from;
        }

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        public static DateTimeOffset From(this FluentTimeSpan from, DateTimeOffset originalValue)
        {
            return originalValue.AddMonths(from.Months).AddYears(from.Years).Add(from.TimeSpan);
        }

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        /// <seealso cref="From(System.TimeSpan,System.DateTimeOffset)"/>
        /// <remarks>
        /// Synonym of <see cref="From(System.TimeSpan,System.DateTimeOffset)"/> method.
        /// </remarks>
        public static DateTimeOffset Since(this TimeSpan from, DateTimeOffset originalValue)
        {
            return From(from, originalValue);
        }

        /// <summary>
        /// Adds given <see cref="TimeSpan"/> to supplied <paramref name="originalValue"/> <see cref="DateTime"/> and returns resulting <see cref="DateTime"/> in the future.
        /// </summary>
        /// <seealso cref="From(System.TimeSpan,System.DateTimeOffset)"/>
        /// <remarks>
        /// Synonym of <see cref="From(System.TimeSpan,System.DateTimeOffset)"/> method.
        /// </remarks>
        public static DateTimeOffset Since(this FluentTimeSpan from, DateTimeOffset originalValue)
        {
            return From(from, originalValue);
        }


        /// <summary>
        /// Convert a <see cref="TimeSpan"/> to a human readable string.
        /// </summary>
        /// <param name="timeSpan">The <see cref="TimeSpan"/> to convert</param>
        /// <returns>A human readable string for <paramref name="timeSpan"/></returns>
        public static string ToDisplayString(this FluentTimeSpan timeSpan)
        {
            return ((TimeSpan)timeSpan).ToDisplayString();
        }
    }
}