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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using OpenTween.Api;
using OpenTween.Api.DataModel;

namespace OpenTween.Connection
{
    public class TwitterApiConnection : IApiConnection, IDisposable
    {
        public static Uri RestApiBase { get; set; } = new Uri("https://api.twitter.com/1.1/");

        public bool IsDisposed { get; private set; } = false;

        public string AccessToken { get; }
        public string AccessSecret { get; }

        internal HttpClient http;

        public TwitterApiConnection(string accessToken, string accessSecret)
        {
            this.AccessToken = accessToken;
            this.AccessSecret = accessSecret;

            this.http = InitializeHttpClient(accessToken, accessSecret);
            Networking.WebProxyChanged += this.Networking_WebProxyChanged;
        }

        public Task<T> GetAsync<T>(Uri uri, IDictionary<string, string> param)
            => this.GetAsync<T>(uri, param, null);

        public async Task<T> GetAsync<T>(Uri uri, IDictionary<string, string> param, string endpointName)
        {
            var requestUri = new Uri(RestApiBase, uri);

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

                    if (endpointName != null)
                        MyCommon.TwitterApiInfo.UpdateFromHeader(response.Headers, endpointName);

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
            catch (HttpRequestException ex)
            {
                throw TwitterApiException.CreateFromException(ex);
            }
            catch (OperationCanceledException ex)
            {
                throw TwitterApiException.CreateFromException(ex);
            }
        }

        public async Task<Stream> GetStreamAsync(Uri uri, IDictionary<string, string> param)
        {
            var requestUri = new Uri(RestApiBase, uri);

            if (param != null)
                requestUri = new Uri(requestUri, "?" + MyCommon.BuildQueryString(param));

            try
            {
                return await this.http.GetStreamAsync(requestUri)
                    .ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                throw TwitterApiException.CreateFromException(ex);
            }
            catch (OperationCanceledException ex)
            {
                throw TwitterApiException.CreateFromException(ex);
            }
        }

        public async Task<LazyJson<T>> PostLazyAsync<T>(Uri uri, IDictionary<string, string> param)
        {
            var requestUri = new Uri(RestApiBase, uri);
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
                catch (HttpRequestException ex)
                {
                    throw TwitterApiException.CreateFromException(ex);
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
            var requestUri = new Uri(RestApiBase, uri);
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            using (var postContent = new MultipartFormDataContent())
            {
                foreach (var kv in param)
                    postContent.Add(new StringContent(kv.Value), kv.Key);

                foreach (var kv in media)
                    postContent.Add(new StreamContent(kv.Value.OpenRead()), kv.Key, kv.Value.Name);

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
                catch (HttpRequestException ex)
                {
                    throw TwitterApiException.CreateFromException(ex);
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

        public OAuthEchoHandler CreateOAuthEchoHandler(Uri authServiceProvider, Uri realm = null)
        {
            var uri = new Uri(RestApiBase, authServiceProvider);

            return OAuthEchoHandler.CreateHandler(Networking.CreateHttpClientHandler(), uri,
                ApplicationSettings.TwitterConsumerKey, ApplicationSettings.TwitterConsumerSecret,
                this.AccessToken, this.AccessSecret, realm);
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

        private void Networking_WebProxyChanged(object sender, EventArgs e)
        {
            this.http = InitializeHttpClient(this.AccessToken, this.AccessSecret);
        }

        public static async Task<Tuple<string, string>> GetRequestTokenAsync()
        {
            var param = new Dictionary<string, string>
            {
                ["oauth_callback"] = "oob",
            };
            var response = await GetOAuthTokenAsync(new Uri("https://api.twitter.com/oauth/request_token"), param, oauthToken: null)
                .ConfigureAwait(false);

            return Tuple.Create(response["oauth_token"], response["oauth_token_secret"]);
        }

        public static Uri GetAuthorizeUri(Tuple<string, string> requestToken, string screenName = null)
        {
            var param = new Dictionary<string, string>
            {
                ["oauth_token"] = requestToken.Item1,
            };

            if (screenName != null)
                param["screen_name"] = screenName;

            return new Uri("https://api.twitter.com/oauth/authorize?" + MyCommon.BuildQueryString(param));
        }

        public static async Task<IDictionary<string, string>> GetAccessTokenAsync(Tuple<string, string> requestToken, string verifier)
        {
            var param = new Dictionary<string, string>
            {
                ["oauth_verifier"] = verifier,
            };
            var response = await GetOAuthTokenAsync(new Uri("https://api.twitter.com/oauth/access_token"), param, requestToken)
                .ConfigureAwait(false);

            return response;
        }

        private static async Task<IDictionary<string, string>> GetOAuthTokenAsync(Uri uri, IDictionary<string, string> param,
            Tuple<string, string> oauthToken)
        {
            HttpClient authorizeClient;
            if (oauthToken != null)
                authorizeClient = InitializeHttpClient(oauthToken.Item1, oauthToken.Item2);
            else
                authorizeClient = InitializeHttpClient("", "");

            var requestUri = new Uri(uri, "?" + MyCommon.BuildQueryString(param));

            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
                using (var response = await authorizeClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false))
                using (var content = response.Content)
                {
                    var responseText = await content.ReadAsStringAsync()
                        .ConfigureAwait(false);

                    if (!response.IsSuccessStatusCode)
                        throw new TwitterApiException(response.StatusCode, responseText);

                    var responseParams = HttpUtility.ParseQueryString(responseText);

                    return responseParams.Cast<string>()
                        .ToDictionary(x => x, x => responseParams[x]);
                }
            }
            catch (HttpRequestException ex)
            {
                throw TwitterApiException.CreateFromException(ex);
            }
            catch (OperationCanceledException ex)
            {
                throw TwitterApiException.CreateFromException(ex);
            }
        }

        private static HttpClient InitializeHttpClient(string accessToken, string accessSecret)
        {
            var innerHandler = Networking.CreateHttpClientHandler();
            innerHandler.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

            var handler = new OAuthHandler(innerHandler,
                ApplicationSettings.TwitterConsumerKey, ApplicationSettings.TwitterConsumerSecret,
                accessToken, accessSecret);

            return Networking.CreateHttpClient(handler);
        }
    }
}
