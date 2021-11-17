#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ReadDomTest.cs
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
using System.Text.Json;
#if NET6_0_OR_GREATER
using System.Text.Json.Nodes;
#endif

using Newtonsoft.Json.Linq;

using NUnit.Framework;

#endregion

namespace KGySoft.Json.PerformanceTests
{
    [TestFixture]
    public class ReadDomTest
    {
        #region Methods

        [Test]
        public void AccessSmallObjectTest()
        {
            JObject newtonsoft = JObject.Parse(TestData.SmallObject);
            JsonDocument system = JsonDocument.Parse(TestData.SmallObject);
            JsonObject kgySoft = JsonObject.Parse(TestData.SmallObject);
#if NET6_0_OR_GREATER
            JsonNode systemNodes = JsonNode.Parse(TestData.SmallObject)!;
#endif

            new PerformanceTest<string>
                {
                    TestName = nameof(AccessSmallObjectTest),
                    TestTime = 500
                }
                .AddCase(() => (string)((JValue)newtonsoft["StrProp"]!).Value!, "Newtonsoft.Json")
                .AddCase(() => system.RootElement.GetProperty("StrProp").GetString()!, "System.Text.Json")
                .AddCase(() => kgySoft["StrProp"].AsString!, "KGySoft.Json")
#if NET6_0_OR_GREATER
                .AddCase(() => systemNodes["StrProp"]!.GetValue<string>()!, "System.Text.Json.Nodes")
#endif
                .DoTest()
                .DumpResults(Console.Out);
        }

        [Test]
        public void AccessLargeObjectTest()
        {
            JObject newtonsoft = JObject.Parse(TestData.LargeObject);
            JsonDocument system = JsonDocument.Parse(TestData.LargeObject);
            JsonObject kgySoft = JsonObject.Parse(TestData.LargeObject);
#if NET6_0_OR_GREATER
            JsonNode systemNodes = JsonNode.Parse(TestData.LargeObject)!;
#endif

            new PerformanceTest<string>
                {
                    TestName = nameof(AccessLargeObjectTest),
                    TestTime = 500
                }
                .AddCase(() => (string)((JValue)newtonsoft["data"]![0]!["details"]![1]!["uTime"]!).Value!, "Newtonsoft.Json")
                .AddCase(() => system.RootElement.GetProperty("data")[0].GetProperty("details")[1].GetProperty("uTime").GetString()!, "System.Text.Json")
                .AddCase(() => kgySoft["data"][0]["details"][1]["uTime"].AsString!, "KGySoft.Json")
#if NET6_0_OR_GREATER
                .AddCase(() => systemNodes["data"]![0]!["details"]![1]!["uTime"]!.GetValue<string>(), "System.Text.Json.Nodes")
#endif
                .DoTest()
                .DumpResults(Console.Out);
        }

        #endregion
    }
}
