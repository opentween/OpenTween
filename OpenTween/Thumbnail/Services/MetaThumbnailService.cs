// OpenTween - Client of Twitter
// Copyright (c) 2012      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Text.RegularExpressions;

namespace OpenTween.Thumbnail.Services
{
    /// <summary>
    /// og:image や twitter:image をスクレイピングしてサムネイルURLを抽出する
    /// </summary>
    class MetaThumbnailService : SimpleThumbnailService
    {
        protected static Regex metaPattern = new Regex("<meta property=[\"'](?<property>.+?)[\"'] content=[\"'](?<content>.+?)[\"']");
        protected static string[] propertyNames = { "twitter:image", "og:image" };

        public MetaThumbnailService(string pattern, string replacement = "${0}")
            : base(pattern, replacement)
        {
        }

        public override ThumbnailInfo GetThumbnailInfo(string url, PostClass post)
        {
            var pageUrl = this.ReplaceUrl(url);
            if (pageUrl == null) return null;

            var thumbnailUrl = this.FetchThumbnailUrl(pageUrl);
            if (string.IsNullOrEmpty(thumbnailUrl)) return null;

            return new ThumbnailInfo()
            {
                ImageUrl = url,
                ThumbnailUrl = thumbnailUrl,
                TooltipText = null,
            };
        }

        protected virtual string FetchThumbnailUrl(string url)
        {
            using (var client = new OTWebClient())
            {
                var content = client.DownloadString(url);
                var matches = MetaThumbnailService.metaPattern.Matches(content);

                foreach (Match match in matches)
                {
                    var propertyName = match.Groups["property"].Value;
                    if (MetaThumbnailService.propertyNames.Contains(propertyName))
                    {
                        return match.Groups["content"].Value;
                    }
                }

                return null;
            }
        }
    }
}
