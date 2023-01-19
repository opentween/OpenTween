// OpenTween - Client of Twitter
// Copyright (c) 2023 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace OpenTween
{
    /// <summary>
    /// <see cref="System.Windows.Forms.ImageList"/> の画像に <see cref="MemoryImage"/> を使用するためのラッパー
    /// </summary>
    public sealed class MemoryImageList : IDisposable, IEnumerable<MemoryImage>
    {
        private readonly ImageList innerImageList = new();
        private readonly Dictionary<string, MemoryImage> images = new();

        public ImageList ImageList
            => this.innerImageList;

        public void Add(string key, MemoryImage image)
        {
            this.images.Add(key, image);
            this.innerImageList.Images.Add(key, image.Image);
        }

        public void Remove(string key)
        {
            this.images.Remove(key);
            this.innerImageList.Images.RemoveByKey(key);
        }

        public void Clear()
        {
            this.images.Clear();
            this.innerImageList.Images.Clear();
        }

        public void Dispose()
        {
            // MemoryImage インスタンスの破棄は行わない
            this.Clear();
            this.innerImageList.Dispose();
        }

        public IEnumerator<MemoryImage> GetEnumerator()
            => this.images.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
