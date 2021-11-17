#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: StringifyTest.cs
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NUnit.Framework;

#endregion

namespace KGySoft.Json.PerformanceTests
{
    [TestFixture]
    public class StringifyTest
    {
        #region Methods

        [TestCase("Small", TestData.SmallObject)]
        [TestCase("Medium", TestData.MediumObject)]
        [TestCase("Large", TestData.LargeObject)]
        public void ToMinimizedString(string name, string json)
        {
            JObject newtonsoft = JObject.Parse(json);
            JsonDocument system = JsonDocument.Parse(json);
            JsonObject kgySoft = JsonObject.Parse(json);
#if NET6_0_OR_GREATER
            JsonNode systemNodes = JsonNode.Parse(json)!;
#endif

            new PerformanceTest<string>
                {
                    TestName = name,
                    TestTime = 500
                }
                .AddCase(() => System.Text.Json.JsonSerializer.Serialize(system), "System.Text.Json")
                .AddCase(() => newtonsoft.ToString(Formatting.None), "Newtonsoft.Json")
                .AddCase(() => kgySoft.ToString(), "KGySoft.Json")
#if NET6_0_OR_GREATER
                .AddCase(() => systemNodes.ToJsonString(), "System.Text.Json.Nodes")
#endif
                .DoTest()
                .DumpResults(Console.Out);
        }

        [TestCase("Small", TestData.SmallObject)]
        [TestCase("Medium", TestData.MediumObject)]
        [TestCase("Large", TestData.LargeObject)]
        public void ToFormattedString(string name, string json)
        {
            JObject newtonsoft = JObject.Parse(json);
            JsonDocument system = JsonDocument.Parse(json);
            JsonObject kgySoft = JsonObject.Parse(json);
            var systemOption = new JsonSerializerOptions { WriteIndented = true };
#if NET6_0_OR_GREATER
            JsonNode systemNodes = JsonNode.Parse(json)!;
#endif

            new PerformanceTest<string>
                {
                    TestName = name,
                    TestTime = 500
                }
                .AddCase(() => System.Text.Json.JsonSerializer.Serialize(system, systemOption), "System.Text.Json")
                .AddCase(() => newtonsoft.ToString(Formatting.Indented), "Newtonsoft.Json")
                .AddCase(() => kgySoft.ToString("  "), "KGySoft.Json")
#if NET6_0_OR_GREATER
                .AddCase(() => systemNodes.ToJsonString(systemOption), "System.Text.Json.Nodes")
#endif
                .DoTest()
                .DumpResults(Console.Out);
        }

        #endregion
    }
}
