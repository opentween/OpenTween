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

using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using OpenTween.Connection;
using Xunit;

namespace OpenTween.Thumbnail.Services
{
    public class TonTwitterComTest
    {
        [Fact]
        public async Task GetThumbnailInfoAsync_Test()
        {
            var mock = new Mock<IApiConnection>();
            TonTwitterCom.GetApiConnection = () => mock.Object;

            var service = new TonTwitterCom();
            var thumb = await service.GetThumbnailInfoAsync(
                "https://ton.twitter.com/1.1/ton/data/dm/123456/123456/abcdef.jpg",
                new(),
                CancellationToken.None
            );

            Assert.NotNull(thumb!);
            Assert.Equal(
                "https://ton.twitter.com/1.1/ton/data/dm/123456/123456/abcdef.jpg:large",
                thumb.MediaPageUrl
            );
            Assert.Equal(
                "https://ton.twitter.com/1.1/ton/data/dm/123456/123456/abcdef.jpg:large",
                thumb.FullSizeImageUrl
            );
            Assert.Equal(
                "https://ton.twitter.com/1.1/ton/data/dm/123456/123456/abcdef.jpg",
                thumb.ThumbnailImageUrl
            );

            TonTwitterCom.GetApiConnection = null;
        }

        [Fact]
        public async Task GetThumbnailInfoAsync_ApiConnectionIsNotSetTest()
        {
            TonTwitterCom.GetApiConnection = null;

            var service = new TonTwitterCom();
            var thumb = await service.GetThumbnailInfoAsync(
                "https://ton.twitter.com/1.1/ton/data/dm/123456/123456/abcdef.jpg",
                new(),
                CancellationToken.None
            );

            Assert.Null(thumb);
        }

        [Fact]
        public async Task GetThumbnailInfoAsync_NotMatchedTest()
        {
            var mock = new Mock<IApiConnection>();
            TonTwitterCom.GetApiConnection = () => mock.Object;

            var service = new TonTwitterCom();
            var thumb = await service.GetThumbnailInfoAsync(
                "https://example.com/abcdef.jpg",
                new(),
                CancellationToken.None
            );

            Assert.Null(thumb);

            TonTwitterCom.GetApiConnection = null;
        }

        [Fact]
        public async Task LoadThumbnailImageAsync_Test()
        {
            using var image = TestUtils.CreateDummyImage();
            using var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(image.Stream.ToArray()),
            };
            using var response = new ApiResponse(responseMessage);

            var mock = new Mock<IApiConnection>();
            mock.Setup(
                    x => x.SendAsync(It.IsAny<IHttpRequest>())
                )
                .Callback<IHttpRequest>(x =>
                {
                    var request = Assert.IsType<GetRequest>(x);
                    Assert.Equal(
                        new("https://ton.twitter.com/1.1/ton/data/dm/123456/123456/abcdef.jpg"),
                        request.RequestUri
                    );
                })
                .ReturnsAsync(response);

            var apiConnection = mock.Object;
            var thumb = new TonTwitterCom.Thumbnail(apiConnection)
            {
                MediaPageUrl = "https://ton.twitter.com/1.1/ton/data/dm/123456/123456/abcdef.jpg:large",
                FullSizeImageUrl = "https://ton.twitter.com/1.1/ton/data/dm/123456/123456/abcdef.jpg:large",
                ThumbnailImageUrl = "https://ton.twitter.com/1.1/ton/data/dm/123456/123456/abcdef.jpg",
            };

            var result = await thumb.LoadThumbnailImageAsync(CancellationToken.None);
            Assert.Equal(image, result);

            mock.VerifyAll();
        }
    }
}
