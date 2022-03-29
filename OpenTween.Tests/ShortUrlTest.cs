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
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
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

namespace OpenTween
{
    public class ShortUrlTest
    {
        [Fact]
        public async Task ExpandUrlAsync_Test()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            // https://t.co/hoge1 -> http://example.com/hoge2
            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("https://t.co/hoge1"), x.RequestUri);

                return this.CreateRedirectResponse("http://example.com/hoge2");
            });

            Assert.Equal(new Uri("http://example.com/hoge2"),
                await shortUrl.ExpandUrlAsync(new Uri("https://t.co/hoge1")));

            Assert.Equal(0, handler.QueueCount);
        }

        [Fact]
        public async Task ExpandUrlAsync_IrregularUrlTest()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            // https://www.flickr.com/photo.gne?short=hoge -> /photos/foo/11111/
            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("https://www.flickr.com/photo.gne?short=hoge"), x.RequestUri);

                return this.CreateRedirectResponse("/photos/foo/11111/", UriKind.Relative);
            });

            Assert.Equal(new Uri("https://www.flickr.com/photos/foo/11111/"),
                await shortUrl.ExpandUrlAsync(new Uri("https://www.flickr.com/photo.gne?short=hoge")));

            Assert.Equal(0, handler.QueueCount);
        }

        [Fact]
        public async Task ExpandUrlAsync_DisableExpandingTest()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            shortUrl.DisableExpanding = true;

            // https://t.co/hoge1 -> http://example.com/hoge2
            handler.Enqueue(x =>
            {
                // このリクエストは実行されないはず
                Assert.True(false);
                return this.CreateRedirectResponse("http://example.com/hoge2");
            });

            Assert.Equal(new Uri("https://t.co/hoge1"),
                await shortUrl.ExpandUrlAsync(new Uri("https://t.co/hoge1")));

            Assert.Equal(1, handler.QueueCount);
        }

        [Fact]
        public async Task ExpandUrlAsync_RecursiveTest()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            // https://t.co/hoge1 -> https://bit.ly/hoge2
            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("https://t.co/hoge1"), x.RequestUri);

                return this.CreateRedirectResponse("https://bit.ly/hoge2");
            });

            // https://bit.ly/hoge2 -> http://example.com/hoge3
            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("https://bit.ly/hoge2"), x.RequestUri);

                return this.CreateRedirectResponse("http://example.com/hoge3");
            });

            Assert.Equal(new Uri("http://example.com/hoge3"),
                await shortUrl.ExpandUrlAsync(new Uri("https://t.co/hoge1")));

            Assert.Equal(0, handler.QueueCount);
        }

        [Fact]
        public async Task ExpandUrlAsync_RecursiveLimitTest()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            // https://t.co/hoge1 -> https://bit.ly/hoge2
            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("https://t.co/hoge1"), x.RequestUri);

                return this.CreateRedirectResponse("https://bit.ly/hoge2");
            });

            // https://bit.ly/hoge2 -> https://tinyurl.com/hoge3
            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("https://bit.ly/hoge2"), x.RequestUri);

                return this.CreateRedirectResponse("https://tinyurl.com/hoge3");
            });

            // https://tinyurl.com/hoge3 -> http://example.com/hoge4
            handler.Enqueue(x =>
            {
                // このリクエストは実行されないはず
                Assert.True(false);
                return this.CreateRedirectResponse("http://example.com/hoge4");
            });

            Assert.Equal(new Uri("https://tinyurl.com/hoge3"),
                await shortUrl.ExpandUrlAsync(new Uri("https://t.co/hoge1"), redirectLimit: 2));

            Assert.Equal(1, handler.QueueCount);
        }

        [Fact]
        public async Task ExpandUrlAsync_UpgradeToHttpsTest()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            // http://t.co/hoge -> http://example.com/hoge
            handler.Enqueue(x =>
            {
                // https:// に変換されてリクエストが送信される
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("https://t.co/hoge"), x.RequestUri);

                return this.CreateRedirectResponse("http://example.com/hoge");
            });

            Assert.Equal(new Uri("http://example.com/hoge"),
                await shortUrl.ExpandUrlAsync(new Uri("http://t.co/hoge")));

            Assert.Equal(0, handler.QueueCount);
        }

        [Fact]
        public async Task ExpandUrlAsync_InsecureDomainTest()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            // http://tinami.jp/hoge -> http://example.com/hoge
            handler.Enqueue(x =>
            {
                // HTTPS非対応のドメインは http:// のままリクエストが送信される
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("http://tinami.jp/hoge"), x.RequestUri);

                return this.CreateRedirectResponse("http://example.com/hoge");
            });

            Assert.Equal(new Uri("http://example.com/hoge"),
                await shortUrl.ExpandUrlAsync(new Uri("http://tinami.jp/hoge")));

            Assert.Equal(0, handler.QueueCount);
        }

        [Fact]
        public async Task ExpandUrlAsync_RelativeUriTest()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            handler.Enqueue(x =>
            {
                // このリクエストは実行されないはず
                Assert.True(false);
                return this.CreateRedirectResponse("");
            });

            // 相対 URI に対しては何も行わない
            Assert.Equal(new Uri("./foo/bar", UriKind.Relative),
                await shortUrl.ExpandUrlAsync(new Uri("./foo/bar", UriKind.Relative)));

            Assert.Equal(1, handler.QueueCount);
        }

        [Fact]
        public async Task ExpandUrlAsync_RelativeRedirectTest()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            // Location に相対 URL を指定したリダイレクト (テストに使う URL は適当)
            // https://t.co/hogehoge -> /tetetete
            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("https://t.co/hogehoge"), x.RequestUri);

                return this.CreateRedirectResponse("/tetetete", UriKind.Relative);
            });

            // https://t.co/tetetete -> http://example.com/tetetete
            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("https://t.co/tetetete"), x.RequestUri);

                return this.CreateRedirectResponse("http://example.com/tetetete");
            });

            Assert.Equal(new Uri("http://example.com/tetetete"),
                await shortUrl.ExpandUrlAsync(new Uri("https://t.co/hogehoge")));

            Assert.Equal(0, handler.QueueCount);
        }

        [Fact]
        public async Task ExpandUrlAsync_String_Test()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            // https://t.co/hoge1 -> http://example.com/hoge2
            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("https://t.co/hoge1"), x.RequestUri);

                return this.CreateRedirectResponse("http://example.com/hoge2");
            });

            Assert.Equal("http://example.com/hoge2",
                await shortUrl.ExpandUrlAsync("https://t.co/hoge1"));

            Assert.Equal(0, handler.QueueCount);
        }

        [Fact]
        public async Task ExpandUrlAsync_String_SchemeLessUrlTest()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            // https://t.co/hoge1 -> http://example.com/hoge2
            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("https://t.co/hoge1"), x.RequestUri);

                return this.CreateRedirectResponse("http://example.com/hoge2");
            });

            // スキームが省略されたURL
            Assert.Equal("http://example.com/hoge2",
                await shortUrl.ExpandUrlAsync("t.co/hoge1"));

            Assert.Equal(0, handler.QueueCount);
        }

        [Fact]
        public async Task ExpandUrlAsync_String_InvalidUrlTest()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            handler.Enqueue(x =>
            {
                // リクエストは送信されないはず
                Assert.True(false);
                return this.CreateRedirectResponse("http://example.com/hoge2");
            });

            // 不正なURL
            Assert.Equal("..hogehoge..", await shortUrl.ExpandUrlAsync("..hogehoge.."));

            Assert.Equal(1, handler.QueueCount);
        }

        [Fact]
        public async Task ExpandUrlAsync_HttpErrorTest()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            // https://t.co/hoge1 -> 503 Service Unavailable
            handler.Enqueue(x =>
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            });

            Assert.Equal(new Uri("https://t.co/hoge1"),
                await shortUrl.ExpandUrlAsync(new Uri("https://t.co/hoge1")));

            Assert.Equal(0, handler.QueueCount);
        }

        [Fact]
        public async Task ExpandUrlHtmlAsync_Test()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            // https://t.co/hoge1 -> http://example.com/hoge2
            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Head, x.Method);
                Assert.Equal(new Uri("https://t.co/hoge1"), x.RequestUri);

                return this.CreateRedirectResponse("http://example.com/hoge2");
            });

            Assert.Equal("<a href=\"http://example.com/hoge2\">hogehoge</a>",
                await shortUrl.ExpandUrlHtmlAsync("<a href=\"https://t.co/hoge1\">hogehoge</a>"));

            Assert.Equal(0, handler.QueueCount);
        }

        [Fact]
        public async Task ExpandUrlHtmlAsync_RelativeUriTest()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            handler.Enqueue(x =>
            {
                // リクエストは送信されないはず
                Assert.True(false);
                return this.CreateRedirectResponse("http://example.com/hoge");
            });

            Assert.Equal("<a href=\"./hoge\">hogehoge</a>",
                await shortUrl.ExpandUrlHtmlAsync("<a href=\"./hoge\">hogehoge</a>"));

            Assert.Equal(1, handler.QueueCount);
        }

        private HttpResponseMessage CreateRedirectResponse(string uriStr)
            => this.CreateRedirectResponse(uriStr, UriKind.Absolute);

        private HttpResponseMessage CreateRedirectResponse(string uriStr, UriKind uriKind)
        {
            var response = new HttpResponseMessage(HttpStatusCode.TemporaryRedirect);
            response.Headers.Location = new Uri(uriStr, uriKind);
            return response;
        }

        [Fact]
        public async Task ShortenUrlAsync_TinyUrlTest()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            handler.Enqueue(async x =>
            {
                Assert.Equal(HttpMethod.Post, x.Method);
                Assert.Equal(new Uri("https://tinyurl.com/api-create.php"), x.RequestUri);
                Assert.Equal("url=http%3A%2F%2Fexample.com%2Fhogehogehoge", await x.Content.ReadAsStringAsync());

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(Encoding.UTF8.GetBytes("http://tinyurl.com/hoge")),
                };
            });

            Assert.Equal(new Uri("https://tinyurl.com/hoge"),
                await shortUrl.ShortenUrlAsync(MyCommon.UrlConverter.TinyUrl, new Uri("http://example.com/hogehogehoge")));

            Assert.Equal(0, handler.QueueCount);
        }

        [Fact]
        public async Task ShortenUrlAsync_UxnuUrlTest()
        {
            var handler = new HttpMessageHandlerMock();
            using var http = new HttpClient(handler);
            var shortUrl = new ShortUrl(http);

            handler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Get, x.Method);
                Assert.Equal("https://ux.nu/api/short?format=plain&url=http:%2F%2Fexample.com%2Fhogehoge",
                    x.RequestUri.AbsoluteUri);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(Encoding.UTF8.GetBytes("https://ux.nu/hoge")),
                };
            });

            Assert.Equal(new Uri("https://ux.nu/hoge"),
                await shortUrl.ShortenUrlAsync(MyCommon.UrlConverter.Uxnu, new Uri("http://example.com/hogehoge")));

            Assert.Equal(0, handler.QueueCount);
        }
    }
}
