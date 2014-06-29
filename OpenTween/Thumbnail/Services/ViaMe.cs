// OpenTween - Client of Twitter
// Copyright (c) 2012 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Net;
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

namespace OpenTween.Thumbnail.Services
{
    class ViaMe : IThumbnailService
    {
        public static readonly Regex UrlPatternRegex =
            new Regex(@"^https?://via\.me/-(\w+)$");

        protected HttpClient http
        {
            get { return this.localHttpClient ?? Networking.Http; }
        }
        private readonly HttpClient localHttpClient;

        public ViaMe()
            : this(null)
        {
        }

        public ViaMe(HttpClient http)
        {
            this.localHttpClient = http;
        }

        public override async Task<ThumbnailInfo> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            var match = ViaMe.UrlPatternRegex.Match(url);
            if (!match.Success)
                return null;

            var postId = match.Groups[1].Value;

            try
            {
                var apiUrl = "http://via.me/api/v1/posts/" + postId;

                var json = await this.http.GetByteArrayAsync(apiUrl)
                    .ConfigureAwait(false);

                using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(json, XmlDictionaryReaderQuotas.Max))
                {
                    var xElm = XElement.Load(jsonReader);

                    var thumbUrlElm = xElm.XPathSelectElement("/response/post/thumb_url");
                    if (thumbUrlElm == null)
                        return null;

                    var textElm = xElm.XPathSelectElement("/response/post/text");

                    return new ThumbnailInfo
                    {
                        ImageUrl = url,
                        ThumbnailUrl = thumbUrlElm.Value,
                        TooltipText = textElm == null ? null : textElm.Value,
                    };
                }
            }
            catch (HttpRequestException) { }

            return null;
        }
    }
}
