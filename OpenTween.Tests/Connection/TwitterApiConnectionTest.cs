﻿// OpenTween - Client of Twitter
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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Moq;
using OpenTween.Api;
using OpenTween.Api.DataModel;
using Xunit;

namespace OpenTween.Connection
{
    public class TwitterApiConnectionTest
    {
        public TwitterApiConnectionTest()
            => this.MyCommonSetup();

        private void MyCommonSetup()
        {
            var mockAssembly = new Mock<_Assembly>();
            mockAssembly.Setup(m => m.GetName()).Returns(new AssemblyName("OpenTween"));

            MyCommon.EntryAssembly = mockAssembly.Object;
        }

        [Fact]
        public async Task SendAsync_Test()
        {
            using var mockHandler = new HttpMessageHandlerMock();
            using var http = new HttpClient(mockHandler);
            using var apiConnection = new TwitterApiConnection();
            apiConnection.Http = http;

            mockHandler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Get, x.Method);
                Assert.Equal("https://api.twitter.com/1.1/hoge/tetete.json",
                    x.RequestUri.GetLeftPart(UriPartial.Path));

                var query = HttpUtility.ParseQueryString(x.RequestUri.Query);

                Assert.Equal("1111", query["aaaa"]);
                Assert.Equal("2222", query["bbbb"]);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("\"hogehoge\""),
                };
            });

            var request = new GetRequest
            {
                RequestUri = new("hoge/tetete.json", UriKind.Relative),
                Query = new Dictionary<string, string>
                {
                    ["aaaa"] = "1111",
                    ["bbbb"] = "2222",
                },
                EndpointName = "/hoge/tetete",
            };

            using var response = await apiConnection.SendAsync(request);

            Assert.Equal("hogehoge", await response.ReadAsJson<string>());

            Assert.Equal(0, mockHandler.QueueCount);
        }

        [Fact]
        public async Task SendAsync_UpdateRateLimitTest()
        {
            using var mockHandler = new HttpMessageHandlerMock();
            using var http = new HttpClient(mockHandler);
            using var apiConnection = new TwitterApiConnection();
            apiConnection.Http = http;

            mockHandler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Get, x.Method);
                Assert.Equal("https://api.twitter.com/1.1/hoge/tetete.json",
                    x.RequestUri.GetLeftPart(UriPartial.Path));

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Headers =
                    {
                        { "X-Rate-Limit-Limit", "150" },
                        { "X-Rate-Limit-Remaining", "100" },
                        { "X-Rate-Limit-Reset", "1356998400" },
                        { "X-Access-Level", "read-write-directmessages" },
                    },
                    Content = new StringContent("\"hogehoge\""),
                };
            });

            var apiStatus = new TwitterApiStatus();
            MyCommon.TwitterApiInfo = apiStatus;

            var request = new GetRequest
            {
                RequestUri = new("hoge/tetete.json", UriKind.Relative),
                EndpointName = "/hoge/tetete",
            };

            using var response = await apiConnection.SendAsync(request);

            Assert.Equal(TwitterApiAccessLevel.ReadWriteAndDirectMessage, apiStatus.AccessLevel);
            Assert.Equal(new ApiLimit(150, 100, new DateTimeUtc(2013, 1, 1, 0, 0, 0)), apiStatus.AccessLimit["/hoge/tetete"]);

            Assert.Equal(0, mockHandler.QueueCount);
        }

        [Fact]
        public async Task SendAsync_ErrorStatusTest()
        {
            using var mockHandler = new HttpMessageHandlerMock();
            using var http = new HttpClient(mockHandler);
            using var apiConnection = new TwitterApiConnection();
            apiConnection.Http = http;

            mockHandler.Enqueue(x =>
            {
                return new HttpResponseMessage(HttpStatusCode.BadGateway)
                {
                    Content = new StringContent("### Invalid JSON Response ###"),
                };
            });

            var request = new GetRequest
            {
                RequestUri = new("hoge/tetete.json", UriKind.Relative),
            };

            var exception = await Assert.ThrowsAsync<TwitterApiException>(
                () => apiConnection.SendAsync(request)
            );

            // エラーレスポンスの読み込みに失敗した場合はステータスコードをそのままメッセージに使用する
            Assert.Equal("BadGateway", exception.Message);
            Assert.Null(exception.ErrorResponse);

            Assert.Equal(0, mockHandler.QueueCount);
        }

        [Fact]
        public async Task SendAsync_ErrorJsonTest()
        {
            using var mockHandler = new HttpMessageHandlerMock();
            using var http = new HttpClient(mockHandler);
            using var apiConnection = new TwitterApiConnection();
            apiConnection.Http = http;

            mockHandler.Enqueue(x =>
            {
                return new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("""{"errors":[{"code":187,"message":"Status is a duplicate."}]}"""),
                };
            });

            var request = new GetRequest
            {
                RequestUri = new("hoge/tetete.json", UriKind.Relative),
            };

            var exception = await Assert.ThrowsAsync<TwitterApiException>(
                () => apiConnection.SendAsync(request)
            );

            // エラーレスポンスの JSON に含まれるエラーコードに基づいてメッセージを出力する
            Assert.Equal("DuplicateStatus", exception.Message);

            Assert.Equal(TwitterErrorCode.DuplicateStatus, exception.Errors[0].Code);
            Assert.Equal("Status is a duplicate.", exception.Errors[0].Message);

            Assert.Equal(0, mockHandler.QueueCount);
        }

        [Fact]
        public async Task HandleTimeout_SuccessTest()
        {
            static async Task<int> AsyncFunc(CancellationToken token)
            {
                await Task.Delay(10);
                token.ThrowIfCancellationRequested();
                return 1;
            }

            var timeout = TimeSpan.FromMilliseconds(200);
            var ret = await TwitterApiConnection.HandleTimeout(AsyncFunc, timeout);

            Assert.Equal(1, ret);
        }

        [Fact]
        public async Task HandleTimeout_TimeoutTest()
        {
            var tcs = new TaskCompletionSource<bool>();

            async Task<int> AsyncFunc(CancellationToken token)
            {
                await Task.Delay(200);
                tcs.SetResult(token.IsCancellationRequested);
                return 1;
            }

            var timeout = TimeSpan.FromMilliseconds(10);
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => TwitterApiConnection.HandleTimeout(AsyncFunc, timeout)
            );

            var cancelRequested = await tcs.Task;
            Assert.True(cancelRequested);
        }

        [Fact]
        public async Task HandleTimeout_ThrowExceptionAfterTimeoutTest()
        {
            var tcs = new TaskCompletionSource<int>();

            async Task<int> AsyncFunc(CancellationToken token)
            {
                await Task.Delay(100);
                tcs.SetResult(1);
                throw new Exception();
            }

            var timeout = TimeSpan.FromMilliseconds(10);
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => TwitterApiConnection.HandleTimeout(AsyncFunc, timeout)
            );

            // キャンセル後に AsyncFunc で発生した例外が無視される（UnobservedTaskException イベントを発生させない）ことをチェックする
            var error = false;
            void UnobservedExceptionHandler(object s, UnobservedTaskExceptionEventArgs e)
                => error = true;

            TaskScheduler.UnobservedTaskException += UnobservedExceptionHandler;

            await tcs.Task;
            await Task.Delay(10);
            GC.Collect(); // UnobservedTaskException は Task のデストラクタで呼ばれるため強制的に GC を実行する
            await Task.Delay(10);

            Assert.False(error);

            TaskScheduler.UnobservedTaskException -= UnobservedExceptionHandler;
        }
    }
}
