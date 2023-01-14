// OpenTween - Client of Twitter
// Copyright (c) 2014 spx (@5px)
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
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Api.DataModel;
using OpenTween.MediaUploadServices;

namespace OpenTween
{
    public partial class MediaSelectorPanel : UserControl
    {
        public event EventHandler<EventArgs>? BeginSelecting;

        public event EventHandler<EventArgs>? EndSelecting;

        public event EventHandler<EventArgs>? FilePickDialogOpening;

        public event EventHandler<EventArgs>? FilePickDialogClosed;

        public event EventHandler<EventArgs>? SelectedServiceChanged;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OpenFileDialog? FilePickDialog { get; set; }

        /// <summary>
        /// 選択されている投稿先名を取得する。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ServiceName
            => this.ImageServiceCombo.Text;

        /// <summary>
        /// 選択されている投稿先を示すインデックスを取得する。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ServiceIndex
            => this.ImageServiceCombo.SelectedIndex;

        /// <summary>
        /// 選択されている投稿先の IMediaUploadService を取得する。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IMediaUploadService? SelectedService
        {
            get
            {
                var serviceName = this.ServiceName;
                if (MyCommon.IsNullOrEmpty(serviceName))
                    return null;

                return this.pictureService.TryGetValue(serviceName, out var service)
                    ? service : null;
            }
        }

        /// <summary>
        /// 指定された投稿先名から、作成済みの IMediaUploadService インスタンスを取得する。
        /// </summary>
        public IMediaUploadService GetService(string serviceName)
        {
            this.pictureService.TryGetValue(serviceName, out var service);
            return service;
        }

        /// <summary>
        /// 利用可能な全ての IMediaUploadService インスタンスを取得する。
        /// </summary>
        public ICollection<IMediaUploadService> GetServices()
            => this.pictureService.Values;

        private Dictionary<string, IMediaUploadService> pictureService = new();

        private readonly List<IMediaItem> mediaItems = new();
        private readonly MemoryImageList thumbnailList = new();
        private Guid? selectedMediaItemId = null;

        private void CreateServices(Twitter tw, TwitterConfiguration twitterConfig)
        {
            this.pictureService?.Clear();

            this.pictureService = new Dictionary<string, IMediaUploadService>
            {
                ["Twitter"] = new TwitterPhoto(tw, twitterConfig),
                ["Imgur"] = new Imgur(twitterConfig),
                ["Mobypicture"] = new Mobypicture(tw, twitterConfig),
            };
        }

        public MediaSelectorPanel()
        {
            this.InitializeComponent();

            this.ImageSelectedPicture.InitialImage = Properties.Resources.InitialImage;
            this.MediaListView.LargeImageList = this.thumbnailList.ImageList;

            var thumbnailWidth = 75 * this.DeviceDpi / 96;
            this.thumbnailList.ImageList.ImageSize = new(thumbnailWidth, thumbnailWidth);

            this.UpdateSelectedMedia();
            this.UpdateAltTextPanelVisible();
        }

        /// <summary>
        /// 投稿先サービスなどを初期化する。
        /// </summary>
        public void Initialize(Twitter tw, TwitterConfiguration twitterConfig, string svc, int? index = null)
        {
            this.CreateServices(tw, twitterConfig);

            this.SetImageServiceCombo();
            this.SelectImageServiceComboItem(svc, index);
        }

        /// <summary>
        /// 投稿先サービスを再作成する。
        /// </summary>
        public void Reset(Twitter tw, TwitterConfiguration twitterConfig)
        {
            this.CreateServices(tw, twitterConfig);

            this.SetImageServiceCombo();
        }

