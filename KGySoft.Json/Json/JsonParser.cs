﻿#region Copyright

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

using System;
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

        internal static JsonValue Parse(TextReader reader)
        {
            char? _ = default;
            return ParseValue(reader, ref _);
        }

        #endregion

        #region Private Methods

        private static JsonValue ParseValue(TextReader reader, ref char? c)
        {
            if (c == null)
            {
                int nextChar = reader.Read();
                if (nextChar == -1)
                    Throw.ArgumentException(Res.UnexpectedEndOfJsonStream, nameof(reader));
                c = (char)nextChar;
            }

            while (true)
            {
                switch (c)
                {
                    case '{':
                        c = null;
                        return new JsonValue(ParseObject(reader));
                    case '[':
                        c = null;
                        return new JsonValue(ParseArray(reader));
                    case '"':
                        c = null;
                        return new JsonValue(JsonValueType.String, ParseString(reader));
                    default:
                        if (IsWhitespace(c.Value))
                            break;
                        bool isNumber = true;
                        if (!CanBeLiteral(c.Value, ref isNumber))
                            Throw.ArgumentException(Res.UnexpectedCharInJsonValue(c.Value), nameof(reader));

                        string s = ParseLiteral(reader, ref c, ref isNumber);
                        return isNumber ? new JsonValue(JsonValueType.Number, s)
                            : s == JsonValue.NullLiteral ? JsonValue.Null
                            : s == JsonValue.TrueLiteral ? JsonValue.True
                            : s == JsonValue.FalseLiteral ? JsonValue.False
                            : s == JsonValue.UndefinedLiteral ? JsonValue.Undefined // actually not valid in a JSON object
                            : new JsonValue(JsonValueType.UnknownLiteral, s);
                }

                int nextChar = reader.Read();
                if (nextChar == -1)
                    Throw.ArgumentException(Res.UnexpectedEndOfJsonStream, nameof(reader));
                c = (char)nextChar;
            }
        }

        private static string ParseLiteral(TextReader reader, ref char? c, ref bool isNumber)
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

                Throw.ArgumentException(Res.UnexpectedCharInJsonLiteral(c.Value), nameof(reader));
            }

            return result.ToString();
        }

        private static string ParseString(TextReader reader)
        {
            var result = new StringBuilder();
            while (true)
            {
                int nextChar = reader.Read();
                if (nextChar == -1)
                    Throw.ArgumentException(Res.UnexpectedEndOfJsonString, nameof(reader));

                char c = (char)nextChar;
                switch (c)
                {
                    case '"':
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

        private static JsonArray ParseArray(TextReader reader)
        {
            var items = new List<JsonValue>();
            bool commaOrEndExpected = false;

            int nextChar = reader.Read();
            if (nextChar == -1)
                Throw.ArgumentException(Res.UnexpectedEndOfJsonArray, nameof(reader));
            var c = (char?)nextChar;

            while (true)
            {
                switch (c)
                {
                    case ']':
                        return new JsonArray(items);
                    case ',':
                        if (!commaOrEndExpected)
                            Throw.ArgumentException(Res.UnexpectedCommaInJsonArray, nameof(reader));
                        commaOrEndExpected = false;
                        break;
                    default:
                        if (IsWhitespace(c.Value))
                            break;
                        if (commaOrEndExpected)
                            Throw.ArgumentException(Res.UnexpectedCharInJsonArray(c.Value), nameof(reader));
                        items.Add(ParseValue(reader, ref c));
                        commaOrEndExpected = true;
                        if (c == null)
                            break;
                        else
                            continue;
                }

                nextChar = reader.Read();
                if (nextChar == -1)
                    Throw.ArgumentException(Res.UnexpectedEndOfJsonArray, nameof(reader));
                c = (char)nextChar;
            }
        }

        private static JsonObject ParseObject(TextReader reader)
        {
            var properties = new List<JsonProperty>();
            bool commaOrEndExpected = false;

            int nextChar = reader.Read();
            if (nextChar == -1)
                Throw.ArgumentException(Res.UnexpectedEndOfJsonObject, nameof(reader));
            var c = (char?)nextChar;

            while (true)
            {
                switch (c)
                {
                    case '}':
                        return new JsonObject(properties);
                    case ',':
                        if (!commaOrEndExpected)
                            Throw.ArgumentException(Res.UnexpectedCommaInJsonObject, nameof(reader));
                        commaOrEndExpected = false;
                        break;
                    case '"':
                        if (commaOrEndExpected)
                            Throw.ArgumentException(Res.MissingCommaInJsonObject, nameof(reader));
                        properties.Add(ParseProperty(reader, ref c));
                        commaOrEndExpected = true;
                        if (c == null)
                            break;
                        else
                            continue;
                    default:
                        if (IsWhitespace(c.Value))
                            break;

                        return Throw.ArgumentException<JsonObject>(Res.UnexpectedCharInJsonObject(c.Value), nameof(reader));
                }

                nextChar = reader.Read();
                if (nextChar == -1)
                    Throw.ArgumentException(Res.UnexpectedEndOfJsonObject, nameof(reader));
                c = (char)nextChar;
            }
        }

        private static JsonProperty ParseProperty(TextReader reader, ref char? c)
        {
            string name = ParseString(reader);
            bool colonExpected = true;
            while (true)
            {
                int nextChar = reader.Read();
                if (nextChar == -1)
                    Throw.ArgumentException(Res.UnexpectedEndOfJsonObject, nameof(reader));

                c = (char)nextChar;
                switch (c)
                {
                    case ':':
                        if (!colonExpected)
                            Throw.ArgumentException(Res.UnexpectedColonInJsonObject, nameof(reader));
                        colonExpected = false;
                        continue;
                    default:
                        if (IsWhitespace(c.Value))
                            break;
                        if (colonExpected)
                            Throw.ArgumentException(Res.UnexpectedCharInJsonObject(c.Value), nameof(reader));
                        return new JsonProperty(name, ParseValue(reader, ref c));
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