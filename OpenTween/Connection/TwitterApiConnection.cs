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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using OpenTween.Api;
using OpenTween.Api.DataModel;

namespace OpenTween.Connection
{
    public class TwitterApiConnection : IApiConnection, IDisposable
    {
        public static Uri RestApiBase { get; set; } = new("https://api.twitter.com/1.1/");

        // SettingCommon.xml の TwitterUrl との互換性のために用意
        public static string RestApiHost
        {
            get => RestApiBase.Host;
            set => RestApiBase = new Uri($"https://{value}/1.1/");
        }

        public bool IsDisposed { get; private set; } = false;

        public string AccessToken { get; }

        public string AccessSecret { get; }

        internal HttpClient Http;
        internal HttpClient HttpUpload;
        internal HttpClient HttpStreaming;

        private readonly ApiKey consumerKey;
        private readonly ApiKey consumerSecret;

        public TwitterApiConnection(ApiKey consumerKey, ApiKey consumerSecret, string accessToken, string accessSecret)
        {
            this.consumerKey = consumerKey;
            this.consumerSecret = consumerSecret;
            this.AccessToken = accessToken;
            this.AccessSecret = accessSecret;

            this.InitializeHttpClients();
            Networking.WebProxyChanged += this.Networking_WebProxyChanged;
        }

        [MemberNotNull(nameof(Http), nameof(HttpUpload), nameof(HttpStreaming))]
        private void InitializeHttpClients()
        {
            this.Http = InitializeHttpClient(this.consumerKey, this.consumerSecret, this.AccessToken, this.AccessSecret);

            this.HttpUpload = InitializeHttpClient(this.consumerKey, this.consumerSecret, this.AccessToken, this.AccessSecret);
            this.HttpUpload.Timeout = Networking.UploadImageTimeout;

            this.HttpStreaming = InitializeHttpClient(this.consumerKey, this.consumerSecret, this.AccessToken, this.AccessSecret, disableGzip: true);
            this.HttpStreaming.Timeout = Timeout.InfiniteTimeSpan;
        }

