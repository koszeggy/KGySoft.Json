#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KGySoft.Collections.ObjectModel;
using KGySoft.CoreLibraries;

#endregion

namespace TradeSystem.Json
{
    // TODO: make VirtualCollection.Count virtual, too, and then
    // - initialize base elements on demand only, too (ctor)
    // - Maintain base list only if list is initialized, too (this[string].set, Remove(string))
    // - Create base list on demand (Insert/Set/Remove)
    // - Override Count (as list/dict), GetItem (EnsureList), GetEnumerator (as list/dict)
    public sealed class JsonObject : VirtualCollection<JsonProperty>
    {
        #region Fields

        private IDictionary<string, JsonValue> _asDictionary;

        #endregion

        #region Indexers

        public JsonValue this[string propertyName]
        {
            get
            {
                if (propertyName == null)
                    throw new ArgumentNullException(nameof(propertyName));
                return EnsureDictionary().GetValueOrDefault(propertyName);
            }
            set
            {
                if (propertyName == null)
                    throw new ArgumentNullException(nameof(propertyName));
                IDictionary<string, JsonValue> dict = EnsureDictionary();
                // TODO: call this only if initialized as list; otherwise, as dict only
                if (dict.TryGetValue(propertyName, out var oldValue))
                    this[IndexOf((propertyName, oldValue))] = (propertyName, value);
                else
                    Add(new JsonProperty(propertyName, value));
            }
        }

        #endregion

        #region Constructors

        public JsonObject()
        {
        }

        public JsonObject(IList<JsonProperty> properties) : base(properties ?? throw new ArgumentNullException(nameof(properties)))
        {
            // Not initializing _asDictionary here. It is initialized on demand when an element is accessed by name.
        }

        public JsonObject(IDictionary<string, JsonValue> properties)
        {
            _asDictionary = properties ?? throw new ArgumentNullException(nameof(properties));
            // TODO: Do not initialize as list here. It is initialized on demand when an element is accessed by index.
            foreach (KeyValuePair<string, JsonValue> property in properties)
                Items.Add(new JsonProperty(property.Key, property.Value));
        }

        #endregion

        #region Methods

        #region Public Methods

        public void Add(string name, JsonValue value)
        {
            // TODO: call this only if already initialized as list; otherwise, as dict only
            Add(new JsonProperty(name, value));
        }

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
            // TODO: EnsureList
            base.InsertItem(index, item);
            _asDictionary?.Add(item.Name, item.Value);
        }

        protected override void SetItem(int index, JsonProperty item)
        {
            if (item.Name == null)
                throw new ArgumentException($"{nameof(item.Name)} is null", nameof(item));
            // TODO: EnsureList
            base.SetItem(index, item);
            if (_asDictionary != null)
                _asDictionary[item.Name] = item.Value;
        }

        protected override void RemoveItem(int index)
        {
            if ((uint)index < (uint)Count)
                _asDictionary?.Remove(this[index].Name);
            // TODO: EnsureList
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            _asDictionary.Clear();
            // TODO: call base only if initialized as list
            base.ClearItems();
        }

        #endregion

        #region Private Methods

        private IDictionary<string, JsonValue> EnsureDictionary()
            => _asDictionary ??= this.ToDictionary(p => p.Name, p => p.Value);

        #endregion

        #endregion
    }
}
