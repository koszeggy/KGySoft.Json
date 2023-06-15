#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonObject.cs
///////////////////////////////////////////////////////////////////////////////
//  Copyright (C) KGy SOFT, 2005-2021 - All Rights Reserved
//
//  You should have received a copy of the LICENSE file at the top-level
//  directory of this distribution.
//
//  Please refer to the LICENSE file if you want to use this source code.
///////////////////////////////////////////////////////////////////////////////

#endregion

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

using KGySoft.Collections;
using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Json
{
    /// <summary>
    /// Represents a JSON object, interpreted as a <see cref="string">string</see>-<see cref="JsonValue"/> dictionary
    /// and also as a list of <see cref="JsonProperty"/> elements.
    /// Use the <see cref="O:KGySoft.Json.JsonObject.ToString">ToString</see> or <see cref="O:KGySoft.Json.JsonObject.WriteTo">WriteTo</see> methods to convert it to JSON.
    /// </summary>
    /// <remarks>
    /// <para>Just like in JavaScript, the <see cref="O:KGySoft.Json.JsonObject.ToString">ToString</see> (and <see cref="O:KGySoft.Json.JsonObject.WriteTo">WriteTo</see>)
    /// methods filter out properties with <see cref="JsonValue.Undefined"/> values.</para>
    /// <para>Obtaining a nonexistent property by the <see cref="this[string]">string indexer</see> returns <see cref="JsonValue.Undefined"/>,
    /// which is also a JavaScript-compatible behavior.</para>
    /// <note>Using LINQ extension methods on a <see cref="JsonObject"/> may cause ambiguity due to its list/dictionary duality.
    /// It is recommended to perform the LINQ operations on the <see cref="Entries"/> property so it is not needed to specify the type arguments of the LINQ extension methods.</note>
    /// <para>Due to performance reasons <see cref="JsonObject"/> allows adding duplicate keys; however, getting the properties by
    /// the <see cref="this[string]">string indexer</see> retrieves always the lastly set value, just like in JavaScript.
    /// <note type="tip">Populating the <see cref="JsonObject"/> only by the <see cref="JsonObject(IDictionary{string, JsonValue})">dictionary constructor</see>
    /// or the <see cref="this[string]">string indexer</see> ensures that no duplicate property names are added.</note></para>
    /// <para>If the <see cref="JsonObject"/> contains duplicate property names, then the <see cref="O:KGySoft.Json.JsonObject.ToString">ToString</see>
    /// and <see cref="O:KGySoft.Json.JsonObject.WriteTo">WriteTo</see> methods dump all of them by default.
    /// It's not an issue for JavaScript, which allows parsing such a JSON string where the duplicate keys will have the lastly defined value.
    /// But you can explicitly call the <see cref="EnsureUniqueKeys">EnsureUniqueKeys</see> method to remove the duplicate keys (keeping the lastly defined values)
    /// before producing the JSON string.</para>
    /// <note type="tip">See the <strong>Remarks</strong> section of the <see cref="JsonValue"/> type for more details and examples.</note>
    /// </remarks>
    /// <seealso cref="JsonValue"/>
    /// <seealso cref="JsonObject"/>
    [Serializable]
    public sealed class JsonObject : IList<JsonProperty>, IDictionary<string, JsonValue>
#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER
        , IReadOnlyList<JsonProperty>, IReadOnlyDictionary<string, JsonValue> 
#endif
    {
        #region Constants

        private const int buildIndexMapThreshold = 5;

        #endregion

        #region Fields

        private List<JsonProperty> properties;
        private StringKeyedDictionary<int>? nameToIndex;

        #endregion

        #region Properties and Indexers

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets the number of properties contained in the <see cref="JsonObject"/>,
        /// including possible duplicates and properties with <see cref="JsonValue.Undefined"/> value.
        /// </summary>
        public int Count => properties.Count;

        /// <summary>
        /// Gets a collection of the property names in this <see cref="JsonObject"/>.
        /// This property returns distinct property names even if there are duplicate keys.
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                // ensuring that duplicate keys are returned only once.
                EnsureMap();
                return nameToIndex!.Keys;
            }
        }

        /// <summary>
        /// Gets a collection of the property values in this <see cref="JsonObject"/>.
        /// If there are duplicate property names, then this property may return more elements than the <see cref="Keys"/> property.
        /// To avoid that call the <see cref="EnsureUniqueKeys">EnsureUniqueKeys</see> method before getting this property.
        /// </summary>
        public ICollection<JsonValue> Values
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

        /// <summary>
        /// Gets the property entries of this <see cref="JsonObject"/>, including possible duplicates.
        /// This property simply returns the self reference. It can be useful to be able to use LINQ extension methods
        /// on a <see cref="JsonObject"/> without ambiguity.
        /// </summary>
        public IList<JsonProperty> Entries => this;

        #endregion

        #region Internal Properties

        internal List<JsonProperty> PropertiesInternal => properties;

        #endregion

        #region Explicitly Implemented Interface Properties

        bool ICollection<JsonProperty>.IsReadOnly => false;
        bool ICollection<KeyValuePair<string, JsonValue>>.IsReadOnly => false;

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER
        IEnumerable<string> IReadOnlyDictionary<string, JsonValue>.Keys => Keys;
        IEnumerable<JsonValue> IReadOnlyDictionary<string, JsonValue>.Values => Values;
