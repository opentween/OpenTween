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
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.IO;

namespace OpenTween
{
    public class OTPictureBox : PictureBox
    {
        [Localizable(true)]
        public new Image Image
        {
            get { return base.Image; }
            set
            {
                base.Image = value;
                this.SizeMode = this._SizeMode;
                if (this.memoryImage != null)
                {
                    this.memoryImage.Dispose();
                    this.memoryImage = null;
                }
            }
        }

        [Localizable(true)]
        public new string ImageLocation
        {
            get { return this._ImageLocation; }
            set
            {
                if (value == null)
                {
                    this.Image = null;
                    return;
                }
                this.LoadAsync(value);
            }
        }
        private string _ImageLocation;

        /// <summary>
        /// 画像に応じた SizeMode を取得・設定する
        /// </summary>
        /// <remarks>
        /// ErrorImage と InitialImage は SizeMode の値に依らず中央等倍に表示する必要があるため、
        /// 画像に応じて SizeMode の状態を弄る
        /// </remarks>
        public new PictureBoxSizeMode SizeMode
        {
            get { return this._SizeMode; }
            set
            {
                this._SizeMode = value;
                if (base.Image == null || (base.Image != base.ErrorImage && base.Image != base.InitialImage))
                {
                    base.SizeMode = value;
                }
                else
                {
                    base.SizeMode = PictureBoxSizeMode.CenterImage;
                }
            }
        }
        private PictureBoxSizeMode _SizeMode;

        /// <summary>
        /// ImageLocation によりロードされた画像
        /// </summary>
        private MemoryImage memoryImage = null;

        private Task loadAsyncTask = null;
        private CancellationTokenSource loadAsyncCancelTokenSource = null;

        public new Task LoadAsync(string url)
        {
            this._ImageLocation = url;

            if (this.loadAsyncTask != null && !this.loadAsyncTask.IsCompleted)
                this.CancelAsync();

            this.Image = base.InitialImage;

            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch (UriFormatException)
            {
                uri = new Uri(Path.GetFullPath(url));
            }

            var client = new OTWebClient();

            client.DownloadProgressChanged += (s, e) =>
            {
                this.OnLoadProgressChanged(new ProgressChangedEventArgs(e.ProgressPercentage, e.UserState));
            };

            this.loadAsyncCancelTokenSource = new CancellationTokenSource();
            var cancelToken = this.loadAsyncCancelTokenSource.Token;
            var loadImageTask = client.DownloadDataAsync(uri, cancelToken);

            // UnobservedTaskException イベントを発生させないようにする
            loadImageTask.ContinueWith(t => { var ignore = t.Exception; }, TaskContinuationOptions.OnlyOnFaulted);

            var uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            return loadImageTask.ContinueWith(t => {
                client.Dispose();

                if (t.IsFaulted) throw t.Exception;

                return MemoryImage.CopyFromBytes(t.Result);
            }, cancelToken)
            .ContinueWith(t =>
            {
                if (!t.IsCanceled)
                {
                    if (t.IsFaulted)
                    {
                        this.Image = base.ErrorImage;
                    }
                    else
                    {
                        this.Image = t.Result.Image;
                        this.memoryImage = t.Result;
                    }
                }

                var exp = t.Exception != null ? t.Exception.Flatten() : null;
                this.OnLoadCompleted(new AsyncCompletedEventArgs(exp, t.IsCanceled, null));
            }, uiScheduler);
        }

        public new void CancelAsync()
        {
            if (this.loadAsyncTask == null || this.loadAsyncTask.IsCompleted)
                return;

            this.loadAsyncCancelTokenSource.Cancel();

            try
            {
                this.loadAsyncTask.Wait();
            }
            catch (AggregateException ae)
            {
                ae.Handle(e =>
                {
                    if (e is OperationCanceledException)
                        return true;
                    if (e is WebException)
                        return true;

                    return false;
                });
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (this.memoryImage != null)
                this.memoryImage.Dispose();

            if (this.loadAsyncCancelTokenSource != null)
                this.loadAsyncCancelTokenSource.Dispose();
        }
    }
}