        /// <summary>
        /// 指定されたファイルの投稿に対応した投稿先があるかどうかを示す値を取得する。
        /// </summary>
        public bool HasUploadableService(string fileName, bool ignoreSize)
        {
            var fl = new FileInfo(fileName);
            var ext = fl.Extension;
            var size = ignoreSize ? (long?)null : fl.Length;

            if (this.IsUploadable(this.ServiceName, ext, size))
                return true;

            foreach (string svc in this.ImageServiceCombo.Items)
            {
                if (this.IsUploadable(svc, ext, size))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 指定された投稿先に投稿可能かどうかを示す値を取得する。
        /// ファイルサイズの指定がなければ拡張子だけで判定する。
        /// </summary>
        private bool IsUploadable(string serviceName, string ext, long? size)
        {
            if (!MyCommon.IsNullOrEmpty(serviceName))
            {
                var imageService = this.pictureService[serviceName];
                if (imageService.CheckFileExtension(ext))
                {
                    if (!size.HasValue)
                        return true;

                    if (imageService.CheckFileSize(ext, size.Value))
                        return true;
                }
            }
            return false;
        }

        private void ClearMediaItems()
        {
            this.selectedMediaItemId = null;
            this.UpdateSelectedMedia();

            this.MediaListView.Items.Clear();

            var mediaItems = this.mediaItems.ToList();
            this.mediaItems.Clear();

            foreach (var mediaItem in mediaItems)
                this.DisposeMediaItem(mediaItem);

            var thumbnailImages = this.thumbnailList.ToList();
            this.thumbnailList.Clear();

            foreach (var image in thumbnailImages)
                image.Dispose();
        }

        public void AddMediaItemFromImage(Image image)
        {
            var mediaItem = this.CreateMemoryImageMediaItem(image, noMsgBox: false);
            if (mediaItem == null)
                return;

            this.AddMediaItem(mediaItem);
            this.SelectMediaItem(mediaItem.Id);
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
                var mediaItem = this.CreateFileMediaItem(path, noMsgBox: false);
                if (mediaItem == null)
                    return;

                mediaItems[index] = mediaItem;
            }

            // 全ての IMediaItem の生成に成功した場合のみ追加する
            foreach (var mediaItem in mediaItems)
                this.AddMediaItem(mediaItem);

            this.SelectMediaItem(mediaItems.Last().Id);
        }

        public void AddMediaItem(IMediaItem item)
        {
            this.mediaItems.Add(item);

            MemoryImage thumbnailImage;
            try
            {
                thumbnailImage = item.CreateImage();
            }
            catch (InvalidImageException)
            {
                thumbnailImage = MemoryImage.CopyFromImage(Properties.Resources.MultiMediaImage);
            }

            var id = item.Id.ToString();
            this.thumbnailList.Add(id, thumbnailImage);

            this.MediaListView.Items.Add(item.Name, id);
        }

        public void SelectMediaItem(Guid id)
        {
            var index = this.mediaItems.FindIndex(x => x.Id == id);
            if (index == -1)
                return;

            // selectedMediaItemId は ImageListView のイベントハンドラ内でセットされる
            this.MediaListView.SelectedIndices.Clear();
            this.MediaListView.SelectedIndices.Add(index);
        }

        /// <summary>
        /// 投稿するファイルとその投稿先を選択するためのコントロールを表示する。
        /// </summary>
        public void BeginSelection()
        {
            this.BeginSelecting?.Invoke(this, EventArgs.Empty);
            this.Enabled = true;
            this.Visible = true;
        }

        /// <summary>
        /// 選択処理を終了してコントロールを隠す。
        /// </summary>
        public void EndSelection()
        {
            this.EndSelecting?.Invoke(this, EventArgs.Empty);
            this.Visible = false;
            this.Enabled = false;
            this.ClearMediaItems();
        }

        /// <summary>
        /// 選択された投稿先名と投稿する MediaItem を取得する。MediaItem は不要になったら呼び出し側にて破棄すること。
        /// </summary>
        public bool TryGetSelectedMedia([NotNullWhen(true)] out string? imageService, [NotNullWhen(true)] out IMediaItem[]? mediaItems)
        {
            imageService = null;
            mediaItems = null;

            var uploadService = this.SelectedService;
            if (uploadService == null || this.mediaItems.Count == 0)
            {
                MessageBox.Show(Properties.Resources.PostPictureWarn1, Properties.Resources.PostPictureWarn2);
                return false;
            }

            foreach (var mediaItem in this.mediaItems)
            {
                if (!this.ValidateMediaItem(uploadService, mediaItem))
                    return false;
            }

            // 収集した MediaItem が破棄されないように、ClearMediaItems を呼ぶ前に mediaItems を空にしておく
            this.mediaItems.Clear();

            imageService = this.ServiceName;
            mediaItems = this.mediaItems.ToArray();
            this.EndSelection();
            return true;
        }

        private MemoryImageMediaItem? CreateMemoryImageMediaItem(Image image, bool noMsgBox)
        {
            if (image == null) return null;

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

                if (!noMsgBox) MessageBox.Show("Unable to create MemoryImage.");
                return null;
            }
        }

        private IMediaItem? CreateFileMediaItem(string path, bool noMsgBox)
        {
            if (MyCommon.IsNullOrEmpty(path)) return null;

            try
            {
                return new FileMediaItem(path);
            }
            catch
            {
                if (!noMsgBox) MessageBox.Show("Invalid file path: " + path);
                return null;
            }
        }

        private void DisposeMediaItem(IMediaItem? item)
        {
            var disposableItem = item as IDisposable;
            disposableItem?.Dispose();
        }

