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
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using OpenTween.Api;

namespace OpenTween.Connection
{
    public sealed class ApiResponse : IDisposable
    {
        public bool IsDisposed { get; private set; }

        private readonly HttpResponseMessage responseMessage;
        private bool preventDisposeResponse;

        public ApiResponse(HttpResponseMessage responseMessage)
            => this.responseMessage = responseMessage;

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            if (!this.preventDisposeResponse)
                this.responseMessage.Dispose();

            this.IsDisposed = true;
        }

        public async Task<byte[]> ReadAsBytes()
        {
            using var content = this.responseMessage.Content;

            return await content.ReadAsByteArrayAsync()
                .ConfigureAwait(false);
        }

        public async Task<T> ReadAsJson<T>()
        {
            var responseBytes = await this.ReadAsBytes()
                .ConfigureAwait(false);

            try
            {
                return MyCommon.CreateDataFromJson<T>(responseBytes);
            }
            catch (SerializationException ex)
            {
                var responseText = Encoding.UTF8.GetString(responseBytes);
                throw TwitterApiException.CreateFromException(ex, responseText);
            }
        }

        public async Task<XElement> ReadAsJsonXml()
        {
            var responseBytes = await this.ReadAsBytes()
                .ConfigureAwait(false);

            using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(
                responseBytes,
                XmlDictionaryReaderQuotas.Max
            );

            try
            {
                return XElement.Load(jsonReader);
            }
            catch (XmlException ex)
            {
                var responseText = Encoding.UTF8.GetString(responseBytes);
                throw new TwitterApiException("Invalid JSON", ex) { ResponseText = responseText };
            }
        }

        public LazyJson<T> ReadAsLazyJson<T>()
        {
            this.preventDisposeResponse = true;

            return new(this.responseMessage);
        }
    }
}
