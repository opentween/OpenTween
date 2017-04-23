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
using System.Threading.Tasks;
using OpenTween.Connection;

namespace OpenTween.Api
{
    public class BitlyApi
    {
        public static readonly Uri ApiBase = new Uri("http://api.bit.ly/");

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
    }
}
