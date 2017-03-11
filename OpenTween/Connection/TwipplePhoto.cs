// OpenTween - Client of Twitter
// Copyright (c) 2013 ANIKITI (@anikiti07) <https://twitter.com/anikiti07>
//           (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Api;
using OpenTween.Api.DataModel;

namespace OpenTween.Connection
{
    public sealed class TwipplePhoto : IMediaUploadService
    {
        private static readonly long MaxFileSize = 4L * 1024 * 1024;
        private static readonly IEnumerable<string> SupportedPictureExtensions = new[]
        {
            ".gif",
            ".jpg",
            ".png",
        };

        private readonly Twitter twitter;
        private readonly TwippleApi twippleApi;

        private TwitterConfiguration twitterConfig;

        #region Constructors

        public TwipplePhoto(Twitter twitter, TwitterConfiguration twitterConfig)
        {
            this.twitter = twitter ?? throw new ArgumentNullException(nameof(twitter));
            this.twitterConfig = twitterConfig ?? throw new ArgumentNullException(nameof(twitterConfig));

            this.twippleApi = new TwippleApi(twitter.Api);
        }

        #endregion

        public int MaxMediaCount
        {
            get { return 1; }
        }

        public string SupportedFormatsStrForDialog
        {
            get
            {
                var filterFormatExtensions = "";
                foreach (var pictureExtension in SupportedPictureExtensions)
                {
                    filterFormatExtensions += '*';
                    filterFormatExtensions += pictureExtension;
                    filterFormatExtensions += ';';
                }
                return "Image Files(" + filterFormatExtensions + ")|" + filterFormatExtensions;
            }
        }

        public bool CanUseAltText => false;

        public bool CheckFileExtension(string fileExtension)
        {
            return SupportedPictureExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
        }

        public bool CheckFileSize(string fileExtension, long fileSize)
        {
            var maxFileSize = this.GetMaxFileSize(fileExtension);
            return maxFileSize == null || fileSize <= maxFileSize.Value;
        }

        public long? GetMaxFileSize(string fileExtension)
        {
            return MaxFileSize;
        }

        public async Task PostStatusAsync(string text, long? inReplyToStatusId, IMediaItem[] mediaItems)
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

            var xml = await this.twippleApi.UploadFileAsync(item)
                .ConfigureAwait(false);

            var imageUrlElm = xml.XPathSelectElement("/rsp/mediaurl");
            if (imageUrlElm == null)
                throw new WebApiException("Invalid API response", xml.ToString());

            var textWithImageUrl = text + " " + imageUrlElm.Value.Trim();

            await this.twitter.PostStatus(textWithImageUrl, inReplyToStatusId)
                .ConfigureAwait(false);
        }

        public int GetReservedTextLength(int mediaCount)
            => this.twitterConfig.ShortUrlLength + 1;

        public void UpdateTwitterConfiguration(TwitterConfiguration config)
        {
            this.twitterConfig = config;
        }

        public class TwippleApi
        {
            private readonly HttpClient http;

            private static readonly Uri UploadEndpoint = new Uri("http://p.twipple.jp/api/upload2");

            private static readonly Uri OAuthRealm = new Uri("http://api.twitter.com/");
            private static readonly Uri AuthServiceProvider = new Uri("https://api.twitter.com/1.1/account/verify_credentials.json");

            public TwippleApi(TwitterApi twitterApi)
            {
                var handler = twitterApi.CreateOAuthEchoHandler(AuthServiceProvider, OAuthRealm);

                this.http = Networking.CreateHttpClient(handler);
                this.http.Timeout = Networking.UploadImageTimeout;
            }

            /// <summary>
            /// 画像のアップロードを行います
            /// </summary>
            /// <exception cref="WebApiException"/>
            /// <exception cref="XmlException"/>
            public async Task<XDocument> UploadFileAsync(IMediaItem item)
            {
                // 参照: http://p.twipple.jp/wiki/API_Upload2/ja

                using (var request = new HttpRequestMessage(HttpMethod.Post, UploadEndpoint))
                using (var multipart = new MultipartFormDataContent())
                {
                    request.Content = multipart;

                    using (var uploadFromContent = new StringContent(Application.ProductName))
                    using (var mediaStream = item.OpenRead())
                    using (var mediaContent = new StreamContent(mediaStream))
                    {
                        multipart.Add(uploadFromContent, "upload_from");
                        multipart.Add(mediaContent, "media", item.Name);

                        using (var response = await this.http.SendAsync(request).ConfigureAwait(false))
                        {
                            var responseText = await response.Content.ReadAsStringAsync()
                                .ConfigureAwait(false);

                            if (!response.IsSuccessStatusCode)
                                throw new WebApiException(response.StatusCode.ToString(), responseText);

                            return XDocument.Parse(responseText);
                        }
                    }
                }
            }
        }
    }
}