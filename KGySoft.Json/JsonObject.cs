#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonObject.cs
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
    public sealed class JsonObject : IList<JsonProperty>, IDictionary<string, JsonValue>
    {
        #region Fields

        private readonly List<JsonProperty> properties;

        private Dictionary<string, int>? nameToIndex;

        #endregion

        #region Properties and Indexers

        #region Properties

        #region Public Properties

        public int Count => properties.Count;

        #endregion

        #region Explicitly Implemented Interface Properties

        bool ICollection<JsonProperty>.IsReadOnly => false;
        bool ICollection<KeyValuePair<string, JsonValue>>.IsReadOnly => false;

        ICollection<string> IDictionary<string, JsonValue>.Keys
        {
            get
            {
                EnsureMap();
                return nameToIndex!.Keys;
            }
        }

        ICollection<JsonValue> IDictionary<string, JsonValue>.Values
        {
            get
            {
                // Note: If there are duplicate keys, then this may return more Values than Keys but this is alright
                int count = Count;
                var result = new JsonValue[count];
                for (int i = 0; i < count; i++)
                    result[i] = properties[i].Value;
                return result;
            }
        }

        #endregion

        #endregion

        #region Indexers

        public JsonProperty this[int index]
        {
            get
            {
                if ((uint)index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return properties[index];
            }
            set
            {
                if ((uint)index >= Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                if (value.Name == null)
                    throw new ArgumentException($"{nameof(value.Name)} is null", nameof(value));
                properties[index] = value;

                Dictionary<string, int>? map = nameToIndex;
                if (map == null)
                    return;
                map[value.Name] = index;
            }
        }

        public JsonValue this[string propertyName]
        {
            get
            {
                if (propertyName == null)
                    throw new ArgumentNullException(nameof(propertyName));
                return TryGetIndex(propertyName, out int index)
                    ? properties[index].Value
                    : JsonValue.Undefined;
            }
            set
            {
                if (propertyName == null)
                    throw new ArgumentNullException(nameof(propertyName));

                if (TryGetIndex(propertyName, out int index))
                {
                    properties[index] = (propertyName, value);
                    return;
                }

                InsertItem(Count, (propertyName, value));
            }
        }

        #endregion

        #endregion

        #region Constructors

        #region Public Constructors

        public JsonObject() => properties = new List<JsonProperty>();

        public JsonObject(IEnumerable<JsonProperty> properties)
            : this(new List<JsonProperty>(properties ?? throw new ArgumentNullException(nameof(properties))))
        {
            // Not initializing the dictionary here. It is initialized on demand when an element is accessed by name.
            // The public constructor always copies the elements into a new list so no consistency check is needed when using the index map.
        }

        #endregion

        #region Internal Constructors

        internal JsonObject(List<JsonProperty> properties) => this.properties = properties;

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        public void Add(JsonProperty item)
        {
            if (item.Name == null)
                throw new ArgumentException($"{nameof(item.Name)} is null", nameof(item));
            InsertItem(Count, item);
        }

        public void Add(string name, JsonValue value) => Add(new JsonProperty(name, value));

        public void Insert(int index, JsonProperty item)
        {
            if ((uint)index > Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (item.Name == null)
                throw new ArgumentException($"{nameof(item.Name)} is null", nameof(item));
            InsertItem(index, item);
        }

        public bool Contains(string propertyName) => TryGetIndex(propertyName, out int index) && index >= 0;

        public int IndexOf(string propertyName) => TryGetIndex(propertyName, out int index) ? index : -1;

        public bool TryGetValue(string propertyName, out JsonValue value)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
            if (TryGetIndex(propertyName, out int index))
            {
                value = properties[index].Value;
                return true;
            }

            value = default;
            return false;
        }

        public void RemoveAt(int index)
        {
            if ((uint)index > Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            properties.RemoveAt(index);
            nameToIndex = null;
        }

        public bool Remove(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));

            Dictionary<string, int>? map = nameToIndex;
            if (map != null)
            {
                if (!map.TryGetValue(propertyName, out int index))
                    return false;

                // nullifying index after removing even from last position because of possible duplicates
                properties.RemoveAt(index);
                nameToIndex = null;
                return true;
            }

            int count = Count;
            IList<JsonProperty> items = properties;
            for (int i = 0; i < count; i++)
            {
                if (items[i].Name == propertyName)
                {
                    items.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void Clear()
        {
            properties.Clear();
            nameToIndex = null;
        }

        public void CopyTo(JsonProperty[] array, int arrayIndex) => properties.CopyTo(array, arrayIndex);

        public IEnumerator<JsonProperty> GetEnumerator() => properties.GetEnumerator();

        public override int GetHashCode()
        {
            int result = Count;
            if (result == 0)
                return result;

            // ReSharper disable once LoopCanBeConvertedToQuery - performance and readability
            foreach (JsonProperty property in properties)
            {
                // to avoid recursion including the hashes of the primitive values only (Equals does it, though)
                result = property.Value.Type <= JsonValueType.String
                    ? (result, property.Name, property.Value).GetHashCode()
                    : (result, property.Name, property.Value.Type).GetHashCode();
            }

            return result;
        }

        public override bool Equals(object obj) => obj is JsonObject other && Count == other.Count && properties.SequenceEqual(other.properties);

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
            builder.Append('{');
            bool first = true;
            foreach (JsonProperty property in properties)
            {
                if (property.Value.IsUndefined)
                    continue;
                if (first)
                    first = false;
                else
                    builder.Append(',');
                property.Dump(builder);
            }

            builder.Append('}');
        }

        #endregion

        #region Private Methods

        private void InsertItem(int index, JsonProperty item)
        {
            properties.Insert(index, item);
            if (nameToIndex == null)
                return;

            // Maintaining index map dictionary only if inserting at the last position
            // (by an offset field inserting at the first position could be maintained, too, but we don't want a field only for this)
            if (index == Count - 1)
                nameToIndex[item.Name] = index;
            else
                nameToIndex = null;
        }

        private bool TryGetIndex(string name, out int index)
        {
            EnsureMap();
            return nameToIndex!.TryGetValue(name, out index);
        }

        private void EnsureMap()
        {
            if (nameToIndex != null)
                return;

            // Initializing index map. Duplicate names will map to the last occurrence.
            IList<JsonProperty> items = properties;
            int count = items.Count;
            var map = new Dictionary<string, int>(count);
            for (int i = 0; i < count; i++)
                map[items[i].Name] = i;
            nameToIndex = map;
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        void ICollection<KeyValuePair<string, JsonValue>>.Add(KeyValuePair<string, JsonValue> item) => Add(item);

        int IList<JsonProperty>.IndexOf(JsonProperty item) => properties.IndexOf(item);
        bool ICollection<JsonProperty>.Contains(JsonProperty item) => properties.Contains(item);
        bool ICollection<KeyValuePair<string, JsonValue>>.Contains(KeyValuePair<string, JsonValue> item) => properties.Contains(item);
        bool IDictionary<string, JsonValue>.ContainsKey(string key) => Contains(key);

        bool ICollection<JsonProperty>.Remove(JsonProperty item)
        {
            bool result = properties.Remove(item);
            if (result)
                nameToIndex = null;
            return result;
        }

        bool ICollection<KeyValuePair<string, JsonValue>>.Remove(KeyValuePair<string, JsonValue> item) => ((ICollection<JsonProperty>)this).Remove(item);

        void ICollection<KeyValuePair<string, JsonValue>>.CopyTo(KeyValuePair<string, JsonValue>[] array, int arrayIndex)
        {
            // This means double copying but we can delegate error checking to List. Just use the public CopyTo whenever possible
            properties.Select(p => new KeyValuePair<string, JsonValue>()).ToList().CopyTo(array, arrayIndex);
        }

        IEnumerator<KeyValuePair<string, JsonValue>> IEnumerable<KeyValuePair<string, JsonValue>>.GetEnumerator()
            => properties.Select(property => new KeyValuePair<string, JsonValue>(property.Name, property.Value)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => properties.GetEnumerator();

        #endregion

        #endregion
    }
}
