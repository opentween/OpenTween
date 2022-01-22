// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTween.Api;
using OpenTween.Api.DataModel;

namespace OpenTween.MediaUploadServices
{
    public class Mobypicture : IMediaUploadService
    {
        private static readonly long MaxFileSize = 5L * 1024 * 1024; // 上限不明

        // 参照: http://developers.mobypicture.com/documentation/1-0/postmedia/
        private static readonly IEnumerable<string> AllowedExtensions = new[]
        {
            // Photo
            ".jpg",
            ".gif",
            ".png",
            ".bmp",

            // Video
            ".flv",
            ".mpg",
            ".mpeg",
            ".mkv",
            ".wmv",
            ".mov",
            ".3gp",
            ".mp4",
            ".avi",

            // Audio
            ".mp3",
            ".wma",
            ".aac",
            ".aif",
            ".au",
            ".flac",
            ".ra",
            ".wav",
            ".ogg",
            ".3gp",
        };

        private readonly IMobypictureApi mobypictureApi;

        private TwitterConfiguration twitterConfig;

        public Mobypicture(Twitter twitter, TwitterConfiguration twitterConfig)
            : this(new MobypictureApi(twitter.Api), twitterConfig)
        {
        }

        public Mobypicture(IMobypictureApi mobypictureApi, TwitterConfiguration twitterConfig)
        {
            this.mobypictureApi = mobypictureApi;
            this.twitterConfig = twitterConfig ?? throw new ArgumentNullException(nameof(twitterConfig));
        }

        public int MaxMediaCount => 1;

        public string SupportedFormatsStrForDialog
        {
            get
            {
                var filterFormatExtensions = "";
                foreach (var pictureExtension in AllowedExtensions)
                {
                    filterFormatExtensions += '*' + pictureExtension + ';';
                }
                return "Media Files(" + filterFormatExtensions + ")|" + filterFormatExtensions;
            }
        }

        public bool CanUseAltText => false;

        public bool CheckFileExtension(string fileExtension)
            => AllowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);

        public bool CheckFileSize(string fileExtension, long fileSize)
        {
            var maxFileSize = this.GetMaxFileSize(fileExtension);
            return maxFileSize == null || fileSize <= maxFileSize.Value;
        }

        public long? GetMaxFileSize(string fileExtension)
            => MaxFileSize;

        public async Task<PostStatusParams> UploadAsync(IMediaItem[] mediaItems, PostStatusParams postParams)
        {
            if (mediaItems == null)
                throw new ArgumentNullException(nameof(mediaItems));

            if (mediaItems.Length != 1)
                throw new ArgumentOutOfRangeException(nameof(mediaItems));

            var item = mediaItems[0];

            if (item == null)
                throw new ArgumentException("Err:Media not specified.");

            if (!item.Exists)
                throw new ArgumentException("Err:Media not found.");

            try
            {
                var imageUrl = await this.mobypictureApi.UploadFileAsync(item, postParams.Text)
                    .ConfigureAwait(false);

                postParams.Text += " " + imageUrl;

                return postParams;
            }
            catch (OperationCanceledException ex)
            {
                throw new WebApiException("Err:Timeout", ex);
            }
        }

        public int GetReservedTextLength(int mediaCount)
            => this.twitterConfig.ShortUrlLength + 1;

        public void UpdateTwitterConfiguration(TwitterConfiguration config)
            => this.twitterConfig = config;
    }
}
