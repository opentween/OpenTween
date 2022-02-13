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

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenTween.Thumbnail;
using System.Threading;
using OpenTween.Models;
using System.Runtime.InteropServices;

namespace OpenTween
{
    public partial class TweetThumbnail : UserControl
    {
        protected internal List<OTPictureBox> pictureBox = new List<OTPictureBox>();
        protected MouseWheelMessageFilter filter = new MouseWheelMessageFilter();

        public event EventHandler<EventArgs>? ThumbnailLoading;
        public event EventHandler<ThumbnailDoubleClickEventArgs>? ThumbnailDoubleClick;
        public event EventHandler<ThumbnailImageSearchEventArgs>? ThumbnailImageSearchClick;

        public ThumbnailInfo Thumbnail
            => (ThumbnailInfo)this.pictureBox[this.scrollBar.Value].Tag;

        public TweetThumbnail()
            => this.InitializeComponent();

        public Task ShowThumbnailAsync(PostClass post)
            => this.ShowThumbnailAsync(post, CancellationToken.None);

        public async Task ShowThumbnailAsync(PostClass post, CancellationToken cancelToken)
        {
            var loadTasks = new List<Task>();

            this.scrollBar.Enabled = false;
            this.scrollBar.Visible = false;

            if (post.ExpandedUrls.Length == 0 && post.Media.Count == 0 && post.PostGeo == null)
            {
                this.SetThumbnailCount(0);
                return;
            }

            var thumbnails = (await this.GetThumbailInfoAsync(post, cancelToken))
                .ToArray();

            cancelToken.ThrowIfCancellationRequested();

            this.SetThumbnailCount(thumbnails.Length);
            if (thumbnails.Length == 0)
                return;

            for (var i = 0; i < thumbnails.Length; i++)
            {
                var thumb = thumbnails[i];
                var picbox = this.pictureBox[i];

                picbox.Tag = thumb;
                picbox.ContextMenuStrip = this.contextMenuStrip;

                var loadTask = picbox.SetImageFromTask(() => thumb.LoadThumbnailImageAsync(cancelToken));
                loadTasks.Add(loadTask);

                var tooltipText = thumb.TooltipText;
                if (!MyCommon.IsNullOrEmpty(tooltipText))
                {
                    this.toolTip.SetToolTip(picbox, tooltipText);
                    picbox.AccessibleDescription = tooltipText;
                }

                cancelToken.ThrowIfCancellationRequested();
            }

            if (thumbnails.Length > 1)
            {
                this.scrollBar.Enabled = true;
                this.scrollBar.Visible = true;
            }

            this.ThumbnailLoading?.Invoke(this, EventArgs.Empty);

            await Task.WhenAll(loadTasks).ConfigureAwait(false);
        }

        private string GetImageSearchUriGoogle(string image_uri)
            => @"https://www.google.com/searchbyimage?image_url=" + Uri.EscapeDataString(image_uri);

        private string GetImageSearchUriSauceNao(string imageUri)
            => @"https://saucenao.com/search.php?url=" + Uri.EscapeDataString(imageUri);

        protected virtual Task<IEnumerable<ThumbnailInfo>> GetThumbailInfoAsync(PostClass post, CancellationToken token)
            => ThumbnailGenerator.GetThumbnailsAsync(post, token);

