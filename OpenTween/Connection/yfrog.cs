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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Api;
using OpenTween.Api.DataModel;

namespace OpenTween.Connection
{
    public class yfrog : IMediaUploadService
    {
        private readonly string[] pictureExt = new[] { ".jpg", ".jpeg", ".gif", ".png" };
        private readonly long MaxFileSize = 5L * 1024 * 1024;

        private readonly Twitter tw;
        private readonly YfrogApi yfrogApi;

        private TwitterConfiguration twitterConfig;

        public yfrog(Twitter twitter, TwitterConfiguration twitterConfig)
        {
            this.tw = twitter;
            this.twitterConfig = twitterConfig;

            this.yfrogApi = new YfrogApi(twitter.Api);
        }

        public int MaxMediaCount
        {
            get { return 1; }
        }

        public string SupportedFormatsStrForDialog
        {
            get
            {
                return "Image Files(*.gif;*.jpg;*.jpeg;*.png)|*.gif;*.jpg;*.jpeg;*.png";
            }
        }

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

            var xml = await this.yfrogApi.UploadFileAsync(item, text)
                .ConfigureAwait(false);

            var imageUrlElm = xml.XPathSelectElement("/rsp/mediaurl");
            if (imageUrlElm == null)
                throw new WebApiException("Invalid API response", xml.ToString());

            var textWithImageUrl = text + " " + imageUrlElm.Value.Trim();

            await this.tw.PostStatus(textWithImageUrl, inReplyToStatusId)
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

        public class YfrogApi
        {
            private readonly HttpClient http;

            private static readonly Uri UploadEndpoint = new Uri("https://yfrog.com/api/xauth_upload");

            private static readonly Uri OAuthRealm = new Uri("http://api.twitter.com/");
            private static readonly Uri AuthServiceProvider = new Uri("https://api.twitter.com/1.1/account/verify_credentials.json");

            public YfrogApi(TwitterApi twitterApi)
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
                // 参照: http://twitter.yfrog.com/page/api#a1

                using (var request = new HttpRequestMessage(HttpMethod.Post, UploadEndpoint))
                using (var multipart = new MultipartFormDataContent())
                {
                    request.Content = multipart;

                    using (var apiKeyContent = new StringContent(ApplicationSettings.YfrogApiKey))
                    using (var mediaStream = item.OpenRead())
                    using (var mediaContent = new StreamContent(mediaStream))
                    {
                        multipart.Add(apiKeyContent, "key");
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
