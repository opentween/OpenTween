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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Api;
using OpenTween.Api.DataModel;

namespace OpenTween.Connection
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

        private readonly Twitter twitter;
        private readonly MobypictureApi mobypictureApi;

        private TwitterConfiguration twitterConfig;

        public Mobypicture(Twitter twitter, TwitterConfiguration twitterConfig)
        {
            if (twitter == null)
                throw new ArgumentNullException(nameof(twitter));
            if (twitterConfig == null)
                throw new ArgumentNullException(nameof(twitterConfig));

            this.twitter = twitter;
            this.twitterConfig = twitterConfig;

            this.mobypictureApi = new MobypictureApi(twitter.Api);
        }

        public int MaxMediaCount
        {
            get { return 1; }
        }

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
        {
            return AllowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
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

            var xml = await this.mobypictureApi.UploadFileAsync(item, text)
                .ConfigureAwait(false);

            var imageUrlElm = xml.XPathSelectElement("/rsp/media/mediaurl");
            if (imageUrlElm == null)
                throw new WebApiException("Invalid API response", xml.ToString());

            var textWithImageUrl = text + " " + imageUrlElm.Value.Trim();

            await this.twitter.PostStatus(textWithImageUrl, inReplyToStatusId)
                .ConfigureAwait(false);
        }

        public int GetReservedTextLength(int mediaCount)
        {
            return this.twitterConfig.ShortUrlLength;
        }

        public void UpdateTwitterConfiguration(TwitterConfiguration config)
        {
            this.twitterConfig = config;
        }

        public class MobypictureApi
        {
            private readonly HttpClient http;

            private static readonly Uri UploadEndpoint = new Uri("https://api.mobypicture.com/2.0/upload.xml");

            private static readonly Uri OAuthRealm = new Uri("http://api.twitter.com/");
            private static readonly Uri AuthServiceProvider = new Uri("https://api.twitter.com/1.1/account/verify_credentials.json");

            public MobypictureApi(TwitterApi twitterApi)
            {
                var handler = twitterApi.CreateOAuthEchoHandler(AuthServiceProvider, OAuthRealm);

                this.http = Networking.CreateHttpClient(handler);
                this.http.Timeout = TimeSpan.FromMinutes(1);
            }

            /// <summary>
            /// 画像のアップロードを行います
            /// </summary>
            /// <exception cref="WebApiException"/>
            /// <exception cref="XmlException"/>
            public async Task<XDocument> UploadFileAsync(IMediaItem item, string message)
            {
                // 参照: http://developers.mobypicture.com/documentation/2-0/upload/

                using (var request = new HttpRequestMessage(HttpMethod.Post, UploadEndpoint))
                using (var multipart = new MultipartFormDataContent())
                {
                    request.Content = multipart;

                    using (var apiKeyContent = new StringContent(ApplicationSettings.MobypictureKey))
                    using (var messageContent = new StringContent(message))
                    using (var mediaStream = item.OpenRead())
                    using (var mediaContent = new StreamContent(mediaStream))
                    {
                        multipart.Add(apiKeyContent, "key");
                        multipart.Add(messageContent, "message");
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
