// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Connection;

namespace OpenTween.Api
{
    public class MobypictureApi : IMobypictureApi
    {
        private readonly ApiKey apiKey;
        private readonly HttpClient http;

        public static readonly Uri UploadEndpoint = new("https://api.mobypicture.com/2.0/upload.xml");

        private static readonly Uri OAuthRealm = new("http://api.twitter.com/");
        private static readonly Uri AuthServiceProvider = new("https://api.twitter.com/1.1/account/verify_credentials.json");

        public MobypictureApi(TwitterApi twitterApi)
            : this(ApplicationSettings.MobypictureKey, twitterApi)
        {
        }

        public MobypictureApi(ApiKey apiKey, TwitterApi twitterApi)
        {
            this.apiKey = apiKey;

            var handler = twitterApi.CreateOAuthEchoHandler(AuthServiceProvider, OAuthRealm);
            this.http = Networking.CreateHttpClient(handler);
            this.http.Timeout = Networking.UploadImageTimeout;
        }

        public MobypictureApi(ApiKey apiKey, HttpClient http)
        {
            this.apiKey = apiKey;
            this.http = http;
        }

        /// <summary>
        /// 画像のアップロードを行います
        /// </summary>
        /// <exception cref="ApiKeyDecryptException"/>
        /// <exception cref="WebApiException"/>
        /// <exception cref="XmlException"/>
        public async Task<string> UploadFileAsync(IMediaItem item, string message)
        {
            using var response = await this.SendRequestAsync(item, message)
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

            var imageUrlElm = responseXml.XPathSelectElement("/rsp/media/mediaurl");
            if (imageUrlElm == null)
                throw new WebApiException("Invalid API response", responseText);

            return imageUrlElm.Value.Trim();
        }

        private async Task<HttpResponseMessage> SendRequestAsync(IMediaItem item, string message)
        {
            if (!this.apiKey.TryGetValue(out var apiKey))
                throw new WebApiException("Err:Mobypicture APIキーが使用できません");

            // 参照: http://developers.mobypicture.com/documentation/2-0/upload/

            using var request = new HttpRequestMessage(HttpMethod.Post, UploadEndpoint);
            using var multipart = new MultipartFormDataContent();
            request.Content = multipart;

            using var apiKeyContent = new StringContent(apiKey);
            using var messageContent = new StringContent(message);
            using var mediaStream = item.OpenRead();
            using var mediaContent = new StreamContent(mediaStream);

            multipart.Add(apiKeyContent, "key");
            multipart.Add(messageContent, "message");
            multipart.Add(mediaContent, "media", item.Name);

            return await this.http.SendAsync(request)
                .ConfigureAwait(false);
        }
    }

    public interface IMobypictureApi
    {
        Task<string> UploadFileAsync(IMediaItem item, string message);
    }
}
