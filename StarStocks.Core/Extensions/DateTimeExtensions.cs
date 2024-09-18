// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Collections;

namespace StarStocks.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToDateTime(this string s,
                  string format = "ddMMyyyy", string cultureString = "tr-TR")
        {
            try
            {
                var r = DateTime.ParseExact(
                    s: s,
                    format: format,
                    provider: CultureInfo.GetCultureInfo(cultureString));
                return r;
            }
            catch (FormatException ex)
            {
                throw ex;
            }
            catch (CultureNotFoundException ex)
            {
                throw ex; // Given Culture is not supported culture
            }
        }

        public static DateTime ToDateTime(this string s,
                    string format, CultureInfo culture)
        {
            try
            {
                var r = DateTime.ParseExact(s: s, format: format,
                                        provider: culture);
                return r;
            }
            catch (FormatException ex)
            {
                throw ex;
            }
            catch (CultureNotFoundException ex)
            {
                throw ex; // Given Culture is not supported culture
            }

        }

        public static DateTime UnixTimeMillisecondsToUtcDatetime(this long time)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(time);
        }

        public static DateTime UnixTimeMillisecondsToUtcDatetime(this double time)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc) + TimeSpan.FromMilliseconds(time);
        }

        public static DateTime ToEstNy(this DateTime time)
        {
            var timeUtc = time.ToUniversalTime();
            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);
        }

        /// <summary>
        /// example:
        /// DateTime? dt = DateTime.Now;
        /// dt.ToStringOrDefault("yyyy-MM-dd hh:mm:ss"); 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="format"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string ToStringOrDefault(this DateTime? source, string format, string defaultValue)
        {
            if (source != null)
            {
                return source.Value.ToString(format);
            }
            else
            {
                return String.IsNullOrEmpty(defaultValue) ? String.Empty : defaultValue;
            }

        }

        public static string ToStringOrDefault(this DateTime? source, string format)
        {
            return ToStringOrDefault(source, format, null);
        }

        /// <summary>
        ///     當 DateTime 物件為 MinValue, 回傳 null
        /// </summary>
        public static DateTime? NullIfZero(this DateTime source)
        {
            return source == DateTime.MinValue ? (DateTime?)null : source;
        }

        [Obsolete("使用 GetValueOrDefault() ")]
        public static DateTime DateTimeValue(this DateTime? d)
        {
            return d == null ? DateTime.MinValue : (DateTime)d;
        }

        /// <summary>
        ///     <code>
        ///         d.Trim( TimeSpan.TicksPerDay );        // 2015-02-03 00:00:00.000
        ///         d.Trim( TimeSpan.TicksPerHour );       // 2015-02-03 20:00:00.000
        ///         d.Trim( TimeSpan.TicksPerMinute );     // 2015-02-03 20:58:00.000
        ///         d.Trim( TimeSpan.TicksPerSecond );     // 2015-02-03 20:58:27.000
        ///         d.Trim( TimeSpan.TicksPerMillisecond ) // 2015-02-03 20:58:27.180
        ///     </code>
        /// </summary>
        public static DateTime Truncate(this DateTime date, long roundTicks)
        {
            return new DateTime(date.Ticks - date.Ticks % roundTicks);
        }

        /// <summary>
        /// Converts a given DateTime into a Unix timestamp
        /// </summary>
        /// <param name="value">Any DateTime</param>
        /// <returns>The given DateTime in Unix timestamp format</returns>
        public static int ToUnixTimestamp(this DateTime value)
        {
            return (int)Math.Truncate((value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }

        /// <summary>
        /// Gets a Unix timestamp representing the current moment
        /// </summary>
        /// <param name="ignored">Parameter ignored</param>
        /// <returns>Now expressed as a Unix timestamp</returns>
        public static int UnixTimestamp(this DateTime ignored)
        {
            return (int)Math.Truncate((DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds);
        }

        public static DateTime UnixTimeStampToDateTime(this int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();

            return dtDateTime;
        }
    }
}
