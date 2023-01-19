// OpenTween - Client of Twitter
// Copyright (c) 2014 spx (@5px)
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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Moq;
using OpenTween.Api;
using OpenTween.Api.DataModel;
using Xunit;

namespace OpenTween
{
    public class MediaSelectorTest
    {
        public MediaSelectorTest()
            => this.MyCommonSetup();

        private void MyCommonSetup()
        {
            var mockAssembly = new Mock<_Assembly>();
            mockAssembly.Setup(m => m.GetName()).Returns(new AssemblyName("OpenTween"));

            MyCommon.EntryAssembly = mockAssembly.Object;
        }

        [Fact]
        public void SelectMediaService_TwitterTest()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);
            using var mediaSelector = new MediaSelector();
            twitter.Initialize("", "", "", 0L);
            mediaSelector.InitializeServices(twitter, TwitterConfiguration.DefaultConfiguration());
            mediaSelector.SelectMediaService("Twitter");

            Assert.Contains(mediaSelector.MediaServices, x => x.Key == "Twitter");

            // 投稿先に Twitter が選択されている
            Assert.Equal("Twitter", mediaSelector.SelectedMediaServiceName);

            // 代替テキストが入力可能な状態
            Assert.True(mediaSelector.CanUseAltText);
        }

        [Fact]
        public void SelectMediaService_ImgurTest()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);
            using var mediaSelector = new MediaSelector();
            twitter.Initialize("", "", "", 0L);
            mediaSelector.InitializeServices(twitter, TwitterConfiguration.DefaultConfiguration());
            mediaSelector.SelectMediaService("Imgur");

            // 投稿先に Imgur が選択されている
            Assert.Equal("Imgur", mediaSelector.SelectedMediaServiceName);

            // 代替テキストが入力できない状態
            Assert.False(mediaSelector.CanUseAltText);
        }

        [Fact]
        public void AddMediaItem_FilePath_SingleTest()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);
            using var mediaSelector = new MediaSelector();
            twitter.Initialize("", "", "", 0L);
            mediaSelector.InitializeServices(twitter, TwitterConfiguration.DefaultConfiguration());
            mediaSelector.SelectMediaService("Twitter");

            var images = new[] { "Resources/re.gif" };
            mediaSelector.AddMediaItemFromFilePath(images);

            // 画像が 1 つ追加された状態
            Assert.Single(mediaSelector.MediaItems);

            // 1 枚目の画像が表示されている
            Assert.Equal(0, mediaSelector.SelectedMediaItemIndex);
            Assert.Equal(Path.GetFullPath("Resources/re.gif"), mediaSelector.SelectedMediaItem!.Path);

            using var imageStream = File.OpenRead("Resources/re.gif");
            using var expectedImage = MemoryImage.CopyFromStream(imageStream);
            using var actualImage = mediaSelector.SelectedMediaItem.CreateImage();
            Assert.Equal(expectedImage, actualImage);
        }

        [Fact]
        public void AddMediaItem_MemoryImageTest()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);
            using var mediaSelector = new MediaSelector();
            twitter.Initialize("", "", "", 0L);
            mediaSelector.InitializeServices(twitter, TwitterConfiguration.DefaultConfiguration());
            mediaSelector.SelectMediaService("Twitter");

            using (var bitmap = new Bitmap(width: 200, height: 200))
                mediaSelector.AddMediaItemFromImage(bitmap);

            // 画像が 1 つ追加された状態
            Assert.Single(mediaSelector.MediaItems);

            // 1 枚目の画像が表示されている
            Assert.Equal(0, mediaSelector.SelectedMediaItemIndex);
            Assert.Matches(@"^<>MemoryImage://\d+.png$", mediaSelector.SelectedMediaItem!.Path);

            using (var bitmap = new Bitmap(width: 200, height: 200))
            {
                using var expectedImage = MemoryImage.CopyFromImage(bitmap);
                using var actualImage = mediaSelector.SelectedMediaItem.CreateImage();
                Assert.Equal(expectedImage, actualImage);
            }
        }

        [Fact]
        public void AddMediaItem_FilePath_MultipleTest()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);
            using var mediaSelector = new MediaSelector();
            twitter.Initialize("", "", "", 0L);
            mediaSelector.InitializeServices(twitter, TwitterConfiguration.DefaultConfiguration());
            mediaSelector.SelectMediaService("Twitter");

            var images = new[] { "Resources/re.gif", "Resources/re1.png" };
            mediaSelector.AddMediaItemFromFilePath(images);

            // 画像が 2 つ追加された状態
            Assert.Equal(2, mediaSelector.MediaItems.Count);

            // 最後の画像（2 枚目）が表示されている
            Assert.Equal(1, mediaSelector.SelectedMediaItemIndex);
            Assert.Equal(Path.GetFullPath("Resources/re1.png"), mediaSelector.SelectedMediaItem!.Path);

            using var imageStream = File.OpenRead("Resources/re1.png");
            using var expectedImage = MemoryImage.CopyFromStream(imageStream);
            using var actualImage = mediaSelector.SelectedMediaItem.CreateImage();
            Assert.Equal(expectedImage, actualImage);
        }

        [Fact]
        public void ClearMediaItems_Test()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);
            using var mediaSelector = new MediaSelector();
            twitter.Initialize("", "", "", 0L);
            mediaSelector.InitializeServices(twitter, TwitterConfiguration.DefaultConfiguration());
            mediaSelector.SelectMediaService("Twitter");

            mediaSelector.AddMediaItemFromFilePath(new[] { "Resources/re.gif" });

            var mediaItems = mediaSelector.MediaItems.ToArray();
            var thumbnailImages = mediaSelector.ThumbnailList.ToArray(); // 表示中の画像

            mediaSelector.ClearMediaItems();

            Assert.True(mediaItems.All(x => x.IsDisposed));
            Assert.True(thumbnailImages.All(x => x.IsDisposed));
        }

        [Fact]
        public void DetachMediaItems_Test()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);
            using var mediaSelector = new MediaSelector();
            twitter.Initialize("", "", "", 0L);
            mediaSelector.InitializeServices(twitter, TwitterConfiguration.DefaultConfiguration());
            mediaSelector.SelectMediaService("Twitter");

            mediaSelector.AddMediaItemFromFilePath(new[] { "Resources/re.gif" });

            var thumbnailImages = mediaSelector.ThumbnailList.ToArray();

            var detachedMediaItems = mediaSelector.DetachMediaItems();

            Assert.Empty(mediaSelector.MediaItems);
            Assert.True(thumbnailImages.All(x => x.IsDisposed));

            // DetachMediaItems で切り離された MediaItem は破棄しない
            Assert.True(detachedMediaItems.All(x => !x.IsDisposed));
        }

        [Fact]
        public void SelectedMediaItemChange_Test()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);
            using var mediaSelector = new MediaSelector();
            twitter.Initialize("", "", "", 0L);
            mediaSelector.InitializeServices(twitter, TwitterConfiguration.DefaultConfiguration());
            mediaSelector.SelectMediaService("Twitter");

            var images = new[] { "Resources/re.gif", "Resources/re1.png" };
            mediaSelector.AddMediaItemFromFilePath(images);

            mediaSelector.SelectedMediaItemIndex = 0;

            // 1 ページ目
            Assert.Equal(Path.GetFullPath("Resources/re.gif"), mediaSelector.SelectedMediaItem!.Path);

            using (var imageStream = File.OpenRead("Resources/re.gif"))
            {
                using var expectedImage = MemoryImage.CopyFromStream(imageStream);
                using var actualImage = mediaSelector.SelectedMediaItem.CreateImage();
                Assert.Equal(expectedImage, actualImage);
            }

            mediaSelector.SelectedMediaItemIndex = 1;

            // 2 ページ目
            Assert.Equal(Path.GetFullPath("Resources/re1.png"), mediaSelector.SelectedMediaItem!.Path);

            using (var imageStream = File.OpenRead("Resources/re1.png"))
            {
                using var expectedImage = MemoryImage.CopyFromStream(imageStream);
                using var actualImage = mediaSelector.SelectedMediaItem.CreateImage();
                Assert.Equal(expectedImage, actualImage);
            }
        }

        [Fact]
        public void SelectedMediaItemChange_DisposeTest()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);
            using var mediaSelector = new MediaSelector();
            twitter.Initialize("", "", "", 0L);
            mediaSelector.InitializeServices(twitter, TwitterConfiguration.DefaultConfiguration());
            mediaSelector.SelectMediaService("Twitter");

            var images = new[] { "Resources/re.gif", "Resources/re1.png" };
            mediaSelector.AddMediaItemFromFilePath(images);

            // 1 枚目
            mediaSelector.SelectedMediaItemIndex = 0;
            var firstImage = mediaSelector.SelectedMediaItemImage;

            // 2 枚目
            mediaSelector.SelectedMediaItemIndex = 1;
            var secondImage = mediaSelector.SelectedMediaItemImage;

            Assert.True(firstImage!.IsDisposed);
        }

        [Fact]
        public void SetSelectedMediaAltText_Test()
        {
            using var twitterApi = new TwitterApi(ApiKey.Create(""), ApiKey.Create(""));
            using var twitter = new Twitter(twitterApi);
            using var mediaSelector = new MediaSelector();
            twitter.Initialize("", "", "", 0L);
            mediaSelector.InitializeServices(twitter, TwitterConfiguration.DefaultConfiguration());
            mediaSelector.SelectMediaService("Twitter");

            var images = new[] { "Resources/re.gif", "Resources/re1.png" };
            mediaSelector.AddMediaItemFromFilePath(images);

            // 1 ページ目
            mediaSelector.SelectedMediaItemIndex = 0;
            mediaSelector.SetSelectedMediaAltText("Page 1");

            // 2 ページ目
            mediaSelector.SelectedMediaItemIndex = 1;
            mediaSelector.SetSelectedMediaAltText("Page 2");

            Assert.Equal("Page 1", mediaSelector.MediaItems[0].AltText);
            Assert.Equal("Page 2", mediaSelector.MediaItems[1].AltText);
        }
    }
}
