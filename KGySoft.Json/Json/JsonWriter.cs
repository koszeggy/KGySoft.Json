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
using System.IO;

#endregion

namespace KGySoft.Json
{
    internal struct JsonWriter
    {
        #region Fields

        private readonly TextWriter writer;
        private readonly string indent;

        private int depth;

        #endregion

        #region Constructors

        internal JsonWriter(TextWriter writer, string? indent)
        {
            this.writer = writer;
            this.indent = indent ?? String.Empty;
            depth = 0;
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
            writer.Write('{');
            bool hasIndent = indent.Length > 0;
            bool empty = true;
            foreach (JsonProperty property in obj)
            {
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
                writer.Write(':');
                if (hasIndent)
                    writer.Write(' ');
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
            writer.Write('[');
            bool empty = true;
            bool hasIndent = indent.Length > 0;
            foreach (JsonValue value in array)
            {
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

                Write(value.IsUndefined ? JsonValue.Null : value);
            }

            if (!empty && hasIndent)
            {
                depth -= 1;
                writer.WriteLine();
                WriteIndent();
            }

            writer.Write(']');
        }

        #endregion

        #region Private Methods

        private void WriteIndent()
        {
            for (int i = 0; i < depth; i++)
                writer.Write(indent);
        }

        private void WriteJsonString(string value)
        {
            writer.Write('"');
            foreach (char c in value)
            {
                switch (c)
                {
                    case '\b':
                        writer.Write(@"\b");
                        break;
                    case '\f':
                        writer.Write(@"\f");
                        break;
                    case '\n':
                        writer.Write(@"\n");
                        break;
                    case '\r':
                        writer.Write(@"\r");
                        break;
                    case '\t':
                        writer.Write(@"\t");
                        break;
                    case '"':
                        writer.Write(@"\""");
                        break;
                    case '\\':
                        writer.Write(@"\\");
                        break;
                    default:
                        writer.Write(c);
                        break;
                }
            }

            writer.Write('"');
        }

        #endregion

        #endregion
    }
}