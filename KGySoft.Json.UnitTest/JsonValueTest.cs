#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonValueTest.cs
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
using System.Linq;

using NUnit.Framework;

#endregion

namespace KGySoft.Json.UnitTest
{
    [TestFixture]
    public class JsonValueTest
    {
        #region Methods

        [Test]
        public void UndefinedValueTest()
        {
            JsonValue json = default;
            Assert.IsTrue(json.IsUndefined);
            Assert.AreEqual(JsonValue.Undefined, json);
            Assert.AreEqual(JsonValueType.Undefined, json.Type);
            Assert.AreEqual(JsonValue.UndefinedLiteral, json.ToString());
            Assert.AreEqual(JsonValue.UndefinedLiteral, json.AsLiteral);
        }

        [Test]
        public void NullValueTest()
        {
            JsonValue json = JsonValue.Null;
            Assert.IsTrue(json.IsNull);
            Assert.AreEqual(JsonValue.Null, json);
            Assert.AreEqual(JsonValueType.Null, json.Type);
            Assert.AreEqual(JsonValue.NullLiteral, json.ToString());
            Assert.AreEqual(JsonValue.NullLiteral, json.AsLiteral);

            // Also from implicit conversions, even though JsonValue is a struct
            json = (string?)null;
            Assert.IsTrue(json.IsNull);

            json = (JsonArray?)null;
            Assert.IsTrue(json.IsNull);

            json = (JsonObject?)null;
            Assert.IsTrue(json.IsNull);
        }

        [Test]
        public void BoolValueTest()
        {
            JsonValue json = true;
            Assert.IsTrue(json == true);
            Assert.IsTrue(json.AsBoolean);
            Assert.AreEqual(true, (bool)json!);
            Assert.AreEqual(JsonValue.True, json);
            Assert.AreEqual(JsonValueType.Boolean, json.Type);
            Assert.AreEqual(JsonValue.TrueLiteral, json.ToString());
            Assert.AreEqual(JsonValue.TrueLiteral, json.AsLiteral);

            json = false;
            Assert.IsTrue(json == false);
            Assert.IsFalse(json.AsBoolean);
            Assert.AreEqual(false, (bool)json!);
            Assert.AreEqual(JsonValue.False, json);
            Assert.AreEqual(JsonValueType.Boolean, json.Type);
            Assert.AreEqual(JsonValue.FalseLiteral, json.ToString());
            Assert.AreEqual(JsonValue.FalseLiteral, json.AsLiteral);
        }

        [Test]
        public void NumberValueTest()
        {
            // from implicit conversion
            JsonValue json = 1.23;
            Assert.AreEqual(JsonValueType.Number, json.Type);
            Assert.AreEqual(1.23, json.AsNumber);
            Assert.AreEqual("1.23", json.AsLiteral);
            Assert.IsNull(json.AsString);
            Assert.AreEqual("1.23", json.ToString());
            Assert.AreEqual(1.23, (double)json!);

            // from int (JavaScript Number is double but int fits into it)
            json = 1;
            Assert.AreEqual(JsonValueType.Number, json.Type);
            Assert.AreEqual(1, json.AsNumber); // actually compared as a double
            Assert.AreEqual("1", json.AsLiteral);
            Assert.IsNull(json.AsString);
            Assert.AreEqual("1", json.ToString());
            Assert.AreEqual(1, (int)json!); // actually via the double conversion
        }

        [Test]
        public void NumberIsDoubleTest()
        {
            long value = (1L << 53) + 1;

            // The operator preserves precision at .NET side but it gives a warning
#pragma warning disable CS0618 // Type or member is obsolete - we purposely use the long operator here
            JsonValue json = value;
#pragma warning restore CS0618

            Assert.AreEqual($"{value}", json.AsLiteral);
            Assert.AreEqual($"{value}", json.ToString());

            // But AsNumber behaves as JavaScript and treats it as a double
            Assert.AreNotEqual($"{value}", $"{json.AsNumber:R}");
        }

        [Test]
        public void StringValueTest()
        {
            // From implicit conversion
            JsonValue json = "value";
            Assert.AreEqual(JsonValueType.String, json.Type);
            Assert.AreEqual("value", json.AsString);
            Assert.AreEqual("value", json.AsLiteral);
            Assert.IsNull(json.AsNumber);
            Assert.AreEqual("\"value\"", json.ToString());
            Assert.AreEqual("value", (string)json!);

            // Large long as string (ToJson creates a string for long values by default)
            long value = (1L << 53) + 1;
            json = value.ToJson();
            Assert.AreEqual($"{value}", json.AsString);
            Assert.AreEqual($"{value}", json.AsLiteral);
            Assert.IsNull(json.AsNumber);
            Assert.AreEqual($"\"{value}\"", json.ToString());
        }

