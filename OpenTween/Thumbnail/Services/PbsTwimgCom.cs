// OpenTween - Client of Twitter
// Copyright (c) 2016 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Models;

namespace OpenTween.Thumbnail.Services
{
    public class PbsTwimgCom : IThumbnailService
    {
        public static readonly Regex ModernUrlPattern =
            new(@"^(?<base_url>https?://pbs\.twimg\.com/[^:.]+)\?([^&]+?&)?format=(?<format>[A-Za-z]+)");

        public static readonly Regex LegacyUrlPattern =
            new(@"^(?<base_url>https?://pbs\.twimg\.com/[^.]+)\.(?<format>[A-Za-z]+)(?:\:.+)?$");

        public override Task<ThumbnailInfo?> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
            => Task.FromResult(this.GetThumbnailInfo(url, post, token));

        private ThumbnailInfo? GetThumbnailInfo(string url, PostClass post, CancellationToken token)
        {
            string baseUrl;
            string format;

            // 現状の Twitter API から返る media_url_https は LegacyUrlPettern の形式になっているが、
            // どちらの形式でも動くようにする
            if (ModernUrlPattern.Match(url) is { Success: true } matchesModern)
            {
                baseUrl = matchesModern.Groups["base_url"].Value;
                format = matchesModern.Groups["format"].Value;
            }
            else if (LegacyUrlPattern.Match(url) is { Success: true } matchesLegacy)
            {
                baseUrl = matchesLegacy.Groups["base_url"].Value;
                format = matchesLegacy.Groups["format"].Value;
            }
            else
            {
                return null;
            }

            var mediaOrig = $"{baseUrl}?format={format}&name=orig";
            var mediaLarge = $"{baseUrl}?format={format}&name=large";

            var media = post.Media.FirstOrDefault(x => x.Url == url);
            var altText = media?.AltText;

            var thumb = new ThumbnailInfo
            {
                MediaPageUrl = mediaOrig,
                ThumbnailImageUrl = mediaLarge,
                TooltipText = altText,
                FullSizeImageUrl = mediaOrig,
            };

            return thumb;
        }
    }
}
