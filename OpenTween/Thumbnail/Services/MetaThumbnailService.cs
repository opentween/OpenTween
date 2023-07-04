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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Connection;
using OpenTween.Models;

namespace OpenTween.Thumbnail.Services
{
    /// <summary>
    /// og:image や twitter:image をスクレイピングしてサムネイルURLを抽出する
    /// </summary>
    public class MetaThumbnailService : IThumbnailService
    {
        protected static Regex[] metaPatterns =
        {
            new Regex("""<meta (name|property)=["'](?<name>.+?)["'] (content|value)=["'](?<content>[^>]+?)["']"""),
            new Regex("""<meta (content|value)=["'](?<content>[^>]+?)["'] (name|property)=["'](?<name>.+?)["']"""),
        };

        protected static string[] defaultPropertyNames = { "og:image", "twitter:image", "twitter:image:src" };

        protected HttpClient Http
            => this.localHttpClient ?? Networking.Http;

        private readonly HttpClient? localHttpClient;

        protected readonly Regex regex;
        protected readonly string[] propertyNames;

        public MetaThumbnailService(string urlPattern)
            : this(null, urlPattern, null)
        {
        }

        public MetaThumbnailService(string urlPattern, string[]? propNames)
            : this(null, urlPattern, propNames)
        {
        }

        public MetaThumbnailService(HttpClient? http, string urlPattern)
            : this(http, urlPattern, null)
        {
        }

        public MetaThumbnailService(HttpClient? http, string urlPattern, string[]? propNames)
        {
            this.localHttpClient = http;
            this.regex = new Regex(urlPattern);
            this.propertyNames = propNames ?? MetaThumbnailService.defaultPropertyNames;
        }

        public override async Task<ThumbnailInfo?> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            if (!this.regex.IsMatch(url))
                return null;

            try
            {
                var content = await this.FetchImageUrlAsync(url, token)
                    .ConfigureAwait(false);

                var thumbnailUrl = this.GetThumbnailUrl(content);
                if (MyCommon.IsNullOrEmpty(thumbnailUrl)) return null;

                return new ThumbnailInfo
                {
                    MediaPageUrl = url,
                    ThumbnailImageUrl = thumbnailUrl,
                    TooltipText = null,
                };
            }
            catch (HttpRequestException)
            {
            }

            return null;
        }

        protected virtual string? GetThumbnailUrl(string html)
        {
            foreach (var pattern in MetaThumbnailService.metaPatterns)
            {
                var matches = pattern.Matches(html);

                foreach (Match match in matches)
                {
                    var propertyName = match.Groups["name"].Value;
                    if (this.propertyNames.Contains(propertyName))
                    {
                        return match.Groups["content"].Value;
                    }
                }
            }

            return null;
        }

        protected virtual async Task<string> FetchImageUrlAsync(string url, CancellationToken token)
        {
            using var response = await this.Http.GetAsync(url, token)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
        }
    }
}
