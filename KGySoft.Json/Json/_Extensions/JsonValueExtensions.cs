﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonValueExtensions.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2022 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
#if !NET35
using System.Numerics;
#endif
using System.Text;

using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Json
{
    /// <summary>
    /// Provides extension methods for <see cref="JsonValue"/> conversions.
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "False alarm, for some reason ReSharper triggers non_field_members_should_be_pascal_case for all As* members")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "ReSharper issue")]
    public static class JsonValueExtensions
    {
        #region Methods

        #region Boolean

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="bool"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="bool"/>;
        /// otherwise, returns <see langword="null"/>.
        /// This method allows interpreting numeric values as booleans where nonzero values are <see langword="true"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="bool"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="bool"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static bool? AsBoolean(this JsonValue json, JsonValueType expectedType = default)
        {
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || json.AsStringInternal is not string s)
                return null;

            return s switch
            {
                JsonValue.TrueLiteral => true,
                JsonValue.FalseLiteral => false,
                "1" => true,
                "0" => false,
                "" => null,
                _ => AsDouble(json, expectedType) switch
                {
                    null => null,
                    0d => false,
                    _ => true
                }
            };
        }

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="bool"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// This method allows interpreting numeric values as booleans where nonzero values are <see langword="true"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="bool"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see langword="false"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetBoolean(this JsonValue json, out bool value, JsonValueType expectedType = default)
        {
            bool? result = json.AsBoolean(expectedType);
            value = result ?? default;
            return result.HasValue;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="bool"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="bool"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// This method allows interpreting numeric values as booleans where nonzero values are <see langword="true"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="bool"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="bool"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static bool GetBooleanOrDefault(this JsonValue json, bool defaultValue = default, JsonValueType expectedType = default)
            => json.AsBoolean(expectedType) ?? defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="bool"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="bool"/>;
        /// otherwise, returns <see langword="false"/>, which is the default value of <see cref="bool"/>.
        /// This method allows interpreting numeric values as booleans where nonzero values are <see langword="true"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="bool"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type..</param>
        /// <returns>A <see cref="bool"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool GetBooleanOrDefault(this JsonValue json, JsonValueType expectedType)
            => json.AsBoolean(expectedType) ?? default;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this bool value) => new JsonValue(value);

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this bool? value) => value?.ToJson() ?? JsonValue.Null;

        #endregion

        #region Byte

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="byte"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="byte"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetByte(this JsonValue json, out byte value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return Byte.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="byte"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="byte"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="byte"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="byte"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static byte? AsByte(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetByte(out byte result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="byte"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="byte"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="byte"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="byte"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static byte GetByteOrDefault(this JsonValue json, byte defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetByte(out byte result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="byte"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="byte"/>;
        /// otherwise, returns <c>0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="byte"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="byte"/> value if <paramref name="json"/> could be converted; otherwise, <c>0</c>.</returns>
        public static byte GetByteOrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetByte(out byte result, expectedType) ? result : (byte)0;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this byte value, bool asString = false)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString(NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this byte? value, bool asString = false) => value?.ToJson(asString) ?? JsonValue.Null;

        #endregion

        #region SByte

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as an <see cref="sbyte"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="sbyte"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        [CLSCompliant(false)]
        public static bool TryGetSByte(this JsonValue json, out sbyte value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return SByte.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="sbyte"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="sbyte"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="sbyte"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="sbyte"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        [CLSCompliant(false)]
        public static sbyte? AsSByte(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetSByte(out sbyte result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="sbyte"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="sbyte"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="sbyte"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="sbyte"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        [CLSCompliant(false)]
        public static sbyte GetSByteOrDefault(this JsonValue json, sbyte defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetSByte(out sbyte result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="sbyte"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="sbyte"/>;
        /// otherwise, returns <c>0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="sbyte"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="sbyte"/> value if <paramref name="json"/> could be converted; otherwise, <c>0</c>.</returns>
        [CLSCompliant(false)]
        public static sbyte GetSByteOrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetSByte(out sbyte result, expectedType) ? result : (sbyte)0;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</returns>
        [CLSCompliant(false)]
        public static JsonValue ToJson(this sbyte value, bool asString = false)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString(NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</returns>
        [CLSCompliant(false)]
        public static JsonValue ToJson(this sbyte? value, bool asString = false) => value?.ToJson(asString) ?? JsonValue.Null;

        #endregion

        #region Int16

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as an <see cref="short"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="short"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetInt16(this JsonValue json, out short value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return Int16.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="short"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="short"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="short"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="short"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static short? AsInt16(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetInt16(out short result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="short"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="short"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="short"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="short"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static short GetInt16OrDefault(this JsonValue json, short defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetInt16(out short result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="short"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="short"/>;
        /// otherwise, returns <c>0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="short"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type..</param>
        /// <returns>An <see cref="short"/> value if <paramref name="json"/> could be converted; otherwise, <c>0</c>.</returns>
        public static short GetInt16OrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetInt16(out short result, expectedType) ? result : (short)0;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this short value, bool asString = false)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString(NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this short? value, bool asString = false) => value?.ToJson(asString) ?? JsonValue.Null;

        #endregion

        #region UInt16

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as an <see cref="ushort"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="ushort"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        [CLSCompliant(false)]
        public static bool TryGetUInt16(this JsonValue json, out ushort value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return UInt16.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="ushort"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="ushort"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="ushort"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="ushort"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        [CLSCompliant(false)]
        public static ushort? AsUInt16(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetUInt16(out ushort result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="ushort"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="ushort"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="ushort"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="ushort"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        [CLSCompliant(false)]
        public static ushort GetUInt16OrDefault(this JsonValue json, ushort defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetUInt16(out ushort result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="ushort"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="ushort"/>;
        /// otherwise, returns <c>0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="ushort"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="ushort"/> value if <paramref name="json"/> could be converted; otherwise, <c>0</c>.</returns>
        [CLSCompliant(false)]
        public static ushort GetUInt16OrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetUInt16(out ushort result, expectedType) ? result : (ushort)0;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        [CLSCompliant(false)]
        public static JsonValue ToJson(this ushort value, bool asString = false)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString(NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        [CLSCompliant(false)]
        public static JsonValue ToJson(this ushort? value, bool asString = false) => value?.ToJson(asString) ?? JsonValue.Null;

        #endregion

        #region Int32

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as an <see cref="int"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="int"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetInt32(this JsonValue json, out int value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return Int32.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="int"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="int"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="int"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="int"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static int? AsInt32(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetInt32(out int result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="int"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="int"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="int"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="int"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static int GetInt32OrDefault(this JsonValue json, int defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetInt32(out int result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="int"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="int"/>;
        /// otherwise, returns <c>0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="int"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>An <see cref="int"/> value if <paramref name="json"/> could be converted; otherwise, <c>0</c>.</returns>
        public static int GetInt32OrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetInt32(out int result, expectedType) ? result : 0;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this int value, bool asString = false)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString(NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this int? value, bool asString = false) => value?.ToJson(asString) ?? JsonValue.Null;

        #endregion

        #region UInt32

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as an <see cref="uint"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="uint"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        [CLSCompliant(false)]
        public static bool TryGetUInt32(this JsonValue json, out uint value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return UInt32.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="uint"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="uint"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="uint"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="uint"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        [CLSCompliant(false)]
        public static uint? AsUInt32(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetUInt32(out uint result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="uint"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="uint"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="uint"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="uint"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        [CLSCompliant(false)]
        public static uint GetUInt32OrDefault(this JsonValue json, uint defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetUInt32(out uint result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="uint"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="uint"/>;
        /// otherwise, returns <c>0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="uint"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>An <see cref="uint"/> value if <paramref name="json"/> could be converted; otherwise, <c>0</c>.</returns>
        [CLSCompliant(false)]
        public static uint GetUInt32OrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetUInt32(out uint result, expectedType) ? result : 0U;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        [CLSCompliant(false)]
        public static JsonValue ToJson(this uint value, bool asString = false)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString(NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        [CLSCompliant(false)]
        public static JsonValue ToJson(this uint? value, bool asString = false) => value?.ToJson(asString) ?? JsonValue.Null;

        #endregion

        #region Int64

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as an <see cref="long"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="long"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetInt64(this JsonValue json, out long value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return Int64.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="long"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="long"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="long"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="long"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static long? AsInt64(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetInt64(out long result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="long"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="long"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="long"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="long"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static long GetInt64OrDefault(this JsonValue json, long defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetInt64(out long result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="long"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="long"/>;
        /// otherwise, returns <c>0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="long"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>An <see cref="long"/> value if <paramref name="json"/> could be converted; otherwise, <c>0</c>.</returns>
        public static long GetInt64OrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetInt64(out long result, expectedType) ? result : 0L;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// To prevent losing precision the default value of the <paramref name="asString"/> parameter is <see langword="true"/> in this overload.
        /// <br/>See the <strong>Remarks</strong> section of the <see cref="JsonValue.AsNumber"/> property for details.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this long value, bool asString = true)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString(NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// To prevent losing precision the default value of the <paramref name="asString"/> parameter is <see langword="true"/> in this overload.
        /// <br/>See the <strong>Remarks</strong> section of the <see cref="JsonValue.AsNumber"/> property for details.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this long? value, bool asString = true) => value?.ToJson(asString) ?? JsonValue.Null;

        #endregion

        #region UInt64

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as an <see cref="ulong"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="ulong"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        [CLSCompliant(false)]
        public static bool TryGetUInt64(this JsonValue json, out ulong value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return UInt64.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="ulong"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="ulong"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="ulong"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="ulong"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        [CLSCompliant(false)]
        public static ulong? AsUInt64(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetUInt64(out ulong result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="ulong"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="ulong"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="ulong"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="ulong"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        [CLSCompliant(false)]
        public static ulong GetUInt64OrDefault(this JsonValue json, ulong defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetUInt64(out ulong result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="ulong"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="ulong"/>;
        /// otherwise, returns <c>0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="ulong"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>An <see cref="ulong"/> value if <paramref name="json"/> could be converted; otherwise, <c>0</c>.</returns>
        [CLSCompliant(false)]
        public static ulong GetUInt64OrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetUInt64(out ulong result, expectedType) ? result : 0UL;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// To prevent losing precision the default value of the <paramref name="asString"/> parameter is <see langword="true"/> in this overload.
        /// <br/>See the <strong>Remarks</strong> section of the <see cref="JsonValue.AsNumber"/> property for details.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        [CLSCompliant(false)]
        public static JsonValue ToJson(this ulong value, bool asString = true)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString(NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// To prevent losing precision the default value of the <paramref name="asString"/> parameter is <see langword="true"/> in this overload.
        /// <br/>See the <strong>Remarks</strong> section of the <see cref="JsonValue.AsNumber"/> property for details.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        [CLSCompliant(false)]
        public static JsonValue ToJson(this ulong? value, bool asString = true) => value?.ToJson(asString) ?? JsonValue.Null;

        #endregion

        #region BigInteger
#if !NET35

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="BigInteger"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="BigInteger"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetBigInteger(this JsonValue json, out BigInteger value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return BigInteger.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="BigInteger"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="BigInteger"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="BigInteger"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="BigInteger"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static BigInteger? AsBigInteger(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetBigInteger(out BigInteger result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="BigInteger"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="BigInteger"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="BigInteger"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="BigInteger"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static BigInteger GetBigIntegerOrDefault(this JsonValue json, BigInteger defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetBigInteger(out BigInteger result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="BigInteger"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="BigInteger"/>;
        /// otherwise, returns <c>0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="BigInteger"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="BigInteger"/> value if <paramref name="json"/> could be converted; otherwise, <c>0</c>.</returns>
        public static BigInteger GetBigIntegerOrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetBigInteger(out BigInteger result, expectedType) ? result : default;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// To prevent losing precision the default value of the <paramref name="asString"/> parameter is <see langword="true"/> in this overload.
        /// <br/>See the <strong>Remarks</strong> section of the <see cref="JsonValue.AsNumber"/> property for details.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this BigInteger value, bool asString = true)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString(NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// To prevent losing precision the default value of the <paramref name="asString"/> parameter is <see langword="true"/> in this overload.
        /// <br/>See the <strong>Remarks</strong> section of the <see cref="JsonValue.AsNumber"/> property for details.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this BigInteger? value, bool asString = true) => value?.ToJson(asString) ?? JsonValue.Null;

#endif
        #endregion

        #region Int128
#if NET7_0_OR_GREATER

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as an <see cref="Int128"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="Int128"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetInt128(this JsonValue json, out Int128 value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return Int128.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="Int128"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="Int128"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="Int128"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="Int128"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static Int128? AsInt128(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetInt128(out Int128 result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="Int128"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="Int128"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="Int128"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="Int128"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static Int128 GetInt128OrDefault(this JsonValue json, Int128 defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetInt128(out Int128 result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="Int128"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="Int128"/>;
        /// otherwise, returns <c>0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="Int128"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>An <see cref="Int128"/> value if <paramref name="json"/> could be converted; otherwise, <c>0</c>.</returns>
        public static Int128 GetInt128OrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetInt128(out Int128 result, expectedType) ? result : default;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// To prevent losing precision the default value of the <paramref name="asString"/> parameter is <see langword="true"/> in this overload.
        /// <br/>See the <strong>Remarks</strong> section of the <see cref="JsonValue.AsNumber"/> property for details.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this Int128 value, bool asString = true)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString(NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// To prevent losing precision the default value of the <paramref name="asString"/> parameter is <see langword="true"/> in this overload.
        /// <br/>See the <strong>Remarks</strong> section of the <see cref="JsonValue.AsNumber"/> property for details.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this Int128? value, bool asString = true) => value?.ToJson(asString) ?? JsonValue.Null;

#endif
        #endregion

        #region UInt128
#if NET7_0_OR_GREATER

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as an <see cref="UInt128"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="UInt128"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        [CLSCompliant(false)]
        public static bool TryGetUInt128(this JsonValue json, out UInt128 value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return UInt128.TryParse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="UInt128"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="UInt128"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="UInt128"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="ulong"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        [CLSCompliant(false)]
        public static UInt128? AsUInt128(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetUInt128(out UInt128 result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="UInt128"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="UInt128"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="UInt128"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>An <see cref="UInt128"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        [CLSCompliant(false)]
        public static UInt128 GetUInt128OrDefault(this JsonValue json, UInt128 defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetUInt128(out UInt128 result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as an <see cref="UInt128"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="UInt128"/>;
        /// otherwise, returns <c>0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="UInt128"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>An <see cref="UInt128"/> value if <paramref name="json"/> could be converted; otherwise, <c>0</c>.</returns>
        [CLSCompliant(false)]
        public static UInt128 GetUInt128OrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetUInt128(out UInt128 result, expectedType) ? result : default;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// To prevent losing precision the default value of the <paramref name="asString"/> parameter is <see langword="true"/> in this overload.
        /// <br/>See the <strong>Remarks</strong> section of the <see cref="JsonValue.AsNumber"/> property for details.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        [CLSCompliant(false)]
        public static JsonValue ToJson(this UInt128 value, bool asString = true)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString(NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// To prevent losing precision the default value of the <paramref name="asString"/> parameter is <see langword="true"/> in this overload.
        /// <br/>See the <strong>Remarks</strong> section of the <see cref="JsonValue.AsNumber"/> property for details.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        [CLSCompliant(false)]
        public static JsonValue ToJson(this UInt128? value, bool asString = true) => value?.ToJson(asString) ?? JsonValue.Null;

#endif
        #endregion

        #region Half
#if NET5_0_OR_GREATER

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="Half"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="Half"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0.0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetHalf(this JsonValue json, out Half value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return Half.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="Half"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="Half"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="Half"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="Half"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static Half? AsHalf(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetHalf(out Half result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="Half"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="Half"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="Half"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0.0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="Half"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static Half GetHalfOrDefault(this JsonValue json, Half defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetHalf(out Half result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="Half"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="Half"/>;
        /// otherwise, returns <c>0.0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="Half"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="Half"/> value if <paramref name="json"/> could be converted; otherwise, <c>0.0</c>.</returns>
        public static Half GetHalfOrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetHalf(out Half result, expectedType) ? result : (Half)0f;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this Half value, bool asString = false)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString("R", NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this Half? value, bool asString = false) => value?.ToJson(asString) ?? JsonValue.Null;

#endif
        #endregion

        #region Single

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="float"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="float"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0.0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetSingle(this JsonValue json, out float value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return Single.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="float"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="float"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="float"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="float"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static float? AsSingle(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetSingle(out float result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="float"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="float"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="float"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0.0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="float"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static float GetSingleOrDefault(this JsonValue json, float defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetSingle(out float result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="float"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="float"/>;
        /// otherwise, returns <c>0.0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="float"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="float"/> value if <paramref name="json"/> could be converted; otherwise, <c>0.0</c>.</returns>
        public static float GetSingleOrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetSingle(out float result, expectedType) ? result : 0f;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this float value, bool asString = false)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString("R", NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this float? value, bool asString = false) => value?.ToJson(asString) ?? JsonValue.Null;

        #endregion

        #region Double

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="double"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="double"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0.0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetDouble(this JsonValue json, out double value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return Double.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="double"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="double"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="double"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="double"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static double? AsDouble(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetDouble(out double result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="double"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="double"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="double"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0.0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="double"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static double GetDoubleOrDefault(this JsonValue json, double defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetDouble(out double result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="double"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="double"/>;
        /// otherwise, returns <c>0.0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="double"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="double"/> value if <paramref name="json"/> could be converted; otherwise, <c>0.0</c>.</returns>
        public static double GetDoubleOrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetDouble(out double result, expectedType) ? result : 0d;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this double value, bool asString = false)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString("R", NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this double? value, bool asString = false) => value?.ToJson(asString) ?? JsonValue.Null;

        #endregion

        #region Decimal

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="decimal"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="decimal"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <c>0.0</c>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetDecimal(this JsonValue json, out decimal value, JsonValueType expectedType = default)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return Decimal.TryParse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="decimal"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="decimal"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="decimal"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="decimal"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static decimal? AsDecimal(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetDecimal(out decimal result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="decimal"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="decimal"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="decimal"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <c>0.0</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="decimal"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static decimal GetDecimalOrDefault(this JsonValue json, decimal defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetDecimal(out decimal result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="decimal"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="decimal"/>;
        /// otherwise, returns <c>0.0</c>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="decimal"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="decimal"/> value if <paramref name="json"/> could be converted; otherwise, <c>0.0</c>.</returns>
        public static decimal GetDecimalOrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetDecimal(out decimal result, expectedType) ? result : 0m;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// To prevent losing precision the default value of the <paramref name="asString"/> parameter is <see langword="true"/> in this overload.
        /// <br/>See the <strong>Remarks</strong> section of the <see cref="JsonValue.AsNumber"/> property for details.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this decimal value, bool asString = true)
            => new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, value.ToString(NumberFormatInfo.InvariantInfo));

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// To prevent losing precision the default value of the <paramref name="asString"/> parameter is <see langword="true"/> in this overload.
        /// <br/>See the <strong>Remarks</strong> section of the <see cref="JsonValue.AsNumber"/> property for details.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this decimal? value, bool asString = true) => value?.ToJson(asString) ?? JsonValue.Null;

        #endregion

        #region String

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="string"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="string"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <param name="allowNullIfStringIsExpected"><see langword="true"/> to return <see langword="true"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.String"/>
        /// but the actual <see cref="JsonValue.Type"/> is <see cref="JsonValueType.Null"/>; otherwise, <see langword="false"/>. This parameter is optional.
        /// <br/>Default value: <see langword="false"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetString(this JsonValue json, out string? value, JsonValueType expectedType = default, bool allowNullIfStringIsExpected = false)
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
            {
                value = json.IsNull ? null : s;
                return true;
            }

            value = default;
            return expectedType == JsonValueType.String && json.IsNull && allowNullIfStringIsExpected;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="string"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="string"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="string"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="string"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static string? AsString(this JsonValue json, JsonValueType expectedType = default)
            => json.TryGetString(out string? result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="string"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="string"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="string"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="string"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static string? GetStringOrDefault(this JsonValue json, string? defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetString(out string? result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="string"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="string"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="string"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="string"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static string? GetStringOrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetString(out string? result, expectedType) ? result : default;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this string? value) => new JsonValue(value);

        #endregion

        #region Enum

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <typeparamref name="TEnum"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, the default value of <typeparamref name="TEnum"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetEnum<TEnum>(this JsonValue json, out TEnum value, JsonValueType expectedType = default)
            where TEnum : struct, Enum
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && !json.IsNull && json.AsStringInternal is string s)
                return Enum<TEnum>.TryParse(s, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="ignoreFormat"><see langword="true"/> to remove underscores or hyphens, and ignore case when parsing the value; otherwise, <see langword="false"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, the default value of <typeparamref name="TEnum"/>. This parameter is passed uninitialized.</param>
        /// <param name="flagsSeparator">Specifies the separator if the <paramref name="json"/> value consists of multiple flags. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>, which uses the default <c>","</c> separator.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetEnum<TEnum>(this JsonValue json, bool ignoreFormat, out TEnum value, string? flagsSeparator = null, JsonValueType expectedType = default)
            where TEnum : struct, Enum
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && !json.IsNull && json.AsStringInternal is string s)
            {
                // First trying to parse without removing characters. Will not work if the value contains hyphens, which is not valid in C# identifiers
                // but will work if the original identifier also contains underscores.
#if NETFRAMEWORK || NETSTANDARD2_0 || NETCOREAPP2_0
                if (!s.Contains("-", StringComparison.Ordinal) && Enum<TEnum>.TryParse(s, flagsSeparator, ignoreFormat, out value))
#else
                if (!s.Contains('-') && Enum<TEnum>.TryParse(s, flagsSeparator, ignoreFormat, out value))
#endif
                {
                    return true;
                }

                // Trying to remove underscores and hyphens. Will not work if the original identifier contains underscores, which was formatted
                // with adding underscores or hyphens. This could also work by replacing "__" to "_" first but such formatting is intentionally considered invalid.
                if (ignoreFormat && s.IndexOfAny(new[] { '_', '-' }) > 0)
                {
                    s = s.Replace("_", String.Empty);
                    s = s.Replace("-", String.Empty);
                    return Enum<TEnum>.TryParse(s, flagsSeparator, true, out value);
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="ignoreFormat"><see langword="true"/> to remove underscores or hyphens, and ignore case when parsing the value; otherwise, <see langword="false"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, the default value of <typeparamref name="TEnum"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetEnum<TEnum>(this JsonValue json, bool ignoreFormat, out TEnum value, JsonValueType expectedType) where TEnum : struct, Enum
            => TryGetEnum(json, ignoreFormat, out value, null, expectedType);

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <typeparamref name="TEnum"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <typeparamref name="TEnum"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static TEnum? AsEnum<TEnum>(this JsonValue json, JsonValueType expectedType = default) where TEnum : struct, Enum
            => json.TryGetEnum(out TEnum result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <typeparamref name="TEnum"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="ignoreFormat"><see langword="true"/> to remove underscores or hyphens, and ignore case when parsing the value; otherwise, <see langword="false"/>.</param>
        /// <param name="flagsSeparator">Specifies the separator if the <paramref name="json"/> value consists of multiple flags. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>, which uses the default <c>","</c> separator.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <typeparamref name="TEnum"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static TEnum? AsEnum<TEnum>(this JsonValue json, bool ignoreFormat, string? flagsSeparator = null, JsonValueType expectedType = default) where TEnum : struct, Enum
            => json.TryGetEnum(ignoreFormat, out TEnum result, flagsSeparator, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <typeparamref name="TEnum"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="ignoreFormat"><see langword="true"/> to remove underscores or hyphens, and ignore case when parsing the value; otherwise, <see langword="false"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <typeparamref name="TEnum"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static TEnum? AsEnum<TEnum>(this JsonValue json, bool ignoreFormat, JsonValueType expectedType) where TEnum : struct, Enum
            => json.TryGetEnum(ignoreFormat, out TEnum result, null, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <typeparamref name="TEnum"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <typeparamref name="TEnum"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static TEnum GetEnumOrDefault<TEnum>(this JsonValue json, TEnum defaultValue = default, JsonValueType expectedType = default) where TEnum : struct, Enum
            => json.TryGetEnum(out TEnum result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <typeparamref name="TEnum"/>;
        /// otherwise, returns the default value of <typeparamref name="TEnum"/>.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <typeparamref name="TEnum"/> value if <paramref name="json"/> could be converted; otherwise, the default value of <typeparamref name="TEnum"/>.</returns>
        public static TEnum GetEnumOrDefault<TEnum>(this JsonValue json, JsonValueType expectedType) where TEnum : struct, Enum
            => json.TryGetEnum(out TEnum result, expectedType) ? result : default;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <typeparamref name="TEnum"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="ignoreFormat"><see langword="true"/> to remove underscores or hyphens, and ignore case when parsing the value; otherwise, <see langword="false"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <param name="flagsSeparator">Specifies the separator character if the <paramref name="json"/> value consists of multiple flags. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>, which uses the default <c>","</c> separator.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <typeparamref name="TEnum"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static TEnum GetEnumOrDefault<TEnum>(this JsonValue json, bool ignoreFormat, TEnum defaultValue = default, string? flagsSeparator = null, JsonValueType expectedType = default) where TEnum : struct, Enum
            => json.TryGetEnum(ignoreFormat, out TEnum result, flagsSeparator, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <typeparamref name="TEnum"/>;
        /// otherwise, returns the default value of <typeparamref name="TEnum"/>.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="ignoreFormat"><see langword="true"/> to remove underscores or hyphens, and ignore case when parsing the value; otherwise, <see langword="false"/>.</param>
        /// <param name="flagsSeparator">Specifies the separator character if the <paramref name="json"/> value consists of multiple flags.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <typeparamref name="TEnum"/> value if <paramref name="json"/> could be converted; otherwise, the default value of <typeparamref name="TEnum"/>.</returns>
        public static TEnum GetEnumOrDefault<TEnum>(this JsonValue json, bool ignoreFormat, string? flagsSeparator, JsonValueType expectedType = default) where TEnum : struct, Enum
            => json.TryGetEnum(ignoreFormat, out TEnum result, flagsSeparator, expectedType) ? result : default;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <typeparamref name="TEnum"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="ignoreFormat"><see langword="true"/> to remove underscores or hyphens, and ignore case when parsing the value; otherwise, <see langword="false"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <typeparamref name="TEnum"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static TEnum GetEnumOrDefault<TEnum>(this JsonValue json, bool ignoreFormat, TEnum defaultValue, JsonValueType expectedType) where TEnum : struct, Enum
            => json.TryGetEnum(ignoreFormat, out TEnum result, null, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <typeparamref name="TEnum"/>;
        /// otherwise, returns the default value of <typeparamref name="TEnum"/>.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="ignoreFormat"><see langword="true"/> to remove underscores or hyphens, and ignore case when parsing the value; otherwise, <see langword="false"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <typeparamref name="TEnum"/> value if <paramref name="json"/> could be converted; otherwise, the default value of <typeparamref name="TEnum"/>.</returns>
        public static TEnum GetEnumOrDefault<TEnum>(this JsonValue json, bool ignoreFormat, JsonValueType expectedType) where TEnum : struct, Enum
            => json.TryGetEnum(ignoreFormat, out TEnum result, null, expectedType) ? result : default;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the format of the enum in the JSON value. This parameter is optional.
        /// <br/>Default value: <see cref="JsonEnumFormat.PascalCase"/>.</param>
        /// <param name="flagsSeparator">Specifies the separator if <paramref name="value"/> consists of multiple flags. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>, which uses the default <c>", "</c> separator.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/> is not one of the defined values.</exception>
        public static JsonValue ToJson<TEnum>(this TEnum value, JsonEnumFormat format = JsonEnumFormat.PascalCase, string? flagsSeparator = null)
            where TEnum : struct, Enum
        {
            const string defaultSeparator = ", ";

            #region Local Methods

            static string AdjustFirstCharsCasing(string value, bool upperCase, string? flagsSeparator)
            {
                flagsSeparator ??= defaultSeparator;
                Func<char, bool> needsTransform;
                Func<char, char> transform;
                if (upperCase)
                {
                    if (!Char.IsLower(value[0]) && !value.Contains(flagsSeparator))
                        return value;

                    needsTransform = Char.IsLower;
                    transform = Char.ToUpperInvariant;
                }
                else
                {
                    if (!Char.IsUpper(value[0]) && !value.Contains(flagsSeparator))
                        return value;

                    needsTransform = Char.IsUpper;
                    transform = Char.ToLowerInvariant;
                }

                // using a StringBuilder would be an overkill because we don't change the length
                char[] result = value.ToCharArray();

                // first char
                if (needsTransform.Invoke(result[0]))
                    result[0] = transform.Invoke(result[0]);

                // first char of further flags
                int separatorLength = flagsSeparator.Length;
                for (int pos, start = 0; (pos = value.IndexOf(flagsSeparator, start, StringComparison.Ordinal)) >= 0; start = pos)
                {
                    pos += separatorLength;
                    if (pos == value.Length)
                        break;

                    if (needsTransform.Invoke(result[pos]))
                        result[pos] = transform.Invoke(result[pos]);
                }

                return new String(result);
            }

            static string AdjustCasingWithInsertions(string value, bool upperCase, char wordsSeparator, string? flagsSeparator)
            {
                flagsSeparator ??= defaultSeparator;

                // Using a list instead of StringBuilder because it is usually faster in our case. Initializing with fewest possible allocations.
                //var result = new CircularList<char>(value.Length * 2);
                //result.AddRange(value);
                var result = new StringBuilder(value, value.Length * 2);

                // Processing from the end so length change will not be a problem
                int nextFlagPos = value.LastIndexOf(flagsSeparator, StringComparison.Ordinal);
                nextFlagPos = nextFlagPos < 0 ? 0 : nextFlagPos + flagsSeparator.Length;
                for (int i = value.Length - 1; i >= 0; i--)
                {
                    // Assuming that the original value was in PascalCase so inserting separators before upper case letters, except the first letters
                    if (Char.IsUpper(value[i]))
                    {
                        if (!upperCase)
                            result[i] = Char.ToLowerInvariant(value[i]);
                        if (i != nextFlagPos)
                            result.Insert(i, wordsSeparator);
                    }
                    else if (upperCase && Char.IsLower(value[i]))
                        result[i] = Char.ToUpperInvariant(value[i]);

                    if (i == nextFlagPos && i > 0)
                    {
                        nextFlagPos = value.LastIndexOf(flagsSeparator, nextFlagPos - flagsSeparator.Length - 1, StringComparison.Ordinal);
                        nextFlagPos = nextFlagPos < 0 ? 0 : nextFlagPos + flagsSeparator.Length;
                    }
                }

                return result.ToString();
            }

            #endregion

            if ((uint)format > (uint)JsonEnumFormat.NumberAsString)
                Throw.ArgumentOutOfRangeException(PublicResources.EnumOutOfRange(format), nameof(format));

            if (format is JsonEnumFormat.Number or JsonEnumFormat.NumberAsString)
                return new JsonValue(format == JsonEnumFormat.Number ? JsonValueType.Number : JsonValueType.String, Enum<TEnum>.ToString(value, EnumFormattingOptions.Number));

            string enumValue = Enum<TEnum>.ToString(value, flagsSeparator);
            switch (format)
            {
                case JsonEnumFormat.PascalCase:
                    return AdjustFirstCharsCasing(enumValue, true, flagsSeparator);

                case JsonEnumFormat.CamelCase:
                    return AdjustFirstCharsCasing(enumValue, false, flagsSeparator);

                case JsonEnumFormat.LowerCase:
                    return enumValue.ToLowerInvariant();

                case JsonEnumFormat.UpperCase:
                    return enumValue.ToUpperInvariant();

                case JsonEnumFormat.LowerCaseWithUnderscores:
                    return AdjustCasingWithInsertions(enumValue, false, '_', flagsSeparator);

                case JsonEnumFormat.UpperCaseWithUnderscores:
                    return AdjustCasingWithInsertions(enumValue, true, '_', flagsSeparator);

                case JsonEnumFormat.LowerCaseWithHyphens:
                    return AdjustCasingWithInsertions(enumValue, false, '-', flagsSeparator);

                case JsonEnumFormat.UpperCaseWithHyphens:
                    return AdjustCasingWithInsertions(enumValue, true, '-', flagsSeparator);
            }

            Debug.Fail("This point should not be reached");
            return default;
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration. Must be an <see cref="Enum"/> type.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the format of the enum in the JSON value. This parameter is optional.
        /// <br/>Default value: <see cref="JsonEnumFormat.PascalCase"/>.</param>
        /// <param name="flagsSeparator">Specifies the separator if <paramref name="value"/> consists of multiple flags. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>, which uses the default <c>", "</c> separator.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson<TEnum>(this TEnum? value, JsonEnumFormat format = JsonEnumFormat.PascalCase, string? flagsSeparator = null) where TEnum : struct, Enum
            => value?.ToJson(format, flagsSeparator) ?? JsonValue.Null;

        #endregion

        #region DateTime

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// The actual format is attempted to be auto detected. If you know exact format use the
        /// other <see cref="O:KGySoft.Json.JsonValueExtensions.TryGetDateTime">TryGetDateTime</see> overloads.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="DateTime.MinValue"/>. This parameter is passed uninitialized.</param>
        /// <param name="desiredKind">The desired value of the <see cref="DateTime.Kind"/> property of the returned <paramref name="value"/>,
        /// or <see langword="null"/> to preserve the one that could be retrieved from the <see cref="JsonValue"/>.
        /// Converting between <see cref="DateTimeKind.Utc"/> and <see cref="DateTimeKind.Local"/> affects the actual time value,
        /// while changing to or from <see cref="DateTimeKind.Unspecified"/> just changes the <see cref="DateTime.Kind"/> property without converting the value. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetDateTime(this JsonValue json, out DateTime value, DateTimeKind? desiredKind = null, JsonValueType expectedType = default)
            => json.TryGetDateTime(JsonDateTimeFormat.Auto, out value, desiredKind, expectedType);

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// The actual format is attempted to be auto detected. If you know exact format use the
        /// other <see cref="O:KGySoft.Json.JsonValueExtensions.TryGetDateTime">TryGetDateTime</see> overloads.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="DateTime.MinValue"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetDateTime(this JsonValue json, out DateTime value, JsonValueType expectedType)
            => json.TryGetDateTime(JsonDateTimeFormat.Auto, out value, null, expectedType);

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value using the specified <paramref name="format"/>
        /// if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date-time value in the <see cref="JsonValue"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="DateTime.MinValue"/>. This parameter is passed uninitialized.</param>
        /// <param name="desiredKind">The desired value of the <see cref="DateTime.Kind"/> property of the returned <paramref name="value"/>,
        /// or <see langword="null"/> to preserve the one that could be retrieved from the <see cref="JsonValue"/>.
        /// Converting between <see cref="DateTimeKind.Utc"/> and <see cref="DateTimeKind.Local"/> affects the actual time value,
        /// while changing to or from <see cref="DateTimeKind.Unspecified"/> just changes the <see cref="DateTime.Kind"/> property without converting the value. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetDateTime(this JsonValue json, JsonDateTimeFormat format, out DateTime value, DateTimeKind? desiredKind = null, JsonValueType expectedType = default)
        {
            if ((uint)format > (uint)JsonDateTimeFormat.MicrosoftLegacy)
                Throw.ArgumentOutOfRangeException(PublicResources.EnumOutOfRange(format), nameof(format));
            if (desiredKind.HasValue && (uint)desiredKind > (uint)DateTimeKind.Local)
                Throw.ArgumentOutOfRangeException(PublicResources.EnumOutOfRange(desiredKind.Value), nameof(desiredKind));
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || json.AsStringInternal is not string s)
            {
                value = default;
                return false;
            }

            if (!s.TryParseDateTime(format, json.Type == JsonValueType.Number, out value))
                return false;

            if (!desiredKind.HasValue || value.Kind == desiredKind)
                return true;

            if (value.Kind == DateTimeKind.Unspecified || desiredKind == DateTimeKind.Unspecified)
                value = DateTime.SpecifyKind(value, desiredKind.Value);
            else
                value = desiredKind == DateTimeKind.Utc ? value.ToUniversalTime() : value.ToLocalTime();

            return true;
        }

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value using the specified <paramref name="format"/>
        /// if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date-time value in the <see cref="JsonValue"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="DateTime.MinValue"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetDateTime(this JsonValue json, JsonDateTimeFormat format, out DateTime value, JsonValueType expectedType)
            => json.TryGetDateTime(format, out value, null, expectedType);

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value using the specified <paramref name="format"/>
        /// if <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter is <see cref="JsonValueType.String"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="format">Specifies the exact format of the date-time value in the <see cref="JsonValue"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="DateTime.MinValue"/>. This parameter is passed uninitialized.</param>
        /// <param name="desiredKind">The desired value of the <see cref="DateTime.Kind"/> property of the returned <paramref name="value"/>,
        /// or <see langword="null"/> to preserve the one that could be retrieved from the <see cref="JsonValue"/>.
        /// Converting between <see cref="DateTimeKind.Utc"/> and <see cref="DateTimeKind.Local"/> affects the actual time value,
        /// while changing to or from <see cref="DateTimeKind.Unspecified"/> just changes the <see cref="DateTime.Kind"/> property without converting the value. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetDateTime(this JsonValue json, string format, out DateTime value, DateTimeKind? desiredKind = null)
        {
            if (format == null!)
                Throw.ArgumentNullException(nameof(format));
            if (desiredKind.HasValue && (uint)desiredKind > (uint)DateTimeKind.Local)
                Throw.ArgumentOutOfRangeException(PublicResources.EnumOutOfRange(desiredKind.Value), nameof(desiredKind));
            if (json.AsString is not string s)
            {
                value = default;
                return false;
            }

            if (!DateTime.TryParseExact(s, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out value))
                return false;

            if (!desiredKind.HasValue || value.Kind == desiredKind)
                return true;

            if (value.Kind == DateTimeKind.Unspecified || desiredKind == DateTimeKind.Unspecified)
                value = DateTime.SpecifyKind(value, desiredKind.Value);
            else
                value = desiredKind == DateTimeKind.Utc ? value.ToUniversalTime() : value.ToLocalTime();

            return true;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value using the specified <paramref name="format"/>
        /// if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/>
        /// property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="DateTime"/>; otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date-time value in the <see cref="JsonValue"/>. This parameter is optional.
        /// <br/>Default value: <see cref="JsonDateTimeFormat.Auto"/>, which attempts to auto detect the format.</param>
        /// <param name="desiredKind">The desired value of the <see cref="DateTime.Kind"/> property of the returned <see cref="DateTime"/> instance,
        /// or <see langword="null"/> to preserve the one that could be retrieved from the <see cref="JsonValue"/>.
        /// Converting between <see cref="DateTimeKind.Utc"/> and <see cref="DateTimeKind.Local"/> affects the actual time value,
        /// while changing to or from <see cref="DateTimeKind.Unspecified"/> just changes the <see cref="DateTime.Kind"/> property without converting the value. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static DateTime? AsDateTime(this JsonValue json, JsonDateTimeFormat format = JsonDateTimeFormat.Auto, DateTimeKind? desiredKind = null, JsonValueType expectedType = default)
            => json.TryGetDateTime(format, out DateTime result, desiredKind, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value using the specified <paramref name="format"/>
        /// if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/>
        /// property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="DateTime"/>; otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date-time value in the <see cref="JsonValue"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static DateTime? AsDateTime(this JsonValue json, JsonDateTimeFormat format, JsonValueType expectedType)
            => json.TryGetDateTime(format, out DateTime result, null, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value using the specified <paramref name="format"/>
        /// if <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter is <see cref="JsonValueType.String"/>
        /// and it can be converted to <see cref="DateTime"/>; otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="format">Specifies the exact format of the date-time value in the <see cref="JsonValue"/>.</param>
        /// <param name="desiredKind">The desired value of the <see cref="DateTime.Kind"/> property of the returned <see cref="DateTime"/> instance,
        /// or <see langword="null"/> to preserve the one that could be retrieved from the <see cref="JsonValue"/>.
        /// Converting between <see cref="DateTimeKind.Utc"/> and <see cref="DateTimeKind.Local"/> affects the actual time value,
        /// while changing to or from <see cref="DateTimeKind.Unspecified"/> just changes the <see cref="DateTime.Kind"/> property without converting the value. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static DateTime? AsDateTime(this JsonValue json, string format, DateTimeKind? desiredKind = null)
            => json.TryGetDateTime(format, out DateTime result, desiredKind) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="DateTime"/>;
        /// otherwise, returns <paramref name="defaultValue"/>. The actual format is attempted to be auto detected. If you know exact format use the
        /// other <see cref="O:KGySoft.Json.JsonValueExtensions.GetDateTimeOrDefault">GetDateTimeOrDefault</see> overloads.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see cref="DateTime.MinValue"/>.</param>
        /// <param name="desiredKind">The desired value of the <see cref="DateTime.Kind"/> property of the returned <see cref="DateTime"/> instance,
        /// or <see langword="null"/> to preserve the one that could be retrieved from the <see cref="JsonValue"/>.
        /// Converting between <see cref="DateTimeKind.Utc"/> and <see cref="DateTimeKind.Local"/> affects the actual time value,
        /// while changing to or from <see cref="DateTimeKind.Unspecified"/> just changes the <see cref="DateTime.Kind"/> property without converting the value. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static DateTime GetDateTimeOrDefault(this JsonValue json, DateTime defaultValue = default, DateTimeKind? desiredKind = null, JsonValueType expectedType = default)
            => json.TryGetDateTime(JsonDateTimeFormat.Auto, out DateTime result, desiredKind, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="DateTime"/>;
        /// otherwise, returns <paramref name="defaultValue"/>. The actual format is attempted to be auto detected. If you know exact format use the
        /// other <see cref="O:KGySoft.Json.JsonValueExtensions.GetDateTimeOrDefault">GetDateTimeOrDefault</see> overloads.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static DateTime GetDateTimeOrDefault(this JsonValue json, DateTime defaultValue, JsonValueType expectedType)
            => json.TryGetDateTime(JsonDateTimeFormat.Auto, out DateTime result, null, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="DateTime"/>;
        /// otherwise, returns <see cref="DateTime.MinValue">DateTime.MinValue</see>, which is the default value of <see cref="DateTime"/>.
        /// The actual format is attempted to be auto detected. If you know exact format use the
        /// other <see cref="O:KGySoft.Json.JsonValueExtensions.GetDateTimeOrDefault">GetDateTimeOrDefault</see> overloads.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="desiredKind">The desired value of the <see cref="DateTime.Kind"/> property of the returned <see cref="DateTime"/> instance,
        /// or <see langword="null"/> to preserve the one that could be retrieved from the <see cref="JsonValue"/>.
        /// Converting between <see cref="DateTimeKind.Utc"/> and <see cref="DateTimeKind.Local"/> affects the actual time value,
        /// while changing to or from <see cref="DateTimeKind.Unspecified"/> just changes the <see cref="DateTime.Kind"/> property without converting the value.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <see cref="DateTime.MinValue">DateTime.MinValue</see>, which is the default value of <see cref="DateTime"/>.</returns>
        public static DateTime GetDateTimeOrDefault(this JsonValue json, DateTimeKind? desiredKind, JsonValueType expectedType = default)
            => json.TryGetDateTime(JsonDateTimeFormat.Auto, out DateTime result, desiredKind, expectedType) ? result : default;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="DateTime"/>;
        /// otherwise, returns <see cref="DateTime.MinValue">DateTime.MinValue</see>, which is the default value of <see cref="DateTime"/>.
        /// The actual format is attempted to be auto detected. If you know exact format use the
        /// other <see cref="O:KGySoft.Json.JsonValueExtensions.GetDateTimeOrDefault">GetDateTimeOrDefault</see> overloads.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <see cref="DateTime.MinValue">DateTime.MinValue</see>, which is the default value of <see cref="DateTime"/>.</returns>
        public static DateTime GetDateTimeOrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetDateTime(JsonDateTimeFormat.Auto, out DateTime result, null, expectedType) ? result : default;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value using the specified <paramref name="format"/> if <paramref name="expectedType"/>
        /// is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be
        /// converted to <see cref="DateTime"/>; otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date-time value in the <see cref="JsonValue"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see cref="DateTime.MinValue"/>.</param>
        /// <param name="desiredKind">The desired value of the <see cref="DateTime.Kind"/> property of the returned <see cref="DateTime"/> instance,
        /// or <see langword="null"/> to preserve the one that could be retrieved from the <see cref="JsonValue"/>.
        /// Converting between <see cref="DateTimeKind.Utc"/> and <see cref="DateTimeKind.Local"/> affects the actual time value,
        /// while changing to or from <see cref="DateTimeKind.Unspecified"/> just changes the <see cref="DateTime.Kind"/> property without converting the value. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static DateTime GetDateTimeOrDefault(this JsonValue json, JsonDateTimeFormat format, DateTime defaultValue = default, DateTimeKind? desiredKind = null, JsonValueType expectedType = default)
            => json.TryGetDateTime(format, out DateTime result, desiredKind, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value using the specified <paramref name="format"/> if <paramref name="expectedType"/>
        /// is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be
        /// converted to <see cref="DateTime"/>; otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date-time value in the <see cref="JsonValue"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static DateTime GetDateTimeOrDefault(this JsonValue json, JsonDateTimeFormat format, DateTime defaultValue, JsonValueType expectedType)
            => json.TryGetDateTime(format, out DateTime result, null, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value using the specified <paramref name="format"/> if <paramref name="expectedType"/>
        /// is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be
        /// converted to <see cref="DateTime"/>; otherwise, returns <see cref="DateTime.MinValue">DateTime.MinValue</see>, which is the default value of <see cref="DateTime"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date-time value in the <see cref="JsonValue"/>.</param>
        /// <param name="desiredKind">The desired value of the <see cref="DateTime.Kind"/> property of the returned <see cref="DateTime"/> instance,
        /// or <see langword="null"/> to preserve the one that could be retrieved from the <see cref="JsonValue"/>.
        /// Converting between <see cref="DateTimeKind.Utc"/> and <see cref="DateTimeKind.Local"/> affects the actual time value,
        /// while changing to or from <see cref="DateTimeKind.Unspecified"/> just changes the <see cref="DateTime.Kind"/> property without converting the value.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <see cref="DateTime.MinValue">DateTime.MinValue</see>, which is the default value of <see cref="DateTime"/>.</returns>
        public static DateTime GetDateTimeOrDefault(this JsonValue json, JsonDateTimeFormat format, DateTimeKind? desiredKind, JsonValueType expectedType = default)
            => json.TryGetDateTime(format, out DateTime result, desiredKind, expectedType) ? result : default;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value using the specified <paramref name="format"/> if <paramref name="expectedType"/>
        /// is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be
        /// converted to <see cref="DateTime"/>; otherwise, returns <see cref="DateTime.MinValue">DateTime.MinValue</see>, which is the default value of <see cref="DateTime"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date-time value in the <see cref="JsonValue"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <see cref="DateTime.MinValue">DateTime.MinValue</see>, which is the default value of <see cref="DateTime"/>.</returns>
        public static DateTime GetDateTimeOrDefault(this JsonValue json, JsonDateTimeFormat format, JsonValueType expectedType)
            => json.TryGetDateTime(format, out DateTime result, null, expectedType) ? result : default;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value using the specified <paramref name="format"/>
        /// if <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter is <see cref="JsonValueType.String"/> and it can be
        /// converted to <see cref="DateTime"/>; otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="format">Specifies the exact format of the date-time value in the <see cref="JsonValue"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see cref="DateTime.MinValue"/>.</param>
        /// <param name="desiredKind">The desired value of the <see cref="DateTime.Kind"/> property of the returned <see cref="DateTime"/> instance,
        /// or <see langword="null"/> to preserve the one that could be retrieved from the <see cref="JsonValue"/>.
        /// Converting between <see cref="DateTimeKind.Utc"/> and <see cref="DateTimeKind.Local"/> affects the actual time value,
        /// while changing to or from <see cref="DateTimeKind.Unspecified"/> just changes the <see cref="DateTime.Kind"/> property without converting the value. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static DateTime GetDateTimeOrDefault(this JsonValue json, string format, DateTime defaultValue = default, DateTimeKind? desiredKind = null)
            => json.TryGetDateTime(format, out DateTime result, desiredKind) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTime"/> value using the specified <paramref name="format"/>
        /// if <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter is <see cref="JsonValueType.String"/> and it can be
        /// converted to <see cref="DateTime"/>; otherwise, returns <see cref="DateTime.MinValue">DateTime.MinValue</see>, which is the default value of <see cref="DateTime"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTime"/>.</param>
        /// <param name="format">Specifies the exact format of the date-time value in the <see cref="JsonValue"/>.</param>
        /// <param name="desiredKind">The desired value of the <see cref="DateTime.Kind"/> property of the returned <see cref="DateTime"/> instance,
        /// or <see langword="null"/> to preserve the one that could be retrieved from the <see cref="JsonValue"/>.
        /// Converting between <see cref="DateTimeKind.Utc"/> and <see cref="DateTimeKind.Local"/> affects the actual time value,
        /// while changing to or from <see cref="DateTimeKind.Unspecified"/> just changes the <see cref="DateTime.Kind"/> property without converting the value. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <see cref="DateTime.MinValue">DateTime.MinValue</see>, which is the default value of <see cref="DateTime"/>.</returns>
        public static DateTime GetDateTimeOrDefault(this JsonValue json, string format, DateTimeKind? desiredKind)
            => json.TryGetDateTime(format, out DateTime result, desiredKind) ? result : default;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the format of the <paramref name="value"/> as a JSON value. This parameter is optional.
        /// <br/>Default value: <see cref="JsonDateTimeFormat.Auto"/>, which applies <see cref="JsonDateTimeFormat.Iso8601JavaScript"/> if <paramref name="asString"/> is <see langword="true"/>,
        /// or <see cref="JsonDateTimeFormat.UnixMilliseconds"/> if <paramref name="asString"/> is <see langword="false"/>.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; or <see langword="false"/> to convert it to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.Number"/>&#160;<see cref="JsonValue.Type"/>, which is not applicable for all <paramref name="format"/>s. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/> is not one of the defined values.</exception>
        /// <exception cref="ArgumentException"><paramref name="asString"/> is <see langword="false"/> but <paramref name="format"/> represents a string-only format.</exception>
        public static JsonValue ToJson(this DateTime value, JsonDateTimeFormat format = JsonDateTimeFormat.Auto, bool asString = true)
        {
            if ((uint)format > (uint)JsonDateTimeFormat.MicrosoftLegacy)
                Throw.ArgumentOutOfRangeException(PublicResources.EnumOutOfRange(format), nameof(format));
            if (!asString && format > JsonDateTimeFormat.Ticks)
                Throw.ArgumentException(Res.DateTimeFormatIsStringOnly(format), nameof(format));

            if (format == JsonDateTimeFormat.Auto)
                format = asString ? JsonDateTimeFormat.Iso8601JavaScript : JsonDateTimeFormat.UnixMilliseconds;

            string formattedValue = format switch
            {
                JsonDateTimeFormat.Iso8601JavaScript => value.AsUtc().ToString(DateTimeExtensions.Iso8601JavaScriptFormat, DateTimeFormatInfo.InvariantInfo),
                JsonDateTimeFormat.UnixMilliseconds => value.ToUnixMilliseconds().ToString(NumberFormatInfo.InvariantInfo),
                JsonDateTimeFormat.UnixSeconds => value.ToUnixSeconds().ToString(NumberFormatInfo.InvariantInfo),
                JsonDateTimeFormat.UnixSecondsFloat => value.ToUnixSecondsFloat().ToString("F3", NumberFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Ticks => value.AsUtc().Ticks.ToString(NumberFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Iso8601Roundtrip => value.ToString("O", DateTimeFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Iso8601Utc => value.AsUtc().ToString("O", DateTimeFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Iso8601Local => value.AsLocal().ToString("O", DateTimeFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Iso8601Date => value.ToString(DateTimeExtensions.Iso8601DateFormat, DateTimeFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Iso8601Minutes => value.ToString(DateTimeExtensions.Iso8601MinutesFormat, DateTimeFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Iso8601Seconds => value.ToString(DateTimeExtensions.Iso8601SecondsFormat, DateTimeFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Iso8601Milliseconds => value.ToString(DateTimeExtensions.Iso8601MillisecondsFormat, DateTimeFormatInfo.InvariantInfo),
                /*JsonDateTimeFormat.MicrosoftLegacy*/ _ => value.ToMicrosoftJsonDate(),
            };

            return new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, formattedValue);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the format of the <paramref name="value"/> as a JSON value. This parameter is optional.
        /// <br/>Default value: <see cref="JsonDateTimeFormat.Auto"/>, which applies <see cref="JsonDateTimeFormat.Iso8601JavaScript"/> if <paramref name="asString"/> is <see langword="true"/>,
        /// or <see cref="JsonDateTimeFormat.UnixMilliseconds"/> if <paramref name="asString"/> is <see langword="false"/>.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; or <see langword="false"/> to convert it to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.Number"/>&#160;<see cref="JsonValue.Type"/>, which is not applicable for all <paramref name="format"/>s. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/> is not one of the defined values.</exception>
        /// <exception cref="ArgumentException"><paramref name="asString"/> is <see langword="false"/> but <paramref name="format"/> represents a string-only format.</exception>
        public static JsonValue ToJson(this DateTime? value, JsonDateTimeFormat format = JsonDateTimeFormat.Auto, bool asString = true)
            => value?.ToJson(format, asString) ?? JsonValue.Null;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the exact format of the <paramref name="value"/> as a JSON value.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        public static JsonValue ToJson(this DateTime value, string format)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, format CAN be null but MUST NOT be
            => value.ToString(format ?? Throw.ArgumentNullException<string>(nameof(format)), DateTimeFormatInfo.InvariantInfo);

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the exact format of the <paramref name="value"/> as a JSON value.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        public static JsonValue ToJson(this DateTime? value, string format) => value?.ToJson(format) ?? JsonValue.Null;

        #endregion

        #region DateTimeOffset

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="DateTimeOffset"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// The actual format is attempted to be auto detected. If you know exact format use the
        /// other <see cref="O:KGySoft.Json.JsonValueExtensions.TryGetDateTimeOffset">TryGetDateTimeOffset</see> overloads.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTimeOffset"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="DateTimeOffset.MinValue"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetDateTimeOffset(this JsonValue json, out DateTimeOffset value, JsonValueType expectedType = default)
            => json.TryGetDateTimeOffset(JsonDateTimeFormat.Auto, out value, expectedType);

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="DateTimeOffset"/> value using the specified <paramref name="format"/>
        /// if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTimeOffset"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date-time offset value in the <see cref="JsonValue"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="DateTimeOffset.MinValue"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetDateTimeOffset(this JsonValue json, JsonDateTimeFormat format, out DateTimeOffset value, JsonValueType expectedType = default)
        {
            if ((uint)format > (uint)JsonDateTimeFormat.MicrosoftLegacy)
                Throw.ArgumentOutOfRangeException(PublicResources.EnumOutOfRange(format), nameof(format));
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return s.TryParseDateTimeOffset(format, json.Type == JsonValueType.Number, out value);

            value = default;
            return false;

        }

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="DateTimeOffset"/> value using the specified <paramref name="format"/>
        /// if <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter is <see cref="JsonValueType.String"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTimeOffset"/>.</param>
        /// <param name="format">Specifies the exact format of the date-time offset value in the <see cref="JsonValue"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="DateTimeOffset.MinValue"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetDateTimeOffset(this JsonValue json, string format, out DateTimeOffset value)
        {
            if (format == null!)
                Throw.ArgumentNullException(nameof(format));
            if (json.AsString is not string s)
            {
                value = default;
                return false;
            }

            return DateTimeOffset.TryParseExact(s, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out value);
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTimeOffset"/> value using the specified <paramref name="format"/>
        /// if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/>
        /// property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="DateTimeOffset"/>; otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTimeOffset"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date-time offset value in the <see cref="JsonValue"/>. This parameter is optional.
        /// <br/>Default value: <see cref="JsonDateTimeFormat.Auto"/>, which attempts to auto detect the format.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateTimeOffset"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static DateTimeOffset? AsDateTimeOffset(this JsonValue json, JsonDateTimeFormat format = JsonDateTimeFormat.Auto, JsonValueType expectedType = default)
            => json.TryGetDateTimeOffset(format, out DateTimeOffset result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTimeOffset"/> value using the specified <paramref name="format"/>
        /// if <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter is <see cref="JsonValueType.String"/>
        /// and it can be converted to <see cref="DateTimeOffset"/>; otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTimeOffset"/>.</param>
        /// <param name="format">Specifies the exact format of the date-time offset value in the <see cref="JsonValue"/>.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static DateTimeOffset? AsDateTimeOffset(this JsonValue json, string format)
            => json.TryGetDateTimeOffset(format, out DateTimeOffset result) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTimeOffset"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="DateTimeOffset"/>;
        /// otherwise, returns <paramref name="defaultValue"/>. The actual format is attempted to be auto detected. If you know exact format use the
        /// other <see cref="O:KGySoft.Json.JsonValueExtensions.GetDateTimeOffsetOrDefault">GetDateTimeOffsetOrDefault</see> overloads.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTimeOffset"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see cref="DateTimeOffset.MinValue"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateTimeOffset"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static DateTimeOffset GetDateTimeOffsetOrDefault(this JsonValue json, DateTimeOffset defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetDateTimeOffset(JsonDateTimeFormat.Auto, out DateTimeOffset result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTimeOffset"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="DateTimeOffset"/>;
        /// otherwise, returns <see cref="DateTimeOffset.MinValue">DateTimeOffset.MinValue</see>, which is the default value of <see cref="DateTimeOffset"/>.
        /// The actual format is attempted to be auto detected. If you know exact format use the
        /// other <see cref="O:KGySoft.Json.JsonValueExtensions.GetDateTimeOffsetOrDefault">GetDateTimeOffsetOrDefault</see> overloads.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTimeOffset"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="DateTimeOffset"/> value if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="DateTimeOffset.MinValue">DateTimeOffset.MinValue</see>, which is the default value of <see cref="DateTimeOffset"/>.</returns>
        public static DateTimeOffset GetDateTimeOffsetOrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetDateTimeOffset(JsonDateTimeFormat.Auto, out DateTimeOffset result, expectedType) ? result : default;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTimeOffset"/> value using the specified <paramref name="format"/> if <paramref name="expectedType"/>
        /// is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be
        /// converted to <see cref="DateTimeOffset"/>; otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTimeOffset"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date-time offset value in the <see cref="JsonValue"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see cref="DateTimeOffset.MinValue"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateTimeOffset"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static DateTimeOffset GetDateTimeOffsetOrDefault(this JsonValue json, JsonDateTimeFormat format, DateTimeOffset defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetDateTimeOffset(format, out DateTimeOffset result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTimeOffset"/> value using the specified <paramref name="format"/> if <paramref name="expectedType"/>
        /// is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be
        /// converted to <see cref="DateTimeOffset"/>; otherwise, returns <see cref="DateTimeOffset.MinValue">DateTimeOffset.MinValue</see>, which is the default value of <see cref="DateTimeOffset"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTimeOffset"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date-time offset value in the <see cref="JsonValue"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="DateTimeOffset"/> value if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="DateTimeOffset.MinValue">DateTimeOffset.MinValue</see>, which is the default value of <see cref="DateTimeOffset"/>.</returns>
        public static DateTimeOffset GetDateTimeOffsetOrDefault(this JsonValue json, JsonDateTimeFormat format, JsonValueType expectedType)
            => json.TryGetDateTimeOffset(format, out DateTimeOffset result, expectedType) ? result : default;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateTimeOffset"/> value using the specified <paramref name="format"/>
        /// if <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter is <see cref="JsonValueType.String"/> and it can be
        /// converted to <see cref="DateTimeOffset"/>; otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateTimeOffset"/>.</param>
        /// <param name="format">Specifies the exact format of the date-time offset value in the <see cref="JsonValue"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see cref="DateTimeOffset.MinValue"/>.</param>
        /// <returns>A <see cref="DateTimeOffset"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static DateTimeOffset GetDateTimeOffsetOrDefault(this JsonValue json, string format, DateTimeOffset defaultValue = default)
            => json.TryGetDateTimeOffset(format, out DateTimeOffset result) ? result : defaultValue;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the format of the <paramref name="value"/> as a JSON value. This parameter is optional.
        /// <br/>Default value: <see cref="JsonDateTimeFormat.Auto"/>, which applies <see cref="JsonDateTimeFormat.Iso8601JavaScript"/> if <paramref name="asString"/> is <see langword="true"/>,
        /// or <see cref="JsonDateTimeFormat.UnixMilliseconds"/> if <paramref name="asString"/> is <see langword="false"/>.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; or <see langword="false"/> to convert it to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.Number"/>&#160;<see cref="JsonValue.Type"/>, which is not applicable for all <paramref name="format"/>s. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/> is not one of the defined values.</exception>
        /// <exception cref="ArgumentException"><paramref name="asString"/> is <see langword="false"/> but <paramref name="format"/> represents a string-only format.</exception>
        public static JsonValue ToJson(this DateTimeOffset value, JsonDateTimeFormat format = JsonDateTimeFormat.Auto, bool asString = true)
        {
            if ((uint)format > (uint)JsonDateTimeFormat.MicrosoftLegacy)
                Throw.ArgumentOutOfRangeException(PublicResources.EnumOutOfRange(format), nameof(format));
            if (!asString && format > JsonDateTimeFormat.Ticks)
                Throw.ArgumentException(Res.DateTimeFormatIsStringOnly(format), nameof(format));

            if (format == JsonDateTimeFormat.Auto)
                format = asString ? JsonDateTimeFormat.Iso8601JavaScript : JsonDateTimeFormat.UnixMilliseconds;

            string formattedValue = format switch
            {
                JsonDateTimeFormat.Iso8601JavaScript => value.UtcDateTime.ToString(DateTimeExtensions.Iso8601JavaScriptFormat, DateTimeFormatInfo.InvariantInfo),
                JsonDateTimeFormat.UnixMilliseconds => value.UtcDateTime.ToUnixMilliseconds().ToString(NumberFormatInfo.InvariantInfo),
                JsonDateTimeFormat.UnixSeconds => value.UtcDateTime.ToUnixSeconds().ToString(NumberFormatInfo.InvariantInfo),
                JsonDateTimeFormat.UnixSecondsFloat => value.UtcDateTime.ToUnixSecondsFloat().ToString("F3", NumberFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Ticks => value.UtcTicks.ToString(NumberFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Iso8601Roundtrip => value.ToString("O", DateTimeFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Iso8601Utc => value.UtcDateTime.ToString("O", DateTimeFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Iso8601Local => value.ToString("O", DateTimeFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Iso8601Date => value.ToString(DateTimeExtensions.Iso8601DateFormat, DateTimeFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Iso8601Minutes => value.ToString(DateTimeExtensions.Iso8601MinutesFormat, DateTimeFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Iso8601Seconds => value.ToString(DateTimeExtensions.Iso8601SecondsFormat, DateTimeFormatInfo.InvariantInfo),
                JsonDateTimeFormat.Iso8601Milliseconds => value.ToString(DateTimeExtensions.Iso8601MillisecondsFormat, DateTimeFormatInfo.InvariantInfo),
                /*JsonDateTimeFormat.MicrosoftLegacy*/_ => value.ToMicrosoftJsonDate(),
            };

            return new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, formattedValue);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the format of the <paramref name="value"/> as a JSON value. This parameter is optional.
        /// <br/>Default value: <see cref="JsonDateTimeFormat.Auto"/>, which applies <see cref="JsonDateTimeFormat.Iso8601JavaScript"/> if <paramref name="asString"/> is <see langword="true"/>,
        /// or <see cref="JsonDateTimeFormat.UnixMilliseconds"/> if <paramref name="asString"/> is <see langword="false"/>.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; or <see langword="false"/> to convert it to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.Number"/>&#160;<see cref="JsonValue.Type"/>, which is not applicable for all <paramref name="format"/>s. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/> is not one of the defined values.</exception>
        /// <exception cref="ArgumentException"><paramref name="asString"/> is <see langword="false"/> but <paramref name="format"/> represents a string-only format.</exception>
        public static JsonValue ToJson(this DateTimeOffset? value, JsonDateTimeFormat format = JsonDateTimeFormat.Auto, bool asString = true)
            => value?.ToJson(format, asString) ?? JsonValue.Null;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the exact format of the <paramref name="value"/> as a JSON value.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        public static JsonValue ToJson(this DateTimeOffset value, string format)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, format CAN be null but MUST NOT be
            => value.ToString(format ?? Throw.ArgumentNullException<string>(nameof(format)), DateTimeFormatInfo.InvariantInfo);

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the exact format of the <paramref name="value"/> as a JSON value.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        public static JsonValue ToJson(this DateTimeOffset? value, string format) => value?.ToJson(format) ?? JsonValue.Null;

        #endregion

        #region TimeSpan

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="TimeSpan"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// The actual format is attempted to be auto detected. If you know exact format use the
        /// <see cref="TryGetTimeSpan(JsonValue, JsonTimeFormat, out TimeSpan, JsonValueType)"/> overload instead.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="TimeSpan"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="TimeSpan.Zero"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetTimeSpan(this JsonValue json, out TimeSpan value, JsonValueType expectedType = default)
            => json.TryGetTimeSpan(JsonTimeFormat.Auto, out value, expectedType);

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="TimeSpan"/> value using the specified <paramref name="format"/>
        /// if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="TimeSpan"/>.</param>
        /// <param name="format">A <see cref="JsonTimeFormat"/> value that specifies the format of the <see cref="TimeSpan"/> value in the <see cref="JsonValue"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="TimeSpan.Zero"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetTimeSpan(this JsonValue json, JsonTimeFormat format, out TimeSpan value, JsonValueType expectedType = default)
        {
            if ((uint)format > (uint)JsonTimeFormat.Text)
                Throw.ArgumentOutOfRangeException(PublicResources.EnumOutOfRange(format), nameof(format));
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
                return s.TryParseTimeSpan(format, json.Type == JsonValueType.Number, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="TimeSpan"/> value using the specified <paramref name="format"/>
        /// if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/>
        /// property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="TimeSpan"/>; otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="TimeSpan"/>.</param>
        /// <param name="format">A <see cref="JsonTimeFormat"/> value that specifies the format of the <see cref="TimeSpan"/> value in the <see cref="JsonValue"/>. This parameter is optional.
        /// <br/>Default value: <see cref="JsonDateTimeFormat.Auto"/>, which attempts to auto detect the format.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="TimeSpan"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static TimeSpan? AsTimeSpan(this JsonValue json, JsonTimeFormat format = JsonTimeFormat.Auto, JsonValueType expectedType = default)
            => json.TryGetTimeSpan(format, out TimeSpan result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="TimeSpan"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="TimeSpan"/>;
        /// otherwise, returns <paramref name="defaultValue"/>. The actual format is attempted to be auto detected. If you know exact format use the
        /// <see cref="GetTimeSpanOrDefault(JsonValue, JsonTimeFormat, TimeSpan, JsonValueType)"/> overload instead.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="TimeSpan"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see cref="TimeSpan.Zero"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="TimeSpan"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static TimeSpan GetTimeSpanOrDefault(this JsonValue json, TimeSpan defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetTimeSpan(JsonTimeFormat.Auto, out TimeSpan result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="TimeSpan"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="TimeSpan"/>;
        /// otherwise, returns <see cref="TimeSpan.Zero">TimeSpan.Zero</see>, which is the default value of <see cref="TimeSpan"/>.
        /// The actual format is attempted to be auto detected. If you know exact format use the
        /// <see cref="GetTimeSpanOrDefault(JsonValue, JsonTimeFormat, TimeSpan, JsonValueType)"/> overload instead.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="TimeSpan"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="TimeSpan"/> value if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="TimeSpan.Zero">TimeSpan.Zero</see>, which is the default value of <see cref="TimeSpan"/>.</returns>
        public static TimeSpan GetTimeSpanOrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetTimeSpan(JsonTimeFormat.Auto, out TimeSpan result, expectedType) ? result : default;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="TimeSpan"/> value using the specified <paramref name="format"/> if <paramref name="expectedType"/>
        /// is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be
        /// converted to <see cref="TimeSpan"/>; otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="TimeSpan"/>.</param>
        /// <param name="format">A <see cref="JsonTimeFormat"/> value that specifies the format of the <see cref="TimeSpan"/> value in the <see cref="JsonValue"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see cref="TimeSpan.Zero"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="TimeSpan"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static TimeSpan GetTimeSpanOrDefault(this JsonValue json, JsonTimeFormat format, TimeSpan defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetTimeSpan(format, out TimeSpan result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="TimeSpan"/> value using the specified <paramref name="format"/> if <paramref name="expectedType"/>
        /// is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be
        /// converted to <see cref="TimeSpan"/>; otherwise, returns <see cref="TimeSpan.Zero">TimeSpan.Zero</see>, which is the default value of <see cref="TimeSpan"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="TimeSpan"/>.</param>
        /// <param name="format">A <see cref="JsonTimeFormat"/> value that specifies the format of the <see cref="TimeSpan"/> value in the <see cref="JsonValue"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="TimeSpan"/> value if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="TimeSpan.Zero">TimeSpan.Zero</see>, which is the default value of <see cref="TimeSpan"/>.</returns>
        public static TimeSpan GetTimeSpanOrDefault(this JsonValue json, JsonTimeFormat format, JsonValueType expectedType)
            => json.TryGetTimeSpan(format, out TimeSpan result, expectedType) ? result : default;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the format of the <paramref name="value"/> as a JSON value. This parameter is optional.
        /// <br/>Default value: <see cref="JsonTimeFormat.Auto"/>, which applies <see cref="JsonTimeFormat.Text"/> if <paramref name="asString"/> is <see langword="true"/>,
        /// or <see cref="JsonTimeFormat.Milliseconds"/> if <paramref name="asString"/> is <see langword="false"/>.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; or <see langword="false"/> to convert it to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.Number"/>&#160;<see cref="JsonValue.Type"/>, which is not applicable for all <paramref name="format"/>s. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/> is not one of the defined values.</exception>
        /// <exception cref="ArgumentException"><paramref name="asString"/> is <see langword="false"/> but <paramref name="format"/> represents a string-only format.</exception>
        public static JsonValue ToJson(this TimeSpan value, JsonTimeFormat format = JsonTimeFormat.Auto, bool asString = true)
        {
            if ((uint)format > (uint)JsonTimeFormat.Text)
                Throw.ArgumentOutOfRangeException(PublicResources.EnumOutOfRange(format), nameof(format));
            if (!asString && format > JsonTimeFormat.Ticks)
                Throw.ArgumentException(Res.TimeSpanFormatIsStringOnly(format), nameof(format));

            if (format == JsonTimeFormat.Auto)
                format = asString ? JsonTimeFormat.Text : JsonTimeFormat.Milliseconds;

            string formattedValue = format switch
            {
                JsonTimeFormat.Milliseconds => (value.Ticks / TimeSpan.TicksPerMillisecond).ToString(NumberFormatInfo.InvariantInfo),
                JsonTimeFormat.Ticks => value.Ticks.ToString(NumberFormatInfo.InvariantInfo),
                _ /*JsonTimeFormat.Text*/ =>
#if NET35
                    value.ToString(),
#else
                    value.ToString("c", DateTimeFormatInfo.InvariantInfo),
#endif
            };

            return new JsonValue(asString ? JsonValueType.String : JsonValueType.Number, formattedValue);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the format of the <paramref name="value"/> as a JSON value. This parameter is optional.
        /// <br/>Default value: <see cref="JsonTimeFormat.Auto"/>, which applies <see cref="JsonTimeFormat.Text"/> if <paramref name="asString"/> is <see langword="true"/>,
        /// or <see cref="JsonTimeFormat.Milliseconds"/> if <paramref name="asString"/> is <see langword="false"/>.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; or <see langword="false"/> to convert it to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.Number"/>&#160;<see cref="JsonValue.Type"/>, which is not applicable for all <paramref name="format"/>s. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/> is not one of the defined values.</exception>
        /// <exception cref="ArgumentException"><paramref name="asString"/> is <see langword="false"/> but <paramref name="format"/> represents a string-only format.</exception>
        public static JsonValue ToJson(this TimeSpan? value, JsonTimeFormat format = JsonTimeFormat.Auto, bool asString = true)
            => value?.ToJson(format, asString) ?? JsonValue.Null;

        #endregion

        #region DateOnly
#if NET6_0_OR_GREATER

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="DateOnly"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// The actual format is attempted to be auto detected. If you know exact format use the
        /// other <see cref="O:KGySoft.Json.JsonValueExtensions.TryGetDateOnly">TryGetDateOnly</see> overloads.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateOnly"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="DateOnly.MinValue"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetDateOnly(this JsonValue json, out DateOnly value, JsonValueType expectedType = default)
            => json.TryGetDateOnly(JsonDateTimeFormat.Auto, out value, expectedType);

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="DateOnly"/> value using the specified <paramref name="format"/>
        /// if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateOnly"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date in the <see cref="JsonValue"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="DateOnly.MinValue"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetDateOnly(this JsonValue json, JsonDateTimeFormat format, out DateOnly value, JsonValueType expectedType = default)
        {
            if ((uint)format > (uint)JsonDateTimeFormat.MicrosoftLegacy)
                Throw.ArgumentOutOfRangeException(PublicResources.EnumOutOfRange(format), nameof(format));
            if (expectedType != JsonValueType.Undefined && json.Type != expectedType || json.AsStringInternal is not string s)
            {
                value = default;
                return false;
            }

            if (format == JsonDateTimeFormat.Iso8601Roundtrip)
                format = JsonDateTimeFormat.Iso8601Date;

            // Parsing as DateTime so allowing possible timezone information, too
            if (s.TryParseDateTime(format, expectedType == JsonValueType.Number, out DateTime dateTime))
            {
                value = DateOnly.FromDateTime(dateTime);
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="DateOnly"/> value using the specified <paramref name="format"/>
        /// if <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter is <see cref="JsonValueType.String"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateOnly"/>.</param>
        /// <param name="format">Specifies the exact format of the date in the <see cref="JsonValue"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="DateOnly.MinValue"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetDateOnly(this JsonValue json, string format, out DateOnly value)
        {
            if (format == null!)
                Throw.ArgumentNullException(nameof(format));
            if (json.AsString is not string s)
            {
                value = default;
                return false;
            }

            // Parsing as DateTime so allowing possible timezone information, too
            if (DateTime.TryParseExact(s, format, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.RoundtripKind, out DateTime dateTime))
            {
                value = DateOnly.FromDateTime(dateTime);
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateOnly"/> value using the specified <paramref name="format"/>
        /// if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/>
        /// property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="DateOnly"/>; otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateOnly"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date-time value in the <see cref="JsonValue"/>. This parameter is optional.
        /// <br/>Default value: <see cref="JsonDateTimeFormat.Auto"/>, which attempts to auto detect the format.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateOnly"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static DateOnly? AsDateOnly(this JsonValue json, JsonDateTimeFormat format = JsonDateTimeFormat.Auto, JsonValueType expectedType = default)
            => json.TryGetDateOnly(format, out DateOnly result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateOnly"/> value using the specified <paramref name="format"/>
        /// if <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter is <see cref="JsonValueType.String"/>
        /// and it can be converted to <see cref="DateOnly"/>; otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateOnly"/>.</param>
        /// <param name="format">Specifies the exact format of the date-time value in the <see cref="JsonValue"/>.</param>
        /// <returns>A <see cref="DateOnly"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static DateOnly? AsDateOnly(this JsonValue json, string format)
            => json.TryGetDateOnly(format, out DateOnly result) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateOnly"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="DateOnly"/>;
        /// otherwise, returns <paramref name="defaultValue"/>. The actual format is attempted to be auto detected. If you know exact format use the
        /// other <see cref="O:KGySoft.Json.JsonValueExtensions.GetDateOnlyOrDefault">GetDateOnlyOrDefault</see> overloads.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateOnly"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see cref="DateOnly.MinValue"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateOnly"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static DateOnly GetDateOnlyOrDefault(this JsonValue json, DateOnly defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetDateOnly(JsonDateTimeFormat.Auto, out DateOnly result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateOnly"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="DateOnly"/>;
        /// otherwise, returns <see cref="DateOnly.MinValue">DateOnly.MinValue</see>, which is the default value of <see cref="DateOnly"/>.
        /// The actual format is attempted to be auto detected. If you know exact format use the
        /// other <see cref="O:KGySoft.Json.JsonValueExtensions.GetDateOnlyOrDefault">GetDateOnlyOrDefault</see> overloads.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateOnly"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="DateTime"/> value if <paramref name="json"/> could be converted; otherwise, <see cref="DateTime.MinValue">DateTime.MinValue</see>,
        /// which is the default value of <see cref="DateOnly"/>.</returns>
        public static DateOnly GetDateOnlyOrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetDateOnly(JsonDateTimeFormat.Auto, out DateOnly result, expectedType) ? result : default;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateOnly"/> value using the specified <paramref name="format"/> if <paramref name="expectedType"/>
        /// is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be
        /// converted to <see cref="DateOnly"/>; otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateOnly"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date in the <see cref="JsonValue"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see cref="DateOnly.MinValue"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="DateOnly"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static DateOnly GetDateOnlyOrDefault(this JsonValue json, JsonDateTimeFormat format, DateOnly defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetDateOnly(format, out DateOnly result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateOnly"/> value using the specified <paramref name="format"/> if <paramref name="expectedType"/>
        /// is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be
        /// converted to <see cref="DateOnly"/>; otherwise, returns <see cref="DateOnly.MinValue">DateOnly.MinValue</see>, which is the default value of <see cref="DateOnly"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateOnly"/>.</param>
        /// <param name="format">A <see cref="JsonDateTimeFormat"/> value that specifies the format of the date in the <see cref="JsonValue"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="DateOnly"/> value if <paramref name="json"/> could be converted; otherwise, <see cref="DateOnly.MinValue">DateOnly.MinValue</see>,
        /// which is the default value of <see cref="DateOnly"/>.</returns>
        public static DateOnly GetDateOnlyOrDefault(this JsonValue json, JsonDateTimeFormat format, JsonValueType expectedType)
            => json.TryGetDateOnly(format, out DateOnly result, expectedType) ? result : default;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="DateOnly"/> value using the specified <paramref name="format"/>
        /// if <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter is <see cref="JsonValueType.String"/> and it can be
        /// converted to <see cref="DateOnly"/>; otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="DateOnly"/>.</param>
        /// <param name="format">Specifies the exact format of the date in the <see cref="JsonValue"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see cref="DateOnly.MinValue"/>.</param>
        /// <returns>A <see cref="DateOnly"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static DateOnly GetDateOnlyOrDefault(this JsonValue json, string format, DateOnly defaultValue = default)
            => json.TryGetDateOnly(format, out DateOnly result) ? result : defaultValue;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the format of the <paramref name="value"/> as a JSON value. This parameter is optional.
        /// <br/>Default value: <see cref="JsonDateTimeFormat.Auto"/>, which applies <see cref="JsonDateTimeFormat.Iso8601Date"/> if <paramref name="asString"/> is <see langword="true"/>,
        /// or <see cref="JsonDateTimeFormat.UnixMilliseconds"/> if <paramref name="asString"/> is <see langword="false"/>.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; or <see langword="false"/> to convert it to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.Number"/>&#160;<see cref="JsonValue.Type"/>, which is not applicable for all <paramref name="format"/>s. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/> is not one of the defined values.</exception>
        /// <exception cref="ArgumentException"><paramref name="asString"/> is <see langword="false"/> but <paramref name="format"/> represents a string-only format.</exception>
        public static JsonValue ToJson(this DateOnly value, JsonDateTimeFormat format = JsonDateTimeFormat.Auto, bool asString = true)
        {
            if (format == JsonDateTimeFormat.Auto)
                format = asString ? JsonDateTimeFormat.Iso8601Date : JsonDateTimeFormat.Iso8601Milliseconds;
            else if (format == JsonDateTimeFormat.Iso8601Roundtrip)
                format = JsonDateTimeFormat.Iso8601Date;
            return value.ToDateTime(default).ToJson(format, asString);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the format of the <paramref name="value"/> as a JSON value. This parameter is optional.
        /// <br/>Default value: <see cref="JsonDateTimeFormat.Auto"/>, which applies <see cref="JsonDateTimeFormat.Iso8601JavaScript"/> if <paramref name="asString"/> is <see langword="true"/>,
        /// or <see cref="JsonDateTimeFormat.UnixMilliseconds"/> if <paramref name="asString"/> is <see langword="false"/>.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; or <see langword="false"/> to convert it to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.Number"/>&#160;<see cref="JsonValue.Type"/>, which is not applicable for all <paramref name="format"/>s. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/> is not one of the defined values.</exception>
        /// <exception cref="ArgumentException"><paramref name="asString"/> is <see langword="false"/> but <paramref name="format"/> represents a string-only format.</exception>
        public static JsonValue ToJson(this DateOnly? value, JsonDateTimeFormat format = JsonDateTimeFormat.Auto, bool asString = true)
            => value?.ToJson(format, asString) ?? JsonValue.Null;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the exact format of the <paramref name="value"/> as a JSON value.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        public static JsonValue ToJson(this DateOnly value, string format)
            // Formatting as DateTime to be compatible with parsing that allows also zero time parts
            => value.ToDateTime(default).ToJson(format);

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the exact format of the <paramref name="value"/> as a JSON value.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="FormatException"><paramref name="format"/> is invalid.</exception>
        public static JsonValue ToJson(this DateOnly? value, string format) => value?.ToJson(format) ?? JsonValue.Null;

#endif
        #endregion

        #region TimeOnly
#if NET6_0_OR_GREATER

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="TimeOnly"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// The actual format is attempted to be auto detected. If you know exact format use the
        /// <see cref="TryGetTimeOnly(JsonValue, JsonTimeFormat, out TimeOnly, JsonValueType)"/> overload instead.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="TimeOnly"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="TimeOnly.MinValue"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetTimeOnly(this JsonValue json, out TimeOnly value, JsonValueType expectedType = default)
            => json.TryGetTimeOnly(JsonTimeFormat.Auto, out value, expectedType);

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="TimeOnly"/> value using the specified <paramref name="format"/>
        /// if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="TimeOnly"/>.</param>
        /// <param name="format">A <see cref="JsonTimeFormat"/> value that specifies the format of the <see cref="TimeOnly"/> value in the <see cref="JsonValue"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="TimeSpan.Zero"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetTimeOnly(this JsonValue json, JsonTimeFormat format, out TimeOnly value, JsonValueType expectedType = default)
        {
            if ((uint)format > (uint)JsonTimeFormat.Text)
                Throw.ArgumentOutOfRangeException(PublicResources.EnumOutOfRange(format), nameof(format));
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && json.AsStringInternal is string s)
            {
                if (s.TryParseTimeSpan(format, json.Type == JsonValueType.Number, out TimeSpan timeSpan, true))
                {
                    value = TimeOnly.FromTimeSpan(timeSpan);
                    return true;
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="TimeOnly"/> value using the specified <paramref name="format"/>
        /// if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/>
        /// property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="TimeOnly"/>; otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="TimeOnly"/>.</param>
        /// <param name="format">A <see cref="JsonTimeFormat"/> value that specifies the format of the <see cref="TimeOnly"/> value in the <see cref="JsonValue"/>. This parameter is optional.
        /// <br/>Default value: <see cref="JsonDateTimeFormat.Auto"/>, which attempts to auto detect the format.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="TimeOnly"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static TimeOnly? AsTimeOnly(this JsonValue json, JsonTimeFormat format = JsonTimeFormat.Auto, JsonValueType expectedType = default)
            => json.TryGetTimeOnly(format, out TimeOnly result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="TimeOnly"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="TimeOnly"/>;
        /// otherwise, returns <paramref name="defaultValue"/>. The actual format is attempted to be auto detected. If you know exact format use the
        /// <see cref="GetTimeOnlyOrDefault(JsonValue, JsonTimeFormat, TimeOnly, JsonValueType)"/> overload instead.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="TimeOnly"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see cref="TimeOnly.MinValue"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="TimeOnly"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static TimeOnly GetTimeOnlyOrDefault(this JsonValue json, TimeOnly defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetTimeOnly(JsonTimeFormat.Auto, out TimeOnly result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="TimeOnly"/> value if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <see cref="TimeOnly"/>;
        /// otherwise, returns <see cref="TimeOnly.MinValue">TimeOnly.MinValue</see>, which is the default value of <see cref="TimeOnly"/>.
        /// The actual format is attempted to be auto detected. If you know exact format use the
        /// <see cref="GetTimeOnlyOrDefault(JsonValue, JsonTimeFormat, TimeOnly, JsonValueType)"/> overload instead.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="TimeOnly"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="TimeOnly"/> value if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="TimeOnly.MinValue">TimeOnly.MinValue</see>, which is the default value of <see cref="TimeOnly"/>.</returns>
        public static TimeOnly GetTimeOnlyOrDefault(this JsonValue json, JsonValueType expectedType)
            => json.TryGetTimeOnly(JsonTimeFormat.Auto, out TimeOnly result, expectedType) ? result : default;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="TimeOnly"/> value using the specified <paramref name="format"/> if <paramref name="expectedType"/>
        /// is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be
        /// converted to <see cref="TimeOnly"/>; otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="TimeOnly"/>.</param>
        /// <param name="format">A <see cref="JsonTimeFormat"/> value that specifies the format of the <see cref="TimeOnly"/> value in the <see cref="JsonValue"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see cref="TimeOnly.MinValue"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <see cref="TimeOnly"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static TimeOnly GetTimeOnlyOrDefault(this JsonValue json, JsonTimeFormat format, TimeOnly defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetTimeOnly(format, out TimeOnly result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="TimeOnly"/> value using the specified <paramref name="format"/> if <paramref name="expectedType"/>
        /// is <see cref="JsonValueType.Undefined"/> or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be
        /// converted to <see cref="TimeOnly"/>; otherwise, returns <see cref="TimeOnly.MinValue">TimeOnly.MinValue</see>, which is the default value of <see cref="TimeOnly"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="TimeOnly"/>.</param>
        /// <param name="format">A <see cref="JsonTimeFormat"/> value that specifies the format of the <see cref="TimeOnly"/> value in the <see cref="JsonValue"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type.</param>
        /// <returns>A <see cref="TimeOnly"/> value if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="TimeOnly.MinValue">TimeOnly.MinValue</see>, which is the default value of <see cref="TimeOnly"/>.</returns>
        public static TimeOnly GetTimeOnlyOrDefault(this JsonValue json, JsonTimeFormat format, JsonValueType expectedType)
            => json.TryGetTimeOnly(format, out TimeOnly result, expectedType) ? result : default;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the format of the <paramref name="value"/> as a JSON value. This parameter is optional.
        /// <br/>Default value: <see cref="JsonTimeFormat.Auto"/>, which applies <see cref="JsonTimeFormat.Text"/> if <paramref name="asString"/> is <see langword="true"/>,
        /// or <see cref="JsonTimeFormat.Milliseconds"/> if <paramref name="asString"/> is <see langword="false"/>.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; or <see langword="false"/> to convert it to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.Number"/>&#160;<see cref="JsonValue.Type"/>, which is not applicable for all <paramref name="format"/>s. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/> is not one of the defined values.</exception>
        /// <exception cref="ArgumentException"><paramref name="asString"/> is <see langword="false"/> but <paramref name="format"/> represents a string-only format.</exception>
        public static JsonValue ToJson(this TimeOnly value, JsonTimeFormat format = JsonTimeFormat.Auto, bool asString = true)
            => value.ToTimeSpan().ToJson(format, asString);

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the format of the <paramref name="value"/> as a JSON value. This parameter is optional.
        /// <br/>Default value: <see cref="JsonTimeFormat.Auto"/>, which applies <see cref="JsonTimeFormat.Text"/> if <paramref name="asString"/> is <see langword="true"/>,
        /// or <see cref="JsonTimeFormat.Milliseconds"/> if <paramref name="asString"/> is <see langword="false"/>.</param>
        /// <param name="asString"><see langword="true"/> to convert the <paramref name="value"/> to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>; or <see langword="false"/> to convert it to a <see cref="JsonValue"/>
        /// with <see cref="JsonValueType.Number"/>&#160;<see cref="JsonValue.Type"/>, which is not applicable for all <paramref name="format"/>s. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="format"/> is not one of the defined values.</exception>
        /// <exception cref="ArgumentException"><paramref name="asString"/> is <see langword="false"/> but <paramref name="format"/> represents a string-only format.</exception>
        public static JsonValue ToJson(this TimeOnly? value, JsonTimeFormat format = JsonTimeFormat.Auto, bool asString = true)
            => value?.ToJson(format, asString) ?? JsonValue.Null;

#endif
        #endregion

        #region Guid

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as a <see cref="Guid"/> value
        /// if <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter is <see cref="JsonValueType.String"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="Guid"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, <see cref="Guid.Empty"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetGuid(this JsonValue json, out Guid value)
        {
#if NET35
            if (json.AsString is string { Length: 36 } s && s[8] == '-')
            {
                try
                {
                    value = new Guid(s);
                    return true;
                }
                catch (FormatException)
                {
                }
            }
#else
            if (json.AsString is string s)
                return Guid.TryParseExact(s, "D", out value); 
#endif
            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="Guid"/> value
        /// if <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter is <see cref="JsonValueType.String"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="Guid"/>.</param>
        /// <returns>A <see cref="Guid"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static Guid? AsGuid(this JsonValue json) => json.TryGetGuid(out Guid result) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as a <see cref="Guid"/> value
        /// if <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter is <see cref="JsonValueType.String"/>
        /// and it can be converted to <see cref="Guid"/>; otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <see cref="Guid"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see cref="Guid.Empty"/>.</param>
        /// <returns>A <see cref="Guid"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static Guid GetGuidOrDefault(this JsonValue json, Guid defaultValue = default) => json.TryGetGuid(out Guid result) ? result : defaultValue;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this Guid value) => value.ToString("D");

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson(this Guid? value) => value?.ToJson() ?? JsonValue.Null;

        #endregion

        #endregion
    }
}