#endif

        #endregion

        #endregion

        #region Indexers

        /// <summary>
        /// Gets or sets the property at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the property to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater or equal to <see cref="Count"/>.</exception>
        /// <exception cref="ArgumentException">The value is set and the <see cref="JsonProperty.Name"/> of the <paramref name="value"/> is <see langword="null"/>.</exception>
        public JsonProperty this[int index]
        {
            get => properties[index];
            set
            {
                if (value.IsDefault)
                    Throw.ArgumentException(Res.DefaultJsonPropertyInvalid, nameof(value));
                properties[index] = value;

                StringKeyedDictionary<int>? map = nameToIndex;
                if (map == null)
                    return;

                // if name is replaced we invalidate the map
                if (map.TryGetValue(value.Name!, out int origIndex) && origIndex != index)
                    nameToIndex = null;
            }
        }

        /// <summary>
        /// Gets or sets the value of a property by name. When the indexer is read, using a nonexistent <paramref name="propertyName"/>
        /// returns <see cref="JsonValue.Undefined"/>, just like in JavaScript.
        /// </summary>
        /// <param name="propertyName">The name of the property to get or set.</param>
        /// <returns>The value of the property with the specified <paramref name="propertyName"/>, or <see cref="JsonValue.Undefined"/> if no such property is found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
        public JsonValue this[string propertyName]
        {
            get
            {
                if (propertyName == null!)
                    Throw.ArgumentNullException(nameof(propertyName));
                int index = TryGetIndex(propertyName);
                return index >= 0 ? properties[index].Value : JsonValue.Undefined;
            }
            set => SetItem(new JsonProperty(propertyName, value));
        }

        /// <summary>
        /// Gets the value of a property by name. Using a nonexistent <paramref name="propertyName"/>
        /// returns <see cref="JsonValue.Undefined"/>, just like in JavaScript.
        /// </summary>
        /// <param name="propertyName">The name of the property to get or set.</param>
        /// <returns>The value of the property with the specified <paramref name="propertyName"/>, or <see cref="JsonValue.Undefined"/> if no such property is found.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see cref="StringSegment.Null">StringSegment.Null</see>.</exception>
        public JsonValue this[StringSegment propertyName]
        {
            get
            {
                if (propertyName.IsNull)
                    Throw.ArgumentNullException(nameof(propertyName));
                int index = TryGetIndex(propertyName);
                return index >= 0 ? properties[index].Value : JsonValue.Undefined;
            }
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        /// <summary>
        /// Gets the value of a property by name. Using a nonexistent <paramref name="propertyName"/>
        /// returns <see cref="JsonValue.Undefined"/>, just like in JavaScript.
        /// </summary>
        /// <param name="propertyName">The name of the property to get or set.</param>
        /// <returns>The value of the property with the specified <paramref name="propertyName"/>, or <see cref="JsonValue.Undefined"/> if no such property is found.</returns>
        public JsonValue this[ReadOnlySpan<char> propertyName]
        {
            get
            {
                int index = TryGetIndex(propertyName);
                return index >= 0 ? properties[index].Value : JsonValue.Undefined;
            }
        }
#endif

        #endregion

        #endregion

        #region Constructors

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonObject"/> class.
        /// </summary>
        public JsonObject() => properties = new List<JsonProperty>();

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonObject"/> class from a collection of <see cref="JsonProperty"/> items.
        /// </summary>
        /// <param name="properties">The properties to be added to this <see cref="JsonObject"/>.</param>
        /// <param name="allowDuplicates"><see langword="true"/> to allow duplicate multiple elements with the same <see cref="JsonProperty.Name"/>;
        /// <see langword="false"/> to overwrite recurring names with the latest value. This parameter is optional.
        /// <br/>Default value: <see langword="true"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="properties"/> parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="properties"/> parameter contains a <see cref="JsonProperty"/> with <see langword="null"/>&#160;<see cref="JsonProperty.Name"/>.</exception>
        public JsonObject(IEnumerable<JsonProperty> properties, bool allowDuplicates = true)
        {
            if (properties == null!)
                Throw.ArgumentNullException(nameof(properties));

            if (properties is ICollection<JsonProperty> collection)
            {
                this.properties = new List<JsonProperty>(collection.Count);
                
                // Initializing the dictionary only if duplicates are disabled and it is known that enough properties will be added
                if (!allowDuplicates && collection.Count >= buildIndexMapThreshold)
                    nameToIndex = new StringKeyedDictionary<int>(collection.Count, StringSegmentComparer.OrdinalRandomized);
            }
            else
                this.properties = new List<JsonProperty>();

            foreach (JsonProperty property in properties)
            {
                if (property.Name == null)
                    Throw.ArgumentException(Res.DefaultJsonPropertyInvalid, nameof(properties));

                if (allowDuplicates)
                    AddItem(property);
                else
                    SetItem(property);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonObject"/> class from a dictionary.
        /// </summary>
        /// <param name="properties">The properties to be added to this <see cref="JsonObject"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="properties"/> parameter is <see langword="null"/>.</exception>
        public JsonObject(IDictionary<string, JsonValue> properties)
        {
            if (properties == null!)
                Throw.ArgumentNullException(nameof(properties));
            this.properties = new List<JsonProperty>(properties.Count);
            if (properties.Count >= buildIndexMapThreshold)
                nameToIndex = new StringKeyedDictionary<int>(properties.Count, StringSegmentComparer.OrdinalRandomized);
            foreach (KeyValuePair<string, JsonValue> property in properties)
                this[property.Key] = property.Value;
        }

        #endregion

        #region Internal Constructors

        internal JsonObject(List<JsonProperty> properties) => this.properties = properties;

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        /// <summary>
        /// Reads a <see cref="JsonObject"/> from a <see cref="TextReader"/> that contains a JSON object.
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> that will be read for the <see cref="JsonObject"/> content.</param>
        /// <returns>A <see cref="JsonObject"/> that contains the JSON object data that was read from the specified <see cref="TextReader"/>.</returns>
        public static JsonObject Parse(TextReader reader)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, reader CAN be null but MUST NOT be
            => JsonParser.ParseObject(reader ?? Throw.ArgumentNullException<TextReader>(nameof(reader)));

        /// <summary>
        /// Reads a <see cref="JsonObject"/> from a <see cref="Stream"/> that contains JSON object.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> that will be read for the <see cref="JsonObject"/> content.</param>
        /// <param name="encoding">An <see cref="Encoding"/> that specifies the encoding of the JSON object data in the <paramref name="stream"/>.
        /// If <see langword="null"/>, then <see cref="Encoding.UTF8"/> encoding will be used. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <returns>A <see cref="JsonObject"/> that contains the JSON object data that was read from the specified <paramref name="stream"/>.</returns>
        public static JsonObject Parse(Stream stream, Encoding? encoding = null)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, stream CAN be null but MUST NOT be
            => Parse(new StreamReader(stream ?? Throw.ArgumentNullException<Stream>(nameof(stream)), encoding ?? Encoding.UTF8));

        /// <summary>
        /// Reads a <see cref="JsonObject"/> from a <see cref="string">string</see> that contains JSON object.
        /// </summary>
        /// <param name="s">A string that will be read for the <see cref="JsonObject"/> content.</param>
        /// <returns>A <see cref="JsonObject"/> that contains the JSON object data that was read from the specified string.</returns>
        public static JsonObject Parse(string s)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, s CAN be null but MUST NOT be
            => Parse(new StringReader(s ?? Throw.ArgumentNullException<string>(nameof(s))));

        /// <summary>
        /// Tries to read a <see cref="JsonObject"/> from a <see cref="TextReader"/> that contains JSON object.
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> that will be read for the <see cref="JsonObject"/> content.</param>
        /// <param name="value">When this method returns <see langword="true"/>, the result of the parsing;
        /// otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(TextReader reader, [MaybeNullWhen(false)]out JsonObject value)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, reader CAN be null but MUST NOT be
            => JsonParser.TryParseObject(reader ?? Throw.ArgumentNullException<TextReader>(nameof(reader)), out value);

        /// <summary>
        /// Tries to read a <see cref="JsonObject"/> from a <see cref="Stream"/> that contains JSON object.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> that will be read for the <see cref="JsonObject"/> content.</param>
        /// <param name="value">When this method returns <see langword="true"/>, the result of the parsing;
        /// otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <param name="encoding">An <see cref="Encoding"/> that specifies the encoding of the JSON object data in the <paramref name="stream"/>.
        /// If <see langword="null"/>, then <see cref="Encoding.UTF8"/> encoding will be used. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(Stream stream, [MaybeNullWhen(false)]out JsonObject value, Encoding? encoding = null)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, stream CAN be null but MUST NOT be
            => TryParse(new StreamReader(stream ?? Throw.ArgumentNullException<Stream>(nameof(stream)), encoding ?? Encoding.UTF8), out value);

        /// <summary>
        /// Tries to read a <see cref="JsonObject"/> from a <see cref="string">string</see> that contains JSON object.
        /// </summary>
        /// <param name="s">A string that will be read for the <see cref="JsonObject"/> content.</param>
        /// <param name="value">When this method returns <see langword="true"/>, the result of the parsing;
        /// otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(string s, [MaybeNullWhen(false)]out JsonObject value)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, s CAN be null but MUST NOT be
            => TryParse(new StringReader(s ?? Throw.ArgumentNullException<string>(nameof(s))), out value);


        #endregion

        #region Instance Methods

        #region Public Methods

        /// <summary>
        /// Adds an <paramref name="item"/> to this <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="item">The item to add to the <see cref="JsonObject"/>.</param>
        /// <exception cref="ArgumentException">The <see cref="JsonProperty.Name"/> of the <paramref name="item"/> is <see langword="null"/>.</exception>
        public void Add(JsonProperty item)
        {
            if (item.IsDefault)
                Throw.ArgumentException(Res.DefaultJsonPropertyInvalid, nameof(item));
            AddItem(item);
        }

        /// <summary>
        /// Adds a pair of <paramref name="name"/> and <paramref name="value"/> to this <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="name">The name of the property to add to the <see cref="JsonObject"/>.</param>
        /// <param name="value">The value of the property to add to the <see cref="JsonObject"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
        public void Add(string name, JsonValue value) => AddItem(new JsonProperty(name, value));

        /// <summary>
        /// Inserts an <paramref name="item"/> to the <see cref="JsonObject"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item"/> should be inserted.</param>
        /// <param name="item">The <see cref="JsonProperty"/> to insert into the <see cref="JsonObject"/>.</param>
        /// <exception cref="ArgumentException">The <see cref="JsonProperty.Name"/> of the <paramref name="item"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater than <see cref="Count"/>.</exception>
        public void Insert(int index, JsonProperty item)
        {
            if (item.IsDefault)
                Throw.ArgumentException(Res.DefaultJsonPropertyInvalid, nameof(item));

            properties.Insert(index, item);
            if (nameToIndex == null)
                return;

            // Maintaining index map dictionary only if inserting at the last position
            if (index == Count - 1)
                nameToIndex[item.Name!] = index;
            else
                nameToIndex = null;
        }

        /// <summary>
        /// Determines whether the <see cref="JsonObject"/> contains a property with the specified <paramref name="propertyName"/>.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if <paramref name="propertyName"/> is found in the <see cref="JsonObject"/>; otherwise, <see langword="false"/>.
        /// </returns>
        /// <param name="propertyName">The name of the property to locate in the <see cref="JsonObject"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
        public bool Contains(string propertyName)
        {
            if (propertyName == null!)
                Throw.ArgumentNullException(nameof(propertyName));
            return TryGetIndex(propertyName) >= 0;
        }

        /// <summary>
        /// Determines the index of a specific property in the <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to locate in the <see cref="JsonObject"/>.</param>
        /// <returns> The index of the property if found in the <see cref="JsonObject"/>; otherwise, -1.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
        public int IndexOf(string propertyName)
        {
            if (propertyName == null!)
                Throw.ArgumentNullException(nameof(propertyName));
            return TryGetIndex(propertyName);
        }

        /// <summary>
        /// Tries to get the value associated with the specified <paramref name="propertyName"/> from the <see cref="JsonObject"/>.
        /// </summary>
        /// <returns><see langword="true"/> if the key was found in the <see cref="JsonObject"/>; otherwise, <see langword="false"/>.</returns>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified name, if the <paramref name="propertyName"/> is found;
        /// otherwise, <see cref="JsonValue.Undefined"/>. This parameter is passed uninitialized.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
        public bool TryGetValue(string propertyName, out JsonValue value)
        {
            if (propertyName == null!)
                Throw.ArgumentNullException(nameof(propertyName));
            int index = TryGetIndex(propertyName);
            if (index >= 0)
            {
                value = properties[index].Value;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Removes the property from the <see cref="JsonObject"/> at the specified <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the property to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or greater or equal to <see cref="Count"/>.</exception>
        public void RemoveAt(int index)
        {
            properties.RemoveAt(index);
            nameToIndex = null;
        }

        /// <summary>
        /// Removes one occurrence of the properties with the specific <paramref name="propertyName"/> from the <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to remove from the <see cref="JsonObject"/>.</param>
        /// <returns><see langword="true"/> if a property with <paramref name="propertyName"/> was successfully removed from the <see cref="JsonObject"/>; otherwise, <see langword="false"/>.</returns>
        public bool Remove(string propertyName)
        {
            if (propertyName == null!)
                Throw.ArgumentNullException(nameof(propertyName));

            StringKeyedDictionary<int>? map = nameToIndex;
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
            List<JsonProperty> items = properties;
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

        /// <summary>
        /// Removes all properties from the <see cref="JsonObject"/>.
        /// </summary>
        public void Clear()
        {
            if (Count == 0)
                return;
            properties = new List<JsonProperty>();
            nameToIndex = null;
        }

        /// <summary>
        /// Removes possible duplicate keys from the <see cref="JsonObject"/>, keeping only the last occurrence of each key.
        /// </summary>
        public void EnsureUniqueKeys()
        {
            List<JsonProperty> oldItems = properties;
            if (oldItems.Count == 0)
                return;

            EnsureMap();
            if (nameToIndex!.Count == oldItems.Count)
                return;

            var newItems = new List<JsonProperty>(nameToIndex.Count);
            foreach (KeyValuePair<string, int> map in nameToIndex)
            {
                Debug.Assert(map.Key == oldItems[map.Value].Name);
                newItems.Add(oldItems[map.Value]);
            }

            properties = newItems;
            nameToIndex = null;
        }

        /// <summary>
        /// Copies the properties of the <see cref="JsonObject" /> to an <see cref="Array"/> of <see cref="JsonProperty"/> elements, starting at the specified <paramref name="arrayIndex"/>.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements copied from <see cref="JsonObject"/>.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(JsonProperty[] array, int arrayIndex) => properties.CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="JsonObject"/>.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> instance that can be used to iterate though the elements of the <see cref="JsonObject"/>.</returns>
        public List<JsonProperty>.Enumerator GetEnumerator() => properties.GetEnumerator();

        /// <summary>
        /// Returns a hash code for this <see cref="JsonObject"/> instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            int result = Count;
            if (result == 0)
                return result;

            // ReSharper disable once LoopCanBeConvertedToQuery - performance and readability
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            foreach (JsonProperty property in properties)
            {
                // to avoid recursion including the hashes of the primitive values only (Equals does it, though)
                result = property.Value.Type <= JsonValueType.String
                    ? (result, property.Name, property.Value).GetHashCode()
                    : (result, property.Name, property.Value.Type).GetHashCode();
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance. This method performs a deep comparison.
        /// Allows comparing also to <see cref="JsonValue"/> instances with <see cref="JsonValueType.Object"/>&#160;<see cref="JsonValue.Type"/>.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the specified object is equal to this instance; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object? obj) => obj switch
        {
            JsonObject other => ReferenceEquals(this, other) || Entries.SequenceEqual(other),
            JsonValue { Type: JsonValueType.Object } value => ReferenceEquals(this, value.AsObject) || Entries.SequenceEqual(value.AsObject!),
            _ => false
        };

        /// <summary>
        /// Returns a minimized JSON string that represents this <see cref="JsonObject"/>.
        /// </summary>
        /// <returns>A minimized JSON string that represents this <see cref="JsonObject"/>.</returns>
        public override string ToString()
        {
            var result = new StringBuilder();
            Dump(result);
            return result.ToString();
        }

        /// <summary>
        /// Returns a JSON string that represents this <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="indent">Specifies the indentation string to produce a formatted JSON.
        /// If <see langword="null"/> or empty, then a minimized JSON is returned. Using non-whitespace characters may produce an invalid JSON.</param>
        /// <returns>A JSON string that represents this <see cref="JsonObject"/>.</returns>
        public string ToString(string? indent)
        {
            var result = new StringBuilder(64);
            WriteTo(result, indent);
            return result.ToString();
        }

        /// <summary>
        /// Writes this <see cref="JsonObject"/> instance into a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> to write the <see cref="JsonObject"/> into.</param>
        /// <param name="indent">Specifies the indentation string to produce a formatted JSON.
        /// If <see langword="null"/> or empty, then a minimized JSON is returned. Using non-whitespace characters may produce an invalid JSON. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        public void WriteTo(TextWriter writer, string? indent = null)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, writer CAN be null but MUST NOT be
            => new JsonWriter(writer ?? Throw.ArgumentNullException<TextWriter>(nameof(writer)), indent).Write(this);

        /// <summary>
        /// Writes this <see cref="JsonObject"/> instance into a <see cref="JsonObject"/>.
        /// </summary>
        /// <param name="builder">A <see cref="StringBuilder"/> to write the <see cref="JsonValue"/> into.</param>
        /// <param name="indent">Specifies the indentation string to produce a formatted JSON.
        /// If <see langword="null"/> or empty, then a minimized JSON is returned. Using non-whitespace characters may produce an invalid JSON. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        public void WriteTo(StringBuilder builder, string? indent = null)
        {
            if (builder == null!)
                Throw.ArgumentNullException(nameof(builder));

            // shortcut: we don't need to use a writer
            if (String.IsNullOrEmpty(indent))
            {
                Dump(builder);
                return;
            }

            new JsonWriter(new StringWriter(builder), indent).Write(this);
        }

        /// <summary>
        /// Writes this <see cref="JsonObject"/> instance into a <see cref="Stream"/> using the specified <paramref name="encoding"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write the <see cref="JsonObject"/> into.</param>
        /// <param name="encoding">An <see cref="Encoding"/> that specifies the encoding of the JSON data in the <paramref name="stream"/>.
        /// If <see langword="null"/>, then <see cref="Encoding.UTF8"/> encoding will be used. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <param name="indent">Specifies the indentation string to produce a formatted JSON.
        /// If <see langword="null"/> or empty, then a minimized JSON is returned. Using non-whitespace characters may produce an invalid JSON. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        public void WriteTo(Stream stream, Encoding? encoding = null, string? indent = null)
        {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, stream CAN be null but MUST NOT be
            using var writer = new StreamWriter(stream ?? Throw.ArgumentNullException<Stream>(nameof(stream)), encoding ?? Encoding.UTF8);
            new JsonWriter(writer, indent).Write(this);
        }

        #endregion

        #region Internal Methods

        internal void Dump(StringBuilder builder)
        {
            int len = properties.Count;
            if (len == 0)
            {
                builder.Append("{}");
                return;
            }

            builder.Append('{');
            bool first = true;

            for (var i = 0; i < len; i++)
            {
                JsonProperty property = properties[i];
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

        private void AddItem(in JsonProperty item)
        {
            Debug.Assert(!item.IsDefault);
            properties.Add(item);
            if (nameToIndex != null)
                nameToIndex[item.Name!] = Count - 1;
        }

        private void SetItem(in JsonProperty property)
        {
            Debug.Assert(!property.IsDefault);
            int index = TryGetIndex(property.Name!);
            if (index >= 0)
            {
                properties[index] = property;
                return;
            }

            AddItem(property);
        }

        [MethodImpl(MethodImpl.AggressiveInlining)]
        private int TryGetIndex(string name)
        {
            if (Count >= buildIndexMapThreshold)
            {
                EnsureMap();
                return nameToIndex!.TryGetValue(name, out int index) ? index : -1;
            }

            // reverse traversal so finding the last occurrence of possible duplicates
            for (int i = Count - 1; i >= 0; i--)
            {
                if (properties[i].Name == name)
                    return i;
            }

            return -1;
        }

        [MethodImpl(MethodImpl.AggressiveInlining)]
        private int TryGetIndex(StringSegment name)
        {
            if (Count >= buildIndexMapThreshold)
            {
                EnsureMap();
                return nameToIndex!.TryGetValue(name, out int index) ? index : -1;
            }

            // reverse traversal so finding the last occurrence of possible duplicates
            for (int i = Count - 1; i >= 0; i--)
            {
                if (properties[i].Name == name)
                    return i;
            }

            return -1;
        }

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        [MethodImpl(MethodImpl.AggressiveInlining)]
        private int TryGetIndex(ReadOnlySpan<char> name)
        {
            if (Count >= buildIndexMapThreshold)
            {
                EnsureMap();
                return nameToIndex!.TryGetValue(name, out int index) ? index : -1;
            }

            // reverse traversal so finding the last occurrence of possible duplicates
            for (int i = Count - 1; i >= 0; i--)
            {
                if (properties[i].Name == name)
                    return i;
            }

            return -1;
        }
#endif

        [MethodImpl(MethodImpl.AggressiveInlining)]
        private void EnsureMap()
        {
            if (nameToIndex == null)
                BuildMap();
        }

        private void BuildMap()
        {
            Debug.Assert(nameToIndex == null);

            // Initializing index map. Duplicate names will map to the last occurrence.
            List<JsonProperty> items = properties;
            int count = items.Count;
            var map = new StringKeyedDictionary<int>(count, StringSegmentComparer.OrdinalRandomized);
            for (int i = 0; i < count; i++)
                map[items[i].Name!] = i;
            nameToIndex = map;
        }

        #endregion

        #region Explicitly Implemented Interface Methods

        void ICollection<KeyValuePair<string, JsonValue>>.Add(KeyValuePair<string, JsonValue> item) => Add(item);

        int IList<JsonProperty>.IndexOf(JsonProperty item) => properties.IndexOf(item);
        bool ICollection<JsonProperty>.Contains(JsonProperty item) => properties.Contains(item);
        bool ICollection<KeyValuePair<string, JsonValue>>.Contains(KeyValuePair<string, JsonValue> item) => properties.Contains(item);
        bool IDictionary<string, JsonValue>.ContainsKey(string key) => Contains(key);

#if NET45_OR_GREATER || NETSTANDARD2_0_OR_GREATER
        bool IReadOnlyDictionary<string, JsonValue>.ContainsKey(string key) => Contains(key);
#endif

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
            properties.Select(p => new KeyValuePair<string, JsonValue>(p.Name!, p.Value)).ToList().CopyTo(array, arrayIndex);
        }

        IEnumerator<JsonProperty> IEnumerable<JsonProperty>.GetEnumerator() => properties.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => properties.GetEnumerator();

        IEnumerator<KeyValuePair<string, JsonValue>> IEnumerable<KeyValuePair<string, JsonValue>>.GetEnumerator()
        {
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator - not using Select to prevent boxing the List.Enumerator
            foreach (JsonProperty property in properties)
                yield return new KeyValuePair<string, JsonValue>(property.Name!, property.Value);
        }

        #endregion

        #endregion

        #endregion
    }
}
