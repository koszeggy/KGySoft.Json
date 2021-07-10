#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonValueExtensions.cs
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

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
        public static bool? AsBoolean(this in JsonValue json, JsonValueType expectedType = default)
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
        public static bool TryGetBoolean(this in JsonValue json, out bool value, JsonValueType expectedType = default)
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
        public static bool GetBooleanOrDefault(this in JsonValue json, bool defaultValue = default, JsonValueType expectedType = default)
            => json.AsBoolean(expectedType) ?? defaultValue;

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
        public static bool TryGetByte(this in JsonValue json, out byte value, JsonValueType expectedType = default)
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
        public static byte? AsByte(this in JsonValue json, JsonValueType expectedType = default)
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
        public static byte GetByteOrDefault(this in JsonValue json, byte defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetByte(out byte result, expectedType) ? result : defaultValue;

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
        public static bool TryGetSByte(this in JsonValue json, out sbyte value, JsonValueType expectedType = default)
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
        public static sbyte? AsSByte(this in JsonValue json, JsonValueType expectedType = default)
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
        public static sbyte GetSByteOrDefault(this in JsonValue json, sbyte defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetSByte(out sbyte result, expectedType) ? result : defaultValue;

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
        public static bool TryGetInt16(this in JsonValue json, out short value, JsonValueType expectedType = default)
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
        public static short? AsInt16(this in JsonValue json, JsonValueType expectedType = default)
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
        public static short GetInt16OrDefault(this in JsonValue json, short defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetInt16(out short result, expectedType) ? result : defaultValue;

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
        public static bool TryGetUInt16(this in JsonValue json, out ushort value, JsonValueType expectedType = default)
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
        public static ushort? AsUInt16(this in JsonValue json, JsonValueType expectedType = default)
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
        public static ushort GetUInt16OrDefault(this in JsonValue json, ushort defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetUInt16(out ushort result, expectedType) ? result : defaultValue;

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
        public static bool TryGetInt32(this in JsonValue json, out int value, JsonValueType expectedType = default)
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
        public static int? AsInt32(this in JsonValue json, JsonValueType expectedType = default)
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
        public static int GetInt32OrDefault(this in JsonValue json, int defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetInt32(out int result, expectedType) ? result : defaultValue;

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
        public static bool TryGetUInt32(this in JsonValue json, out uint value, JsonValueType expectedType = default)
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
        public static uint? AsUInt32(this in JsonValue json, JsonValueType expectedType = default)
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
        public static uint GetUInt32OrDefault(this in JsonValue json, uint defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetUInt32(out uint result, expectedType) ? result : defaultValue;

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
        public static bool TryGetInt64(this in JsonValue json, out long value, JsonValueType expectedType = default)
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
        public static long? AsInt64(this in JsonValue json, JsonValueType expectedType = default)
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
        public static long GetInt64OrDefault(this in JsonValue json, long defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetInt64(out long result, expectedType) ? result : defaultValue;

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
        public static bool TryGetUInt64(this in JsonValue json, out ulong value, JsonValueType expectedType = default)
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
        public static ulong? AsUInt64(this in JsonValue json, JsonValueType expectedType = default)
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
        public static ulong GetUInt64OrDefault(this in JsonValue json, ulong defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetUInt64(out ulong result, expectedType) ? result : defaultValue;

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
        public static bool TryGetSingle(this in JsonValue json, out float value, JsonValueType expectedType = default)
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
        public static float? AsSingle(this in JsonValue json, JsonValueType expectedType = default)
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
        public static float GetSingleOrDefault(this in JsonValue json, float defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetSingle(out float result, expectedType) ? result : defaultValue;

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
        public static bool TryGetDouble(this in JsonValue json, out double value, JsonValueType expectedType = default)
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
        public static double? AsDouble(this in JsonValue json, JsonValueType expectedType = default)
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
        public static double GetDoubleOrDefault(this in JsonValue json, double defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetDouble(out double result, expectedType) ? result : defaultValue;

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
        public static bool TryGetDecimal(this in JsonValue json, out decimal value, JsonValueType expectedType = default)
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
        public static decimal? AsDecimal(this in JsonValue json, JsonValueType expectedType = default)
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
        public static decimal GetDecimalOrDefault(this in JsonValue json, decimal defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetDecimal(out decimal result, expectedType) ? result : defaultValue;

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
        public static bool TryGetString(this in JsonValue json, [MaybeNullWhen(false)]out string? value, JsonValueType expectedType = default, bool allowNullIfStringIsExpected = false)
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
        public static string? AsString(this in JsonValue json, JsonValueType expectedType = default)
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
        public static string? GetStringOrDefault(this in JsonValue json, string? defaultValue = default, JsonValueType expectedType = default)
            => json.TryGetString(out string? result, expectedType) ? result : defaultValue;

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
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, the default value of <typeparamref name="TEnum"/>. This parameter is passed uninitialized.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetEnum<TEnum>(this in JsonValue json, out TEnum value, JsonValueType expectedType = default)
            where TEnum : struct, Enum
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && !json.IsNull && json.AsStringInternal is string s)
                return Enum.TryParse(s, out value);

            value = default;
            return false;
        }

        /// <summary>
        /// Tries to get the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="ignoreFormat"><see langword="true"/> to remove underscores or hyphens, and ignore case when parsing the value; otherwise, <see langword="false"/>.</param>
        /// <param name="value">When this method returns, the result of the conversion, if <paramref name="json"/> could be converted;
        /// otherwise, the default value of <typeparamref name="TEnum"/>. This parameter is passed uninitialized.</param>
        /// <param name="flagsSeparator">Specifies the separator character if the <paramref name="json"/> value consists of multiple flags. This parameter is optional.
        /// <br/>Default value: <c>,</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns><see langword="true"/> if the specified <see cref="JsonValue"/> could be converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryGetEnum<TEnum>(this in JsonValue json, bool ignoreFormat, out TEnum value, char flagsSeparator = ',', JsonValueType expectedType = default)
            where TEnum : struct, Enum
        {
            if ((expectedType == JsonValueType.Undefined || json.Type == expectedType) && !json.IsNull && json.AsStringInternal is string s)
            {
                if (ignoreFormat && s.IndexOfAny(new[] { '_', '-' }) >= 0)
                {
                    s = s.Replace("_", String.Empty);
                    s = s.Replace("-", String.Empty);
                }

                if (flagsSeparator != ',')
                    s = s.Replace(flagsSeparator, ',');
                return Enum.TryParse(s, ignoreFormat, out value);
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <typeparamref name="TEnum"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <typeparamref name="TEnum"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static TEnum? AsEnum<TEnum>(this in JsonValue json, JsonValueType expectedType = default) where TEnum : struct, Enum
            => json.TryGetEnum(out TEnum result, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <typeparamref name="TEnum"/>;
        /// otherwise, returns <see langword="null"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="ignoreFormat"><see langword="true"/> to remove underscores or hyphens, and ignore case when parsing the value; otherwise, <see langword="false"/>.</param>
        /// <param name="flagsSeparator">Specifies the separator character if the <paramref name="json"/> value consists of multiple flags. This parameter is optional.
        /// <br/>Default value: <c>,</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <typeparamref name="TEnum"/> value if <paramref name="json"/> could be converted; otherwise, <see langword="null"/>.</returns>
        public static TEnum? AsEnum<TEnum>(this in JsonValue json, bool ignoreFormat, char flagsSeparator = ',', JsonValueType expectedType = default) where TEnum : struct, Enum
            => json.TryGetEnum(ignoreFormat, out TEnum result, flagsSeparator, expectedType) ? result : null;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <typeparamref name="TEnum"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <typeparamref name="TEnum"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static TEnum GetEnumOrDefault<TEnum>(this in JsonValue json, TEnum defaultValue = default, JsonValueType expectedType = default) where TEnum : struct, Enum
            => json.TryGetEnum(out TEnum result, expectedType) ? result : defaultValue;

        /// <summary>
        /// Gets the specified <see cref="JsonValue"/> as <typeparamref name="TEnum"/> if <paramref name="expectedType"/> is <see cref="JsonValueType.Undefined"/>
        /// or matches the <see cref="JsonValue.Type"/> property of the specified <paramref name="json"/> parameter and it can be converted to <typeparamref name="TEnum"/>;
        /// otherwise, returns <paramref name="defaultValue"/>.
        /// </summary>
        /// <param name="json">The <see cref="JsonValue"/> to be converted to <typeparamref name="TEnum"/>.</param>
        /// <param name="ignoreFormat"><see langword="true"/> to remove underscores or hyphens, and ignore case when parsing the value; otherwise, <see langword="false"/>.</param>
        /// <param name="defaultValue">The value to be returned if the conversion fails. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <param name="flagsSeparator">Specifies the separator character if the <paramref name="json"/> value consists of multiple flags. This parameter is optional.
        /// <br/>Default value: <c>,</c>.</param>
        /// <param name="expectedType">The expected <see cref="JsonValue.Type"/> of the specified <paramref name="json"/> parameter,
        /// or <see cref="JsonValueType.Undefined"/> to allow any type. This parameter is optional.
        /// <br/>Default value: <see cref="JsonValueType.Undefined"/>.</param>
        /// <returns>A <typeparamref name="TEnum"/> value if <paramref name="json"/> could be converted; otherwise, <paramref name="defaultValue"/>.</returns>
        public static TEnum GetEnumOrDefault<TEnum>(this in JsonValue json, bool ignoreFormat, TEnum defaultValue = default, char flagsSeparator = ',', JsonValueType expectedType = default) where TEnum : struct, Enum
            => json.TryGetEnum(ignoreFormat, out TEnum result, flagsSeparator, expectedType) ? result : defaultValue;

        /// <summary>
        /// Converts the specified <paramref name="value"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="format">Specifies the format of the enum in the JSON value. This parameter is optional.
        /// <br/>Default value: <see cref="JsonEnumFormat.PascalCase"/>.</param>
        /// <param name="flagsSeparator">Specifies the separator if <paramref name="value"/> consists of multiple flags. This parameter is optional.
        /// <br/>Default value: <c>", "</c>.</param>
        /// <returns>A <see cref="JsonValue"/> instance that is the JSON representation of the specified <paramref name="value"/>.</returns>
        public static JsonValue ToJson<TEnum>(this TEnum value, JsonEnumFormat format = JsonEnumFormat.PascalCase, string flagsSeparator = ", ")
            where TEnum : struct, Enum
        {
            #region Local Methods

            static string AdjustString(string value, bool upperCase, char separator)
            {
                List<char> chars = value.ToList();

                chars[0] = upperCase ? Char.ToUpperInvariant(chars[0]) : Char.ToLowerInvariant(chars[0]);
                for (int i = chars.Count - 1; i > 0; i--)
                {
                    // Assuming that the original value was in PascalCase so inserting separators before upper case letters
                    if (Char.IsLower(chars[i]))
                    {
                        if (upperCase)
                            chars[i] = Char.ToUpperInvariant(chars[i]);
                        continue;
                    }

                    if (!upperCase)
                        chars[i] = Char.ToLowerInvariant(chars[i]);
                    chars.Insert(i, separator);
                }

                return new String(chars.ToArray());
            }

            #endregion

            string enumValue;
            switch (format)
            {
                case JsonEnumFormat.PascalCase:
                    enumValue = value.ToString();
                    if (Char.IsLower(enumValue[0]))
                        enumValue = Char.ToUpperInvariant(enumValue[0]) + enumValue.Substring(1);
                    break;
                case JsonEnumFormat.CamelCase:
                    enumValue = value.ToString();
                    if (Char.IsUpper(enumValue[0]))
                        enumValue = Char.ToLowerInvariant(enumValue[0]) + enumValue.Substring(1);
                    break;
                case JsonEnumFormat.LowerCase:
                    enumValue = value.ToString().ToLowerInvariant();
                    break;
                case JsonEnumFormat.UpperCase:
                    enumValue = value.ToString().ToUpperInvariant();
                    break;
                case JsonEnumFormat.LowerCaseWithUnderscores:
                    enumValue = AdjustString(value.ToString(), false, '_');
                    break;
                case JsonEnumFormat.UpperCaseWithUnderscores:
                    enumValue = AdjustString(value.ToString(), true, '_');
                    break;
                case JsonEnumFormat.LowerCaseWithHyphens:
                    enumValue = AdjustString(value.ToString(), false, '-');
                    break;
                case JsonEnumFormat.UpperCaseWithHyphens:
                    enumValue = AdjustString(value.ToString(), true, '-');
                    break;
                case JsonEnumFormat.Number:
                case JsonEnumFormat.NumberAsString:
                    return new JsonValue(format == JsonEnumFormat.Number ? JsonValueType.Number : JsonValueType.String, value.ToString("D"));
                default:
                    Throw.ArgumentOutOfRangeException(nameof(format));
                    return default;
            }

            if (flagsSeparator != ", ")
                enumValue = enumValue.Replace(", ", flagsSeparator);
            return enumValue;
        }

        #endregion

        #endregion
    }
}