// OpenTween - Client of Twitter
// Copyright (c) 2012 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using OpenTween.Models;
using OpenTween.Thumbnail;
using OpenTween.Thumbnail.Services;
using Xunit;

namespace OpenTween
{
    public class TweetThumbnailTest
    {
        private ThumbnailGenerator CreateThumbnailGenerator()
        {
            var imgAzyobuziNet = new ImgAzyobuziNet(autoupdate: false);
            var thumbGenerator = new ThumbnailGenerator(imgAzyobuziNet);
            thumbGenerator.Services.Clear();
            return thumbGenerator;
        }

        private IThumbnailService CreateThumbnailService()
        {
            var thumbnailServiceMock = new Mock<IThumbnailService>();
            thumbnailServiceMock
                .Setup(
                    x => x.GetThumbnailInfoAsync("http://example.com/abcd", It.IsAny<PostClass>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(new MockThumbnailInfo
                {
                    MediaPageUrl = "http://example.com/abcd",
                    ThumbnailImageUrl = "http://img.example.com/abcd.png",
                });
            thumbnailServiceMock
                .Setup(
                    x => x.GetThumbnailInfoAsync("http://example.com/efgh", It.IsAny<PostClass>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(new MockThumbnailInfo
                {
                    MediaPageUrl = "http://example.com/efgh",
                    ThumbnailImageUrl = "http://img.example.com/efgh.png",
                });
            return thumbnailServiceMock.Object;
        }

        [Fact]
        public async Task PrepareThumbnails_Test()
        {
            var thumbnailGenerator = this.CreateThumbnailGenerator();
            thumbnailGenerator.Services.Add(this.CreateThumbnailService());

            var tweetThumbnail = new TweetThumbnail();
            tweetThumbnail.Initialize(thumbnailGenerator);

            var post = new PostClass
            {
                StatusId = new TwitterStatusId("100"),
                Media = new() { new("http://example.com/abcd") },
            };

            await tweetThumbnail.PrepareThumbnails(post, CancellationToken.None)
                .ConfigureAwait(false);

            Assert.True(tweetThumbnail.ThumbnailAvailable);
            Assert.Single(tweetThumbnail.Thumbnails);
            Assert.Equal(0, tweetThumbnail.SelectedIndex);
            Assert.Equal("http://example.com/abcd", tweetThumbnail.CurrentThumbnail.MediaPageUrl);
            Assert.Equal("http://img.example.com/abcd.png", tweetThumbnail.CurrentThumbnail.ThumbnailImageUrl);
        }

        [Fact]
        public async Task PrepareThumbnails_NoThumbnailTest()
        {
            var thumbnailGenerator = this.CreateThumbnailGenerator();
            thumbnailGenerator.Services.Add(this.CreateThumbnailService());

            var tweetThumbnail = new TweetThumbnail();
            tweetThumbnail.Initialize(thumbnailGenerator);

            var post = new PostClass
            {
                StatusId = new TwitterStatusId("100"),
                Media = new() { new("http://hoge.example.com/") },
            };

            await tweetThumbnail.PrepareThumbnails(post, CancellationToken.None)
                .ConfigureAwait(false);

            Assert.False(tweetThumbnail.ThumbnailAvailable);
            Assert.Throws<InvalidOperationException>(() => tweetThumbnail.Thumbnails);
        }

        [Fact]
        public async Task PrepareThumbnails_CancelTest()
        {
            var thumbnailServiceMock = new Mock<IThumbnailService>();
            thumbnailServiceMock
                .Setup(
                    x => x.GetThumbnailInfoAsync("http://slow.example.com/abcd", It.IsAny<PostClass>(), It.IsAny<CancellationToken>())
                )
                .Returns(async () =>
                {
                    await Task.Delay(200);
                    return new MockThumbnailInfo();
                });

            var thumbnailGenerator = this.CreateThumbnailGenerator();
            thumbnailGenerator.Services.Add(thumbnailServiceMock.Object);

            var tweetThumbnail = new TweetThumbnail();
            tweetThumbnail.Initialize(thumbnailGenerator);

            var post = new PostClass
            {
                StatusId = new TwitterStatusId("100"),
                Media = new() { new("http://slow.example.com/abcd") },
            };

            using var tokenSource = new CancellationTokenSource();
            var task = tweetThumbnail.PrepareThumbnails(post, tokenSource.Token);
            tokenSource.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task);
            Assert.True(task.IsCanceled);
        }

        [Fact]
        public async Task LoadSelectedThumbnail_Test()
        {
            using var image = TestUtils.CreateDummyImage();
            var thumbnailInfoMock = new Mock<ThumbnailInfo>() { CallBase = true };
            thumbnailInfoMock
                .Setup(
                    x => x.LoadThumbnailImageAsync(It.IsAny<HttpClient>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(image);

            var thumbnailServiceMock = new Mock<IThumbnailService>();
            thumbnailServiceMock
                .Setup(
                    x => x.GetThumbnailInfoAsync("http://example.com/abcd", It.IsAny<PostClass>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(thumbnailInfoMock.Object);

            var thumbnailGenerator = this.CreateThumbnailGenerator();
            thumbnailGenerator.Services.Add(thumbnailServiceMock.Object);

            var tweetThumbnail = new TweetThumbnail();
            tweetThumbnail.Initialize(thumbnailGenerator);

            var post = new PostClass
            {
                StatusId = new TwitterStatusId("100"),
                Media = new() { new("http://example.com/abcd") },
            };

            await tweetThumbnail.PrepareThumbnails(post, CancellationToken.None);

            var loadedImage = await tweetThumbnail.LoadSelectedThumbnail();
            Assert.Same(image, loadedImage);
        }

        [Fact]
        public async Task LoadSelectedThumbnail_RequestCollapsingTest()
        {
            var tsc = new TaskCompletionSource<MemoryImage>();
            var thumbnailInfoMock = new Mock<ThumbnailInfo>() { CallBase = true };
            thumbnailInfoMock
                .Setup(
                    x => x.LoadThumbnailImageAsync(It.IsAny<HttpClient>(), It.IsAny<CancellationToken>())
                )
                .Returns(tsc.Task);

            var thumbnailServiceMock = new Mock<IThumbnailService>();
            thumbnailServiceMock
                .Setup(
                    x => x.GetThumbnailInfoAsync("http://example.com/abcd", It.IsAny<PostClass>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(thumbnailInfoMock.Object);

            var thumbnailGenerator = this.CreateThumbnailGenerator();
            thumbnailGenerator.Services.Add(thumbnailServiceMock.Object);

            var tweetThumbnail = new TweetThumbnail();
            tweetThumbnail.Initialize(thumbnailGenerator);

            var post = new PostClass
            {
                StatusId = new TwitterStatusId("100"),
                Media = new() { new("http://example.com/abcd") },
            };

            await tweetThumbnail.PrepareThumbnails(post, CancellationToken.None);

            var loadTask1 = tweetThumbnail.LoadSelectedThumbnail();
            await Task.Delay(50);

            // 画像のロードが完了しない間に再度 LoadSelectedThumbnail が呼ばれた場合は同一の Task を返す
            // （複数回呼ばれても画像のリクエストは一本にまとめられる）
            var loadTask2 = tweetThumbnail.LoadSelectedThumbnail();
            Assert.Same(loadTask1, loadTask2);

            using var image = TestUtils.CreateDummyImage();
            tsc.SetResult(image);

            Assert.Same(image, await loadTask1);
        }

        [Fact]
        public async Task SelectedIndex_Test()
        {
            var thumbnailGenerator = this.CreateThumbnailGenerator();
            thumbnailGenerator.Services.Add(this.CreateThumbnailService());

            var tweetThumbnail = new TweetThumbnail();
            tweetThumbnail.Initialize(thumbnailGenerator);

            var post = new PostClass
            {
                StatusId = new TwitterStatusId("100"),
                Media = new() { new("http://example.com/abcd"), new("http://example.com/efgh") },
            };

            await tweetThumbnail.PrepareThumbnails(post, CancellationToken.None)
                .ConfigureAwait(false);

            Assert.Equal(2, tweetThumbnail.Thumbnails.Length);
            Assert.Equal(0, tweetThumbnail.SelectedIndex);
            Assert.Equal("http://example.com/abcd", tweetThumbnail.CurrentThumbnail.MediaPageUrl);

            tweetThumbnail.SelectedIndex = 1;
            Assert.Equal(1, tweetThumbnail.SelectedIndex);
            Assert.Equal("http://example.com/efgh", tweetThumbnail.CurrentThumbnail.MediaPageUrl);

            Assert.Throws<ArgumentOutOfRangeException>(() => tweetThumbnail.SelectedIndex = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => tweetThumbnail.SelectedIndex = 2);
        }

        [Fact]
        public void SelectedIndex_NoThumbnailTest()
        {
            var thumbnailGenerator = this.CreateThumbnailGenerator();
            var tweetThumbnail = new TweetThumbnail();
            tweetThumbnail.Initialize(thumbnailGenerator);

            Assert.False(tweetThumbnail.ThumbnailAvailable);

            // サムネイルが無い場合に 0 以外の値をセットすると例外を発生させる
            tweetThumbnail.SelectedIndex = 0;
            Assert.Throws<ArgumentOutOfRangeException>(() => tweetThumbnail.SelectedIndex = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => tweetThumbnail.SelectedIndex = 1);
        }

        [Fact]
        public async Task GetImageSearchUriGoogle_Test()
        {
            var thumbnailGenerator = this.CreateThumbnailGenerator();
            thumbnailGenerator.Services.Add(this.CreateThumbnailService());

            var tweetThumbnail = new TweetThumbnail();
            tweetThumbnail.Initialize(thumbnailGenerator);

            var post = new PostClass
            {
                StatusId = new TwitterStatusId("100"),
                Media = new() { new("http://example.com/abcd") },
            };

            await tweetThumbnail.PrepareThumbnails(post, CancellationToken.None)
                .ConfigureAwait(false);

            Assert.Equal("http://img.example.com/abcd.png", tweetThumbnail.CurrentThumbnail.ThumbnailImageUrl);
            Assert.Equal(
                new(@"https://lens.google.com/uploadbyurl?url=http%3A%2F%2Fimg.example.com%2Fabcd.png"),
                tweetThumbnail.GetImageSearchUriGoogle()
            );
        }

        [Fact]
        public async Task GetImageSearchUriSauceNao_Test()
        {
            var thumbnailGenerator = this.CreateThumbnailGenerator();
            thumbnailGenerator.Services.Add(this.CreateThumbnailService());

            var tweetThumbnail = new TweetThumbnail();
            tweetThumbnail.Initialize(thumbnailGenerator);

            var post = new PostClass
            {
                StatusId = new TwitterStatusId("100"),
                Media = new() { new("http://example.com/abcd") },
            };

            await tweetThumbnail.PrepareThumbnails(post, CancellationToken.None)
                .ConfigureAwait(false);

            Assert.Equal("http://img.example.com/abcd.png", tweetThumbnail.CurrentThumbnail.ThumbnailImageUrl);
            Assert.Equal(
                new(@"https://saucenao.com/search.php?url=http%3A%2F%2Fimg.example.com%2Fabcd.png"),
                tweetThumbnail.GetImageSearchUriSauceNao()
            );
        }

        [Fact]
        public async Task Scroll_Test()
        {
            var thumbnailGenerator = this.CreateThumbnailGenerator();
            thumbnailGenerator.Services.Add(this.CreateThumbnailService());

            var tweetThumbnail = new TweetThumbnail();
            tweetThumbnail.Initialize(thumbnailGenerator);

            var post = new PostClass
            {
                StatusId = new TwitterStatusId("100"),
                Media = new() { new("http://example.com/abcd"), new("http://example.com/efgh") },
            };

            await tweetThumbnail.PrepareThumbnails(post, CancellationToken.None)
                .ConfigureAwait(false);

            Assert.Equal(2, tweetThumbnail.Thumbnails.Length);
            Assert.Equal(0, tweetThumbnail.SelectedIndex);

            tweetThumbnail.ScrollDown();
            Assert.Equal(1, tweetThumbnail.SelectedIndex);

            tweetThumbnail.ScrollDown();
            Assert.Equal(1, tweetThumbnail.SelectedIndex);

            tweetThumbnail.ScrollUp();
            Assert.Equal(0, tweetThumbnail.SelectedIndex);

            tweetThumbnail.ScrollUp();
            Assert.Equal(0, tweetThumbnail.SelectedIndex);
        }

        private class MockThumbnailInfo : ThumbnailInfo
        {
            public override Task<MemoryImage> LoadThumbnailImageAsync(HttpClient http, CancellationToken cancellationToken)
                => Task.FromResult(TestUtils.CreateDummyImage());
        }
    }
}
