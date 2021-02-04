#region Usings

using System;
using System.Globalization;

#endregion

namespace TradeSystem.Json
{
    public static class JsonValueExtensions
    {
        #region Methods

        public static int? AsInt32(this in JsonValue value, JsonValueType expectedType = default)
        {
            string s;
            if (expectedType != JsonValueType.Undefined && value.Type != expectedType || (s = value.AsLiteral) == null)
                return null;

            return Int32.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out int result)
                ? result
                : default(int?);
        }

        public static string AsString(this in JsonValue value, JsonValueType expectedType)
        {
            string s;
            if (expectedType != JsonValueType.Undefined && value.Type != expectedType || (s = value.AsLiteral) == null)
                return null;

            return s;
        }

        // TODO: for all other .NET types

        #endregion
    }
}