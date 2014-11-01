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
using OpenTween.Connection;

namespace OpenTween.Thumbnail.Services
{
    class Pixiv : MetaThumbnailService
    {
        public static readonly string UrlPattern =
            @"^https?://www\.pixiv\.net/(member_illust|index)\.php\?(?=.*mode=(medium|big))(?=.*illust_id=(?<illustId>[0-9]+)).*$";

        public Pixiv()
            : base(Pixiv.UrlPattern)
        {
        }

        public Pixiv(HttpClient http)
            : base(http, Pixiv.UrlPattern)
        {
        }

        public override async Task<ThumbnailInfo> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            var thumb = await base.GetThumbnailInfoAsync(url, post, token)
                .ConfigureAwait(false);

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
            public async override Task<MemoryImage> LoadThumbnailImageAsync(HttpClient http, CancellationToken cancellationToken)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, this.ThumbnailUrl);

                request.Headers.Add("User-Agent", Networking.GetUserAgentString(fakeMSIE: true));
                request.Headers.Referrer = new Uri(this.ImageUrl);

                using (var response = await http.SendAsync(request, cancellationToken).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();

                    using (var imageStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        return await MemoryImage.CopyFromStreamAsync(imageStream)
                            .ConfigureAwait(false);
                    }
                }
            }
        }
    }
}
