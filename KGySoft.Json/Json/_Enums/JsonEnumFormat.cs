﻿namespace KGySoft.Json
{
    /// <summary>
    /// Specifies how .NET enums (assuming conventional C# Pascal casing) are formatted when converted to JSON
    /// by the <see cref="JsonValueExtensions.ToJson{TEnum}(TEnum, JsonEnumFormat, string)"/> extension method.
    /// </summary>
    public enum JsonEnumFormat
    {
        /// <summary>
        /// Represents the Pascal casing, eg. <c>EnumValue</c>.
        /// Assuming that enums are already use Pascal casing as per .NET conventions this formatting preserves the original format.
        /// When using the <see cref="JsonValueExtensions.ToJson{TEnum}(TEnum, JsonEnumFormat, string)"/> method it adjust only the first character if that is not upper case.
        /// </summary>
        PascalCase,

        /// <summary>
        /// Represents the camel casing, eg. <c>enumValue</c>.
        /// Assuming that enums are already use Pascal casing as per .NET conventions this formatting adjusts the first character only.
        /// When using the <see cref="JsonValueExtensions.ToJson{TEnum}(TEnum, JsonEnumFormat, string)"/> method it adjust only the first character if that is not lower case.
        /// </summary>
        CamelCase,

        /// <summary>
        /// Represents the lower casing, eg. <c>enumvalue</c>.
        /// </summary>
        LowerCase,

        /// <summary>
        /// Represents the lower casing, eg. <c>ENUMVALUE</c>.
        /// </summary>
        UpperCase,

        /// <summary>
        /// Represents the lower casing with underscores, eg. <c>enum_value</c>.
        /// </summary>
        LowerCaseWithUnderscores,

        /// <summary>
        /// Represents the lower casing with underscores, eg. <c>ENUM_VALUE</c>.
        /// </summary>
        UpperCaseWithUnderscores,

        /// <summary>
        /// Represents the lower casing with hyphens, eg. <c>enum-value</c>.
        /// </summary>
        LowerCaseWithHyphens,

        /// <summary>
        /// Represents the lower casing with hyphens, eg. <c>ENUM-VALUE</c>.
        /// </summary>
        UpperCaseWithHyphens,

        /// <summary>
        /// Represents the numeric formatting of enum values even if they have named representation.
        /// The result <see cref="JsonValue"/> will have <see cref="JsonValueType.Number"/>&#160;<see cref="JsonValue.Type"/>.
        /// </summary>
        Number,

        /// <summary>
        /// Represents the numeric formatting of enum values even if they have named representation.
        /// The result <see cref="JsonValue"/> will have <see cref="JsonValueType.String"/>&#160;<see cref="JsonValue.Type"/>.
        /// </summary>
        NumberAsString
    }
}