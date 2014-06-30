// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
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
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenTween
{
    public class ImageListViewItem : ListViewItem
    {
        protected readonly ImageCache imageCache;
        protected readonly string imageUrl;

        /// <summary>
        /// 状態表示に使用するアイコンのインデックスを取得・設定する。
        /// </summary>
        /// <remarks>
        /// StateImageIndex は不必要な処理が挟まるため、使用しないようにする。
        /// </remarks>
        public int StateIndex { get; set; }

        private WeakReference imageReference = new WeakReference(null);
        private Task imageTask = null;

        public event EventHandler ImageDownloaded;

        public ImageListViewItem(string[] items)
            : this(items, null, null)
        {
        }

        public ImageListViewItem(string[] items, ImageCache imageCache, string imageUrl)
            : base(items)
        {
            this.imageCache = imageCache;
            this.imageUrl = imageUrl;
            this.StateIndex = -1;

            if (imageCache != null)
            {
                var image = imageCache.TryGetFromCache(imageUrl);

                if (image != null)
                    this.imageReference.Target = image;
            }
        }

        public Task GetImageAsync(bool force = false)
        {
            if (this.imageTask == null || this.imageTask.IsCompleted)
            {
                this.imageTask = this.GetImageAsyncInternal(force);
            }

            return this.imageTask;
        }

        private async Task GetImageAsyncInternal(bool force)
        {
            if (string.IsNullOrEmpty(this.imageUrl))
                return;

            if (!force && this.imageReference.Target != null)
                return;

            try
            {
                var image = await this.imageCache.DownloadImageAsync(this.imageUrl, force);

                this.imageReference.Target = image;

                if (this.ListView == null || !this.ListView.Created || this.ListView.IsDisposed)
                    return;

                if (this.Index < this.ListView.VirtualListSize)
                {
                    this.ListView.RedrawItems(this.Index, this.Index, true);

                    if (this.ImageDownloaded != null)
                        this.ImageDownloaded(this, EventArgs.Empty);
                }
            }
            catch (HttpRequestException) { }
            catch (InvalidImageException) { }
            catch (TaskCanceledException) { }
        }

        public MemoryImage Image
        {
            get
            {
                return (MemoryImage)this.imageReference.Target;
            }
        }

        public Task RefreshImageAsync()
        {
            this.imageReference.Target = null;
            return this.GetImageAsync(true);
        }
    }
}
