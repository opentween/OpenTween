// OpenTween - Client of Twitter
// Copyright (c) 2017 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using OpenTween.Connection;

namespace OpenTween.Api
{
    public class BitlyApi
    {
        public static readonly Uri ApiBase = new("https://api-ssl.bitly.com/");

        public string EndUserAccessToken { get; set; } = "";

        public string EndUserLoginName { get; set; } = "";

        public string EndUserApiKey { get; set; } = "";

        private readonly ApiKey clientId;
        private readonly ApiKey clientSecret;

        private HttpClient Http => this.localHttpClient ?? Networking.Http;

        private readonly HttpClient? localHttpClient;

        public BitlyApi()
            : this(ApplicationSettings.BitlyClientId, ApplicationSettings.BitlyClientSecret, null)
        {
        }

        public BitlyApi(ApiKey clientId, ApiKey clientSecret, HttpClient? http)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.localHttpClient = http;
        }

        public async Task<Uri> ShortenAsync(Uri srcUri, string? domain = null)
        {
            var query = new Dictionary<string, string>
            {
                ["format"] = "txt",
                ["longUrl"] = srcUri.OriginalString,
            };

            if (!MyCommon.IsNullOrEmpty(domain))
                query["domain"] = domain;

            var uri = new Uri("/v3/shorten", UriKind.Relative);
            var responseText = await this.GetAsync(uri, query).ConfigureAwait(false);

            if (!Regex.IsMatch(responseText, @"^https?://"))
                throw new WebApiException("Failed to create URL.", responseText);

            return new Uri(responseText.TrimEnd());
        }

        public async Task<string> GetAsync(Uri endpoint, IEnumerable<KeyValuePair<string, string>> param)
        {
            var paramWithToken = param.Concat(this.CreateAccessTokenParams());

            var requestUri = new Uri(new Uri(ApiBase, endpoint), "?" + MyCommon.BuildQueryString(paramWithToken));
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            using var response = await this.Http.SendAsync(request)
                .ConfigureAwait(false);

            return await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
        }

        public async Task<string> GetAccessTokenAsync(string username, string password)
        {
            var param = new Dictionary<string, string>
            {
                ["grant_type"] = "password",
                ["username"] = username,
                ["password"] = password,
            };

            var endpoint = new Uri(ApiBase, "/oauth/access_token");

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            using var postContent = new FormUrlEncodedContent(param);

            if (!(this.clientId, this.clientSecret).TryGetValue(out var keyPair))
                throw new WebApiException("bit.ly APIキーが使用できません");

            var (clientId, clientSecret) = keyPair;

            var authzParam = clientId + ":" + clientSecret;
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(authzParam)));

            request.Content = postContent;

            using var response = await this.Http.SendAsync(request)
                .ConfigureAwait(false);
            var responseBytes = await response.Content.ReadAsByteArrayAsync()
                .ConfigureAwait(false);

            return this.ParseOAuthCredential(responseBytes);
        }

        private string ParseOAuthCredential(byte[] responseBytes)
        {
            using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(responseBytes, XmlDictionaryReaderQuotas.Max);
            var xElm = XElement.Load(jsonReader);

            var statusCode = xElm.Element("status_code")?.Value ?? "200";
            if (statusCode != "200")
            {
                var statusText = xElm.Element("status_txt")?.Value;
                throw new WebApiException(statusText ?? $"status_code = {statusCode}");
            }

            var accessToken = xElm.Element("access_token")?.Value;
            if (accessToken == null)
                throw new WebApiException("Property `access_token` required");

            return accessToken;
        }

        private IEnumerable<KeyValuePair<string, string>> CreateAccessTokenParams()
        {
            if (MyCommon.IsNullOrEmpty(this.EndUserAccessToken))
            {
                return new[]
                {
                    new KeyValuePair<string, string>("login", this.EndUserLoginName),
                    new KeyValuePair<string, string>("apiKey", this.EndUserApiKey),
                };
            }

            return new[]
            {
                new KeyValuePair<string, string>("access_token", this.EndUserAccessToken),
            };
        }
    }
}
