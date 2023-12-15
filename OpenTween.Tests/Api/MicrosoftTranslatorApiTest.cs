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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Moq;
using Xunit;

namespace OpenTween.Api
{
    public class MicrosoftTranslatorApiTest
    {
        [Fact]
        public async Task TranslateAsync_Test()
        {
            using var mockHandler = new HttpMessageHandlerMock();
            using var http = new HttpClient(mockHandler);

            var mock = new Mock<MicrosoftTranslatorApi>(ApiKey.Create("fake_api_key"), http);
            mock.Setup(x => x.GetAccessTokenAsync())
                .ReturnsAsync(("1234abcd", TimeSpan.FromSeconds(1000)));

            var translateApi = mock.Object;

            mockHandler.Enqueue(async x =>
            {
                Assert.Equal(HttpMethod.Post, x.Method);
                Assert.Equal(MicrosoftTranslatorApi.TranslateEndpoint.AbsoluteUri,
                    x.RequestUri.GetLeftPart(UriPartial.Path));

                var query = HttpUtility.ParseQueryString(x.RequestUri.Query);

                Assert.Equal("3.0", query["api-version"]);
                Assert.Equal("ja", query["to"]);
                Assert.Equal("en", query["from"]);

                var requestBody = await x.Content.ReadAsByteArrayAsync();

                using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(requestBody, XmlDictionaryReaderQuotas.Max))
                {
                    var xElm = XElement.Load(jsonReader);

                    var textElm = xElm.XPathSelectElement("/item/Text");
                    Assert.Equal("hogehoge", textElm.Value);
                }

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        [
                            {
                                "translations": [
                                    {
                                        "text": "ほげほげ",
                                        "to": "ja"
                                    }
                                ]
                            }
                        ]
                        """),
                };
            });

            var result = await translateApi.TranslateAsync("hogehoge", langTo: "ja", langFrom: "en");
            Assert.Equal("ほげほげ", result);

            mock.Verify(x => x.GetAccessTokenAsync(), Times.Once());
            Assert.Equal(0, mockHandler.QueueCount);
        }

        [Fact]
        public async Task TranslateAsync_ApiKeyErrorTest()
        {
            using var mockHandler = new HttpMessageHandlerMock();
            using var http = new HttpClient(mockHandler);

            var mock = new Mock<MicrosoftTranslatorApi>(ApiKey.Create("%e%INVALID_API_KEY"), http);
            var translateApi = mock.Object;

            await Assert.ThrowsAsync<WebApiException>(
                () => translateApi.TranslateAsync("hogehoge", langTo: "ja", langFrom: "en")
            );

            mock.Verify(x => x.GetAccessTokenAsync(), Times.Never());
            Assert.Equal(0, mockHandler.QueueCount);
        }

        [Fact]
        public async Task UpdateAccessTokenIfExpired_FirstCallTest()
        {
            var mock = new Mock<MicrosoftTranslatorApi>(ApiKey.Create("fake_api_key"), null!);
            mock.Setup(x => x.GetAccessTokenAsync())
                .ReturnsAsync(("1234abcd", TimeSpan.FromSeconds(1000)));

            var translateApi = mock.Object;

            await translateApi.UpdateAccessTokenIfExpired();

            Assert.Equal("1234abcd", translateApi.AccessToken);

            // 期待値との差が 3 秒以内であるか
            var expectedExpiresAt = DateTimeUtc.Now + TimeSpan.FromSeconds(1000 - 30);
            Assert.True((translateApi.RefreshAccessTokenAt - expectedExpiresAt).Duration() < TimeSpan.FromSeconds(3));
        }

        [Fact]
        public async Task UpdateAccessTokenIfExpired_NotExpiredTest()
        {
            var mock = new Mock<MicrosoftTranslatorApi>(ApiKey.Create("fake_api_key"), null!);

            var translateApi = mock.Object;
            translateApi.AccessToken = "1234abcd";
            translateApi.RefreshAccessTokenAt = DateTimeUtc.Now + TimeSpan.FromMinutes(3);

            await translateApi.UpdateAccessTokenIfExpired();

            // RefreshAccessTokenAt の時刻を過ぎるまでは GetAccessTokenAsync は呼ばれない
            mock.Verify(x => x.GetAccessTokenAsync(), Times.Never());
        }

        [Fact]
        public async Task UpdateAccessTokenIfExpired_ExpiredTest()
        {
            var mock = new Mock<MicrosoftTranslatorApi>(ApiKey.Create("fake_api_key"), null!);
            mock.Setup(x => x.GetAccessTokenAsync())
                .ReturnsAsync(("5678efgh", TimeSpan.FromSeconds(1000)));

            var translateApi = mock.Object;
            translateApi.AccessToken = "1234abcd";
            translateApi.RefreshAccessTokenAt = DateTimeUtc.Now - TimeSpan.FromMinutes(3);

            await translateApi.UpdateAccessTokenIfExpired();

            Assert.Equal("5678efgh", translateApi.AccessToken);

            // 期待値との差が 3 秒以内であるか
            var expectedExpiresAt = DateTimeUtc.Now + TimeSpan.FromSeconds(1000 - 30);
            Assert.True((translateApi.RefreshAccessTokenAt - expectedExpiresAt).Duration() < TimeSpan.FromSeconds(3));
        }

        [Fact]
        public async Task GetAccessTokenAsync_Test()
        {
            using var mockHandler = new HttpMessageHandlerMock();
            using var http = new HttpClient(mockHandler);
            var translateApi = new MicrosoftTranslatorApi(ApiKey.Create("fake_api_key"), http);

            mockHandler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Post, x.Method);
                Assert.Equal(MicrosoftTranslatorApi.IssueTokenEndpoint, x.RequestUri);

                var keyHeader = x.Headers.First(y => y.Key == "Ocp-Apim-Subscription-Key");
                Assert.Equal("fake_api_key", keyHeader.Value.Single());

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(@"ACCESS_TOKEN"),
                };
            });

            var result = await translateApi.GetAccessTokenAsync();

            var expectedToken = (@"ACCESS_TOKEN", TimeSpan.FromMinutes(10));
            Assert.Equal(expectedToken, result);

            Assert.Equal(0, mockHandler.QueueCount);
        }
    }
}
