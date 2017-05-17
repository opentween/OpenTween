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
        public static readonly Uri IssueTokenEndpoint = new Uri("https://api.cognitive.microsoft.com/sts/v1.0/issueToken");
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

            var (accessToken, expiresIn) = await this.GetAccessTokenAsync()
                .ConfigureAwait(false);

            this.AccessToken = accessToken;

            // アクセストークンの実際の有効期限より 30 秒早めに失効として扱う
            this.RefreshAccessTokenAt = DateTime.Now + expiresIn - TimeSpan.FromSeconds(30);
        }

        internal virtual async Task<(string AccessToken, TimeSpan ExpiresIn)> GetAccessTokenAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, IssueTokenEndpoint))
            {
                request.Headers.Add("Ocp-Apim-Subscription-Key", ApplicationSettings.TranslatorSubscriptionKey);

                using (var response = await this.Http.SendAsync(request).ConfigureAwait(false))
                {
                    var accessToken = await response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);

                    return (accessToken, TimeSpan.FromMinutes(10));
                }
            }
        }
    }
}
