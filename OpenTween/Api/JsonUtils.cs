// OpenTween - Client of Twitter
// Copyright (c) 2019 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;

namespace OpenTween.Api
{
    internal static class JsonUtils
    {
        /// <summary>JSON に出力する文字列を ECMA-404 に従ってエスケープする</summary>
        public static string EscapeJsonString(string rawText)
        {
            var builder = new StringBuilder(rawText.Length + 20);

            foreach (var c in rawText)
            {
                if (c <= 0x1F || char.IsSurrogate(c))
                    builder.AppendFormat(@"\u{0:X4}", (int)c);
                else if (c == '\\' || c == '\"')
                    builder.Append('\\').Append(c);
                else
                    builder.Append(c);
            }

            return builder.ToString();
        }

        /// <summary>
        /// <see cref="JsonReaderWriterFactory"/>で読み込んだ<see cref="XElement"/>をJSONとして書き出す
        /// </summary>
        public static string JsonXmlToString(XElement element)
        {
            var isRoot = element.Name == "root";

            using var stream = new MemoryStream();
            using (var jsonWriter = JsonReaderWriterFactory.CreateJsonWriter(stream))
            {
                if (isRoot)
                {
                    element.WriteTo(jsonWriter);
                }
                else
                {
                    jsonWriter.WriteStartElement("root");
                    jsonWriter.WriteAttributeString("type", element.Attribute("type").Value);
                    foreach (var child in element.Elements())
                        child.WriteTo(jsonWriter);
                    jsonWriter.WriteEndElement();
                }
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }
    }
}
