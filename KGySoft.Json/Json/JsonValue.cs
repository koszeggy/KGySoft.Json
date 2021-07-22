#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonValue.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

#endregion

namespace KGySoft.Json
{
    /// <summary>
    /// Represents a value that can be converted to JSON. It can hold JavaScript primitive types
    /// such as <see cref="JsonValueType.Null"/>, <see cref="JsonValueType.Boolean"/>, <see cref="JsonValueType.Number"/> and <see cref="JsonValueType.String"/>,
    /// and it is also assignable from <see cref="JsonArray"/> and <see cref="JsonObject"/> types.
    /// Its default value represents the JavaScript <see cref="JsonValueType.Undefined"/> value.
    /// Use the <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> or <see cref="O:KGySoft.Json.JsonValue.WriteTo">WriteTo</see> methods to convert it to JSON.
    /// <br/>See the <strong>Remarks</strong> section for details.
    /// </summary>
    /// <remarks>
    /// TODO
    /// - JSON: ToString/WriteTo
    /// - Implicit conversions
    /// - numbers
    ///   - ToString preserves any precision but that may be lost at JS side
    ///   - ToString supports NaN/infinity even though they are not supported in JSON
    ///   - AsNumber is compatible with JavaScript, use AsLiteral or extensions
    ///   - Long, implicit cast with warning, use ToJson instead
    /// - Objects
    ///   - indexer access, tolerates nonexistent values
    ///   - To set cast to JsonObject or use AsObject (needed because JsonValue is a struct)
    ///   - JsonProperty implicit conversion to ValueTuple
    /// - Arrays
    ///   - Just like in JS, nonexistent index is tolerated
    ///   - To set cast to JsonArray or use AsArray (needed because JsonValue is a struct)
    /// - Undefined:
    ///   - Unlike in JS, a standalone undefined is serialized as `undefined` by ToString
    ///   - But WriteTo writes nothing so Parse will say unexpected end
    ///   - It is replaced by null in arrays and skipped in objects
    /// - ToString vs WriteTo
    ///   - ToString always minimizes
    ///   - ToString writes undefined as a root
    ///   - WriteTo can indent and supports different encodings (eg. direct UTF8)
    /// </remarks>
    /// <seealso cref="JsonArray"/>
    /// <seealso cref="JsonObject"/>
    /// <seealso cref="JsonValueExtensions"/>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "False alarm, for some reason ReSharper triggers non_field_members_should_be_pascal_case for all As* members")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "ReSharper issue")]
    [Serializable]
    public readonly struct JsonValue : IEquatable<JsonValue>
    {
        #region Constants

        internal const string UndefinedLiteral = "undefined";
        internal const string NullLiteral = "null";
        internal const string TrueLiteral = "true";
        internal const string FalseLiteral = "false";

        #endregion

        #region Fields

        #region Static Fields

        /// <summary>
        /// Represents the JavaScript <c>undefined</c> value. The <see cref="Type"/> of the value is also <see cref="JsonValueType.Undefined"/>.
        /// This is the value of a default <see cref="JsonValue"/> instance.
        /// </summary>
        public static readonly JsonValue Undefined = default;

        /// <summary>
        /// Represents the JavaScript <c>null</c> value. The <see cref="Type"/> of the value is also <see cref="JsonValueType.Null"/>.
        /// </summary>
        public static readonly JsonValue Null = new JsonValue(JsonValueType.Null, NullLiteral);

        /// <summary>
        /// Represents the JavaScript <c>true</c> value. The <see cref="Type"/> of the value is <see cref="JsonValueType.Boolean"/>.
        /// </summary>
        public static readonly JsonValue True = new JsonValue(JsonValueType.Boolean, TrueLiteral);

        /// <summary>
        /// Represents the JavaScript <c>false</c> value. The <see cref="Type"/> of the value is <see cref="JsonValueType.Boolean"/>.
        /// </summary>
        public static readonly JsonValue False = new JsonValue(JsonValueType.Boolean, FalseLiteral);

        #endregion

        #region Instance Fields

        /// <summary>
        /// The stored value. Can be <see langword="null"/> (undefined),
        /// <see cref="string"/> (all primitive types except undefined),
        /// <see cref="JsonArray"/> and <see cref="JsonObject"/>.
        /// </summary>
        private readonly object? value;

        #endregion

        #endregion

        #region Properties and Indexers

        #region Properties
        
        #region Public Properties

        /// <summary>
        /// Gets the JavaScript type of this <see cref="JsonValue"/>.
        /// </summary>
        public JsonValueType Type { get; }

        /// <summary>
        /// Gets whether this <see cref="JsonValue"/> instance has <see cref="JsonValueType.Undefined"/>&#160;<see cref="Type"/> and equals to the <see cref="Undefined"/> instance.
        /// </summary>
        public bool IsUndefined => Type == JsonValueType.Undefined;

        /// <summary>
        /// Gets whether this <see cref="JsonValue"/> instance has <see cref="JsonValueType.Null"/>&#160;<see cref="Type"/> and equals to the <see cref="Null"/> instance.
        /// </summary>
        public bool IsNull => Type == JsonValueType.Null;

        /// <summary>
        /// Gets the <see cref="bool">bool</see> value of this <see cref="JsonValue"/> instance if it has <see cref="JsonValueType.Boolean"/>&#160;<see cref="Type"/>;
        /// or <see langword="null"/>, if its <see cref="Type"/> is not <see cref="JsonValueType.Boolean"/>.
        /// To interpret other types as boolean you can use the <see cref="JsonValueExtensions.AsBoolean"/> extension method instead.
        /// </summary>
        public bool? AsBoolean => Type == JsonValueType.Boolean ? TrueLiteral.Equals(value) : default(bool?);

        /// <summary>
        /// Gets the <see cref="string">string</see> value of this <see cref="JsonValue"/> instance if it has <see cref="JsonValueType.String"/>&#160;<see cref="Type"/>;
        /// or <see langword="null"/>, if its <see cref="Type"/> is not <see cref="JsonValueType.String"/>.
        /// <br/>See the <strong>Remarks</strong> section for details.
        /// </summary>
        /// <remarks>
        /// <para>This property returns <see langword="null"/> if this <see cref="JsonValue"/> represents a non-string primitive JavaScript literal.
        /// For non-string primitive types you can use the <see cref="AsLiteral"/> property to get their literal value.</para>
        /// <para>This property gets the string value without quotes and escapes. To return it as a parseable JSON string, use the <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> method overloads instead.</para>
        /// </remarks>
        public string? AsString => Type == JsonValueType.String ? (string)value! : null;

        /// <summary>
        /// Gets the numeric value of this <see cref="JsonValue"/> instance if it has <see cref="JsonValueType.Number"/>&#160;<see cref="Type"/>;
        /// or <see langword="null"/>, if its <see cref="Type"/> is not <see cref="JsonValueType.Number"/>.
        /// The returned value is a <see cref="double">double</see> to be confirm with JSON <see cref="JsonValueType.Number"/> type.
        /// To retrieve the actual stored raw value use the <see cref="AsLiteral"/> property.
        /// To retrieve the value as .NET numeric types use the methods in the <see cref="JsonValueExtensions"/> class.
        /// <br/>See the <strong>Remarks</strong> section for details.
        /// </summary>
        /// <remarks>
        /// <note type="warning">The JavaScript <see cref="JsonValueType.Number"/> type is
        /// always a <a href="https://en.wikipedia.org/wiki/Double-precision_floating-point_format" target="_blank">double-precision 64-bit binary format IEEE 754</a> value,
        /// which is the equivalent of the <see cref="double">double</see> type in C#. It is not recommended to store C# <see cref="long">long</see> and <see cref="decimal">decimal</see>
        /// types as JavaScript numbers because their precision might be lost silently if the JSON is processed by JavaScript.</note>
        /// <para>When getting this property the stored underlying string is converted to a <see cref="double">double</see>
        /// so it has the same behavior as a JavaScript <see cref="JsonValueType.Number"/>.</para>
        /// <para>If this <see cref="JsonValue"/> was created from a C# <see cref="long">long</see> or <see cref="decimal">decimal</see> value (see
        /// the <see cref="O:KGySoft.Json.JsonValueExtensions.ToJson">ToJson</see> overloads), then this property may return a different value due to loss of precision.
        /// This is how JavaScript also behaves. To get the value as .NET numeric types use the extension methods in the <see cref="JsonValueExtensions"/> class.</para>
        /// <para>To retrieve the stored actual raw value without any conversion you can use the <see cref="AsLiteral"/> property.</para>
        /// <para>This property may return <see langword="null"/> if this instance was created by the <see cref="CreateNumberUnchecked">CreateNumberUnchecked</see>
        /// method and contains an invalid number.</para>
        /// <para>This property can also return <see langword="null"/> when a <c>NaN</c> or <c>Infinity</c>/<c>-Infinity</c> was parsed, which are not valid in JSON.
        /// But even such values can be retrieved as a <see cref="double">double</see> by the <see cref="JsonValueExtensions.AsDouble">AsDouble</see> extension method.</para>
        /// </remarks>
        public double? AsNumber => Type == JsonValueType.Number && Double.TryParse((string)value!, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double result)
            ? result
            : default(double?);

        /// <summary>
        /// If this <see cref="JsonValue"/> represents a primitive type (<see cref="JsonValueType.Undefined"/>, <see cref="JsonValueType.Null"/>, <see cref="JsonValueType.Boolean"/>,
        /// <see cref="JsonValueType.Number"/>, <see cref="JsonValueType.String"/>) or it has a <see cref="JsonValueType.UnknownLiteral"/>&#160;<see cref="Type"/>, then gets the underlying literal;
        /// otherwise, gets <see langword="null"/>.
        /// </summary>
        public string? AsLiteral => IsUndefined ? UndefinedLiteral : value as string;

        /// <summary>
        /// Gets this <see cref="JsonValue"/> instance as a <see cref="JsonArray"/> if it has <see cref="JsonValueType.Array"/>&#160;<see cref="Type"/>;
        /// or <see langword="null"/>, if its <see cref="Type"/> is not <see cref="JsonValueType.Array"/>.
        /// <br/>See the <strong>Remarks</strong> section for details.
        /// </summary>
        /// <remarks>
        /// <para>To get array elements you can also read the numeric <see cref="this[int]">indexer</see> without obtaining the value as a <see cref="JsonArray"/>.</para>
        /// <para>To set/add/remove array elements in a <see cref="JsonValue"/> instance you need to use this property or the explicit cast to <see cref="JsonArray"/>.</para>
        /// </remarks>
        public JsonArray? AsArray => value as JsonArray;

        /// <summary>
        /// Gets this <see cref="JsonValue"/> instance as a <see cref="JsonObject"/> if it has <see cref="JsonValueType.Object"/>&#160;<see cref="Type"/>;
        /// or <see langword="null"/>, if its <see cref="Type"/> is not <see cref="JsonValueType.Object"/>.
        /// <br/>See the <strong>Remarks</strong> section for details.
        /// </summary>
        /// <remarks>
        /// <para>To get property values you can also read the string <see cref="this[string]">indexer</see> without obtaining the value as a <see cref="JsonObject"/>.</para>
        /// <para>To set/add/remove object properties in a <see cref="JsonValue"/> instance you need to use this property or the explicit cast to <see cref="JsonObject"/>.</para>
        /// </remarks>
        public JsonObject? AsObject => value as JsonObject;

        #endregion

        #region Internal Properties

        /// <summary>
        /// Similar to <see cref="AsLiteral"/> but returns <see langword="null"/> for <see cref="Undefined"/>.
        /// </summary>
        internal string? AsStringInternal => value as string;

        #endregion

        #endregion

        #region Indexers

        /// <summary>
        /// If the type of this <see cref="JsonValue"/> is <see cref="JsonValueType.Array"/> and <paramref name="arrayIndex"/> is within the valid bounds,
        /// then gets the value at the specified <paramref name="arrayIndex"/>; otherwise, returns <see cref="Undefined"/>.
        /// Just like in JavaScript, using an invalid index returns <see cref="Undefined"/>.
        /// </summary>
        /// <param name="arrayIndex">The index of the array element to get.</param>
        /// <returns>The value at the specified <paramref name="arrayIndex"/>, or <see cref="Undefined"/>
        /// if <paramref name="arrayIndex"/> is invalid or <see cref="Type"/> is not <see cref="JsonValueType.Array"/>.</returns>
        public JsonValue this[int arrayIndex] => value is JsonArray array
            ? array[arrayIndex]
            : Undefined;

        /// <summary>
        /// If the type of this <see cref="JsonValue"/> is <see cref="JsonValueType.Object"/> and <paramref name="propertyName"/> denotes an existing property,
        /// then gets the value of the specified <paramref name="propertyName"/>; otherwise, returns <see cref="Undefined"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to return its value.</param>
        /// <returns>The value of the specified <paramref name="propertyName"/>, or <see cref="Undefined"/>
        /// if <paramref name="propertyName"/> is invalid or <see cref="Type"/> is not <see cref="JsonValueType.Object"/>.</returns>
        public JsonValue this[string propertyName] => value is JsonObject obj
            ? obj[propertyName]
            : Undefined;

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Determines whether two specified <see cref="JsonValue"/> instances have the same value.
        /// </summary>
        /// <param name="left">The left argument of the equality check.</param>
        /// <param name="right">The right argument of the equality check.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator ==(JsonValue left, JsonValue right) => left.Equals(right);

        /// <summary>
        /// Determines whether two specified <see cref="JsonValue"/> instances have different values.
        /// </summary>
        /// <param name="left">The left argument of the inequality check.</param>
        /// <param name="right">The right argument of the inequality check.</param>
        /// <returns>The result of the inequality check.</returns>
        public static bool operator !=(JsonValue left, JsonValue right) => !left.Equals(right);

        /// <summary>
        /// Performs an implicit conversion from <see cref="bool">bool</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(bool value) => new JsonValue(value);

        /// <summary>
        /// Performs an implicit conversion from nullable <see cref="bool">bool</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(bool? value) => value == null ? Null : value.Value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="string">string</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(string? value) => new JsonValue(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="double">double</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(double value) => new JsonValue(value);

        /// <summary>
        /// Performs an implicit conversion from nullable <see cref="double">double</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(double? value) => value == null ? Null : value.Value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="JsonArray"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="array">The <see cref="JsonArray"/> to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(JsonArray? array) => new JsonValue(array);

        /// <summary>
        /// Performs an implicit conversion from <see cref="JsonObject"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="obj">The <see cref="JsonObject"/> to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(JsonObject? obj) => new JsonValue(obj);

        /// <summary>
        /// Performs an implicit conversion from <see cref="int">int</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(int value)
            // just for performance, the double conversion covers this functionality
            => new JsonValue(JsonValueType.Number, value.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Performs an implicit conversion from nullable <see cref="int">int</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(int? value)
            // just for performance, the double conversion covers this functionality
            => value == null ? Null : value.Value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="uint">uint</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        [CLSCompliant(false)]
        public static implicit operator JsonValue(uint value)
            // just for performance, the double conversion covers this functionality
            => new JsonValue(JsonValueType.Number, value.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Performs an implicit conversion from nullable <see cref="uint">uint</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        [CLSCompliant(false)]
        public static implicit operator JsonValue(uint? value)
            // just for performance, the double conversion covers this functionality
            => value == null ? Null : value.Value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="long">long</see> to <see cref="JsonValue"/>.
        /// This operator exists only to produce a warning because otherwise the implicit conversion from double would also match <see cref="long"/> values.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        [Obsolete("Warning: Using Int64 as a JSON Number may cause loss of precision. It is recommended to use the ToJson extension method instead. You can pass false to the asString parameter to express your intention and to avoid this warning.")]
        public static implicit operator JsonValue(long value)
            => new JsonValue(JsonValueType.Number, value.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Performs an implicit conversion from nullable <see cref="long">long</see> to <see cref="JsonValue"/>.
        /// This operator exists only to produce a warning because otherwise the implicit conversion from double would also match <see cref="long"/> values.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        [Obsolete("Warning: Using Int64 as a JSON Number may cause loss of precision. It is recommended to use the ToJson extension method instead. You can pass false to the asString parameter to express your intention and to avoid this warning.")]
        public static implicit operator JsonValue(long? value)
            => value == null ? Null : value.Value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="ulong">ulong</see> to <see cref="JsonValue"/>.
        /// This operator exists only to produce a warning because otherwise the implicit conversion from double would also match <see cref="ulong"/> values.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        [CLSCompliant(false)]
        [Obsolete("Warning: Using UInt64 as a JSON Number may cause loss of precision. It is recommended to use the ToJson extension method instead. You can pass false to the asString parameter to express your intention and to avoid this warning.")]
        public static implicit operator JsonValue(ulong value)
            => new JsonValue(JsonValueType.Number, value.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Performs an implicit conversion from nullable <see cref="ulong">ulong</see> to <see cref="JsonValue"/>.
        /// This operator exists only to produce a warning because otherwise the implicit conversion from double would also match <see cref="ulong"/> values.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        [CLSCompliant(false)]
        [Obsolete("Warning: Using UInt64 as a JSON Number may cause loss of precision. It is recommended to use the ToJson extension method instead. You can pass false to the asString parameter to express your intention and to avoid this warning.")]
        public static implicit operator JsonValue(ulong? value)
            => value == null ? Null : value.Value;

        /// <summary>
        /// Performs an explicit conversion from <see cref="JsonValue"/> to nullable <see cref="bool">bool</see>.
        /// The conversion succeeds if the <see cref="Type"/> property is <see cref="JsonValueType.Null"/> or <see cref="JsonValueType.Boolean"/>; otherwise, an <see cref="InvalidCastException"/> is thrown.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="bool">bool</see>.</param>
        /// <returns>
        /// A <see cref="bool">bool</see> instance that represents the original value.
        /// </returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> does not represent a boolean value.</exception>
        public static explicit operator bool?(JsonValue value) => value.IsNull ? null : value.AsBoolean ?? Throw.InvalidCastException<bool>(Res.JsonValueInvalidCast<bool>(value.Type));

        /// <summary>
        /// Performs an explicit conversion from <see cref="JsonValue"/> to <see cref="string">string</see>.
        /// The conversion succeeds if the <see cref="Type"/> property is <see cref="JsonValueType.Null"/> or <see cref="JsonValueType.String"/>; otherwise, an <see cref="InvalidCastException"/> is thrown.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="string">string</see>.</param>
        /// <returns>
        /// A <see cref="string">string</see> instance that represents the original value.
        /// </returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> does not represent a string value.</exception>
        public static explicit operator string?(JsonValue value) => value.IsNull ? null : value.AsString ?? Throw.InvalidCastException<string>(Res.JsonValueInvalidCast<string>(value.Type));

        /// <summary>
        /// Performs an explicit conversion from <see cref="JsonValue"/> to nullable <see cref="double">double</see>.
        /// The conversion succeeds if the <see cref="Type"/> property is <see cref="JsonValueType.Null"/> or <see cref="JsonValueType.Number"/>; otherwise, an <see cref="InvalidCastException"/> is thrown.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="double">double</see>.</param>
        /// <returns>
        /// A <see cref="double">double</see> instance that represents the original value.
        /// </returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> does not represent a numeric value.</exception>
        public static explicit operator double?(JsonValue value) => value.IsNull ? null : value.AsNumber ?? Throw.InvalidCastException<double>(Res.JsonValueInvalidCast<double>(value.Type));

        /// <summary>
        /// Performs an explicit conversion from <see cref="JsonValue"/> to <see cref="JsonArray"/>.
        /// The conversion succeeds if the <see cref="Type"/> property is <see cref="JsonValueType.Null"/> or <see cref="JsonValueType.Array"/>; otherwise, an <see cref="InvalidCastException"/> is thrown.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonArray"/>.</param>
        /// <returns>
        /// A <see cref="JsonArray"/> instance that represents the original value.
        /// </returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> does not represent an array.</exception>
        public static explicit operator JsonArray?(JsonValue value) => value.IsNull ? null : value.AsArray ?? Throw.InvalidCastException<JsonArray>(Res.JsonValueInvalidCast<JsonArray>(value.Type));

        /// <summary>
        /// Performs an explicit conversion from <see cref="JsonValue"/> to <see cref="JsonObject"/>.
        /// The conversion succeeds if the <see cref="Type"/> property is <see cref="JsonValueType.Null"/> or <see cref="JsonValueType.Object"/>; otherwise, an <see cref="InvalidCastException"/> is thrown.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonObject"/>.</param>
        /// <returns>
        /// A <see cref="JsonObject"/> instance that represents the original value.
        /// </returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> does not represent an object.</exception>
        public static explicit operator JsonObject?(JsonValue value) => value.IsNull ? null : value.AsObject ?? Throw.InvalidCastException<JsonObject>(Res.JsonValueInvalidCast<JsonArray>(value.Type));

        #endregion

        #region Constructors

        #region Public Constructors

        /// <summary>
        /// Initializes a new <see cref="JsonValue"/> struct that represents a boolean value.
        /// An implicit conversion from the <see cref="bool">bool</see> type also exists.
        /// </summary>
        /// <param name="value">The value to initialize the <see cref="JsonValue"/> from.</param>
        public JsonValue(bool value) => this = value ? True : False;

        /// <summary>
        /// Initializes a new <see cref="JsonValue"/> struct that represents a string.
        /// An implicit conversion from the <see cref="string">string</see> type also exists.
        /// </summary>
        /// <param name="value">The value to initialize the <see cref="JsonValue"/> from.</param>
        public JsonValue(string? value)
        {
            if (value == null)
            {
                this = Null;
                return;
            }

            Type = JsonValueType.String;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new <see cref="JsonValue"/> struct that represents a number.
        /// An implicit conversion from the <see cref="double">double</see> type also exists.
        /// Some .NET numeric types such as <see cref="long">long</see> and <see cref="decimal">decimal</see> are not recommended to be encoded as JSON numbers.
        /// <br/>See the <strong>Remarks</strong> section for details.
        /// </summary>
        /// <param name="value">The value to initialize the <see cref="JsonValue"/> from.</param>
        /// <remarks>
        /// <note type="warning">The JavaScript <see cref="JsonValueType.Number"/> type is
        /// always a <a href="https://en.wikipedia.org/wiki/Double-precision_floating-point_format" target="_blank">double-precision 64-bit binary format IEEE 754</a> value,
        /// which is the equivalent of the <see cref="double">double</see> type in C#. It is not recommended to store C# <see cref="long">long</see> and <see cref="decimal">decimal</see>
        /// types as JavaScript numbers because their precision might be lost silently if the JSON is processed by JavaScript.</note>
        /// <note><list type="bullet">
        /// <item>JavaScript Number type is actually a double. Other large numeric types (<see cref="long">[u]long</see>/<see cref="decimal">decimal</see>) must be encoded as string to
        /// prevent loss of precision at a real JavaScript side. If you are sure that you want to forcibly treat such types as numbers use
        /// the <see cref="O:KGySoft.Json.JsonValueExtensions.ToJson">ToJson</see> overloads and pass <see langword="true"/> to their <c>asString</c> parameter.
        /// You can use also the <see cref="CreateNumberUnchecked">CreateNumberUnchecked</see> method to create a JSON number directly from a string.</item>
        /// <item>This method allows <see cref="Double.NaN"/> and <see cref="Double.PositiveInfinity"/>/<see cref="Double.NegativeInfinity"/>,
        /// which are also invalid in JSON. Parsing these values works though their <see cref="Type"/> will be <see cref="JsonValueType.UnknownLiteral"/> after parsing.</item>
        /// </list></note>
        /// </remarks>
        /// <seealso cref="AsNumber"/>
        /// <seealso cref="CreateNumberUnchecked"/>
        public JsonValue(double value)
        {
            Type = JsonValueType.Number;
            this.value = value.ToString("R", NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Initializes a new <see cref="JsonValue"/> struct that represents an array.
        /// </summary>
        /// <param name="array">The <see cref="JsonArray"/> to initialize the <see cref="JsonValue"/> from.</param>
        public JsonValue(JsonArray? array)
        {
            if (array == null)
            {
                this = Null;
                return;
            }

            Type = JsonValueType.Array;
            value = array;
        }

        /// <summary>
        /// Initializes a new <see cref="JsonValue"/> struct that represents an array.
        /// </summary>
        /// <param name="obj">The <see cref="JsonObject"/> to initialize the <see cref="JsonValue"/> from.</param>
        public JsonValue(JsonObject? obj)
        {
            if (obj == null)
            {
                this = Null;
                return;
            }

            Type = JsonValueType.Object;
            value = obj;
        }

        #endregion

        #region Internal Constructors

        internal JsonValue(JsonValueType type, string value)
        {
            this.value = value;
            Type = type;
        }

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        #region Public Methods

        /// <summary>
        /// Reads a <see cref="JsonValue"/> from a <see cref="TextReader"/> that contains JSON data.
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> that will be read for the <see cref="JsonValue"/> content.</param>
        /// <returns>A <see cref="JsonValue"/> that contains the JSON data that was read from the specified <see cref="TextReader"/>.</returns>
        public static JsonValue Parse(TextReader reader)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, reader CAN be null but MUST NOT be
            => JsonParser.ParseValue(reader ?? Throw.ArgumentNullException<TextReader>(nameof(reader)));

        /// <summary>
        /// Reads a <see cref="JsonValue"/> from a <see cref="Stream"/> that contains JSON data.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> that will be read for the <see cref="JsonValue"/> content.</param>
        /// <param name="encoding">An <see cref="Encoding"/> that specifies the encoding of the JSON data in the <paramref name="stream"/>.
        /// If <see langword="null"/>, then <see cref="Encoding.UTF8"/> encoding will be used. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <returns>A <see cref="JsonValue"/> that contains the JSON data that was read from the specified <paramref name="stream"/>.</returns>
        public static JsonValue Parse(Stream stream, Encoding? encoding = null)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, stream CAN be null but MUST NOT be
            => Parse(new StreamReader(stream ?? Throw.ArgumentNullException<Stream>(nameof(stream)), encoding ?? Encoding.UTF8));

        /// <summary>
        /// Reads a <see cref="JsonValue"/> from a <see cref="string">string</see> that contains JSON data.
        /// </summary>
        /// <param name="s">A string that will be read for the <see cref="JsonValue"/> content.</param>
        /// <returns>A <see cref="JsonValue"/> that contains the JSON data that was read from the specified string.</returns>
        public static JsonValue Parse(string s)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, s CAN be null but MUST NOT be
            => Parse(new StringReader(s ?? Throw.ArgumentNullException<string>(nameof(s))));

        /// <summary>
        /// Tries to read a <see cref="JsonValue"/> from a <see cref="TextReader"/> that contains JSON data.
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> that will be read for the <see cref="JsonValue"/> content.</param>
        /// <param name="value">When this method returns <see langword="true"/>, the result of the parsing;
        /// otherwise, <see cref="Undefined"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(TextReader reader, out JsonValue value)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, reader CAN be null but MUST NOT be
            => JsonParser.TryParseValue(reader ?? Throw.ArgumentNullException<TextReader>(nameof(reader)), out value);

        /// <summary>
        /// Tries to read a <see cref="JsonValue"/> from a <see cref="Stream"/> that contains JSON data.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> that will be read for the <see cref="JsonValue"/> content.</param>
        /// <param name="value">When this method returns <see langword="true"/>, the result of the parsing;
        /// otherwise, <see cref="Undefined"/>. This parameter is passed uninitialized.</param>
        /// <param name="encoding">An <see cref="Encoding"/> that specifies the encoding of the JSON data in the <paramref name="stream"/>.
        /// If <see langword="null"/>, then <see cref="Encoding.UTF8"/> encoding will be used. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(Stream stream, out JsonValue value, Encoding? encoding = null)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, stream CAN be null but MUST NOT be
            => TryParse(new StreamReader(stream ?? Throw.ArgumentNullException<Stream>(nameof(stream)), encoding ?? Encoding.UTF8), out value);

        /// <summary>
        /// Tries to read a <see cref="JsonValue"/> from a <see cref="string">string</see> that contains JSON data.
        /// </summary>
        /// <param name="s">A string that will be read for the <see cref="JsonValue"/> content.</param>
        /// <param name="value">When this method returns <see langword="true"/>, the result of the parsing;
        /// otherwise, <see cref="Undefined"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(string s, out JsonValue value)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, s CAN be null but MUST NOT be
            => TryParse(new StringReader(s ?? Throw.ArgumentNullException<string>(nameof(s))), out value);


        /// <summary>
        /// Creates a <see cref="JsonValue"/> that forcibly treats <paramref name="value"/> as a JSON number,
        /// even if it cannot be represented as a valid number in JavaScript.
        /// <br/>See the <strong>Remarks</strong> section for details.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><see cref="Null"/>, if <paramref name="value"/> was <see langword="null"/>; otherwise, a <see cref="JsonValue"/>
        /// that contains the specified value as a <see cref="JsonValueType.Number"/>.</returns>
        /// <remarks>
        /// <note type="warning">This method makes possible to create invalid JSON.</note>
        /// <para>The <see cref="Type"/> property of the result will return <see cref="JsonValueType.Number"/> even if <paramref name="value"/> is not a valid number.</para>
        /// <para>The <see cref="AsLiteral"/> property will return the specified <paramref name="value"/>.</para>
        /// <para>The <see cref="AsNumber"/> property of the result may return a less precise value, or even <see langword="null"/>,
        /// though serializing to JSON by the <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> and <see cref="O:KGySoft.Json.JsonValue.WriteTo">WriteTo</see>
        /// methods preserves the specified <paramref name="value"/>.</para>
        /// </remarks>
        public static JsonValue CreateNumberUnchecked(string? value) => value == null ? Null : new JsonValue(JsonValueType.Number, value);

        /// <summary>
        /// Creates a <see cref="JsonValue"/> that forcibly treats <paramref name="value"/> as a JSON literal, even if it is invalid in JSON.
        /// <br/>See the <strong>Remarks</strong> section for details.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><see cref="Null"/>, if <paramref name="value"/> was <see langword="null"/>; otherwise, a <see cref="JsonValue"/>,
        /// whose <see cref="Type"/> property returns <see cref="JsonValueType.UnknownLiteral"/>.</returns>
        /// <remarks>
        /// <note type="warning">This method makes possible to create invalid JSON.</note>
        /// <para>The <see cref="Type"/> property of the result will return <see cref="JsonValueType.UnknownLiteral"/> even if <paramref name="value"/> is actually a valid JSON literal.</para>
        /// <para>The <see cref="AsLiteral"/> property will return the specified <paramref name="value"/>.</para>
        /// <para>Serializing to JSON by the <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> and <see cref="O:KGySoft.Json.JsonValue.WriteTo">WriteTo</see>
        /// methods preserves the specified <paramref name="value"/>.</para>
        /// </remarks>
        public static JsonValue CreateLiteralUnchecked(string? value) => value == null ? Null : new JsonValue(JsonValueType.UnknownLiteral, value);

        #endregion

        #region Internal Methods

        internal static void WriteJsonString(StringBuilder builder, string value)
        {
            builder.Append('"');
            int len = value.Length;
            for (var i = 0; i < len; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case > '\\': // 92
                        builder.Append(c);
                        break;

                    case > '"': // 34
                        if (c == '\\')
                            builder.Append(@"\\");
                        else
                            builder.Append(c);
                        break;

                    case >= ' ': // 32
                        if (c == '"')
                            builder.Append(@"\""");
                        else
                            builder.Append(c);
                        break;

                    case '\r':
                        builder.Append(@"\r");
                        break;
                    case '\n':
                        builder.Append(@"\n");
                        break;
                    case '\t':
                        builder.Append(@"\t");
                        break;
                    case '\f':
                        builder.Append(@"\f");
                        break;
                    case '\b':
                        builder.Append(@"\b");
                        break;
                    default:
                        builder.Append("\\u");
                        builder.Append(((int)c).ToString("X4", NumberFormatInfo.InvariantInfo));
                        break;
                }
            }

            builder.Append('"');
        }

        #endregion

        #endregion

        #region Instance Methods

        #region Public Methods

        /// <summary>
        /// Returns a minimized JSON string that represents this <see cref="JsonValue"/>.
        /// </summary>
        /// <returns>A minimized JSON string that represents this <see cref="JsonValue"/>.</returns>
        public override string ToString()
        {
            if (Type <= JsonValueType.Number)
                return AsLiteral!;

            var result = new StringBuilder(value is string s ? s.Length + 2 : 64);
            Dump(result);
            return result.ToString();
        }

        /// <summary>
        /// Returns a JSON string that represents this <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="indent">Specifies the indentation string to produce a formatted JSON.
        /// If <see langword="null"/> or empty, then a minimized JSON is returned. Using non-whitespace characters may produce an invalid JSON.</param>
        /// <returns>A JSON string that represents this <see cref="JsonValue"/>.</returns>
        public string ToString(string? indent)
        {
            if (Type <= JsonValueType.Number || String.IsNullOrEmpty(indent))
                return ToString();

            var result = new StringBuilder(value is string s ? s.Length + 2 : 64);
            WriteTo(result, indent);
            return result.ToString();
        }

        /// <summary>
        /// Indicates whether the current <see cref="JsonValue"/> instance is equal to another one specified in the <paramref name="other"/> parameter.
        /// </summary>
        /// <param name="other">A <see cref="JsonValue"/> instance to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public bool Equals(JsonValue other) => Type == other.Type && Equals(value, other.value);

        /// <summary>
        /// Determines whether the specified <see cref="object">object</see> is equal to this instance.
        /// </summary>
        /// <param name="obj">An <see cref="object"/> to compare with this instance.</param>
        /// <returns><see langword="true"/> if the specified object is equal to this instance; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object? obj) => obj is JsonValue other && Equals(other);

        /// <summary>
        /// Returns a hash code for this <see cref="JsonValue"/> instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() => (Type, value).GetHashCode();

        /// <summary>
        /// Writes this <see cref="JsonValue"/> instance into a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> to write the <see cref="JsonValue"/> into.</param>
        /// <param name="indent">Specifies the indentation string to produce a formatted JSON.
        /// If <see langword="null"/> or empty, then a minimized JSON is returned. Using non-whitespace characters may produce an invalid JSON. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        public void WriteTo(TextWriter writer, string? indent = null)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, writer CAN be null but MUST NOT be
            => new JsonWriter(writer ?? Throw.ArgumentNullException<TextWriter>(nameof(writer)), indent).Write(this);

        /// <summary>
        /// Writes this <see cref="JsonValue"/> instance into a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="builder">A <see cref="StringBuilder"/> to write the <see cref="JsonValue"/> into.</param>
        /// <param name="indent">Specifies the indentation string to produce a formatted JSON.
        /// If <see langword="null"/> or empty, then a minimized JSON is returned. Using non-whitespace characters may produce an invalid JSON. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        public void WriteTo(StringBuilder builder, string? indent = null)
        {
            if (builder == null!)
                Throw.ArgumentNullException(nameof(builder));

            // shortcut: we don't need to use a writer
            if (String.IsNullOrEmpty(indent))
            {
                Dump(builder);
                return;
            }

            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, builder CAN be null but MUST NOT be
            new JsonWriter(new StringWriter(builder ?? Throw.ArgumentNullException<StringBuilder>(nameof(builder))), indent).Write(this);
        }

        /// <summary>
        /// Writes this <see cref="JsonValue"/> instance into a <see cref="Stream"/> using the specified <paramref name="encoding"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write the <see cref="JsonValue"/> into.</param>
        /// <param name="encoding">An <see cref="Encoding"/> that specifies the encoding of the JSON data in the <paramref name="stream"/>.
        /// If <see langword="null"/>, then <see cref="Encoding.UTF8"/> encoding will be used. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <param name="indent">Specifies the indentation string to produce a formatted JSON.
        /// If <see langword="null"/> or empty, then a minimized JSON is returned. Using non-whitespace characters may produce an invalid JSON. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        public void WriteTo(Stream stream, Encoding? encoding = null, string? indent = null)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, stream CAN be null but MUST NOT be
            => new JsonWriter(new StreamWriter(stream ?? Throw.ArgumentNullException<Stream>(nameof(stream)), encoding ?? Encoding.UTF8), indent).Write(this);

        #endregion

        #region Internal Methods

        internal void Dump(StringBuilder builder)
        {
            switch (Type)
            {
                case JsonValueType.String:
                    WriteJsonString(builder, AsLiteral!);
                    return;

                case JsonValueType.Object:
                    AsObject!.Dump(builder);
                    return;

                case JsonValueType.Array:
                    AsArray!.Dump(builder);
                    return;

                default:
                    // In Dump undefined may occur only as a root from WriteTo(StringBuilder,null), in which case it is skipped
                    if (!IsUndefined)
                        builder.Append(AsLiteral);
                    return;
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