        private void AddMediaButton_Click(object sender, EventArgs e)
        {
            var service = this.SelectedService;

            if (this.FilePickDialog == null || service == null) return;
            this.FilePickDialog.Filter = service.SupportedFormatsStrForDialog;
            this.FilePickDialog.Title = Properties.Resources.PickPictureDialog1;
            this.FilePickDialog.FileName = "";

            this.FilePickDialogOpening?.Invoke(this, EventArgs.Empty);

            try
            {
                if (this.FilePickDialog.ShowDialog() == DialogResult.Cancel) return;
            }
            finally
            {
                this.FilePickDialogClosed?.Invoke(this, EventArgs.Empty);
            }

            this.AddMediaItemFromFilePath(this.FilePickDialog.FileNames);
        }

        private bool ValidateMediaItem(IMediaUploadService imageService, IMediaItem item)
        {
            var ext = item.Extension;
            var size = item.Size;

            if (!imageService.CheckFileExtension(ext))
            {
                // 画像以外の形式
                MessageBox.Show(
                    string.Format(Properties.Resources.PostPictureWarn3, this.ServiceName, this.MakeAvailableServiceText(ext, size), ext, item.Name),
                    Properties.Resources.PostPictureWarn4,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            if (!imageService.CheckFileSize(ext, size))
            {
                // ファイルサイズが大きすぎる
                MessageBox.Show(
                    string.Format(Properties.Resources.PostPictureWarn5, this.ServiceName, this.MakeAvailableServiceText(ext, size), item.Name),
                    Properties.Resources.PostPictureWarn4,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            return true;
        }

        private string MakeAvailableServiceText(string ext, long fileSize)
        {
            var text = string.Join(", ",
                this.ImageServiceCombo.Items.Cast<string>()
                    .Where(serviceName =>
                        !MyCommon.IsNullOrEmpty(serviceName) &&
                        this.pictureService[serviceName].CheckFileExtension(ext) &&
                        this.pictureService[serviceName].CheckFileSize(ext, fileSize)));

            if (MyCommon.IsNullOrEmpty(text))
                return Properties.Resources.PostPictureWarn6;

            return text;
        }

        private void ImageCancelButton_Click(object sender, EventArgs e)
            => this.EndSelection();

        private void SetImageServiceCombo()
        {
            using (ControlTransaction.Update(this.ImageServiceCombo))
            {
                var svc = "";
                if (this.ImageServiceCombo.SelectedIndex > -1) svc = this.ImageServiceCombo.Text;
                this.ImageServiceCombo.Items.Clear();

                // Add service names to combobox
                foreach (var key in this.pictureService.Keys)
                {
                    this.ImageServiceCombo.Items.Add(key);
                }

                this.SelectImageServiceComboItem(svc);
            }
        }

        private void SelectImageServiceComboItem(string svc, int? index = null)
        {
            int idx;
            if (MyCommon.IsNullOrEmpty(svc))
            {
                idx = index ?? 0;
            }
            else
            {
                idx = this.ImageServiceCombo.Items.IndexOf(svc);

                // svc が空白以外かつ存在しないサービス名の場合は Twitter を選択させる
                // (廃止されたサービスを選択していた場合の対応)
                if (idx == -1) idx = 0;
            }

            try
            {
                this.ImageServiceCombo.SelectedIndex = idx;
            }
            catch (ArgumentOutOfRangeException)
            {
                this.ImageServiceCombo.SelectedIndex = 0;
            }

            this.UpdateAltTextPanelVisible();
        }

        private void UpdateAltTextPanelVisible()
            => this.AlternativeTextPanel.Visible = this.SelectedService switch
            {
                null => false,
                var service => service.CanUseAltText,
            };

        private void UpdateSelectedMedia()
        {
            using (ControlTransaction.Update(this))
            {
                if (this.selectedMediaItemId == null)
                {
                    this.AlternativeTextBox.Text = "";
                    this.AlternativeTextPanel.Enabled = false;
                    this.ImageSelectedPicture.ShowInitialImage();
                }
                else
                {
                    var media = this.mediaItems.First(x => x.Id == this.selectedMediaItemId);

                    this.AlternativeTextBox.Text = media.AltText;
                    this.AlternativeTextPanel.Enabled = true;
                    this.ImageSelectedPicture.Image = media.CreateImage();
                }
            }
        }

        private void ImageServiceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.UpdateAltTextPanelVisible();
            this.SelectedServiceChanged?.Invoke(this, EventArgs.Empty);
        }

        private void MediaListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var indices = this.MediaListView.SelectedIndices;
            if (indices.Count == 0)
            {
                this.selectedMediaItemId = null;
            }
            else
            {
                var media = this.mediaItems[indices[0]];
                this.selectedMediaItemId = media.Id;
            }

            this.UpdateSelectedMedia();
        }

        private void AlternativeTextBox_Validated(object sender, EventArgs e)
        {
            if (this.selectedMediaItemId == null)
                return;

            var media = this.mediaItems.First(x => x.Id == this.selectedMediaItemId);
            media.AltText = this.AlternativeTextBox.Text.Trim();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ClearMediaItems();
                this.thumbnailList.Dispose();
                this.components?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
