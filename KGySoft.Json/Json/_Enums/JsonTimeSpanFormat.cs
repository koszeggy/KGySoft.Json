#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonTimeSpanFormat.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

using System;

namespace KGySoft.Json
{
    /// <summary>
    /// Specifies how <see cref="TimeSpan"/> instances are formatted and parsed by the <see cref="JsonValueExtensions"/> methods.
    /// </summary>
    public enum JsonTimeSpanFormat
    {
        /// <summary>
        /// <para>When converting to JSON, it is equivalent to <see cref="Text"/> in case of a JSON <see cref="JsonValueType.String"/>,
        /// or <see cref="Milliseconds"/> in case of a JSON <see cref="JsonValueType.Number"/>.</para>
        /// <para>When parsing a <see cref="JsonValue"/>, it represents any predefined format in the <see cref="JsonTimeSpanFormat"/> enumeration.</para>
        /// <note>Parsing <see cref="TimeSpan"/> values with the <see cref="Auto"/> option formatted as numeric values (<see cref="Milliseconds"/>
        /// and <see cref="Ticks"/>) can ambiguous. Though a "sanity check" is applied for parsing such values use a
        /// specific option whenever possible.</note>
        /// </summary>
        Auto,

        /// <summary>
        /// Represents a time span in milliseconds. This is conform with the difference of
        /// two <a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date" target="_blank">Date</a> instances in JavaScript.
        /// </summary>
        Milliseconds,

        /// <summary>
        /// Represents a time span in 100 nanoseconds. This is conform with the constructor of the .NET <see cref="TimeSpan"/>
        /// type and its <see cref="TimeSpan.Ticks"/> property.
        /// </summary>
        Ticks,

        /// <summary>
        /// Represents a time span as a textual value. This is conform with the result of the <see cref="TimeSpan.ToString()">TimeSpan.ToString</see> method.
        /// </summary>
        Text
    }
}