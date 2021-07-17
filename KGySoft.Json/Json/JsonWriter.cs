#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonWriter.cs
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
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;

#endregion

namespace KGySoft.Json
{
    internal struct JsonWriter
    {
        #region Fields

        private readonly TextWriter writer;
        private readonly string indent;
        private readonly char[]? indent10;

        private int depth;

        #endregion

        #region Constructors

        internal JsonWriter(TextWriter writer, string? indent) : this()
        {
            this.writer = writer;
            this.indent = indent ??= String.Empty;
            int length = indent.Length;
            if (length == 0)
                return;
            indent10 = new char[length * 10];
            for (int i = 0; i < 10; i++)
                indent.CopyTo(0, indent10, i * length, length);
        }

        #endregion

        #region Methods

        #region Internal Methods

        internal void Write(in JsonValue value)
        {
            switch (value.Type)
            {
                case JsonValueType.String:
                    WriteJsonString(value.AsLiteral!);
                    return;

                case JsonValueType.Object:
                    Write(value.AsObject!);
                    return;

                case JsonValueType.Array:
                    Write(value.AsArray!);
                    return;

                default:
                    if (!value.IsUndefined)
                        writer.Write(value.AsLiteral);
                    return;
            }
        }

        internal void Write(JsonObject obj)
        {
            int len = obj.Count;
            if (len == 0)
            {
                writer.Write("{}");
                return;
            }

            writer.Write('{');
            bool hasIndent = indent.Length > 0;
            bool empty = true;
            List<JsonProperty> properties = obj.PropertiesInternal;
            for (var i = 0; i < len; i++)
            {
                JsonProperty property = properties[i];
                if (property.Value.IsUndefined)
                    continue;
                if (empty)
                {
                    empty = false;
                    depth += 1;
                }
                else
                    writer.Write(',');

                if (hasIndent)
                {
                    writer.WriteLine();
                    WriteIndent();
                }

                WriteJsonString(property.Name!);
                if (hasIndent)
                    writer.Write(": ");
                else
                    writer.Write(':');
                Write(property.Value);
            }

            if (!empty && hasIndent)
            {
                depth -= 1;
                writer.WriteLine();
                WriteIndent();
            }

            writer.Write('}');
        }

        internal void Write(JsonArray array)
        {
            int len = array.Count;
            if (len == 0)
            {
                writer.Write("[]");
                return;
            }

            writer.Write('[');
            bool first = true;
            bool hasIndent = indent.Length > 0;
            depth += 1;
            List<JsonValue> items = array.ItemsInternal;
            for (var i = 0; i < len; i++)
            {
                JsonValue value = items[i];
                if (first)
                    first = false;
                else
                    writer.Write(',');

                if (hasIndent)
                {
                    writer.WriteLine();
                    WriteIndent();
                }

                Write(value.IsUndefined ? JsonValue.Null : value);
            }

            depth -= 1;
            if (hasIndent)
            {
                writer.WriteLine();
                WriteIndent();
            }

            writer.Write(']');
        }

        #endregion

        #region Private Methods

        [MethodImpl(MethodImpl.AggressiveInlining)]
        private void WriteIndent()
        {
            if (depth == 1)
            {
                writer.Write(indent);
                return;
            }

            for (int d = depth; d > 0; d -= 10)
                writer.Write(indent10!, 0, indent.Length * Math.Min(10, d));
        }

        private void WriteJsonString(string value)
        {
            writer.Write('"');
            int len = value.Length;
            for (var i = 0; i < len; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case > '\\': // 92
                        writer.Write(c);
                        break;

                    case > '"': // 34
                        if (c == '\\')
                            writer.Write(@"\\");
                        else
                            writer.Write(c);
                        break;

                    case >= ' ': // 32
                        if (c == '"')
                            writer.Write(@"\""");
                        else
                            writer.Write(c);
                        break;

                    case '\r':
                        writer.Write(@"\r");
                        break;
                    case '\n':
                        writer.Write(@"\n");
                        break;
                    case '\t':
                        writer.Write(@"\t");
                        break;
                    case '\f':
                        writer.Write(@"\f");
                        break;
                    case '\b':
                        writer.Write(@"\b");
                        break;
                    default:
                        writer.Write("\\u");
                        writer.Write(((int)c).ToString("X4", NumberFormatInfo.InvariantInfo));
                        break;
                }
            }

            writer.Write('"');
        }

        #endregion

        #endregion
    }
}