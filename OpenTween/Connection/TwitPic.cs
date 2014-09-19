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
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Api;

namespace OpenTween.Connection
{
    public class TwitPic : IMediaUploadService
    {
        private readonly string[] pictureExt = new[] { ".jpg", ".jpeg", ".gif", ".png" };

        private readonly string[] multimediaExt = new[] { ".avi", ".wmv", ".flv", ".m4v", ".mov", ".mp4", ".rm", ".mpeg", ".mpg", ".3gp", ".3g2" };

        private readonly long MaxFileSize = 10L * 1024 * 1024; // Image only
        // Multimedia filesize limit unknown. But length limit is 1:30.

        private readonly Twitter tw;
        private readonly TwitpicApi twitpicApi;

        private TwitterConfiguration twitterConfig;

        public TwitPic(Twitter twitter, TwitterConfiguration twitterConfig)
        {
            this.tw = twitter;
            this.twitterConfig = twitterConfig;

            this.twitpicApi = new TwitpicApi(twitter.AccessToken, twitter.AccessTokenSecret);
        }

        public int MaxMediaCount
        {
            get { return 1; }
        }

        public string SupportedFormatsStrForDialog
        {
            get
            {
                return "Image Files(*" + string.Join(";*", this.pictureExt) + ")|*" + string.Join(";*", this.pictureExt)
                    + "|Videos(*" + string.Join(";*", this.multimediaExt) + ")|*" + string.Join(";*", this.multimediaExt);
            }
        }

        public bool CheckFileExtension(string fileExtension)
        {
            fileExtension = fileExtension.ToLower();

            return this.pictureExt.Contains(fileExtension) ||
                this.multimediaExt.Contains(fileExtension);
        }

        public bool CheckFileSize(string fileExtension, long fileSize)
        {
            var maxFileSize = this.GetMaxFileSize(fileExtension);
            return maxFileSize == null || fileSize <= maxFileSize.Value;
        }

        public long? GetMaxFileSize(string fileExtension)
        {
            if (this.multimediaExt.Contains(fileExtension))
                return null; // Multimedia : no check

            return MaxFileSize;
        }

        public async Task PostStatusAsync(string text, long? inReplyToStatusId, string[] filePaths)
        {
            if (filePaths.Length != 1)
                throw new ArgumentOutOfRangeException("filePaths");

            var file = new FileInfo(filePaths[0]);

            if (!file.Exists)
                throw new ArgumentException("Err:File isn't exists.", "filePaths[0]");

            var xml = await this.twitpicApi.UploadFileAsync(file, text)
                .ConfigureAwait(false);

            var imageUrlElm = xml.XPathSelectElement("/image/url");
            if (imageUrlElm == null)
                throw new WebApiException("Invalid API response", xml.ToString());

            var textWithImageUrl = text + " " + imageUrlElm.Value.Trim();

            await Task.Run(() => this.tw.PostStatus(textWithImageUrl, inReplyToStatusId))
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

        public class TwitpicApi : HttpConnectionOAuthEcho
        {
            private static readonly Uri UploadEndpoint = new Uri("http://api.twitpic.com/2/upload.xml");

            public TwitpicApi(string twitterAccessToken, string twitterAccessTokenSecret)
                : base(new Uri("http://api.twitter.com/"), new Uri("https://api.twitter.com/1.1/account/verify_credentials.json"))
            {
                this.Initialize(ApplicationSettings.TwitterConsumerKey, ApplicationSettings.TwitterConsumerSecret,
                    twitterAccessToken, twitterAccessTokenSecret, "", "");

                this.InstanceTimeout = 120000;
            }

            /// <summary>
            /// 画像のアップロードを行います
            /// </summary>
            /// <exception cref="WebApiException"/>
            /// <exception cref="XmlException"/>
            public async Task<XDocument> UploadFileAsync(FileInfo file, string message)
            {
                // 参照: http://dev.twitpic.com/docs/2/upload/

                var param = new Dictionary<string, string>
                {
                    {"key", ApplicationSettings.TwitpicApiKey},
                    {"message", message},
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
