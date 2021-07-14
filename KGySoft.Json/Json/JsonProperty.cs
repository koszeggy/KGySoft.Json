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
    /// <summary>
    /// Represents a property in a <see cref="JsonObject"/>.
    /// </summary>
    /// <seealso cref="JsonObject"/>
    public readonly struct JsonProperty : IEquatable<JsonProperty>
    {
        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the name of the property. It can be <see langword="null"/> only for a default <see cref="JsonProperty"/>
        /// instance that was created without the public <see cref="JsonProperty(string, JsonValue)">constructor</see>.
        /// </summary>
        public string? Name { get; }

        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        public JsonValue Value { get; }

        #endregion

        #region Internal Properties

        internal bool IsDefault => Name == default!;

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Determines whether two specified <see cref="JsonProperty"/> instances have the same value.
        /// </summary>
        /// <param name="left">The left argument of the equality check.</param>
        /// <param name="right">The right argument of the equality check.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator ==(JsonProperty left, JsonProperty right) => left.Equals(right);

        /// <summary>
        /// Determines whether two specified <see cref="JsonProperty"/> instances have different values.
        /// </summary>
        /// <param name="left">The left argument of the inequality check.</param>
        /// <param name="right">The right argument of the inequality check.</param>
        /// <returns>The result of the inequality check.</returns>
        public static bool operator !=(JsonProperty left, JsonProperty right) => !left.Equals(right);

#if NETSTANDARD2_0_OR_GREATER || NET47_OR_GREATER
        /// <summary>
        /// Performs an implicit conversion from <see cref="ValueTuple{T1, T2}"/> to <see cref="JsonProperty"/>.
        /// </summary>
        /// <param name="property">The tuple to be converted to a <see cref="JsonProperty"/>.</param>
        /// <returns>
        /// A <see cref="JsonProperty"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonProperty((string Name, JsonValue Value) property) => new JsonProperty(property.Name, property.Value);
#endif

        /// <summary>
        /// Performs an implicit conversion from <see cref="KeyValuePair{TKey,TValue}"/> to <see cref="JsonProperty"/>.
        /// </summary>
        /// <param name="property">The key-value pair to be converted to a <see cref="JsonProperty"/>.</param>
        /// <returns>
        /// A <see cref="JsonProperty"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonProperty(KeyValuePair<string, JsonValue> property) => new JsonProperty(property.Key, property.Value);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonProperty"/> struct.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        public JsonProperty(string name, JsonValue value)
        {
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, name CAN be null but MUST NOT be
            Name = name ?? Throw.ArgumentNullException<string>(nameof(name));
            Value = value;
        }

        #endregion

        #region Methods

        #region Public Methods

        /// <summary>
        /// Indicates whether the current <see cref="JsonProperty"/> instance is equal to another one specified in the <paramref name="other"/> parameter.
        /// </summary>
        /// <param name="other">A <see cref="JsonProperty"/> instance to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public bool Equals(JsonProperty other) => Name == other.Name && Value == other.Value;

        /// <summary>
        /// Determines whether the specified <see cref="object">object</see> is equal to this instance.
        /// </summary>
        /// <param name="obj">An <see cref="object"/> to compare with this instance.</param>
        /// <returns><see langword="true"/> if the specified object is equal to this instance; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object? obj) => obj is JsonProperty other && Equals(other);

        /// <summary>
        /// Returns a hash code for this <see cref="JsonProperty"/> instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() => (Name, Value).GetHashCode();

        /// <summary>
        /// Gets the string representation of this <see cref="JsonProperty"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => IsDefault ? JsonValue.UndefinedLiteral : $"{Name}:{Value}";

        #endregion

        #region Internal Methods

        internal void Dump(StringBuilder builder)
        {
            Debug.Assert(Name != null && !Value.IsUndefined);
            JsonValue.WriteJsonString(builder, Name!);
            builder.Append(':');
            Value.Dump(builder);
        }

        #endregion

        #endregion
    }
}
