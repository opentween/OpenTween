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
using OpenTween.Connection;

namespace OpenTween.Api
{
    public class MicrosoftTranslatorApi
    {
        public static readonly Uri OAuthEndpoint = new Uri("https://datamarket.accesscontrol.windows.net/v2/OAuth2-13");
        public static readonly Uri TranslateEndpoint = new Uri("https://api.microsofttranslator.com/v2/Http.svc/Translate");

        public string AccessToken { get; internal set; }
        public DateTime RefreshAccessTokenAt { get; internal set; }

        private HttpClient Http => this.localHttpClient ?? Networking.Http;
        private readonly HttpClient localHttpClient;

        public MicrosoftTranslatorApi()
            : this(null)
        {
        }

        public MicrosoftTranslatorApi(HttpClient http)
        {
            this.localHttpClient = http;
        }

        public async Task<string> TranslateAsync(string text, string langTo, string langFrom = null)
        {
            await this.UpdateAccessTokenIfExpired()
                .ConfigureAwait(false);

            var param = new Dictionary<string, string>
            {
                ["text"] = text,
                ["to"] = langTo,
            };

            if (langFrom != null)
                param["from"] = langFrom;

            var requestUri = new Uri(TranslateEndpoint, "?" + MyCommon.BuildQueryString(param));

            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);

                using (var response = await this.Http.SendAsync(request).ConfigureAwait(false))
                {
                    return await response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);
                }
            }
        }

        public async Task UpdateAccessTokenIfExpired()
        {
            if (this.AccessToken != null && this.RefreshAccessTokenAt > DateTime.Now)
                return;

            var accessToken = await this.GetAccessTokenAsync()
                .ConfigureAwait(false);

            this.AccessToken = accessToken.Item1;

            // expires_in の示す時刻より 30 秒早めに再発行する
            this.RefreshAccessTokenAt = DateTime.Now + accessToken.Item2 - TimeSpan.FromSeconds(30);
        }

        internal virtual async Task<Tuple<string, TimeSpan>> GetAccessTokenAsync()
        {
            var param = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = ApplicationSettings.AzureClientId,
                ["client_secret"] = ApplicationSettings.AzureClientSecret,
                ["scope"] = "http://api.microsofttranslator.com",
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, OAuthEndpoint))
            using (var postContent = new FormUrlEncodedContent(param))
            {
                request.Content = postContent;

                using (var response = await this.Http.SendAsync(request).ConfigureAwait(false))
                {
                    var responseBytes = await response.Content.ReadAsByteArrayAsync()
                        .ConfigureAwait(false);

                    return ParseOAuthCredential(responseBytes);
                }
            }
        }

        internal static Tuple<string, TimeSpan> ParseOAuthCredential(byte[] responseBytes)
        {
            using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(responseBytes, XmlDictionaryReaderQuotas.Max))
            {
                var xElm = XElement.Load(jsonReader);

                var tokenTypeElm = xElm.Element("token_type");
                if (tokenTypeElm == null)
                    throw new WebApiException("Property `token_type` required");

                var accessTokenElm = xElm.Element("access_token");
                if (accessTokenElm == null)
                    throw new WebApiException("Property `access_token` required");

                var expiresInElm = xElm.Element("expires_in");

                int expiresInSeconds;
                if (expiresInElm != null)
                {
                    if (!int.TryParse(expiresInElm.Value, out expiresInSeconds))
                        throw new WebApiException("Invalid number: expires_in = " + expiresInElm.Value);
                }
                else
                {
                    // expires_in が省略された場合は有効期間が不明なので、
                    // 次回のリクエスト時は経過時間に関わらずアクセストークンの再発行を行う
                    expiresInSeconds = 0;
                }

                return Tuple.Create(accessTokenElm.Value, TimeSpan.FromSeconds(expiresInSeconds));
            }
        }
    }
}
