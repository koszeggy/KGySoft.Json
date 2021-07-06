#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonProperty.cs
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
using System.Diagnostics;
using System.Text;

#endregion

namespace KGySoft.Json
{
    public readonly struct JsonProperty : IEquatable<JsonProperty>
    {
        #region Properties

        #region Public Properties
        
        public string Name { get; }
        public JsonValue Value { get; }

        #endregion

        #region Internal Properties

        internal bool IsDefault => Name == default!;

        #endregion

        #endregion

        #region Operators

        public static bool operator ==(JsonProperty left, JsonProperty right) => left.Equals(right);
        public static bool operator !=(JsonProperty left, JsonProperty right) => !left.Equals(right);

        public static implicit operator JsonProperty((string Name, JsonValue Value) property) => new JsonProperty(property.Name, property.Value);
        public static implicit operator JsonProperty(KeyValuePair<string, JsonValue> property) => new JsonProperty(property.Key, property.Value);

        #endregion

        #region Constructors

        public JsonProperty(string name, JsonValue value)
        {
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, name CAN be null but MUST NOT be
            Name = name ?? Throw .ArgumentNullException<string>(nameof(name));
            Value = value;
        }

        #endregion

        #region Methods

        #region Public Methods
        
        public bool Equals(JsonProperty other) => Name == other.Name && Value == other.Value;

        public override bool Equals(object obj) => obj is JsonValue other && Equals(other);

        public override int GetHashCode() => (Name, Value).GetHashCode();

        public override string ToString() => IsDefault ? JsonValue.UndefinedLiteral : $"{Name}:{Value}";

        #endregion

        #region Internal Methods

        internal void Dump(StringBuilder builder)
        {
            Debug.Assert(Name != null && !Value.IsUndefined);
            JsonValue.WriteJsonString(builder, Name);
            builder.Append(':');
            Value.Dump(builder);
        }

        #endregion

        #endregion
    }
}
