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
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xunit;

namespace OpenTween.Api
{
    public class BitlyApiTest
    {
        [Fact]
        public async Task ShortenAsync_OAuth2Test()
        {
            using (var mockHandler = new HttpMessageHandlerMock())
            using (var http = new HttpClient(mockHandler))
            {
                var bitly = new BitlyApi(http);

                mockHandler.Enqueue(x =>
                {
                    Assert.Equal(HttpMethod.Get, x.Method);
                    Assert.Equal("https://api-ssl.bitly.com/v3/shorten",
                        x.RequestUri.GetLeftPart(UriPartial.Path));

                    var query = HttpUtility.ParseQueryString(x.RequestUri.Query);

                    Assert.Equal("http://www.example.com/", query["longUrl"]);
                    Assert.Equal("bit.ly", query["domain"]);
                    Assert.Equal("hogehoge", query["access_token"]);

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("http://bit.ly/foo"),
                    };
                });

                bitly.EndUserAccessToken = "hogehoge";

                var result = await bitly.ShortenAsync(new Uri("http://www.example.com/"), "bit.ly")
                    .ConfigureAwait(false);
                Assert.Equal("http://bit.ly/foo", result.OriginalString);

                Assert.Equal(0, mockHandler.QueueCount);
            }
        }

        [Fact]
        public async Task ShortenAsync_LegacyApiKeyTest()
        {
            using (var mockHandler = new HttpMessageHandlerMock())
            using (var http = new HttpClient(mockHandler))
            {
                var bitly = new BitlyApi(http);

                mockHandler.Enqueue(x =>
                {
                    Assert.Equal(HttpMethod.Get, x.Method);
                    Assert.Equal("https://api-ssl.bitly.com/v3/shorten",
                        x.RequestUri.GetLeftPart(UriPartial.Path));

                    var query = HttpUtility.ParseQueryString(x.RequestUri.Query);

                    Assert.Equal("http://www.example.com/", query["longUrl"]);
                    Assert.Equal("bit.ly", query["domain"]);
                    Assert.Equal("username", query["login"]);
                    Assert.Equal("hogehoge", query["apiKey"]);

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("http://bit.ly/foo"),
                    };
                });

                bitly.EndUserLoginName = "username";
                bitly.EndUserApiKey = "hogehoge";

                var result = await bitly.ShortenAsync(new Uri("http://www.example.com/"), "bit.ly")
                    .ConfigureAwait(false);
                Assert.Equal("http://bit.ly/foo", result.OriginalString);

                Assert.Equal(0, mockHandler.QueueCount);
            }
        }

        [Fact]
        public async Task GetAccessTokenAsync_Test()
        {
            using (var mockHandler = new HttpMessageHandlerMock())
            using (var http = new HttpClient(mockHandler))
            {
                var bitly = new BitlyApi(http);

                mockHandler.Enqueue(async x =>
                {
                    Assert.Equal(HttpMethod.Post, x.Method);
                    Assert.Equal("https://api-ssl.bitly.com/oauth/access_token",
                        x.RequestUri.GetLeftPart(UriPartial.Path));

                    Assert.Equal("Basic", x.Headers.Authorization.Scheme);
                    Assert.Equal(ApplicationSettings.BitlyClientId + ":" + ApplicationSettings.BitlyClientSecret,
                        Encoding.UTF8.GetString(Convert.FromBase64String(x.Headers.Authorization.Parameter)));

                    var body = await x.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);
                    var query = HttpUtility.ParseQueryString(body);

                    Assert.Equal("password", query["grant_type"]);
                    Assert.Equal("hogehoge", query["username"]);
                    Assert.Equal("tetete", query["password"]);

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"access_token\": \"abcdefg\"}"),
                    };
                });

                var result = await bitly.GetAccessTokenAsync("hogehoge", "tetete")
                    .ConfigureAwait(false);
                Assert.Equal("abcdefg", result);

                Assert.Equal(0, mockHandler.QueueCount);
            }
        }

        [Fact]
        public async Task GetAccessTokenAsync_ErrorResponseTest()
        {
            using (var mockHandler = new HttpMessageHandlerMock())
            using (var http = new HttpClient(mockHandler))
            {
                var bitly = new BitlyApi(http);

                mockHandler.Enqueue(x =>
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"status_code\": \"500\", \"status_txt\": \"MISSING_ARG_USERNAME\"}"),
                    };
                });

                await Assert.ThrowsAsync<WebApiException>(() => bitly.GetAccessTokenAsync("hogehoge", "tetete"))
                    .ConfigureAwait(false);

                Assert.Equal(0, mockHandler.QueueCount);
            }
        }
    }
}
