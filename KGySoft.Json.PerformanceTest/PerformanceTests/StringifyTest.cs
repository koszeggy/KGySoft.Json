﻿#region Copyright

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

            new PerformanceTest<string>
                {
                    TestName = name,
                    TestTime = 500
                }
                .AddCase(() => System.Text.Json.JsonSerializer.Serialize(system), "System.Text.Json")
                .AddCase(() => newtonsoft.ToString(Formatting.None), "Newtonsoft.Json")
                .AddCase(() => kgySoft.ToString(), "KGySoft.Json")
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

            new PerformanceTest<string>
                {
                    TestName = name,
                    TestTime = 500
                }
                .AddCase(() => System.Text.Json.JsonSerializer.Serialize(system, systemOption), "System.Text.Json")
                .AddCase(() => newtonsoft.ToString(Formatting.Indented), "Newtonsoft.Json")
                .AddCase(() => kgySoft.ToString("  "), "KGySoft.Json")
                .DoTest()
                .DumpResults(Console.Out);
        }

        #endregion
    }
}