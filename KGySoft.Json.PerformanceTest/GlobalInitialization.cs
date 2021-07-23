#if !NETFRAMEWORK
#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: GlobalInitialization.cs
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

using NUnit.Framework;

#endregion

namespace KGySoft.Json.PerformanceTest
{
    [SetUpFixture]
    public class GlobalInitialization
    {
        #region Methods

#if DEBUG
        [OneTimeSetUp]
        public void Initialize() => Assert.Inconclusive("The performance tests should be executed with the Release build."); 
#endif

        #endregion
    }
}
#endif
