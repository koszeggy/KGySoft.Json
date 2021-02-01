#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using KGySoft.CoreLibraries;

#endregion

namespace TradeSystem.Json
{
    public readonly struct JsonValue : IEquatable<JsonValue>
    {
        #region Constants

        private const string _null = "null";
        private const string _true = "true";
        private const string _false = "false";

        #endregion

        #region Fields

        #region Static Fields

        public static readonly JsonValue None = default;
        public static readonly JsonValue Null = new JsonValue(JsonValueType.Null, _null);
        public static readonly JsonValue True = new JsonValue(JsonValueType.Boolean, _true);
        public static readonly JsonValue False = new JsonValue(JsonValueType.Boolean, _false);

        #endregion

        #region Instance Fields

        private readonly object _value;

        #endregion

        #endregion

        #region Properties and Indexers
        
        #region Properties

        public JsonValueType Type { get; }

        public string AsLiteral => Type <= JsonValueType.String ? (string)_value : null;
        public IList<JsonValue> AsArray => _value as IList<JsonValue>;
        public IDictionary<string, JsonValue> AsObject => _value as IDictionary<string, JsonValue>;

        #endregion

        #region Indexers

        public JsonValue this[int arrayIndex] => _value is IList<JsonValue> list && (uint)arrayIndex < (uint)list.Count
            ? list[arrayIndex] 
            : None;

        public JsonValue this[string propertyName] => _value is IDictionary<string, JsonValue> dict
            ? dict.GetValueOrDefault(propertyName)
            : None;

        #endregion

        #endregion

        #region Operators

        public static bool operator ==(JsonValue left, JsonValue right) => left.Equals(right);
        public static bool operator !=(JsonValue left, JsonValue right) => !left.Equals(right);

        #endregion

        #region Constructors

        #region Internal Constructors

        internal JsonValue(StringBuilder value, bool isString)
            : this(isString ? JsonValueType.String : JsonValueType.Undefined, value.ToString())
        {
            if (Type != JsonValueType.Undefined)
                return;

            string s = (string)_value;
            if (s == _null)
                this = Null;
            else if (s == _true)
                this = True;
            else if (s == _false)
                this = False;
            else if (Double.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out double d)
                && !Double.IsInfinity(d) && !Double.IsNaN(d))
            {
                Type = JsonValueType.Number;
            }
        }

        internal JsonValue(IList<JsonValue> values)
        {
            Type = JsonValueType.Array;
            _value = values;
        }

        internal JsonValue(IDictionary<string, JsonValue> properties)
        {
            Type = JsonValueType.Object;
            _value = properties;
        }

        #endregion

        #region Private Constructors

        private JsonValue(JsonValueType type, string value)
        {
            Type = type;
            _value = value;
        }

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        #region Public Methods

        public static JsonValue FromBoolean(bool value) => value ? True : False;

        public static JsonValue FromString(string value)
            => new JsonValue(JsonValueType.String, value ?? throw new ArgumentNullException(nameof(value)));

        public static JsonValue FromNumber(double value)
            => Double.IsInfinity(value) || Double.IsNaN(value)
                ? throw new ArgumentOutOfRangeException(nameof(value))
                : new JsonValue(JsonValueType.Number, value.ToRoundtripString());

        public static JsonValue FromArray(IList<JsonValue> values = null) => new JsonValue(values ?? new List<JsonValue>());

        public static JsonValue FromObject(IDictionary<string, JsonValue> properties = null) => new JsonValue(properties ?? new Dictionary<string, JsonValue>());

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
                    return $"{{{String.Join(",", AsObject.Select(p => new JsonProperty(p.Key, p.Value)))}}}";
                case JsonValueType.Array:
                    return $"[{String.Join(",", AsArray)}]";
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