        [Test]
        public void ArrayValueTest()
        {
            JsonValue json = new JsonArray { default, true, 1, 1.23, "value", (string?)null };
            Assert.AreEqual(JsonValueType.Array, json.Type);
            Assert.IsNotNull(json.AsArray);
            Assert.IsTrue(json[0].IsUndefined);
            Assert.IsTrue(json[1] == true);
            Assert.IsTrue(json[2] == 1);
            Assert.IsTrue(json[3] == 1.23);
            Assert.IsTrue(json[4] == "value");
            Assert.IsTrue(json[5].IsNull);

            // just like in JavaScript, out-of-bounds index return undefined
            Assert.IsTrue(json[6].IsUndefined);

            // just like in JavaScript, serialized JSON string replaces undefined values with null
            string serialized = json.ToString();
            Console.WriteLine(serialized);
            JsonValue deserialized = JsonValue.Parse(serialized);
            Assert.AreEqual(JsonValueType.Array, deserialized.Type);
            Assert.AreEqual(json.AsArray!.Count, deserialized.AsArray!.Count);
            Assert.IsTrue(deserialized[0].IsNull);
            CollectionAssert.AreEqual(json.AsArray.Skip(1), deserialized.AsArray.Skip(1));
        }

        [Test]
        public void ObjectValueTest()
        {
            JsonValue json = new JsonObject
            {
                { "undefined", default },
                { "null", (string?)null },
                { "bool", true },
                { "number", 1.23 },
                { "string", "value" },
                { "array", new JsonArray { 1, true, "value" } },
                { "object", new JsonObject {
                    { "name", "value" } }
                }
            };
            Assert.AreEqual(JsonValueType.Object, json.Type);
            Assert.IsNotNull(json.AsObject);
            Assert.IsTrue(json["undefined"].IsUndefined);
            Assert.IsTrue(json["null"].IsNull);
            Assert.IsTrue(json["bool"] == true);
            Assert.IsTrue(json["number"] == 1.23);
            Assert.IsTrue(json["string"] == "value");
            Assert.IsTrue(json["array"][0] == 1);
            Assert.IsTrue(json["object"]["name"] == "value");

            // just like in JavaScript, nonexistent properties return undefined
            Assert.IsTrue(json["unknown"].IsUndefined);

            // just like in JavaScript, serialized JSON string leaves out undefined properties
            string serialized = json.ToString();
            Console.WriteLine(serialized);
            Assert.AreEqual(7, json.AsObject!.Count);
            json.AsObject.Remove("undefined");
            Assert.AreEqual(6, json.AsObject.Count);
            Assert.AreEqual(serialized, json.ToString());
        }

        [Test]
        public void CreateNumberUncheckedTest()
        {
            JsonValue num = JsonValue.CreateNumberUnchecked("NaN");
            Assert.AreEqual(JsonValueType.Number, num.Type);
            Assert.AreEqual(Double.NaN, num.AsNumber);
            Assert.AreEqual("NaN", num.AsLiteral);
            Assert.AreEqual("NaN", num.ToString());

            num = JsonValue.CreateNumberUnchecked("invalid");
            Assert.AreEqual(JsonValueType.Number, num.Type);
            Assert.IsNull(num.AsNumber);
            Assert.AreEqual("invalid", num.ToString());
        }

        [Test]
        public void CreateLiteralUncheckedTest()
        {
            JsonValue num = JsonValue.CreateLiteralUnchecked("ping");
            Assert.AreEqual(JsonValueType.UnknownLiteral, num.Type);
            Assert.AreEqual("ping", num.AsLiteral);
            Assert.AreEqual("ping", num.ToString());
        }

        [TestCase("undefined")]
        [TestCase("null")]
        [TestCase("true")]
        [TestCase("false")]
        [TestCase("unknown")]
        [TestCase("\"string\"")]
        [TestCase("2435.2354")]
        [TestCase("-1.25e-10")]
        [TestCase("[]")]
        [TestCase("[ 1 ,\"aaa\" ]")]
        [TestCase("{}")]
        [TestCase("{ \"NullProp\" : null , \"StrProp\": \"strValue\", \"ArrProp\": [ 1 , null, \"aaa\" , [ ] , { } ] , \"ObjProp\" : { \"xxx\" : null, \"yyy\" : {}, \"zzz\": \"Str\" } }")]
        public void ParseTest(string raw)
        {
            JsonValue json = JsonValue.Parse(raw);
            string serialized = json.ToString();
            Console.WriteLine(serialized);
            Assert.AreEqual(json, JsonValue.Parse(serialized));
            Assert.IsTrue(JsonValue.TryParse(raw, out json));
            Assert.AreEqual(serialized, json.ToString());

            string formatted = json.ToString("\t");
            Console.WriteLine(formatted);
            Assert.AreEqual(json, JsonValue.Parse(formatted));
        }

        [Test]
        public void StringEscapesTest()
        {
            string json = @"""escapes:\r\n\""value\""  \/\/ \uD834\uDD1e\u0000""";
            Console.WriteLine($"Orig JSON: {json}");
            JsonValue value = JsonValue.Parse(json);
            Assert.AreEqual("escapes:\r\n\"value\"  // 𝄞\0", value.AsString);

            string serialized = value.ToString();
            Console.WriteLine($"New JSON: {serialized}");
            JsonValue deserialized = JsonValue.Parse(serialized);
            Assert.AreEqual(value, deserialized);

            Console.WriteLine($"Value: {value.AsString}");
            Assert.AreEqual(value.AsString, deserialized.AsString);
        }

        #endregion
    }
}
