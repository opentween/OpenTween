// OpenTween - Client of Twitter
// Copyright (c) 2014 spx (@5px)
// Copyright (c) 2023 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Text;
using System.Threading.Tasks;
using OpenTween.Api.DataModel;
using OpenTween.MediaUploadServices;

namespace OpenTween
{
    public sealed class MediaSelector : NotifyPropertyChangedBase, IDisposable
    {
        private KeyValuePair<string, IMediaUploadService>[] pictureServices = Array.Empty<KeyValuePair<string, IMediaUploadService>>();
        private readonly BindingList<IMediaItem> mediaItems = new();
        private string selectedMediaServiceName = "";
        private Guid? selectedMediaItemId = null;
        private MemoryImage? selectedMediaItemImage = null;

        public bool IsDisposed { get; private set; } = false;

        public KeyValuePair<string, IMediaUploadService>[] MediaServices
        {
            get => this.pictureServices;
            private set => this.SetProperty(ref this.pictureServices, value);
        }

        public BindingList<IMediaItem> MediaItems
            => this.mediaItems;

        public MemoryImageList ThumbnailList { get; } = new();

        /// <summary>
        /// 選択されている投稿先名を取得する。
        /// </summary>
        public string SelectedMediaServiceName
        {
            get => this.selectedMediaServiceName;
            set => this.SetProperty(ref this.selectedMediaServiceName, value);
        }

        /// <summary>
        /// 選択されている投稿先を示すインデックスを取得する。
        /// </summary>
        public int SelectedMediaServiceIndex
            => this.MediaServices.FindIndex(x => x.Key == this.SelectedMediaServiceName);

        /// <summary>
        /// 選択されている投稿先の IMediaUploadService を取得する。
        /// </summary>
        public IMediaUploadService? SelectedMediaService
            => this.GetService(this.SelectedMediaServiceName);

        public bool CanUseAltText
            => this.SelectedMediaService?.CanUseAltText ?? false;

        public Guid? SelectedMediaItemId
        {
            get => this.selectedMediaItemId;
            set
            {
                if (this.selectedMediaItemId == value)
                    return;

                this.SetProperty(ref this.selectedMediaItemId, value);
                this.LoadSelectedMediaItemImage();
            }
        }

        public IMediaItem? SelectedMediaItem
            => this.SelectedMediaItemId != null ? this.MediaItems.First(x => x.Id == this.SelectedMediaItemId) : null;

        public int SelectedMediaItemIndex
        {
            get => this.MediaItems.FindIndex(x => x.Id == this.SelectedMediaItemId);
            set => this.SelectedMediaItemId = value != -1 ? this.MediaItems[value].Id : null;
        }

        public MemoryImage? SelectedMediaItemImage
        {
            get => this.selectedMediaItemImage;
            set => this.SetProperty(ref this.selectedMediaItemImage, value);
        }

        /// <summary>
        /// 指定された投稿先名から、作成済みの IMediaUploadService インスタンスを取得する。
        /// </summary>
        public IMediaUploadService? GetService(string serviceName)
        {
            var index = this.MediaServices.FindIndex(x => x.Key == serviceName);
            return index != -1 ? this.MediaServices[index].Value : null;
        }

        public void InitializeServices(Twitter tw, TwitterConfiguration twitterConfig)
        {
            this.MediaServices = new KeyValuePair<string, IMediaUploadService>[]
            {
                new("Twitter", new TwitterPhoto(tw, twitterConfig)),
                new("Imgur", new Imgur(twitterConfig)),
                new("Mobypicture", new Mobypicture(tw, twitterConfig)),
            };
        }

        public void SelectMediaService(string serviceName, int? index = null)
        {
            int idx;
            if (MyCommon.IsNullOrEmpty(serviceName))
            {
                // 引数の index は serviceName が空の場合のみ使用する
                idx = index ?? 0;
            }
            else
            {
                idx = this.MediaServices.FindIndex(x => x.Key == serviceName);

                // svc が空白以外かつ存在しないサービス名の場合は Twitter を選択させる
                // (廃止されたサービスを選択していた場合の対応)
                if (idx == -1)
                    idx = 0;
            }

            this.SelectedMediaServiceName = this.MediaServices[idx].Key;
        }

        /// <summary>
        /// 指定されたファイルの投稿に対応した投稿先があるかどうかを示す値を取得する。
        /// </summary>
        public bool HasUploadableService(string fileName, bool ignoreSize)
        {
            var fl = new FileInfo(fileName);
            var ext = fl.Extension;
            var size = ignoreSize ? (long?)null : fl.Length;

            return this.GetAvailableServiceNames(ext, size).Any();
        }

        public string[] GetAvailableServiceNames(string extension, long? fileSize)
            => this.MediaServices
                .Where(x => x.Value.CheckFileExtension(extension) && (fileSize == null || x.Value.CheckFileSize(extension, fileSize.Value)))
                .Select(x => x.Key)
                .ToArray();

        public void AddMediaItemFromImage(Image image)
        {
            var mediaItem = this.CreateMemoryImageMediaItem(image);
            if (mediaItem == null)
                return;

            this.AddMediaItem(mediaItem);
            this.SelectedMediaItemId = mediaItem.Id;
        }

