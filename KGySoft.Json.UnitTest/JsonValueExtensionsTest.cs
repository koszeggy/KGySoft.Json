#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonValueExtensionsTest.cs
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

using NUnit.Framework;

#endregion

namespace KGySoft.Json.UnitTest
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

        #endregion
    }
}
