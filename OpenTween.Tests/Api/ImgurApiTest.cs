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
    public class ImgurApiTest
    {
        [Fact]
        public async Task UploadFileAsync_Test()
        {
            using var mockHandler = new HttpMessageHandlerMock();
            using var http = new HttpClient(mockHandler);

            mockHandler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Post, x.Method);
                Assert.Equal(ImgurApi.UploadEndpoint, x.RequestUri);

                Assert.Equal("Client-ID", x.Headers.Authorization.Scheme);
                Assert.Equal("fake_api_key", x.Headers.Authorization.Parameter);

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""
                        <?xml version="1.0" encoding="utf-8"?>
                        <data type="array" success="1" status="200">
                            <id>aaaaaaa</id>
                            <title>てすと</title>
                            <description/>
                            <datetime>1234567890</datetime>
                            <type>image/png</type>
                            <animated>false</animated>
                            <width>300</width>
                            <height>300</height>
                            <size>1000</size>
                            <views>0</views>
                            <bandwidth>0</bandwidth>
                            <vote/>
                            <favorite>false</favorite>
                            <nsfw/>
                            <section/>
                            <account_url/>
                            <account_id>0</account_id>
                            <is_ad>false</is_ad>
                            <in_most_viral>false</in_most_viral>
                            <has_sound>false</has_sound>
                            <tags/>
                            <ad_type>0</ad_type>
                            <ad_url/>
                            <edited>0</edited>
                            <in_gallery>false</in_gallery>
                            <deletehash>aaaaaaaaaaaaaaa</deletehash>
                            <name/>
                            <link>https://i.imgur.com/aaaaaaa.png</link>
                        </data>
                        """),
                };
            });

            var imgurApi = new ImgurApi(ApiKey.Create("fake_api_key"), http);
            using var mediaItem = TestUtils.CreateDummyMediaItem();
            var uploadedUrl = await imgurApi.UploadFileAsync(mediaItem, "てすと");
            Assert.Equal("https://i.imgur.com/aaaaaaa.png", uploadedUrl);

            Assert.Equal(0, mockHandler.QueueCount);
        }

        [Fact]
        public async Task UploadFileAsync_ErrorResponseTest()
        {
            using var mockHandler = new HttpMessageHandlerMock();
            using var http = new HttpClient(mockHandler);

            mockHandler.Enqueue(x =>
            {
                Assert.Equal(HttpMethod.Post, x.Method);
                Assert.Equal(ImgurApi.UploadEndpoint, x.RequestUri);

                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("""
                        <?xml version="1.0" encoding="utf-8"?>
                        <data type="array" success="0" status="400">
                            <error>No image data was sent to the upload api</error>
                            <request>/3/image.xml</request>
                            <method>POST</method>
                        </data>
                        """),
                };
            });

            var imgurApi = new ImgurApi(ApiKey.Create("fake_api_key"), http);
            using var mediaItem = TestUtils.CreateDummyMediaItem();
            await Assert.ThrowsAsync<WebApiException>(
                () => imgurApi.UploadFileAsync(mediaItem, "てすと")
            );

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
                Assert.Equal(ImgurApi.UploadEndpoint, x.RequestUri);

                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("INVALID RESPONSE"),
                };
            });

            var imgurApi = new ImgurApi(ApiKey.Create("fake_api_key"), http);
            using var mediaItem = TestUtils.CreateDummyMediaItem();
            await Assert.ThrowsAsync<WebApiException>(
                () => imgurApi.UploadFileAsync(mediaItem, "てすと")
            );

            Assert.Equal(0, mockHandler.QueueCount);
        }

        [Fact]
        public async Task UploadFileAsync_ApiKeyErrorTest()
        {
            using var mockHandler = new HttpMessageHandlerMock();
            using var http = new HttpClient(mockHandler);

            var imgurApi = new ImgurApi(ApiKey.Create("%e%INVALID_API_KEY"), http);
            using var mediaItem = TestUtils.CreateDummyMediaItem();
            await Assert.ThrowsAsync<WebApiException>(
                () => imgurApi.UploadFileAsync(mediaItem, "てすと")
            );

            Assert.Equal(0, mockHandler.QueueCount);
        }
    }
}
