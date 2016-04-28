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
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using OpenTween.Api;
using OpenTween.Api.DataModel;

namespace OpenTween.Connection
{
    public class TwitterApiConnection : IApiConnection, IDisposable
    {
        public Uri RestApiBase { get; set; } = new Uri("https://api.twitter.com/1.1/");

        public bool IsDisposed { get; private set; } = false;

        public string AccessToken { get; }
        public string AccessSecret { get; }

        internal HttpClient http;

        public TwitterApiConnection(string accessToken, string accessSecret)
        {
            this.AccessToken = accessToken;
            this.AccessSecret = accessSecret;

            this.InitializeHttpClient(accessToken, accessSecret);
            Networking.WebProxyChanged += this.Networking_WebProxyChanged;
        }

        public async Task<T> GetAsync<T>(Uri uri, IDictionary<string, string> param)
        {
            var requestUri = new Uri(this.RestApiBase, uri);

            if (param != null)
                requestUri = new Uri(requestUri, "?" + MyCommon.BuildQueryString(param));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            try
            {
                using (var response = await this.http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false))
                {
                    await this.CheckStatusCode(response)
                        .ConfigureAwait(false);

                    using (var content = response.Content)
                    {
                        var responseText = await content.ReadAsStringAsync()
                            .ConfigureAwait(false);

                        try
                        {
                            return MyCommon.CreateDataFromJson<T>(responseText);
                        }
                        catch (SerializationException ex)
                        {
                            throw TwitterApiException.CreateFromException(ex, responseText);
                        }
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                throw TwitterApiException.CreateFromException(ex);
            }
        }

        public async Task<LazyJson<T>> PostLazyAsync<T>(Uri uri, IDictionary<string, string> param)
        {
            var requestUri = new Uri(this.RestApiBase, uri);
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            using (var postContent = new FormUrlEncodedContent(param))
            {
                request.Content = postContent;

                HttpResponseMessage response = null;
                try
                {
                    response = await this.http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false);

                    await this.CheckStatusCode(response)
                        .ConfigureAwait(false);

                    var result = new LazyJson<T>(response);
                    response = null;

                    return result;
                }
                catch (OperationCanceledException ex)
                {
                    throw TwitterApiException.CreateFromException(ex);
                }
                finally
                {
                    response?.Dispose();
                }
            }
        }

        public async Task<LazyJson<T>> PostLazyAsync<T>(Uri uri, IDictionary<string, string> param, IDictionary<string, IMediaItem> media)
        {
            var requestUri = new Uri(this.RestApiBase, uri);
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            using (var postContent = new MultipartFormDataContent())
            {
                foreach (var kv in param)
                    postContent.Add(new StringContent(kv.Value), kv.Key);

                foreach (var kv in media)
                    postContent.Add(new StreamContent(kv.Value.OpenRead()), kv.Key);

                request.Content = postContent;

                HttpResponseMessage response = null;
                try
                {
                    response = await this.http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(false);

                    await this.CheckStatusCode(response)
                        .ConfigureAwait(false);

                    var result = new LazyJson<T>(response);
                    response = null;

                    return result;
                }
                catch (OperationCanceledException ex)
                {
                    throw TwitterApiException.CreateFromException(ex);
                }
                finally
                {
                    response?.Dispose();
                }
            }
        }

        protected async Task CheckStatusCode(HttpResponseMessage response)
        {
            var statusCode = response.StatusCode;
            if (statusCode == HttpStatusCode.OK)
            {
                Twitter.AccountState = MyCommon.ACCOUNT_STATE.Valid;
                return;
            }

            string responseText;
            using (var content = response.Content)
            {
                responseText = await content.ReadAsStringAsync()
                    .ConfigureAwait(false);
            }

            if (string.IsNullOrWhiteSpace(responseText))
            {
                if (statusCode == HttpStatusCode.Unauthorized)
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;

                throw new TwitterApiException(statusCode, responseText);
            }

            try
            {
                var error = TwitterError.ParseJson(responseText);

                if (error?.Errors == null || error.Errors.Length == 0)
                    throw new TwitterApiException(statusCode, responseText);

                var errorCodes = error.Errors.Select(x => x.Code);
                if (errorCodes.Any(x => x == TwitterErrorCode.InternalError || x == TwitterErrorCode.SuspendedAccount))
                {
                    Twitter.AccountState = MyCommon.ACCOUNT_STATE.Invalid;
                }

                throw new TwitterApiException(error, responseText);
            }
            catch (SerializationException)
            {
                throw new TwitterApiException(statusCode, responseText);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.IsDisposed)
                return;

            this.IsDisposed = true;

            if (disposing)
            {
                Networking.WebProxyChanged -= this.Networking_WebProxyChanged;
                this.http.Dispose();
            }
        }

        ~TwitterApiConnection()
        {
            this.Dispose(false);
        }

        private void InitializeHttpClient(string accessToken, string accessSecret)
        {
            var handler = new OAuthHandler(Networking.CreateHttpClientHandler(),
                ApplicationSettings.TwitterConsumerKey, ApplicationSettings.TwitterConsumerSecret,
                accessToken, accessSecret);

            this.http = Networking.CreateHttpClient(handler);
        }

        private void Networking_WebProxyChanged(object sender, EventArgs e)
        {
            this.InitializeHttpClient(this.AccessToken, this.AccessSecret);
        }
    }
}
