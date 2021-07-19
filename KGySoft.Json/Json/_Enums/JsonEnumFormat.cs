#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonEnumFormat.cs
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
        /// Assuming that enums are already use Pascal casing as per .NET conventions this formatting preserves the original format.
        /// When using the <see cref="JsonValueExtensions.ToJson{TEnum}(TEnum, JsonEnumFormat, string)"/> method it adjusts only the first character if that is not an upper case one.
        /// </summary>
        PascalCase,

        /// <summary>
        /// Represents camel casing, eg. <c>enumValue</c>.
        /// Assuming that enums are already use Pascal casing as per .NET conventions this formatting adjusts the first character only.
        /// When using the <see cref="JsonValueExtensions.ToJson{TEnum}(TEnum, JsonEnumFormat, string)"/> method it adjusts only the first character if that is not a lower case one.
        /// </summary>
        CamelCase,

        /// <summary>
        /// Represents lower casing, eg. <c>enumvalue</c>.
        /// </summary>
        LowerCase,

        /// <summary>
        /// Represents upper casing, eg. <c>ENUMVALUE</c>.
        /// </summary>
        UpperCase,

        /// <summary>
        /// Represents lower casing with underscores, eg. <c>enum_value</c>.
        /// </summary>
        LowerCaseWithUnderscores,

        /// <summary>
        /// Represents lower casing with underscores, eg. <c>ENUM_VALUE</c>.
        /// </summary>
        UpperCaseWithUnderscores,

        /// <summary>
        /// Represents lower casing with hyphens, eg. <c>enum-value</c>.
        /// </summary>
        LowerCaseWithHyphens,

        /// <summary>
        /// Represents lower casing with hyphens, eg. <c>ENUM-VALUE</c>.
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
