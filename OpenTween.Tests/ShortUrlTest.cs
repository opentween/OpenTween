// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details.
//
// You should have received a copy of the GNU General public License along
// with this program. if (not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

#pragma warning disable 1998 // awaitが無いasyncラムダ式に対する警告を抑制

namespace OpenTween
{
    public class ShortUrlTest
    {
        [Fact]
        public async Task ExpandUrlAsync_Test()
        {
            var handler = new HttpMessageHandlerMock();
            var shortUrl = new ShortUrl(new HttpClient(handler));

            // http://t.co/hoge1 -> http://example.com/hoge2
            handler.Queue.Enqueue(async x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("http://t.co/hoge1"), x.RequestUri);

                return this.CreateRedirectResponse("http://example.com/hoge2");
            });

            Assert.Equal(new Uri("http://example.com/hoge2"),
                await shortUrl.ExpandUrlAsync(new Uri("http://t.co/hoge1")));
        }

        [Fact]
        public async Task ExpandUrlAsync_DisableExpandingTest()
        {
            var handler = new HttpMessageHandlerMock();
            var shortUrl = new ShortUrl(new HttpClient(handler));

            shortUrl.DisableExpanding = true;

            // http://t.co/hoge1 -> http://example.com/hoge2
            handler.Queue.Enqueue(async x =>
            {
                // このリクエストは実行されないはず
                Assert.True(false);
                return this.CreateRedirectResponse("http://example.com/hoge2");
            });

            Assert.Equal(new Uri("http://t.co/hoge1"),
                await shortUrl.ExpandUrlAsync(new Uri("http://t.co/hoge1")));
        }

        [Fact]
        public async Task ExpandUrlAsync_RecursiveTest()
        {
            var handler = new HttpMessageHandlerMock();
            var shortUrl = new ShortUrl(new HttpClient(handler));

            // http://t.co/hoge1 -> http://bit.ly/hoge2
            handler.Queue.Enqueue(async x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("http://t.co/hoge1"), x.RequestUri);

                return this.CreateRedirectResponse("http://bit.ly/hoge2");
            });

            // http://bit.ly/hoge2 -> http://example.com/hoge3
            handler.Queue.Enqueue(async x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("http://bit.ly/hoge2"), x.RequestUri);

                return this.CreateRedirectResponse("http://example.com/hoge3");
            });

            Assert.Equal(new Uri("http://example.com/hoge3"),
                await shortUrl.ExpandUrlAsync(new Uri("http://t.co/hoge1")));
        }

        [Fact]
        public async Task ExpandUrlAsync_RecursiveLimitTest()
        {
            var handler = new HttpMessageHandlerMock();
            var shortUrl = new ShortUrl(new HttpClient(handler));

            // http://t.co/hoge1 -> http://bit.ly/hoge2
            handler.Queue.Enqueue(async x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("http://t.co/hoge1"), x.RequestUri);

                return this.CreateRedirectResponse("http://bit.ly/hoge2");
            });

            // http://bit.ly/hoge2 -> http://tinyurl.com/hoge3
            handler.Queue.Enqueue(async x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("http://bit.ly/hoge2"), x.RequestUri);

                return this.CreateRedirectResponse("http://tinyurl.com/hoge3");
            });

            // http://tinyurl.com/hoge3 -> http://example.com/hoge4
            handler.Queue.Enqueue(async x =>
            {
                // このリクエストは実行されないはず
                Assert.True(false);
                return this.CreateRedirectResponse("http://example.com/hoge4");
            });

            Assert.Equal(new Uri("http://tinyurl.com/hoge3"),
                await shortUrl.ExpandUrlAsync(new Uri("http://t.co/hoge1"), redirectLimit: 2));
        }

        [Fact]
        public async Task ExpandUrlStrAsync_Test()
        {
            var handler = new HttpMessageHandlerMock();
            var shortUrl = new ShortUrl(new HttpClient(handler));

            // http://t.co/hoge1 -> http://example.com/hoge2
            handler.Queue.Enqueue(async x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("http://t.co/hoge1"), x.RequestUri);

                return this.CreateRedirectResponse("http://example.com/hoge2");
            });

            Assert.Equal("http://example.com/hoge2",
                await shortUrl.ExpandUrlStrAsync("http://t.co/hoge1"));
        }

        [Fact]
        public async Task ExpandUrlStrAsync_SchemeLessUrlTest()
        {
            var handler = new HttpMessageHandlerMock();
            var shortUrl = new ShortUrl(new HttpClient(handler));

            // http://t.co/hoge1 -> http://example.com/hoge2
            handler.Queue.Enqueue(async x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("http://t.co/hoge1"), x.RequestUri);

                return this.CreateRedirectResponse("http://example.com/hoge2");
            });

            // スキームが省略されたURL
            Assert.Equal("http://example.com/hoge2",
                await shortUrl.ExpandUrlStrAsync("t.co/hoge1"));
        }

        [Fact]
        public async Task ExpandUrlStrAsync_InvalidUrlTest()
        {
            var handler = new HttpMessageHandlerMock();
            var shortUrl = new ShortUrl(new HttpClient(handler));

            handler.Queue.Enqueue(async x =>
            {
                // リクエストは送信されないはず
                Assert.True(false);
                return this.CreateRedirectResponse("http://example.com/hoge2");
            });

            // 不正なURL
            Assert.Equal("..hogehoge..", await shortUrl.ExpandUrlStrAsync("..hogehoge.."));
        }

        [Fact]
        public async Task ExpandUrlAsync_HttpErrorTest()
        {
            var handler = new HttpMessageHandlerMock();
            var shortUrl = new ShortUrl(new HttpClient(handler));

            // http://t.co/hoge1 -> 503 Service Unavailable
            handler.Queue.Enqueue(async x =>
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            });

            Assert.Equal(new Uri("http://t.co/hoge1"),
                await shortUrl.ExpandUrlAsync(new Uri("http://t.co/hoge1")));
        }

        [Fact]
        public async Task ExpandUrlHtmlAsync_Test()
        {
            var handler = new HttpMessageHandlerMock();
            var shortUrl = new ShortUrl(new HttpClient(handler));

            // http://t.co/hoge1 -> http://example.com/hoge2
            handler.Queue.Enqueue(async x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("http://t.co/hoge1"), x.RequestUri);

                return this.CreateRedirectResponse("http://example.com/hoge2");
            });

            Assert.Equal("<a href=\"http://example.com/hoge2\">hogehoge</a>",
                await shortUrl.ExpandUrlHtmlAsync("<a href=\"http://t.co/hoge1\">hogehoge</a>"));
        }

        private HttpResponseMessage CreateRedirectResponse(string uriStr)
        {
            var response = new HttpResponseMessage(HttpStatusCode.TemporaryRedirect);
            response.Headers.Location = new Uri(uriStr);
            return response;
        }

        [Fact]
        public async Task ShortenUrlAsync_TinyUrlTest()
        {
            var handler = new HttpMessageHandlerMock();
            var shortUrl = new ShortUrl(new HttpClient(handler));

            handler.Queue.Enqueue(async x =>
            {
                Assert.Equal(HttpMethod.Post, x.Method);
                Assert.Equal(new Uri("http://tinyurl.com/api-create.php"), x.RequestUri);
                Assert.Equal("url=http%3A%2F%2Fexample.com%2Fhogehoge", await x.Content.ReadAsStringAsync());

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(Encoding.UTF8.GetBytes("http://tinyurl.com/hoge")),
                };
            });

            Assert.Equal(new Uri("http://tinyurl.com/hoge"),
                await shortUrl.ShortenUrlAsync(MyCommon.UrlConverter.TinyUrl, new Uri("http://example.com/hogehoge")));
        }

        [Fact]
        public async Task ShortenUrlAsync_UxnuUrlTest()
        {
            var handler = new HttpMessageHandlerMock();
            var shortUrl = new ShortUrl(new HttpClient(handler));

            handler.Queue.Enqueue(async x =>
            {
                Assert.Equal(HttpMethod.Get, x.Method);
                Assert.Equal("http://ux.nu/api/short?format=plain&url=http://example.com/hogehoge",
                    x.RequestUri.ToString());

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(Encoding.UTF8.GetBytes("http://ux.nu/hoge")),
                };
            });

            Assert.Equal(new Uri("http://ux.nu/hoge"),
                await shortUrl.ShortenUrlAsync(MyCommon.UrlConverter.Uxnu, new Uri("http://example.com/hogehoge")));
        }
    }
}
