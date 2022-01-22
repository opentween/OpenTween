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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Moq;
using OpenTween.Api;
using OpenTween.Api.DataModel;
using Xunit;

namespace OpenTween.MediaUploadServices
{
    public class ImgurTest
    {
        [Fact]
        public async Task UploadAsync_Test()
        {
            var mockApi = new Mock<IImgurApi>();
            var imgurApi = mockApi.Object;

            var imgur = new Imgur(imgurApi, TwitterConfiguration.DefaultConfiguration());
            using var mediaItem = TestUtils.CreateDummyMediaItem();

            mockApi.Setup((x) => x.UploadFileAsync(mediaItem, "てすと"))
                .ReturnsAsync("https://i.imgur.com/aaaaaaa.png");

            var param = new PostStatusParams
            {
                Text = "てすと",
            };
            await imgur.UploadAsync(new[] { mediaItem }, param);

            Assert.Equal("てすと https://i.imgur.com/aaaaaaa.png", param.Text);
        }

        [Fact]
        public async Task UploadAsync_ErrorResponseTest()
        {
            var mockApi = new Mock<IImgurApi>();
            var imgurApi = mockApi.Object;

            var imgur = new Imgur(imgurApi, TwitterConfiguration.DefaultConfiguration());
            using var mediaItem = TestUtils.CreateDummyMediaItem();

            mockApi.Setup((x) => x.UploadFileAsync(mediaItem, "てすと"))
                .Throws<WebApiException>();

            var param = new PostStatusParams
            {
                Text = "てすと",
            };
            await Assert.ThrowsAsync<WebApiException>(
                () => imgur.UploadAsync(new[] { mediaItem }, param)
            );
        }

        [Fact]
        public async Task UploadAsync_TimeoutTest()
        {
            var mockApi = new Mock<IImgurApi>();
            var imgurApi = mockApi.Object;

            var imgur = new Imgur(imgurApi, TwitterConfiguration.DefaultConfiguration());
            using var mediaItem = TestUtils.CreateDummyMediaItem();

            mockApi.Setup((x) => x.UploadFileAsync(mediaItem, "てすと"))
                .Throws<OperationCanceledException>();

            var param = new PostStatusParams
            {
                Text = "てすと",
            };
            await Assert.ThrowsAsync<WebApiException>(
                () => imgur.UploadAsync(new[] { mediaItem }, param)
            );
        }
    }
}
