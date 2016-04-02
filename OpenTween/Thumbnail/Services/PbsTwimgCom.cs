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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTween.Thumbnail.Services
{
    class PbsTwimgCom : SimpleThumbnailService
    {
        public static readonly string UrlPattern = @"^(https?://pbs\.twimg\.com/[^:]+)(?:\:.+)?$";

        public PbsTwimgCom()
            : base(UrlPattern, "${1}", "${1}:orig")
        {
        }

        public override async Task<ThumbnailInfo> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            var thumb = await base.GetThumbnailInfoAsync(url, post, token)
                .ConfigureAwait(false);

            if (thumb == null)
                return null;

            var media = post.Media.FirstOrDefault(x => x.Url == url);
            if (media != null)
            {
                thumb.TooltipText = media.AltText;
            }

            return thumb;
        }
    }
}
