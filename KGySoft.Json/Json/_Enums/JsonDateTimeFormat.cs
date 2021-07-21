#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonDateTimeFormat.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
/////////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;

#endregion

namespace KGySoft.Json
{
    /// <summary>
    /// Specifies how <see cref="DateTime"/> and <see cref="DateTimeOffset"/> instances are formatted and parsed by the <see cref="JsonValueExtensions"/> methods.
    /// </summary>
    public enum JsonDateTimeFormat
    {
        /// <summary>
        /// <para>When converting to JSON, it is equivalent to <see cref="Iso8601JavaScript"/> in case of a JSON <see cref="JsonValueType.String"/>,
        /// or <see cref="UnixMilliseconds"/> in case of a JSON <see cref="JsonValueType.Number"/>.</para>
        /// <para>When parsing a <see cref="JsonValue"/>, it represents any format, including some ISO 8601 formats,
        /// which are not covered by the other values in the <see cref="JsonDateTimeFormat"/> enumeration.</para>
        /// <note>Parsing date-time values with the <see cref="Auto"/> option formatted as numeric values (<see cref="UnixMilliseconds"/>, <see cref="UnixSeconds"/>,
        /// <see cref="UnixSecondsFloat"/> or <see cref="Ticks"/>) can be ambiguous. Though a "sanity check" is applied for parsing such values use a
        /// specific option whenever possible.</note>
        /// </summary>
        Auto,

        /// <summary>
        /// <para>Represents the time elapsed since the Unix Epoch time (1970-01-01T00:00Z) in milliseconds.
        /// This is conform with the constructor of the <a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date/Date" target="_blank">Date</a>
        /// object and also with its <a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date/getTime" target="_blank">getTime</a> method in JavaScript.</para>
        /// <para>When converting to JSON, local times will be adjusted to UTC. When parsing, the value is interpreted as UTC time.</para>
        /// <note>Example: 2020-01-01T00:00+01:00 becomes <c>1577833200000</c></note>
        /// </summary>
        UnixMilliseconds,

        /// <summary>
        /// <para>Represents the time elapsed since the Unix Epoch time (1970-01-01T00:00Z) in seconds.</para>
        /// <para>When converting to JSON, local times will be adjusted to UTC. When parsing, the value is interpreted as UTC time.</para>
        /// <note>Example: 2020-01-01T00:00+01:00 becomes <c>1577833200</c></note>
        /// </summary>
        UnixSeconds,

        /// <summary>
        /// <para>Represents the time elapsed since the Unix Epoch time (1970-01-01T00:00Z) in seconds as a floating point number.</para>
        /// <para>When converting to JSON, local times will be adjusted to UTC. When parsing, the value is interpreted as UTC time.</para>
        /// <note>Example: 2020-01-01T00:00+01:00 becomes <c>1577833200.000</c></note>
        /// </summary>
        UnixSecondsFloat,

        /// <summary>
        /// <para>Represents the time elapsed since 0001-01-01T00:00Z in 100 nanoseconds, in UTC.
        /// This is conform with the constructor of the .NET <see cref="DateTime"/> type and its <see cref="DateTime.Ticks"/> property.</para>
        /// <para>When converting to JSON, local times will be adjusted to UTC. When parsing, the value is interpreted as UTC time.</para>
        /// <note>Example: 2020-01-01T00:00+01:00 becomes <c>637134300000000000</c></note>
        /// </summary>
        Ticks,

        // Note: below there are string-only formats

        /// <summary>
        /// <para>Represents an ISO 8601 conform date/time format in UTC, as it is returned by the
        /// <a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date/toJSON" target="_blank">toJSON</a>
        /// method of the JavaScript <a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date" target="_blank">Date</a>
        /// object prototype by most of the recent JavaScript implementations.</para>
        /// <para>When converting to JSON, local times will be adjusted to UTC. When parsing, the value is interpreted as UTC time.</para>
        /// <note>Example: 2020-01-01T00:00+01:00 becomes <c>2019-12-31T23:00:00.000Z</c></note>
        /// </summary>
        Iso8601JavaScript,

