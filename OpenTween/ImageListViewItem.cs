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
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace OpenTween
{
    public class ImageListViewItem : ListViewItem
    {
        public event EventHandler ImageDownloaded;
        private ImageCache imageCache = null;
        private string imageUrl;

        private WeakReference _ImageReference = new WeakReference(null);

        public ImageListViewItem(string[] items, string imageKey)
            : base(items, imageKey)
        {
        }

        public ImageListViewItem(string[] items, ImageCache imageDictionary, string imageKey)
            : base(items, imageKey)
        {
            this.imageCache = imageDictionary;
            this.imageUrl = imageKey;

            var image = this.imageCache.TryGetFromCache(imageKey);

            if (image == null)
                this.GetImageAsync();
            else
                this._ImageReference.Target = image;
        }

        private Task GetImageAsync(bool force = false)
        {
            return this.imageCache.DownloadImageAsync(this.imageUrl, force).ContinueWith(t =>
            {
                var image = t.Result;

                if (image == null) return;

                this._ImageReference.Target = image;

                if (this.ListView != null &&
                    this.ListView.Created &&
                    !this.ListView.IsDisposed)
                {
                    this.ListView.Invoke(new MethodInvoker(() =>
                    {
                        if (this.Index < this.ListView.VirtualListSize)
                        {
                            this.ListView.RedrawItems(this.Index, this.Index, true);
                            if (ImageDownloaded != null)
                            {
                                ImageDownloaded(this, EventArgs.Empty);
                            }
                        }
                    }));
                }
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public MemoryImage Image
        {
            get
            {
                if (string.IsNullOrEmpty(this.imageUrl))
                    return null;

                var img = this._ImageReference.Target as MemoryImage;

                return img;
            }
        }

        public void RegetImage()
        {
            this._ImageReference.Target = null;
            this.GetImageAsync(true);
        }

        //~ImageListViewItem()
        //    if (this.Image IsNot null)
        //    {
        //        this.Image.Dispose()
        //        this.Image = null
        //    }
        //    MyBase.Finalize()
        //}
    }
}
