// OpenTween - Client of Twitter
// Copyright (c) 2023 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace OpenTween.Api.GraphQL
{
    public static class ErrorResponse
    {
        public static void ThrowIfError(string? jsonString)
        {
            if (MyCommon.IsNullOrEmpty(jsonString))
                return;

            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(jsonBytes, XmlDictionaryReaderQuotas.Max);

            var rootElm = XElement.Load(jsonReader);
            ThrowIfError(rootElm);
        }

        public static void ThrowIfError(XElement rootElm)
        {
            // errors と data プロパティが両方ともある場合はエラーを無視して正常なレスポンスとして扱う
            if (rootElm.Element("data")?.HasElements == true)
                return;

            var errorsElm = rootElm.Element("errors") ?? null;
            if (errorsElm == null)
                return;

            var messageElm = rootElm.XPathSelectElement("/errors/item/message") ?? null;
            var messageText = messageElm?.Value ?? "Error";
            var responseJson = JsonUtils.JsonXmlToString(rootElm);

            throw new WebApiException(messageText, responseJson);
        }
    }
}
