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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenTween.Connection;

namespace OpenTween.Api
{
    public class BitlyApi
    {
        public static readonly Uri ApiBase = new Uri("https://api-ssl.bitly.com/");

        public string EndUserAccessToken { get; set; }

        public string EndUserLoginName { get; set; }
        public string EndUserApiKey { get; set; }

        private HttpClient http => this.localHttpClient ?? Networking.Http;
        private readonly HttpClient localHttpClient;

        public BitlyApi()
            : this(null)
        {
        }

        public BitlyApi(HttpClient http)
        {
            this.localHttpClient = http;
        }

        public async Task<Uri> ShortenAsync(Uri srcUri, string domain = null)
        {
            var query = new Dictionary<string, string>
            {
                ["format"] = "txt",
                ["longUrl"] = srcUri.OriginalString,
            };

            if (!string.IsNullOrEmpty(domain))
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

            using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
            using (var response = await this.http.SendAsync(request).ConfigureAwait(false))
            {
                return await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);
            }
        }

        public bool ValidateApiKey(string login, string apiKey)
            => this.ValidateApiKeyAsync(login, apiKey).Result;

        public async Task<bool> ValidateApiKeyAsync(string login, string apikey)
        {
            try
            {
                var requestUri = new Uri(ApiBase, "/v3/validate");
                var param = new Dictionary<string, string>
                {
                    ["login"] = ApplicationSettings.BitlyLoginId,
                    ["apiKey"] = ApplicationSettings.BitlyApiKey,
                    ["x_login"] = login,
                    ["x_apiKey"] = apikey,
                    ["format"] = "txt",
                };

                using (var postContent = new FormUrlEncodedContent(param))
                using (var response = await this.http.PostAsync(requestUri, postContent).ConfigureAwait(false))
                {
                    var responseText = await response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);

                    return responseText.TrimEnd() == "1";
                }
            }
            catch (OperationCanceledException) { }
            catch (HttpRequestException) { }

            return false;
        }

        private IEnumerable<KeyValuePair<string, string>> CreateAccessTokenParams()
        {
            if (string.IsNullOrEmpty(this.EndUserAccessToken))
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
