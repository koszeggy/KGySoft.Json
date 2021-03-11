#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using KGySoft.Collections.ObjectModel;

#endregion

namespace TradeSystem.Json
{
    public sealed class JsonArray : VirtualCollection<JsonValue>
    {
        #region Constructors

        public JsonArray()
        {
        }

        public JsonArray(IList<JsonValue> items) : base(items ?? throw new ArgumentNullException(nameof(items)))
        {
        }

        #endregion

        #region Methods

        #region Public Methods

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

        #region Protected Methods

        protected override JsonValue GetItem(int index) => (uint)index < Count ? base.GetItem(index) : JsonValue.Undefined;

        #endregion

        #endregion
    }
}