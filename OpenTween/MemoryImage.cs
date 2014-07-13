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
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Diagnostics.CodeAnalysis;
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
    public class MemoryImage : ICloneable, IDisposable
    {
        public readonly Stream Stream;
        public readonly Image Image;

        protected bool disposed = false;

        /// <exception cref="InvalidImageException">
        /// ストリームから読みだされる画像データが不正な場合にスローされる
        /// </exception>
        protected MemoryImage(Stream stream)
        {
            try
            {
                this.Image = Image.FromStream(stream);
            }
            catch (ArgumentException e)
            {
                stream.Dispose();
                throw new InvalidImageException("Invalid image", e);
            }
            catch (OutOfMemoryException e)
            {
                // GDI+ がサポートしない画像形式で OutOfMemoryException がスローされる場合があるらしい
                stream.Dispose();
                throw new InvalidImageException("Invalid image?", e);
            }
            catch (ExternalException e)
            {
                // 「GDI+ で汎用エラーが発生しました」という大雑把な例外がスローされる場合があるらしい
                stream.Dispose();
                throw new InvalidImageException("Invalid image?", e);
            }
            catch (Exception)
            {
                stream.Dispose();
                throw;
            }

            this.Stream = stream;
        }

        /// <summary>
        /// MemoryImage インスタンスを複製します
        /// </summary>
        /// <remarks>
        /// メソッド実行中にストリームのシークが行われないよう注意して下さい。
        /// 特に PictureBox で Gif アニメーションを表示している場合は Enabled に false をセットするなどして更新を止めて下さい。
        /// </remarks>
        /// <returns>複製された MemoryImage</returns>
        public MemoryImage Clone()
        {
            this.Stream.Seek(0, SeekOrigin.Begin);

            return MemoryImage.CopyFromStream(this.Stream);
        }

        /// <summary>
        /// MemoryImage インスタンスを非同期に複製します
        /// </summary>
        /// <remarks>
        /// メソッド実行中にストリームのシークが行われないよう注意して下さい。
        /// 特に PictureBox で Gif アニメーションを表示している場合は Enabled に false をセットするなどして更新を止めて下さい。
        /// </remarks>
        /// <returns>複製された MemoryImage を返すタスク</returns>
        public Task<MemoryImage> CloneAsync()
        {
            this.Stream.Seek(0, SeekOrigin.Begin);

            return MemoryImage.CopyFromStreamAsync(this.Stream);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
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
        /// <exception cref="InvalidImageException">不正な画像データが入力された場合</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        public static MemoryImage CopyFromStream(Stream stream)
        {
            var memstream = new MemoryStream();

            stream.CopyTo(memstream);

            return new MemoryImage(memstream);
        }

        /// <summary>
        /// 指定された Stream から MemoryImage を非同期に作成します。
        /// </summary>
        /// <remarks>
        /// ストリームの内容はメモリ上に展開した後に使用されるため、
        /// 引数に指定した Stream を MemoryImage より先に破棄しても問題ありません。
        /// </remarks>
        /// <param name="stream">読み込む対象となる Stream</param>
        /// <returns>作成された MemoryImage を返すタスク</returns>
        /// <exception cref="InvalidImageException">不正な画像データが入力された場合</exception>
        public async static Task<MemoryImage> CopyFromStreamAsync(Stream stream)
        {
            var memstream = new MemoryStream();

            await stream.CopyToAsync(memstream).ConfigureAwait(false);

            return new MemoryImage(memstream);
        }

        /// <summary>
        /// 指定されたバイト列から MemoryImage を作成します。
        /// </summary>
        /// <param name="bytes">読み込む対象となるバイト列</param>
        /// <returns>作成された MemoryImage</returns>
        /// <exception cref="InvalidImageException">不正な画像データが入力された場合</exception>
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        public static MemoryImage CopyFromBytes(byte[] bytes)
        {
            return new MemoryImage(new MemoryStream(bytes));
        }
    }

    /// <summary>
    /// 不正な画像データに対してスローされる例外
    /// </summary>
    [Serializable]
    public class InvalidImageException : Exception
    {
        public InvalidImageException() : base() { }
        public InvalidImageException(string message) : base(message) { }
        public InvalidImageException(string message, Exception innerException) : base(message, innerException) { }
        protected InvalidImageException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
