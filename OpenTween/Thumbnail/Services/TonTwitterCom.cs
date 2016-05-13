// OpenTween - Client of Twitter
// Copyright (c) 2013 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Connection;
using OpenTween.Models;

namespace OpenTween.Thumbnail.Services
{
    /// <summary>
    /// Twitter の DM に添付された画像をサムネイル表示するためのクラス
    /// </summary>
    class TonTwitterCom : IThumbnailService
    {
        internal static Func<IApiConnection> GetApiConnection;

        public override Task<ThumbnailInfo> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            return Task.Run<ThumbnailInfo>(() =>
            {
                if (GetApiConnection == null)
                    return null;

                if (!url.StartsWith(@"https://ton.twitter.com/1.1/ton/data/", StringComparison.Ordinal))
                    return null;

                return new TonTwitterCom.Thumbnail
                {
                    MediaPageUrl = url,
                    ThumbnailImageUrl = url,
                    TooltipText = null,
                    FullSizeImageUrl = url,
                };
            }, token);
        }

        public class Thumbnail : ThumbnailInfo
        {
            public override Task<MemoryImage> LoadThumbnailImageAsync(HttpClient http, CancellationToken cancellationToken)
            {
                return Task.Run(async () =>
                {
                    var apiConnection = TonTwitterCom.GetApiConnection();

                    using (var imageStream = await apiConnection.GetStreamAsync(new Uri(this.ThumbnailImageUrl), null)
                        .ConfigureAwait(false))
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        return await MemoryImage.CopyFromStreamAsync(imageStream)
                            .ConfigureAwait(false);
                    }
                }, cancellationToken);
            }
        }
    }
}
