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
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace OpenTween
{
    /// <summary>
    /// Image と Stream を対に保持するためのクラス
    /// </summary>
    /// <remarks>
    /// Image.FromStream() を使用して Image を生成する場合、
    /// Image を破棄するまでの間は元となった Stream を破棄できないためその対策として使用する。
    /// </remarks>
    public class MemoryImage : IDisposable
    {
        public readonly Stream Stream;
        public readonly Image Image;

        protected bool disposed = false;

        protected MemoryImage(Stream stream)
        {
            this.Stream = stream;
            this.Image = Image.FromStream(stream);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;

            if (disposing)
            {
                this.Image.Dispose();
                this.Stream.Dispose();
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);

            // 明示的にDisposeが呼ばれた場合はファイナライザを使用しない
            GC.SuppressFinalize(this);
        }

        ~MemoryImage()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 指定された Stream から MemoryImage を作成します。
        /// </summary>
        /// <remarks>
        /// ストリームの内容はメモリ上に展開した後に使用されるため、
        /// 引数に指定した Stream を MemoryImage より先に破棄しても問題ありません。
        /// </remarks>
        /// <param name="stream">読み込む対象となる Stream</param>
        /// <returns>作成された MemoryImage</returns>
        public static MemoryImage CopyFromStream(Stream stream)
        {
            var memstream = new MemoryStream();

            var buffer = new byte[1024];
            int length;
            while ((length = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                memstream.Write(buffer, 0, length);
            }

            return new MemoryImage(memstream);
        }

        /// <summary>
        /// 指定されたバイト列から MemoryImage を作成します。
        /// </summary>
        /// <param name="stream">読み込む対象となるバイト列</param>
        /// <returns>作成された MemoryImage</returns>
        public static MemoryImage CopyFromBytes(byte[] bytes)
        {
            return new MemoryImage(new MemoryStream(bytes));
        }
    }
}
