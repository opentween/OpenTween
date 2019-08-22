// OpenTween - Client of Twitter
// Copyright (c) 2018 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using OpenTween.Connection;
using OpenTween.Thumbnail;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTween.Models
{
    public sealed class MediaViewerLight : NotifyPropertyChangedBase, IDisposable
    {
        public ThumbnailInfo[] MediaItems
        {
            get => this._mediaItems;
            private set => this.SetProperty(ref this._mediaItems, value);
        }
        private ThumbnailInfo[] _mediaItems = new ThumbnailInfo[0];

        public int DisplayMediaIndex
        {
            get => this._displayMediaIndex;
            private set => this.SetProperty(ref this._displayMediaIndex, value);
        }
        private int _displayMediaIndex;

        public ThumbnailInfo DisplayMedia
            => this.MediaItems[this.DisplayMediaIndex];

        public string ImageUrl
        {
            get => this._imageUrl;
            private set => this.SetProperty(ref this._imageUrl, value);
        }
        private string _imageUrl;

        public MemoryImage Image
        {
            get => this._image;
            private set => this.SetProperty(ref this._image, value);
        }
        private MemoryImage _image;

        public LoadStateEnum LoadState
        {
            get => this._loadState;
            private set => this.SetProperty(ref this._loadState, value);
        }
        private LoadStateEnum _loadState;

        public long? ImageSize
        {
            get => this._imageSize;
            private set => this.SetProperty(ref this._imageSize, value);
        }
        private long? _imageSize;

        public long? ReceivedSize
        {
            get => this._receivedSize;
            private set => this.SetProperty(ref this._receivedSize, value);
        }
        private long? _receivedSize;

        private CancellationTokenSource cts;

        public enum LoadStateEnum
        {
            BeforeLoad = 0,
            HeaderArrived = 1,
            LoadSuccessed = 2,
            LoadError = 3,
        }

        public void SetMediaItems(ThumbnailInfo[] thumbnails)
        {
            this.DisplayMediaIndex = 0;
            this.MediaItems = thumbnails;
        }

        public async Task SelectMedia(int displayIndex)
        {
            this.DisplayMediaIndex = displayIndex;

            var media = this.MediaItems[displayIndex];
            await this.LoadAsync(media);
        }

        public async Task SelectPreviousMedia()
        {
            var currentIndex = this.DisplayMediaIndex;
            if (currentIndex == 0)
                return;

            await this.SelectMedia(currentIndex - 1);
        }

        public async Task SelectNextMedia()
        {
            var currentIndex = this.DisplayMediaIndex;
            if (currentIndex == this.MediaItems.Length - 1)
                return;

            await this.SelectMedia(currentIndex + 1);
        }

        internal async Task LoadAsync(ThumbnailInfo media)
        {
            var newCts = new CancellationTokenSource();
            var oldCts = Interlocked.Exchange(ref this.cts, newCts);
            if (oldCts != null)
            {
                oldCts.Cancel();
                oldCts.Dispose();
            }

            var imageUrl = media.FullSizeImageUrl ?? media.ThumbnailImageUrl;
            if (imageUrl != null)
            {
                await this.LoadAsync(imageUrl, newCts.Token);
            }
            else
            {
                await this.LoadAsync(() => media.LoadThumbnailImageAsync(newCts.Token));
            }
        }

        internal async Task LoadAsync(string imageUrl, CancellationToken cancellationToken)
        {
            try
            {
                this.ImageUrl = imageUrl;
                this.Image = null;
                this.ImageSize = null;
                this.ReceivedSize = null;
                this.LoadState = LoadStateEnum.BeforeLoad;

                using (var response = await Networking.Http.GetAsync(this.ImageUrl,
                    HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();

                    this.ImageSize = response.Content.Headers.ContentLength;
                    this.ReceivedSize = 0;
                    this.LoadState = LoadStateEnum.HeaderArrived;

                    var initialSize = (int?)this.ImageSize ?? 2 * 1024 * 1024;
                    using (var memstream = new MemoryStream(initialSize))
                    {
                        using (var responseStream = await response.Content.ReadAsStreamAsync())
                        {
                            var bufferSize = 1 * 1024 * 1024;
                            var buffer = new byte[bufferSize];

                            int received;
                            while ((received = await responseStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
                            {
                                memstream.Write(buffer, 0, received);

                                this.ReceivedSize += received;
                            }
                        }

                        memstream.Position = 0;
                        this.Image = MemoryImage.CopyFromStream(memstream);
                    }
                }

                this.LoadState = LoadStateEnum.LoadSuccessed;
            }
            catch (Exception)
            {
                this.LoadState = LoadStateEnum.LoadError;

                try
                {
                    throw;
                }
                catch (HttpRequestException) { }
                catch (InvalidImageException) { }
                catch (OperationCanceledException) { }
                catch (IOException) { }
            }
        }

        internal async Task LoadAsync(Func<Task<MemoryImage>> imageTaskFunc)
        {
            try
            {
                this.ImageUrl = null;
                this.Image = null;
                this.ImageSize = null;
                this.ReceivedSize = null;
                this.LoadState = LoadStateEnum.BeforeLoad;

                var image = await imageTaskFunc();

                this.Image = image;
                this.LoadState = LoadStateEnum.LoadSuccessed;
            }
            catch (Exception)
            {
                this.LoadState = LoadStateEnum.LoadError;

                try
                {
                    throw;
                }
                catch (HttpRequestException) { }
                catch (InvalidImageException) { }
                catch (OperationCanceledException) { }
                catch (IOException) { }
            }
        }

        public void AbortLoad()
        {
            var oldCts = Interlocked.Exchange(ref this.cts, null);
            if (oldCts != null)
            {
                oldCts.Cancel();
                oldCts.Dispose();
            }
        }

        public void Dispose()
        {
            this.cts?.Dispose();
            this.Image?.Dispose();
        }
    }
}
