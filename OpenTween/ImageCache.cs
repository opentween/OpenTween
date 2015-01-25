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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.Xml.Serialization;
using System.Net.Http;
using OpenTween.Connection;

namespace OpenTween
{
    public class ImageCache : IDisposable
    {
        /// <summary>
        /// キャッシュとして URL と取得した画像を対に保持する辞書
        /// </summary>
        internal LRUCacheDictionary<string, Task<MemoryImage>> innerDictionary;

        /// <summary>
        /// 非同期タスクをキャンセルするためのトークンのもと
        /// </summary>
        private CancellationTokenSource cancelTokenSource;

        /// <summary>
        /// innerDictionary の排他制御のためのロックオブジェクト
        /// </summary>
        private object lockObject = new object();

        /// <summary>
        /// オブジェクトが破棄された否か
        /// </summary>
        private bool disposed = false;

        public ImageCache()
        {
            this.innerDictionary = new LRUCacheDictionary<string, Task<MemoryImage>>(trimLimit: 300, autoTrimCount: 100);
            this.innerDictionary.CacheRemoved += (s, e) => {
                // まだ参照されている場合もあるのでDisposeはファイナライザ任せ
                this.CacheRemoveCount++;
            };

            this.cancelTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// 保持しているキャッシュの件数
        /// </summary>
        public long CacheCount
        {
            get { return this.innerDictionary.Count; }
        }

        /// <summary>
        /// 破棄されたキャッシュの件数
        /// </summary>
        public int CacheRemoveCount { get; private set; }

        /// <summary>
        /// 指定された URL にある画像を非同期に取得するメソッド
        /// </summary>
        /// <param name="address">取得先の URL</param>
        /// <param name="force">キャッシュを使用せずに取得する場合は true</param>
        /// <returns>非同期に画像を取得するタスク</returns>
        public Task<MemoryImage> DownloadImageAsync(string address, bool force = false)
        {
            var cancelToken = this.cancelTokenSource.Token;

            return Task.Run(() =>
            {
                Task<MemoryImage> cachedImageTask = null;
                lock (this.lockObject)
                {
                    innerDictionary.TryGetValue(address, out cachedImageTask);

                    if (cachedImageTask != null)
                    {
                        if (force)
                        {
                            this.innerDictionary.Remove(address);
                            cachedImageTask = null;
                        }
                        else
                            return cachedImageTask;
                    }

                    cancelToken.ThrowIfCancellationRequested();

                    var imageTask = this.FetchImageAsync(address, cancelToken);
                    this.innerDictionary[address] = imageTask;

                    return imageTask;
                }
            }, cancelToken);
        }

        private async Task<MemoryImage> FetchImageAsync(string uri, CancellationToken cancelToken)
        {
            using (var response = await Networking.Http.GetAsync(uri, cancelToken).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                using (var imageStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    return await MemoryImage.CopyFromStreamAsync(imageStream)
                        .ConfigureAwait(false);
                }
            }
        }

        public MemoryImage TryGetFromCache(string address)
        {
            lock (this.lockObject)
            {
                Task<MemoryImage> imageTask;
                if (!this.innerDictionary.TryGetValue(address, out imageTask) ||
                    imageTask.Status != TaskStatus.RanToCompletion)
                    return null;

                return imageTask.Result;
            }
        }

        public void CancelAsync()
        {
            lock (this.lockObject)
            {
                var oldTokenSource = this.cancelTokenSource;
                this.cancelTokenSource = new CancellationTokenSource();

                oldTokenSource.Cancel();
                oldTokenSource.Dispose();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;

            if (disposing)
            {
                this.CancelAsync();

                lock (this.lockObject)
                {
                    foreach (var item in this.innerDictionary)
                    {
                        var task = item.Value;
                        if (task.Status == TaskStatus.RanToCompletion && task.Result != null)
                            task.Result.Dispose();
                    }

                    this.innerDictionary.Clear();
                    this.cancelTokenSource.Dispose();
                }
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ImageCache()
        {
            this.Dispose(false);
        }
    }
}
