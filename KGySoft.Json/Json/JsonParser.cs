#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonParser.cs
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

using System.Collections.Generic;
using System.IO;
using System.Text;

#endregion

namespace KGySoft.Json
{
    internal static class JsonParser
    {
        #region Methods

        #region Internal Methods

        internal static JsonValue ParseValue(TextReader reader)
        {
            char? _ = default;
            JsonValue result = DoParseValue(reader, ref _, out string? error);
            if (error != null)
                Throw.ArgumentException(error, nameof(reader));
            return result;
        }

        internal static bool TryParseValue(TextReader reader, out JsonValue result)
        {
            char? _ = default;
            result = DoParseValue(reader, ref _, out string? error);
            return error == null;
        }

        #endregion

        #region Private Methods

        private static JsonValue DoParseValue(TextReader reader, ref char? c, out string? error)
        {
            if (c == null)
            {
                int nextChar = reader.Read();
                if (nextChar == -1)
                {
                    error = Res.UnexpectedEndOfJsonStream;
                    return default;
                }

                c = (char)nextChar;
            }

            error = null;
            while (true)
            {
                switch (c)
                {
                    case '{':
                        c = null;
                        return DoParseObject(reader, out error) is JsonObject obj ? obj : default(JsonValue);
                    case '[':
                        c = null;
                        return DoParseArray(reader, out error) is JsonArray arr ? arr : default(JsonValue);
                    case '"':
                        c = null;
                        return ParseString(reader, out error) is string str ? str : default(JsonValue);
                    default:
                        if (IsWhitespace(c.Value))
                            break;
                        bool isNumber = true;
                        if (!CanBeLiteral(c.Value, ref isNumber))
                        {
                            error = Res.UnexpectedCharInJsonValue(c.Value);
                            return default;
                        }

                        string? s = ParseLiteral(reader, ref c, ref isNumber, out error);
                        if (s == null)
                            return default;
                        return isNumber ? new JsonValue(JsonValueType.Number, s)
                            : s == JsonValue.NullLiteral ? JsonValue.Null
                            : s == JsonValue.TrueLiteral ? JsonValue.True
                            : s == JsonValue.FalseLiteral ? JsonValue.False
                            : s == JsonValue.UndefinedLiteral ? JsonValue.Undefined // actually not valid in a JSON object
                            : new JsonValue(JsonValueType.UnknownLiteral, s);
                }

                int nextChar = reader.Read();
                if (nextChar == -1)
                {
                    error = Res.UnexpectedEndOfJsonStream;
                    return default;
                }

                c = (char)nextChar;
            }
        }

        private static string? ParseLiteral(TextReader reader, ref char? c, ref bool isNumber, out string? error)
        {
            var result = new StringBuilder();
            result.Append(c);

            while (true)
            {
                int nextChar = reader.Read();
                if (nextChar == -1)
                {
                    c = null;
                    break;
                }

                c = (char)nextChar;
                if (IsWhitespace(c.Value) || c.Value is ',' or '}' or ']')
                    break;

                if (CanBeLiteral(c.Value, ref isNumber))
                {
                    result.Append(c);
                    continue;
                }

                error = Res.UnexpectedCharInJsonLiteral(c.Value);
                return null;
            }

            error = null;
            return result.ToString();
        }

        private static string? ParseString(TextReader reader, out string? error)
        {
            var result = new StringBuilder();
            while (true)
            {
                int nextChar = reader.Read();
                if (nextChar == -1)
                {
                    error = Res.UnexpectedEndOfJsonString;
                    return null;
                }

                char c = (char)nextChar;
                switch (c)
                {
                    case '"':
                        error = null;
                        return result.ToString();
                    case '\\':
                        nextChar = reader.Read();
                        if (nextChar == -1)
                            continue;
                        c = (char)nextChar;
                        switch (c)
                        {
                            case 'b':
                                result.Append('\b');
                                break;
                            case 'f':
                                result.Append('\f');
                                break;
                            case 'n':
                                result.Append('\n');
                                break;
                            case 'r':
                                result.Append('\r');
                                break;
                            case 't':
                                result.Append('\t');
                                break;
                            case '"':
                                result.Append('"');
                                break;
                            case '\\':
                                result.Append(@"\\");
                                break;
                            default:
                                result.Append($"\\{c}");
                                break;
                        }
                        break;
                    default:
                        result.Append(c);
                        break;
                }
            }
        }