        /// <summary>
        /// 表示するサムネイルの数を設定する
        /// </summary>
        /// <param name="count">表示するサムネイルの数</param>
        protected void SetThumbnailCount(int count)
        {
            if (count == 0 && this.pictureBox.Count == 0)
                return;

            using (ControlTransaction.Layout(this.panelPictureBox, false))
            {
                this.panelPictureBox.Controls.Clear();
                foreach (var picbox in this.pictureBox)
                {
                    var memoryImage = picbox.Image;

                    this.filter.Unregister(picbox);

                    picbox.MouseWheel -= this.pictureBox_MouseWheel;
                    picbox.DoubleClick -= this.pictureBox_DoubleClick;
                    picbox.Dispose();

                    memoryImage?.Dispose();

                    // メモリリーク対策 (http://stackoverflow.com/questions/2792427#2793714)
                    picbox.ContextMenuStrip = null;
                }
                this.pictureBox.Clear();

                this.scrollBar.Maximum = (count > 0) ? count - 1 : 0;
                this.scrollBar.Value = 0;

                for (var i = 0; i < count; i++)
                {
                    var picbox = this.CreatePictureBox("pictureBox" + i);
                    picbox.Visible = (i == 0);
                    picbox.MouseWheel += this.pictureBox_MouseWheel;
                    picbox.DoubleClick += this.pictureBox_DoubleClick;

                    this.filter.Register(picbox);

                    this.panelPictureBox.Controls.Add(picbox);
                    this.pictureBox.Add(picbox);
                }
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        protected virtual OTPictureBox CreatePictureBox(string name)
        {
            return new OTPictureBox
            {
                Name = name,
                SizeMode = PictureBoxSizeMode.Zoom,
                WaitOnLoad = false,
                Dock = DockStyle.Fill,
                AccessibleRole = AccessibleRole.Graphic,
            };
        }

        public void OpenImage(ThumbnailInfo thumb)
            => this.ThumbnailDoubleClick?.Invoke(this, new ThumbnailDoubleClickEventArgs(thumb));

        public void ScrollUp()
        {
            var newval = this.scrollBar.Value - this.scrollBar.SmallChange;

            if (newval < this.scrollBar.Minimum)
                newval = this.scrollBar.Minimum;

            this.scrollBar.Value = newval;
        }

        public void ScrollDown()
        {
            var newval = this.scrollBar.Value + this.scrollBar.SmallChange;

            if (newval > this.scrollBar.Maximum)
                newval = this.scrollBar.Maximum;

            this.scrollBar.Value = newval;
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);
            OTBaseForm.ScaleChildControl(this.scrollBar, factor);
        }

        private void scrollBar_ValueChanged(object sender, EventArgs e)
        {
            using (ControlTransaction.Layout(this, false))
            {
                var value = this.scrollBar.Value;
                for (var i = 0; i < this.pictureBox.Count; i++)
                {
                    this.pictureBox[i].Visible = (i == value);
                }
            }
        }

        private void pictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
                this.ScrollUp();
            else
                this.ScrollDown();
        }

        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            if (((PictureBox)sender).Tag is ThumbnailInfo thumb)
                this.OpenImage(thumb);
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            var picbox = (OTPictureBox)this.contextMenuStrip.SourceControl;
            var thumb = (ThumbnailInfo)picbox.Tag;

            var searchTargetUri = thumb.FullSizeImageUrl ?? thumb.ThumbnailImageUrl ?? null;
            if (searchTargetUri != null)
            {
                this.searchImageGoogleMenuItem.Enabled = true;
                this.searchImageGoogleMenuItem.Tag = searchTargetUri;
                this.searchImageSauceNaoMenuItem.Enabled = true;
                this.searchImageSauceNaoMenuItem.Tag = searchTargetUri;
            }
            else
            {
                this.searchImageGoogleMenuItem.Enabled = false;
                this.searchImageSauceNaoMenuItem.Enabled = false;
            }
        }

        private void searchSimilarImageMenuItem_Click(object sender, EventArgs e)
        {
            var searchTargetUri = (string)this.searchImageGoogleMenuItem.Tag;
            var searchUri = this.GetImageSearchUriGoogle(searchTargetUri);

            this.ThumbnailImageSearchClick?.Invoke(this, new ThumbnailImageSearchEventArgs(searchUri));
        }

        private void searchImageSauceNaoMenuItem_Click(object sender, EventArgs e)
        {
            var searchTargetUri = (string)this.searchImageSauceNaoMenuItem.Tag;
            var searchUri = this.GetImageSearchUriSauceNao(searchTargetUri);

            this.ThumbnailImageSearchClick?.Invoke(this, new ThumbnailImageSearchEventArgs(searchUri));
        }

        private void openMenuItem_Click(object sender, EventArgs e)
            => this.OpenImage(this.Thumbnail);

        private void copyUrlMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(this.Thumbnail.FullSizeImageUrl ?? this.Thumbnail.MediaPageUrl);
            }
            catch (ExternalException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class ThumbnailDoubleClickEventArgs : EventArgs
    {
        public ThumbnailInfo Thumbnail { get; }

        public ThumbnailDoubleClickEventArgs(ThumbnailInfo thumbnail)
            => this.Thumbnail = thumbnail;
    }

    public class ThumbnailImageSearchEventArgs : EventArgs
    {
        public string ImageUrl { get; }

        public ThumbnailImageSearchEventArgs(string url)
            => this.ImageUrl = url;
    }
}
