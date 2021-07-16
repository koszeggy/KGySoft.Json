#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: TestData.cs
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

namespace KGySoft.Json.PerformanceTest
{
    internal static class TestData
    {
        #region Constants

        internal const string SmallObject = "{ \"NullProp\" : null , \"StrProp\": \"strValue\" }";
        internal const string MediumObject = "{ \"NullProp\" : null , \"StrProp\": \"strValue\", \"ArrProp\": [ 1 , null, \"aaa\" , [ ] , { } ] , \"ObjProp\" : { \"xxx\" : null, \"yyy\" : {}, \"zzz\": \"Str\" } }";
        internal const string LargeObject = "{\r\n    \"code\": \"0\",\r\n    \"data\": [\r\n        {\r\n            \"adjEq\": \"10679688.0460531643092577\",\r\n            \"details\": [\r\n                {\r\n                    \"availBal\": \"\",\r\n                    \"availEq\": \"9930359.9998\",\r\n                    \"cashBal\": \"9930359.9998\",\r\n                    \"ccy\": \"USDT\",\r\n                    \"crossLiab\": \"0\",\r\n                    \"disEq\": \"9439737.0772999514\",\r\n                    \"eq\": \"9930359.9998\",\r\n                    \"eqUsd\": \"9933041.196999946\",\r\n                    \"frozenBal\": \"0\",\r\n                    \"interest\": \"0\",\r\n                    \"isoEq\": \"0\",\r\n                    \"isoLiab\": \"0\",\r\n                    \"liab\": \"0\",\r\n                    \"maxLoan\": \"10000\",\r\n                    \"mgnRatio\": \"\",\r\n                    \"notionalLever\": \"\",\r\n                    \"ordFrozen\": \"0\",\r\n                    \"twap\": \"0\",\r\n                    \"uTime\": \"1620722938250\",\r\n                    \"upl\": \"0\",\r\n                    \"uplLiab\": \"0\"\r\n                },\r\n                {\r\n                    \"availBal\": \"\",\r\n                    \"availEq\": \"33.6799714158199414\",\r\n                    \"cashBal\": \"33.2009985\",\r\n                    \"ccy\": \"BTC\",\r\n                    \"crossLiab\": \"0\",\r\n                    \"disEq\": \"1239950.9687532129092577\",\r\n                    \"eq\": \"33.771820625136023\",\r\n                    \"eqUsd\": \"1239950.9687532129092577\",\r\n                    \"frozenBal\": \"0.0918492093160816\",\r\n                    \"interest\": \"0\",\r\n                    \"isoEq\": \"0\",\r\n                    \"isoLiab\": \"0\",\r\n                    \"liab\": \"0\",\r\n                    \"maxLoan\": \"1453.92289531493594\",\r\n                    \"mgnRatio\": \"\",\r\n                    \"notionalLever\": \"\",\r\n                    \"ordFrozen\": \"0\",\r\n                    \"twap\": \"0\",\r\n                    \"uTime\": \"1620722938250\",\r\n                    \"upl\": \"0.570822125136023\",\r\n                    \"uplLiab\": \"0\"\r\n                }\r\n            ],\r\n            \"imr\": \"3372.2942371050594217\",\r\n            \"isoEq\": \"0\",\r\n            \"mgnRatio\": \"70375.35408747017\",\r\n            \"mmr\": \"134.8917694842024\",\r\n            \"notionalUsd\": \"33722.9423710505978888\",\r\n            \"ordFroz\": \"0\",\r\n            \"totalEq\": \"11172992.1657531589092577\",\r\n            \"uTime\": \"1623392334718\"\r\n        }\r\n    ],\r\n    \"msg\": \"\"\r\n}";

        #endregion
    }
}