        public async Task<T> GetAsync<T>(Uri uri, IDictionary<string, string>? param, string? endpointName)
        {
            // レートリミット規制中はAPIリクエストを送信せずに直ちにエラーを発生させる
            if (endpointName != null)
                this.ThrowIfRateLimitExceeded(endpointName);

            var requestUri = new Uri(RestApiBase, uri);

            if (param != null)
                requestUri = new Uri(requestUri, "?" + MyCommon.BuildQueryString(param));

            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

            try
            {
                using var response = await this.Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

                if (endpointName != null)
                    MyCommon.TwitterApiInfo.UpdateFromHeader(response.Headers, endpointName);

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
                    throw TwitterApiException.CreateFromException(ex, responseText);
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

        /// <summary>
        /// 指定されたエンドポイントがレートリミット規制中であれば例外を発生させる
        /// </summary>
        private void ThrowIfRateLimitExceeded(string endpointName)
        {
            var limit = MyCommon.TwitterApiInfo.AccessLimit[endpointName];
            if (limit == null)
                return;

            if (limit.AccessLimitRemain == 0 && limit.AccessLimitResetDate > DateTimeUtc.Now)
            {
                var error = new TwitterError
                {
                    Errors = new[]
                    {
                        new TwitterErrorItem { Code = TwitterErrorCode.RateLimit, Message = "" },
                    },
                };
                throw new TwitterApiException(0, error, "");
            }
        }

        public async Task<Stream> GetStreamAsync(Uri uri, IDictionary<string, string>? param)
        {
            var requestUri = new Uri(RestApiBase, uri);

            if (param != null)
                requestUri = new Uri(requestUri, "?" + MyCommon.BuildQueryString(param));

            try
            {
                return await this.Http.GetStreamAsync(requestUri)
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

        public async Task<Stream> GetStreamingStreamAsync(Uri uri, IDictionary<string, string>? param)
        {
            var requestUri = new Uri(RestApiBase, uri);

            if (param != null)
                requestUri = new Uri(requestUri, "?" + MyCommon.BuildQueryString(param));

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                var response = await this.HttpStreaming.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

                await this.CheckStatusCode(response)
                    .ConfigureAwait(false);

                return await response.Content.ReadAsStreamAsync()
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

        public async Task<LazyJson<T>> PostLazyAsync<T>(Uri uri, IDictionary<string, string>? param)
        {
            var requestUri = new Uri(RestApiBase, uri);
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            using var postContent = new FormUrlEncodedContent(param);
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

        public async Task<LazyJson<T>> PostLazyAsync<T>(Uri uri, IDictionary<string, string>? param, IDictionary<string, IMediaItem>? media)
        {
            var requestUri = new Uri(RestApiBase, uri);
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            using var postContent = new MultipartFormDataContent();
            if (param != null)
            {
                foreach (var (key, value) in param)
                    postContent.Add(new StringContent(value), key);
            }
            if (media != null)
            {
                foreach (var (key, value) in media)
                    postContent.Add(new StreamContent(value.OpenRead()), key, value.Name);
            }

            request.Content = postContent;

            HttpResponseMessage? response = null;
            try
            {
                response = await this.HttpUpload.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
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

        public async Task PostAsync(Uri uri, IDictionary<string, string>? param, IDictionary<string, IMediaItem>? media)
        {
            var requestUri = new Uri(RestApiBase, uri);
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            using var postContent = new MultipartFormDataContent();
            if (param != null)
            {
                foreach (var (key, value) in param)
                    postContent.Add(new StringContent(value), key);
            }
            if (media != null)
            {
                foreach (var (key, value) in media)
                    postContent.Add(new StreamContent(value.OpenRead()), key, value.Name);
            }

            request.Content = postContent;

            try
            {
                using var response = await this.HttpUpload.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

                await this.CheckStatusCode(response)
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

        public async Task PostJsonAsync(Uri uri, string json)
            => await this.PostJsonAsync<object>(uri, json)
                         .IgnoreResponse()
                         .ConfigureAwait(false);

        public async Task<LazyJson<T>> PostJsonAsync<T>(Uri uri, string json)
        {
            var requestUri = new Uri(RestApiBase, uri);
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);

            using var postContent = new StringContent(json, Encoding.UTF8, "application/json");
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

        public async Task DeleteAsync(Uri uri)
        {
            var requestUri = new Uri(RestApiBase, uri);
            using var request = new HttpRequestMessage(HttpMethod.Delete, requestUri);

            try
            {
                using var response = await this.Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

                await this.CheckStatusCode(response)
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

        protected async Task CheckStatusCode(HttpResponseMessage response)
        {
            var statusCode = response.StatusCode;

            if ((int)statusCode >= 200 && (int)statusCode <= 299)
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

                throw new TwitterApiException(statusCode, error, responseText);
            }
            catch (SerializationException)
            {
                throw new TwitterApiException(statusCode, responseText);
            }
        }

        public OAuthEchoHandler CreateOAuthEchoHandler(Uri authServiceProvider, Uri? realm = null)
        {
            var uri = new Uri(RestApiBase, authServiceProvider);

            return OAuthEchoHandler.CreateHandler(
                Networking.CreateHttpClientHandler(),
                uri,
                this.consumerKey,
                this.consumerSecret,
                this.AccessToken,
                this.AccessSecret,
                realm);
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
                this.Http.Dispose();
                this.HttpUpload.Dispose();
                this.HttpStreaming.Dispose();
            }
        }

        ~TwitterApiConnection()
            => this.Dispose(false);

        private void Networking_WebProxyChanged(object sender, EventArgs e)
            => this.InitializeHttpClients();

        public static Task<(string Token, string TokenSecret)> GetRequestTokenAsync()
            => GetRequestTokenAsync(ApplicationSettings.TwitterConsumerKey, ApplicationSettings.TwitterConsumerSecret);

        public static async Task<(string Token, string TokenSecret)> GetRequestTokenAsync(ApiKey consumerKey, ApiKey consumerSecret)
        {
            var param = new Dictionary<string, string>
            {
                ["oauth_callback"] = "oob",
            };
            var response = await GetOAuthTokenAsync(new Uri("https://api.twitter.com/oauth/request_token"), param, consumerKey, consumerSecret, oauthToken: null)
                .ConfigureAwait(false);

            return (response["oauth_token"], response["oauth_token_secret"]);
        }

        public static Uri GetAuthorizeUri((string Token, string TokenSecret) requestToken, string? screenName = null)
        {
            var param = new Dictionary<string, string>
            {
                ["oauth_token"] = requestToken.Token,
            };

            if (screenName != null)
                param["screen_name"] = screenName;

            return new Uri("https://api.twitter.com/oauth/authorize?" + MyCommon.BuildQueryString(param));
        }

        public static Task<IDictionary<string, string>> GetAccessTokenAsync((string Token, string TokenSecret) requestToken, string verifier)
            => GetAccessTokenAsync(ApplicationSettings.TwitterConsumerKey, ApplicationSettings.TwitterConsumerSecret, requestToken, verifier);

        public static async Task<IDictionary<string, string>> GetAccessTokenAsync(ApiKey consumerKey, ApiKey consumerSecret, (string Token, string TokenSecret) requestToken, string verifier)
        {
            var param = new Dictionary<string, string>
            {
                ["oauth_verifier"] = verifier,
            };
            var response = await GetOAuthTokenAsync(new Uri("https://api.twitter.com/oauth/access_token"), param, consumerKey, consumerSecret, requestToken)
                .ConfigureAwait(false);

            return response;
        }

        private static async Task<IDictionary<string, string>> GetOAuthTokenAsync(
            Uri uri,
            IDictionary<string, string> param,
            ApiKey consumerKey,
            ApiKey consumerSecret,
            (string Token, string TokenSecret)? oauthToken)
        {
            HttpClient authorizeClient;
            if (oauthToken != null)
                authorizeClient = InitializeHttpClient(consumerKey, consumerSecret, oauthToken.Value.Token, oauthToken.Value.TokenSecret);
            else
                authorizeClient = InitializeHttpClient(consumerKey, consumerSecret, "", "");

            var requestUri = new Uri(uri, "?" + MyCommon.BuildQueryString(param));

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
                using var response = await authorizeClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

                using var content = response.Content;
                var responseText = await content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                    throw new TwitterApiException(response.StatusCode, responseText);

                var responseParams = HttpUtility.ParseQueryString(responseText);

                return responseParams.Cast<string>()
                    .ToDictionary(x => x, x => responseParams[x]);
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

        private static HttpClient InitializeHttpClient(ApiKey consumerKey, ApiKey consumerSecret, string accessToken, string accessSecret, bool disableGzip = false)
        {
            var innerHandler = Networking.CreateHttpClientHandler();
            innerHandler.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

            if (disableGzip)
                innerHandler.AutomaticDecompression = DecompressionMethods.None;

            var handler = new OAuthHandler(innerHandler, consumerKey, consumerSecret, accessToken, accessSecret);

            return Networking.CreateHttpClient(handler);
        }
    }
}
