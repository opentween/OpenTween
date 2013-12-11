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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml.Linq;
using System.Xml;

namespace OpenTween.Connection
{
    public class Imgur : HttpConnectionOAuth, IMultimediaShareService
    {
        private readonly static long MaxFileSize = 10L * 1024 * 1024;
        private readonly static Uri UploadEndpoint = new Uri("https://api.imgur.com/3/image.xml");

        private readonly static IEnumerable<string> SupportedExtensions = new[]
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

        private readonly Twitter _twitter;

        public Imgur(Twitter tw)
        {
            this._twitter = tw;

            Initialize(ApplicationSettings.TwitterConsumerKey, ApplicationSettings.TwitterConsumerSecret,
                       tw.AccessToken, tw.AccessTokenSecret,
                       "", "");
        }

        protected override void AppendOAuthInfo(HttpWebRequest webRequest, Dictionary<string, string> query, string token, string tokenSecret)
        {
            webRequest.Headers[HttpRequestHeader.Authorization] =
                string.Format("Client-ID {0}", ApplicationSettings.ImgurClientID);
        }

        public string Upload(ref string filePath, ref string message, long? reply_to)
        {
            if (!File.Exists(filePath))
                return "Err:File isn't exists.";

            var mediaFile = new FileInfo(filePath);
            var content = "";
            HttpStatusCode result;
            try
            {
                result = this.UploadFile(mediaFile, message, ref content);
            }
            catch (Exception ex)
            {
                return "Err:" + ex.Message;
            }

            if (result != HttpStatusCode.OK)
            {
                return "Err:" + result;
            }

            var imageUrl = "";
            try
            {
                var xdoc = XDocument.Parse(content);
                var image = xdoc.Element("data");
                if (image.Attribute("success").Value != "1")
                {
                    return "APIErr:" + image.Attribute("status").Value;
                }
                imageUrl = image.Element("link").Value;
            }
            catch (XmlException ex)
            {
                return "XmlErr:" + ex.Message;
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

        private HttpStatusCode UploadFile(FileInfo mediaFile, string message, ref string content)
        {
            if (!CheckValidExtension(mediaFile.Extension))
                throw new ArgumentException("Service don't support this filetype", "mediaFile");
            if (!CheckValidFilesize(mediaFile.Extension, mediaFile.Length))
                throw new ArgumentException("File is too large", "mediaFile");

            var param = new Dictionary<string, string>
            {
                {"title", message},
            };
            var binary = new List<KeyValuePair<string, FileInfo>>
            {
                new KeyValuePair<string, FileInfo>("image", mediaFile)
            };
            this.InstanceTimeout = 60000;

            return this.GetContent(PostMethod, UploadEndpoint, param, binary, ref content, null, null);
        }

        public bool CheckValidExtension(string ext)
        {
            return SupportedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase);
        }

        public string GetFileOpenDialogFilter()
        {
            var formats = new StringBuilder();

            foreach (var extension in SupportedExtensions)
                formats.AppendFormat("*{0};", extension);

            return "Image Files(" + formats + ")|" + formats;
        }

        public MyCommon.UploadFileType GetFileType(string ext)
        {
            return this.CheckValidExtension(ext)
                ? MyCommon.UploadFileType.Picture
                : MyCommon.UploadFileType.Invalid;
        }

        public bool IsSupportedFileType(MyCommon.UploadFileType type)
        {
            return type == MyCommon.UploadFileType.Picture;
        }

        public bool CheckValidFilesize(string ext, long fileSize)
        {
            return CheckValidExtension(ext) && fileSize <= MaxFileSize;
        }

        public bool Configuration(string key, object value)
        {
            throw new NotImplementedException();
        }
    }
}