        public void AddMediaItemFromFilePath(string[] filePathArray)
        {
            if (filePathArray.Length == 0)
                return;

            var mediaItems = new IMediaItem[filePathArray.Length];

            // 連番のファイル名を一括でアップロードする場合の利便性のためソートする
            var sortedFilePath = filePathArray.OrderBy(x => x);

            foreach (var (path, index) in sortedFilePath.WithIndex())
            {
                var mediaItem = this.CreateFileMediaItem(path);
                if (mediaItem == null)
                    continue;

                mediaItems[index] = mediaItem;
            }

            // 全ての IMediaItem の生成に成功した場合のみ追加する
            foreach (var mediaItem in mediaItems)
                this.AddMediaItem(mediaItem);

            this.SelectedMediaItemId = mediaItems.Last().Id;
        }

        public void AddMediaItem(IMediaItem item)
        {
            var id = item.Id.ToString();
            var thumbnailImage = this.GenerateThumbnailImage(item);
            this.ThumbnailList.Add(id, thumbnailImage);
            this.MediaItems.Add(item);
        }

        private MemoryImage GenerateThumbnailImage(IMediaItem item)
        {
            using var origImage = this.CreateMediaItemImage(item);
            var origSize = origImage.Image.Size;
            var thumbSize = this.ThumbnailList.ImageList.ImageSize;

            using var bitmap = new Bitmap(thumbSize.Width, thumbSize.Height);

            // 縦横比を維持したまま thumbSize に収まるサイズに縮小する
            using (var g = Graphics.FromImage(bitmap))
            {
                var scale = Math.Min(
                    (float)thumbSize.Width / origSize.Width,
                    (float)thumbSize.Height / origSize.Height
                );
                var fitSize = new SizeF(origSize.Width * scale, origSize.Height * scale);
                var pos = new PointF(
                    x: (thumbSize.Width - fitSize.Width) / 2.0f,
                    y: (thumbSize.Height - fitSize.Height) / 2.0f
                );
                g.DrawImage(origImage.Image, new RectangleF(pos, fitSize));
            }

            return MemoryImage.CopyFromImage(bitmap);
        }

        public void ClearMediaItems()
        {
            this.SelectedMediaItemId = null;

            var mediaItems = this.MediaItems.ToList();
            this.MediaItems.Clear();

            foreach (var mediaItem in mediaItems)
                mediaItem.Dispose();

            var thumbnailImages = this.ThumbnailList.ToList();
            this.ThumbnailList.Clear();

            foreach (var image in thumbnailImages)
                image.Dispose();
        }

        public IMediaItem[] DetachMediaItems()
        {
            // ClearMediaItems では MediaItem が破棄されるため、外部で使用する場合はこのメソッドを使用して MediaItems から切り離す
            var mediaItems = this.MediaItems.ToArray();
            this.MediaItems.Clear();
            this.ClearMediaItems();

            return mediaItems;
        }

        private MemoryImageMediaItem? CreateMemoryImageMediaItem(Image image)
        {
            if (image == null)
                return null;

            MemoryImage? memoryImage = null;
            try
            {
                // image から png 形式の MemoryImage を生成
                memoryImage = MemoryImage.CopyFromImage(image);

                return new MemoryImageMediaItem(memoryImage);
            }
            catch
            {
                memoryImage?.Dispose();
                return null;
            }
        }

        private FileMediaItem? CreateFileMediaItem(string path)
        {
            if (MyCommon.IsNullOrEmpty(path))
                return null;

            try
            {
                return new FileMediaItem(path);
            }
            catch
            {
                return null;
            }
        }

        private void LoadSelectedMediaItemImage()
        {
            var previousImage = this.selectedMediaItemImage;

            if (this.SelectedMediaItem == null)
            {
                this.SelectedMediaItemImage = null;
                previousImage?.Dispose();
                return;
            }

            this.SelectedMediaItemImage = this.CreateMediaItemImage(this.SelectedMediaItem);
            previousImage?.Dispose();
        }

        private MemoryImage CreateMediaItemImage(IMediaItem media)
        {
            try
            {
                return media.CreateImage();
            }
            catch (InvalidImageException)
            {
                return MemoryImage.CopyFromImage(Properties.Resources.MultiMediaImage);
            }
        }

        public void SetSelectedMediaAltText(string altText)
        {
            var selectedMedia = this.SelectedMediaItem;
            if (selectedMedia == null)
                return;

            selectedMedia.AltText = altText.Trim();
        }

        public MediaSelectorErrorType Validate(out IMediaItem? rejectedMedia)
        {
            rejectedMedia = null;

            if (this.MediaItems.Count == 0)
                return MediaSelectorErrorType.MediaItemNotSet;

            var uploadService = this.SelectedMediaService;
            if (uploadService == null)
                return MediaSelectorErrorType.ServiceNotSelected;

            foreach (var mediaItem in this.MediaItems)
            {
                var error = this.ValidateMediaItem(uploadService, mediaItem);
                if (error != MediaSelectorErrorType.None)
                {
                    rejectedMedia = mediaItem;
                    return error;
                }
            }

            return MediaSelectorErrorType.None;
        }

        private MediaSelectorErrorType ValidateMediaItem(IMediaUploadService imageService, IMediaItem item)
        {
            var ext = item.Extension;
            var size = item.Size;

            if (!imageService.CheckFileExtension(ext))
                return MediaSelectorErrorType.UnsupportedFileExtension;

            if (!imageService.CheckFileSize(ext, size))
                return MediaSelectorErrorType.FileSizeExceeded;

            return MediaSelectorErrorType.None;
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.IsDisposed = true;
            this.ThumbnailList.Dispose();
        }
    }

    public enum MediaSelectorErrorType
    {
        None,
        MediaItemNotSet,
        ServiceNotSelected,
        UnsupportedFileExtension,
        FileSizeExceeded,
    }
}
