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
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Connection;

namespace OpenTween.Thumbnail
{
    public class ThumbnailInfo : IEquatable<ThumbnailInfo>
    {
        /// <summary>サムネイルとして表示するメディアが掲載されている URL</summary>
        /// <remarks>
        /// 例えば Youtube のサムネイルの場合、動画そのものの URL ではなく
        /// https://www.youtube.com/watch?v=****** 形式の URL が含まれる
        /// </remarks>
        public string MediaPageUrl { get; set; }

        /// <summary>サムネイルとして表示する画像の URL</summary>
        /// <remarks>
        /// ここに含まれる URL は直接画像として表示可能である必要がある
        /// </remarks>
        public string ThumbnailImageUrl { get; set; }

        /// <summary>最も高解像度な画像の URL</summary>
        /// <remarks>
        /// サムネイルとしては不適だが、より高解像度な画像を表示する場面に使用できる
        /// URL があればここに含まれる
        /// </remarks>
        public string FullSizeImageUrl { get; set; }

        /// <summary>ツールチップとして表示するテキスト</summary>
        /// <remarks>
        /// サムネイル画像にマウスオーバーした際に表示されるテキスト
        /// </remarks>
        public string TooltipText { get; set; }

        /// <summary>
        /// 対象となるメディアが動画や音声など再生可能なものであるか否か
        /// </summary>
        public bool IsPlayable { get; set; }

        public Task<MemoryImage> LoadThumbnailImageAsync()
        {
            return this.LoadThumbnailImageAsync(CancellationToken.None);
        }

        public Task<MemoryImage> LoadThumbnailImageAsync(CancellationToken cancellationToken)
        {
            return this.LoadThumbnailImageAsync(Networking.Http, cancellationToken);
        }

        public async virtual Task<MemoryImage> LoadThumbnailImageAsync(HttpClient http, CancellationToken cancellationToken)
        {
            MemoryImage image = null;
            try
            {
                using (var response = await http.GetAsync(this.ThumbnailImageUrl, cancellationToken).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();

                    using (var imageStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    {
                        image = await MemoryImage.CopyFromStreamAsync(imageStream)
                            .ConfigureAwait(false);

                        cancellationToken.ThrowIfCancellationRequested();

                        return image;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                image?.Dispose();
                throw;
            }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ThumbnailInfo);
        }

        public bool Equals(ThumbnailInfo other)
        {
            return other != null &&
                other.MediaPageUrl == this.MediaPageUrl &&
                other.ThumbnailImageUrl == this.ThumbnailImageUrl &&
                other.TooltipText == this.TooltipText &&
                other.FullSizeImageUrl == this.FullSizeImageUrl &&
                other.IsPlayable == this.IsPlayable;
        }

        public override int GetHashCode()
        {
            return this.MediaPageUrl.GetHashCode() ^ this.ThumbnailImageUrl.GetHashCode();
        }
    }
}
