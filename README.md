[![KGy SOFT .net](https://user-images.githubusercontent.com/27336165/124292367-c93f3d00-db55-11eb-8003-6d943ee7d7fa.png)](https://kgysoft.net)

# KGy SOFT JSON Libraries

KGy SOFT JSON Libraries offer a simple and fast way to build and parse JSON content.

[![Website](https://img.shields.io/website/https/kgysoft.net/json.svg)](https://kgysoft.net/json)
[![Online Help](https://img.shields.io/website/https/docs.kgysoft.net/json.svg?label=online%20help&up_message=available)](https://docs.kgysoft.net/json)
[![GitHub Repo](https://img.shields.io/github/repo-size/koszeggy/KGySoft.Json.svg?label=github)](https://github.com/koszeggy/KGySoft.Json)
[![Nuget](https://img.shields.io/nuget/vpre/KGySoft.Json.svg)](https://www.nuget.org/packages/KGySoft.Json)

## Table of Contents:
1. [What is KGySoft.Json](#what-is-kgysoftjson)
   - [What's wrong with JSON.NET?](#whats-wrong-with-jsonnet)
   - [What's wrong with System.Text.Json?](#whats-wrong-with-systemtextjson)
2. [Examples](#examples)
   - [Simple syntax](#simple-syntax)
   - [Writing JSON](#writing-json)
   - [Parsing JSON](#parsing-json)
   - [Manipulating JSON](#manipulating-json)
3. [Performance Comparisons](#performance-comparisons)
   - [Parse test](#parse-test)
   - [Access element test](#access-element-test)
   - [Write minimized JSON test](#write-minimized-json-test)
4. [Download](#download)
   - [Download Binaries](#download-binaries)
5. [Project Site](#project-site)
6. [Documentation](#documentation)
7. [Release Notes](#release-notes)
8. [License](#license)

## What is KGySoft.Json

First of all, what it is **not**: It's _not_ a serializer (though see the **Examples** section of the [`JsonValue`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonValue.htm) type). It is rather a simple Domain Object Model (DOM) for JSON (or LINQ to JSON if you like), which makes possible to manipulate JSON content in memory in an object-oriented way (similarly to `XDocument` and its friends for XML).

### What's wrong with JSON.NET?

Nothing. It knows everything (and more). It was just too heavy-weight for my needs (see also [Performance Comparisons](#performance-comparisons)) and I didn't like that a lot of libraries reference different versions of it, causing version conflicts. Note though that it's _not_ the fault of JSON.NET; it's just the consequence of popularity (the same could be true even for this library if it was so popular... but I don't really think it could be a real issue anytime soon).

### What's wrong with System.Text.Json?

Well, it's a bit different thing. As an in-memory JSON tool, it is read-only (`JsonDocument`/`JsonElement`) so you cannot build in-memory JSON content with it. It was introduced with .NET Core 3.0, so below that you have to use NuGet packages, which may lead to the same issue as above (but even with NuGet, it's not supported below .NET Framework 4.6.1). Apart from those issues, it's really fast, and mostly allocation free (well, as long as your source is already in UTF8 and you don't access string elements).

> _.NET 6 Update:_ Starting with .NET 6, a new `System.Text.Json.Nodes` namespace has been introduced that supports JSON DOM manipulation as well, and uses a very similar approach to this library. Even the member names are very similar. There are some important differences, though: the System version does not tolerate getting lost in the domain, eg. `json["someProperty"][42]["WhereAmI"]` will throw exceptions instead of returning `undefined`. Similarly, the System `AsObject`/`AsArray` members may throw exceptions, whereas in the KGy SOFT version these return nullable results. If you can target at least .NET 6, then the choice can be a matter of taste. See also the [performance comparisons](#performance-comparisons) below.

## Examples

> _Tip:_ See also the examples at the **Remarks** section of the [`JsonValue`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonValue.htm) type.

### Simple Syntax

The [`JsonValue`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonValue.htm) type is the base building block of JSON content, which can be considered as a representation of a JavaScript variable (see also the [`JsonValueType`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonValueType.htm) enumeration). It also behaves somewhat similarly to JavaScript:

#### `undefined`

```cs
JsonValue value = default; // or: value = JsonValue.Undefined; value = new JsonValue();

Console.WriteLine(value); // undefined
Console.WriteLine(value.Type); // Undefined
Console.WriteLine(value.IsUndefined); // True
```

#### `null`

```cs
JsonValue value = JsonValue.Null; // or: value = (bool?)null; value = (string?)null; etc.

Console.WriteLine(value); // null
Console.WriteLine(value.Type); // Null
Console.WriteLine(value.IsNull); // True
```

#### Boolean

```cs
JsonValue value = true; // or: value = JsonValue.True; value = new JsonValue(true);

Console.WriteLine(value); // true
Console.WriteLine(value.Type); // Boolean
Console.WriteLine(value.AsBoolean); // True
```

#### Number

```cs
JsonValue value = 1.23; // or: value = new JsonValue(1.23);

Console.WriteLine(value); // 1.23
Console.WriteLine(value.Type); // Number
Console.WriteLine(value.AsNumber); // 1.23
```

> _Note:_  A JavaScript number is always a [double-precision 64-bit binary format IEEE 754 value](https://en.wikipedia.org/wiki/Double-precision_floating-point_format), which has the same precision as the .NET `Double` type. Though supported, it is **not recommended** to encode any numeric .NET type as a JSON Number (eg. `Int64` or `Decimal`) because when such a JSON content is processed by JavaScript the precision might be silently lost.

```cs
// Using a long value beyond double precision
long longValue = (1L << 53) + 1;

JsonValue value = longValue; // this produces a compile-time warning about possible loss of precision
value = longValue.ToJson(asString: false); // this is how the compile-time warning can be avoided

Console.WriteLine(value); // 9007199254740993
Console.WriteLine($"{value.AsNumber:R}"); // 9007199254740992 - this is how JavaScript interprets it
Console.WriteLine(value.AsLiteral); // 9007199254740993 - this is the actual stored value
```

#### String

```cs
JsonValue value = "value"; // or: value = new JsonValue("value");

Console.WriteLine(value); // "value"
Console.WriteLine(value.Type); // String
Console.WriteLine(value.AsString); // value
```

#### Array

```cs
JsonValue array = new JsonArray { true, 1, 2.35, JsonValue.Null, "value" };
// which is the shorthand of: new JsonValue(new JsonArray { JsonValue.True, new JsonValue(1), new JsonValue(2.35), JsonValue.Null, new JsonValue("value") });

Console.WriteLine(array); // [true,1,2.35,null,"value"]
Console.WriteLine(array.Type); // Array
Console.WriteLine(array[2]); // 2.35
Console.WriteLine(array[42]); // undefined
```

#### Object

```cs
JsonValue obj = new JsonObject
{
    ("Bool", true), // which is the shorthand of new JsonProperty("Bool", new JsonValue(true))
    // { "Bool", true }, // alternative syntax on platforms where ValueTuple types are not available
    ("Number", 1.23),
    ("String", "value"),
    ("Object", new JsonObject
    {
       ("Null", JsonValue.Null),
       ("Array", new JsonArray { 42 }),
    })
};

Console.WriteLine(obj); // {"Bool":true,"Number":1.23,"String":"value","Object":{"Null":null,"Array":[42]}}
Console.WriteLine(obj.Type); // Object
Console.WriteLine(obj["Object"]["Array"][0]); // 42
Console.WriteLine(obj["UnknownProperty"]); // undefined
```

#### Interop with common .NET types

Though the [`JsonValue`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonValue.htm) type has a JavaScript-like approach (eg. it has a single `AsNumber` property with nullable `double` return type for any numbers) the [`JsonValueExtensions`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonValueExtensions.htm) class provides .NET type specific conversions for most .NET numeric types, including `Int64`, `Decimal`, `BigInteger` and more. Additionally, it offers conversions for `Enum`, `DateTime`, `DateTimeOffset`, `TimeSpan`, `DateOnly`, `TimeOnly` and `Guid` types as well.

```cs
// Use the ToJson extension methods to convert common .NET types to JsonValue
var obj = new JsonObject
{
    ("Id", Guid.NewGuid().ToJson()),
    ("Timestamp", DateTime.UtcNow.ToJson(JsonDateTimeFormat.UnixMilliseconds)),
    ("Status", someEnumValue.ToJson(JsonEnumFormat.LowerCaseWithHyphens)),
    ("Balance", someDecimalValue.ToJson(asString: true))
};
```

To obtain the values you can use 3 different approaches:
```cs
// 1.) TryGet... methods: bool return value and an out parameter
if (obj["Balance"].TryGetDecimal(out decimal value)) {}

// 2.) As... methods: nullable return type
decimal? valueOrNull = obj["Balance"].AsDecimal(); // or: AsDecimal(JsonValueType.String) to accept strings only

// 3.) Get...OrDefault: non-nullable return type;
decimal balance = obj["Balance"].GetDecimalOrDefault(); // or: GetDecimalOrDefault(-1m) to specify a default value
```

> _Tip:_ There are several predefined formats for enums (see [`JsonEnumFormat`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonEnumFormat.htm)), `DateTime`, `DateTimeOffset` and `DateOnly` types (see [`JsonDateTimeFormat`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonDateTimeFormat.htm)) and `TimeSpan`/`TimeOnly` values (see [`JsonTimeFormat`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonTimeFormat.htm)).

### Writing JSON

Converting a `JsonValue` (or `JsonArray`/`JsonObject`) to a JSON string is as easy as calling the `ToString` method. To produce an indented result you can pass a string to the `ToString` method. Alternatively, you can use the `WriteTo` method to write the JSON content into a `Stream` (you can also specify an `Encoding`), `TextWriter` or `StringBuilder`.

```cs
JsonValue obj = new JsonObject
{
    ("Bool", true), // which is the shorthand of new JsonProperty("Bool", new JsonValue(true))
    // { "Bool", true }, // alternative syntax on platforms where ValueTuple types are not available
    ("Number", 1.23),
    ("String", "value"),
    ("Object", new JsonObject
    {
       ("Null", JsonValue.Null),
       ("Array", new JsonArray { 42 }),
    })
};

obj.WriteTo(someStream, Encoding.UTF8); // to write it into a stream (UTF8 is actually the default encoding)
Console.WriteLine(obj.ToString("  ")); // to print a formatted JSON using two spaces as indentation
```

The example above prints the following in the Console:
```json
{
  "Bool": true,
  "Number": 1.23,
  "String": "value",
  "Object": {
    "Null": null,
    "Array": [
      42
    ]
  }
}
```

### Parsing JSON

Use the [`JsonValue.Parse`](https://docs.kgysoft.net/json/?topic=html/Overload_KGySoft_Json_JsonValue_Parse.htm)/[`TryParse`](https://docs.kgysoft.net/json/?topic=html/Overload_KGySoft_Json_JsonValue_TryParse.htm) methods to parse a JSON document from `string`, `TextReader` or `Stream`. You can also specify an `Encoding` for the `Stream` overload. If you expect the result to be an array or an object, then you can find these methods on the [`JsonArray`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonArray.htm) and [`JsonObject`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonObject.htm) types as well.

As you could see [above](#object) navigation in a parsed object graph is pretty straightforward. You can use the `int` indexer for arrays and the `string` indexer for objects. Using an invalid array index or property name returns an `undefined` value:

```cs
var json = JsonValue.Parse(someStream);

// a possible way of validation
JsonValue value = json["data"][0]["id"];
if (value.IsUndefined)
    throw new ArgumentException("Unexpected content");
// ... do something with value

// alternative way
long id = json["data"][0]["id"].AsInt64() ?? throw new ArgumentException("Unexpected content");
```

### Manipulating JSON

[`JsonValue`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonOValue.htm) is a read-only struct but [`JsonArray`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonArray.htm) and [`JsonObject`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonObject.htm) types are mutable classes. They implement some generic collection interfaces so they support LINQ extension methods.

```cs
JsonValue value = JsonValue.Parse(someStream);

JsonObject? toBeChanged = value["data"][0].AsObject;
// ... do null check if needed
toBeChanged["newProp"] = 123; // or: toBeChanged.Add("newProp", 123);

// Note that though JsonValue is a readonly struct, the changes are visible even from the original value.
// It's because we didn't replace any existing objects just appended one of them.
Console.WriteLine(value["data"][0]["newProp"].Type) // Number
```

> _Tip:_ [`JsonObject`](https://docs.kgysoft.net/json/?topic=html/T_KGySoft_Json_JsonObject.htm) implements both `IList<JsonProperty>` and `IDictionary<string, JsonValue>` so without casting or specifying some type arguments many LINQ methods might be ambiguous on a `JsonObject` instance. To avoid ambiguity and to keep also the syntax simple you can perform the LINQ operations on its `Entries` property.

## Performance Comparisons

**Notes:**
* The tests compare JSON.NET (aka. Newtonsoft.Json), System.Text.Json and KGy SOFT JSON Libraries in .NET 6.
* Starting with .NET 6 the new System.Text.Json.Nodes is also available with read-write capabilities, this is now included in the test results
* The test cases were executed for 500ms after a warm-up period.
* The test cases below all used the same formatted JSON [test data](https://github.com/koszeggy/KGySoft.Json/blob/Development/KGySoft.Json.PerformanceTest/TestData.cs#L24) as an input

> _Tip:_ See the source code of the tests along with more test cases in the `KGySoft.Json.PerformanceTest` project.

### Parse test

Parsing a test JSON from a UTF8 stream, without accessing any content. This is where System.Text.Json has the best performance, in which case it is completely allocation free. It is reflected in the results, too:

```
1. System.Text.Json: 60,769 iterations in 500.01 ms. Adjusted for 500 ms: 60,768.36
2. System.Text.Json.Nodes: 60,329 iterations in 500.00 ms. Adjusted for 500 ms: 60,328.86 (-439.50 / 99.28%)
3. KGySoft.Json: 24,539 iterations in 500.01 ms. Adjusted for 500 ms: 24,538.49 (-36,229.87 / 40.38%)
4. Newtonsoft.Json: 14,577 iterations in 500.03 ms. Adjusted for 500 ms: 14,576.09 (-46,192.27 / 23.99%)
```

As you can see, if we don't do anything with the parsed data, System.Text.Json[.Nodes] is the fastest one, KGy SOFT comes next, whereas JSON.NET (Newtonsoft.Json) is the slowest one, which is 4x slower than System.Text.Json.

### Access element test

Using the same JSON as above, this test reads a deeply embedded string element. When retrieving DOM elements, KGySoft.Json becomes the fastest one and System.Text.Json is the slowest one:

```
1. KGySoft.Json: 5,758,040 iterations in 500.00 ms. Adjusted for 500 ms: 5,758,030.79
2. Newtonsoft.Json: 4,749,643 iterations in 500.01 ms. Adjusted for 500 ms: 4,749,529.96 (-1,008,500.83 / 82.49%)
3. System.Text.Json.Nodes: 2,608,747 iterations in 500.00 ms. Adjusted for 500 ms: 2,608,744.39 (-3,149,286.40 / 45.31%)
4. System.Text.Json: 2,352,065 iterations in 500.00 ms. Adjusted for 500 ms: 2,352,059.36 (-3,405,971.43 / 40.85%)
```

Please also note that KGySoft.Json has the most [compact](https://github.com/koszeggy/KGySoft.Json/blob/master/KGySoft.Json.PerformanceTest/PerformanceTests/ReadDomTest.cs#L63) syntax. The syntax of the other two libraries would be even more verbose if we couldn't be sure that the accessed element exists.

### Write minimized JSON test

Creating a minimized JSON string of the input stream.

> _Note:_ System.Text.Json doesn't really count here as its `JsonDocument`/`JsonElement` types are read-only anyway. Still, it can be used to reformat the original JSON stream either with or without indenting.

```
1. KGySoft.Json: 160,980 iterations in 500.02 ms. Adjusted for 500 ms: 160,974.49
2. System.Text.Json.Nodes: 109,297 iterations in 500.00 ms. Adjusted for 500 ms: 109,296.50 (-51,678.00 / 67.90%)
3. System.Text.Json: 107,749 iterations in 500.12 ms. Adjusted for 500 ms: 107,722.28 (-53,252.21 / 66.92%)
4. Newtonsoft.Json: 91,016 iterations in 500.00 ms. Adjusted for 500 ms: 91,015.85 (-69,958.64 / 56.54%)
```

## Download:

### Download Binaries:

The binaries can be downloaded as a NuGet package directly from [nuget.org](https://www.nuget.org/packages/KGySoft.Json)

However, the preferred way is to install the package in VisualStudio either by looking for the `KGySoft.Json` package in the Nuget Package Manager GUI, or by sending the following command at the Package Manager Console prompt:

    PM> Install-Package KGySoft.Json

Alternatively, you can download the binaries as a .7z file attached to the [releases](https://github.com/koszeggy/KGySoft.Json/releases).

## Project Site

Find the project site at [kgysoft.net](https://kgysoft.net/json/).

## Documentation

* [Online documentation](https://docs.kgysoft.net/json)
* [Offline .chm documentation](https://github.com/koszeggy/KGySoft.Json/raw/master/KGySoft.Json/Help/KGySoft.Json.chm)

## Release Notes

See the [change log](https://github.com/koszeggy/KGySoft.Json/blob/master/KGySoft.Json/changelog.txt).

## License
KGy SOFT Core Libraries are under the [KGy SOFT License 1.0](https://github.com/koszeggy/KGySoft.CoreLibraries/blob/master/LICENSE), which is a permissive GPL-like license. It allows you to copy and redistribute the material in any medium or format for any purpose, even commercially. The only thing is not allowed is to distribute a modified material as yours: though you are free to change and re-use anything, do that by giving appropriate credit. See the [LICENSE](https://github.com/koszeggy/KGySoft.CoreLibraries/blob/master/LICENSE) file for details.

---

[![KGy SOFT .net](https://user-images.githubusercontent.com/27336165/124292367-c93f3d00-db55-11eb-8003-6d943ee7d7fa.png)](https://kgysoft.net)
