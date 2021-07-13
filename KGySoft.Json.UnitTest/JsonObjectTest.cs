#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonObjectTest.cs
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
    public class JsonObjectTest
    {
        #region Methods

        [Test]
        public void BuildTest()
        {
            var json = new JsonObject { { "string", "b" }, ("int", 1) };
            Assert.AreEqual(2, json.Count);

            json["x"] = JsonValue.Null;
            Assert.AreEqual(3, json.Count);
            Assert.IsTrue(json["y"].IsUndefined);

            JsonValue value = json;
            Assert.AreEqual(JsonValueType.Object, value.Type);
            Assert.AreEqual(json["x"], value["x"]);
            Assert.AreEqual(json["x"], value.AsObject!["x"]);
            Assert.IsTrue(value["y"].IsUndefined);
            Assert.IsTrue(value[0].IsUndefined);
        }

        #endregion
    }
}