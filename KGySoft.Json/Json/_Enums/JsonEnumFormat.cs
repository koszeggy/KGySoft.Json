﻿#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonEnumFormat.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

namespace KGySoft.Json
{
    /// <summary>
    /// Specifies how .NET enums (assuming conventional C# Pascal casing) are formatted when converted to JSON
    /// by the <see cref="JsonValueExtensions.ToJson{TEnum}(TEnum, JsonEnumFormat, string)"/> extension method.
    /// </summary>
    public enum JsonEnumFormat
    {
        /// <summary>
        /// Represents Pascal casing, eg. <c>EnumValue</c>.
        /// <br/>Assuming that enums already use Pascal casing as per .NET conventions this formatting preserves the original format.
        /// When using the <see cref="JsonValueExtensions.ToJson{TEnum}(TEnum, JsonEnumFormat, string)"/> method it adjusts only the first character if that is not an upper case one.
        /// </summary>
        PascalCase,

        /// <summary>
        /// Represents camel casing, eg. <c>enumValue</c>.
        /// <br/>Assuming that enums use Pascal casing as per .NET conventions this formatting adjusts the first character only.
        /// When using the <see cref="JsonValueExtensions.ToJson{TEnum}(TEnum, JsonEnumFormat, string)"/> method it adjusts only the first character if that is not a lower case one.
        /// </summary>
        CamelCase,

        /// <summary>
        /// Represents lower casing, eg. <c>enumvalue</c>. Possible underscores are not removed from the result.
        /// </summary>
        LowerCase,

        /// <summary>
        /// Represents upper casing, eg. <c>ENUMVALUE</c>. Possible underscores are not removed from the result.
        /// </summary>
        UpperCase,

        /// <summary>
        /// Represents lower casing with underscores, eg. <c>enum_value</c>.
        /// <br/>Assuming that enums use Pascal casing as per .NET conventions this formatting just inserts underscores
        /// before the originally upper case letters, except the first one. If the original value also contains underscores,
        /// then they might be duplicated in the result. For such enums use the <see cref="LowerCase"/> formatting instead.
        /// </summary>
        LowerCaseWithUnderscores,

        /// <summary>
        /// Represents upper casing with underscores, eg. <c>ENUM_VALUE</c>.
        /// <br/>Assuming that enums use Pascal casing as per .NET conventions this formatting just inserts underscores
        /// before the originally upper case letters, except the first one. If the original value also contains underscores,
        /// then they might be duplicated in the result. For such enums use the <see cref="UpperCase"/> formatting instead.
        /// </summary>
        UpperCaseWithUnderscores,

        /// <summary>
        /// Represents lower casing with hyphens, eg. <c>enum-value</c>.
        /// <br/>Assuming that enums use Pascal casing as per .NET conventions this formatting just inserts hyphens
        /// before the originally upper case letters, except the first one. If the original value contains underscores,
        /// then they also will be preserved.
        /// </summary>
        LowerCaseWithHyphens,

        /// <summary>
        /// Represents upper casing with hyphens, eg. <c>ENUM-VALUE</c>.
        /// <br/>Assuming that enums use Pascal casing as per .NET conventions this formatting just inserts hyphens
        /// before the originally upper case letters, except the first one. If the original value contains underscores,
        /// then they also will be preserved.
        /// </summary>
        UpperCaseWithHyphens,

        /// <summary>
        /// Represents numeric formatting of enum values even if they have named representation.
        /// The result <see cref="JsonValue"/> will have <see cref="JsonValueType.Number"/>&#160;<see cref="JsonValue.Type"/>.
        /// </summary>
        Number,

        /// <summary>
        /// Represents numeric formatting of enum values even if they have named representation.
        /// The result <see cref="JsonValue"/> will have <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>.
        /// </summary>
        NumberAsString
    }
}
