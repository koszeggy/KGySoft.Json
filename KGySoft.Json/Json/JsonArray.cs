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
using System.Linq;
using System.Text;

#endregion

namespace KGySoft.Json
{
    public sealed class JsonArray : IList<JsonValue>
    {
        #region Fields

        private readonly List<JsonValue> items;

        #endregion

        #region Properties and Indexers

        #region Public Properties
        
        #region Properties

        public int Count => items.Count;


        #endregion

        #region Explicitly Implemented Interface Properties

        bool ICollection<JsonValue>.IsReadOnly => false;

        #endregion

        #endregion

        #region Indexers

        public JsonValue this[int index]
        {
            get => (uint)index < Count ? items[index] : JsonValue.Undefined;
            set => items[index] = value;
        }

        #endregion
        
        #endregion

        #region Constructors

        #region Public Constructors

        public JsonArray()
        {
        }

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

        #region Public Methods

        public void Add(JsonValue item) => items.Add(item);

        public void Insert(int index, JsonValue item) => items.Insert(index, item);

        public bool Contains(JsonValue item) => items.Contains(item);

        public int IndexOf(JsonValue item) => items.IndexOf(item);

        public bool Remove(JsonValue item) => items.Remove(item);

        public void RemoveAt(int index) => items.RemoveAt(index);

        public void Clear() => items.Clear();

        public void CopyTo(JsonValue[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

        public IEnumerator<JsonValue> GetEnumerator() => items.GetEnumerator();

        public override int GetHashCode()
        {
            int result = Count;
            if (result == 0)
                return result;

            // ReSharper disable once LoopCanBeConvertedToQuery - performance and readability
            foreach (JsonValue item in this)
            {
                // to avoid recursion including the hashes of the primitive values only (Equals does it, though)
                result = item.Type <= JsonValueType.String
                    ? (result, item).GetHashCode()
                    : (result, item.Type).GetHashCode();
            }

            return result;
        }

        public override bool Equals(object obj) => obj is JsonArray other && Count == other.Count && this.SequenceEqual(other);

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
            foreach (JsonValue value in this)
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
    }
}