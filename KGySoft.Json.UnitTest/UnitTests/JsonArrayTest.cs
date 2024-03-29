﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonArrayTest.cs
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

namespace KGySoft.Json.UnitTests
{
    [TestFixture]
    public class JsonArrayTest
    {
        #region Methods

        [Test]
        public void ImplicitConversionsTest()
        {
            JsonArray array = new() { default, true, 1, 1.23, "value", (string?)null };
            Assert.AreEqual(6, array.Length);
            Assert.IsTrue(array[0].IsUndefined);
            Assert.IsTrue(array[1] == true);
            Assert.IsTrue(array[2] == 1);
            Assert.IsTrue(array[3] == 1.23);
            Assert.IsTrue(array[4] == "value");
            Assert.IsTrue(array[5].IsNull);
        }

        [Test]
        public void ToStringTest()
        {
            Assert.AreEqual("[]", new JsonArray().ToString());

            JsonArray array = new() { default, true, 1, 1.23, "value", (string?)null };

            // just like in JavaScript, serialized JSON string replaces undefined values with null
            string serialized = array.ToString();
            Console.WriteLine(serialized);
            JsonArray deserialized = JsonArray.Parse(serialized);
            Assert.AreEqual(array.Length, deserialized.Length);
            Assert.IsTrue(deserialized[0].IsNull);
            CollectionAssert.AreEqual(array.Skip(1), deserialized.Skip(1));
        }

        [Test]
        public void IndexerReadOutOfBoundsTest()
        {
            // just like in JavaScript, out-of-bounds index returns undefined
            Assert.IsTrue(new JsonArray()[42].IsUndefined);
        }

        [Test]
        public void ListTest()
        {
            JsonArray array = new() { true };
            Assert.AreEqual(1, array.Length);

            array.Insert(0, 1);
            array.Add("string");
            Assert.AreEqual(3, array.Length);
            Assert.AreEqual(JsonValueType.Number, array[0].Type);
            Assert.AreEqual(1, array[0].AsNumber);
            Assert.IsTrue(array[0] == 1);

            Assert.IsTrue(array.Contains(1));
            Assert.AreEqual(0, array.IndexOf(1));
            Assert.AreEqual(1, array.IndexOf(true));
            Assert.AreEqual(2, array.IndexOf("string"));

            var values = new JsonValue[array.Length];
            array.CopyTo(values, 0);
            Assert.IsTrue(array.SequenceEqual(values));

            array.RemoveAt(0);
            Assert.AreEqual(2, array.Length);

            Assert.IsFalse(array.Remove(42));
            Assert.IsTrue(array.Remove("string"));
            Assert.AreEqual(1, array.Length);

            array.Clear();
            Assert.AreEqual(0, array.Length);
        }

        [TestCase("undefined", false)]
        [TestCase("null", false)]
        [TestCase("true", false)]
        [TestCase("false", false)]
        [TestCase("unknown", false)]
        [TestCase("\"string\"", false)]
        [TestCase("2435.2354", false)]
        [TestCase("-1.25e-10", false)]
        [TestCase("[]", true)]
        [TestCase("[ 1 ,\"string\" ]", true)]
        [TestCase("[ 1 ,\"string\", [42, true], { \"NullProp\" : null , \"StrProp\": \"strValue\" } ]", true)]
        [TestCase("{}", false)]
        [TestCase("{ \"NullProp\" : null , \"StrProp\": \"strValue\" }", false)]
        public void ParseTest(string json, bool success)
        {
            Assert.AreEqual(success, JsonArray.TryParse(json, out JsonArray? array));
            if (!success)
            {
                Assert.IsNull(array);
                Assert.Throws<ArgumentException>(() => JsonArray.Parse(json));
                return;
            }

            Assert.IsNotNull(array);
            string serialized = array!.ToString();
            JsonArray deserialized = JsonArray.Parse(serialized);
            Assert.AreEqual(array, deserialized);
            Assert.AreEqual(serialized, deserialized.ToString());

            string formatted = array.ToString("\t");
            Console.WriteLine(formatted);
            Assert.AreEqual(deserialized, JsonArray.Parse(formatted));
        }

        #endregion
    }
}