        /// <summary>
        /// <para>Represents an ISO 8601 conform date/time, which can encode .NET <see cref="DateTime"/> and <see cref="DateTimeOffset"/> instances without losing precision.
        /// It can be parsed also by JavaScript, although the time value beyond milliseconds precision might be lost.</para>
        /// <para>When converting a <see cref="DateTime"/> instance to JSON, the <see cref="DateTime.Kind">DateTime.Kind</see>
        /// is also reflected in the result, which is restored on parsing.</para>
        /// <para>A <see cref="DateTimeOffset"/> is always treated as a local time of the time zone specified by its offset.
        /// You can obtain its <see cref="DateTimeOffset.UtcDateTime"/> property to encode it as a UTC time.</para>
        /// <note>Examples:
        /// <list type="bullet">
        /// <item>If <see cref="DateTime.Kind">DateTime.Kind</see> is <see cref="DateTimeKind.Unspecified"/>: <c>2020-01-01T00:00:00.0000000</c> (JavaScript interprets it as a local time)</item>
        /// <item>If <see cref="DateTime.Kind">DateTime.Kind</see> is <see cref="DateTimeKind.Utc"/>:<c>2020-01-01T00:00:00.0000000Z</c></item>
        /// <item>If <see cref="DateTime.Kind">DateTime.Kind</see> is <see cref="DateTimeKind.Local"/> and <see cref="DateTimeOffset"/> instances: <c>2020-01-01T00:00:00.0000000+01:00</c></item>
        /// </list></note>
        /// </summary>
        Iso8601Roundtrip,

        /// <summary>
        /// <para>Represents an ISO 8601 conform date/time format in UTC, which can encode .NET <see cref="DateTime"/> and <see cref="DateTimeOffset"/> instances without losing precision.
        /// It can be parsed also by JavaScript, although the time value beyond milliseconds precision might be lost.</para>
        /// <para>When converting to JSON, local times will be adjusted to UTC. When parsing, the value is interpreted as UTC time.</para>
        /// <note>Example: 2020-01-01T00:00+01:00 becomes <c>2019-12-31T23:00:00.0000000Z</c></note>
        /// </summary>
        Iso8601Utc,

        /// <summary>
        /// <para>Represents an ISO 8601 conform date/time format with time zone, which can encode .NET <see cref="DateTime"/> and <see cref="DateTimeOffset"/> instances without losing precision.
        /// It can be parsed also by JavaScript, although the time value beyond milliseconds precision might be lost.</para>
        /// <para>When converting to JSON, <see cref="DateTime"/> instances with <see cref="DateTimeKind.Utc"/>&#160;<see cref="DateTime.Kind"/>
        /// will be adjusted to local time. When parsing as <see cref="DateTime"/>, the value will be adjusted to the actual local time.</para>
        /// <note>Example: 2020-01-01T00:00+01:00 becomes <c>2020-01-01T00:00:00.0000000+01:00</c></note>
        /// </summary>
        Iso8601Local,

        /// <summary>
        /// <para>Represents an ISO 8601 conform date, without time and time zone information.</para>
        /// <para>When converting a <see cref="DateTime"/> instance to JSON, the <see cref="DateTime.Kind">Kind</see> property is ignored and the time value is not adjusted.
        /// When parsing as a <see cref="DateTime"/> the <see cref="DateTime.Kind">Kind</see> will be <see cref="DateTimeKind.Unspecified"/> by default.</para>
        /// <para>A <see cref="DateTimeOffset"/> is always treated as a local time of the time zone specified by its offset.
        /// You can obtain its <see cref="DateTimeOffset.UtcDateTime"/> property to encode it as a UTC date.</para>
        /// <note>Example: 2020-01-01T00:00+01:00 becomes <c>2020-01-01</c></note>
        /// </summary>
        Iso8601Date,

        /// <summary>
        /// <para>Represents an ISO 8601 conform date/time with hours and minutes.</para>
        /// <para>When converting a <see cref="DateTime"/> instance to JSON, the <see cref="DateTime.Kind">DateTime.Kind</see>
        /// is also reflected in the result, which is restored on parsing.</para>
        /// <para>A <see cref="DateTimeOffset"/> is always treated as a local time of the time zone specified by its offset.
        /// You can obtain its <see cref="DateTimeOffset.UtcDateTime"/> property to encode it as a UTC time.</para>
        /// <note>Examples:
        /// <list type="bullet">
        /// <item>If <see cref="DateTime.Kind">DateTime.Kind</see> is <see cref="DateTimeKind.Unspecified"/>: <c>2020-01-01T00:00</c> (JavaScript interprets it as a local time)</item>
        /// <item>If <see cref="DateTime.Kind">DateTime.Kind</see> is <see cref="DateTimeKind.Utc"/>:<c>2020-01-01T00:00Z</c></item>
        /// <item>If <see cref="DateTime.Kind">DateTime.Kind</see> is <see cref="DateTimeKind.Local"/> and <see cref="DateTimeOffset"/> instances: <c>2020-01-01T00:00+01:00</c></item>
        /// </list></note>
        /// </summary>
        Iso8601Minutes,

