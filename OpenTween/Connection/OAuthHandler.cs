// OpenTween - Client of Twitter
// Copyright (c) 2015 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace OpenTween.Connection
{
    /// <summary>
    /// HttpClientで使用するOAuthハンドラー
    /// </summary>
    public class OAuthHandler : DelegatingHandler
    {
        public string ConsumerKey { get; }
        public string ConsumerSecret { get; }
        public string AccessToken { get; }
        public string AccessSecret { get; }

        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        public OAuthHandler(string consumerKey, string consumerSecret, string accessToken, string accessSecret)
            : this(new HttpClientHandler(), consumerKey, consumerSecret, accessToken, accessSecret)
        {
        }

        public OAuthHandler(HttpMessageHandler innerHandler, string consumerKey, string consumerSecret, string accessToken, string accessSecret)
            : base(innerHandler)
        {
            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecret;
            this.AccessToken = accessToken;
            this.AccessSecret = accessSecret;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var query = await GetParameters(request.RequestUri, request.Content)
                .ConfigureAwait(false);

            var credential = OAuthUtility.CreateAuthorization(request.Method.ToString().ToUpperInvariant(), request.RequestUri, query,
                this.ConsumerKey, this.ConsumerSecret, this.AccessToken, this.AccessSecret);
            request.Headers.TryAddWithoutValidation("Authorization", credential);

            return await base.SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// OAuthの署名の対象となるパラメータを抽出します
        /// </summary>
        internal static async Task<IEnumerable<KeyValuePair<string, string>>> GetParameters(Uri requestUri, HttpContent content)
        {
            var parameters = Enumerable.Empty<KeyValuePair<string, string>>();

            var postContent = content as FormUrlEncodedContent;
            if (postContent != null)
            {
                var query = await postContent.ReadAsStringAsync()
                    .ConfigureAwait(false);

                var postParams = HttpUtility.ParseQueryString(query);
                var postParamsKvp = postParams.AllKeys.Cast<string>()
                    .Select(x => new KeyValuePair<string, string>(x, postParams[x]));
                parameters = parameters.Concat(postParamsKvp);
            }

            var getParams = HttpUtility.ParseQueryString(requestUri.Query);
            var getParamsKvp = getParams.AllKeys.Cast<string>()
                .Select(x => new KeyValuePair<string, string>(x, getParams[x]));
            parameters = parameters.Concat(getParamsKvp);

            return parameters;
        }
    }
}
