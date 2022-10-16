#region Copyright

///////////////////////////////////////////////////////////////////////////////
//  File: JsonWriter.cs
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

#endregion

namespace KGySoft.Json
{
    internal struct JsonWriter
    {
        #region Fields

        private readonly TextWriter writer;
        private readonly string indent;

        private char[]? indent10;
        private int depth;

        #endregion

        #region Constructors

        internal JsonWriter(TextWriter writer, string? indent) : this()
        {
            this.writer = writer;
            this.indent = indent ?? String.Empty;
        }

        #endregion

        #region Methods

        #region Static Methods

        #region Internal Methods
        
        internal static void WriteJsonString(StringBuilder builder, string value)
        {
            int len = value.Length;
            if (len == 0)
            {
                builder.Append("\"\"");
                return;
            }

            builder.Append('"');
            int pos = 0;
            int escapePos;
            while ((escapePos = GetNextEscapeIndex(value, pos)) >= 0)
            {
                if (escapePos > pos)
                    builder.Append(value, pos, escapePos - pos);

                switch (value[escapePos])
                {
                    case '"':
                        builder.Append(@"\""");
                        break;
                    case '\\':
                        builder.Append(@"\\");
                        break;
                    case '\r':
                        builder.Append(@"\r");
                        break;
                    case '\n':
                        builder.Append(@"\n");
                        break;
                    case '\t':
                        builder.Append(@"\t");
                        break;
                    case '\f':
                        builder.Append(@"\f");
                        break;
                    case '\b':
                        builder.Append(@"\b");
                        break;
                    default:
                        Debug.Assert(value[escapePos] < ' ');
                        builder.Append("\\u" + ((int)value[escapePos]).ToString("X4", NumberFormatInfo.InvariantInfo));
                        break;
                }

                pos = escapePos + 1;
            }

            if (pos < len)
            {
                if (pos == 0)
                    builder.Append(value);
                else
                    builder.Append(value, pos, len - pos);
            }

            builder.Append('"');
        }

        #endregion

        #region Static Methods
        
        private static int GetNextEscapeIndex(string s, int startIndex)
        {
            int len = s.Length;
            for (int i = startIndex; i < len; i++)
            {
                if (s[i] is < ' ' or '\\' or '"')
                    return i;
            }

            return -1;
        }

        #endregion
        
        #endregion

        #region Instance Methods

        #region Internal Methods

        internal void Write(JsonValue value)
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
            int len = array.Length;
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

        private void WriteIndent()
        {
            if (depth == 1)
            {
                writer.Write(indent);
                return;
            }

            if (indent10 == null)
            {
                int length = indent.Length;
                indent10 = new char[length * 10];
                for (int i = 0; i < 10; i++)
                    indent.CopyTo(0, indent10, i * length, length);
            }

            for (int d = depth; d > 0; d -= 10)
                writer.Write(indent10!, 0, indent.Length * Math.Min(10, d));
        }

        private void WriteJsonString(string value)
        {
            int len = value.Length;
            if (len == 0)
            {
                writer.Write("\"\"");
                return;
            }

            // Non-derived StringWriter: shortcut to StringBuilder (this also prevents Substrings where Spans are not available)
            if (writer.GetType() == typeof(StringWriter))
            {
                WriteJsonString(((StringWriter)writer).GetStringBuilder(), value);
                return;
            }

            writer.Write('"');

            int pos = 0;
            int escapePos;
            while ((escapePos = GetNextEscapeIndex(value, pos)) >= 0)
            {
                if (escapePos > pos)
                {
#if NETSTANDARD2_1_OR_GREATER
                    writer.Write(value.AsSpan(pos, escapePos - pos));
#else
                    writer.Write(value.Substring(pos, escapePos - pos));
#endif
                }

                switch (value[escapePos])
                {
                    case '"':
                        writer.Write(@"\""");
                        break;
                    case '\\':
                        writer.Write(@"\\");
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
                        Debug.Assert(value[escapePos] < ' ');
                        writer.Write("\\u" + ((int)value[escapePos]).ToString("X4", NumberFormatInfo.InvariantInfo));
                        break;
                }

                pos = escapePos + 1;
            }

            if (pos < len)
            {
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression - false alarm due to #if
                if (pos == 0)
                    writer.Write(value);
                else
                {
#if NETSTANDARD2_1_OR_GREATER
                    writer.Write(value.AsSpan(pos));
#else
                    writer.Write(value.Substring(pos));
#endif
                }
            }

            writer.Write('"');
        }

        #endregion
        
        #endregion

        #endregion
    }
}