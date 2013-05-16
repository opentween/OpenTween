﻿// OpenTween - Client of Twitter
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OpenTween.Thumbnail;
using System.Threading;

namespace OpenTween
{
    public partial class TweetThumbnail : UserControl
    {
        protected internal List<OTPictureBox> pictureBox = new List<OTPictureBox>();

        private Task task = null;
        private CancellationTokenSource cancelTokenSource;

        public event EventHandler ThumbnailLoading;
        public event EventHandler<ThumbnailDoubleClickEventArgs> ThumbnailDoubleClick;
        public event EventHandler<ThumbnailImageSearchEventArgs> ThumbnailImageSearchClick;

        public ThumbnailInfo Thumbnail
        {
            get { return this.pictureBox[this.scrollBar.Value].Tag as ThumbnailInfo; }
        }

        public TweetThumbnail()
        {
            InitializeComponent();
        }

        public Task ShowThumbnailAsync(PostClass post)
        {
            this.CancelAsync();

            this.scrollBar.Enabled = false;

            this.cancelTokenSource = new CancellationTokenSource();
            var cancelToken = this.cancelTokenSource.Token;

            this.task = Task.Factory.StartNew(() => this.GetThumbailInfo(post), cancelToken)
                .ContinueWith( /* await使いたい */
                    t =>
                    {
                        var thumbnails = t.Result;

                        this.SetThumbnailCount(thumbnails.Count);
                        if (thumbnails.Count == 0) return;

                        for (int i = 0; i < thumbnails.Count; i++)
                        {
                            var thumb = thumbnails[i];
                            var picbox = this.pictureBox[i];

                            picbox.Tag = thumb;
                            picbox.LoadAsync(thumb.ThumbnailUrl);
                            picbox.ContextMenu = CreateContextMenu(thumb);

                            var tooltipText = thumb.TooltipText;
                            if (!string.IsNullOrEmpty(tooltipText))
                            {
                                this.toolTip.SetToolTip(picbox, tooltipText);
                            }

                            cancelToken.ThrowIfCancellationRequested();
                        }

                        this.scrollBar.Maximum = thumbnails.Count - 1;

                        if (thumbnails.Count > 1)
                            this.scrollBar.Enabled = true;

                        if (this.ThumbnailLoading != null)
                            this.ThumbnailLoading(this, new EventArgs());
                    },
                    cancelToken,
                    TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.FromCurrentSynchronizationContext()
                );

            return this.task;
        }

        private ContextMenu CreateContextMenu(ThumbnailInfo thumb)
        {
            var contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(CreateImageSearchMenuItem(thumb));
            return contextMenu;
        }

        private MenuItem CreateImageSearchMenuItem(ThumbnailInfo thumb)
        {
            var item = new MenuItem();
            item.Text = Properties.Resources.SearchSimilarImageText;
            string search_targe_url =
                thumb.FullSizeImageUrl != null
                    ? thumb.FullSizeImageUrl
                    : thumb.ThumbnailUrl != null
                        ? thumb.ThumbnailUrl
                        : null;

            if (search_targe_url != null)
            {
                item.Click += (sender, e) =>
                {
                    string uri = GetImageSearchUri(search_targe_url);
                    if (this.ThumbnailImageSearchClick != null)
                    {
                        this.ThumbnailImageSearchClick(this, new ThumbnailImageSearchEventArgs(uri));
                    }
                };
            }
            else
            {
                item.Enabled = false;
            }

            return item;
        }

        private string GetImageSearchUri(string image_uri)
        {
            return @"https://www.google.com/searchbyimage?image_url=" + Uri.EscapeDataString(image_uri);
        }

        protected virtual List<ThumbnailInfo> GetThumbailInfo(PostClass post)
        {
            return ThumbnailGenerator.GetThumbnails(post);
        }

        public void CancelAsync()
        {
            if (this.task == null || this.task.IsCompleted) return;

            this.cancelTokenSource.Cancel();

            // this.task.Status は、GetThumbailInfo() の実行中であれば TaskStatus.WaitingForActivation となる。
            // ContinueWith の処理も含めて終了していれば RanToCompletion などになる。
            // もしこれが Running である場合は、PictureBox に対する操作の途中である可能性が高いため
            // 必ず Wait() を実行してタスクの終了を待つ。
            // (ContinueWith のタスクは ThumbnailLoading イベントが足を引っ張らない限り20ms程で完了する)

            if (this.task.Status != TaskStatus.Running) return;

            try
            {
                this.task.Wait();
            }
            catch (AggregateException ae)
            {
                ae.Handle(e => e is TaskCanceledException);
            }
        }

        /// <summary>
        /// 表示するサムネイルの数を設定する
        /// </summary>
        /// <param name="count">表示するサムネイルの数</param>
        protected void SetThumbnailCount(int count)
        {
            this.SuspendLayout();

            this.scrollBar.Maximum = count;

            this.panelPictureBox.Controls.Clear();
            foreach (var picbox in this.pictureBox)
            {
                picbox.Dispose();
            }
            this.pictureBox.Clear();

            for (int i = 0; i < count; i++)
            {
                var picbox = CreatePictureBox("pictureBox" + i);
                picbox.Visible = (i == 0);
                picbox.DoubleClick += this.pictureBox_DoubleClick;

                this.panelPictureBox.Controls.Add(picbox);
                this.pictureBox.Add(picbox);
            }

            this.ResumeLayout(false);
        }

        protected virtual OTPictureBox CreatePictureBox(string name)
        {
            return new OTPictureBox()
            {
                Name = name,
                SizeMode = PictureBoxSizeMode.Zoom,
                WaitOnLoad = false,
                Dock = DockStyle.Fill,
            };
        }

        public void ScrollUp()
        {
            var newval = this.scrollBar.Value + this.scrollBar.SmallChange;

            if (newval > this.scrollBar.Maximum)
                newval = this.scrollBar.Maximum;

            this.scrollBar.Value = newval;
        }

        public void ScrollDown()
        {
            var newval = this.scrollBar.Value - this.scrollBar.SmallChange;

            if (newval < this.scrollBar.Minimum)
                newval = this.scrollBar.Minimum;

            this.scrollBar.Value = newval;
        }

        private void scrollBar_ValueChanged(object sender, EventArgs e)
        {
            this.SuspendLayout();

            var value = this.scrollBar.Value;
            for (var i = 0; i < this.pictureBox.Count; i++)
            {
                this.pictureBox[i].Visible = (i == value);
            }

            this.ResumeLayout(false);
        }

        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            var thumb = ((PictureBox)sender).Tag as ThumbnailInfo;

            if (thumb == null) return;

            if (this.ThumbnailDoubleClick != null)
            {
                this.ThumbnailDoubleClick(this, new ThumbnailDoubleClickEventArgs(thumb));
            }
        }
    }

    public class ThumbnailDoubleClickEventArgs : EventArgs
    {
        public ThumbnailInfo Thumbnail { get; private set; }

        public ThumbnailDoubleClickEventArgs(ThumbnailInfo thumbnail)
        {
            this.Thumbnail = thumbnail;
        }
    }

    public class ThumbnailImageSearchEventArgs : EventArgs
    {
        public string ImageUrl { get; private set; }

        public ThumbnailImageSearchEventArgs(string url)
        {
            this.ImageUrl = url;
        }
    }
}
