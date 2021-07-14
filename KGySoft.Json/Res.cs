#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: Res.cs
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
using System.Globalization;
using System.Resources;
using System.Threading;

#endregion

namespace KGySoft
{
    internal static class Res
    {
        #region Constants

        private const string unavailableResource = "Resource ID not found: {0}";
        private const string invalidResource = "Resource text is not valid for {0} arguments: {1}";
        private const string nullRef = "<null>";

        #endregion

        #region Fields

        private static readonly ResourceManager resourceManager = new ResourceManager("KGySoft.Json.Messages", typeof(Res).Assembly);

        #endregion

        #region Properties

        /// <summary>A default JsonProperty value is invalid here.</summary>
        internal static string DefaultJsonPropertyInvalid => Get(nameof(DefaultJsonPropertyInvalid));

        /// <summary>Unexpected end of JSON stream.</summary>
        internal static string UnexpectedEndOfJsonStream => Get(nameof(UnexpectedEndOfJsonStream));

        /// <summary>Unexpected end of JSON string.</summary>
        internal static string UnexpectedEndOfJsonString => Get(nameof(UnexpectedEndOfJsonString));

        /// <summary>Unexpected end of JSON array.</summary>
        internal static string UnexpectedEndOfJsonArray => Get(nameof(UnexpectedEndOfJsonArray));

        /// <summary>Unexpected comma in JSON array.</summary>
        internal static string UnexpectedCommaInJsonArray => Get(nameof(UnexpectedCommaInJsonArray));

        /// <summary>Unexpected end of JSON object.</summary>
        internal static string UnexpectedEndOfJsonObject => Get(nameof(UnexpectedEndOfJsonObject));

        /// <summary>Unexpected comma in JSON object.</summary>
        internal static string UnexpectedCommaInJsonObject => Get(nameof(UnexpectedCommaInJsonObject));

        /// <summary>Unexpected colon in JSON object.</summary>
        internal static string UnexpectedColonInJsonObject => Get(nameof(UnexpectedColonInJsonObject));

        /// <summary>Missing comma between properties in JSON object.</summary>
        internal static string MissingCommaInJsonObject => Get(nameof(MissingCommaInJsonObject));

        #endregion

        #region Methods

        #region Internal Methods

        /// <summary>Unexpected character in JSON value: '{0}'</summary>
        internal static string UnexpectedCharInJsonValue(char c) => Get("UnexpectedCharInJsonValue_Format", c);

        /// <summary>Unexpected character in JSON literal: '{0}'</summary>
        internal static string UnexpectedCharInJsonLiteral(char c) => Get("UnexpectedCharInJsonLiteral_Format", c);

        /// <summary>Unexpected character in JSON array: '{0}'</summary>
        internal static string UnexpectedCharInJsonArray(char c) => Get("UnexpectedCharInJsonArray_Format", c);

        /// <summary>Unexpected character in JSON object: '{0}'</summary>
        internal static string UnexpectedCharInJsonObject(char c) => Get("UnexpectedCharInJsonObject_Format", c);

        #endregion

        #region Private Methods

        private static string Get(string id) => resourceManager.GetString(id, Thread.CurrentThread.CurrentUICulture) ?? String.Format(CultureInfo.InvariantCulture, unavailableResource, id);

        private static string Get(string id, params object?[]? args)
        {
            string format = Get(id);
            return args == null ? format : SafeFormat(format, args);
        }

        private static string SafeFormat(string format, object?[] args)
        {
            try
            {
                int i = Array.IndexOf(args, null);
                if (i >= 0)
                {
                    for (; i < args.Length; i++)
                        args[i] ??= nullRef;
                }

                return String.Format(Thread.CurrentThread.CurrentCulture, format, args);
            }
            catch (FormatException)
            {
                return String.Format(CultureInfo.InvariantCulture, invalidResource, args.Length, format);
            }
        }

        #endregion

        #endregion
    }
}
