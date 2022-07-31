#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonObjectTest.cs
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
using System.Collections.Generic;
using System.Linq;

using KGySoft.CoreLibraries;

using NUnit.Framework;

#endregion

namespace KGySoft.Json.UnitTests
{
    [TestFixture]
    public class JsonObjectTest
    {
        #region Methods

        [Test]
        public void ImplicitConversionsTest()
        {
            JsonObject json = new()
            {
                new JsonProperty("name", "value"), // regular
                { "string", "string value" }, // from name and value
#if (NET47_OR_GREATER || NETSTANDARD || NETCOREAPP)
                ("int", 1), // from tuple
#else
                new("int", 1),
#endif
                new KeyValuePair<string, JsonValue>("bool", true) // from key-value pair
            };

            Assert.AreEqual(4, json.Count);
            json["number"] = 1.23;

            Assert.AreEqual(5, json.Count);
            Assert.IsTrue(json["unknown"].IsUndefined);

            // as JsonValue
            JsonValue value = json;
            Assert.AreEqual(JsonValueType.Object, value.Type);
            Assert.AreEqual(json["x"], value["x"]);
            Assert.AreEqual(json["x"], value.AsObject!["x"]);
            Assert.IsTrue(value["y"].IsUndefined);
            Assert.IsTrue(value[0].IsUndefined);
        }

        [Test]
        public void ToStringTest()
        {
            Assert.AreEqual("{}", new JsonObject().ToString());

            JsonObject obj = new()
            {
                new("undefined", default),
                new("null", JsonValue.Null),
                new("bool", true),
                new("number", 1.23),
                new("string", "value"),
                new("array", new JsonArray { true, 1, "value" }),
                new("nestedObject", new JsonObject
                {
                    new ("propName", "propValue")
                })
            };

            // just like in JavaScript, serialized JSON string leaves out properties with undefined values
            string serialized = obj.ToString();
            Console.WriteLine(serialized);
            Assert.AreEqual(7, obj.Count);
            obj.RemoveAt(0);
            Assert.AreEqual(6, obj.Count);
            Assert.AreEqual(serialized, obj.ToString());
        }

