// OpenTween - Client of Twitter
// Copyright (c) 2013 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
    public class Imgur : IMediaUploadService
    {
        private static readonly long MaxFileSize = 10L * 1024 * 1024;

        private static readonly IEnumerable<string> SupportedExtensions = new[]
        {
            ".jpg",
            ".jpeg",
            ".gif",
            ".png",
            ".tif",
            ".tiff",
            ".bmp",
            ".pdf",
            ".xcf",
        };

        private readonly IImgurApi imgurApi;

        private TwitterConfiguration twitterConfig;

        public Imgur(TwitterConfiguration twitterConfig)
            : this(new ImgurApi(), twitterConfig)
        {
        }

        public Imgur(IImgurApi imgurApi, TwitterConfiguration twitterConfig)
        {
            this.imgurApi = imgurApi;
            this.twitterConfig = twitterConfig;
        }

        public int MaxMediaCount => 1;

        public string SupportedFormatsStrForDialog
        {
            get
            {
                var formats = new StringBuilder();

                foreach (var extension in SupportedExtensions)
                    formats.AppendFormat("*{0};", extension);

                return "Image Files(" + formats + ")|" + formats;
            }
        }

        public bool CanUseAltText => false;

        public bool CheckFileExtension(string fileExtension)
            => SupportedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);

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
                var imageUrl = await this.imgurApi.UploadFileAsync(item, postParams.Text)
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
