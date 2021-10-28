#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: DateTimeExtensions.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Globalization;

using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Json
{
    /// <summary>
    /// Helper methods for parsing <see cref="DateTime"/>, <see cref="DateTimeOffset"/> and <see cref="TimeSpan"/> instances.
    /// </summary>
    internal static class DateTimeExtensions
    {
        #region Constants

        #region Internal Constants

        internal const string Iso8601JavaScriptFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
        internal const string Iso8601DateFormat = "yyyy'-'MM'-'dd";
        internal const string Iso8601MinutesFormat = "yyyy'-'MM'-'dd'T'HH':'mmK";
        internal const string Iso8601SecondsFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";
        internal const string Iso8601MillisecondsFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK";

        #endregion

        #region Private Constants

        private const long maxDateTimeTicks = 3_155_378_975_999_999_999; // DateTime.MaxValue.Ticks
        private const long minUnixMilliseconds = -62_135_596_800_000; // DateTimeOffset.MinValue.ToUnixTimeMilliseconds()
        private const long maxUnixMilliseconds = 253_402_300_799_999; // DateTimeOffset.MaxValue.ToUnixTimeMilliseconds()
        private const long minUnixSeconds = -62_135_596_800;
        private const long maxUnixSeconds = 253_402_300_799;
        private const long minTimeSpanMilliseconds = -922_337_203_685_477; // TimeSpan.MinValue.Ticks / TimeSpan.TicksPerMillisecond
        private const long maxTimeSpanMilliseconds = 922_337_203_685_477; // TimeSpan.MaxValue.Ticks / TimeSpan.TicksPerMillisecond
        private const long maxTimeOnlyTicks = 863_999_999_999; // TimeOnly.MaxValue.Ticks
        private const long maxTimeOnlyMilliseconds = 86_399_999; // TimeOnly.MaxValue.Ticks / TimeSpan.TicksPerMillisecond

        private const string iso8601FractionalSecondsFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFFFFK";
        private const string iso8601RoundtripFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffK";
        private const string iso8601RoundtripUtcFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff'Z'";
        private const string iso8601RoundtripLocalFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffzzz";
        private const string msPrefix = "/Date(";
        private const string msPostfix = ")/";

        #endregion

        #endregion

        #region Fields

        private static readonly string[] iso8601Formats =
        {
            iso8601FractionalSecondsFormat,
            Iso8601SecondsFormat,
            Iso8601MinutesFormat,
            Iso8601DateFormat + "K",
            "yyyy'-'MMK", // year-month only
            "yyyy'-'MM'-'dd'T'HHK", // hours only
        };

        #endregion

        #region Methods

        #region Internal Methods

        internal static double ToUnixSecondsFloat(this DateTime value) => value.ToUnixMilliseconds() / 1000d;

        internal static string ToMicrosoftJsonDate(this DateTime value)
        {
            long ms = value.ToUnixMilliseconds();
            if (value.Kind != DateTimeKind.Local)
                return $"{msPrefix}{ms}{msPostfix}";
            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(value);
            return $"{msPrefix}{ms}{(offset < TimeSpan.Zero ? '-' : '+')}{Math.Abs(offset.Hours):00}{Math.Abs(offset.Minutes):00}{msPostfix}";
        }

        internal static string ToMicrosoftJsonDate(this DateTimeOffset value)
        {
            long ms = value.UtcDateTime.ToUnixMilliseconds();
            TimeSpan offset = value.Offset;
            return $"{msPrefix}{ms}{(value.Offset < TimeSpan.Zero ? '-' : '+')}{Math.Abs(offset.Hours):00}{Math.Abs(offset.Minutes):00}{msPostfix}";
        }

        internal static bool TryParseDateTime(this string s, JsonDateTimeFormat format, bool isNumber, out DateTime value)
        {
            if (!isNumber || format <= JsonDateTimeFormat.Ticks)
            {
                long longValue;
                switch (format)
                {
                    case JsonDateTimeFormat.Auto:
                        return TryParseDateTimeDetectFormat(s, isNumber, out value);

                    case JsonDateTimeFormat.UnixMilliseconds:
                        if (TryParseInt64(s, minUnixMilliseconds, maxUnixMilliseconds, out longValue))
                        {
                            value = FromUnixMilliseconds(longValue);
                            return true;
                        }

                        break;

                    case JsonDateTimeFormat.UnixSeconds:
                        if (TryParseInt64(s, minUnixSeconds, maxUnixSeconds, out longValue))
                        {
                            value = FromUnixSeconds(longValue);
                            return true;
                        }

                        break;

                    case JsonDateTimeFormat.Ticks:
                        if (TryParseInt64(s, 0L, maxDateTimeTicks, out longValue))
                        {
                            value = new DateTime(longValue, DateTimeKind.Utc);
                            return true;
                        }

                        break;

                    case JsonDateTimeFormat.UnixSecondsFloat:
                        if (TryParseFloatUnixSeconds(s, out double doubleValue))
                        {
                            value = FromUnixMilliseconds((long)(doubleValue * 1000d));
                            return true;
                        }

                        break;

                    // For DateTime using RoundtripKind everywhere to prevent converting everything to local time.
                    // Not using AssumeUniversal even for UTC only formats (Iso8601JavaScript, Iso8601Utc) because the Z in the format would turn the Kind Local.
                    case JsonDateTimeFormat.Iso8601JavaScript:
                        if (!DateTime.TryParseExact(s, Iso8601JavaScriptFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out value))
                            return false;
                        value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        return true;

                    case JsonDateTimeFormat.Iso8601Roundtrip:
                        return DateTime.TryParseExact(s, iso8601RoundtripFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out value);

                    case JsonDateTimeFormat.Iso8601Utc:
                        if (!DateTime.TryParseExact(s, iso8601RoundtripUtcFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out value))
                            return false;
                        value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                        return true;

                    case JsonDateTimeFormat.Iso8601Local:
                        return DateTime.TryParseExact(s, iso8601RoundtripLocalFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out value);

                    case JsonDateTimeFormat.Iso8601Date:
                        return DateTime.TryParseExact(s, Iso8601DateFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out value);

                    case JsonDateTimeFormat.Iso8601Minutes:
                        return DateTime.TryParseExact(s, Iso8601MinutesFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out value);

                    case JsonDateTimeFormat.Iso8601Seconds:
                        return DateTime.TryParseExact(s, Iso8601SecondsFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out value);

                    case JsonDateTimeFormat.Iso8601Milliseconds:
                        return DateTime.TryParseExact(s, Iso8601MillisecondsFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out value);

                    case JsonDateTimeFormat.MicrosoftLegacy:
                        if (TryParseMSDateTime(s, out DateTimeOffset timestamp, out bool hasTimeZone))
                        {
                            value = hasTimeZone ? timestamp.LocalDateTime : timestamp.UtcDateTime;
                            return true;
                        }

                        break;
                }
            }

            value = default;
            return false;
        }

        internal static bool TryParseDateTimeOffset(this string s, JsonDateTimeFormat format, bool isNumber, out DateTimeOffset value)
        {
            if (!isNumber || format <= JsonDateTimeFormat.Ticks)
            {
                long longValue;
                switch (format)
                {
                    case JsonDateTimeFormat.Auto:
                        return TryParseDateTimeOffsetDetectFormat(s, isNumber, out value);

                    case JsonDateTimeFormat.UnixMilliseconds:
                        if (TryParseInt64(s, minUnixMilliseconds, maxUnixMilliseconds, out longValue))
                        {
                            value = new DateTimeOffset(FromUnixMilliseconds(longValue));
                            return true;
                        }

                        break;

                    case JsonDateTimeFormat.UnixSeconds:
                        if (TryParseInt64(s, minUnixSeconds, maxUnixSeconds, out longValue))
                        {
                            value = new DateTimeOffset(FromUnixSeconds(longValue));
                            return true;
                        }

                        break;

                    case JsonDateTimeFormat.Ticks:
                        if (TryParseInt64(s, 0L, maxDateTimeTicks, out longValue))
                        {
                            value = new DateTimeOffset(longValue, TimeSpan.Zero);
                            return true;
                        }

                        break;

                    case JsonDateTimeFormat.UnixSecondsFloat:
                        if (TryParseFloatUnixSeconds(s, out double doubleValue))
                        {
                            value = new DateTimeOffset(FromUnixMilliseconds((long)(doubleValue * 1000d)));
                            return true;
                        }

                        break;

                    // For DateTimeOffset using AssumeUniversal to prevent treating unspecified times as local ones (including UTC times with Z postfix if format contains Z as literal).
                    // AssumeUniversal will not cause conflict if the time contains a time zone, it be simply ignored in such cases.
                    case JsonDateTimeFormat.Iso8601JavaScript:
                        return DateTimeOffset.TryParseExact(s, Iso8601JavaScriptFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out value);

                    case JsonDateTimeFormat.Iso8601Roundtrip:
                        return DateTimeOffset.TryParseExact(s, iso8601RoundtripFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out value);

                    case JsonDateTimeFormat.Iso8601Utc:
                        return DateTimeOffset.TryParseExact(s, iso8601RoundtripUtcFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out value);

                    case JsonDateTimeFormat.Iso8601Local:
                        return DateTimeOffset.TryParseExact(s, iso8601RoundtripLocalFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out value);

                    case JsonDateTimeFormat.Iso8601Date:
                        return DateTimeOffset.TryParseExact(s, Iso8601DateFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out value);

                    case JsonDateTimeFormat.Iso8601Minutes:
                        return DateTimeOffset.TryParseExact(s, Iso8601MinutesFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out value);

                    case JsonDateTimeFormat.Iso8601Seconds:
                        return DateTimeOffset.TryParseExact(s, Iso8601SecondsFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out value);

                    case JsonDateTimeFormat.Iso8601Milliseconds:
                        return DateTimeOffset.TryParseExact(s, Iso8601MillisecondsFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out value);

                    case JsonDateTimeFormat.MicrosoftLegacy:
                        return TryParseMSDateTime(s, out value, out var _);
                }
            }

            value = default;
            return false;
        }

        internal static bool TryParseTimeSpan(this string s, JsonTimeFormat format, bool isNumber, out TimeSpan value, bool timeOfDayOnly = false)
        {
            if (!isNumber || format <= JsonTimeFormat.Ticks)
            {
                long longValue;
                switch (format)
                {
                    case JsonTimeFormat.Auto:
                        return TryParseTimeSpanDetectFormat(s, isNumber, out value, timeOfDayOnly);

                    case JsonTimeFormat.Milliseconds:
                        if (TryParseInt64(s, minTimeSpanMilliseconds, maxTimeSpanMilliseconds, out longValue))
                        {
                            value = new TimeSpan(longValue * TimeSpan.TicksPerMillisecond);
                            return !timeOfDayOnly || value.Ticks is >= 0L and <= maxTimeOnlyTicks;
                        }

                        break;

                    case JsonTimeFormat.Ticks:
                        if (TryParseInt64(s, Int64.MinValue, Int64.MaxValue, out longValue))
                        {
                            value = new TimeSpan(longValue);
                            return !timeOfDayOnly || value.Ticks is >= 0L and <= maxTimeOnlyTicks;
                        }

                        break;

                    case JsonTimeFormat.Text:
#if NET35
                        return TimeSpan.TryParse(s, out value)
#else
                        return TimeSpan.TryParseExact(s, "c", DateTimeFormatInfo.InvariantInfo, out value)
#endif
                            && !timeOfDayOnly || value.Ticks is >= 0L and <= maxTimeOnlyTicks;
                }
            }

            value = default;
            return false;
        }

        #endregion

        #region Private Methods

        private static bool TryParseInt64(string s, long min, long max, out long value)
            => Int64.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value) && value >= min && value <= max;

        private static bool TryParseFloatUnixSeconds(string s, out double value)
            => Double.TryParse(s, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out value) && value is >= minUnixSeconds and <= maxUnixSeconds;

        private static DateTime FromUnixMilliseconds(long milliseconds) => CoreLibraries.DateTimeExtensions.FromUnixMilliseconds(milliseconds);
        private static DateTime FromUnixSeconds(long seconds) => CoreLibraries.DateTimeExtensions.FromUnixSeconds(seconds);

        private static bool TryParseDateTimeDetectFormat(string s, bool isNumber, out DateTime value)
        {
            if (!isNumber)
            {
                // Trying ISO 8601 formats first
                if (s.Length >= 7 && s[4] == '-')
                    return DateTime.TryParseExact(s, iso8601Formats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out value);

                // Legacy Microsoft formats
                if (TryParseMSDateTime(s, out DateTimeOffset timestamp, out bool hasTimeZone))
                {
                    value = hasTimeZone ? timestamp.LocalDateTime : timestamp.UtcDateTime;
                    return true;
                }
            }

            // integer: trying to detect a sensible range
            if (TryParseInt64(s, minUnixMilliseconds, maxDateTimeTicks, out long longValue))
            {
                value = longValue > maxUnixMilliseconds ? new DateTime(longValue, DateTimeKind.Utc)
                    : longValue is >= Int32.MinValue and <= Int32.MaxValue ? FromUnixSeconds(longValue)
                    : FromUnixMilliseconds(longValue);
                return true;
            }

            // double: allowing only Unix time seconds
            if (TryParseFloatUnixSeconds(s, out double doubleValue))
            {
                value = FromUnixMilliseconds((long)(doubleValue * 1000d));
                return true;
            }

            value = default;
            return false;
        }

        private static bool TryParseDateTimeOffsetDetectFormat(string s, bool isNumber, out DateTimeOffset value)
        {
            if (!isNumber)
            {
                // Trying ISO 8601 formats first. AssumeUniversal is for unspecified time zone information to prevent treating it as local time.
                if (s.Length >= 7 && s[4] == '-')
                    return DateTimeOffset.TryParseExact(s, iso8601Formats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out value);

                // Legacy Microsoft formats
                if (TryParseMSDateTime(s, out DateTimeOffset timestamp, out bool hasTimeZone))
                {
                    value = hasTimeZone ? timestamp.LocalDateTime : timestamp.UtcDateTime;
                    return true;
                }
            }

            // integer: trying to detect a sensible range
            if (TryParseInt64(s, minUnixMilliseconds, maxDateTimeTicks, out long longValue))
            {
                value = longValue > maxUnixMilliseconds ? new DateTimeOffset(longValue, TimeSpan.Zero)
                    : longValue is >= Int32.MinValue and <= Int32.MaxValue ? new DateTimeOffset(FromUnixSeconds(longValue))
                    : new DateTimeOffset(FromUnixMilliseconds(longValue));
                return true;
            }

            // double: allowing only Unix time seconds
            if (TryParseFloatUnixSeconds(s, out double doubleValue))
            {
                value = new DateTimeOffset(FromUnixMilliseconds((long)(doubleValue * 1000d)));
                return true;
            }

            value = default;
            return false;
        }

        private static bool TryParseTimeSpanDetectFormat(string s, bool isNumber, out TimeSpan value, bool timeOfDayOnly)
        {
            if (!isNumber)
            {
#if NET35
                if (TimeSpan.TryParse(s, out value))
#else
                if (TimeSpan.TryParseExact(s, "c", DateTimeFormatInfo.InvariantInfo, out value))
#endif
                    return !timeOfDayOnly || value.Ticks is >= 0L and <= maxTimeOnlyTicks;
            }

            // integer: trying to detect a sensible range
            if (TryParseInt64(s, Int64.MinValue, Int64.MaxValue, out long longValue))
            {
                value = !timeOfDayOnly && longValue is >= minTimeSpanMilliseconds and <= maxTimeSpanMilliseconds || timeOfDayOnly && longValue is >= 0L and <= maxTimeOnlyMilliseconds
                    ? new TimeSpan(longValue * TimeSpan.TicksPerMillisecond)
                    : new TimeSpan(longValue);
                return !timeOfDayOnly || value.Ticks is >= 0L and <= maxTimeOnlyTicks;
            }

            value = default;
            return false;
        }

        private static bool TryParseMSDateTime(string s, out DateTimeOffset value, out bool hasTimeZone)
        {
            if (s.StartsWith(msPrefix, StringComparison.Ordinal) && s.EndsWith(msPostfix, StringComparison.Ordinal))
            {
                // 13: prefix + postfix + sign + 4 digits
                hasTimeZone = s.Length > 13 && s[s.Length - 7] is '+' or '-';
                if (Int64.TryParse(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                    s.AsSpan(6, s.Length - (hasTimeZone ? 13 : 8)),
#else
                    s.Substring(6, s.Length - (hasTimeZone ? 13 : 8)),
#endif

                    NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out long longValue))
                {
                    value = new DateTimeOffset(FromUnixMilliseconds(longValue));
                    if (!hasTimeZone)
                        return true;

                    if (Int32.TryParse(
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
                        s.AsSpan(s.Length - 6, 4),
#else
                        s.Substring(s.Length - 6, 4),
#endif
                        NumberStyles.None, NumberFormatInfo.InvariantInfo, out int offsetValue))
                    {
                        TimeSpan offset = new TimeSpan(offsetValue / 100, offsetValue % 100, 0);
                        if (s[s.Length - 7] == '-')
                            offset = -offset;
                        value = value.ToOffset(offset);
                        return true;
                    }
                }
            }

            hasTimeZone = default;
            value = default;
            return false;
        }

        #endregion

        #endregion
    }
}
