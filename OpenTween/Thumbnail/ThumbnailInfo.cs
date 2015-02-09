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
        public string ImageUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string TooltipText { get; set; }
        public string FullSizeImageUrl { get; set; }

        public bool IsPlayable
        {
            get { return _IsPlayable; }
            set { _IsPlayable = value; }
        }
        private bool _IsPlayable = false;

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
            using (var response = await http.GetAsync(this.ThumbnailUrl, cancellationToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                using (var imageStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    return await MemoryImage.CopyFromStreamAsync(imageStream)
                        .ConfigureAwait(false);
                }
            }
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ThumbnailInfo);
        }

        public bool Equals(ThumbnailInfo other)
        {
            return other != null &&
                other.ImageUrl == this.ImageUrl &&
                other.ThumbnailUrl == this.ThumbnailUrl &&
                other.TooltipText == this.TooltipText &&
                other.FullSizeImageUrl == this.FullSizeImageUrl &&
                other.IsPlayable == this.IsPlayable;
        }

        public override int GetHashCode()
        {
            return this.ImageUrl.GetHashCode() ^ this.ThumbnailUrl.GetHashCode();
        }
    }
}
