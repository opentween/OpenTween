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
using System.Drawing.Imaging;

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
        private readonly MemoryStream stream;
        private readonly Image image;

        protected bool disposed = false;

        /// <exception cref="InvalidImageException">
        /// ストリームから読みだされる画像データが不正な場合にスローされる
        /// </exception>
        protected MemoryImage(MemoryStream stream)
        {
            try
            {
                this.image = Image.FromStream(stream);
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

            this.stream = stream;
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
        public MemoryStream Stream
        {
            // MemoryStream は破棄されていても一部のメソッドが使用可能なためここでは例外を投げない
            get { return this.stream; }
        }

        /// <summary>
        /// MemoryImage が破棄されているか否か
        /// </summary>
        public bool IsDisposed
        {
            get { return this.disposed; }
        }

        /// <summary>
        /// MemoryImage が保持している画像のフォーマット
        /// </summary>
        public ImageFormat ImageFormat
        {
            get { return this.Image.RawFormat; }
        }

        /// <summary>
        /// MemoryImage が保持している画像のフォーマットに相当する拡張子 (ピリオド付き)
        /// </summary>
        public string ImageFormatExt
        {
            get
            {
                var format = this.ImageFormat;

                // ImageFormat は == で正しく比較できないため Equals を使用する必要がある
                if (format.Equals(ImageFormat.Bmp))
                    return ".bmp";
                if (format.Equals(ImageFormat.Emf))
                    return ".emf";
                if (format.Equals(ImageFormat.Gif))
                    return ".gif";
                if (format.Equals(ImageFormat.Icon))
                    return ".ico";
                if (format.Equals(ImageFormat.Jpeg))
                    return ".jpg";
                if (format.Equals(ImageFormat.MemoryBmp))
                    return ".bmp";
                if (format.Equals(ImageFormat.Png))
                    return ".png";
                if (format.Equals(ImageFormat.Tiff))
                    return ".tiff";
                if (format.Equals(ImageFormat.Wmf))
                    return ".wmf";

                // 対応する形式がなければ空文字列を返す
                // (上記以外のフォーマットは Image.FromStream を通過できないため、ここが実行されることはまず無い)
                return string.Empty;
            }
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

        public override int GetHashCode()
        {
            using (var sha1service = new System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                var hash = sha1service.ComputeHash(this.Stream.GetBuffer(), 0, (int)this.Stream.Length);
                return Convert.ToBase64String(hash).GetHashCode();
            }
        }

        public override bool Equals(object other)
        {
            return this.Equals(other as MemoryImage);
        }

        public bool Equals(MemoryImage other)
        {
            if (object.ReferenceEquals(this, other))
                return true;

            if (other == null)
                return false;

            // それぞれが保持する MemoryStream の内容が等しいことを検証する

            var selfLength = this.Stream.Length;
            var otherLength = other.Stream.Length;

            if (selfLength != otherLength)
                return false;

            var selfBuffer = this.Stream.GetBuffer();
            var otherBuffer = other.Stream.GetBuffer();

            for (var pos = 0L; pos < selfLength; pos++)
            {
                if (selfBuffer[pos] != otherBuffer[pos])
                    return false;
            }

            return true;
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
        public static MemoryImage CopyFromStream(Stream stream)
        {
            MemoryStream memstream = null;
            try
            {
                memstream = new MemoryStream();

                stream.CopyTo(memstream);

                return new MemoryImage(memstream);
            }
            catch
            {
                memstream?.Dispose();
                throw;
            }
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
            MemoryStream memstream = null;
            try
            {
                memstream = new MemoryStream();

                await stream.CopyToAsync(memstream).ConfigureAwait(false);

                return new MemoryImage(memstream);
            }
            catch
            {
                memstream?.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 指定されたバイト列から MemoryImage を作成します。
        /// </summary>
        /// <param name="bytes">読み込む対象となるバイト列</param>
        /// <returns>作成された MemoryImage</returns>
        /// <exception cref="InvalidImageException">不正な画像データが入力された場合</exception>
        public static MemoryImage CopyFromBytes(byte[] bytes)
        {
            MemoryStream memstream = null;
            try
            {
                memstream = new MemoryStream(bytes);
                return new MemoryImage(memstream);
            }
            catch
            {
                memstream?.Dispose();
                throw;
            }
        }

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
            MemoryStream memstream = null;
            try
            {
                memstream = new MemoryStream();

                image.Save(memstream, ImageFormat.Png);

                return new MemoryImage(memstream);
            }
            catch
            {
                memstream?.Dispose();
                throw;
            }
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
