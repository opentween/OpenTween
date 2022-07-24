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

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Thumbnail;

namespace OpenTween
{
    public class OTPictureBox : PictureBox
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new MemoryImage? Image
        {
            get => this.memoryImage;
            set
            {
                this.memoryImage = value;
                base.Image = value?.Image;

                this.RestoreSizeMode();
            }
        }

        private MemoryImage? memoryImage;

        [Localizable(true)]
        [DefaultValue(PictureBoxSizeMode.Normal)]
        public new PictureBoxSizeMode SizeMode
        {
            get => this.currentSizeMode;
            set
            {
                this.currentSizeMode = value;

                if (base.Image != base.InitialImage && base.Image != base.ErrorImage)
                {
                    base.SizeMode = value;
                }
            }
        }

        /// <summary>
        /// InitialImage や ErrorImage の表示に SizeMode を一時的に変更するため、
        /// 現在の SizeMode を記憶しておくためのフィールド
        /// </summary>
        private PictureBoxSizeMode currentSizeMode;

        public void ShowInitialImage()
        {
            base.Image = base.InitialImage;

            // InitialImage は SizeMode の値に依らず中央等倍に表示する必要がある
            base.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        public void ShowErrorImage()
        {
            base.Image = base.ErrorImage;

            // ErrorImage は SizeMode の値に依らず中央等倍に表示する必要がある
            base.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void RestoreSizeMode()
            => base.SizeMode = this.currentSizeMode;

        /// <summary>
        /// SetImageFromTask メソッドを連続で呼び出した際に設定される画像が前後するのを防ぐため、
        /// 現在進行中の Task を表す Id を記憶しておくためのフィールド
        /// </summary>
        private int currentImageTaskId = 0;

        public async Task SetImageFromTask(Func<Task<MemoryImage>> imageTask, bool useStatusImage = true)
        {
            var id = Interlocked.Increment(ref this.currentImageTaskId);

            try
            {
                if (useStatusImage)
                    this.ShowInitialImage();

                var image = await imageTask();

                if (id == this.currentImageTaskId)
                    this.Image = image;
            }
            catch (Exception)
            {
                if (id == this.currentImageTaskId && useStatusImage)
                    this.ShowErrorImage();
                try
                {
                    throw;
                }
                catch (HttpRequestException)
                {
                }
                catch (InvalidImageException)
                {
                }
                catch (OperationCanceledException)
                {
                }
                catch (ObjectDisposedException)
                {
                }
                catch (WebException)
                {
                }
                catch (IOException)
                {
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            try
            {
                base.OnPaint(pe);

                // 動画なら再生ボタンを上から描画
                this.DrawPlayableMark(pe);
            }
            catch (ExternalException)
            {
                // アニメーション GIF 再生中に発生するエラーの対策
                // 参照: https://sourceforge.jp/ticket/browse.php?group_id=6526&tid=32894
                this.ShowErrorImage();
            }
        }

        private void DrawPlayableMark(PaintEventArgs pe)
        {
            if (!(this.Tag is ThumbnailInfo thumb && thumb.IsPlayable)) return;
            if (base.Image == base.InitialImage || base.Image == base.ErrorImage) return;

            var overlayImage = Properties.Resources.PlayableOverlayImage;

            var overlaySize = Math.Min(this.Width, this.Height) / 4;
            var destRect = new Rectangle(
                (this.Width - overlaySize) / 2,
                (this.Height - overlaySize) / 2,
                overlaySize,
                overlaySize);

            pe.Graphics.DrawImage(overlayImage, destRect, 0, 0, overlayImage.Width, overlayImage.Height, GraphicsUnit.Pixel);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string? ImageLocation
        {
            get => null;
            set { }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new void Load(string url)
            => throw new NotSupportedException();
    }
}
