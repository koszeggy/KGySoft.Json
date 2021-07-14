#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonArray.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace KGySoft.Json
{
    /// <summary>
    /// Represents a JSON array, interpreted as a list of <see cref="JsonValue"/> elements.
    /// Use the <see cref="ToString">ToString</see> method to convert it to a JSON string.
    /// </summary>
    /// <seealso cref="JsonValue"/>
    /// <seealso cref="JsonObject"/>
    public sealed class JsonArray : IList<JsonValue>
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER
        , IReadOnlyList<JsonValue> 
#endif
    {
        #region Fields

        private readonly List<JsonValue> items;

        #endregion

        #region Properties and Indexers

        #region Public Properties

        #region Properties

        /// <summary>
        /// Gets the number of elements contained in the <see cref="JsonArray"/>.
        /// </summary>
        public int Count => items.Count;

        #endregion

        #region Explicitly Implemented Interface Properties

        bool ICollection<JsonValue>.IsReadOnly => false;

        #endregion

        #endregion

        #region Indexers

        /// <summary>
        /// Gets or sets the element at the specified <paramref name="index"/>.
        /// When the indexer is read, using an invalid index returns <see cref="JsonValue.Undefined"/>, just like in JavaScript.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The indexer is set and <paramref name="index"/> is less than zero or greater or equal to <see cref="Count"/>.</exception>
        public JsonValue this[int index]
        {
            get => (uint)index < Count ? items[index] : JsonValue.Undefined;
            set => items[index] = value;
        }

        #endregion

        #endregion

        #region Constructors

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonArray"/> class.
        /// </summary>
        public JsonArray() => items = new List<JsonValue>();

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonArray"/> class from a collection of <see cref="JsonValue"/> items.
        /// </summary>
        /// <param name="items">The items to be added to this <see cref="JsonArray"/>.</param>
        public JsonArray(IEnumerable<JsonValue> items)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, items CAN be null but MUST NOT be
            : this(new List<JsonValue>(items ?? Throw.ArgumentNullException<List<JsonValue>>(nameof(items))))
        {
        }

        #endregion

        #region Internal Constructors

        internal JsonArray(List<JsonValue> items) => this.items = items;

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        /// <summary>
        /// Reads a <see cref="JsonArray"/> from a <see cref="TextReader"/> that contains a JSON array.
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> that will be read for the <see cref="JsonArray"/> content.</param>
        /// <returns>A <see cref="JsonArray"/> that contains the JSON array data that was read from the specified <see cref="TextReader"/>.</returns>
        public static JsonArray Parse(TextReader reader)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, reader CAN be null but MUST NOT be
            => JsonParser.ParseArray(reader ?? Throw.ArgumentNullException<TextReader>(nameof(reader)));

        /// <summary>
        /// Reads a <see cref="JsonArray"/> from a <see cref="Stream"/> that contains JSON array.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> that will be read for the <see cref="JsonArray"/> content.</param>
        /// <param name="encoding">An <see cref="Encoding"/> that specifies the encoding of the JSON array data in the <paramref name="stream"/>.
        /// If <see langword="null"/>, then <see cref="Encoding.UTF8"/> encoding will be used. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <returns>A <see cref="JsonArray"/> that contains the JSON array data that was read from the specified <paramref name="stream"/>.</returns>
        public static JsonArray Parse(Stream stream, Encoding? encoding = null)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, stream CAN be null but MUST NOT be
            => Parse(new StreamReader(stream ?? Throw.ArgumentNullException<Stream>(nameof(stream)), encoding ?? Encoding.UTF8));

        /// <summary>
        /// Reads a <see cref="JsonArray"/> from a <see cref="string">string</see> that contains JSON array.
        /// </summary>
        /// <param name="s">A string that will be read for the <see cref="JsonArray"/> content.</param>
        /// <returns>A <see cref="JsonArray"/> that contains the JSON array data that was read from the specified string.</returns>
        public static JsonArray Parse(string s)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, s CAN be null but MUST NOT be
            => Parse(new StringReader(s ?? Throw.ArgumentNullException<string>(nameof(s))));

        /// <summary>
        /// Tries to read a <see cref="JsonArray"/> from a <see cref="TextReader"/> that contains JSON array.
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> that will be read for the <see cref="JsonArray"/> content.</param>
        /// <param name="value">When this method returns <see langword="true"/>, the result of the parsing;
        /// otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(TextReader reader, [MaybeNullWhen(false)]out JsonArray value)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, reader CAN be null but MUST NOT be
            => JsonParser.TryParseArray(reader ?? Throw.ArgumentNullException<TextReader>(nameof(reader)), out value);

        /// <summary>
        /// Tries to read a <see cref="JsonArray"/> from a <see cref="Stream"/> that contains JSON array.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> that will be read for the <see cref="JsonArray"/> content.</param>
        /// <param name="value">When this method returns <see langword="true"/>, the result of the parsing;
        /// otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <param name="encoding">An <see cref="Encoding"/> that specifies the encoding of the JSON array data in the <paramref name="stream"/>.
        /// If <see langword="null"/>, then <see cref="Encoding.UTF8"/> encoding will be used. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(Stream stream, [MaybeNullWhen(false)]out JsonArray value, Encoding? encoding = null)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, stream CAN be null but MUST NOT be
            => TryParse(new StreamReader(stream ?? Throw.ArgumentNullException<Stream>(nameof(stream)), encoding ?? Encoding.UTF8), out value);

        /// <summary>
        /// Tries to read a <see cref="JsonArray"/> from a <see cref="string">string</see> that contains JSON array.
        /// </summary>
        /// <param name="s">A string that will be read for the <see cref="JsonArray"/> content.</param>
        /// <param name="value">When this method returns <see langword="true"/>, the result of the parsing;
        /// otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(string s, [MaybeNullWhen(false)]out JsonArray value)
            // ReSharper disable once ConstantNullCoalescingCondition - false alarm, s CAN be null but MUST NOT be
            => TryParse(new StringReader(s ?? Throw.ArgumentNullException<string>(nameof(s))), out value);

        #endregion

        #region Instance Methods

        #region Public Methods

        /// <summary>
        /// Adds an <paramref name="item"/> to the <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="item">The item to add to the <see cref="JsonArray"/>.</param>
        public void Add(JsonValue item) => items.Add(item);

        /// <summary>
        /// Inserts an <paramref name="item"/> to the <see cref="JsonArray"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The <see cref="JsonValue"/> to insert into the <see cref="JsonArray"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than <see cref="Count"/>.</exception>
        public void Insert(int index, JsonValue item) => items.Insert(index, item);

        /// <summary>
        /// Determines whether the <see cref="JsonArray"/> contains the specific <paramref name="item"/>.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if <paramref name="item"/> is found in the <see cref="JsonArray"/>; otherwise, <see langword="false"/>.
        /// </returns>
        /// <param name="item">The <see cref="JsonValue"/> to locate in the <see cref="JsonArray"/>.</param>
        public bool Contains(JsonValue item) => items.Contains(item);

        /// <summary>
        /// Determines the index of a specific value in the <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="item">The <see cref="JsonValue"/> to locate in the <see cref="JsonArray"/>.</param>
        /// <returns>The index of <paramref name="item"/> if found in the array; otherwise, -1. </returns>
        public int IndexOf(JsonValue item) => items.IndexOf(item);

        /// <summary>
        /// Removes the first occurrence of the specific <paramref name="item"/> from the <see cref="JsonArray"/>.
        /// </summary>
        /// <param name="item">The <see cref="JsonValue"/> to remove from the <see cref="JsonArray"/>.</param>
        /// <returns> <see langword="true"/> if <paramref name="item"/> was successfully removed from the <see cref="JsonArray"/>; otherwise, <see langword="false"/>.</returns>
        public bool Remove(JsonValue item) => items.Remove(item);

        /// <summary>
        /// Removes the value from the <see cref="JsonArray"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not a valid index in the <see cref="JsonArray"/>.</exception>
        public void RemoveAt(int index) => items.RemoveAt(index);

        /// <summary>
        /// Removes all items from the <see cref="JsonArray"/>.
        /// </summary>
        public void Clear() => items.Clear();

        /// <summary>
        /// Copies the elements of the <see cref="JsonArray" /> to an <see cref="Array"/> of <see cref="JsonValue"/> elements, starting at the specified <paramref name="arrayIndex"/>.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="JsonArray"/>.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(JsonValue[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="JsonArray"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> instance that can be used to iterate though the elements of the <see cref="JsonArray"/>.</returns>
        public IEnumerator<JsonValue> GetEnumerator() => items.GetEnumerator();

        /// <summary>
        /// Returns a hash code for this <see cref="JsonArray"/> instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            int result = Count;
            if (result == 0)
                return result;

            // ReSharper disable once LoopCanBeConvertedToQuery - performance and readability
            foreach (JsonValue item in items)
            {
                // to avoid recursion including the hashes of the primitive values only (Equals does it, though)
                result = item.Type <= JsonValueType.String
                    ? (result, item).GetHashCode()
                    : (result, item.Type).GetHashCode();
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the specified object is equal to this instance; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object? obj) => obj is JsonArray other && Count == other.Count && this.SequenceEqual(other);

        /// <summary>
        /// Returns a minimized JSON string for this <see cref="JsonArray"/>.
        /// </summary>
        /// <returns>A minimized JSON string for this <see cref="JsonArray"/>.</returns>
        public override string ToString()
        {
            var result = new StringBuilder();
            Dump(result);
            return result.ToString();
        }

        #endregion

        #region Internal Methods

        internal void Dump(StringBuilder builder)
        {
            builder.Append('[');
            bool first = true;
            foreach (JsonValue value in items)
            {
                if (value.IsUndefined)
                    continue;
                if (first)
                    first = false;
                else
                    builder.Append(',');
                value.Dump(builder);
            }

            builder.Append(']');
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();

        #endregion

        #endregion

        #endregion
    }
}