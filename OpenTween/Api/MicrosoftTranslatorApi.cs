// OpenTween - Client of Twitter
// Copyright (c) 2016 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Connection;

namespace OpenTween.Api
{
    public class MicrosoftTranslatorApi
    {
        public static readonly Uri IssueTokenEndpoint = new("https://api.cognitive.microsoft.com/sts/v1.0/issueToken");
        public static readonly Uri TranslateEndpoint = new("https://api.cognitive.microsofttranslator.com/translate");

        public string AccessToken { get; internal set; } = "";

        public DateTimeUtc RefreshAccessTokenAt { get; internal set; } = DateTimeUtc.MinValue;

        private readonly ApiKey subscriptionKey;

        private HttpClient Http => this.localHttpClient ?? Networking.Http;

        private readonly HttpClient? localHttpClient;

        public MicrosoftTranslatorApi()
            : this(ApplicationSettings.TranslatorSubscriptionKey, null)
        {
        }

        public MicrosoftTranslatorApi(ApiKey subscriptionKey, HttpClient? http)
        {
            this.subscriptionKey = subscriptionKey;
            this.localHttpClient = http;
        }

        public async Task<string> TranslateAsync(string text, string langTo, string? langFrom = null)
        {
            if (!this.subscriptionKey.TryGetValue(out _))
                throw new WebApiException("APIキーが使用できません");

            await this.UpdateAccessTokenIfExpired()
                .ConfigureAwait(false);

            var param = new Dictionary<string, string>
            {
                ["api-version"] = "3.0",
                ["to"] = langTo,
            };

            if (langFrom != null)
                param["from"] = langFrom;

            var requestUri = new Uri(TranslateEndpoint, "?" + MyCommon.BuildQueryString(param));

            using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);

            var escapedText = JsonUtils.EscapeJsonString(text);
            var json = $@"[{{""Text"": ""{escapedText}""}}]";

            using var body = new StringContent(json, Encoding.UTF8, "application/json");
            request.Content = body;

            using var response = await this.Http.SendAsync(request)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new WebApiException(response.StatusCode.ToString());

            var responseJson = await response.Content.ReadAsByteArrayAsync()
                .ConfigureAwait(false);

            using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(responseJson, XmlDictionaryReaderQuotas.Max);
            var xElm = XElement.Load(jsonReader);
            var transtlationTextElm = xElm.XPathSelectElement("/item/translations/item/text[1]");

            return transtlationTextElm?.Value ?? "";
        }

        public async Task UpdateAccessTokenIfExpired()
        {
            if (!MyCommon.IsNullOrEmpty(this.AccessToken) && this.RefreshAccessTokenAt > DateTimeUtc.Now)
                return;

            var (accessToken, expiresIn) = await this.GetAccessTokenAsync()
                .ConfigureAwait(false);

            this.AccessToken = accessToken;

            // アクセストークンの実際の有効期限より 30 秒早めに失効として扱う
            this.RefreshAccessTokenAt = DateTimeUtc.Now + expiresIn - TimeSpan.FromSeconds(30);
        }

        internal virtual async Task<(string AccessToken, TimeSpan ExpiresIn)> GetAccessTokenAsync()
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, IssueTokenEndpoint);
            request.Headers.Add("Ocp-Apim-Subscription-Key", this.subscriptionKey.Value);

            using var response = await this.Http.SendAsync(request)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new WebApiException(response.StatusCode.ToString());

            var accessToken = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);

            return (accessToken, TimeSpan.FromMinutes(10));
        }
    }
}
