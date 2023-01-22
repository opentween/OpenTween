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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using OpenTween.Api.DataModel;

namespace OpenTween.Connection
{
    public sealed class MastodonApiConnection : IMastodonApiConnection
    {
        public Uri InstanceUri { get; }

        public Uri WebsocketUri { get; }

        public string? AccessToken { get; }

        internal HttpClient Http = null!;

        public MastodonApiConnection(Uri instanceUri)
            : this(instanceUri, accessToken: null)
        {
        }

        public MastodonApiConnection(Uri instanceUri, string? accessToken)
        {
            this.InstanceUri = instanceUri;
            this.AccessToken = accessToken;

            var websocketUri = new UriBuilder(this.InstanceUri);
            websocketUri.Scheme = websocketUri.Scheme == "https" ? "wss" : "ws";
            this.WebsocketUri = websocketUri.Uri;

            this.InitializeHttpClient();
            Networking.WebProxyChanged += this.Networking_WebProxyChanged;
        }

        public async Task<T> GetAsync<T>(Uri uri, IEnumerable<KeyValuePair<string, string>>? param)
        {
            var requestUri = new Uri(this.InstanceUri, uri);

            if (param != null)
                requestUri = new Uri(requestUri, "?" + MyCommon.BuildQueryString(param));

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                if (!MyCommon.IsNullOrEmpty(this.AccessToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);

                using var response = await this.Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

                await this.CheckStatusCode(response)
                    .ConfigureAwait(false);

                using var content = response.Content;
                var responseText = await content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                try
                {
                    return MyCommon.CreateDataFromJson<T>(responseText);
                }
                catch (SerializationException ex)
                {
                    throw new WebApiException("Invalid Response", responseText, ex);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new WebApiException(ex.InnerException?.Message ?? ex.Message, ex);
            }
            catch (OperationCanceledException ex)
            {
                throw new WebApiException("Timeout", ex);
            }
        }

        public async Task<Stream> GetStreamAsync(Uri uri, IEnumerable<KeyValuePair<string, string>>? param)
        {
            var requestUri = new Uri(this.InstanceUri, uri);

            if (param != null)
                requestUri = new Uri(requestUri, "?" + MyCommon.BuildQueryString(param));

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                if (!MyCommon.IsNullOrEmpty(this.AccessToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);

                using var response = await this.Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

                await this.CheckStatusCode(response)
                    .ConfigureAwait(false);

                return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                throw new WebApiException(ex.InnerException?.Message ?? ex.Message, ex);
            }
            catch (OperationCanceledException ex)
            {
                throw new WebApiException("Timeout", ex);
            }
        }

        public Task<LazyJson<T>> PostLazyAsync<T>(Uri uri, IEnumerable<KeyValuePair<string, string>>? param)
            => this.PostLazyAsync<T>(HttpMethod.Post, uri, param);

        public async Task<LazyJson<T>> PostLazyAsync<T>(HttpMethod method, Uri uri, IEnumerable<KeyValuePair<string, string>>? param)
        {
            var requestUri = new Uri(this.InstanceUri, uri);

            if (param == null)
                param = Enumerable.Empty<KeyValuePair<string, string>>();

            try
            {
                using var request = new HttpRequestMessage(method, requestUri);
                using var postContent = new FormUrlEncodedContent(param);

                if (!string.IsNullOrEmpty(this.AccessToken))
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", this.AccessToken);

                request.Content = postContent;

                HttpResponseMessage? response = null;
                try
                {
                    response = await this.Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false);

                    await this.CheckStatusCode(response)
                        .ConfigureAwait(false);

                    var result = new LazyJson<T>(response);
                    response = null;

                    return result;
                }
                finally
                {
                    response?.Dispose();
                }
            }
            catch (HttpRequestException ex)
            {
                throw new WebApiException(ex.InnerException?.Message ?? ex.Message, ex);
            }
            catch (OperationCanceledException ex)
            {
                throw new WebApiException("Timeout", ex);
            }
        }

        public void Dispose()
        {
            Networking.WebProxyChanged -= this.Networking_WebProxyChanged;
            this.Http.Dispose();
        }

        private async Task CheckStatusCode(HttpResponseMessage response)
        {
            var statusCode = response.StatusCode;
            if (statusCode == HttpStatusCode.OK)
                return;

            string responseText;
            using (var content = response.Content)
            {
                responseText = await content.ReadAsStringAsync()
                    .ConfigureAwait(false);
            }

            if (!string.IsNullOrWhiteSpace(responseText))
            {
                try
                {
                    var error = MyCommon.CreateDataFromJson<MastodonError>(responseText);
                    var errorText = error?.Error;

                    if (!MyCommon.IsNullOrEmpty(errorText))
                        throw new WebApiException(errorText, responseText);
                }
                catch (SerializationException)
                {
                }
            }

            throw new WebApiException(statusCode.ToString(), responseText);
        }

        private void InitializeHttpClient()
        {
            var innerHandler = Networking.CreateHttpClientHandler();
            innerHandler.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

            this.Http = Networking.CreateHttpClient(innerHandler);
        }

        private void Networking_WebProxyChanged(object sender, EventArgs e)
            => this.InitializeHttpClient();
    }
}
