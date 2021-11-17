#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ParseTest.cs
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
using System.IO;
using System.Text;
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
    public class ParseTest
    {
        #region Methods

        [TestCase("Small", TestData.SmallObject)]
        [TestCase("Medium", TestData.MediumObject)]
        [TestCase("Large", TestData.LargeObject)]
        public void FromString(string name, string json) => new PerformanceTest
            {
                TestName = name,
                TestTime = 500,
            }
            .AddCase(() => { JToken.Parse(json); }, "Newtonsoft.Json")
            .AddCase(() => { JsonDocument.Parse(json); }, "System.Text.Json")
            .AddCase(() => { JsonValue.Parse(json); }, "KGySoft.Json")
#if NET6_0_OR_GREATER
            .AddCase(() => { JsonNode.Parse(json); }, "System.Text.Json.Nodes")
#endif
            .DoTest()
            .DumpResults(Console.Out);

        [TestCase("Small", TestData.SmallObject)]
        [TestCase("Medium", TestData.MediumObject)]
        [TestCase("Large", TestData.LargeObject)]
        public void FromUtf8Stream(string name, string json)
        {
            var utf8 = new MemoryStream(Encoding.UTF8.GetBytes(json));
            new PerformanceTest
                {
                    TestName = name,
                    TestTime = 500
                }
                .AddCase(() =>
                {
                    utf8.Position = 0L;
                    using var reader = new JsonTextReader(new StreamReader(utf8, leaveOpen:true));
                    JToken.ReadFrom(reader);
                }, "Newtonsoft.Json")
                .AddCase(() =>
                {
                    utf8.Position = 0L;
                    JsonDocument.Parse(utf8);
                }, "System.Text.Json")
                .AddCase(() =>
                {
                    utf8.Position = 0L;
                    JsonValue.Parse(utf8);
                }, "KGySoft.Json")
#if NET6_0_OR_GREATER
                        .AddCase(() =>
                {
                    utf8.Position = 0L;
                    JsonNode.Parse(utf8);
                }, "System.Text.Json.Nodes")
#endif
                .DoTest()
                .DumpResults(Console.Out);
        }

        #endregion
    }
}
