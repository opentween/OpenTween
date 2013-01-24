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
                if (base.Image != null && this.imageFromLoadMethod)
                {
                    this.DisposeImageFromStream();
                    this.imageFromLoadMethod = false;
                }
                base.Image = value;
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

        private bool imageFromLoadMethod = false;
        private Stream imageStream = null;

        private Task loadAsyncTask = null;
        private CancellationTokenSource loadAsyncCancelTokenSource = null;

        public new Task LoadAsync(string url)
        {
            this._ImageLocation = url;

            if (this.loadAsyncTask != null && !this.loadAsyncTask.IsCompleted)
                this.CancelAsync();

            if (this.expandedInitialImage != null)
                this.Image = this.expandedInitialImage;

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
                if (t.IsFaulted) throw t.Exception;

                var bytes = t.Result;

                // InitialImageの表示時に DisposeImageFromStream() が呼ばれるためここでは不要

                this.imageStream = new MemoryStream(bytes);
                this.imageStream.Write(bytes, 0, bytes.Length);

                return Image.FromStream(this.imageStream, true, true);
            }, cancelToken)
            .ContinueWith(t =>
            {
                if (!t.IsCanceled)
                {
                    if (t.IsFaulted)
                    {
                        this.Image = this.expandedErrorImage;
                    }
                    else
                    {
                        this.Image = t.Result;
                        this.imageFromLoadMethod = true;
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

        /// <summary>
        /// LoadAsync メソッドで取得した画像の破棄
        /// </summary>
        protected void DisposeImageFromStream()
        {
            if (this.Image != null)
                this.Image.Dispose();

            if (this.imageStream != null)
                this.imageStream.Dispose();
        }

        public new Image ErrorImage
        {
            get { return base.ErrorImage; }
            set
            {
                base.ErrorImage = value;
                this.UpdateStatusImages();
            }
        }

        public new Image InitialImage
        {
            get { return base.InitialImage; }
            set
            {
                base.InitialImage = value;
                this.UpdateStatusImages();
            }
        }

        private Image expandedErrorImage = null;
        private Image expandedInitialImage = null;

        /// <summary>
        /// ErrorImage と InitialImage の表示用の画像を生成する
        /// </summary>
        /// <remarks>
        /// ErrorImage と InitialImage は SizeMode の値に依らず中央等倍に表示する必要があるため、
        /// 事前にコントロールのサイズに合わせた画像を生成しておく
        /// </remarks>
        private void UpdateStatusImages()
        {
            var isError = false;
            var isInit = false;

            if (this.Image != null)
            {
                // ErrorImage か InitialImage を使用中であれば記憶しておく
                isError = (this.Image == this.expandedErrorImage);
                isInit = (this.Image == this.expandedInitialImage);
            }

            if (isError || isInit)
                this.Image = null;

            if (this.expandedErrorImage != null)
                this.expandedErrorImage.Dispose();

            if (this.expandedInitialImage != null)
                this.expandedInitialImage.Dispose();

            this.expandedErrorImage = this.ExpandImage(this.ErrorImage);
            this.expandedInitialImage = this.ExpandImage(this.InitialImage);

            if (isError)
                this.Image = this.expandedErrorImage;

            if (isInit)
                this.Image = this.expandedInitialImage;
        }

        private Image ExpandImage(Image image)
        {
            if (image == null) return null;

            var bitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);

            using (var g = this.CreateGraphics())
            {
                bitmap.SetResolution(g.DpiX, g.DpiY);
            }

            using (var g = Graphics.FromImage(bitmap))
            {
                var posx = (bitmap.Width - image.Width) / 2;
                var posy = (bitmap.Height - image.Height) / 2;

                g.DrawImage(image,
                    new Rectangle(posx, posy, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height),
                    GraphicsUnit.Pixel);
            }

            return bitmap;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.UpdateStatusImages();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (this.expandedErrorImage != null)
                this.expandedErrorImage.Dispose();

            if (this.expandedInitialImage != null)
                this.expandedInitialImage.Dispose();

            if (this.imageFromLoadMethod)
                this.DisposeImageFromStream();
        }
    }
}
