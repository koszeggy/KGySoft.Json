#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: ParseTest.cs
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
using System.IO;
using System.Text;
using System.Text.Json;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using NUnit.Framework;

#endregion

namespace KGySoft.Json.PerformanceTest
{
    [TestFixture]
    public class ParseTest
    {
        #region Methods

        [TestCase("Small", TestData.SmallObject)]
        [TestCase("Medium", TestData.MediumObject)]
        [TestCase("Large", TestData.LargeObject)]
        public void FromString(string name, string json) => new Diagnostics.PerformanceTest
            {
                TestName = name,
                TestTime = 500
            }
            .AddCase(() => { JToken.Parse(json); }, "Newtonsoft.Json")
            .AddCase(() => { JsonDocument.Parse(json); }, "System.Text.Json")
            .AddCase(() => { JsonValue.Parse(json); }, "KGySoft.Json")
            .DoTest()
            .DumpResults(Console.Out);

        [TestCase("Small", TestData.SmallObject)]
        [TestCase("Medium", TestData.MediumObject)]
        [TestCase("Large", TestData.LargeObject)]
        public void FromUtf8Stream(string name, string json)
        {
            var utf8 = new MemoryStream(Encoding.UTF8.GetBytes(json));
            new Diagnostics.PerformanceTest
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
                .DoTest()
                .DumpResults(Console.Out);
        }

        #endregion
    }
}