        private static JsonArray? DoParseArray(TextReader reader, out string? error)
        {
            var items = new List<JsonValue>();
            bool commaOrEndExpected = false;

            int nextChar = reader.Read();
            if (nextChar == -1)
            {
                error = Res.UnexpectedEndOfJsonArray;
                return null;
            }

            var c = (char?)nextChar;
            error = null;
            while (true)
            {
                switch (c)
                {
                    case ']':
                        return new JsonArray(items);
                    case ',':
                        if (!commaOrEndExpected)
                        {
                            error = Res.UnexpectedCommaInJsonArray;
                            return null;
                        }

                        commaOrEndExpected = false;
                        break;
                    default:
                        if (IsWhitespace(c.Value))
                            break;
                        if (commaOrEndExpected)
                        {
                            error = Res.UnexpectedCharInJsonArray(c.Value);
                            return null;
                        }

                        JsonValue item = DoParseValue(reader, ref c, out error);
                        if (error != null)
                            return null;
                        items.Add(item);
                        commaOrEndExpected = true;
                        if (c == null)
                            break;
                        else
                            continue;
                }

                nextChar = reader.Read();
                if (nextChar == -1)
                {
                    error = Res.UnexpectedEndOfJsonArray;
                    return null;
                }

                c = (char)nextChar;
            }
        }

        private static JsonObject? DoParseObject(TextReader reader, out string? error)
        {
            var properties = new List<JsonProperty>();
            bool commaOrEndExpected = false;

            int nextChar = reader.Read();
            if (nextChar == -1)
            {
                error = Res.UnexpectedEndOfJsonObject;
                return null;
            }
            
            var c = (char?)nextChar;
            error = null;
            while (true)
            {
                switch (c)
                {
                    case '}':
                        return new JsonObject(properties);
                    case ',':
                        if (!commaOrEndExpected)
                        {
                            error = Res.UnexpectedCommaInJsonObject;
                            return null;
                        }

                        commaOrEndExpected = false;
                        break;
                    case '"':
                        if (commaOrEndExpected)
                        {
                            error = Res.MissingCommaInJsonObject;
                            return null;
                        }

                        JsonProperty item = ParseProperty(reader, ref c, out error);
                        if (error != null)
                            return null;
                        properties.Add(item);
                        commaOrEndExpected = true;
                        if (c == null)
                            break;
                        else
                            continue;
                    default:
                        if (IsWhitespace(c.Value))
                            break;

                        error = Res.UnexpectedCharInJsonObject(c.Value);
                        return null;
                }

                nextChar = reader.Read();
                if (nextChar == -1)
                {
                    error = Res.UnexpectedEndOfJsonObject;
                    return null;
                }

                c = (char)nextChar;
            }
        }

        private static JsonProperty ParseProperty(TextReader reader, ref char? c, out string? error)
        {
            string? name = ParseString(reader, out error);
            if (name == null)
                return default;

            bool colonExpected = true;
            while (true)
            {
                int nextChar = reader.Read();
                if (nextChar == -1)
                {
                    error = Res.UnexpectedEndOfJsonObject;
                    return default;
                }

                c = (char)nextChar;
                switch (c)
                {
                    case ':':
                        if (!colonExpected)
                        {
                            error = Res.UnexpectedColonInJsonObject;
                            return default;
                        }

                        colonExpected = false;
                        continue;
                    default:
                        if (IsWhitespace(c.Value))
                            break;
                        if (colonExpected)
                        {
                            error = Res.UnexpectedCharInJsonObject(c.Value);
                            return default;
                        }

                        return new JsonProperty(name, DoParseValue(reader, ref c, out error));
                }
            }
        }

        private static bool IsWhitespace(char c) => c is ' ' or '\t' or '\r' or '\n';

        private static bool CanBeLiteral(char c, ref bool canBeNumber)
        {
            // not a precise validation but works well for valid content
            if (canBeNumber && (c is >= '0' and <= '9' or '-' or '.' or 'e' or 'E' or '+'))
                return true;

            canBeNumber = false;
            return c is >= '0' and <= '9'
                or >= 'a' and <= 'z'
                or >= 'A' and <= 'Z'
                or '-' or '.' or '+';
        }

        #endregion

        #endregion
    }
}
