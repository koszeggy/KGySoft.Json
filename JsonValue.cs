#region Usings

using System;
using System.Collections.Generic;
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
        public double? AsNumber => Type == JsonValueType.Number && Double.TryParse((string) _value, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double value)
                ? value
                : (double?)null;

        public string AsLiteral => _value as string; // for undefined, returns null!
        public IList<JsonValue> AsArray => _value as IList<JsonValue>;
        public IList<JsonProperty> AsObject => _value as IList<JsonProperty>;

        #endregion

        #region Indexers

        public JsonValue this[int arrayIndex] => _value is IList<JsonValue> list && (uint)arrayIndex < (uint)list.Count
            ? list[arrayIndex]
            : Undefined;

        public JsonValue this[string propertyName] => _value is IList<JsonProperty> properties
            ? properties.FirstOrDefault(p => p.Name == propertyName).Value
            : Undefined;

        #endregion

        #endregion

        #region Operators

        public static bool operator ==(JsonValue left, JsonValue right) => left.Equals(right);
        public static bool operator !=(JsonValue left, JsonValue right) => !left.Equals(right);

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

        public JsonValue(double value)
        {
            // Note: JSON Number type is actually a double. Other numeric types (long/decimal) must be encoded as a string to
            // prevent loss of precision at JS side.
            // Note 2: This allows NaN and +-Infinity, which is also invalid JSON. Parsing those will work though type will be UnknownLiteral
            Type = JsonValueType.Number;
            _value = value.ToRoundtripString();
        }

        public JsonValue(IList<JsonValue> values)
        {
            if (values == null)
            {
                this = Null;
                return;
            }

            Type = JsonValueType.Array;
            _value = values;
        }

        public JsonValue(IList<JsonProperty> properties)
        {
            if (properties == null)
            {
                this = Null;
                return;
            }

            Type = JsonValueType.Object;
            _value = properties;
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

        public static JsonValue Parse(Stream utf8Stream) => JsonParser.Parse(utf8Stream ?? throw new ArgumentNullException(nameof(utf8Stream)));
        public static JsonValue Parse(string s) => Parse(new MemoryStream(Encoding.UTF8.GetBytes(s ?? throw new ArgumentNullException(nameof(s)))));

        #endregion

        #region Internal Methods

        internal static string ToJsonString(string stringValue)
        {
            var result = new StringBuilder(stringValue.Length + 2);
            result.Append('"');
            foreach (char c in stringValue)
            {
                switch (c)
                {
                    case '\b':
                        result.Append(@"\b");
                        break;
                    case '\f':
                        result.Append(@"\f");
                        break;
                    case '\n':
                        result.Append(@"\n");
                        break;
                    case '\r':
                        result.Append(@"\r");
                        break;
                    case '\t':
                        result.Append(@"\t");
                        break;
                    case '"':
                        result.Append(@"\""");
                        break;
                    case '\\':
                        result.Append(@"\\");
                        break;
                    default:
                        result.Append(c);
                        break;
                }
            }

            result.Append('"');
            return result.ToString();
        }

        #endregion

        #endregion

        #region Instance Methods

        public override string ToString()
        {
            switch (Type)
            {
                case JsonValueType.String:
                    return ToJsonString(AsLiteral);
                case JsonValueType.Object:
                    return $"{{{String.Join(",", AsObject.Where(p => !p.Value.IsUndefined))}}}";
                case JsonValueType.Array:
                    return $"[{String.Join(",", AsArray.Where(i => !i.IsUndefined))}]";
                case JsonValueType.Undefined:
                    return UndefinedLiteral;
                default:
                    return AsLiteral;
            }
        }

        public bool Equals(JsonValue other) => Type == other.Type && Equals(_value, other._value);

        public override bool Equals(object obj) => obj is JsonValue other && Equals(other);

        public override int GetHashCode() => (Type, _value).GetHashCode();

        #endregion

        #endregion
    }
}
