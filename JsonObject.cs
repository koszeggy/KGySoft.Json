#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

#endregion

namespace TradeSystem.Json
{
    public sealed class JsonObject : Collection<JsonProperty>
    {
        #region Fields

        private Dictionary<string, int> _nameToIndex;

        #endregion

        #region Indexers

        public JsonValue this[string propertyName]
        {
            get
            {
                if (propertyName == null)
                    throw new ArgumentNullException(nameof(propertyName));
                return TryGetIndex(propertyName, out int index)
                    ? Items[index].Value
                    : JsonValue.Undefined;
            }
            set
            {
                if (propertyName == null)
                    throw new ArgumentNullException(nameof(propertyName));

                if (TryGetIndex(propertyName, out int index))
                {
                    Items[index] = (propertyName, value);
                    return;
                }

                InsertItem(Count, (propertyName, value));
            }
        }

        #endregion

        #region Constructors

        #region Public Constructors

        public JsonObject()
        {
        }

        public JsonObject(IEnumerable<JsonProperty> properties)
            : this(new List<JsonProperty>(properties ?? throw new ArgumentNullException(nameof(properties))))
        {
            // Not initializing the dictionary here. It is initialized on demand when an element is accessed by name.
            // The public constructor always copies the elements into a new list so no consistency check is needed when using the index map.
        }

        #endregion

        #region Internal Constructors

        internal JsonObject(List<JsonProperty> properties) : base(properties)
        {
        }

        #endregion

        #endregion

        #region Methods

        #region Public Methods

        public void Add(string name, JsonValue value) => Add(new JsonProperty(name, value));

        public override int GetHashCode()
        {
            int result = Count;
            if (result == 0)
                return result;

            // ReSharper disable once LoopCanBeConvertedToQuery - performance and readability
            foreach (JsonProperty property in this)
            {
                // to avoid recursion including the hashes of the primitive values only (Equals does it, though)
                result = property.Value.Type <= JsonValueType.String
                    ? (result, property.Name, property.Value).GetHashCode()
                    : (result, property.Name, property.Value.Type).GetHashCode();
            }

            return result;
        }

        public override bool Equals(object obj) => obj is JsonObject other && Count == other.Count && this.SequenceEqual(other);

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
            foreach (JsonProperty property in this)
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

        #region Protected Methods

        protected override void InsertItem(int index, JsonProperty item)
        {
            if (item.Name == null)
                throw new ArgumentException($"{nameof(item.Name)} is null", nameof(item));
            base.InsertItem(index, item);
            if (_nameToIndex == null)
                return;

            // Maintaining index map dictionary only if inserting at the last position
            // (by an offset field inserting at the first position could be maintained, too, but we don't want a field only for this)
            if (index == Count - 1)
                _nameToIndex[item.Name] = index;
            else
                _nameToIndex = null;
        }

        protected override void SetItem(int index, JsonProperty item)
        {
            if (item.Name == null)
                throw new ArgumentException($"{nameof(item.Name)} is null", nameof(item));

            Dictionary<string, int> map = _nameToIndex;
            if (map == null)
                return;
            map[item.Name] = index;
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
            _nameToIndex = null;
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            _nameToIndex = null;
        }

        #endregion

        #region Private Methods

        private bool TryGetIndex(string name, out int index)
        {
            IList<JsonProperty> items = Items;
            Dictionary<string, int> map = _nameToIndex;
            if (map != null)
                return map.TryGetValue(name, out index);

            // Initializing index map. Duplicate names will map to the last occurrence.
            int count = items.Count;
            map = new Dictionary<string, int>(count);
            for (int i = 0; i < count; i++)
                map[items[i].Name] = i;
            _nameToIndex = map;
            return map.TryGetValue(name, out index);
        }

        #endregion

        #endregion
    }
}
