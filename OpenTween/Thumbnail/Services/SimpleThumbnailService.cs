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

namespace OpenTween.Thumbnail.Services
{
    /// <summary>
    /// 正規表現によるURLの単純な置換でサムネイルURLを生成する
    /// </summary>
    class SimpleThumbnailService : IThumbnailService
    {
        protected Regex regex;
        protected string thumb_replacement;
        protected string fullsize_replacement;

        public SimpleThumbnailService(string pattern, string replacement)
            : this(pattern, replacement, null)
        {
        }

        public SimpleThumbnailService(string pattern, string replacement, string file_replacement)
        {
            this.regex = new Regex(pattern, RegexOptions.IgnoreCase);
            this.thumb_replacement = replacement;
            this.fullsize_replacement = file_replacement;
        }

        public override Task<ThumbnailInfo> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            return Task.Run(() =>
            {
                var thumbnailUrl = this.ReplaceUrl(url);
                if (thumbnailUrl == null) return null;

                return new ThumbnailInfo
                {
                    ImageUrl = url,
                    ThumbnailUrl = thumbnailUrl,
                    TooltipText = null,
                    FullSizeImageUrl = ReplaceUrl(url, this.fullsize_replacement)
                };
            }, token);
        }

        protected string ReplaceUrl(string url)
        {
            return ReplaceUrl(url, this.thumb_replacement);
        }

        protected string ReplaceUrl(string url, string replacement)
        {
            if (replacement == null) return null;
            var match = this.regex.Match(url);

            return match.Success ? match.Result(replacement) : null;
        }
    }
}
