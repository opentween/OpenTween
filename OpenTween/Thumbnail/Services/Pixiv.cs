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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTween.Thumbnail.Services
{
    class Pixiv : MetaThumbnailService
    {
        public Pixiv(string pattern, string replacement = "${0}")
            : base(pattern, replacement)
        {
        }

        public override ThumbnailInfo GetThumbnailInfo(string url, PostClass post)
        {
            var thumb = base.GetThumbnailInfo(url, post);
            if (thumb == null) return null;

            return new Pixiv.Thumbnail
            {
                ImageUrl = thumb.ImageUrl,
                ThumbnailUrl = thumb.ThumbnailUrl,
                TooltipText = thumb.TooltipText,
                FullSizeImageUrl = thumb.FullSizeImageUrl,
            };
        }

        public class Thumbnail : ThumbnailInfo
        {
            public override Task<MemoryImage> LoadThumbnailImageAsync(CancellationToken token)
            {
                var client = new OTWebClient();

                client.UserAgent = MyCommon.GetUserAgentString(fakeMSIE: true);
                client.Headers[HttpRequestHeader.Referer] = this.ImageUrl;

                var task = client.DownloadDataAsync(new Uri(this.ThumbnailUrl), token)
                    .ContinueWith(t => MemoryImage.CopyFromBytes(t.Result));

                task.ContinueWith(_ => client.Dispose());

                return task;
            }
        }
    }
}
