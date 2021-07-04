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



#endregion

using System;
using System.Diagnostics;
using System.Text;

namespace KGySoft.Json
{
    public readonly struct JsonProperty : IEquatable<JsonProperty>
    {
        #region Properties

        public string Name { get; }
        public JsonValue Value { get; }

        #endregion

        #region Operators

        public static bool operator ==(JsonProperty left, JsonProperty right) => left.Equals(right);
        public static bool operator !=(JsonProperty left, JsonProperty right) => !left.Equals(right);

        public static implicit operator JsonProperty((string Name, JsonValue Value) property) => new JsonProperty(property.Name, property.Value);

        #endregion

        #region Constructors

        public JsonProperty(string name, JsonValue value)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Value = value;
        }

        #endregion

        #region Methods

        #region Public Methods
        
        public bool Equals(JsonProperty other) => Name == other.Name && Value == other.Value;

        public override bool Equals(object obj) => obj is JsonValue other && Equals(other);

        public override int GetHashCode() => (Name, Value).GetHashCode();

        public override string ToString() => Name == null ? base.ToString() : $"{Name}:{Value}";

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
