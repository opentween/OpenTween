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
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public MediaSelector Model { get; } = new();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OpenFileDialog? FilePickDialog { get; set; }

        public MediaSelectorPanel()
        {
            this.InitializeComponent();

            this.ImageSelectedPicture.InitialImage = Properties.Resources.InitialImage;

            this.MediaListView.LargeImageList = this.Model.ThumbnailList.ImageList;

            var thumbnailWidth = 75 * this.DeviceDpi / 96;
            this.Model.ThumbnailList.ImageList.ColorDepth = ColorDepth.Depth24Bit;
            this.Model.ThumbnailList.ImageList.ImageSize = new(thumbnailWidth, thumbnailWidth);

            this.Model.PropertyChanged +=
                (s, e) => this.TryInvoke(() => this.Model_PropertyChanged(s, e));
            this.Model.MediaItems.ListChanged +=
                (s, e) => this.TryInvoke(() => this.Model_MediaItems_ListChanged(s, e));

            this.UpdateSelectedMedia();
            this.UpdateAltTextPanelVisible();
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
            this.Model.ClearMediaItems();
        }

        /// <summary>
        /// 選択された投稿先名と投稿する MediaItem を取得する。MediaItem は不要になったら呼び出し側にて破棄すること。
        /// </summary>
        public bool TryGetSelectedMedia([NotNullWhen(true)] out string? imageService, [NotNullWhen(true)] out IMediaItem[]? mediaItems)
        {
            var selectedServiceName = this.Model.SelectedMediaServiceName;

            var error = this.Model.Validate(out var rejectedMedia);
            if (error != MediaSelectorErrorType.None)
            {
                var message = error switch
                {
                    MediaSelectorErrorType.MediaItemNotSet
                        => Properties.Resources.PostPictureWarn1,
                    MediaSelectorErrorType.ServiceNotSelected
                        => Properties.Resources.PostPictureWarn1,
                    MediaSelectorErrorType.UnsupportedFileExtension
                        => string.Format(
                            Properties.Resources.PostPictureWarn3,
                            selectedServiceName,
                            this.MakeAvailableServiceText(rejectedMedia!),
                            rejectedMedia!.Extension,
                            rejectedMedia!.Name
                        ),
                    MediaSelectorErrorType.FileSizeExceeded
                        => string.Format(
                            Properties.Resources.PostPictureWarn5,
                            selectedServiceName,
                            this.MakeAvailableServiceText(rejectedMedia!),
                            rejectedMedia!.Name
                        ),
                    _ => throw new NotImplementedException(),
                };

                MessageBox.Show(
                    message,
                    Properties.Resources.PostPictureWarn2,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                imageService = null;
                mediaItems = null;
                return false;
            }

            imageService = selectedServiceName;
            mediaItems = this.Model.DetachMediaItems();
            return true;
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MediaSelector.MediaServices):
                    this.UpdateImageServiceComboItems();
                    break;
                case nameof(MediaSelector.SelectedMediaServiceName):
                    this.UpdateImageServiceComboSelection();
                    this.UpdateAltTextPanelVisible();
                    this.SelectedServiceChanged?.Invoke(this, EventArgs.Empty);
                    break;
                case nameof(MediaSelector.SelectedMediaItemId):
                    this.UpdateSelectedMedia();
                    break;
                case nameof(MediaSelector.SelectedMediaItemImage):
                    this.UpdateSelectedMediaImage();
                    break;
                default:
                    break;
            }
        }

        private void Model_MediaItems_ListChanged(object sender, ListChangedEventArgs e)
        {
            void AddMediaListViewItem(IMediaItem media, int index)
                => this.MediaListView.Items.Insert(index, media.Name, media.Id.ToString());

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    var addedMedia = this.Model.MediaItems[e.NewIndex];
                    AddMediaListViewItem(addedMedia, e.NewIndex);
                    break;
                case ListChangedType.Reset:
                    this.MediaListView.Items.Clear();
                    foreach (var (media, index) in this.Model.MediaItems.WithIndex())
                        AddMediaListViewItem(media, index);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void UpdateImageServiceComboItems()
        {
            using (ControlTransaction.Update(this.ImageServiceCombo))
            {
                this.ImageServiceCombo.Items.Clear();

                // Add service names to combobox
                var serviceNames = this.Model.MediaServices.Select(x => x.Key).ToArray();
                this.ImageServiceCombo.Items.AddRange(serviceNames);

                this.UpdateImageServiceComboSelection();
            }
        }

        private void UpdateImageServiceComboSelection()
            => this.ImageServiceCombo.SelectedIndex = this.Model.SelectedMediaServiceIndex;

        private void AddMediaButton_Click(object sender, EventArgs e)
        {
            var service = this.Model.SelectedMediaService;

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

            this.Model.AddMediaItemFromFilePath(this.FilePickDialog.FileNames);
        }

        private string MakeAvailableServiceText(IMediaItem media)
        {
            var ext = media.Extension;
            var fileSize = media.Size;

            var availableServiceNames = this.Model.GetAvailableServiceNames(ext, fileSize);
            if (availableServiceNames.Length == 0)
                return Properties.Resources.PostPictureWarn6;

            return string.Join(", ", availableServiceNames);
        }

        private void ImageCancelButton_Click(object sender, EventArgs e)
            => this.EndSelection();

        private void UpdateAltTextPanelVisible()
            => this.AlternativeTextPanel.Visible = this.Model.CanUseAltText;

        private void UpdateSelectedMedia()
        {
            using (ControlTransaction.Update(this))
            {
                var selectedMedia = this.Model.SelectedMediaItem;
                if (selectedMedia == null)
                {
                    this.AlternativeTextBox.Text = "";
                    this.AlternativeTextPanel.Enabled = false;
                }
                else
                {
                    this.AlternativeTextBox.Text = selectedMedia.AltText;
                    this.AlternativeTextPanel.Enabled = true;
                }

                var index = this.Model.SelectedMediaItemIndex;
                var listViewSelectedIndex = this.MediaListView.SelectedIndices.Cast<int>().DefaultIfEmpty(-1).Single();
                if (listViewSelectedIndex != index)
                {
                    this.MediaListView.SelectedIndices.Clear();
                    if (index != -1)
                        this.MediaListView.SelectedIndices.Add(index);
                }
            }
        }

        private void UpdateSelectedMediaImage()
            => this.ImageSelectedPicture.Image = this.Model.SelectedMediaItemImage;

        private void ImageServiceCombo_SelectedIndexChanged(object sender, EventArgs e)
            => this.Model.SelectedMediaServiceName = this.ImageServiceCombo.Text;

        private void MediaListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var indices = this.MediaListView.SelectedIndices;
            if (indices.Count == 0)
                return;

            this.Model.SelectedMediaItemIndex = indices[0];
        }

        private void AlternativeTextBox_Validated(object sender, EventArgs e)
            => this.Model.SetSelectedMediaAltText(this.AlternativeTextBox.Text);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.components?.Dispose();
                this.Model.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
