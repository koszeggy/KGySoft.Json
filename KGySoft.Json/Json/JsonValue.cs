#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonValue.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

using KGySoft.CoreLibraries;

#endregion

namespace KGySoft.Json
{
    /// <summary>
    /// Represents a value that can be converted to JSON. It can hold JavaScript primitive types
    /// such as <see cref="JsonValueType.Null"/>, <see cref="JsonValueType.Boolean"/>, <see cref="JsonValueType.Number"/> and <see cref="JsonValueType.String"/>,
    /// and it can be assigned also from <see cref="JsonArray"/> and <see cref="JsonObject"/> types.
    /// Its default value represents the JavaScript <see cref="JsonValueType.Undefined"/> value.
    /// Use the <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> or <see cref="O:KGySoft.Json.JsonValue.WriteTo">WriteTo</see> methods to convert it to JSON.
    /// </summary>
    /// <remarks>
    /// <para>A <see cref="JsonValue"/> instance represents any JavaScript type that can appear in JSON (see also the <see cref="Type"/> property
    /// and the <see cref="JsonValueType"/> enumeration), including the <see cref="Undefined"/> value, which is not valid in a JSON document.</para>
    /// <para>The default value of a <see cref="JsonValue"/> instance equals to the <see cref="Undefined"/> field, which represents the <c>undefined</c> type in JavaScript.
    /// Just like in JavaScript, you can add <see cref="Undefined"/> values to arrays and objects but when you "stringify" them
    /// by the <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> or <see cref="O:KGySoft.Json.JsonValue.WriteTo">WriteTo</see> methods, they will be
    /// either replaced with <see cref="Null"/> (in a <see cref="JsonArray"/>) or simply ignored (in a <see cref="JsonObject"/>).
    /// <code lang="C#"><![CDATA[
    /// JsonValue value = default; // = JsonValue.Undefined; = new JsonValue();
    /// 
    /// Console.WriteLine(value); // undefined
    /// Console.WriteLine(value.Type); // Undefined
    /// Console.WriteLine(value.IsUndefined); // True
    /// Console.WriteLine(value == JsonValue.Undefined); // True
    /// ]]></code>
    /// <note><see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> and <see cref="O:KGySoft.Json.JsonValue.WriteTo">WriteTo</see> behave differently
    /// for a standalone <see cref="Undefined"/> value. Whereas <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> produces the string <c>undefined</c>,
    /// <see cref="O:KGySoft.Json.JsonValue.WriteTo">WriteTo</see> will not write anything into the output.</note></para>
    /// <para>The <see cref="Null"/> field represents the <c>null</c> type in JavaScript. Conversion from a .NET <see langword="null"/> is also possible
    /// but only with an explicit type cast.
    /// <code lang="C#"><![CDATA[
    /// JsonValue value = JsonValue.Null; // = (string)null; = (bool?)null; = (JsonObject)null; etc.
    /// 
    /// Console.WriteLine(value); // null
    /// Console.WriteLine(value.Type); // Null
    /// Console.WriteLine(value.IsNull); // True
    /// Console.WriteLine(value == JsonValue.Null); // True
    /// ]]></code></para>
    /// <para>A <see cref="JsonValue"/> can also store a <see cref="JsonValueType.Boolean"/> value, which can be either the value of the <see cref="True"/> or <see cref="False"/> fields.
    /// An implicit conversion from the .NET <see cref="bool"/> type also exists.
    /// <code lang="C#"><![CDATA[
    /// JsonValue value = true; // = JsonValue.True; = new JsonValue(true);
    /// 
    /// Console.WriteLine(value); // true
    /// Console.WriteLine(value.Type); // Boolean
    /// Console.WriteLine(value.AsBoolean); // True
    /// Console.WriteLine(value == JsonValue.True); // True
    /// Console.WriteLine(value == true); // True
    /// ]]></code></para>
    /// <para>A <see cref="JsonValue"/> can also store a <see cref="JsonValueType.Number"/>. Though a JavaScript number is always
    /// a <a href="https://en.wikipedia.org/wiki/Double-precision_floating-point_format" target="_blank">double-precision 64-bit binary format IEEE 754</a> value,
    /// which is the same as the .NET <see cref="double"/> type, a <see cref="JsonValue"/> can hold any number with any precision.
    /// You can use the <see cref="AsNumber"/> property to get the same value as JavaScript would also get and the <see cref="AsLiteral"/> property, which returns
    /// the actual value as it will be dumped when converting the value to JSON. An implicit conversion from the .NET <see cref="double"/> type also exists.
    /// <note type="warning">It is not recommended to write wide numeric .NET types (<see cref="long"/>, <see cref="decimal"/>, etc.) as a
    /// JSON <see cref="JsonValueType.Number"/> because when processed by JavaScript, the precision of these values might be lost without any warning.
    /// If you are sure that you want to store such values as numbers, use the <see cref="CreateNumberUnchecked"/> method or
    /// the <see cref="O:KGySoft.Json.JsonValueExtensions.ToJson">JsonValueExtensions.ToJson</see> overloads with <c>asString: false</c> parameter.</note>
    /// <code lang="C#"><![CDATA[
    /// JsonValue value = 1.25; // = new JsonValue(1.25);
    /// 
    /// Console.WriteLine(value); // 1.25
    /// Console.WriteLine(value.Type); // Number
    /// Console.WriteLine(value.AsNumber); // 1.25
    ///
    /// // Using a long value beyond double precision
    /// long longValue = (1L << 53) + 1;
    /// value = longValue; // this produces a compile-time warning about possible loss of precision
    /// Console.WriteLine(value); // 9007199254740993
    /// Console.WriteLine($"{value.AsNumber:R}"); // 9007199254740992 - this is what JavaScript will see
    /// Console.WriteLine(value.AsLiteral); // 9007199254740993 - this is the actual stored value
    ///
    /// value = longValue.ToJson(asString: false); // this is how the compile-time warning can be avoided
    /// Console.WriteLine(value); // 9007199254740993
    ///
    /// value = longValue.ToJson(); // this is the recommended solution to prevent losing precision
    /// Console.WriteLine(value); // "9007199254740993" - note that ToJson produces strings for wide numeric types by default
    /// ]]></code></para>
    /// <para>A <see cref="JsonValue"/> can also store a <see cref="JsonValueType.String"/>. An implicit conversion from the .NET <see cref="string"/> type also exists.
    /// <code lang="C#"><![CDATA[
    /// JsonValue value = "some \"value\""; // = new JsonValue("some \"value\"");
    /// 
    /// Console.WriteLine(value); // "some \"value\""
    /// Console.WriteLine(value.Type); // String
    /// Console.WriteLine(value.AsString); // some "value"
    /// ]]></code></para>
    /// <para>A <see cref="JsonValue"/> can also store an <see cref="JsonValueType.Array"/>, which is represented by the <see cref="JsonArray"/> type.
    /// An implicit conversion from <see cref="JsonArray"/> also exists. If a <see cref="JsonValue"/> represents an array, then you can use the
    /// <see cref="this[int]">int indexer</see> to access the array elements. Just like in JavaScript, accessing an invalid index returns <see cref="Undefined"/>.
    /// To change array values use the <see cref="JsonArray"/> instance returned by the <see cref="AsArray"/> property.
    /// <code lang="C#"><![CDATA[
    /// JsonValue value = new JsonArray { true, 1, 2.35, JsonValue.Null, "value" };
    /// // which is the shorthand of: new JsonValue(new JsonArray { JsonValue.True, new JsonValue(1), new JsonValue(2.35), JsonValue.Null, new JsonValue("value") });
    /// 
    /// Console.WriteLine(value); // [true,1,2.35,null,"value"]
    /// Console.WriteLine(value.Type); // Array
    /// Console.WriteLine(value[2]); // 2.35
    /// Console.WriteLine(value[42]); // undefined
    /// ]]></code></para>
    /// <para>A <see cref="JsonValue"/> can also store an <see cref="JsonValueType.Object"/>, which is represented by the <see cref="JsonObject"/> type.
    /// An implicit conversion from <see cref="JsonObject"/> also exists. If a <see cref="JsonValue"/> represents an object, then you can use the
    /// <see cref="this[string]">string indexer</see> to access the properties by name. Just like in JavaScript, accessing a nonexistent property returns <see cref="Undefined"/>.
    /// To change object properties use the <see cref="JsonObject"/> instance returned by the <see cref="AsObject"/> property.
    /// <code lang="C#"><![CDATA[
    /// JsonValue value = new JsonObject
    /// {
    ///     ("Bool", true), // which is the shorthand of new JsonProperty("Bool", new JsonValue(true))
    ///     // { "Bool", true }, // alternative syntax on platforms where ValueTuple types are not available
    ///     ("Number", 1.23),
    ///     ("String", "value"),
    ///     ("Object", new JsonObject
    ///     {
    ///        ("Null", JsonValue.Null),
    ///        ("Array", new JsonArray { 42 }),
    ///     })
    /// };
    ///
    /// Console.WriteLine(value); // {"Bool":true,"Number":1.23,"String":"value","Object":{"Null":null,"Array":[42]}}
    /// Console.WriteLine(value.Type); // Object
    /// Console.WriteLine(value["Object"]); // {"Null":null,"Array":[42]}
    /// Console.WriteLine(value["Object"]["Array"]); // [42]
    /// Console.WriteLine(value["Object"]["Array"][0]); // 42
    /// Console.WriteLine(value["UnknownProperty"]); // undefined
    /// ]]></code></para>
    /// <note type="tip">
    /// <see cref="JsonValue"/> members provide conversions to and from types that have their counterpart in JavaScript.
    /// If you want to treat the JSON <see cref="JsonValueType.Number"/> and <see cref="JsonValueType.String"/> types as specific .NET types
    /// such as <see cref="long"/>, <see cref="decimal"/>, <see cref="Enum"/>, <see cref="DateTime"/>, <see cref="Guid"/> and more,
    /// then use the extension methods of the <see cref="JsonValueExtensions"/> class. It also has a bunch of <see cref="O:KGySoft.Json.JsonValueExtensions.ToJson">ToJson</see>
    /// methods that can convert the common .NET types to <see cref="JsonValue"/>.
    /// </note>
    /// </remarks>
    /// <example>
    /// <para>The following examples demonstrate how to serialize and deserialize objects to and from <see cref="JsonValue"/>.
    /// <note><see cref="N:KGySoft.Json">KGySoft.Json</see> has no built-in automated ways to serialize and deserialize C# types.
    /// But in practice even when using other serializers you either need to decorate the C# classes with attributes or you have to
    /// define converters where you specify the exact mapping because .NET classes and JSON objects usually cannot be
    /// just automatically mapped to each other if they both follow the usual naming conventions. The following examples show
    /// how to manually do the serialization and deserialization, which actually can be more effective than letting a serializer
    /// resolve attributes or expressions by reflection.</note>
    /// </para>
    /// <para>Consider the following JSON document:
    /// <code lang="JSON"><![CDATA[
    /// {
    ///   "id": "a4a5b192-fac9-4d7c-a826-1653761fe200",
    ///   "firstName": "John",
    ///   "lastName": "Smith",
    ///   "birth": "19780611",
    ///   "active": true,
    ///   "lastLogin": 1579955315,
    ///   "status": "fully-trusted",
    ///   "balances": [
    ///     {
    ///       "currency": "USD",
    ///       "balance": "23462.4527"
    ///     },
    ///     {
    ///       "currency": "BTC",
    ///       "balance": "0.0567521461"
    ///     }
    ///   ]
    /// }]]></code></para>
    /// <para>And the corresponding C# model:
    /// <code lang="C#"><![CDATA[
    /// public class Account
    /// {
    ///     public Guid Id { get; set; }
    ///     public string FirstName { get; set; }
    ///     public string? MiddleName { get; set; } // optional
    ///     public string LastName { get; set; }
    ///     public DateTime DateOfBirth { get; set; } // yyyyMMdd
    ///     public bool IsActive { get; set; }
    ///     public DateTime? LastLogin { get; set; } // Stored as Unix seconds
    ///     public AccountStatus Status { get; set; } // Stored as lowercase values with hyphens
    ///     public IList<AccountBalance> Balances { get; } = new Collection<AccountBalance>();
    /// }
    ///
    /// public enum AccountStatus { Basic, Confirmed, FullyTrusted, Disabled } // It follows the .NET naming conventions
    /// 
    /// public class AccountBalance
    /// {
    ///    public string Currency { get; set; }
    ///    public decimal Balance { get; set; }
    /// }
    /// ]]></code></para>
    /// <para><strong>Serialization:</strong> When serializing, we just build a <see cref="JsonValue"/>. The actual serialization is done by the <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see>
    /// or <see cref="O:KGySoft.Json.JsonValue.WriteTo">WriteTo</see> methods.
    /// <code lang="C#"><![CDATA[
    /// // in this example conversion is separated from the model as an extension method
    /// public static JsonValue ToJson(this Account acc)
    /// {
    ///     var result = new JsonObject
    ///     {
    ///         ("id", acc.Id.ToJson()), // using ToJson(Guid)
    ///         ("firstName", acc.FirstName), // implicit conversion from string
    ///         ("lastName", acc.LastName), // implicit conversion from string
    ///         ("birth", acc.DateOfBirth.ToJson("yyyyMMdd")), // ToJson(DateTime, string) with exact format
    ///         ("active", acc.IsActive), // implicit conversion from bool
    ///         ("lastLogin", acc.LastLogin.ToJson(JsonDateTimeFormat.UnixSeconds, asString: false)), // Unix seconds as number
    ///         ("status", acc.Status.ToJson(JsonEnumFormat.LowerCaseWithHyphens)) // using ToJson<TEnum>()
    ///     };
    ///
    ///     // adding the optional middle name (it wasn't defined in the example JSON above)
    ///     if (acc.MiddleName != null)
    ///         result.Add("middleName", acc.MiddleName); // or: result["middleName"] = acc.MiddleName;
    ///
    ///     // for collections and nested objects we can delegate the job to other extension methods
    ///     if (acc.Balances.Count > 0)
    ///         result["balances"] = new JsonArray(acc.Balances.Select(b => b.ToJson()));
    ///
    ///     return result; // now it will be converted to JsonValue but we can also change the return type to JsonObject
    /// }
    ///
    /// public static JsonValue ToJson(this AccountBalance balance) => new JsonObject
    /// {
    ///     ("currency", balance.Currency), // implicit conversion from string
    ///     ("balance", balance.Balance.ToJson(asString: true)), // the default of ToJson(decimal) would be a string anyway
    /// };
    /// ]]></code></para>
    /// <para><strong>Deserialization:</strong> Once you have a parsed <see cref="JsonValue"/> (see the <see cref="O:KGySoft.Json.JsonValue.Parse">Parse</see>
    /// and <see cref="O:KGySoft.Json.JsonValue.TryParse">TryParse</see> methods), retrieving the values becomes very straightforward:
    /// <code lang="C#"><![CDATA[
    /// // as above, this is now an extension method but could be even a constructor with JsonValue or JsonObject parameter
    /// public static Account ToAccount(this JsonValue json)
    /// {
    ///     // Here we mainly use the As... methods that return null if the conversion fails
    ///     // but you can also use the TryGet... or Get...OrDefault methods.
    ///     var result = new Account
    ///     {
    ///         Id = json["id"].AsGuid() ?? throw new ArgumentException("'id' is missing or invalid"),
    ///         FirstName = json["firstName"].AsString ?? throw new ArgumentException("'firstName' is missing or invalid"),
    ///         MiddleName = json["middleName"].AsString, // simply returns null if missing (json["middleName"].IsUndefined)
    ///         LastName = json["lastName"].AsString ?? throw new ArgumentException("'lastName' is missing or invalid"),
    ///         DateOfBirth = json["birth"].AsDateTime("yyyyMMdd") ?? throw new ArgumentException("'birth' is missing or invalid"),
    ///         IsActive = json["active"].GetBooleanOrDefault(false), // it will be false if missing (could be AsBoolean ?? false)
    ///         LastLogin = json["lastLogin"].AsDateTime(JsonDateTimeFormat.UnixSeconds), // will be null if missing or invalid
    ///         Status = json["status"].GetEnumOrDefault(true, AccountStatus.Disabled) // true to ignore case and hyphens
    ///     };
    ///
    ///     var balances = json["balances"];
    /// 
    ///     // a missing 'balances' is accepted
    ///     if (balances.IsUndefined) // or: balances == JsonValue.Undefined
    ///         return result;
    ///
    ///     // but if exists, must be an array
    ///     if (balances.Type != JsonValueType.Array) // or: balances.AsArray is not JsonArray
    ///         throw new ArgumentException("'balances' is invalid");
    ///
    ///     foreach (JsonValue balance in balances.AsArray)
    ///         result.Balances.Add(balance.ToBalance());
    ///
    ///     return result;
    /// }
    ///
    /// public static AccountBalance ToBalance(this JsonValue json) => new AccountBalance
    /// {
    ///     Currency = json["currency"].AsString ?? throw new ArgumentException("'currency' is missing or invalid"),
    ///     Balance = json["balance"].AsDecimal() ?? 0m // or AsDecimal(JsonValueType.String) to disallow JSON numbers
    /// };
    /// ]]></code></para>
    /// </example>
    /// <seealso cref="JsonArray"/>
    /// <seealso cref="JsonObject"/>
    /// <seealso cref="JsonValueExtensions"/>
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "False alarm, for some reason ReSharper triggers non_field_members_should_be_pascal_case for all As* members")]
    [Serializable]
    public readonly struct JsonValue : IEquatable<JsonValue>
    {
        #region Constants

        internal const string UndefinedLiteral = "undefined";
        internal const string NullLiteral = "null";
        internal const string TrueLiteral = "true";
        internal const string FalseLiteral = "false";

        #endregion

        #region Fields

        #region Static Fields

        /// <summary>
        /// Represents the JavaScript <c>undefined</c> value. The <see cref="Type"/> of the value is also <see cref="JsonValueType.Undefined"/>.
        /// This is the value of a default <see cref="JsonValue"/> instance.
        /// </summary>
        public static readonly JsonValue Undefined = default;

        /// <summary>
        /// Represents the JavaScript <c>null</c> value. The <see cref="Type"/> of the value is also <see cref="JsonValueType.Null"/>.
        /// </summary>
        public static readonly JsonValue Null = new JsonValue(JsonValueType.Null, NullLiteral);

        /// <summary>
        /// Represents the JavaScript <c>true</c> value. The <see cref="Type"/> of the value is <see cref="JsonValueType.Boolean"/>.
        /// </summary>
        public static readonly JsonValue True = new JsonValue(JsonValueType.Boolean, TrueLiteral);

        /// <summary>
        /// Represents the JavaScript <c>false</c> value. The <see cref="Type"/> of the value is <see cref="JsonValueType.Boolean"/>.
        /// </summary>
        public static readonly JsonValue False = new JsonValue(JsonValueType.Boolean, FalseLiteral);

        #endregion

        #region Instance Fields

        /// <summary>
        /// The stored value. Can be <see langword="null"/> (undefined),
        /// <see cref="string"/> (all primitive types except undefined),
        /// <see cref="JsonArray"/> and <see cref="JsonObject"/>.
        /// </summary>
        private readonly object? value;

        #endregion

        #endregion

        #region Properties and Indexers

        #region Properties
        
        #region Public Properties

        /// <summary>
        /// Gets the JavaScript type of this <see cref="JsonValue"/>.
        /// </summary>
        public JsonValueType Type { get; }

        /// <summary>
        /// Gets whether this <see cref="JsonValue"/> instance has <see cref="JsonValueType.Undefined"/>&#160;<see cref="Type"/> and equals to the <see cref="Undefined"/> instance.
        /// </summary>
        public bool IsUndefined => Type == JsonValueType.Undefined;

        /// <summary>
        /// Gets whether this <see cref="JsonValue"/> instance has <see cref="JsonValueType.Null"/>&#160;<see cref="Type"/> and equals to the <see cref="Null"/> instance.
        /// </summary>
        public bool IsNull => Type == JsonValueType.Null;

        /// <summary>
        /// Gets the <see cref="bool">bool</see> value of this <see cref="JsonValue"/> instance if it has <see cref="JsonValueType.Boolean"/>&#160;<see cref="Type"/>;
        /// or <see langword="null"/>, if its <see cref="Type"/> is not <see cref="JsonValueType.Boolean"/>.
        /// To interpret other types as boolean you can use the <see cref="JsonValueExtensions.AsBoolean"/> extension method instead.
        /// </summary>
        public bool? AsBoolean => Type == JsonValueType.Boolean ? TrueLiteral.Equals(value) : default(bool?);

        /// <summary>
        /// Gets the <see cref="string">string</see> value of this <see cref="JsonValue"/> instance if it has <see cref="JsonValueType.String"/>&#160;<see cref="Type"/>;
        /// or <see langword="null"/>, if its <see cref="Type"/> is not <see cref="JsonValueType.String"/>.
        /// </summary>
        /// <remarks>
        /// <para>This property returns <see langword="null"/> if this <see cref="JsonValue"/> represents a non-string primitive JavaScript literal.
        /// For non-string primitive types you can use the <see cref="AsLiteral"/> property to get their literal value.</para>
        /// <para>This property gets the string value without quotes and escapes. To return it as a parseable JSON string, use the <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> method overloads instead.</para>
        /// </remarks>
        public string? AsString => Type == JsonValueType.String ? (string)value! : null;

        /// <summary>
        /// Gets the numeric value of this <see cref="JsonValue"/> instance if it has <see cref="JsonValueType.Number"/>&#160;<see cref="Type"/>;
        /// or <see langword="null"/>, if its <see cref="Type"/> is not <see cref="JsonValueType.Number"/>.
        /// The returned value is a <see cref="double">double</see> to be conform with JSON <see cref="JsonValueType.Number"/> type.
        /// To retrieve the actual stored raw value use the <see cref="AsLiteral"/> property.
        /// To retrieve the value as .NET numeric types use the methods in the <see cref="JsonValueExtensions"/> class.
        /// </summary>
        /// <remarks>
        /// <note type="warning">The JavaScript <see cref="JsonValueType.Number"/> type is
        /// always a <a href="https://en.wikipedia.org/wiki/Double-precision_floating-point_format" target="_blank">double-precision 64-bit binary format IEEE 754</a> value,
        /// which is the equivalent of the <see cref="double">double</see> type in C#. It is not recommended to store C# <see cref="long">long</see> and <see cref="decimal">decimal</see>
        /// types as JavaScript numbers because their precision might be lost silently if the JSON is processed by JavaScript. If you still want to do so use
        /// the <see cref="O:KGySoft.Json.JsonValueExtensions.ToJson">ToJson</see> extension methods or the <see cref="CreateNumberUnchecked">CreateNumberUnchecked</see> method.</note>
        /// <para>When getting this property the stored underlying string is converted to a <see cref="double">double</see>
        /// so it has the same behavior as a JavaScript <see cref="JsonValueType.Number"/>.</para>
        /// <para>If this <see cref="JsonValue"/> was created from a C# <see cref="long">long</see> or <see cref="decimal">decimal</see> value (see
        /// the <see cref="O:KGySoft.Json.JsonValueExtensions.ToJson">ToJson</see> overloads), then this property may return a different value due to loss of precision.
        /// This is how JavaScript also behaves. To get the value as specific .NET numeric types use the extension methods in the <see cref="JsonValueExtensions"/> class.</para>
        /// <para>To retrieve the stored actual raw value without any conversion you can use the <see cref="AsLiteral"/> property.</para>
        /// <para>This property may return <see langword="null"/> if this instance was created by the <see cref="CreateNumberUnchecked">CreateNumberUnchecked</see>
        /// method and contains an invalid number.</para>
        /// <para>This property can also return <see langword="null"/> when a <c>NaN</c> or <c>Infinity</c>/<c>-Infinity</c> was parsed, which are not valid in JSON.
        /// But even such values can be retrieved as a <see cref="double">double</see> by the <see cref="JsonValueExtensions.AsDouble">AsDouble</see> extension method.</para>
        /// </remarks>
        public double? AsNumber => Type == JsonValueType.Number && Double.TryParse((string)value!, NumberStyles.Float, NumberFormatInfo.InvariantInfo, out double result)
            ? result
            : default(double?);

        /// <summary>
        /// If this <see cref="JsonValue"/> represents a primitive JavaScript type (<see cref="JsonValueType.Undefined"/>, <see cref="JsonValueType.Null"/>, <see cref="JsonValueType.Boolean"/>,
        /// <see cref="JsonValueType.Number"/>, <see cref="JsonValueType.String"/>) or its <see cref="Type"/> is <see cref="JsonValueType.UnknownLiteral"/>, then gets the underlying string literal;
        /// otherwise, gets <see langword="null"/>.
        /// </summary>
        public string? AsLiteral => IsUndefined ? UndefinedLiteral : value as string;

        /// <summary>
        /// Gets this <see cref="JsonValue"/> instance as a <see cref="JsonArray"/> if it has <see cref="JsonValueType.Array"/>&#160;<see cref="Type"/>;
        /// or <see langword="null"/>, if its <see cref="Type"/> is not <see cref="JsonValueType.Array"/>.
        /// </summary>
        /// <remarks>
        /// <para>To get array elements you can also read the numeric <see cref="this[int]">indexer</see> without obtaining the value as a <see cref="JsonArray"/>.</para>
        /// <para>To set/add/remove array elements in a <see cref="JsonValue"/> instance you need to use this property or the explicit cast to <see cref="JsonArray"/>.</para>
        /// </remarks>
        public JsonArray? AsArray => value as JsonArray;

        /// <summary>
        /// Gets this <see cref="JsonValue"/> instance as a <see cref="JsonObject"/> if it has <see cref="JsonValueType.Object"/>&#160;<see cref="Type"/>;
        /// or <see langword="null"/>, if its <see cref="Type"/> is not <see cref="JsonValueType.Object"/>.
        /// </summary>
        /// <remarks>
        /// <para>To get property values you can also read the string <see cref="this[string]">indexer</see> without obtaining the value as a <see cref="JsonObject"/>.</para>
        /// <para>To set/add/remove object properties in a <see cref="JsonValue"/> instance you need to use this property or the explicit cast to <see cref="JsonObject"/>.</para>
        /// </remarks>
        public JsonObject? AsObject => value as JsonObject;

        #endregion

        #region Internal Properties

        /// <summary>
        /// Similar to <see cref="AsLiteral"/> but returns <see langword="null"/> for <see cref="Undefined"/>.
        /// </summary>
        internal string? AsStringInternal => value as string;

        #endregion

        #endregion

        #region Indexers

        /// <summary>
        /// If the type of this <see cref="JsonValue"/> is <see cref="JsonValueType.Array"/> and <paramref name="arrayIndex"/> is within the valid bounds,
        /// then gets the value at the specified <paramref name="arrayIndex"/>; otherwise, returns <see cref="Undefined"/>.
        /// Just like in JavaScript, using an invalid index returns <see cref="Undefined"/>.
        /// </summary>
        /// <param name="arrayIndex">The index of the array element to get.</param>
        /// <returns>The value at the specified <paramref name="arrayIndex"/>, or <see cref="Undefined"/>
        /// if <paramref name="arrayIndex"/> is invalid or <see cref="Type"/> is not <see cref="JsonValueType.Array"/>.</returns>
        public JsonValue this[int arrayIndex] => value is JsonArray array
            ? array[arrayIndex]
            : Undefined;

        /// <summary>
        /// If the type of this <see cref="JsonValue"/> is <see cref="JsonValueType.Object"/> and <paramref name="propertyName"/> denotes an existing property,
        /// then gets the value of the specified <paramref name="propertyName"/>; otherwise, returns <see cref="Undefined"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to return its value.</param>
        /// <returns>The value of the specified <paramref name="propertyName"/>, or <see cref="Undefined"/>
        /// if <paramref name="propertyName"/> is invalid or <see cref="Type"/> is not <see cref="JsonValueType.Object"/>.</returns>
        public JsonValue this[string propertyName] => value is JsonObject obj
            ? obj[propertyName]
            : Undefined;

        /// <summary>
        /// If the type of this <see cref="JsonValue"/> is <see cref="JsonValueType.Object"/> and <paramref name="propertyName"/> denotes an existing property,
        /// then gets the value of the specified <paramref name="propertyName"/>; otherwise, returns <see cref="Undefined"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to return its value.</param>
        /// <returns>The value of the specified <paramref name="propertyName"/>, or <see cref="Undefined"/>
        /// if <paramref name="propertyName"/> is invalid or <see cref="Type"/> is not <see cref="JsonValueType.Object"/>.</returns>
        public JsonValue this[StringSegment propertyName] => value is JsonObject obj
            ? obj[propertyName]
            : Undefined;

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
        /// <summary>
        /// If the type of this <see cref="JsonValue"/> is <see cref="JsonValueType.Object"/> and <paramref name="propertyName"/> denotes an existing property,
        /// then gets the value of the specified <paramref name="propertyName"/>; otherwise, returns <see cref="Undefined"/>.
        /// </summary>
        /// <param name="propertyName">The name of the property to return its value.</param>
        /// <returns>The value of the specified <paramref name="propertyName"/>, or <see cref="Undefined"/>
        /// if <paramref name="propertyName"/> is invalid or <see cref="Type"/> is not <see cref="JsonValueType.Object"/>.</returns>
        public JsonValue this[ReadOnlySpan<char> propertyName] => value is JsonObject obj
            ? obj[propertyName]
            : Undefined;
#endif

        #endregion

        #endregion

        #region Operators

        /// <summary>
        /// Determines whether two specified <see cref="JsonValue"/> instances have the same value.
        /// </summary>
        /// <param name="left">The left argument of the equality check.</param>
        /// <param name="right">The right argument of the equality check.</param>
        /// <returns>The result of the equality check.</returns>
        public static bool operator ==(JsonValue left, JsonValue right) => left.Equals(right);

        /// <summary>
        /// Determines whether two specified <see cref="JsonValue"/> instances have different values.
        /// </summary>
        /// <param name="left">The left argument of the inequality check.</param>
        /// <param name="right">The right argument of the inequality check.</param>
        /// <returns>The result of the inequality check.</returns>
        public static bool operator !=(JsonValue left, JsonValue right) => !left.Equals(right);

        /// <summary>
        /// Performs an implicit conversion from <see cref="bool">bool</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(bool value) => new JsonValue(value);

        /// <summary>
        /// Performs an implicit conversion from nullable <see cref="bool">bool</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(bool? value) => value == null ? Null : value.Value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="string">string</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(string? value) => new JsonValue(value);

        /// <summary>
        /// Performs an implicit conversion from <see cref="double">double</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(double value) => new JsonValue(value);

        /// <summary>
        /// Performs an implicit conversion from nullable <see cref="double">double</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(double? value) => value == null ? Null : value.Value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="JsonArray"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="array">The <see cref="JsonArray"/> to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(JsonArray? array) => new JsonValue(array);

        /// <summary>
        /// Performs an implicit conversion from <see cref="JsonObject"/> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="obj">The <see cref="JsonObject"/> to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(JsonObject? obj) => new JsonValue(obj);

        /// <summary>
        /// Performs an implicit conversion from <see cref="int">int</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(int value)
            // just for performance, the double conversion covers this functionality
            => new JsonValue(JsonValueType.Number, value.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Performs an implicit conversion from nullable <see cref="int">int</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        public static implicit operator JsonValue(int? value)
            // just for performance, the double conversion covers this functionality
            => value == null ? Null : value.Value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="uint">uint</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        [CLSCompliant(false)]
        public static implicit operator JsonValue(uint value)
            // just for performance, the double conversion covers this functionality
            => new JsonValue(JsonValueType.Number, value.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Performs an implicit conversion from nullable <see cref="uint">uint</see> to <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        [CLSCompliant(false)]
        public static implicit operator JsonValue(uint? value)
            // just for performance, the double conversion covers this functionality
            => value == null ? Null : value.Value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="long">long</see> to <see cref="JsonValue"/>.
        /// This operator exists only to produce a warning because otherwise the implicit conversion from double would also match <see cref="long"/> values.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        /// <remarks>
        /// <note><strong>Obsolete Note:</strong> Using <see cref="long"/> as a JSON Number may cause loss of precision.
        /// It is recommended to use the <see cref="JsonValueExtensions.ToJson(long, bool)">ToJson</see> extension method instead.
        /// You can pass <see langword="false"/> to the <c>asString</c> parameter to express your intention and to avoid this warning.</note>
        /// </remarks>
        [Obsolete("Warning: Using Int64 as a JSON Number may cause loss of precision. It is recommended to use the ToJson extension method instead. You can pass false to the asString parameter to express your intention and to avoid this warning.")]
        public static implicit operator JsonValue(long value)
            => new JsonValue(JsonValueType.Number, value.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Performs an implicit conversion from nullable <see cref="long">long</see> to <see cref="JsonValue"/>.
        /// This operator exists only to produce a warning because otherwise the implicit conversion from double would also match <see cref="long"/> values.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        /// <remarks>
        /// <note><strong>Obsolete Note:</strong> Using <see cref="long"/> as a JSON Number may cause loss of precision.
        /// It is recommended to use the <see cref="JsonValueExtensions.ToJson(long?, bool)">ToJson</see> extension method instead.
        /// You can pass <see langword="false"/> to the <c>asString</c> parameter to express your intention and to avoid this warning.</note>
        /// </remarks>
        [Obsolete("Warning: Using Int64 as a JSON Number may cause loss of precision. It is recommended to use the ToJson extension method instead. You can pass false to the asString parameter to express your intention and to avoid this warning.")]
        public static implicit operator JsonValue(long? value)
            => value == null ? Null : value.Value;

        /// <summary>
        /// Performs an implicit conversion from <see cref="ulong">ulong</see> to <see cref="JsonValue"/>.
        /// This operator exists only to produce a warning because otherwise the implicit conversion from double would also match <see cref="ulong"/> values.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        /// <remarks>
        /// <note><strong>Obsolete Note:</strong> Using <see cref="ulong"/> as a JSON Number may cause loss of precision.
        /// It is recommended to use the <see cref="JsonValueExtensions.ToJson(ulong, bool)">ToJson</see> extension method instead.
        /// You can pass <see langword="false"/> to the <c>asString</c> parameter to express your intention and to avoid this warning.</note>
        /// </remarks>
        [CLSCompliant(false)]
        [Obsolete("Warning: Using UInt64 as a JSON Number may cause loss of precision. It is recommended to use the ToJson extension method instead. You can pass false to the asString parameter to express your intention and to avoid this warning.")]
        public static implicit operator JsonValue(ulong value)
            => new JsonValue(JsonValueType.Number, value.ToString(CultureInfo.InvariantCulture));

        /// <summary>
        /// Performs an implicit conversion from nullable <see cref="ulong">ulong</see> to <see cref="JsonValue"/>.
        /// This operator exists only to produce a warning because otherwise the implicit conversion from double would also match <see cref="ulong"/> values.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonValue"/>.</param>
        /// <returns>
        /// A <see cref="JsonValue"/> instance that represents the original value.
        /// </returns>
        /// <remarks>
        /// <note><strong>Obsolete Note:</strong> Using <see cref="ulong"/> as a JSON Number may cause loss of precision.
        /// It is recommended to use the <see cref="JsonValueExtensions.ToJson(ulong, bool)">ToJson</see> extension method instead.
        /// You can pass <see langword="false"/> to the <c>asString</c> parameter to express your intention and to avoid this warning.</note>
        /// </remarks>
        [CLSCompliant(false)]
        [Obsolete("Warning: Using UInt64 as a JSON Number may cause loss of precision. It is recommended to use the ToJson extension method instead. You can pass false to the asString parameter to express your intention and to avoid this warning.")]
        public static implicit operator JsonValue(ulong? value)
            => value == null ? Null : value.Value;

        /// <summary>
        /// Performs an explicit conversion from <see cref="JsonValue"/> to nullable <see cref="bool">bool</see>.
        /// The conversion succeeds if the <see cref="Type"/> property is <see cref="JsonValueType.Null"/> or <see cref="JsonValueType.Boolean"/>; otherwise, an <see cref="InvalidCastException"/> is thrown.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="bool">bool</see>.</param>
        /// <returns>
        /// A <see cref="bool">bool</see> instance that represents the original value.
        /// </returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> does not represent a boolean value.</exception>
        public static explicit operator bool?(JsonValue value) => value.IsNull ? null : value.AsBoolean ?? Throw.InvalidCastException<bool>(Res.JsonValueInvalidCast<bool>(value.Type));

        /// <summary>
        /// Performs an explicit conversion from <see cref="JsonValue"/> to <see cref="string">string</see>.
        /// The conversion succeeds if the <see cref="Type"/> property is <see cref="JsonValueType.Null"/> or <see cref="JsonValueType.String"/>; otherwise, an <see cref="InvalidCastException"/> is thrown.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="string">string</see>.</param>
        /// <returns>
        /// A <see cref="string">string</see> instance that represents the original value.
        /// </returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> does not represent a string value.</exception>
        public static explicit operator string?(JsonValue value) => value.IsNull ? null : value.AsString ?? Throw.InvalidCastException<string>(Res.JsonValueInvalidCast<string>(value.Type));

        /// <summary>
        /// Performs an explicit conversion from <see cref="JsonValue"/> to nullable <see cref="double">double</see>.
        /// The conversion succeeds if the <see cref="Type"/> property is <see cref="JsonValueType.Null"/> or <see cref="JsonValueType.Number"/>; otherwise, an <see cref="InvalidCastException"/> is thrown.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="double">double</see>.</param>
        /// <returns>
        /// A <see cref="double">double</see> instance that represents the original value.
        /// </returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> does not represent a numeric value.</exception>
        public static explicit operator double?(JsonValue value) => value.IsNull ? null : value.AsNumber ?? Throw.InvalidCastException<double>(Res.JsonValueInvalidCast<double>(value.Type));

        /// <summary>
        /// Performs an explicit conversion from <see cref="JsonValue"/> to <see cref="JsonArray"/>.
        /// The conversion succeeds if the <see cref="Type"/> property is <see cref="JsonValueType.Null"/> or <see cref="JsonValueType.Array"/>; otherwise, an <see cref="InvalidCastException"/> is thrown.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonArray"/>.</param>
        /// <returns>
        /// A <see cref="JsonArray"/> instance that represents the original value.
        /// </returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> does not represent an array.</exception>
        public static explicit operator JsonArray?(JsonValue value) => value.IsNull ? null : value.AsArray ?? Throw.InvalidCastException<JsonArray>(Res.JsonValueInvalidCast<JsonArray>(value.Type));

        /// <summary>
        /// Performs an explicit conversion from <see cref="JsonValue"/> to <see cref="JsonObject"/>.
        /// The conversion succeeds if the <see cref="Type"/> property is <see cref="JsonValueType.Null"/> or <see cref="JsonValueType.Object"/>; otherwise, an <see cref="InvalidCastException"/> is thrown.
        /// </summary>
        /// <param name="value">The value to be converted to a <see cref="JsonObject"/>.</param>
        /// <returns>
        /// A <see cref="JsonObject"/> instance that represents the original value.
        /// </returns>
        /// <exception cref="InvalidCastException"><paramref name="value"/> does not represent an object.</exception>
        public static explicit operator JsonObject?(JsonValue value) => value.IsNull ? null : value.AsObject ?? Throw.InvalidCastException<JsonObject>(Res.JsonValueInvalidCast<JsonArray>(value.Type));

        #endregion

        #region Constructors

        #region Public Constructors

        /// <summary>
        /// Initializes a new <see cref="JsonValue"/> struct that represents a boolean value.
        /// An implicit conversion from the <see cref="bool">bool</see> type also exists.
        /// </summary>
        /// <param name="value">The value to initialize the <see cref="JsonValue"/> from.</param>
        public JsonValue(bool value) => this = value ? True : False;

        /// <summary>
        /// Initializes a new <see cref="JsonValue"/> struct that represents a string.
        /// An implicit conversion from the <see cref="string">string</see> type also exists.
        /// </summary>
        /// <param name="value">The value to initialize the <see cref="JsonValue"/> from.</param>
        public JsonValue(string? value)
        {
            if (value == null)
            {
                this = Null;
                return;
            }

            Type = JsonValueType.String;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new <see cref="JsonValue"/> struct that represents a number.
        /// An implicit conversion from the <see cref="double">double</see> type also exists.
        /// Some .NET numeric types such as <see cref="long">long</see> and <see cref="decimal">decimal</see> are not recommended to be encoded as JSON numbers.
        /// Use the <see cref="O:KGySoft.Json.JsonValueExtensions.ToJson">ToJson</see> extension methods if you still want to do so.
        /// </summary>
        /// <param name="value">The value to initialize the <see cref="JsonValue"/> from.</param>
        /// <remarks>
        /// <note type="warning">The JavaScript <see cref="JsonValueType.Number"/> type is
        /// always a <a href="https://en.wikipedia.org/wiki/Double-precision_floating-point_format" target="_blank">double-precision 64-bit binary format IEEE 754</a> value,
        /// which is the equivalent of the <see cref="double">double</see> type in C#. It is not recommended to store C# <see cref="long">long</see> and <see cref="decimal">decimal</see>
        /// types as JavaScript numbers because their precision might be lost silently if the JSON is processed by JavaScript.</note>
        /// <note><list type="bullet">
        /// <item>JavaScript Number type is actually a double. Other large numeric types (<see cref="long">[u]long</see>/<see cref="decimal">decimal</see>) must be encoded as string to
        /// prevent loss of precision at a real JavaScript side. If you are sure that you want to forcibly treat such types as numbers use
        /// the <see cref="O:KGySoft.Json.JsonValueExtensions.ToJson">ToJson</see> overloads and pass <see langword="true"/> to their <c>asString</c> parameter.
        /// You can use also the <see cref="CreateNumberUnchecked">CreateNumberUnchecked</see> method to create a JSON number directly from a string.</item>
        /// <item>This method allows <see cref="Double.NaN"/> and <see cref="Double.PositiveInfinity"/>/<see cref="Double.NegativeInfinity"/>,
        /// which are also invalid in JSON. Parsing these values works though their <see cref="Type"/> will be <see cref="JsonValueType.UnknownLiteral"/> after parsing.</item>
        /// </list></note>
        /// </remarks>
        /// <seealso cref="AsNumber"/>
        /// <seealso cref="CreateNumberUnchecked"/>
        public JsonValue(double value)
        {
            Type = JsonValueType.Number;
            this.value = value.ToString("R", NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Initializes a new <see cref="JsonValue"/> struct that represents an array.
        /// </summary>
        /// <param name="array">The <see cref="JsonArray"/> to initialize the <see cref="JsonValue"/> from.</param>
        public JsonValue(JsonArray? array)
        {
            if (array == null)
            {
                this = Null;
                return;
            }

            Type = JsonValueType.Array;
            value = array;
        }

        /// <summary>
        /// Initializes a new <see cref="JsonValue"/> struct that represents an object.
        /// </summary>
        /// <param name="obj">The <see cref="JsonObject"/> to initialize the <see cref="JsonValue"/> from.</param>
        public JsonValue(JsonObject? obj)
        {
            if (obj == null)
            {
                this = Null;
                return;
            }

            Type = JsonValueType.Object;
            value = obj;
        }

        #endregion

        #region Internal Constructors

        internal JsonValue(JsonValueType type, string value)
        {
            this.value = value;
            Type = type;
        }

        #endregion

        #endregion

        #region Methods

        #region Static Methods

        /// <summary>
        /// Reads a <see cref="JsonValue"/> from a <see cref="TextReader"/> that contains JSON data.
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> that will be read for the <see cref="JsonValue"/> content.</param>
        /// <returns>A <see cref="JsonValue"/> that contains the JSON data that was read from the specified <see cref="TextReader"/>.</returns>
        public static JsonValue Parse(TextReader reader)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, reader CAN be null but MUST NOT be
            => JsonParser.ParseValue(reader ?? Throw.ArgumentNullException<TextReader>(nameof(reader)));

        /// <summary>
        /// Reads a <see cref="JsonValue"/> from a <see cref="Stream"/> that contains JSON data.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> that will be read for the <see cref="JsonValue"/> content.</param>
        /// <param name="encoding">An <see cref="Encoding"/> that specifies the encoding of the JSON data in the <paramref name="stream"/>.
        /// If <see langword="null"/>, then <see cref="Encoding.UTF8"/> encoding will be used. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <returns>A <see cref="JsonValue"/> that contains the JSON data that was read from the specified <paramref name="stream"/>.</returns>
        public static JsonValue Parse(Stream stream, Encoding? encoding = null)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, stream CAN be null but MUST NOT be
            => Parse(new StreamReader(stream ?? Throw.ArgumentNullException<Stream>(nameof(stream)), encoding ?? Encoding.UTF8));

        /// <summary>
        /// Reads a <see cref="JsonValue"/> from a <see cref="string">string</see> that contains JSON data.
        /// </summary>
        /// <param name="s">A string that will be read for the <see cref="JsonValue"/> content.</param>
        /// <returns>A <see cref="JsonValue"/> that contains the JSON data that was read from the specified string.</returns>
        public static JsonValue Parse(string s)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, s CAN be null but MUST NOT be
            => Parse(new StringReader(s ?? Throw.ArgumentNullException<string>(nameof(s))));

        /// <summary>
        /// Tries to read a <see cref="JsonValue"/> from a <see cref="TextReader"/> that contains JSON data.
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> that will be read for the <see cref="JsonValue"/> content.</param>
        /// <param name="value">When this method returns <see langword="true"/>, the result of the parsing;
        /// otherwise, <see cref="Undefined"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(TextReader reader, out JsonValue value)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, reader CAN be null but MUST NOT be
            => JsonParser.TryParseValue(reader ?? Throw.ArgumentNullException<TextReader>(nameof(reader)), out value);

        /// <summary>
        /// Tries to read a <see cref="JsonValue"/> from a <see cref="Stream"/> that contains JSON data.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> that will be read for the <see cref="JsonValue"/> content.</param>
        /// <param name="value">When this method returns <see langword="true"/>, the result of the parsing;
        /// otherwise, <see cref="Undefined"/>. This parameter is passed uninitialized.</param>
        /// <param name="encoding">An <see cref="Encoding"/> that specifies the encoding of the JSON data in the <paramref name="stream"/>.
        /// If <see langword="null"/>, then <see cref="Encoding.UTF8"/> encoding will be used. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(Stream stream, out JsonValue value, Encoding? encoding = null)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, stream CAN be null but MUST NOT be
            => TryParse(new StreamReader(stream ?? Throw.ArgumentNullException<Stream>(nameof(stream)), encoding ?? Encoding.UTF8), out value);

        /// <summary>
        /// Tries to read a <see cref="JsonValue"/> from a <see cref="string">string</see> that contains JSON data.
        /// </summary>
        /// <param name="s">A string that will be read for the <see cref="JsonValue"/> content.</param>
        /// <param name="value">When this method returns <see langword="true"/>, the result of the parsing;
        /// otherwise, <see cref="Undefined"/>. This parameter is passed uninitialized.</param>
        /// <returns><see langword="true"/> if the parsing was successful; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse(string s, out JsonValue value)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, s CAN be null but MUST NOT be
            => TryParse(new StringReader(s ?? Throw.ArgumentNullException<string>(nameof(s))), out value);


        /// <summary>
        /// Creates a <see cref="JsonValue"/> that forcibly treats <paramref name="value"/> as a JSON number,
        /// even if it cannot be represented as a valid number in JavaScript.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><see cref="Null"/>, if <paramref name="value"/> was <see langword="null"/>; otherwise, a <see cref="JsonValue"/>
        /// that contains the specified value as a <see cref="JsonValueType.Number"/>.</returns>
        /// <remarks>
        /// <note type="warning">This method makes possible to create invalid JSON.</note>
        /// <para>The <see cref="Type"/> property of the result will return <see cref="JsonValueType.Number"/> even if <paramref name="value"/> is not a valid number.</para>
        /// <para>The <see cref="AsLiteral"/> property will return the specified <paramref name="value"/>.</para>
        /// <para>The <see cref="AsNumber"/> property of the result may return a less precise value, or even <see langword="null"/>,
        /// though serializing to JSON by the <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> and <see cref="O:KGySoft.Json.JsonValue.WriteTo">WriteTo</see>
        /// methods preserves the specified <paramref name="value"/>.</para>
        /// </remarks>
        public static JsonValue CreateNumberUnchecked(string? value) => value == null ? Null : new JsonValue(JsonValueType.Number, value);

        /// <summary>
        /// Creates a <see cref="JsonValue"/> that forcibly treats <paramref name="value"/> as a JSON literal, even if it is invalid in JSON.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><see cref="Null"/>, if <paramref name="value"/> was <see langword="null"/>; otherwise, a <see cref="JsonValue"/>,
        /// whose <see cref="Type"/> property returns <see cref="JsonValueType.UnknownLiteral"/>.</returns>
        /// <remarks>
        /// <note type="warning">This method makes possible to create invalid JSON.</note>
        /// <para>The <see cref="Type"/> property of the result will return <see cref="JsonValueType.UnknownLiteral"/> even if <paramref name="value"/> is actually a valid JSON literal.</para>
        /// <para>The <see cref="AsLiteral"/> property will return the specified <paramref name="value"/>.</para>
        /// <para>Serializing to JSON by the <see cref="O:KGySoft.Json.JsonValue.ToString">ToString</see> and <see cref="O:KGySoft.Json.JsonValue.WriteTo">WriteTo</see>
        /// methods preserves the specified <paramref name="value"/>.</para>
        /// </remarks>
        public static JsonValue CreateLiteralUnchecked(string? value) => value == null ? Null : new JsonValue(JsonValueType.UnknownLiteral, value);

        #endregion

        #region Instance Methods

        #region Public Methods

        /// <summary>
        /// Returns a minimized JSON string that represents this <see cref="JsonValue"/>.
        /// </summary>
        /// <returns>A minimized JSON string that represents this <see cref="JsonValue"/>.</returns>
        public override string ToString()
        {
            if (Type <= JsonValueType.Number)
                return AsLiteral!;

            var result = new StringBuilder(value is string s ? s.Length + 2 : 64);
            Dump(result);
            return result.ToString();
        }

        /// <summary>
        /// Returns a JSON string that represents this <see cref="JsonValue"/>.
        /// </summary>
        /// <param name="indent">Specifies the indentation string to produce a formatted JSON.
        /// If <see langword="null"/> or empty, then a minimized JSON is returned. Using non-whitespace characters may produce an invalid JSON.</param>
        /// <returns>A JSON string that represents this <see cref="JsonValue"/>.</returns>
        public string ToString(string? indent)
        {
            if (Type <= JsonValueType.Number || String.IsNullOrEmpty(indent))
                return ToString();

            var result = new StringBuilder(value is string s ? s.Length + 2 : 64);
            WriteTo(result, indent);
            return result.ToString();
        }

        /// <summary>
        /// Indicates whether the current <see cref="JsonValue"/> instance is equal to another one specified in the <paramref name="other"/> parameter.
        /// </summary>
        /// <param name="other">A <see cref="JsonValue"/> instance to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <see langword="false"/>.</returns>
        public bool Equals(JsonValue other) => Type == other.Type && Equals(value, other.value);

        /// <summary>
        /// Determines whether the specified <see cref="object">object</see> is equal to this instance.
        /// Allows comparing also to <see cref="JsonArray"/>, <see cref="JsonObject"/>, <see cref="string">string</see>, <see cref="bool">bool</see> and .NET numeric types.
        /// </summary>
        /// <param name="obj">An <see cref="object"/> to compare with this instance.</param>
        /// <returns><see langword="true"/> if the specified object is equal to this instance; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object? obj) => obj switch
        {
            JsonValue other => Equals(other),
            JsonArray array => array.Equals(value),
            JsonObject @object => @object.Equals(value),
            string s => AsString == s,
            bool b => AsBoolean == b,
            // Numbers: comparing their string value
            IConvertible convertible => Type == JsonValueType.Number && convertible.GetTypeCode() is >= TypeCode.SByte and <= TypeCode.Decimal
                && convertible.ToString(NumberFormatInfo.InvariantInfo) == AsStringInternal,
            _ => false
        };

        /// <summary>
        /// Returns a hash code for this <see cref="JsonValue"/> instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode() => (Type, value).GetHashCode();

        /// <summary>
        /// Writes this <see cref="JsonValue"/> instance into a <see cref="TextReader"/>.
        /// </summary>
        /// <param name="writer">A <see cref="TextWriter"/> to write the <see cref="JsonValue"/> into.</param>
        /// <param name="indent">Specifies the indentation string to produce a formatted JSON.
        /// If <see langword="null"/> or empty, then a minimized JSON is returned. Using non-whitespace characters may produce an invalid JSON. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        public void WriteTo(TextWriter writer, string? indent = null)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, writer CAN be null but MUST NOT be
            => new JsonWriter(writer ?? Throw.ArgumentNullException<TextWriter>(nameof(writer)), indent).Write(this);

        /// <summary>
        /// Writes this <see cref="JsonValue"/> instance into a <see cref="StringBuilder"/>.
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
        /// Writes this <see cref="JsonValue"/> instance into a <see cref="Stream"/> using the specified <paramref name="encoding"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to write the <see cref="JsonValue"/> into.</param>
        /// <param name="encoding">An <see cref="Encoding"/> that specifies the encoding of the JSON data in the <paramref name="stream"/>.
        /// If <see langword="null"/>, then <see cref="Encoding.UTF8"/> encoding will be used. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        /// <param name="indent">Specifies the indentation string to produce a formatted JSON.
        /// If <see langword="null"/> or empty, then a minimized JSON is returned. Using non-whitespace characters may produce an invalid JSON. This parameter is optional.
        /// <br/>Default value: <see langword="null"/>.</param>
        public void WriteTo(Stream stream, Encoding? encoding = null, string? indent = null)
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract - false alarm, stream CAN be null but MUST NOT be
            => new JsonWriter(new StreamWriter(stream ?? Throw.ArgumentNullException<Stream>(nameof(stream)), encoding ?? Encoding.UTF8), indent).Write(this);

        #endregion

        #region Internal Methods

        internal void Dump(StringBuilder builder)
        {
            switch (Type)
            {
                case JsonValueType.String:
                    JsonWriter.WriteJsonString(builder, AsLiteral!);
                    return;

                case JsonValueType.Object:
                    AsObject!.Dump(builder);
                    return;

                case JsonValueType.Array:
                    AsArray!.Dump(builder);
                    return;

                default:
                    // In Dump undefined may occur only as a root from WriteTo(StringBuilder,null), in which case it is skipped
                    if (!IsUndefined)
                        builder.Append(AsLiteral);
                    return;
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
