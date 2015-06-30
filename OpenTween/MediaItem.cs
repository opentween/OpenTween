// OpenTween - Client of Twitter
// Copyright (c) 2015 spx (@5px)
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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

namespace OpenTween
{
    public interface IMediaItem
    {
        /// <summary>
        /// メディアへの絶対パス
        /// </summary>
        string Path { get; }

        /// <summary>
        /// メディア名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// メディアの拡張子
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// メディアが存在するかどうかを示す真偽値
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// メディアのサイズ（バイト単位）
        /// </summary>
        long Size { get; }

        /// <summary>
        /// 表示用の MemoryImage を作成する
        /// </summary>
        MemoryImage CreateImage();

        /// <summary>
        /// メディアの内容を Stream へ書き込む
        /// </summary>
        void CopyTo(Stream stream);
    }

    /// <summary>
    /// ファイル用の MediaItem クラス
    /// </summary>
    public class FileMediaItem : IMediaItem
    {
        private FileInfo _fileInfo = null;

        public FileMediaItem(string path)
        {
            this._fileInfo = new FileInfo(path);
        }

        public FileMediaItem(FileInfo fileInfo)
            : this(fileInfo.FullName)
        {
        }

        public virtual string Path
        {
            get { return this._fileInfo.FullName; }
        }

        public virtual string Name
        {
            get { return this._fileInfo.Name; }
        }

        public virtual string Extension
        {
            get { return this._fileInfo.Extension; }
        }

        public virtual bool Exists
        {
            get { return this._fileInfo.Exists; }
        }

        public virtual long Size
        {
            get { return this._fileInfo.Length; }
        }

        public virtual MemoryImage CreateImage()
        {
            using (var fs = this._fileInfo.OpenRead())
            {
                return MemoryImage.CopyFromStream(fs);
            }
        }

        public virtual void CopyTo(Stream stream)
        {
            using (var fs = this._fileInfo.OpenRead())
            {
                fs.CopyTo(stream);
            }
        }

        public FileInfo FileInfo
        {
            get { return this._fileInfo; }
        }
    }

    /// <summary>
    /// MemoryImage 用の MediaItem クラス
    /// </summary>
    /// <remarks>
    /// 用途の関係上、メモリ使用量が大きくなるため、不要になればできるだけ破棄すること
    /// </remarks>
    public class MemoryImageMediaItem : IMediaItem, IDisposable
    {
        public static readonly string PathPrefix = "<>MemoryImage://";
        private static int _fileNumber = 0;

        private bool _disposed = false;

        private string _path;
        private MemoryImage _image;

        public MemoryImageMediaItem(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            // image から png 形式の MemoryImage を生成
            using (var bitmap = new Bitmap(image))
            {
                this._image = MemoryImage.CopyFromBitmap(bitmap);
            }

            var num = Interlocked.Increment(ref _fileNumber);
            this._path = PathPrefix + num + this._image.ImageFormatExt;
        }

        public virtual string Path
        {
            get { return this._path; }
        }

        public virtual string Name
        {
            get { return this._path.Substring(PathPrefix.Length); }
        }

        public virtual string Extension
        {
            get { return this._image.ImageFormatExt; }
        }

        public virtual bool Exists
        {
            get { return this._image != null; }
        }

        public virtual long Size
        {
            get { return this._image.Stream.Length; }
        }

        public virtual MemoryImage CreateImage()
        {
            return this._image.Clone();
        }

        public virtual void CopyTo(Stream stream)
        {
            this._image.Stream.Seek(0, SeekOrigin.Begin);
            this._image.Stream.CopyTo(stream);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed) return;

            if (disposing)
            {
                this._image.Dispose();
            }

            this._disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);

            // 明示的にDisposeが呼ばれた場合はファイナライザを使用しない
            GC.SuppressFinalize(this);
        }

        ~MemoryImageMediaItem()
        {
            this.Dispose(false);
        }
    }
}
