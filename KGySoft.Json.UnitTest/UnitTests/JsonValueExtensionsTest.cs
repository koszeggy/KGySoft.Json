#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonValueExtensionsTest.cs
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
#if !NET35
using System.Numerics;
#endif

using NUnit.Framework;

#endregion

namespace KGySoft.Json.UnitTests
{
    [TestFixture]
    public class JsonValueExtensionsTest
    {
        #region Methods

        [TestCase("true", JsonValueType.Undefined, true)]
        [TestCase("true", JsonValueType.Boolean, true)]
        [TestCase("true", JsonValueType.String, null)]
        [TestCase("\"true\"", JsonValueType.Undefined, true)]
        [TestCase("\"true\"", JsonValueType.Boolean, null)]
        [TestCase("\"true\"", JsonValueType.String, true)]
        [TestCase("null", JsonValueType.Boolean, null)]
        [TestCase("null", JsonValueType.String, null)]
        [TestCase("0", JsonValueType.Boolean, null)]
        [TestCase("0", JsonValueType.Number, false)]
        [TestCase("\"0\"", JsonValueType.Number, null)]
        [TestCase("\"0\"", JsonValueType.String, false)]
        [TestCase("1", JsonValueType.Boolean, null)]
        [TestCase("1", JsonValueType.Number, true)]
        [TestCase("\"1\"", JsonValueType.Number, null)]
        [TestCase("\"1\"", JsonValueType.String, true)]
        public void BoolTest(string json, JsonValueType expectedType, bool? expectedResult)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual(expectedResult, value.AsBoolean(expectedType));
            Assert.AreEqual(expectedResult.HasValue, value.TryGetBoolean(out bool actualValue, expectedType));
            Assert.AreEqual(expectedResult.HasValue && expectedResult.Value, actualValue);
            Assert.AreEqual(expectedResult.HasValue && expectedResult.Value, value.GetBooleanOrDefault(default, expectedType));
            Assert.AreEqual(expectedResult, expectedResult.ToJson().AsBoolean);
        }

