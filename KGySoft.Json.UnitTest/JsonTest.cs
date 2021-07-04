#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonTest.cs
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

using NUnit.Framework;

#endregion

namespace KGySoft.Json.UnitTest
{
    [TestFixture]
    public class JsonTest
    {
        #region Methods

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
            var json = JsonValue.Parse(raw);
            string serialized = json.ToString();
            Console.WriteLine(serialized);
            Assert.AreEqual(json, JsonValue.Parse(serialized));
        }

        [Test]
        public void BuildTest()
        {
            JsonValue json = new JsonObject { { "string", "b" }, ("int", 1) };
            Assert.AreEqual(JsonValueType.Object, json.Type);
            Assert.AreEqual(2, json.AsObject.Count);

            json.AsObject["x"] = JsonValue.Null;
            Assert.AreEqual(3, json.AsObject.Count);

            Assert.AreEqual(json["x"], json.AsObject["x"]);
            Assert.IsTrue(json["y"].IsUndefined);
            Assert.IsTrue(json.AsObject["y"].IsUndefined);
            Assert.IsTrue(json[0].IsUndefined);
        }

        #endregion
    }
}
