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
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using OpenTween.Connection;

namespace OpenTween.Thumbnail.Services
{
    class Tinami : IThumbnailService
    {
        public static readonly Regex UrlPatternRegex =
            new Regex(@"^https?://www\.tinami\.com/view/(?<ContentId>\d+)$");

        protected HttpClient http
        {
            get { return this.localHttpClient ?? Networking.Http; }
        }
        private readonly HttpClient localHttpClient;

        public Tinami()
            : this(null)
        {
        }

        public Tinami(HttpClient http)
        {
            this.localHttpClient = http;
        }

        public override async Task<ThumbnailInfo> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            var match = Tinami.UrlPatternRegex.Match(url);
            if (!match.Success)
                return null;

            var contentId = match.Groups["ContentId"].Value;

            try
            {
                var xdoc = await this.FetchContentInfoApiAsync(contentId, token)
                    .ConfigureAwait(false);

                if (xdoc.XPathSelectElement("/rsp").Attribute("stat").Value != "ok")
                    return null;

                var thumbUrlElm = xdoc.XPathSelectElement("/rsp/content/thumbnails/thumbnail_150x150");
                if (thumbUrlElm == null)
                    return null;

                var descElm = xdoc.XPathSelectElement("/rsp/content/description");

                return new ThumbnailInfo
                {
                    ImageUrl = url,
                    ThumbnailUrl = thumbUrlElm.Attribute("url").Value,
                    TooltipText = descElm == null ? null : descElm.Value,
                };
            }
            catch (HttpRequestException) { }

            return null;
        }

        protected virtual async Task<XDocument> FetchContentInfoApiAsync(string contentId, CancellationToken token)
        {
            var query = new Dictionary<string, string>
            {
                {"api_key", ApplicationSettings.TINAMIApiKey},
                {"cont_id", contentId},
            };

            var apiUrl = new Uri("http://api.tinami.com/content/info?" + MyCommon.BuildQueryString(query));

            using (var response = await this.http.GetAsync(apiUrl, token).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                var xmlStr = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                return XDocument.Parse(xmlStr);
            }
        }
    }
}
