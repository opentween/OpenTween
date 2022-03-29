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
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Connection;
using OpenTween.Models;

namespace OpenTween.Thumbnail.Services
{
    public class Tumblr : IThumbnailService
    {
        public static readonly Regex UrlPatternRegex =
            new(@"^https?://(?<host>[^.]+\.tumblr\.com|tumblr\.[^.]+\.[^.]+)/post/(?<postId>[0-9]+)(/.*)?");

        protected HttpClient Http
            => this.localHttpClient ?? Networking.Http;

        private readonly ApiKey tumblrConsumerKey;
        private readonly HttpClient? localHttpClient;

        public Tumblr()
            : this(ApplicationSettings.TumblrConsumerKey, null)
        {
        }

        public Tumblr(ApiKey apiKey, HttpClient? http)
        {
            this.tumblrConsumerKey = apiKey;
            this.localHttpClient = http;
        }

        public override async Task<ThumbnailInfo?> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            var match = Tumblr.UrlPatternRegex.Match(url);
            if (!match.Success)
                return null;

            if (!this.tumblrConsumerKey.TryGetValue(out var apiKey))
                return null;

            // 参照: http://www.tumblr.com/docs/en/api/v2#photo-posts

            var host = match.Groups["host"].Value;
            var postId = match.Groups["postId"].Value;

            var param = new Dictionary<string, string>
            {
                ["api_key"] = apiKey,
                ["id"] = postId,
            };

            try
            {
                var apiUrl = string.Format("https://api.tumblr.com/v2/blog/{0}/posts?", host) + MyCommon.BuildQueryString(param);
                using var response = await this.Http.GetAsync(apiUrl, token)
                    .ConfigureAwait(false);

                var jsonBytes = await response.Content.ReadAsByteArrayAsync()
                    .ConfigureAwait(false);

                var thumbs = ParsePhotoPostJson(jsonBytes);

                return thumbs.FirstOrDefault();
            }
            catch (XmlException)
            {
            }
            catch (HttpRequestException)
            {
                // たまに api.tumblr.com が名前解決できない
            }

            return null;
        }

        internal static ThumbnailInfo[] ParsePhotoPostJson(byte[] jsonBytes)
        {
            using var jsonReader = JsonReaderWriterFactory.CreateJsonReader(jsonBytes, XmlDictionaryReaderQuotas.Max);
            var xElm = XElement.Load(jsonReader);

            var item = xElm.XPathSelectElement("/response/posts/item[1]");
            if (item == null)
                return Array.Empty<ThumbnailInfo>();

            var postUrlElm = item.Element("post_url");

            var thumbs =
                from photoElm in item.XPathSelectElements("photos/item/alt_sizes/item[1]/url")
                select new ThumbnailInfo
                {
                    MediaPageUrl = postUrlElm.Value,
                    ThumbnailImageUrl = photoElm.Value,
                    TooltipText = null,
                };

            return thumbs.ToArray();
        }
    }
}
