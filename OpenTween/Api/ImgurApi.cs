// OpenTween - Client of Twitter
// Copyright (c) 2013 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Connection;

namespace OpenTween.Api
{
    public class ImgurApi : IImgurApi
    {
        private readonly ApiKey clientId;
        private readonly HttpClient http;

        public static readonly Uri UploadEndpoint = new("https://api.imgur.com/3/image.xml");

        public ImgurApi()
            : this(ApplicationSettings.ImgurClientId, null)
        {
        }

        public ImgurApi(ApiKey clientId, HttpClient? http)
        {
            this.clientId = clientId;

            if (http != null)
            {
                this.http = http;
            }
            else
            {
                this.http = Networking.CreateHttpClient(Networking.CreateHttpClientHandler());
                this.http.Timeout = Networking.UploadImageTimeout;
            }
        }

        public async Task<string> UploadFileAsync(IMediaItem item, string title)
        {
            using var response = await this.SendRequestAsync(item, title)
                .ConfigureAwait(false);

            var responseText = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            XDocument responseXml;
            try
            {
                responseXml = XDocument.Parse(responseText);
            }
            catch (XmlException ex)
            {
                var errorMessage = response.IsSuccessStatusCode ? "Invalid response" : response.StatusCode.ToString();
                throw new WebApiException("Err:" + errorMessage, responseText, ex);
            }

            var imageElm = responseXml.Element("data");
            if (imageElm?.Attribute("success")?.Value != "1")
            {
                var errorMessage = imageElm?.Element("error")?.Value ?? "Invalid response";
                throw new WebApiException("Err:" + errorMessage, responseText);
            }

            var imageUrl = responseXml.XPathSelectElement("/data/link")?.Value;
            if (imageUrl == null)
                throw new WebApiException("Err:Invalid response", responseText);

            return imageUrl.Trim();
        }

        private async Task<HttpResponseMessage> SendRequestAsync(IMediaItem item, string title)
        {
            if (!this.clientId.TryGetValue(out var clientId))
                throw new WebApiException("Err:imgur APIキーが使用できません");

            using var content = new MultipartFormDataContent();
            using var mediaStream = item.OpenRead();
            using var mediaContent = new StreamContent(mediaStream);
            using var titleContent = new StringContent(title);

            content.Add(mediaContent, "image", item.Name);
            content.Add(titleContent, "title");

            using var request = new HttpRequestMessage(HttpMethod.Post, UploadEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Client-ID", clientId);
            request.Content = content;

            return await this.http.SendAsync(request)
                .ConfigureAwait(false);
        }
    }

    public interface IImgurApi
    {
        Task<string> UploadFileAsync(IMediaItem item, string title);
    }
}
