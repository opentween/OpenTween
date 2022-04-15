// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details.
//
// You should have received a copy of the GNU General public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

using System;
using OpenTween.Api;
using OpenTween.Models;
using OpenTween.Setting;
using OpenTween.Thumbnail;
using OpenTween.Thumbnail.Services;

namespace OpenTween
{
    public sealed class ApplicationContainer : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        public SettingManager Settings { get; }

        public TabInformations TabInfo { get; } = TabInformations.GetInstance();

        public CultureService CultureService
            => this.cultureServiceLazy.Value;

        public TwitterApi TwitterApi
            => this.twitterApiLazy.Value;

        public Twitter Twitter
            => this.twitterLazy.Value;

        public ImageCache ImageCache
            => this.imageCacheLazy.Value;

        public IconAssetsManager IconAssetsManager
            => this.iconAssetsManagerLazy.Value;

        public ImgAzyobuziNet ImgAzyobuziNet
            => this.imgAzyobuziNetLazy.Value;

        public ThumbnailGenerator ThumbnailGenerator
            => this.thumbnailGeneratorLazy.Value;

        public TweenMain MainForm
            => this.mainFormLazy.Value;

        private readonly Lazy<CultureService> cultureServiceLazy;
        private readonly DisposableLazy<TwitterApi> twitterApiLazy;
        private readonly DisposableLazy<Twitter> twitterLazy;
        private readonly DisposableLazy<ImageCache> imageCacheLazy;
        private readonly DisposableLazy<IconAssetsManager> iconAssetsManagerLazy;
        private readonly DisposableLazy<ImgAzyobuziNet> imgAzyobuziNetLazy;
        private readonly Lazy<ThumbnailGenerator> thumbnailGeneratorLazy;
        private readonly DisposableLazy<TweenMain> mainFormLazy;

        public ApplicationContainer(SettingManager settings)
        {
            this.Settings = settings;
            SettingManager.Instance = settings;

            this.cultureServiceLazy = new(this.CreateCultureService);
            this.twitterApiLazy = new(this.CreateTwitterApi);
            this.twitterLazy = new(this.CreateTwitter);
            this.imageCacheLazy = new(this.CreateImageCache);
            this.iconAssetsManagerLazy = new(this.CreateIconAssetsManager);
            this.imgAzyobuziNetLazy = new(this.CreateImgAzyobuziNet);
            this.thumbnailGeneratorLazy = new(this.CreateThumbnailGenerator);
            this.mainFormLazy = new(this.CreateTweenMain);
        }

        private CultureService CreateCultureService()
            => new(this.Settings.Common);

        private TwitterApi CreateTwitterApi()
            => new(ApplicationSettings.TwitterConsumerKey, ApplicationSettings.TwitterConsumerSecret);

        private Twitter CreateTwitter()
            => new(this.TwitterApi);

        private ImageCache CreateImageCache()
            => new();

        private IconAssetsManager CreateIconAssetsManager()
            => new();

        private ImgAzyobuziNet CreateImgAzyobuziNet()
            => new(autoupdate: false);

        private ThumbnailGenerator CreateThumbnailGenerator()
            => new(this.ImgAzyobuziNet);

        private TweenMain CreateTweenMain()
            => new(this.Settings, this.TabInfo, this.Twitter, this.ImageCache, this.IconAssetsManager, this.ThumbnailGenerator);

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.IsDisposed = true;
            this.mainFormLazy.Dispose();
            this.imgAzyobuziNetLazy.Dispose();
            this.twitterLazy.Dispose();
            this.twitterApiLazy.Dispose();
            this.iconAssetsManagerLazy.Dispose();
            this.imageCacheLazy.Dispose();
        }
    }
}
