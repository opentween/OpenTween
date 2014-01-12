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
        public new MemoryImage Image
        {
            get { return this.memoryImage; }
            set
            {
                this.memoryImage = value;
                base.Image = value.Image;

                this.RestoreSizeMode();
            }
        }
        private MemoryImage memoryImage;

        /// <summary>
        /// InitialImage や ErrorImage の表示に SizeMode を一時的に変更するため、
        /// 以前の SizeMode を記憶しておくためのフィールド
        /// </summary>
        private PictureBoxSizeMode previousSizeMode;

        public void ShowInitialImage()
        {
            if (base.Image != base.InitialImage && base.Image != base.ErrorImage)
            {
                this.previousSizeMode = this.SizeMode;
            }
            base.Image = base.InitialImage;

            // InitialImage は SizeMode の値に依らず中央等倍に表示する必要がある
            this.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        public void ShowErrorImage()
        {
            if (base.Image != base.InitialImage && base.Image != base.ErrorImage)
            {
                this.previousSizeMode = this.SizeMode;
            }
            base.Image = base.ErrorImage;

            // ErrorImage は SizeMode の値に依らず中央等倍に表示する必要がある
            this.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void RestoreSizeMode()
        {
            this.SizeMode = this.previousSizeMode;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new string ImageLocation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new void Load(string url)
        {
            throw new NotImplementedException();
        }
    }
}
