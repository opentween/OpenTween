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

#nullable enable

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
using OpenTween.Models;

namespace OpenTween.Thumbnail.Services
{
    public class Vimeo : IThumbnailService
    {
        public static readonly Regex UrlPatternRegex =
            new(@"https?://vimeo\.com/(?<postID>[0-9]+)");

        protected HttpClient Http
            => this.localHttpClient ?? Networking.Http;

        private readonly HttpClient? localHttpClient;

        public Vimeo()
            : this(null)
        {
        }

        public Vimeo(HttpClient? http)
            => this.localHttpClient = http;

        public override async Task<ThumbnailInfo?> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            var match = Vimeo.UrlPatternRegex.Match(url);
            if (!match.Success)
                return null;

            try
            {
                var apiUrl = "https://vimeo.com/api/oembed.xml?url=" + Uri.EscapeDataString(url);

                var xmlStr = await this.Http.GetStringAsync(apiUrl)
                    .ConfigureAwait(false);

                var xdoc = XDocument.Parse(xmlStr);

                var thumbUrlElm = xdoc.XPathSelectElement("/oembed/thumbnail_url");
                if (thumbUrlElm == null)
                    return null;

                var titleElm = xdoc.XPathSelectElement("/oembed/title");
                var durationElm = xdoc.XPathSelectElement("/oembed/duration");

                var tooltipText = "";
                if (titleElm != null && durationElm != null)
                {
                    var duration = int.Parse(durationElm.Value);
                    var minute = duration / 60;
                    var second = duration % 60;
                    tooltipText = string.Format("{0} ({1:00}:{2:00})", titleElm.Value, minute, second);
                }

                return new ThumbnailInfo
                {
                    MediaPageUrl = url,
                    ThumbnailImageUrl = thumbUrlElm.Value,
                    TooltipText = tooltipText,
                    IsPlayable = true,
                };
            }
            catch (HttpRequestException)
            {
            }

            return null;
        }
    }
}
