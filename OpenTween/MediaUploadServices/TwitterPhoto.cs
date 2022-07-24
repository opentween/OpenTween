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

#nullable enable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OpenTween.Api.DataModel;
using OpenTween.Setting;

namespace OpenTween.MediaUploadServices
{
    public class TwitterPhoto : IMediaUploadService
    {
        private readonly string[] pictureExt = { ".jpg", ".jpeg", ".gif", ".png" };

        private readonly Twitter tw;
        private TwitterConfiguration twitterConfig;

        public TwitterPhoto(Twitter twitter, TwitterConfiguration twitterConfig)
        {
            this.tw = twitter;
            this.twitterConfig = twitterConfig;
        }

        public int MaxMediaCount => 4;

        public string SupportedFormatsStrForDialog => "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png";

        public bool CanUseAltText => true;

        public bool CheckFileExtension(string fileExtension)
            => this.pictureExt.Contains(fileExtension, StringComparer.InvariantCultureIgnoreCase);

        public bool CheckFileSize(string fileExtension, long fileSize)
        {
            var maxFileSize = this.GetMaxFileSize(fileExtension);
            return maxFileSize == null || fileSize <= maxFileSize.Value;
        }

        public long? GetMaxFileSize(string fileExtension)
            => this.twitterConfig.PhotoSizeLimit;

        public async Task<PostStatusParams> UploadAsync(IMediaItem[] mediaItems, PostStatusParams postParams)
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

            long[] mediaIds;

            if (Twitter.DMSendTextRegex.IsMatch(postParams.Text))
                mediaIds = new[] { await this.UploadMediaForDM(mediaItems).ConfigureAwait(false) };
            else
                mediaIds = await this.UploadMediaForTweet(mediaItems).ConfigureAwait(false);

            postParams.MediaIds = mediaIds;

            return postParams;
        }

        // pic.twitter.com の URL は文字数にカウントされない
        public int GetReservedTextLength(int mediaCount)
            => 0;

        public void UpdateTwitterConfiguration(TwitterConfiguration config)
            => this.twitterConfig = config;

        private async Task<long[]> UploadMediaForTweet(IMediaItem[] mediaItems)
        {
            var uploadTasks = from m in mediaItems
                              select this.UploadMediaItem(m, mediaCategory: null);

            var mediaIds = await Task.WhenAll(uploadTasks)
                .ConfigureAwait(false);

            return mediaIds;
        }

        private async Task<long> UploadMediaForDM(IMediaItem[] mediaItems)
        {
            if (mediaItems.Length > 1)
                throw new InvalidOperationException("Err:Can't attach multiple media to DM.");

            var mediaItem = mediaItems[0];
            var mediaCategory = mediaItem.Extension switch
            {
                ".gif" => "dm_gif",
                _ => "dm_image",
            };

            var mediaId = await this.UploadMediaItem(mediaItems[0], mediaCategory)
                .ConfigureAwait(false);

            return mediaId;
        }

        private async Task<long> UploadMediaItem(IMediaItem mediaItem, string? mediaCategory)
        {
            async Task<long> UploadInternal(IMediaItem media, string? category)
            {
                var mediaId = await this.tw.UploadMedia(media, category)
                    .ConfigureAwait(false);

                if (!MyCommon.IsNullOrEmpty(media.AltText))
                {
                    await this.tw.Api.MediaMetadataCreate(mediaId, media.AltText)
                        .ConfigureAwait(false);
                }

                return mediaId;
            }

            using var origImage = mediaItem.CreateImage();

            if (SettingManager.Instance.Common.AlphaPNGWorkaround && this.AddAlphaChannelIfNeeded(origImage.Image, out var newImage))
            {
                using var newMediaItem = new MemoryImageMediaItem(newImage!);
                newMediaItem.AltText = mediaItem.AltText;

                return await UploadInternal(newMediaItem, mediaCategory);
            }
            else
            {
                return await UploadInternal(mediaItem, mediaCategory);
            }
        }

        /// <summary>
        /// pic.twitter.com アップロード時に JPEG への変換を回避するための加工を行う
        /// </summary>
        /// <remarks>
        /// pic.twitter.com へのアップロード時に、アルファチャンネルを持たない PNG 画像が
        /// JPEG 形式に変換され画質が低下する問題を回避します。
        /// PNG 以外の画像や、すでにアルファチャンネルを持つ PNG 画像に対しては何もしません。
        /// </remarks>
        /// <returns>加工が行われた場合は true、そうでない場合は false</returns>
        private bool AddAlphaChannelIfNeeded(Image origImage, out MemoryImage? newImage)
        {
            newImage = null;

            // PNG 画像以外に対しては何もしない
            if (origImage.RawFormat.Guid != ImageFormat.Png.Guid)
                return false;

            using var bitmap = new Bitmap(origImage);

            // アルファ値が 255 以外のピクセルが含まれていた場合は何もしない
            foreach (var x in Enumerable.Range(0, bitmap.Width))
            {
                foreach (var y in Enumerable.Range(0, bitmap.Height))
                {
                    if (bitmap.GetPixel(x, y).A != 255)
                        return false;
                }
            }

            // 左上の 1px だけアルファ値を 254 にする
            var pixel = bitmap.GetPixel(0, 0);
            var newPixel = Color.FromArgb(pixel.A - 1, pixel.R, pixel.G, pixel.B);
            bitmap.SetPixel(0, 0, newPixel);

            // MemoryImage 作成時に画像はコピーされるため、この後 bitmap は破棄しても問題ない
            newImage = MemoryImage.CopyFromImage(bitmap);

            return true;
        }
    }
}