        [TestCase("1", JsonValueType.Undefined, 1)]
        [TestCase("1", JsonValueType.Number, 1)]
        [TestCase("1", JsonValueType.String, null)]
        [TestCase("\"1\"", JsonValueType.Undefined, 1)]
        [TestCase("\"1\"", JsonValueType.Number, null)]
        [TestCase("\"1\"", JsonValueType.String, 1)]
        [TestCase("null", JsonValueType.Number, null)]
        [TestCase("null", JsonValueType.String, null)]
        [TestCase("1.23", JsonValueType.Number, null)]
        [TestCase("\"x\"", JsonValueType.String, null)]
        public void ByteTest(string json, JsonValueType expectedType, byte? expectedResult)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual(expectedResult, value.AsByte(expectedType));
            Assert.AreEqual(expectedResult.HasValue, value.TryGetByte(out byte actualValue, expectedType));
            Assert.AreEqual(expectedResult ?? 0, actualValue);
            Assert.AreEqual(expectedResult ?? 0, value.GetByteOrDefault(default, expectedType));
            Assert.AreEqual(expectedResult, expectedResult.ToJson().AsByte());
            Assert.AreEqual(expectedResult, expectedResult.ToJson(true).AsByte());
        }

        [TestCase("1", JsonValueType.Undefined, 1)]
        [TestCase("1", JsonValueType.Number, 1)]
        [TestCase("1", JsonValueType.String, null)]
        [TestCase("\"1\"", JsonValueType.Undefined, 1)]
        [TestCase("\"1\"", JsonValueType.Number, null)]
        [TestCase("\"1\"", JsonValueType.String, 1)]
        [TestCase("null", JsonValueType.Number, null)]
        [TestCase("null", JsonValueType.String, null)]
        [TestCase("1.23", JsonValueType.Number, null)]
        [TestCase("\"x\"", JsonValueType.String, null)]
        public void SByteTest(string json, JsonValueType expectedType, sbyte? expectedResult)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual(expectedResult, value.AsSByte(expectedType));
            Assert.AreEqual(expectedResult.HasValue, value.TryGetSByte(out sbyte actualValue, expectedType));
            Assert.AreEqual(expectedResult ?? 0, actualValue);
            Assert.AreEqual(expectedResult ?? 0, value.GetSByteOrDefault(default, expectedType));
            Assert.AreEqual(expectedResult, expectedResult.ToJson().AsSByte());
            Assert.AreEqual(expectedResult, expectedResult.ToJson(true).AsSByte());
        }

        [TestCase("1", JsonValueType.Undefined, 1)]
        [TestCase("1", JsonValueType.Number, 1)]
        [TestCase("1", JsonValueType.String, null)]
        [TestCase("\"1\"", JsonValueType.Undefined, 1)]
        [TestCase("\"1\"", JsonValueType.Number, null)]
        [TestCase("\"1\"", JsonValueType.String, 1)]
        [TestCase("null", JsonValueType.Number, null)]
        [TestCase("null", JsonValueType.String, null)]
        [TestCase("1.23", JsonValueType.Number, null)]
        [TestCase("\"x\"", JsonValueType.String, null)]
        public void Int16Test(string json, JsonValueType expectedType, short? expectedResult)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual(expectedResult, value.AsInt16(expectedType));
            Assert.AreEqual(expectedResult.HasValue, value.TryGetInt16(out short actualValue, expectedType));
            Assert.AreEqual(expectedResult ?? 0, actualValue);
            Assert.AreEqual(expectedResult ?? 0, value.GetInt16OrDefault(default, expectedType));
            Assert.AreEqual(expectedResult, expectedResult.ToJson().AsInt16());
            Assert.AreEqual(expectedResult, expectedResult.ToJson(true).AsInt16());
        }

        [TestCase("1", JsonValueType.Undefined, (ushort)1)]
        [TestCase("1", JsonValueType.Number, (ushort)1)]
        [TestCase("1", JsonValueType.String, null)]
        [TestCase("\"1\"", JsonValueType.Undefined, (ushort)1)]
        [TestCase("\"1\"", JsonValueType.Number, null)]
        [TestCase("\"1\"", JsonValueType.String, (ushort)1)]
        [TestCase("null", JsonValueType.Number, null)]
        [TestCase("null", JsonValueType.String, null)]
        [TestCase("1.23", JsonValueType.Number, null)]
        [TestCase("\"x\"", JsonValueType.String, null)]
        public void UInt16Test(string json, JsonValueType expectedType, ushort? expectedResult)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual(expectedResult, value.AsUInt16(expectedType));
            Assert.AreEqual(expectedResult.HasValue, value.TryGetUInt16(out ushort actualValue, expectedType));
            Assert.AreEqual(expectedResult ?? 0, actualValue);
            Assert.AreEqual(expectedResult ?? 0, value.GetUInt16OrDefault(default, expectedType));
            Assert.AreEqual(expectedResult, expectedResult.ToJson().AsUInt16());
            Assert.AreEqual(expectedResult, expectedResult.ToJson(true).AsUInt16());
        }

        [TestCase("1", JsonValueType.Undefined, 1)]
        [TestCase("1", JsonValueType.Number, 1)]
        [TestCase("1", JsonValueType.String, null)]
        [TestCase("\"1\"", JsonValueType.Undefined, 1)]
        [TestCase("\"1\"", JsonValueType.Number, null)]
        [TestCase("\"1\"", JsonValueType.String, 1)]
        [TestCase("null", JsonValueType.Number, null)]
        [TestCase("null", JsonValueType.String, null)]
        [TestCase("1.23", JsonValueType.Number, null)]
        [TestCase("\"x\"", JsonValueType.String, null)]
        public void Int32Test(string json, JsonValueType expectedType, int? expectedResult)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual(expectedResult, value.AsInt32(expectedType));
            Assert.AreEqual(expectedResult.HasValue, value.TryGetInt32(out int actualValue, expectedType));
            Assert.AreEqual(expectedResult ?? 0, actualValue);
            Assert.AreEqual(expectedResult ?? 0, value.GetInt32OrDefault(default, expectedType));
            Assert.AreEqual(expectedResult, expectedResult.ToJson().AsInt32());
            Assert.AreEqual(expectedResult, expectedResult.ToJson(true).AsInt32());
        }

        [TestCase("1", JsonValueType.Undefined, 1U)]
        [TestCase("1", JsonValueType.Number, 1U)]
        [TestCase("1", JsonValueType.String, null)]
        [TestCase("\"1\"", JsonValueType.Undefined, 1U)]
        [TestCase("\"1\"", JsonValueType.Number, null)]
        [TestCase("\"1\"", JsonValueType.String, 1U)]
        [TestCase("null", JsonValueType.Number, null)]
        [TestCase("null", JsonValueType.String, null)]
        [TestCase("1.23", JsonValueType.Number, null)]
        [TestCase("\"x\"", JsonValueType.String, null)]
        public void UInt32Test(string json, JsonValueType expectedType, uint? expectedResult)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual(expectedResult, value.AsUInt32(expectedType));
            Assert.AreEqual(expectedResult.HasValue, value.TryGetUInt32(out uint actualValue, expectedType));
            Assert.AreEqual(expectedResult ?? 0, actualValue);
            Assert.AreEqual(expectedResult ?? 0, value.GetUInt32OrDefault(default, expectedType));
            Assert.AreEqual(expectedResult, expectedResult.ToJson().AsUInt32());
            Assert.AreEqual(expectedResult, expectedResult.ToJson(true).AsUInt32());
        }

        [TestCase("1", JsonValueType.Undefined, 1L)]
        [TestCase("1", JsonValueType.Number, 1L)]
        [TestCase("1", JsonValueType.String, null)]
        [TestCase("\"1\"", JsonValueType.Undefined, 1L)]
        [TestCase("\"1\"", JsonValueType.Number, null)]
        [TestCase("\"1\"", JsonValueType.String, 1L)]
        [TestCase("null", JsonValueType.Number, null)]
        [TestCase("null", JsonValueType.String, null)]
        [TestCase("1.23", JsonValueType.Number, null)]
        [TestCase("\"x\"", JsonValueType.String, null)]
        public void Int64Test(string json, JsonValueType expectedType, long? expectedResult)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual(expectedResult, value.AsInt64(expectedType));
            Assert.AreEqual(expectedResult.HasValue, value.TryGetInt64(out long actualValue, expectedType));
            Assert.AreEqual(expectedResult ?? 0, actualValue);
            Assert.AreEqual(expectedResult ?? 0, value.GetInt64OrDefault(default, expectedType));
            Assert.AreEqual(expectedResult, expectedResult.ToJson(false).AsInt64());
            Assert.AreEqual(expectedResult, expectedResult.ToJson().AsInt64());
        }

        [TestCase("1", JsonValueType.Undefined, 1UL)]
        [TestCase("1", JsonValueType.Number, 1UL)]
        [TestCase("1", JsonValueType.String, null)]
        [TestCase("\"1\"", JsonValueType.Undefined, 1UL)]
        [TestCase("\"1\"", JsonValueType.Number, null)]
        [TestCase("\"1\"", JsonValueType.String, 1UL)]
        [TestCase("null", JsonValueType.Number, null)]
        [TestCase("null", JsonValueType.String, null)]
        [TestCase("1.23", JsonValueType.Number, null)]
        [TestCase("\"x\"", JsonValueType.String, null)]
        public void UInt64Test(string json, JsonValueType expectedType, ulong? expectedResult)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual(expectedResult, value.AsUInt64(expectedType));
            Assert.AreEqual(expectedResult.HasValue, value.TryGetUInt64(out ulong actualValue, expectedType));
            Assert.AreEqual(expectedResult ?? 0, actualValue);
            Assert.AreEqual(expectedResult ?? 0, value.GetUInt64OrDefault(default, expectedType));
            Assert.AreEqual(expectedResult, expectedResult.ToJson(false).AsUInt64());
            Assert.AreEqual(expectedResult, expectedResult.ToJson().AsUInt64());
        }

