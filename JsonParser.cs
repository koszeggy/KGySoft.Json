#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using KGySoft.CoreLibraries;

#endregion

namespace TradeSystem.Json
{
    public static class JsonParser
    {
        #region Methods

        #region Internal Methods

        public static JsonValue Parse(Stream stream)
        {
            while (true)
            {
                int nextByte = stream.ReadByte();
                if (nextByte == -1)
                    throw new ArgumentException("Unexpected end of JSON message.", nameof(stream));

                char c = (char)nextByte;
                switch (c)
                {
                    case '{':
                        var properties = new Dictionary<string, JsonValue>();
                        ParseJsonObject(stream, properties);
                        return new JsonValue(properties);
                    case '[':
                        var items = new List<JsonValue>();
                        ParseJsonArray(stream, items);
                        return new JsonValue(items);
                    case '"':
                        var value = new StringBuilder();
                        ParseJsonString(stream, value);
                        return new JsonValue(value, true);
                    default:
                        if (IsWhitespace(c))
                            continue;
                        if (IsValidLiteral(c))
                        {
                            value = new StringBuilder();
                            value.Append(c);
                            ParseJsonLiteral(stream, value);
                            return new JsonValue(value, false);
                        }

                        throw new ArgumentException($"Unexpected character in JSON value: {c}", nameof(stream));
                }
            }
        }

        #endregion

        #region Private Methods

        private static void ParseJsonLiteral(Stream stream, StringBuilder value)
        {
            while (true)
            {
                int nextByte = stream.ReadByte();
                if (nextByte == -1)
                    return;

                char c = (char)nextByte;
                if (IsWhitespace(c) || c.In(',', '}', ']'))
                {
                    stream.Position--;
                    return;
                }

                if (IsValidLiteral(c))
                {
                    value.Append(c);
                    continue;
                }

                throw new ArgumentException($"Unexpected character in JSON literal: {c}", nameof(stream));
            }
        }

        private static void ParseJsonString(Stream stream, StringBuilder value)
        {
            while (true)
            {
                int nextByte = stream.ReadByte();
                if (nextByte == -1)
                    throw new ArgumentException("Unexpected end of JSON message.", nameof(stream));

                char c = (char)nextByte;
                switch (c)
                {
                    case '"':
                        return;
                    case '\\':
                        nextByte = stream.ReadByte();
                        if (nextByte == -1)
                            continue;
                        c = (char)nextByte;
                        switch (c)
                        {
                            case 'b':
                                value.Append('\b');
                                break;
                            case 'f':
                                value.Append('\f');
                                break;
                            case 'n':
                                value.Append('\n');
                                break;
                            case 'r':
                                value.Append('\r');
                                break;
                            case 't':
                                value.Append('\t');
                                break;
                            case '"':
                                value.Append('"');
                                break;
                            case '\\':
                                value.Append(@"\\");
                                break;
                            default:
                                Logger.Warn(cb => cb($"Invalid JSON escape sequence: \\{c}"));
                                value.Append($"\\{c}");
                                break;
                        }
                        break;
                    default:
                        value.Append(c);
                        break;
                }
            }
        }

        private static void ParseJsonArray(Stream stream, List<JsonValue> items)
        {
            bool commaOrEndExpected = false;
            while (true)
            {
                int nextByte = stream.ReadByte();
                if (nextByte == -1)
                    throw new ArgumentException("Unexpected end of JSON message.", nameof(stream));

                char c = (char)nextByte;
                switch (c)
                {
                    case ']':
                        return;
                    case ',':
                        if (!commaOrEndExpected)
                            throw new ArgumentException("Unexpected comma in JSON array", nameof(stream));
                        commaOrEndExpected = false;
                        continue;
                    default:
                        if (IsWhitespace(c))
                            continue;
                        if (commaOrEndExpected)
                            throw new ArgumentException($"Comma expected but '{c}' found in JSON object", nameof(stream));
                        stream.Position--;
                        items.Add(Parse(stream));
                        commaOrEndExpected = true;
                        continue;
                }
            }
        }

        private static void ParseJsonObject(Stream stream, Dictionary<string, JsonValue> properties)
        {
            bool commaOrEndExpected = false;
            while (true)
            {
                int nextByte = stream.ReadByte();
                if (nextByte == -1)
                    throw new ArgumentException("Unexpected end of JSON message.", nameof(stream));

                char c = (char)nextByte;
                switch (c)
                {
                    case '}':
                        return;
                    case ',':
                        if (!commaOrEndExpected)
                            throw new ArgumentException("Unexpected comma in JSON property", nameof(stream));
                        commaOrEndExpected = false;
                        continue;
                    case '"':
                        if (commaOrEndExpected)
                            throw new ArgumentException("Missing comma between properties in JSON object", nameof(stream));
                        JsonProperty property = ParseJsonProperty(stream);
                        properties[property.Name] = property.Value;
                        commaOrEndExpected = true;
                        continue;
                    default:
                        if (IsWhitespace(c))
                            continue;
                        if (commaOrEndExpected)
                            throw new ArgumentException($"Comma expected but '{c}' found in JSON object", nameof(stream));
                        throw new ArgumentException($"Double quote or object end expected but '{c}' found in JSON object", nameof(stream));
                }
            }
        }

        private static JsonProperty ParseJsonProperty(Stream stream)
        {
            StringBuilder name = new StringBuilder();
            ParseJsonString(stream, name);
            bool colonExpected = true;
            while (true)
            {
                int nextByte = stream.ReadByte();
                if (nextByte == -1)
                    throw new ArgumentException("Unexpected end of JSON message.", nameof(stream));

                char c = (char)nextByte;
                switch (c)
                {
                    case ':':
                        if (!colonExpected)
                            throw new ArgumentException("Unexpected colon in JSON object", nameof(stream));
                        colonExpected = false;
                        continue;
                    default:
                        if (colonExpected)
                            throw new ArgumentException($"Colon expected but '{c}' found in JSON object", nameof(stream));
                        stream.Position--;
                        return new JsonProperty(name.ToString(), Parse(stream));
                }
            }
        }

        private static bool IsWhitespace(char c) => c.In(' ', '\t', '\r', '\n');

        private static bool IsValidLiteral(char c) => c >= '0' && c <= '9'
            || c >= 'a' && c <= 'z'
            || c >= 'A' && c <= 'Z'
            || c.In('-', '.');

        #endregion

        #endregion
    }
}
