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
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Moq;
using Xunit;

namespace OpenTween.Api
{
    public class MicrosoftTranslatorApiTest
    {
        [Fact]
        public async Task TranslateAsync_Test()
        {
            using (var mockHandler = new HttpMessageHandlerMock())
            using (var http = new HttpClient(mockHandler))
            {
                var mock = new Mock<MicrosoftTranslatorApi>(http);
                mock.Setup(x => x.GetAccessTokenAsync())
                    .ReturnsAsync(("1234abcd", TimeSpan.FromSeconds(1000)));

                var translateApi = mock.Object;

                mockHandler.Enqueue(x =>
                {
                    Assert.Equal(HttpMethod.Get, x.Method);
                    Assert.Equal(MicrosoftTranslatorApi.TranslateEndpoint.AbsoluteUri,
                        x.RequestUri.GetLeftPart(UriPartial.Path));

                    var query = HttpUtility.ParseQueryString(x.RequestUri.Query);

                    Assert.Equal("hogehoge", query["text"]);
                    Assert.Equal("ja", query["to"]);
                    Assert.Equal("en", query["from"]);

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("ほげほげ"),
                    };
                });

                var result = await translateApi.TranslateAsync("hogehoge", langTo: "ja", langFrom: "en")
                    .ConfigureAwait(false);
                Assert.Equal("ほげほげ", result);

                mock.Verify(x => x.GetAccessTokenAsync(), Times.Once());
                Assert.Equal(0, mockHandler.QueueCount);
            }
        }

        [Fact]
        public async Task UpdateAccessTokenIfExpired_FirstCallTest()
        {
            var mock = new Mock<MicrosoftTranslatorApi>();
            mock.Setup(x => x.GetAccessTokenAsync())
                .ReturnsAsync(("1234abcd", TimeSpan.FromSeconds(1000)));

            var translateApi = mock.Object;

            await translateApi.UpdateAccessTokenIfExpired()
                .ConfigureAwait(false);

            Assert.Equal("1234abcd", translateApi.AccessToken);

            // 期待値との差が 3 秒以内であるか
            var expectedExpiresAt = DateTime.Now + TimeSpan.FromSeconds(1000 - 30);
            Assert.True((translateApi.RefreshAccessTokenAt - expectedExpiresAt).Duration() < TimeSpan.FromSeconds(3));
        }

        [Fact]
        public async Task UpdateAccessTokenIfExpired_NotExpiredTest()
        {
            var mock = new Mock<MicrosoftTranslatorApi>();

            var translateApi = mock.Object;
            translateApi.AccessToken = "1234abcd";
            translateApi.RefreshAccessTokenAt = DateTime.Now + TimeSpan.FromMinutes(3);

            await translateApi.UpdateAccessTokenIfExpired()
                .ConfigureAwait(false);

            // RefreshAccessTokenAt の時刻を過ぎるまでは GetAccessTokenAsync は呼ばれない
            mock.Verify(x => x.GetAccessTokenAsync(), Times.Never());
        }

        [Fact]
        public async Task UpdateAccessTokenIfExpired_ExpiredTest()
        {
            var mock = new Mock<MicrosoftTranslatorApi>();
            mock.Setup(x => x.GetAccessTokenAsync())
                .ReturnsAsync(("5678efgh", TimeSpan.FromSeconds(1000)));

            var translateApi = mock.Object;
            translateApi.AccessToken = "1234abcd";
            translateApi.RefreshAccessTokenAt = DateTime.Now - TimeSpan.FromMinutes(3);

            await translateApi.UpdateAccessTokenIfExpired()
                .ConfigureAwait(false);

            Assert.Equal("5678efgh", translateApi.AccessToken);

            // 期待値との差が 3 秒以内であるか
            var expectedExpiresAt = DateTime.Now + TimeSpan.FromSeconds(1000 - 30);
            Assert.True((translateApi.RefreshAccessTokenAt - expectedExpiresAt).Duration() < TimeSpan.FromSeconds(3));
        }

        [Fact]
        public async Task GetAccessTokenAsync_Test()
        {
            using (var mockHandler = new HttpMessageHandlerMock())
            using (var http = new HttpClient(mockHandler))
            {
                var translateApi = new MicrosoftTranslatorApi(http);

                mockHandler.Enqueue(x =>
                {
                    Assert.Equal(HttpMethod.Post, x.Method);
                    Assert.Equal(MicrosoftTranslatorApi.IssueTokenEndpoint, x.RequestUri);

                    var keyHeader = x.Headers.First(y => y.Key == "Ocp-Apim-Subscription-Key");
                    Assert.Equal(ApplicationSettings.TranslatorSubscriptionKey, keyHeader.Value.Single());

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(@"ACCESS_TOKEN"),
                    };
                });

                var result = await translateApi.GetAccessTokenAsync()
                    .ConfigureAwait(false);

                var expectedToken = (@"ACCESS_TOKEN", TimeSpan.FromMinutes(10));
                Assert.Equal(expectedToken, result);

                Assert.Equal(0, mockHandler.QueueCount);
            }
        }
    }
}