        [Test]
        public void IndexerReadUnknownPropertyTest()
        {
            // just like in JavaScript, nonexistent property returns undefined
            Assert.IsTrue(new JsonObject()["unknown"].IsUndefined);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void CreateFromListTest(bool allowDuplicates)
        {
            var list = new List<JsonProperty>
            {
                new ("bool", true),
                new ("int", 1),
                new ("int", 2),
            };

            var obj = new JsonObject(list, allowDuplicates);
            Assert.AreEqual(allowDuplicates ? 3 : 2, obj.Count);
            Assert.AreEqual(2, obj["int"].AsNumber);

            obj.EnsureUniqueKeys();
            Assert.AreEqual(2, obj.Count);
            Assert.AreEqual(2, obj["int"].AsNumber);
        }

        [Test]
        public void CreateFromDictionaryTest()
        {
            var dict = new Dictionary<string, JsonValue>
            {
                { "bool", true },
                { "int", 1 },
            };

            var obj = new JsonObject(dict);
            Assert.AreEqual(2, obj.Count);
            Assert.AreEqual(1, obj["int"].AsNumber);
        }

        [Test]
        public void CollectionOperationsTest()
        {
            JsonObject obj = new() { { "bool", true } };
            Assert.AreEqual(1, obj.Count);

            // Adding some more elements, including a duplicate
            obj.Insert(0, new("int", 1));
            obj.Add(new("string", "orig value")); // as list (item is a JsonProperty)
            obj.Add("string", "new value"); // as dictionary (item is a key-value pair)
            Assert.AreEqual(4, obj.Count);

            // Keys returns distinct values
            Assert.AreEqual(3, obj.Keys.Count);
            CollectionAssert.AreEqual(obj.Entries.Select(p => p.Name).Distinct(), obj.Keys);

            // But Values returns all values
            Assert.AreEqual(4, obj.Values.Count);
            CollectionAssert.AreEqual(obj.Entries.Select(p => p.Value), obj.Values);

            // Removing duplicates
            obj.EnsureUniqueKeys();
            Assert.AreEqual(3, obj.Count);
            Assert.AreEqual(3, obj.Values.Count);
            Assert.IsFalse(obj.Values.Contains("orig value"));
            Assert.IsTrue(obj.Values.Contains("new value"));

            // accessing by index
            Assert.AreEqual(JsonValueType.Number, obj[0].Value.Type);
            Assert.AreEqual(1, obj[0].Value.AsNumber);
            Assert.IsTrue(obj[0].Value == 1);

            // accessing by name
            Assert.AreEqual(JsonValueType.Number, obj["int"].Type);
            Assert.AreEqual(1, obj["int"].AsNumber);
            Assert.IsTrue(obj["int"] == 1);
            Assert.AreEqual(JsonValueType.Number, obj["int".AsSegment()].Type);
            Assert.AreEqual(1, obj["int".AsSegment()].AsNumber);
            Assert.IsTrue(obj["int".AsSegment()] == 1);
#if NETCOREAPP2_1_OR_GREATER
            Assert.AreEqual(JsonValueType.Number, obj["int".AsSpan()].Type);
            Assert.AreEqual(1, obj["int".AsSpan()].AsNumber);
            Assert.IsTrue(obj["int".AsSpan()] == 1);
#endif

            var asList = (IList<JsonProperty>)obj;
            var asDict = (IDictionary<string, JsonValue>)obj;

            // Contains
            Assert.IsTrue(obj.Contains("int"));
            Assert.IsTrue(asList.Contains(new("int", 1)));
            Assert.IsTrue(asDict.Contains(new("int", 1)));
            Assert.IsTrue(asDict.ContainsKey("int"));

            // IndexOf
            Assert.AreEqual(0, obj.IndexOf("int"));
            Assert.AreEqual(0, asList.IndexOf(new("int", 1)));

            // CopyTo
            var properties = new JsonProperty[obj.Count];
            obj.CopyTo(properties, 0);
            Assert.IsTrue(obj.SequenceEqual(properties));

            // set by index
            obj[0] = new("replaced", "replaced value");
            Assert.AreEqual(3, obj.Count);
            Assert.AreEqual(new JsonProperty("replaced", "replaced value"), obj[0]);
            Assert.AreEqual(-1, obj.IndexOf("int"));
            Assert.AreEqual(0, obj.IndexOf("replaced"));

            // set by name
            obj["replaced"] = (string?)null;
            Assert.AreEqual(3, obj.Count);
            Assert.IsTrue(obj["replaced"].IsNull);
            Assert.AreEqual(0, obj.IndexOf("replaced"));

            // add by name
            obj["new"] = 42;
            Assert.AreEqual(4, obj.Count);
            Assert.AreEqual(42, obj["new"].AsNumber);
            Assert.AreEqual(obj.Count - 1, obj.IndexOf("new"));

            // remove
            obj.RemoveAt(0);
            Assert.AreEqual(3, obj.Count);
            Assert.IsFalse(obj.Remove("unknown"));
            Assert.IsTrue(obj.Remove("string"));
            Assert.AreEqual(2, obj.Count);
            Assert.IsFalse(asDict.Remove(new KeyValuePair<string, JsonValue>("new", 0)));
            Assert.IsTrue(asDict.Remove(new KeyValuePair<string, JsonValue>("new", 42)));
            Assert.AreEqual(1, obj.Count);

            // clear
            obj.Clear();
            Assert.AreEqual(0, obj.Count);
        }

        [TestCase("undefined", false)]
        [TestCase("null", false)]
        [TestCase("true", false)]
        [TestCase("false", false)]
        [TestCase("unknown", false)]
        [TestCase("\"string\"", false)]
        [TestCase("2435.2354", false)]
        [TestCase("-1.25e-10", false)]
        [TestCase("[]", false)]
        [TestCase("[ 1 ,\"string\" ]", false)]
        [TestCase("{}", true)]
        [TestCase("{ \"NullProp\" : null , \"StrProp\": \"strValue\", \"ArrProp\": [ 1 , null, \"aaa\" , [ ] , { } ] , \"ObjProp\" : { \"xxx\" : null, \"yyy\" : {}, \"zzz\": \"Str\" } }", true)]
        public void ParseTest(string json, bool success)
        {
            Assert.AreEqual(success, JsonObject.TryParse(json, out JsonObject? obj));
            if (!success)
            {
                Assert.IsNull(obj);
                Assert.Throws<ArgumentException>(() => JsonObject.Parse(json));
                return;
            }

            Assert.IsNotNull(obj);
            string serialized = obj!.ToString();
            JsonObject deserialized = JsonObject.Parse(serialized);
            Assert.AreEqual(obj, deserialized);
            Assert.AreEqual(serialized, deserialized.ToString());

            string formatted = obj.ToString("\t");
            Console.WriteLine(formatted);
            Assert.AreEqual(deserialized, JsonObject.Parse(formatted));
        }

        #endregion
    }
}