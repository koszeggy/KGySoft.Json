#region Usings

using System;
using System.Globalization;

#endregion

namespace TradeSystem.Json
{
    public static class JsonValueExtensions
    {
        #region Methods

        public static int? AsInt32(this in JsonValue json, JsonValueType expectedType = default)
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
                return null;

            return Int32.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out int result)
                ? result
                : default(int?);
        }

        public static bool TryGetInt32(this in JsonValue json, out int value, JsonValueType expectedType = default)
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
            {
                value = default;
                return false;
            }

            return Int32.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);
        }

        public static long? AsInt64(this in JsonValue json, JsonValueType expectedType = default)
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
                return null;

            return Int64.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out long result)
                ? result
                : default(long?);
        }

        public static bool TryGetInt64(this in JsonValue json, out long value, JsonValueType expectedType = default)
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
            {
                value = default;
                return false;
            }

            return Int64.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);
        }

        public static double? AsDouble(this in JsonValue json, JsonValueType expectedType = default)
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
                return null;

            return Double.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double result)
                ? result
                : default(double?);
        }

        public static bool TryGetDouble(this in JsonValue json, out double value, JsonValueType expectedType = default)
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
            {
                value = default;
                return false;
            }

            return Double.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out value);
        }

        public static decimal? AsDecimal(this in JsonValue json, JsonValueType expectedType = default)
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
                return null;

            return Decimal.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out decimal result)
                ? result
                : default(decimal?);
        }

        public static bool TryGetDecimal(this in JsonValue json, out decimal value, JsonValueType expectedType = default)
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
            {
                value = default;
                return false;
            }

            return Decimal.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out value);
        }

        public static string AsString(this in JsonValue json, JsonValueType expectedType = default)
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
                return null;

            return json.IsNull ? null : s;
        }

        public static bool TryGetString(this in JsonValue json, out string value, JsonValueType expectedType = default, bool allowNullIfStringIsExpected = false)
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
            {
                value = default;
                return expectedType == JsonValueType.String && json.IsNull && allowNullIfStringIsExpected;
            }

            value = json.IsNull ? null : s;
            return true;
        }

        // TODO: for all other .NET types

        #endregion
    }
}