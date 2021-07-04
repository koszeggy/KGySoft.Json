#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonValueExtensions.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution. If not, then this file is considered as
//  an illegal copy.
//
//  Unauthorized copying of this file, via any medium is strictly prohibited.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Globalization;

using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Json
{
    public static class JsonValueExtensions
    {
        #region Methods

        public static bool? AsBoolean(this in JsonValue json, JsonValueType expectedType = default)
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
                return null;

            return s switch
            {
                JsonValue.TrueLiteral => true,
                JsonValue.FalseLiteral => false,
                _ => null
            };
        }

        public static bool TryGetBoolean(this in JsonValue json, out bool value, JsonValueType expectedType = default)
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
            {
                value = default;
                return false;
            }

            value = s == JsonValue.TrueLiteral;
            return value || s == JsonValue.FalseLiteral;
        }

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

        public static TEnum? AsEnum<TEnum>(this in JsonValue json, JsonValueType expectedType = default)
            where TEnum : struct, Enum
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
                return null;

            return json.IsNull ? null : s.ToEnum<TEnum>();
        }

        public static TEnum? AsEnum<TEnum>(this in JsonValue json, bool ignoreCase, JsonValueType expectedType = default)
            where TEnum : struct, Enum
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
                return null;

            return json.IsNull ? null : Enum<TEnum>.TryParse(s, ignoreCase, out TEnum result) ? result : null;
        }

        public static bool TryGetEnum<TEnum>(this in JsonValue json, out TEnum value, JsonValueType expectedType = default)
            where TEnum : struct, Enum
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null)
            {
                value = default;
                return false;
            }

            TEnum? result = json.IsNull ? default(TEnum?) : s.ToEnum<TEnum>();
            value = result.GetValueOrDefault();
            return result.HasValue;
        }

        public static bool TryGetEnum<TEnum>(this in JsonValue json, bool ignoreCase, out TEnum value, JsonValueType expectedType = default)
            where TEnum : struct, Enum
        {
            string s;
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || (s = json.AsLiteral) == null || json.IsNull)
            {
                value = default;
                return false;
            }

            return Enum<TEnum>.TryParse(s, ignoreCase, out value);
        }

        // TODO: for all other .NET types

        #endregion
    }
}