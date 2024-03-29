﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: PerformanceTest.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2019 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#if DEBUG
using NUnit.Framework;
#endif

namespace KGySoft.Json
{
    internal class PerformanceTest : KGySoft.Diagnostics.PerformanceTest
    {
        #region Methods

        protected override void OnInitialize()
        {
#if DEBUG
            Assert.Inconclusive("Run the performance test in Release Build");
#endif
            base.OnInitialize();
        }

        #endregion
    }

    internal class PerformanceTest<TResult> : KGySoft.Diagnostics.PerformanceTest<TResult>
    {
        #region Methods

        protected override void OnInitialize()
        {
#if DEBUG
            Assert.Inconclusive("Run the performance test in Release Build");
#endif
            base.OnInitialize();
        }

        #endregion
    }
}
