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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Moq;
using OpenTween.Models;
using OpenTween.Thumbnail;
using OpenTween.Thumbnail.Services;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class TweetThumbnailTest
    {
        private class TestThumbnailService : IThumbnailService
        {
            private readonly Regex regex;
            private readonly string replaceUrl;
            private readonly string? replaceTooltip;

            public TestThumbnailService(string pattern, string replaceUrl, string? replaceTooltip)
            {
                this.regex = new Regex(pattern);
                this.replaceUrl = replaceUrl;
                this.replaceTooltip = replaceTooltip;
            }

            public override async Task<ThumbnailInfo?> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
            {
                var match = this.regex.Match(url);

                if (!match.Success) return null;

                if (url.StartsWith("http://slow.example.com/", StringComparison.Ordinal))
                    await Task.Delay(1000, token).ConfigureAwait(false);

                return new MockThumbnailInfo
                {
                    MediaPageUrl = url,
                    ThumbnailImageUrl = match.Result(this.replaceUrl),
                    TooltipText = this.replaceTooltip != null ? match.Result(this.replaceTooltip) : null,
                };
            }

            private class MockThumbnailInfo : ThumbnailInfo
            {
                public override Task<MemoryImage> LoadThumbnailImageAsync(HttpClient http, CancellationToken cancellationToken)
                    => Task.FromResult(TestUtils.CreateDummyImage());
            }
        }

        public TweetThumbnailTest()
            => this.MyCommonSetup();

        private ThumbnailGenerator CreateThumbnailGenerator()
        {
            var imgAzyobuziNet = new ImgAzyobuziNet(autoupdate: false);
            var thumbGenerator = new ThumbnailGenerator(imgAzyobuziNet);
            thumbGenerator.Services.Clear();
            thumbGenerator.Services.AddRange(new[]
            {
                new TestThumbnailService(@"^https?://foo.example.com/(.+)$", @"http://img.example.com/${1}.png", null),
                new TestThumbnailService(@"^https?://bar.example.com/(.+)$", @"http://img.example.com/${1}.png", @"${1}"),
                new TestThumbnailService(@"^https?://slow.example.com/(.+)$", @"http://img.example.com/${1}.png", null),
            });

            return thumbGenerator;
        }

        private void MyCommonSetup()
        {
            var mockAssembly = new Mock<_Assembly>();
            mockAssembly.Setup(m => m.GetName()).Returns(new AssemblyName("OpenTween"));

            MyCommon.EntryAssembly = mockAssembly.Object;
        }

        [Fact]
        public void CreatePictureBoxTest()
        {
            using var thumbBox = new TweetThumbnail();

            var method = typeof(TweetThumbnail).GetMethod("CreatePictureBox", BindingFlags.Instance | BindingFlags.NonPublic);
            var picbox = method.Invoke(thumbBox, new[] { "pictureBox1" }) as PictureBox;

            Assert.NotNull(picbox);
            Assert.Equal("pictureBox1", picbox!.Name);
            Assert.Equal(PictureBoxSizeMode.Zoom, picbox.SizeMode);
            Assert.False(picbox.WaitOnLoad);
            Assert.Equal(DockStyle.Fill, picbox.Dock);

            picbox.Dispose();
        }

        [Fact]
        public async Task CancelAsyncTest()
        {
            var post = new PostClass
            {
                TextFromApi = "てすと http://slow.example.com/abcd",
                Media = new List<MediaInfo>
                {
                    new MediaInfo("http://slow.example.com/abcd"),
                },
            };

            using var thumbbox = new TweetThumbnail();
            thumbbox.Initialize(this.CreateThumbnailGenerator());

            using var tokenSource = new CancellationTokenSource();

            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            var task = thumbbox.ShowThumbnailAsync(post, tokenSource.Token);

            tokenSource.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await task);
            Assert.True(task.IsCanceled);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void SetThumbnailCountTest(int count)
        {
            using var thumbbox = new TweetThumbnail();
            thumbbox.Initialize(this.CreateThumbnailGenerator());

            var method = typeof(TweetThumbnail).GetMethod("SetThumbnailCount", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(thumbbox, new[] { (object)count });

            Assert.Equal(count, thumbbox.PictureBox.Count);

            var num = 0;
            foreach (var picbox in thumbbox.PictureBox)
            {
                Assert.Equal("pictureBox" + num, picbox.Name);
                num++;
            }

            Assert.Equal(thumbbox.PictureBox, thumbbox.panelPictureBox.Controls.Cast<OTPictureBox>());

            Assert.Equal(0, thumbbox.scrollBar.Minimum);

            if (count == 0)
                Assert.Equal(0, thumbbox.scrollBar.Maximum);
            else
                Assert.Equal(count - 1, thumbbox.scrollBar.Maximum);
        }

        [Fact]
        public async Task ShowThumbnailAsyncTest()
        {
            var post = new PostClass
            {
                TextFromApi = "てすと http://foo.example.com/abcd",
                Media = new List<MediaInfo>
                {
                    new MediaInfo("http://foo.example.com/abcd"),
                },
            };

            using var thumbbox = new TweetThumbnail();
            thumbbox.Initialize(this.CreateThumbnailGenerator());

            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            await thumbbox.ShowThumbnailAsync(post);

            Assert.Equal(0, thumbbox.scrollBar.Maximum);
            Assert.False(thumbbox.scrollBar.Enabled);

            Assert.Single(thumbbox.PictureBox);
            Assert.NotNull(thumbbox.PictureBox[0].Image);

            Assert.IsAssignableFrom<ThumbnailInfo>(thumbbox.PictureBox[0].Tag);
            var thumbinfo = (ThumbnailInfo)thumbbox.PictureBox[0].Tag;

            Assert.Equal("http://foo.example.com/abcd", thumbinfo.MediaPageUrl);
            Assert.Equal("http://img.example.com/abcd.png", thumbinfo.ThumbnailImageUrl);

            Assert.Equal("", thumbbox.toolTip.GetToolTip(thumbbox.PictureBox[0]));
        }

        [Fact]
        public async Task ShowThumbnailAsyncTest2()
        {
            var post = new PostClass
            {
                TextFromApi = "てすと http://foo.example.com/abcd http://bar.example.com/efgh",
                Media = new List<MediaInfo>
                {
                    new MediaInfo("http://foo.example.com/abcd"),
                    new MediaInfo("http://bar.example.com/efgh"),
                },
            };

            using var thumbbox = new TweetThumbnail();
            thumbbox.Initialize(this.CreateThumbnailGenerator());

            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            await thumbbox.ShowThumbnailAsync(post);

            Assert.Equal(1, thumbbox.scrollBar.Maximum);
            Assert.True(thumbbox.scrollBar.Enabled);

            Assert.Equal(2, thumbbox.PictureBox.Count);
            Assert.NotNull(thumbbox.PictureBox[0].Image);
            Assert.NotNull(thumbbox.PictureBox[1].Image);

            Assert.IsAssignableFrom<ThumbnailInfo>(thumbbox.PictureBox[0].Tag);
            var thumbinfo = (ThumbnailInfo)thumbbox.PictureBox[0].Tag;

            Assert.Equal("http://foo.example.com/abcd", thumbinfo.MediaPageUrl);
            Assert.Equal("http://img.example.com/abcd.png", thumbinfo.ThumbnailImageUrl);

            Assert.IsAssignableFrom<ThumbnailInfo>(thumbbox.PictureBox[1].Tag);
            thumbinfo = (ThumbnailInfo)thumbbox.PictureBox[1].Tag;

            Assert.Equal("http://bar.example.com/efgh", thumbinfo.MediaPageUrl);
            Assert.Equal("http://img.example.com/efgh.png", thumbinfo.ThumbnailImageUrl);

            Assert.Equal("", thumbbox.toolTip.GetToolTip(thumbbox.PictureBox[0]));
            Assert.Equal("efgh", thumbbox.toolTip.GetToolTip(thumbbox.PictureBox[1]));
        }

        [Fact]
        public async Task ThumbnailLoadingEventTest()
        {
            using var thumbbox = new TweetThumbnail();
            thumbbox.Initialize(this.CreateThumbnailGenerator());

            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var post = new PostClass
            {
                TextFromApi = "てすと",
                Media = new List<MediaInfo>
                {
                },
            };
            await TestUtils.NotRaisesAsync<EventArgs>(
                x => thumbbox.ThumbnailLoading += x,
                x => thumbbox.ThumbnailLoading -= x,
                () => thumbbox.ShowThumbnailAsync(post)
            );

            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var post2 = new PostClass
            {
                TextFromApi = "てすと http://foo.example.com/abcd",
                Media = new List<MediaInfo>
                    {
                        new MediaInfo("http://foo.example.com/abcd"),
                    },
            };

            await Assert.RaisesAsync<EventArgs>(
                x => thumbbox.ThumbnailLoading += x,
                x => thumbbox.ThumbnailLoading -= x,
                () => thumbbox.ShowThumbnailAsync(post2)
            );
        }

        [Fact]
        public async Task ScrollTest()
        {
            var post = new PostClass
            {
                TextFromApi = "てすと http://foo.example.com/abcd http://foo.example.com/efgh",
                Media = new List<MediaInfo>
                {
                    new MediaInfo("http://foo.example.com/abcd"),
                    new MediaInfo("http://foo.example.com/efgh"),
                },
            };

            using var thumbbox = new TweetThumbnail();
            thumbbox.Initialize(this.CreateThumbnailGenerator());

            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            await thumbbox.ShowThumbnailAsync(post);

            Assert.Equal(0, thumbbox.scrollBar.Minimum);
            Assert.Equal(1, thumbbox.scrollBar.Maximum);

            thumbbox.scrollBar.Value = 0;

            thumbbox.ScrollDown();
            Assert.Equal(1, thumbbox.scrollBar.Value);
            Assert.False(thumbbox.PictureBox[0].Visible);
            Assert.True(thumbbox.PictureBox[1].Visible);

            thumbbox.ScrollDown();
            Assert.Equal(1, thumbbox.scrollBar.Value);
            Assert.False(thumbbox.PictureBox[0].Visible);
            Assert.True(thumbbox.PictureBox[1].Visible);

            thumbbox.ScrollUp();
            Assert.Equal(0, thumbbox.scrollBar.Value);
            Assert.True(thumbbox.PictureBox[0].Visible);
            Assert.False(thumbbox.PictureBox[1].Visible);

            thumbbox.ScrollUp();
            Assert.Equal(0, thumbbox.scrollBar.Value);
            Assert.True(thumbbox.PictureBox[0].Visible);
            Assert.False(thumbbox.PictureBox[1].Visible);
        }
    }
}
