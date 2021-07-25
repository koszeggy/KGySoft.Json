#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonValueType.cs
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
    /// Represents the possible values of the <see cref="JsonValue.Type"/> property of a <see cref="JsonValue"/> instance.
    /// </summary>
    public enum JsonValueType
    {
        /// <summary>
        /// The <see cref="JsonValue"/> contains an unknown JSON literal.
        /// Can occur when parsing an invalid JSON or when the <see cref="JsonValue"/> was created by the <see cref="JsonValue.CreateLiteralUnchecked">CreateLiteralUnchecked</see> method.
        /// If the <see cref="JsonValue.Type"/> property of a <see cref="JsonValue"/> instance is <see cref="UnknownLiteral"/>,
        /// then both <see cref="JsonValue.AsLiteral"/> and <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> return the actual value.
        /// </summary>
        UnknownLiteral = -1,

        /// <summary>
        /// Represents the <c>undefined</c> type in JavaScript.
        /// This is the <see cref="JsonValue.Type"/> of a default <see cref="JsonValue"/> instance and also the <see cref="JsonValue.Undefined"/> field.
        /// If the <see cref="JsonValue.Type"/> property of a <see cref="JsonValue"/> instance is <see cref="Undefined"/>,
        /// then <see cref="JsonValue.IsUndefined"/> returns <see langword="true"/>,
        /// and both <see cref="JsonValue.AsLiteral"/> and <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> return <c>undefined</c>.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Represents the <c>null</c> type in JavaScript.
        /// This is the <see cref="JsonValue.Type"/> of the <see cref="JsonValue.Null"/> field.
        /// If the <see cref="JsonValue.Type"/> property of a <see cref="JsonValue"/> instance is <see cref="Null"/>,
        /// then <see cref="JsonValue.IsNull"/> returns <see langword="true"/>,
        /// and both <see cref="JsonValue.AsLiteral"/> and <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> return <c>null</c>.
        /// </summary>
        Null,

        /// <summary>
        /// Represents the <c>Boolean</c> type in JavaScript.
        /// This is the <see cref="JsonValue.Type"/> of the <see cref="JsonValue.True"/> and <see cref="JsonValue.False"/> fields.
        /// If the <see cref="JsonValue.Type"/> property of a <see cref="JsonValue"/> instance is <see cref="Boolean"/>,
        /// then <see cref="JsonValue.AsBoolean"/> returns the actual value,
        /// and both <see cref="JsonValue.AsLiteral"/> and <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> return its string representation.
        /// </summary>
        Boolean,

        /// <summary>
        /// Represents the <c>Number</c> type in JavaScript. The actual number is always stored as a string in <see cref="JsonValue"/>.
        /// If the <see cref="JsonValue.Type"/> property of a <see cref="JsonValue"/> instance is <see cref="Number"/>,
        /// then <see cref="JsonValue.AsNumber"/> returns the represented value using the same precision as JavaScript does,
        /// whereas <see cref="JsonValue.AsLiteral"/> and <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> return the actual underlying value preserving the original precision.
        /// <br/>See also the <strong>Remarks</strong> section of the <see cref="JsonValue.AsNumber"/> property for details.
        /// </summary>
        Number,

        /// <summary>
        /// Represents the <c>String</c> type in JavaScript.
        /// If the <see cref="JsonValue.Type"/> property of a <see cref="JsonValue"/> instance is <see cref="String"/>,
        /// then both <see cref="JsonValue.AsString"/> and <see cref="JsonValue.AsLiteral"/> return the actual string,
        /// wheres <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> returns the JSON representation with the added quotes and possible escape characters.
        /// </summary>
        String,

        /// <summary>
        /// Represents the <c>Array</c> type in JavaScript.
        /// If the <see cref="JsonValue.Type"/> property of a <see cref="JsonValue"/> instance is <see cref="Array"/>,
        /// then <see cref="JsonValue.AsArray"/> returns a non-<see langword="null"/> instance, the elements can be accessed by
        /// the <see cref="int">int</see> value <see cref="JsonValue.this[int]">indexer</see>,
        /// and <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> returns the JSON representation of the array.
        /// </summary>
        Array,

        /// <summary>
        /// Represents the <c>Object</c> type in JavaScript.
        /// If the <see cref="JsonValue.Type"/> property of a <see cref="JsonValue"/> instance is <see cref="Object"/>,
        /// then <see cref="JsonValue.AsObject"/> returns a non-<see langword="null"/> instance, the property values can be accessed by
        /// the <see cref="string">string</see> value <see cref="JsonValue.this[string]">indexer</see>,
        /// and <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> returns the JSON representation of the object.
        /// </summary>
        Object
    }
}