        /// <summary>
        /// <para>Represents an ISO 8601 conform date/time with hours, minutes and seconds.</para>
        /// <para>When converting a <see cref="DateTime"/> instance to JSON, the <see cref="DateTime.Kind">DateTime.Kind</see>
        /// is also reflected in the result, which is restored on parsing.</para>
        /// <para>A <see cref="DateTimeOffset"/> is always treated as a local time of the time zone specified by its offset.
        /// You can obtain its <see cref="DateTimeOffset.UtcDateTime"/> property to encode it as a UTC time.</para>
        /// <note>Examples:
        /// <list type="bullet">
        /// <item>If <see cref="DateTime.Kind">DateTime.Kind</see> is <see cref="DateTimeKind.Unspecified"/>: <c>2020-01-01T00:00:00</c> (JavaScript interprets it as a local time)</item>
        /// <item>If <see cref="DateTime.Kind">DateTime.Kind</see> is <see cref="DateTimeKind.Utc"/>:<c>2020-01-01T00:00:00Z</c></item>
        /// <item>If <see cref="DateTime.Kind">DateTime.Kind</see> is <see cref="DateTimeKind.Local"/> and <see cref="DateTimeOffset"/> instances: <c>2020-01-01T00:00:00+01:00</c></item>
        /// </list></note>
        /// </summary>
        Iso8601Seconds,

        /// <summary>
        /// <para>Represents an ISO 8601 conform date/time with milliseconds precision. Similar to the <see cref="Iso8601JavaScript"/> format,
        /// except that this one does not convert local times to UTC.</para>
        /// <para>When converting a <see cref="DateTime"/> instance to JSON, the <see cref="DateTime.Kind">DateTime.Kind</see>
        /// is also reflected in the result, which is restored on parsing.</para>
        /// <para>A <see cref="DateTimeOffset"/> is always treated as a local time of the time zone specified by its offset.
        /// You can obtain its <see cref="DateTimeOffset.UtcDateTime"/> property to encode it as a UTC time.</para>
        /// <note>Examples:
        /// <list type="bullet">
        /// <item>If <see cref="DateTime.Kind">DateTime.Kind</see> is <see cref="DateTimeKind.Unspecified"/>: <c>2020-01-01T00:00:00.000</c> (JavaScript interprets it as a local time)</item>
        /// <item>If <see cref="DateTime.Kind">DateTime.Kind</see> is <see cref="DateTimeKind.Utc"/>:<c>2020-01-01T00:00:00.000Z</c></item>
        /// <item>If <see cref="DateTime.Kind">DateTime.Kind</see> is <see cref="DateTimeKind.Local"/> and <see cref="DateTimeOffset"/> instances: <c>2020-01-01T00:00:00.000+01:00</c></item>
        /// </list></note>
        /// </summary>
        Iso8601Milliseconds,

        /// <summary>
        /// <para>Represents Microsoft's legacy AJAX and WCF REST date-time format. Similarly to <see cref="UnixMilliseconds"/>, it is also
        /// based elapsed milliseconds since Unix Epic time but has a specific string format and can also encode time offset.</para>
        /// <note>Examples:
        /// <list type="bullet">
        /// <item>If <see cref="DateTime.Kind">DateTime.Kind</see> is <see cref="DateTimeKind.Utc"/> or <see cref="DateTimeKind.Unspecified"/>: <c>/Date(1577833200000)/</c></item>
        /// <item>If <see cref="DateTime.Kind">DateTime.Kind</see> is <see cref="DateTimeKind.Local"/> and <see cref="DateTimeOffset"/> instances: <c>/Date(1577833200000+0100)/</c></item>
        /// </list></note>
        /// </summary>
        MicrosoftLegacy
    }
}
