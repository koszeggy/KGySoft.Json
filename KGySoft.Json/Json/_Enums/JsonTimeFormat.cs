#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonTimeFormat.cs
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

#endregion

#region Suppressions

#if !NET6_0_OR_GREATER
#pragma warning disable CS1574 // Some types in the XML documentation are not available in every targeted platform
#endif

#endregion

namespace KGySoft.Json
{
    /// <summary>
    /// Specifies how <see cref="TimeSpan"/> and <see cref="TimeOnly"/> instances are formatted and parsed by the <see cref="JsonValueExtensions"/> methods.
    /// </summary>
    public enum JsonTimeFormat
    {
        /// <summary>
        /// <para>When converting to JSON, it is equivalent to the <see cref="Text"/> format if the target <see cref="JsonValue.Type"/> is <see cref="JsonValueType.String"/>,
        /// or the <see cref="Milliseconds"/> format if the target <see cref="JsonValue.Type"/> is <see cref="JsonValueType.Number"/>.</para>
        /// <para>When parsing a <see cref="JsonValue"/>, it represents any defined <see cref="JsonTimeFormat"/> format.</para>
        /// <para><strong>Note:</strong> Parsing <see cref="TimeSpan"/>/<see cref="TimeOnly"/> values with the <see cref="Auto"/> option formatted as numeric values (<see cref="Milliseconds"/>
        /// and <see cref="Ticks"/>) can be ambiguous. Though a "sanity check" is applied for parsing such values use a
        /// specific option whenever possible.</para>
        /// </summary>
        Auto,

        /// <summary>
        /// Represents a time in milliseconds. This is conform with the difference of
        /// two <a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date" target="_blank">Date</a> instances in JavaScript.
        /// </summary>
        Milliseconds,

        /// <summary>
        /// Represents a time in 100 nanoseconds. This is conform with the constructor of the .NET <see cref="TimeSpan"/>/<see cref="TimeOnly"/>
        /// types and their <c>Ticks</c> property.
        /// </summary>
        Ticks,

        /// <summary>
        /// Represents a time as a textual value. This is conform with the regular string representation of the <see cref="TimeSpan"/>/<see cref="TimeOnly"/> types.
        /// </summary>
        Text
    }
}