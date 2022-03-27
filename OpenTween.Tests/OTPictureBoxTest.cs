// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xunit;

namespace OpenTween
{
    public class OTPictureBoxTest
    {
        [Fact]
        public void SizeMode_SetterGetterTest()
        {
            using var picbox = new OTPictureBox();
            picbox.SizeMode = PictureBoxSizeMode.Zoom;

            Assert.Equal(PictureBoxSizeMode.Zoom, picbox.SizeMode);
            Assert.Equal(PictureBoxSizeMode.Zoom, ((PictureBox)picbox).SizeMode);
        }

        [Fact]
        public void SizeMode_ErrorImageTest()
        {
            using var picbox = new OTPictureBox();
            picbox.SizeMode = PictureBoxSizeMode.Zoom;

            picbox.ShowErrorImage();

            Assert.Equal(PictureBoxSizeMode.Zoom, picbox.SizeMode);
            Assert.Equal(PictureBoxSizeMode.CenterImage, ((PictureBox)picbox).SizeMode);
        }

        [Fact]
        public void SizeMode_ErrorImageTest2()
        {
            using var picbox = new OTPictureBox();
            picbox.ShowErrorImage();

            picbox.SizeMode = PictureBoxSizeMode.Zoom;

            Assert.Equal(PictureBoxSizeMode.Zoom, picbox.SizeMode);
            Assert.Equal(PictureBoxSizeMode.CenterImage, ((PictureBox)picbox).SizeMode);
        }

        [Fact]
        public void SizeMode_RestoreTest()
        {
            using var picbox = new OTPictureBox();
            picbox.SizeMode = PictureBoxSizeMode.Zoom;

            picbox.ShowErrorImage();

            picbox.Image = TestUtils.CreateDummyImage();

            Assert.Equal(PictureBoxSizeMode.Zoom, picbox.SizeMode);
            Assert.Equal(PictureBoxSizeMode.Zoom, ((PictureBox)picbox).SizeMode);
        }

        [Fact]
        public async Task SetImageFromAsync_Test()
        {
            using var picbox = new OTPictureBox();

            // Mono でのテスト実行時にデッドロックする問題の対策
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var tcs = new TaskCompletionSource<MemoryImage>();

            var setImageTask = picbox.SetImageFromTask(() => tcs.Task);

            Assert.Equal(picbox.InitialImage, ((PictureBox)picbox).Image);

            var image = TestUtils.CreateDummyImage();
            tcs.SetResult(image);
            await setImageTask;

            Assert.Equal(image, picbox.Image);
        }

        [Fact]
        public async Task SetImageFromAsync_ErrorTest()
        {
            using var picbox = new OTPictureBox();

            // Mono でのテスト実行時にデッドロックする問題の対策
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            var tcs = new TaskCompletionSource<MemoryImage>();

            var setImageTask = picbox.SetImageFromTask(() => tcs.Task);

            Assert.Equal(picbox.InitialImage, ((PictureBox)picbox).Image);

            tcs.SetException(new InvalidImageException());
            await setImageTask;

            Assert.Equal(picbox.ErrorImage, ((PictureBox)picbox).Image);
        }
    }
}
