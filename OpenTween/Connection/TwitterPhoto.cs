// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      spinor (@tplantd) <http://d.hatena.ne.jp/spinor/>
//           (c) 2014      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Threading.Tasks;
using OpenTween.Api.DataModel;

namespace OpenTween.Connection
{
    public class TwitterPhoto : IMediaUploadService
    {
        private readonly string[] pictureExt = new[] { ".jpg", ".jpeg", ".gif", ".png" };

        private readonly Twitter tw;
        private TwitterConfiguration twitterConfig;

        public TwitterPhoto(Twitter twitter, TwitterConfiguration twitterConfig)
        {
            this.tw = twitter;
            this.twitterConfig = twitterConfig;
        }

        public int MaxMediaCount
        {
            get { return 4; }
        }

        public string SupportedFormatsStrForDialog
        {
            get
            {
                return "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png";
            }
        }

        public bool CanUseAltText => true;

        public bool CheckFileExtension(string fileExtension)
        {
            return this.pictureExt.Contains(fileExtension.ToLowerInvariant());
        }

        public bool CheckFileSize(string fileExtension, long fileSize)
        {
            var maxFileSize = this.GetMaxFileSize(fileExtension);
            return maxFileSize == null || fileSize <= maxFileSize.Value;
        }

        public long? GetMaxFileSize(string fileExtension)
        {
            return this.twitterConfig.PhotoSizeLimit;
        }

        public async Task PostStatusAsync(string text, long? inReplyToStatusId, IMediaItem[] mediaItems)
        {
            if (mediaItems == null)
                throw new ArgumentNullException(nameof(mediaItems));

            if (mediaItems.Length == 0)
                throw new ArgumentException("Err:Media not specified.");

            foreach (var item in mediaItems)
            {
                if (item == null)
                    throw new ArgumentException("Err:Media not specified.");

                if (!item.Exists)
                    throw new ArgumentException("Err:Media not found.");
            }

            var uploadTasks = from m in mediaItems
                              select this.UploadMediaItem(m);

            var mediaIds = await Task.WhenAll(uploadTasks)
                .ConfigureAwait(false);

            await this.tw.PostStatus(text, inReplyToStatusId, mediaIds)
                .ConfigureAwait(false);
        }

        public int GetReservedTextLength(int mediaCount)
        {
            // 枚数に関わらず文字数は一定
            return this.twitterConfig.ShortUrlLength;
        }

        public void UpdateTwitterConfiguration(TwitterConfiguration config)
        {
            this.twitterConfig = config;
        }

        private async Task<long> UploadMediaItem(IMediaItem mediaItem)
        {
            var mediaId = await this.tw.UploadMedia(mediaItem)
                .ConfigureAwait(false);

            if (!string.IsNullOrEmpty(mediaItem.AltText))
            {
                await this.tw.Api.MediaMetadataCreate(mediaId, mediaItem.AltText)
                    .ConfigureAwait(false);
            }

            return mediaId;
        }
    }
}
