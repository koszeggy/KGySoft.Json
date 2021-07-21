#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Throw.cs
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
using System.Diagnostics.CodeAnalysis;

#endregion

namespace KGySoft
{
    internal static class Throw
    {
        #region Methods

        [DoesNotReturn]internal static void ArgumentNullException(string paramName) => throw new ArgumentNullException(paramName);
        [DoesNotReturn]internal static T ArgumentNullException<T>(string paramName) => throw new ArgumentNullException(paramName);

        [DoesNotReturn]internal static void ArgumentException(string message, string paramName) => throw new ArgumentException(message, paramName);
        [DoesNotReturn]internal static T ArgumentException<T>(string message, string paramName) => throw new ArgumentException(message, paramName);

        [DoesNotReturn]internal static T InvalidCastException<T>() => throw new InvalidCastException();

        [DoesNotReturn]internal static void ArgumentOutOfRangeException(string paramName) => throw new ArgumentOutOfRangeException(paramName);

        #endregion
    }
}