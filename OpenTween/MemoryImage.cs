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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
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
    public class MemoryImage : ICloneable, IDisposable, IEquatable<MemoryImage>
    {
        private readonly byte[] buffer;
        private readonly int bufferOffset;
        private readonly int bufferCount;
        private readonly Image image;

        private static readonly Dictionary<ImageFormat, string> ExtensionByFormat = new()
        {
            { ImageFormat.Bmp, ".bmp" },
            { ImageFormat.Emf, ".emf" },
            { ImageFormat.Gif, ".gif" },
            { ImageFormat.Icon, ".ico" },
            { ImageFormat.Jpeg, ".jpg" },
            { ImageFormat.MemoryBmp, ".bmp" },
            { ImageFormat.Png, ".png" },
            { ImageFormat.Tiff, ".tiff" },
            { ImageFormat.Wmf, ".wmf" },
        };

        /// <exception cref="InvalidImageException">
        /// ストリームから読みだされる画像データが不正な場合にスローされる
        /// </exception>
        protected MemoryImage(byte[] buffer, int offset, int count)
        {
            try
            {
                this.buffer = buffer;
                this.bufferOffset = offset;
                this.bufferCount = count;

                this.Stream = new(buffer, offset, count, writable: false);
                this.image = this.CreateImage(this.Stream);
            }
            catch
            {
                this.Stream?.Dispose();
                throw;
            }
        }

        private Image CreateImage(Stream stream)
        {
            try
            {
                return Image.FromStream(stream);
            }
            catch (ArgumentException e)
            {
                throw new InvalidImageException("Invalid image", e);
            }
            catch (OutOfMemoryException e)
            {
                // GDI+ がサポートしない画像形式で OutOfMemoryException がスローされる場合があるらしい
                throw new InvalidImageException("Invalid image?", e);
            }
            catch (ExternalException e)
            {
                // 「GDI+ で汎用エラーが発生しました」という大雑把な例外がスローされる場合があるらしい
                throw new InvalidImageException("Invalid image?", e);
            }
        }

        /// <summary>
        /// MemoryImage が保持している画像
        /// </summary>
        public Image Image
        {
            get
            {
                if (this.IsDisposed)
                    throw new ObjectDisposedException("this");

                return this.image;
            }
        }

        /// <summary>
        /// MemoryImage が保持している画像のストリーム
        /// </summary>
        public MemoryStream Stream { get; }

        /// <summary>
        /// MemoryImage が破棄されているか否か
        /// </summary>
        public bool IsDisposed { get; private set; } = false;

        /// <summary>
        /// MemoryImage が保持している画像のフォーマット
        /// </summary>
        public ImageFormat ImageFormat
            => this.Image.RawFormat;

        /// <summary>
        /// MemoryImage が保持している画像のフォーマットに相当する拡張子 (ピリオド付き)
        /// </summary>
        public string ImageFormatExt
            => MemoryImage.ExtensionByFormat.TryGetValue(this.ImageFormat, out var ext) ? ext : "";

        /// <summary>
        /// MemoryImage インスタンスを複製します
        /// </summary>
        /// <returns>複製された MemoryImage</returns>
        public MemoryImage Clone()
            => new(this.buffer, this.bufferOffset, this.bufferCount);

        public override int GetHashCode()
        {
            using var sha1service = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            var hash = sha1service.ComputeHash(this.buffer, this.bufferOffset, this.bufferCount);
            return Convert.ToBase64String(hash).GetHashCode();
        }

        public override bool Equals(object? other)
            => this.Equals(other as MemoryImage);

        public bool Equals(MemoryImage? other)
        {
            if (object.ReferenceEquals(this, other))
                return true;

            if (other == null)
                return false;

            // それぞれが保持する MemoryStream の内容が等しいことを検証する
            var selfBuffer = new ArraySegment<byte>(this.buffer, this.bufferOffset, this.bufferCount);
            var otherBuffer = new ArraySegment<byte>(other.buffer, other.bufferOffset, other.bufferCount);

            if (selfBuffer.Count != otherBuffer.Count)
                return false;

            return selfBuffer.Zip(otherBuffer, (x, y) => x == y).All(x => x);
        }

        object ICloneable.Clone()
            => this.Clone();

        protected virtual void Dispose(bool disposing)
        {
            if (this.IsDisposed) return;

            if (disposing)
            {
                this.Image.Dispose();
                this.Stream.Dispose();
            }

            this.IsDisposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);

            // 明示的にDisposeが呼ばれた場合はファイナライザを使用しない
            GC.SuppressFinalize(this);
        }

        ~MemoryImage()
            => this.Dispose(false);

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
        public static MemoryImage CopyFromStream(Stream stream)
        {
            using var memstream = new MemoryStream();

            stream.CopyTo(memstream);

            return new(memstream.GetBuffer(), 0, (int)memstream.Length);
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
        public static async Task<MemoryImage> CopyFromStreamAsync(Stream stream, int capacity = 0)
        {
            using var memstream = new MemoryStream(capacity);

            await stream.CopyToAsync(memstream)
                .ConfigureAwait(false);

            return new(memstream.GetBuffer(), 0, (int)memstream.Length);
        }

        /// <summary>
        /// 指定されたバイト列から MemoryImage を作成します。
        /// </summary>
        /// <param name="bytes">読み込む対象となるバイト列</param>
        /// <returns>作成された MemoryImage</returns>
        /// <exception cref="InvalidImageException">不正な画像データが入力された場合</exception>
        public static MemoryImage CopyFromBytes(byte[] bytes)
            => new(bytes, 0, bytes.Length);

        /// <summary>
        /// Image インスタンスから MemoryImage を作成します
        /// </summary>
        /// <remarks>
        /// PNG 画像として描画し直す処理を含むため、極力 Stream や byte[] を渡す他のメソッドを使用すべきです
        /// </remarks>
        /// <param name="image">対象となる画像</param>
        /// <returns>作成された MemoryImage</returns>
        public static MemoryImage CopyFromImage(Image image)
        {
            using var memstream = new MemoryStream();

            image.Save(memstream, ImageFormat.Png);

            return new(memstream.GetBuffer(), 0, (int)memstream.Length);
        }
    }

    /// <summary>
    /// 不正な画像データに対してスローされる例外
    /// </summary>
    [Serializable]
    public class InvalidImageException : Exception
    {
        public InvalidImageException()
        {
        }

        public InvalidImageException(string message)
            : base(message)
        {
        }

        public InvalidImageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidImageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
