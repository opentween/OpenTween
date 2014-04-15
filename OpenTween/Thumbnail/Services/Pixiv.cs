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

        public override Task<ThumbnailInfo> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            return Task.Run<ThumbnailInfo>(async () =>
            {
                var thumb = await base.GetThumbnailInfoAsync(url, post, token);
                if (thumb == null) return null;

                return new Pixiv.Thumbnail(this.http)
                {
                    ImageUrl = thumb.ImageUrl,
                    ThumbnailUrl = thumb.ThumbnailUrl,
                    TooltipText = thumb.TooltipText,
                    FullSizeImageUrl = thumb.FullSizeImageUrl,
                };
            }, token);
        }

        public class Thumbnail : ThumbnailInfo
        {
            public Thumbnail(HttpClient http)
                : base(http)
            {
            }

            public async override Task<MemoryImage> LoadThumbnailImageAsync(CancellationToken token)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, this.ThumbnailUrl);

                request.Headers.Add("User-Agent", MyCommon.GetUserAgentString(fakeMSIE: true));
                request.Headers.Referrer = new Uri(this.ImageUrl);

                using (var response = await this.http.SendAsync(request, token).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();

                    var imageStream = await response.Content.ReadAsStreamAsync()
                        .ConfigureAwait(false);

                    return await MemoryImage.CopyFromStreamAsync(imageStream)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
