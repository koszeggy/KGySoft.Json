﻿#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using KGySoft.CoreLibraries;

#endregion

namespace TradeSystem.Json
{
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

        public static readonly JsonValue Undefined = default;
        public static readonly JsonValue Null = new JsonValue(JsonValueType.Null, NullLiteral);
        public static readonly JsonValue True = new JsonValue(JsonValueType.Boolean, TrueLiteral);
        public static readonly JsonValue False = new JsonValue(JsonValueType.Boolean, FalseLiteral);

        #endregion

        #region Instance Fields

        private readonly object _value;

        #endregion

        #endregion

        #region Properties and Indexers

        #region Properties

        public JsonValueType Type { get; }

        public bool IsUndefined => Type == JsonValueType.Undefined;
        public bool IsNull => Type == JsonValueType.Null;

        public bool? AsBoolean => this == True ? true
            : this == False ? false
            : (bool?)null;

        public string AsString => Type == JsonValueType.String ? (string)_value : null;

        /// <summary>
        /// Gets the value as a number if <see cref="Type"/> is <see cref="JsonValueType.Number"/>.
        /// A valid JSON Number type is essentially a double. C# Long/Decimal type must be strings to ensure precision
        /// but if they are number literals their string value still can be accessed by the <see cref="AsLiteral"/> property.
        /// </summary>
        public double? AsNumber => Type == JsonValueType.Number && Double.TryParse((string)_value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double value)
                ? value
                : (double?)null;

        public string AsLiteral => _value as string; // for undefined, returns null! (unlike ToString, which returns undefined)
        public JsonArray AsArray => _value as JsonArray;
        public JsonObject AsObject => _value as JsonObject;

        #endregion

        #region Indexers

        public JsonValue this[int arrayIndex] => _value is JsonArray array
            ? array[arrayIndex]
            : Undefined;

        public JsonValue this[string propertyName] => _value is JsonObject obj
            ? obj[propertyName]
            : Undefined;

        #endregion

        #endregion

        #region Operators

        public static bool operator ==(JsonValue left, JsonValue right) => left.Equals(right);
        public static bool operator !=(JsonValue left, JsonValue right) => !left.Equals(right);

        public static implicit operator JsonValue(bool value) => new JsonValue(value);
        public static implicit operator JsonValue(string value) => new JsonValue(value);
        public static implicit operator JsonValue(double value) => new JsonValue(value);
        public static implicit operator JsonValue(JsonArray array) => new JsonValue(array);
        public static implicit operator JsonValue(JsonObject obj) => new JsonValue(obj);

        #endregion

        #region Constructors

        #region Public Constructors

        public JsonValue(bool value) => this = value ? True : False;

        public JsonValue(string value)
        {
            if (value == null)
            {
                this = Null;
                return;
            }

            Type = JsonValueType.String;
            _value = value;
        }

        /// <summary>
        /// Initializes a new <see cref="JsonValue"/> struct that represents a number.
        /// <note><list type="bullet">
        /// <item>JavaScript's Number type is actually a double. Other large numeric types (<see cref="long"/>/<see cref="decimal"/>) must be encoded as a string to
        /// prevent loss of precision at a real JS side. If you are sure that you want to forcibly treat such types as numbers in the resulting JSON
        /// use the <see cref="FromNumberUnchecked"/> method.</item>
        /// <item>This method allows <see cref="Double.NaN"/> and <see cref="Double.PositiveInfinity"/>/<see cref="Double.NegativeInfinity"/>,
        /// which are also invalid in JSON. Parsing these values works though their <see cref="Type"/> will be <see cref="JsonValueType.UnknownLiteral"/> after parsing.</item>
        /// </list></note>
        /// </summary>
        /// <param name="value">The value.</param>
        public JsonValue(double value)
        {
            Type = JsonValueType.Number;
            _value = value.ToRoundtripString();
        }

        public JsonValue(JsonArray array)
        {
            if (array == null)
            {
                this = Null;
                return;
            }

            Type = JsonValueType.Array;
            _value = array;
        }

        public JsonValue(JsonObject obj)
        {
            if (obj == null)
            {
                this = Null;
                return;
            }

            Type = JsonValueType.Object;
            _value = obj;
        }

        #endregion

        #region Internal Constructors

        internal JsonValue(JsonValueType type, string value)
        {
            _value = value;
            Type = type;
        }

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        #region Public Methods

        public static JsonValue Parse(TextReader reader) => JsonParser.Parse(reader ?? throw new ArgumentNullException(nameof(reader)));
        public static JsonValue Parse(Stream stream, Encoding encoding = null) => Parse(new StreamReader(stream ?? throw new ArgumentNullException(nameof(stream)), encoding ?? Encoding.UTF8));
        public static JsonValue Parse(string s) => Parse(new StringReader(s));

        /// <summary>
        /// Forces <paramref name="value"/> to be treated as a JSON number, even if it cannot be represented as a valid number in JavaScript.
        /// <br/><note>
        /// Please note that <see cref="AsNumber"/> property of the result may return a less precise value, or even <see langword="null"/>,
        /// though serializing to JSON by the <see cref="ToString"/> method preserves the specified <paramref name="value"/>.
        /// </note>
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static JsonValue FromNumberUnchecked(string value) => value == null ? Null : new JsonValue(JsonValueType.Number, value);

        // NOTE: This may allow produce invalid JSON (just like FromNumberUnchecked)
        public static JsonValue FromLiteralUnchecked(string value) => value == null ? Null : new JsonValue(JsonValueType.UnknownLiteral, value);

        #endregion

        #region Internal Methods

        internal static void WriteJsonString(StringBuilder builder, string value)
        {
            builder.Append('"');
            foreach (char c in value)
            {
                switch (c)
                {
                    case '\b':
                        builder.Append(@"\b");
                        break;
                    case '\f':
                        builder.Append(@"\f");
                        break;
                    case '\n':
                        builder.Append(@"\n");
                        break;
                    case '\r':
                        builder.Append(@"\r");
                        break;
                    case '\t':
                        builder.Append(@"\t");
                        break;
                    case '"':
                        builder.Append(@"\""");
                        break;
                    case '\\':
                        builder.Append(@"\\");
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }

            builder.Append('"');
        }

        #endregion

        #endregion

        #region Instance Methods

        #region Public Methods
        
        public override string ToString()
        {
            if (Type <= JsonValueType.Number)
                return Type == JsonValueType.Undefined ? UndefinedLiteral : AsLiteral;

            var result = new StringBuilder(_value is string s ? s.Length + 2 : 64);
            Dump(result);
            return result.ToString();
        }

        public bool Equals(JsonValue other) => Type == other.Type && Equals(_value, other._value);

        public override bool Equals(object obj) => obj is JsonValue other && Equals(other);

        public override int GetHashCode() => (Type, _value).GetHashCode();

        #endregion

        #region Internal Methods

        internal void Dump(StringBuilder builder)
        {
            switch (Type)
            {
                case JsonValueType.String:
                    WriteJsonString(builder, AsLiteral);
                    return;

                case JsonValueType.Object:
                    AsObject.Dump(builder);
                    return;

                case JsonValueType.Array:
                    AsArray.Dump(builder);
                    return;

                default:
                    Debug.Assert(!IsUndefined, "Undefined value is not expected in Dump");
                    builder.Append(AsLiteral);
                    return;
            }

        }

        #endregion

        #endregion

        #endregion
    }
}
