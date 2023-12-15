// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using Xunit;

namespace OpenTween.Api
{
    public class MobypictureApiTest
    {
        [Fact]
        public async Task UploadFileAsync_Test()
        {
            using var mockHandler = new HttpMessageHandlerMock();
            using var http = new HttpClient(mockHandler);

            mockHandler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Post, x.Method);
                Assert.Equal(MobypictureApi.UploadEndpoint, x.RequestUri);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        <?xml version="1.0" encoding="utf-8"?>
                        <rsp>
                            <media>
                                <mediaurl>https://www.mobypicture.com/user/OpenTween/view/00000000</mediaurl>
                            </media>
                        </rsp>
                        """),
                };
            });

            var mobypictureApi = new MobypictureApi(ApiKey.Create("fake_api_key"), http);
            using var mediaItem = TestUtils.CreateDummyMediaItem();
            var uploadedUrl = await mobypictureApi.UploadFileAsync(mediaItem, "てすと");
            Assert.Equal("https://www.mobypicture.com/user/OpenTween/view/00000000", uploadedUrl);

            Assert.Equal(0, mockHandler.QueueCount);
        }

        [Fact]
        public async Task UploadFileAsync_InvalidResponseTest()
        {
            using var mockHandler = new HttpMessageHandlerMock();
            using var http = new HttpClient(mockHandler);

            mockHandler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Post, x.Method);
                Assert.Equal(MobypictureApi.UploadEndpoint, x.RequestUri);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("INVALID RESPONSE"),
                };
            });

            var mobypictureApi = new MobypictureApi(ApiKey.Create("fake_api_key"), http);
            using var mediaItem = TestUtils.CreateDummyMediaItem();
            await Assert.ThrowsAsync<WebApiException>(
                () => mobypictureApi.UploadFileAsync(mediaItem, "てすと")
            );

            Assert.Equal(0, mockHandler.QueueCount);
        }

        [Fact]
        public async Task UploadFileAsync_ApiKeyErrorTest()
        {
            using var mockHandler = new HttpMessageHandlerMock();
            using var http = new HttpClient(mockHandler);

            var mobypictureApi = new MobypictureApi(ApiKey.Create("%e%INVALID_API_KEY"), http);
            using var mediaItem = TestUtils.CreateDummyMediaItem();
            await Assert.ThrowsAsync<WebApiException>(
                () => mobypictureApi.UploadFileAsync(mediaItem, "てすと")
            );

            Assert.Equal(0, mockHandler.QueueCount);
        }
    }
}
