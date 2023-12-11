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
    public class TwitterApiConnection : IApiConnection, IApiConnectionLegacy, IDisposable
    {
        public static Uri RestApiBase { get; set; } = new("https://api.twitter.com/1.1/");

        // SettingCommon.xml の TwitterUrl との互換性のために用意
        public static string RestApiHost
        {
            get => RestApiBase.Host;
            set => RestApiBase = new Uri($"https://{value}/1.1/");
        }

        public bool IsDisposed { get; private set; } = false;

        internal HttpClient Http;
        internal HttpClient HttpUpload;
        internal HttpClient HttpStreaming;

        internal ITwitterCredential Credential { get; }

        public TwitterApiConnection()
            : this(new TwitterCredentialNone())
        {
        }

        public TwitterApiConnection(ITwitterCredential credential)
        {
            this.Credential = credential;

            this.InitializeHttpClients();
            Networking.WebProxyChanged += this.Networking_WebProxyChanged;
        }

        [MemberNotNull(nameof(Http), nameof(HttpUpload), nameof(HttpStreaming))]
        private void InitializeHttpClients()
        {
            this.Http = InitializeHttpClient(this.Credential);

            this.HttpUpload = InitializeHttpClient(this.Credential);
            this.HttpUpload.Timeout = Networking.UploadImageTimeout;

            this.HttpStreaming = InitializeHttpClient(this.Credential, disableGzip: true);
            this.HttpStreaming.Timeout = Timeout.InfiniteTimeSpan;
        }

        public async Task<ApiResponse> SendAsync(IHttpRequest request)
        {
            var endpointName = request.EndpointName;

            // レートリミット規制中はAPIリクエストを送信せずに直ちにエラーを発生させる
            if (endpointName != null)
                this.ThrowIfRateLimitExceeded(endpointName);

            using var requestMessage = request.CreateMessage(RestApiBase);

            HttpResponseMessage? responseMessage = null;
            try
            {
                responseMessage = await HandleTimeout(
                    (token) => this.Http.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, token),
                    Networking.DefaultTimeout
                );

                if (endpointName != null)
                    MyCommon.TwitterApiInfo.UpdateFromHeader(responseMessage.Headers, endpointName);

                await TwitterApiConnection.CheckStatusCode(responseMessage)
                    .ConfigureAwait(false);

                var response = new ApiResponse(responseMessage);
                responseMessage = null; // responseMessage は ApiResponse で使用するため破棄されないようにする

                return response;
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
                responseMessage?.Dispose();
            }
        }

        public async Task<T> GetAsync<T>(Uri uri, IDictionary<string, string>? param, string? endpointName)
        {
            var request = new GetRequest
            {
                RequestUri = uri,
                Query = param,
                EndpointName = endpointName,
            };

            using var response = await this.SendAsync(request)
                .ConfigureAwait(false);

            return await response.ReadAsJson<T>()
                .ConfigureAwait(false);
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

                await TwitterApiConnection.CheckStatusCode(response)
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

                await TwitterApiConnection.CheckStatusCode(response)
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

                await TwitterApiConnection.CheckStatusCode(response)
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

                await TwitterApiConnection.CheckStatusCode(response)
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

        public static async Task<T> HandleTimeout<T>(Func<CancellationToken, Task<T>> func, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource();
            var cancellactionToken = cts.Token;

            var task = Task.Run(() => func(cancellactionToken), cancellactionToken);
            var timeoutTask = Task.Delay(timeout);

            if (await Task.WhenAny(task, timeoutTask) == timeoutTask)
            {
                // タイムアウト

                // キャンセル後のタスクで発生した例外は無視する
                static async Task IgnoreExceptions(Task task)
                {
                    try
                    {
                        await task.ConfigureAwait(false);
                    }
                    catch
                    {
                    }
                }
                _ = IgnoreExceptions(task);
                cts.Cancel();

                throw new OperationCanceledException("Timeout", cancellactionToken);
            }

            return await task;
        }

        protected static async Task CheckStatusCode(HttpResponseMessage response)
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

        public OAuthEchoHandler CreateOAuthEchoHandler(HttpMessageHandler innerHandler, Uri authServiceProvider, Uri? realm = null)
        {
            var uri = new Uri(RestApiBase, authServiceProvider);

            if (this.Credential is TwitterCredentialOAuth1 oauthCredential)
            {
                return OAuthEchoHandler.CreateHandler(
                    innerHandler,
                    uri,
                    oauthCredential.AppToken.OAuth1ConsumerKey,
                    oauthCredential.AppToken.OAuth1ConsumerSecret,
                    oauthCredential.Token,
                    oauthCredential.TokenSecret,
                    realm);
            }
            else
            {
                // MobipictureApi クラス向けの暫定対応
                return OAuthEchoHandler.CreateHandler(
                    innerHandler,
                    uri,
                    ApiKey.Create(""),
                    ApiKey.Create(""),
                    "",
                    "",
                    realm);
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
                this.Http.Dispose();
                this.HttpUpload.Dispose();
                this.HttpStreaming.Dispose();
            }
        }

        ~TwitterApiConnection()
            => this.Dispose(false);

        private void Networking_WebProxyChanged(object sender, EventArgs e)
            => this.InitializeHttpClients();

        public static async Task<TwitterCredentialOAuth1> GetRequestTokenAsync(TwitterAppToken appToken)
        {
            var emptyCredential = new TwitterCredentialOAuth1(appToken, "", "");
            var param = new Dictionary<string, string>
            {
                ["oauth_callback"] = "oob",
            };
            var response = await GetOAuthTokenAsync(new Uri("https://api.twitter.com/oauth/request_token"), param, emptyCredential)
                .ConfigureAwait(false);

            return new(appToken, response["oauth_token"], response["oauth_token_secret"]);
        }

        public static Uri GetAuthorizeUri(TwitterCredentialOAuth1 requestToken, string? screenName = null)
        {
            var param = new Dictionary<string, string>
            {
                ["oauth_token"] = requestToken.Token,
            };

            if (screenName != null)
                param["screen_name"] = screenName;

            return new Uri("https://api.twitter.com/oauth/authorize?" + MyCommon.BuildQueryString(param));
        }

        public static async Task<IDictionary<string, string>> GetAccessTokenAsync(TwitterCredentialOAuth1 credential, string verifier)
        {
            var param = new Dictionary<string, string>
            {
                ["oauth_verifier"] = verifier,
            };
            var response = await GetOAuthTokenAsync(new Uri("https://api.twitter.com/oauth/access_token"), param, credential)
                .ConfigureAwait(false);

            return response;
        }

        private static async Task<IDictionary<string, string>> GetOAuthTokenAsync(
            Uri uri,
            IDictionary<string, string> param,
            TwitterCredentialOAuth1 credential
        )
        {
            using var authorizeClient = InitializeHttpClient(credential);

            var requestUri = new Uri(uri, "?" + MyCommon.BuildQueryString(param));

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
                using var response = await authorizeClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

                using var content = response.Content;
                var responseText = await content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                await TwitterApiConnection.CheckStatusCode(response)
                    .ConfigureAwait(false);

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

        private static HttpClient InitializeHttpClient(ITwitterCredential credential, bool disableGzip = false)
        {
            var builder = Networking.CreateHttpClientBuilder();

            builder.SetupHttpClientHandler(x =>
            {
                x.CachePolicy = new RequestCachePolicy(RequestCacheLevel.BypassCache);

                if (disableGzip)
                    x.AutomaticDecompression = DecompressionMethods.None;
            });

            builder.AddHandler(x => credential.CreateHttpHandler(x));

            return builder.Build();
        }
    }
}
