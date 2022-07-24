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
    public partial class MediaSelector : UserControl
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

        private class SelectedMedia
        {
            public IMediaItem? Item { get; set; }

            public MyCommon.UploadFileType Type { get; set; }

            public string Text { get; set; }

            public SelectedMedia(IMediaItem? item, MyCommon.UploadFileType type, string text)
            {
                this.Item = item;
                this.Type = type;
                this.Text = text;
            }

            public SelectedMedia(string text)
                : this(null, MyCommon.UploadFileType.Invalid, text)
            {
            }

            public bool IsValid
                => this.Item != null && this.Type != MyCommon.UploadFileType.Invalid;

            public string Path
                => this.Item?.Path ?? "";

            public string AltText => this.Item?.AltText ?? "";

            public override string ToString()
                => this.Text;
        }

        private Dictionary<string, IMediaUploadService> pictureService = new();

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

        public MediaSelector()
        {
            this.InitializeComponent();

            this.ImageSelectedPicture.InitialImage = Properties.Resources.InitialImage;
        }

        /// <summary>
        /// 投稿先サービスなどを初期化する。
        /// </summary>
        public void Initialize(Twitter tw, TwitterConfiguration twitterConfig, string svc, int? index = null)
        {
            this.CreateServices(tw, twitterConfig);

            this.SetImageServiceCombo();
            this.SetImagePageCombo();

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

        /// <summary>
        /// 投稿するファイルとその投稿先を選択するためのコントロールを表示する。
        /// </summary>
        private void BeginSelection(IMediaItem[] items)
        {
            if (items == null || items.Length == 0)
            {
                this.BeginSelection();
                return;
            }

            var service = this.SelectedService;
            if (service == null) return;

            var count = Math.Min(items.Length, service.MaxMediaCount);
            if (!this.Visible || count > 1)
            {
                // 非表示時または複数のファイル指定は新規選択として扱う
                this.SetImagePageCombo();

                this.BeginSelecting?.Invoke(this, EventArgs.Empty);

                this.Visible = true;
            }
            this.Enabled = true;

            if (count == 1)
            {
                this.ImagefilePathText.Text = items[0].Path;
                this.AlternativeTextBox.Text = items[0].AltText;
                this.ImageFromSelectedFile(items[0], false);
            }
            else
            {
                for (var i = 0; i < count; i++)
                {
                    var index = this.ImagePageCombo.Items.Count - 1;
                    if (index == 0)
                    {
                        this.ImagefilePathText.Text = items[i].Path;
                        this.AlternativeTextBox.Text = items[i].AltText;
                    }
                    this.ImageFromSelectedFile(index, items[i], false);
                }
            }
        }

        /// <summary>
        /// 投稿するファイルとその投稿先を選択するためのコントロールを表示する（主にD&amp;D用）。
        /// </summary>
        public void BeginSelection(string[] fileNames)
        {
            if (fileNames == null || fileNames.Length == 0)
            {
                this.BeginSelection();
                return;
            }

            var items = fileNames.Select(x => this.CreateFileMediaItem(x, false)).OfType<IMediaItem>().ToArray();
            this.BeginSelection(items);
        }

        /// <summary>
        /// 投稿するファイルとその投稿先を選択するためのコントロールを表示する。
        /// </summary>
        public void BeginSelection(Image image)
        {
            if (image == null)
            {
                this.BeginSelection();
                return;
            }

            var items = new[] { this.CreateMemoryImageMediaItem(image, false) }.OfType<IMediaItem>().ToArray();
            this.BeginSelection(items);
        }

        /// <summary>
        /// 投稿するファイルとその投稿先を選択するためのコントロールを表示する。
        /// </summary>
        public void BeginSelection()
        {
            if (!this.Visible)
            {
                this.BeginSelecting?.Invoke(this, EventArgs.Empty);

                this.Visible = true;
                this.Enabled = true;

                var media = (SelectedMedia)this.ImagePageCombo.SelectedItem;
                this.ImageFromSelectedFile(media.Item, true);
                this.ImagefilePathText.Focus();
            }
        }

        /// <summary>
        /// 選択処理を終了してコントロールを隠す。
        /// </summary>
        public void EndSelection()
        {
            if (this.Visible)
            {
                this.ImagefilePathText.CausesValidation = false;

                this.EndSelecting?.Invoke(this, EventArgs.Empty);

                this.Visible = false;
                this.Enabled = false;
                this.ClearImageSelectedPicture();

                this.ImagefilePathText.CausesValidation = true;
            }
        }

        /// <summary>
        /// 選択された投稿先名と投稿する MediaItem を取得する。MediaItem は不要になったら呼び出し側にて破棄すること。
        /// </summary>
        public bool TryGetSelectedMedia([NotNullWhen(true)] out string? imageService, [NotNullWhen(true)] out IMediaItem[]? mediaItems)
        {
            var validItems = this.ImagePageCombo.Items.Cast<SelectedMedia>()
                             .Where(x => x.IsValid).Select(x => x.Item).OfType<IMediaItem>().ToArray();

            if (validItems.Length > 0 &&
                this.ImageServiceCombo.SelectedIndex > -1)
            {
                var serviceName = this.ServiceName;
                if (MessageBox.Show(string.Format(Properties.Resources.PostPictureConfirm1, serviceName, validItems.Length),
                                   Properties.Resources.PostPictureConfirm2,
                                   MessageBoxButtons.OKCancel,
                                   MessageBoxIcon.Question,
                                   MessageBoxDefaultButton.Button1)
                               == DialogResult.OK)
                {
                    // 収集した MediaItem が破棄されないように、予め null を代入しておく
                    foreach (SelectedMedia media in this.ImagePageCombo.Items)
                    {
                        if (media != null) media.Item = null;
                    }

                    imageService = serviceName;
                    mediaItems = validItems;
                    this.EndSelection();
                    this.SetImagePageCombo();
                    return true;
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.PostPictureWarn1, Properties.Resources.PostPictureWarn2);
            }

            imageService = null;
            mediaItems = null;
            return false;
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

        private void ValidateNewFileMediaItem(string path, string altText, bool noMsgBox)
        {
            var media = (SelectedMedia)this.ImagePageCombo.SelectedItem;
            var item = media.Item;

            if (path != media.Path)
            {
                this.DisposeMediaItem(media.Item);
                media.Item = null;

                item = this.CreateFileMediaItem(path, noMsgBox);
            }

            if (item != null)
                item.AltText = altText;

            this.ImagefilePathText.Text = path;
            this.AlternativeTextBox.Text = altText;
            this.ImageFromSelectedFile(item, noMsgBox);
        }

        private void DisposeMediaItem(IMediaItem? item)
        {
            var disposableItem = item as IDisposable;
            disposableItem?.Dispose();
        }

        private void FilePickButton_Click(object sender, EventArgs e)
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

            this.ValidateNewFileMediaItem(this.FilePickDialog.FileName, this.AlternativeTextBox.Text.Trim(), false);
        }

        private void ImagefilePathText_Validating(object sender, CancelEventArgs e)
        {
            if (this.ImageCancelButton.Focused)
            {
                this.ImagefilePathText.CausesValidation = false;
                return;
            }

            this.ValidateNewFileMediaItem(this.ImagefilePathText.Text.Trim(), this.AlternativeTextBox.Text.Trim(), false);
        }

        private void ImageFromSelectedFile(IMediaItem? item, bool noMsgBox)
            => this.ImageFromSelectedFile(-1, item, noMsgBox);

        private void ImageFromSelectedFile(int index, IMediaItem? item, bool noMsgBox)
        {
            var valid = false;

            try
            {
                var imageService = this.SelectedService;
                if (imageService == null) return;

                var selectedIndex = this.ImagePageCombo.SelectedIndex;
                if (index < 0) index = selectedIndex;

                if (index >= this.ImagePageCombo.Items.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var isSelectedPage = index == selectedIndex;

                if (isSelectedPage)
                    this.ClearImageSelectedPicture();

                if (item == null || MyCommon.IsNullOrEmpty(item.Path)) return;

                try
                {
                    var ext = item.Extension;
                    var size = item.Size;

                    if (!imageService.CheckFileExtension(ext))
                    {
                        // 画像以外の形式
                        if (!noMsgBox)
                        {
                            MessageBox.Show(
                                string.Format(Properties.Resources.PostPictureWarn3, this.ServiceName, this.MakeAvailableServiceText(ext, size), ext, item.Name),
                                Properties.Resources.PostPictureWarn4,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                        return;
                    }

                    if (!imageService.CheckFileSize(ext, size))
                    {
                        // ファイルサイズが大きすぎる
                        if (!noMsgBox)
                        {
                            MessageBox.Show(
                                string.Format(Properties.Resources.PostPictureWarn5, this.ServiceName, this.MakeAvailableServiceText(ext, size), item.Name),
                                Properties.Resources.PostPictureWarn4,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                        return;
                    }

                    if (item.IsImage)
                    {
                        if (isSelectedPage)
                            this.ImageSelectedPicture.Image = item.CreateImage();
                        this.SetImagePage(index, item, MyCommon.UploadFileType.Picture);
                    }
                    else
                    {
                        this.SetImagePage(index, item, MyCommon.UploadFileType.MultiMedia);
                    }

                    valid = true;  // 正常終了
                }
                catch (FileNotFoundException)
                {
                    if (!noMsgBox) MessageBox.Show("File not found.");
                }
                catch (Exception)
                {
                    if (!noMsgBox) MessageBox.Show("The type of this file is not image.");
                }
            }
            finally
            {
                if (!valid)
                {
                    this.ClearImagePage(index);
                    this.DisposeMediaItem(item);
                }
            }
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

        private void ClearImageSelectedPicture()
        {
            var oldImage = this.ImageSelectedPicture.Image;
            this.ImageSelectedPicture.Image = null;
            oldImage?.Dispose();

            this.ImageSelectedPicture.ShowInitialImage();
        }

        private void ImageCancelButton_Click(object sender, EventArgs e)
            => this.EndSelection();

        private void ImageSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.EndSelection();
            }
        }

        private void ImageSelection_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 0x1B)
            {
                this.ImagefilePathText.CausesValidation = false;
                e.Handled = true;
            }
        }

        private void ImageSelection_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.ImagefilePathText.CausesValidation = false;
            }
        }

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

        private void ImageServiceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                var imageService = this.SelectedService;
                if (imageService != null)
                {
                    this.UpdateAltTextPanelVisible();

                    if (this.ImagePageCombo.Items.Count > 0)
                    {
                        // 画像が選択された投稿先に対応しているかをチェックする
                        // TODO: 複数の選択済み画像があるなら、できれば全てを再チェックしたほうがいい
                        if (this.ServiceName == "Twitter")
                        {
                            this.ValidateSelectedImagePage();
                        }
                        else
                        {
                            if (this.ImagePageCombo.Items.Count > 1)
                            {
                                // 複数の選択済み画像のうち、1枚目のみを残す
                                this.SetImagePageCombo((SelectedMedia)this.ImagePageCombo.Items[0]);
                            }
                            else
                            {
                                this.ImagePageCombo.Enabled = false;
                                var valid = false;

                                try
                                {
                                    var item = ((SelectedMedia)this.ImagePageCombo.Items[0]).Item;
                                    if (item != null)
                                    {
                                        var ext = item.Extension;
                                        if (imageService.CheckFileExtension(ext) &&
                                            imageService.CheckFileSize(ext, item.Size))
                                        {
                                            valid = true;
                                        }
                                    }
                                }
                                catch
                                {
                                }
                                finally
                                {
                                    if (!valid)
                                    {
                                        this.ClearImageSelectedPicture();
                                        this.ClearSelectedImagePage();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            this.SelectedServiceChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetImagePageCombo(SelectedMedia? media = null)
        {
            using (ControlTransaction.Update(this.ImagePageCombo))
            {
                this.ImagePageCombo.Enabled = false;

                foreach (SelectedMedia oldMedia in this.ImagePageCombo.Items)
                {
                    if (oldMedia == null || oldMedia == media) continue;
                    this.DisposeMediaItem(oldMedia.Item);
                }
                this.ImagePageCombo.Items.Clear();

                if (media == null)
                    media = new SelectedMedia("1");

                this.ImagePageCombo.Items.Add(media);
                this.ImagefilePathText.Text = media.Path;
                this.AlternativeTextBox.Text = media.AltText;

                this.ImagePageCombo.SelectedIndex = 0;
            }
        }

        private void AddNewImagePage(int selectedIndex)
        {
            var service = this.SelectedService;
            if (service == null) return;

            if (selectedIndex < service.MaxMediaCount - 1)
            {
                // 投稿先の投稿可能枚数まで選択できるようにする
                var count = this.ImagePageCombo.Items.Count;
                if (selectedIndex == count - 1)
                {
                    count++;
                    this.ImagePageCombo.Items.Add(new SelectedMedia(count.ToString()));
                    this.ImagePageCombo.Enabled = true;
                }
            }
        }

        private void SetSelectedImagePage(IMediaItem item, MyCommon.UploadFileType type)
            => this.SetImagePage(-1, item, type);

        private void SetImagePage(int index, IMediaItem item, MyCommon.UploadFileType type)
        {
            var selectedIndex = this.ImagePageCombo.SelectedIndex;
            if (index < 0) index = selectedIndex;

            var media = (SelectedMedia)this.ImagePageCombo.Items[index];
            if (media.Item != item)
            {
                this.DisposeMediaItem(media.Item);
                media.Item = item;
            }
            media.Type = type;

            this.AddNewImagePage(index);
        }

        private void ClearSelectedImagePage()
            => this.ClearImagePage(-1);

        private void ClearImagePage(int index)
        {
            var selectedIndex = this.ImagePageCombo.SelectedIndex;
            if (index < 0) index = selectedIndex;

            var media = (SelectedMedia)this.ImagePageCombo.Items[index];
            this.DisposeMediaItem(media.Item);
            media.Item = null;
            media.Type = MyCommon.UploadFileType.Invalid;

            if (index == selectedIndex)
            {
                this.ImagefilePathText.Text = "";
                this.AlternativeTextBox.Text = "";
            }
        }

        private void ValidateSelectedImagePage()
        {
            var idx = this.ImagePageCombo.SelectedIndex;
            var media = (SelectedMedia)this.ImagePageCombo.Items[idx];
            this.ImageServiceCombo.Enabled = idx == 0;  // idx == 0 以外では投稿先サービスを選べないようにする
            this.ImagefilePathText.Text = media.Path;
            this.AlternativeTextBox.Text = media.AltText;
            this.ImageFromSelectedFile(media.Item, true);
        }

        private void ImagePageCombo_SelectedIndexChanged(object sender, EventArgs e)
            => this.ValidateSelectedImagePage();

        private void AlternativeTextBox_Validating(object sender, CancelEventArgs e)
        {
            var imageFilePath = this.ImagefilePathText.Text.Trim();
            var altText = this.AlternativeTextBox.Text.Trim();
            this.ValidateNewFileMediaItem(imageFilePath, altText, noMsgBox: false);
        }
    }
}
