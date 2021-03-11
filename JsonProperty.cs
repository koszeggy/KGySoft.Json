#region Usings



#endregion

using System;
using System.Diagnostics;
using System.Text;

namespace TradeSystem.Json
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
