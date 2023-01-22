// OpenTween - Client of Twitter
// Copyright (c) 2023 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Xunit;

namespace OpenTween.Connection
{
    public class TwitterComCookieHandlerTest
    {
        [Fact]
        public void ParseCookie_Test()
        {
            var cookie = "guest_id_marketing=hoge; guest_id_ads=hoge; personalization_id=\"hoge\"; guest_id=hoge; ct0=aaaaaaaaaa; kdt=hoge; twid=hoge; auth_token=bbbbbbbbbb; dnt=1";
            var innerHandler = Mock.Of<HttpMessageHandler>();
            using var handler = new TwitterComCookieHandler(innerHandler, cookie);
            Assert.Equal("aaaaaaaaaa", handler.CsrfToken);
            Assert.Equal("bbbbbbbbbb", handler.AuthToken);
        }

        internal interface IHttpMessageHandlerProtected
        {
            Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
        }

        [Fact]
        public async Task SendAsync_Test()
        {
            var mock = new Mock<HttpMessageHandler>();
            mock.Protected()
                .As<IHttpMessageHandlerProtected>()
                .Setup(
                    x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>())
                )
                .Returns((HttpRequestMessage req, CancellationToken ct) =>
                {
                    var headers = req.Headers;
                    Assert.Equal(new[] { "OAuth2Session" }, headers.GetValues("x-twitter-auth-type"));
                    Assert.Equal(new[] { "aaaaaaaaaa" }, headers.GetValues("x-csrf-token"));
                    Assert.Equal(new[] { "ct0=aaaaaaaaaa; auth_token=bbbbbbbbbb" }, headers.GetValues("cookie"));
                    Assert.Equal(new[] { "Bearer tetete" }, headers.GetValues("authorization"));

                    return Task.FromResult(new HttpResponseMessage());
                });

            using var handler = new TwitterComCookieHandler(mock.Object, ApiKey.Create("tetete"));
            handler.SetCookie("ct0=aaaaaaaaaa; auth_token=bbbbbbbbbb");

            using var http = new HttpClient(handler);
            await http.GetAsync("https://api.twitter.com/1.1/account/verify_credentials.json");
        }
    }
}
