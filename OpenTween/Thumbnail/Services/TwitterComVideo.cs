// OpenTween - Client of Twitter
// Copyright (c) 2015 spx (@5px)
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
    /// サムネイル用URLと動画用URLを用意する
    /// </summary>
    class TwitterComVideo : MetaThumbnailService
    {
        public static readonly string UrlPattern = @"^https?://amp\.twimg\.com/v/.*$";

        public TwitterComVideo()
            : base(null, TwitterComVideo.UrlPattern)
        {
        }

        public TwitterComVideo(HttpClient http)
            : base(http, TwitterComVideo.UrlPattern)
        {
        }

        public override async Task<ThumbnailInfo> GetThumbnailInfoAsync(string url, PostClass post, CancellationToken token)
        {
            // 前処理で動画用URLが準備されていればそれを使う
            var mediaInfo = post.Media.FirstOrDefault(x => x.Url == url);
            if (mediaInfo.VideoUrl != null)
            {
                return new ThumbnailInfo
                {
                    ImageUrl = mediaInfo.VideoUrl,
                    ThumbnailUrl = url,
                    IsPlayable = true,
                };
            }

            // amp.twimg.com のメタデータからサムネイル用URLを取得する
            var thumbInfo = await base.GetThumbnailInfoAsync(url, post, token)
                .ConfigureAwait(false);

            if (thumbInfo != null)
            {
                thumbInfo.IsPlayable = true;
                return thumbInfo;
            }

            return null;
        }
    }
}