#if !NET35
        [TestCase("1", JsonValueType.Undefined, 1)]
        [TestCase("1", JsonValueType.Number, 1)]
        [TestCase("1", JsonValueType.String, null)]
        [TestCase("\"1\"", JsonValueType.Undefined, 1)]
        [TestCase("\"1\"", JsonValueType.Number, null)]
        [TestCase("\"1\"", JsonValueType.String, 1)]
        [TestCase("null", JsonValueType.Number, null)]
        [TestCase("null", JsonValueType.String, null)]
        [TestCase("1.23", JsonValueType.Number, null)]
        [TestCase("\"x\"", JsonValueType.String, null)]
        public void BigIntegerTest(string json, JsonValueType expectedType, int? expectedResultAsInt)
        {
            BigInteger? expectedResult = expectedResultAsInt;
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual(expectedResult, value.AsBigInteger(expectedType));
            Assert.AreEqual(expectedResult.HasValue, value.TryGetBigInteger(out BigInteger actualValue, expectedType));
            Assert.AreEqual(expectedResult ?? BigInteger.Zero, actualValue);
            Assert.AreEqual(expectedResult ?? BigInteger.Zero, value.GetBigIntegerOrDefault(default, expectedType));
            Assert.AreEqual(expectedResult, expectedResult.ToJson(false).AsBigInteger());
            Assert.AreEqual(expectedResult, expectedResult.ToJson().AsBigInteger());
        }
