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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Models;
using OpenTween.Thumbnail;

namespace OpenTween
{
    public class TweetThumbnail : NotifyPropertyChangedBase
    {
        private ThumbnailGenerator? thumbGenerator;
        private bool thumbnailAvailable;
        private PostId? currentPostId;
        private ThumbnailInfo[] thumbnails = Array.Empty<ThumbnailInfo>();
        private Task<MemoryImage>?[] loadImageTasks = Array.Empty<Task<MemoryImage>?>();
        private int selectedIndex = 0;

        public bool ThumbnailAvailable
        {
            get => this.thumbnailAvailable;
            set => this.SetProperty(ref this.thumbnailAvailable, value);
        }

        public int SelectedIndex
        {
            get => this.selectedIndex;
            set
            {
                if (this.ThumbnailAvailable)
                {
                    if (value < 0 || value >= this.Thumbnails.Length)
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
                else
                {
                    if (value != 0)
                        throw new ArgumentOutOfRangeException(nameof(value));
                }
                this.SetProperty(ref this.selectedIndex, value);
            }
        }

        public ThumbnailInfo[] Thumbnails
            => this.ThumbnailAvailable ? this.thumbnails : throw new InvalidOperationException("Thumbnail is not available.");

        public ThumbnailInfo CurrentThumbnail
            => this.Thumbnails[this.selectedIndex];

        private ThumbnailGenerator ThumbGenerator
            => this.thumbGenerator ?? throw this.NotInitializedException();

        public void Initialize(ThumbnailGenerator thumbnailGenerator)
            => this.thumbGenerator = thumbnailGenerator;

        public async Task PrepareThumbnails(PostClass post, CancellationToken token)
        {
            if (this.currentPostId == post.StatusId)
                return;

            this.currentPostId = post.StatusId;
            this.ThumbnailAvailable = false;

            var thumbnails = (await this.GetThumbailInfoAsync(post, token)).ToArray();

            if (this.currentPostId != post.StatusId)
                return;

            this.DisposeImages();
            this.SelectedIndex = 0;
            this.thumbnails = thumbnails;
            this.loadImageTasks = new Task<MemoryImage>?[thumbnails.Length];

            if (thumbnails.Length > 0)
                this.ThumbnailAvailable = true;
        }

        public Task<MemoryImage> LoadSelectedThumbnail()
        {
            var runningTask = this.loadImageTasks[this.selectedIndex];
            if (runningTask != null)
                return runningTask;

            var newTask = Task.Run(() => this.thumbnails[this.selectedIndex].LoadThumbnailImageAsync());
            this.loadImageTasks[this.selectedIndex] = newTask;

            return newTask;
        }

        public string GetUrlForImageSearch()
            => this.CurrentThumbnail.FullSizeImageUrl ?? this.CurrentThumbnail.ThumbnailImageUrl;

        public Uri GetImageSearchUriGoogle()
        {
            var imageUrl = this.GetUrlForImageSearch();
            return new(@"https://lens.google.com/uploadbyurl?url=" + Uri.EscapeDataString(imageUrl));
        }

        public Uri GetImageSearchUriSauceNao()
        {
            var imageUrl = this.GetUrlForImageSearch();
            return new(@"https://saucenao.com/search.php?url=" + Uri.EscapeDataString(imageUrl));
        }

        public void ScrollUp()
        {
            if (!this.ThumbnailAvailable)
                return;
            if (this.SelectedIndex == 0)
                return;

            this.SelectedIndex--;
        }

        public void ScrollDown()
        {
            if (!this.ThumbnailAvailable)
                return;
            if (this.SelectedIndex == this.Thumbnails.Length - 1)
                return;

            this.SelectedIndex++;
        }

        private Exception NotInitializedException()
            => new InvalidOperationException("Cannot call before initialization");

        private async Task<IEnumerable<ThumbnailInfo>> GetThumbailInfoAsync(PostClass post, CancellationToken token)
            => await Task.Run(() => this.ThumbGenerator.GetThumbnailsAsync(post, token));

        private void DisposeImages()
        {
            var oldImageTasks = this.loadImageTasks.OfType<Task<MemoryImage>>().ToArray();
            _ = AsyncExceptionBoundary.IgnoreExceptionAndDispose(oldImageTasks);
        }
    }
}
