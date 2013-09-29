// OpenTween - Client of Twitter
// Copyright (c) 2013 ANIKITI (@anikiti07) <https://twitter.com/anikiti07>
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
using System.Xml;

namespace OpenTween.Connection
{
    public sealed class TwipplePhoto : HttpConnectionOAuthEcho,
                                       IMultimediaShareService
    {
        private const long MaxFileSize = 4 * 1024 * 1024;

        private readonly Twitter _twitter;
        private readonly Uri _twipplePhotoUploadUri = new Uri("http://p.twipple.jp/api/upload2");
        private readonly IEnumerable<string> _supportedPictureExtensions = new[]
        {
            ".gif",
            ".jpg",
            ".png"
        };

        #region Constructors

        public TwipplePhoto(Twitter twitter)
            : base(new Uri("http://api.twitter.com/"), new Uri("https://api.twitter.com/1.1/account/verify_credentials.json"))
        {
            if (twitter == null)
                throw new ArgumentNullException("twitter");

            _twitter = twitter;
            Initialize(ApplicationSettings.TwitterConsumerKey, ApplicationSettings.TwitterConsumerSecret,
                       _twitter.AccessToken, _twitter.AccessTokenSecret,
                       "", "");
        }

        #endregion

        #region IMultimediaShareService Members

        #region Upload Methods

        public string Upload(ref string filePath, ref string message, long? reply_to)
        {
            if (!File.Exists(filePath))
                return "Err:File isn't exists.";

            var mediaFile = new FileInfo(filePath);
            var content = "";
            HttpStatusCode result;
            try
            {
                result = UploadFile(mediaFile, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            var imageUrl = "";
            if (result == HttpStatusCode.OK)
            {
                try
                {
                    var xdoc = new XmlDocument();
                    xdoc.LoadXml(content);
                    var urlNode = xdoc.SelectSingleNode("/rsp/mediaurl");
                    if (urlNode != null)
                    {
                        imageUrl = urlNode.InnerText;
                    }
                }
                catch (XmlException ex)
                {
                    return "XmlErr:" + ex.Message;
                }
            }
            else
            {
                return "Err:" + result;
            }

            filePath = "";
            if (message == null)
                message = "";

            // Post to twitter
            if (message.Length + AppendSettingDialog.Instance.TwitterConfiguration.CharactersReservedPerMedia + 1 > 140)
            {
                message = message.Substring(0, 140 - AppendSettingDialog.Instance.TwitterConfiguration.CharactersReservedPerMedia - 1) + " " + imageUrl;
            }
            else
            {
                message += " " + imageUrl;
            }
            return _twitter.PostStatus(message, reply_to);
        }

        private HttpStatusCode UploadFile(FileInfo mediaFile, ref string content)
        {
            if (!CheckValidExtension(mediaFile.Extension))
                throw new ArgumentException("Service don't support this filetype", "mediaFile");
            if (!CheckValidFilesize(mediaFile.Extension, mediaFile.Length))
                throw new ArgumentException("File is too large", "mediaFile");

            var binaly = new List<KeyValuePair<string, FileInfo>>
            {
                new KeyValuePair<string, FileInfo>("media", mediaFile)
            };
            InstanceTimeout = 60000;

            return GetContent(PostMethod, _twipplePhotoUploadUri, null, binaly, ref content, null, null);
        }

        #endregion

        public bool CheckValidExtension(string ext)
        {
            return _supportedPictureExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase);
        }

        public string GetFileOpenDialogFilter()
        {
            string filterFormatExtensions = "";
            foreach (var pictureExtension in _supportedPictureExtensions)
            {
                filterFormatExtensions += '*';
                filterFormatExtensions += pictureExtension;
                filterFormatExtensions += ';';
            }
            return "Image Files(" + filterFormatExtensions + ")|" + filterFormatExtensions;
        }

        public MyCommon.UploadFileType GetFileType(string extension)
        {
            return CheckValidExtension(extension)
                       ? MyCommon.UploadFileType.Picture
                       : MyCommon.UploadFileType.Invalid;
        }

        public bool IsSupportedFileType(MyCommon.UploadFileType type)
        {
            return type == MyCommon.UploadFileType.Picture;
        }

        public bool CheckValidFilesize(string extension, long fileSize)
        {
            return CheckValidExtension(extension) && fileSize <= MaxFileSize;
        }

        public bool Configuration(string key, object value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}