#endif

        [TestCase("1", JsonValueType.Undefined, 1f)]
        [TestCase("1", JsonValueType.Number, 1f)]
        [TestCase("1", JsonValueType.String, null)]
        [TestCase("\"1\"", JsonValueType.Undefined, 1f)]
        [TestCase("\"1\"", JsonValueType.Number, null)]
        [TestCase("\"1\"", JsonValueType.String, 1f)]
        [TestCase("null", JsonValueType.Number, null)]
        [TestCase("null", JsonValueType.String, null)]
        [TestCase("1.23", JsonValueType.Number, 1.23f)]
        [TestCase("\"x\"", JsonValueType.String, null)]
        public void SingleTest(string json, JsonValueType expectedType, float? expectedResult)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual(expectedResult, value.AsSingle(expectedType));
            Assert.AreEqual(expectedResult.HasValue, value.TryGetSingle(out float actualValue, expectedType));
            Assert.AreEqual(expectedResult ?? 0, actualValue);
            Assert.AreEqual(expectedResult ?? 0, value.GetSingleOrDefault(default, expectedType));
            Assert.AreEqual(expectedResult, expectedResult.ToJson().AsSingle());
            Assert.AreEqual(expectedResult, expectedResult.ToJson(true).AsSingle());
        }

        [TestCase("1", JsonValueType.Undefined, 1d)]
        [TestCase("1", JsonValueType.Number, 1d)]
        [TestCase("1", JsonValueType.String, null)]
        [TestCase("\"1\"", JsonValueType.Undefined, 1d)]
        [TestCase("\"1\"", JsonValueType.Number, null)]
        [TestCase("\"1\"", JsonValueType.String, 1d)]
        [TestCase("null", JsonValueType.Number, null)]
        [TestCase("null", JsonValueType.String, null)]
        [TestCase("1.23", JsonValueType.Number, 1.23d)]
        [TestCase("\"x\"", JsonValueType.String, null)]
        public void DoubleTest(string json, JsonValueType expectedType, double? expectedResult)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual(expectedResult, value.AsDouble(expectedType));
            Assert.AreEqual(expectedResult.HasValue, value.TryGetDouble(out double actualValue, expectedType));
            Assert.AreEqual(expectedResult ?? 0, actualValue);
            Assert.AreEqual(expectedResult ?? 0, value.GetDoubleOrDefault(default, expectedType));
            Assert.AreEqual(expectedResult, expectedResult.ToJson().AsDouble());
            Assert.AreEqual(expectedResult, expectedResult.ToJson(true).AsDouble());
        }

        [TestCase("1", JsonValueType.Undefined, 1)]
        [TestCase("1", JsonValueType.Number, 1)]
        [TestCase("1", JsonValueType.String, null)]
        [TestCase("\"1\"", JsonValueType.Undefined, 1)]
        [TestCase("\"1\"", JsonValueType.Number, null)]
        [TestCase("\"1\"", JsonValueType.String, 1)]
        [TestCase("null", JsonValueType.Number, null)]
        [TestCase("null", JsonValueType.String, null)]
        [TestCase("1.23", JsonValueType.Number, 1.23)]
        [TestCase("\"x\"", JsonValueType.String, null)]
        public void DecimalTest(string json, JsonValueType expectedType, decimal? expectedResult)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual(expectedResult, value.AsDecimal(expectedType));
            Assert.AreEqual(expectedResult.HasValue, value.TryGetDecimal(out decimal actualValue, expectedType));
            Assert.AreEqual(expectedResult ?? 0, actualValue);
            Assert.AreEqual(expectedResult ?? 0, value.GetDecimalOrDefault(default, expectedType));
            Assert.AreEqual(expectedResult, expectedResult.ToJson(false).AsDecimal());
            Assert.AreEqual(expectedResult, expectedResult.ToJson().AsDecimal());
        }

        [TestCase("1", JsonValueType.Undefined, "1")]
        [TestCase("1", JsonValueType.Number, "1")]
        [TestCase("1", JsonValueType.String, null)]
        [TestCase("\"1\"", JsonValueType.Undefined, "1")]
        [TestCase("\"1\"", JsonValueType.Number, null)]
        [TestCase("\"1\"", JsonValueType.String, "1")]
        [TestCase("null", JsonValueType.Number, null)]
        [TestCase("null", JsonValueType.String, null)]
        [TestCase("1.23", JsonValueType.Number, "1.23")]
        [TestCase("\"x\"", JsonValueType.String, "x")]
        [TestCase("true", JsonValueType.Undefined, "true")]
        [TestCase("true", JsonValueType.String, null)]
        [TestCase("true", JsonValueType.Boolean, "true")]
        [TestCase("\"true\"", JsonValueType.Undefined, "true")]
        [TestCase("\"true\"", JsonValueType.String, "true")]
        [TestCase("\"true\"", JsonValueType.Boolean, null)]
        public void StringTest(string json, JsonValueType expectedType, string? expectedResult)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual(expectedResult, value.AsString(expectedType));
            Assert.AreEqual(expectedResult != null, value.TryGetString(out string? actualValue, expectedType));
            Assert.AreEqual(expectedResult, actualValue);
            Assert.AreEqual(expectedResult, value.GetStringOrDefault(default, expectedType));
            Assert.AreEqual(expectedResult, expectedResult.ToJson().AsString());
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TryGetString_AllowNullIfStringIsExpectedTest(bool allowNullIfStringIsExpected)
        {
            var json = new JsonObject
            {
                { "String", "Value" },
                { "Null", JsonValue.Null },
            };

            // string as string
            Assert.IsTrue(json["String"].TryGetString(out string? value, JsonValueType.String, allowNullIfStringIsExpected));
            Assert.AreEqual("Value", value);

            // string as anything
            Assert.IsTrue(json["String"].TryGetString(out value, JsonValueType.Undefined, allowNullIfStringIsExpected));
            Assert.AreEqual("Value", value);

            // nonexistent property as string
            Assert.IsFalse(json["Unknown"].TryGetString(out value, JsonValueType.String, allowNullIfStringIsExpected));
            Assert.IsNull(value);

            // nonexistent property as anything
            Assert.IsFalse(json["Unknown"].TryGetString(out value, JsonValueType.Undefined, allowNullIfStringIsExpected));
            Assert.IsNull(value);

            // null as anything
            Assert.IsTrue(json["Null"].TryGetString(out value, JsonValueType.Undefined, allowNullIfStringIsExpected));
            Assert.IsNull(value);

            // null as null
            Assert.IsTrue(json["Null"].TryGetString(out value, JsonValueType.Null, allowNullIfStringIsExpected));
            Assert.IsNull(value);

            // null as string
            Assert.AreEqual(allowNullIfStringIsExpected, json["Null"].TryGetString(out value, JsonValueType.String, allowNullIfStringIsExpected));
            Assert.IsNull(value);
        }

        [TestCase(ConsoleColor.DarkBlue, JsonEnumFormat.PascalCase, "\"DarkBlue\"")]
        [TestCase(ConsoleColor.DarkBlue, JsonEnumFormat.CamelCase, "\"darkBlue\"")]
        [TestCase(ConsoleColor.DarkBlue, JsonEnumFormat.LowerCase, "\"darkblue\"")]
        [TestCase(ConsoleColor.DarkBlue, JsonEnumFormat.LowerCaseWithUnderscores, "\"dark_blue\"")]
        [TestCase(ConsoleColor.DarkBlue, JsonEnumFormat.LowerCaseWithHyphens, "\"dark-blue\"")]
        [TestCase(ConsoleColor.DarkBlue, JsonEnumFormat.UpperCase, "\"DARKBLUE\"")]
        [TestCase(ConsoleColor.DarkBlue, JsonEnumFormat.UpperCaseWithUnderscores, "\"DARK_BLUE\"")]
        [TestCase(ConsoleColor.DarkBlue, JsonEnumFormat.UpperCaseWithHyphens, "\"DARK-BLUE\"")]
        [TestCase(ConsoleColor.DarkBlue, JsonEnumFormat.Number, "1")]
        [TestCase(ConsoleColor.DarkBlue, JsonEnumFormat.NumberAsString, "\"1\"")]
        [TestCase(null, JsonEnumFormat.PascalCase, "null")]
        public void FormatEnumTest(ConsoleColor? value, JsonEnumFormat format, string expectedResult)
        {
            JsonValue json = value.ToJson(format);
            Assert.AreEqual(expectedResult, json.ToString());
            Assert.AreEqual(value.HasValue, json.TryGetEnum(true, out ConsoleColor result));
            Assert.AreEqual(value ?? default, result);
        }

        [TestCase(ConsoleModifiers.Alt | ConsoleModifiers.Control, JsonEnumFormat.PascalCase, ",", "\"Alt,Control\"")]
        [TestCase(ConsoleModifiers.Alt | ConsoleModifiers.Control, JsonEnumFormat.LowerCase, "|", "\"alt|control\"")]
        public void FormatFlagsEnumTest(ConsoleModifiers value, JsonEnumFormat format, string separator, string expectedResult)
        {
            JsonValue json = value.ToJson(format, separator);
            Assert.AreEqual(expectedResult, json.ToString());
            Assert.IsTrue(json.TryGetEnum(true, out ConsoleModifiers result, separator));
            Assert.AreEqual(value, result);
        }

        [TestCase("1578870000000", JsonDateTimeFormat.UnixMilliseconds, DateTimeKind.Utc)]
        [TestCase("\"1578870000000\"", JsonDateTimeFormat.UnixMilliseconds, DateTimeKind.Utc)]
        [TestCase("1578870000", JsonDateTimeFormat.UnixSeconds, DateTimeKind.Utc)]
        [TestCase("\"1578870000\"", JsonDateTimeFormat.UnixSeconds, DateTimeKind.Utc)]
        [TestCase("1578870000.000", JsonDateTimeFormat.UnixSecondsFloat, DateTimeKind.Utc)]
        [TestCase("\"1578870000.000\"", JsonDateTimeFormat.UnixSecondsFloat, DateTimeKind.Utc)]
        [TestCase("637144668000000000", JsonDateTimeFormat.Ticks, DateTimeKind.Utc)]
        [TestCase("\"637144668000000000\"", JsonDateTimeFormat.Ticks, DateTimeKind.Utc)]
        [TestCase("\"2020-01-13T01:02:03.456Z\"", JsonDateTimeFormat.Iso8601JavaScript, DateTimeKind.Utc)]
        [TestCase("\"2020-01-13T01:02:03.1234567Z\"", JsonDateTimeFormat.Iso8601Utc, DateTimeKind.Utc)]
        [TestCase("\"2020-01-13T01:02:03.1234567+01:00\"", JsonDateTimeFormat.Iso8601Local, DateTimeKind.Local)]
        [TestCase("\"2020-01-13T01:02:03.1234567\"", JsonDateTimeFormat.Iso8601Roundtrip, DateTimeKind.Unspecified)]
        [TestCase("\"2020-01-13T01:02:03.1234567Z\"", JsonDateTimeFormat.Iso8601Roundtrip, DateTimeKind.Utc)]
        [TestCase("\"2020-01-13T01:02:03.1234567+01:00\"", JsonDateTimeFormat.Iso8601Roundtrip, DateTimeKind.Local)]
        [TestCase("\"2020-01-13\"", JsonDateTimeFormat.Iso8601Date, DateTimeKind.Unspecified)]
        [TestCase("\"2020-01-13T01:02\"", JsonDateTimeFormat.Iso8601Minutes, DateTimeKind.Unspecified)]
        [TestCase("\"2020-01-13T01:02Z\"", JsonDateTimeFormat.Iso8601Minutes, DateTimeKind.Utc)]
        [TestCase("\"2020-01-13T01:02+01:00\"", JsonDateTimeFormat.Iso8601Minutes, DateTimeKind.Local)]
        [TestCase("\"2020-01-13T01:02:03\"", JsonDateTimeFormat.Iso8601Seconds, DateTimeKind.Unspecified)]
        [TestCase("\"2020-01-13T01:02:03Z\"", JsonDateTimeFormat.Iso8601Seconds, DateTimeKind.Utc)]
        [TestCase("\"2020-01-13T01:02:03+01:00\"", JsonDateTimeFormat.Iso8601Seconds, DateTimeKind.Local)]
        [TestCase("\"2020-01-13T01:02:03.123\"", JsonDateTimeFormat.Iso8601Milliseconds, DateTimeKind.Unspecified)]
        [TestCase("\"2020-01-13T01:02:03.123Z\"", JsonDateTimeFormat.Iso8601Milliseconds, DateTimeKind.Utc)]
        [TestCase("\"2020-01-13T01:02:03.123+01:00\"", JsonDateTimeFormat.Iso8601Milliseconds, DateTimeKind.Local)]
        [TestCase("\"/Date(1578870000000)/\"", JsonDateTimeFormat.MicrosoftLegacy, DateTimeKind.Utc)]
        [TestCase("\"/Date(1578870000000+0100)/\"", JsonDateTimeFormat.MicrosoftLegacy, DateTimeKind.Local)]
        public void DateTimePredefinedFormatsTest(string json, JsonDateTimeFormat format, DateTimeKind expectedKind)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.IsTrue(value.TryGetDateTime(format, out DateTime dateTime));
            if (expectedKind != DateTimeKind.Local || TimeZoneInfo.Local.GetUtcOffset(dateTime) == TimeSpan.FromHours(1))
                Assert.AreEqual(value, dateTime.ToJson(format, value.Type == JsonValueType.String));
            Assert.AreEqual(expectedKind, dateTime.Kind);
            Assert.AreEqual(dateTime, value.AsDateTime(format));
            Assert.AreEqual(dateTime, value.AsDateTime());
            Assert.AreEqual(dateTime, value.GetDateTimeOrDefault(format));
            Assert.AreEqual(dateTime, value.GetDateTimeOrDefault());
        }

        [TestCase("\"20200101\"", "yyyyMMdd", DateTimeKind.Unspecified)]
        [TestCase("\"20200101|010203|Z\"", "yyyyMMdd'|'HHmmss'|'K", DateTimeKind.Utc)]
        [TestCase("\"20200101|010203|+01:00\"", "yyyyMMdd'|'HHmmss'|'zzz", DateTimeKind.Local)]
        public void DateTimeExactFormatTest(string json, string format, DateTimeKind expectedKind)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.IsTrue(value.TryGetDateTime(format, out DateTime dateTime));
            if (expectedKind != DateTimeKind.Local || TimeZoneInfo.Local.GetUtcOffset(dateTime) == TimeSpan.FromHours(1))
                Assert.AreEqual(value, dateTime.ToJson(format));
            Assert.AreEqual(expectedKind, dateTime.Kind);
            Assert.AreEqual(dateTime, value.AsDateTime(format));
            Assert.AreEqual(dateTime, value.GetDateTimeOrDefault(format));
        }

        [TestCase("1578870000000", JsonDateTimeFormat.UnixMilliseconds)]
        [TestCase("\"1578870000000\"", JsonDateTimeFormat.UnixMilliseconds)]
        [TestCase("1578870000", JsonDateTimeFormat.UnixSeconds)]
        [TestCase("\"1578870000\"", JsonDateTimeFormat.UnixSeconds)]
        [TestCase("1578870000.000", JsonDateTimeFormat.UnixSecondsFloat)]
        [TestCase("\"1578870000.000\"", JsonDateTimeFormat.UnixSecondsFloat)]
        [TestCase("637144668000000000", JsonDateTimeFormat.Ticks)]
        [TestCase("\"637144668000000000\"", JsonDateTimeFormat.Ticks)]
        [TestCase("\"2020-01-13T01:02:03.456Z\"", JsonDateTimeFormat.Iso8601JavaScript)]
        [TestCase("\"2020-01-13T01:02:03.1234567Z\"", JsonDateTimeFormat.Iso8601Utc)]
        [TestCase("\"2020-01-13T01:02:03.1234567+01:00\"", JsonDateTimeFormat.Iso8601Local)]
        [TestCase("\"2020-01-13T01:02:03.1234567+01:00\"", JsonDateTimeFormat.Iso8601Roundtrip)]
        [TestCase("\"2020-01-13\"", JsonDateTimeFormat.Iso8601Date)]
        [TestCase("\"2020-01-13T01:02+01:00\"", JsonDateTimeFormat.Iso8601Minutes)]
        [TestCase("\"2020-01-13T01:02:03+01:00\"", JsonDateTimeFormat.Iso8601Seconds)]
        [TestCase("\"2020-01-13T01:02:03.123+01:00\"", JsonDateTimeFormat.Iso8601Milliseconds)]
        [TestCase("\"/Date(1578870000000+0000)/\"", JsonDateTimeFormat.MicrosoftLegacy)]
        [TestCase("\"/Date(1578870000000+0100)/\"", JsonDateTimeFormat.MicrosoftLegacy)]
        public void DateTimeOffsetPredefinedFormatsTest(string json, JsonDateTimeFormat format)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.IsTrue(value.TryGetDateTimeOffset(format, out DateTimeOffset dateTimeOffset));
            Assert.AreEqual(value, dateTimeOffset.ToJson(format, value.Type == JsonValueType.String));
            Assert.AreEqual(dateTimeOffset, value.AsDateTimeOffset(format));
            Assert.AreEqual(dateTimeOffset, value.AsDateTimeOffset());
            Assert.AreEqual(dateTimeOffset, value.GetDateTimeOffsetOrDefault(format));
            Assert.AreEqual(dateTimeOffset, value.GetDateTimeOffsetOrDefault());
        }

        [TestCase("\"20200101\"", "yyyyMMdd")]
        [TestCase("\"20200101|010203|+01:00\"", "yyyyMMdd'|'HHmmss'|'zzz")]
        public void DateTimeOffsetExactFormatTest(string json, string format)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.IsTrue(value.TryGetDateTimeOffset(format, out DateTimeOffset dateTimeOffset));
            Assert.AreEqual(value, dateTimeOffset.ToJson(format));
            Assert.AreEqual(dateTimeOffset, value.AsDateTimeOffset(format));
            Assert.AreEqual(dateTimeOffset, value.GetDateTimeOffsetOrDefault(format));
        }

        [TestCase("86400000", JsonTimeSpanFormat.Milliseconds)]
        [TestCase("\"86400000\"", JsonTimeSpanFormat.Milliseconds)]
        [TestCase("637144668000000000", JsonTimeSpanFormat.Ticks)]
        [TestCase("\"637144668000000000\"", JsonTimeSpanFormat.Ticks)]
        [TestCase("\"01:02:03\"", JsonTimeSpanFormat.Text)]
        [TestCase("\"00:00:00.1234567\"", JsonTimeSpanFormat.Text)]
        public void TimeSpanTest(string json, JsonTimeSpanFormat format)
        {
            JsonValue value = JsonValue.Parse(json);
            Assert.IsTrue(value.TryGetTimeSpan(format, out TimeSpan timeSpan));
            Assert.AreEqual(value, timeSpan.ToJson(format, value.Type == JsonValueType.String));
            Assert.AreEqual(timeSpan, value.AsTimeSpan(format));
            Assert.AreEqual(timeSpan, value.AsTimeSpan());
            Assert.AreEqual(timeSpan, value.GetTimeSpanOrDefault(format));
            Assert.AreEqual(timeSpan, value.GetTimeSpanOrDefault());
        }

        [Test]
        public void GuidTest()
        {
            JsonValue value = JsonValue.Parse($"\"{Guid.NewGuid():D}\"");
            Assert.IsTrue(value.TryGetGuid(out Guid guid));
            Assert.AreEqual(value, guid.ToJson());
            Assert.AreEqual(guid, value.AsGuid());
            Assert.AreEqual(guid, value.GetGuidOrDefault());
        }

        #endregion
    }
}
