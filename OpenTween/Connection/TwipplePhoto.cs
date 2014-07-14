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
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Api;

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
            if (twitter == null)
                throw new ArgumentNullException("twitter");
            if (twitterConfig == null)
                throw new ArgumentNullException("twitterConfig");

            this.twitter = twitter;
            this.twitterConfig = twitterConfig;

            this.twippleApi = new TwippleApi(twitter.AccessToken, twitter.AccessTokenSecret);
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

        public async Task PostStatusAsync(string text, long? inReplyToStatusId, string[] filePaths)
        {
            if (filePaths.Length != 1)
                throw new ArgumentOutOfRangeException("filePaths");

            var file = new FileInfo(filePaths[0]);

            if (!file.Exists)
                throw new ArgumentException("Err:File isn't exists.", "filePaths[0]");

            var xml = await this.twippleApi.UploadFileAsync(file)
                .ConfigureAwait(false);

            var imageUrlElm = xml.XPathSelectElement("/rsp/mediaurl");
            if (imageUrlElm == null)
                throw new WebApiException("Invalid API response", xml.ToString());

            var textWithImageUrl = text + " " + imageUrlElm.Value.Trim();

            await Task.Run(() => this.twitter.PostStatus(textWithImageUrl, inReplyToStatusId))
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

        public class TwippleApi : HttpConnectionOAuthEcho
        {
            private static readonly Uri UploadEndpoint = new Uri("http://p.twipple.jp/api/upload2");

            public TwippleApi(string twitterAccessToken, string twitterAccessTokenSecret)
                : base(new Uri("http://api.twitter.com/"), new Uri("https://api.twitter.com/1.1/account/verify_credentials.json"))
            {
                this.Initialize(ApplicationSettings.TwitterConsumerKey, ApplicationSettings.TwitterConsumerSecret,
                    twitterAccessToken, twitterAccessTokenSecret, "", "");

                this.InstanceTimeout = 60000;
            }

            /// <summary>
            /// 画像のアップロードを行います
            /// </summary>
            /// <exception cref="WebApiException"/>
            /// <exception cref="XmlException"/>
            public async Task<XDocument> UploadFileAsync(FileInfo file)
            {
                // 参照: http://p.twipple.jp/wiki/API_Upload2/ja

                var param = new Dictionary<string, string>
                {
                    {"upload_from", Application.ProductName},
                };
                var paramFiles = new List<KeyValuePair<string, FileInfo>>
                {
                    new KeyValuePair<string, FileInfo>("media", file),
                };
                var response = "";

                var uploadTask = Task.Run(() => this.GetContent(HttpConnection.PostMethod,
                    UploadEndpoint, param, paramFiles, ref response, null, null));

                var ret = await uploadTask.ConfigureAwait(false);

                if (ret != HttpStatusCode.OK)
                    throw new WebApiException("Err:" + ret, response);

                return XDocument.Parse(response);
            }
        }
    }
}