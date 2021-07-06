#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Throw.cs
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
using System.Diagnostics.CodeAnalysis;

#endregion

namespace KGySoft
{
    internal static class Throw
    {
        #region Methods

        #region Internal Methods

        [DoesNotReturn]internal static void ArgumentNullException(string paramName) => throw CreateArgumentNullException(paramName);
        [DoesNotReturn]internal static T ArgumentNullException<T>(string paramName) => throw CreateArgumentNullException(paramName);

        [DoesNotReturn]internal static void ArgumentException(string message, string paramName) => throw CreateArgumentException(message, paramName);
        [DoesNotReturn]internal static T ArgumentException<T>(string message, string paramName) => throw CreateArgumentException(message, paramName);

        #endregion

        #region Private Methods

        private static ArgumentNullException CreateArgumentNullException(string paramName) => new ArgumentNullException(paramName);
        private static ArgumentException CreateArgumentException(string message, string paramName) => new ArgumentException(message, paramName);

        #endregion

        #endregion
    }
}