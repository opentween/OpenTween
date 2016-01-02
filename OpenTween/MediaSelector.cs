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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Api.DataModel;
using OpenTween.Connection;

namespace OpenTween
{
    public partial class MediaSelector : UserControl
    {
        public event EventHandler BeginSelecting;
        public event EventHandler EndSelecting;

        public event EventHandler FilePickDialogOpening;
        public event EventHandler FilePickDialogClosed;

        public event EventHandler SelectedServiceChanged;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public OpenFileDialog FilePickDialog { get; set; }

        /// <summary>
        /// 選択されている投稿先名を取得する。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ServiceName
        {
            get { return ImageServiceCombo.Text; }
        }

        /// <summary>
        /// 選択されている投稿先を示すインデックスを取得する。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ServiceIndex
        {
            get { return ImageServiceCombo.SelectedIndex; }
        }

        /// <summary>
        /// 選択されている投稿先の IMediaUploadService を取得する。
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IMediaUploadService SelectedService
        {
            get
            {
                var serviceName = this.ServiceName;
                if (string.IsNullOrEmpty(serviceName))
                    return null;

                IMediaUploadService service;
                return this.pictureService.TryGetValue(serviceName, out service) ? service : null;
            }
        }

        /// <summary>
        /// 指定された投稿先名から、作成済みの IMediaUploadService インスタンスを取得する。
        /// </summary>
        public IMediaUploadService GetService(string serviceName)
        {
            IMediaUploadService service;
            this.pictureService.TryGetValue(serviceName, out service);
            return service;
        }

        /// <summary>
        /// 利用可能な全ての IMediaUploadService インスタンスを取得する。
        /// </summary>
        public ICollection<IMediaUploadService> GetServices()
        {
            return this.pictureService.Values;
        }

        private class SelectedMedia
        {
            public IMediaItem Item { get; set; }
            public MyCommon.UploadFileType Type { get; set; }
            public string Text { get; set; }

            public SelectedMedia(IMediaItem item, MyCommon.UploadFileType type, string text)
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
            {
                get
                {
                    return this.Item != null && this.Type != MyCommon.UploadFileType.Invalid;
                }
            }

            public string Path
            {
                get
                {
                    return this.Item?.Path ?? "";
                }
            }

            public override string ToString()
            {
                return this.Text;
            }
        }

        private Dictionary<string, IMediaUploadService> pictureService;

        private void CreateServices(Twitter tw, TwitterConfiguration twitterConfig)
        {
            this.pictureService?.Clear();
            this.pictureService = null;

            this.pictureService = new Dictionary<string, IMediaUploadService> {
                ["Twitter"] = new TwitterPhoto(tw, twitterConfig),
                ["img.ly"] = new imgly(tw, twitterConfig),
                ["yfrog"] = new yfrog(tw, twitterConfig),
                ["ついっぷるフォト"] = new TwipplePhoto(tw, twitterConfig),
                ["Imgur"] = new Imgur(tw, twitterConfig),
                ["Mobypicture"] = new Mobypicture(tw, twitterConfig),
            };
        }

        public MediaSelector()
        {
            InitializeComponent();

            this.ImageSelectedPicture.InitialImage = Properties.Resources.InitialImage;
        }

        /// <summary>
        /// 投稿先サービスなどを初期化する。
        /// </summary>
        public void Initialize(Twitter tw, TwitterConfiguration twitterConfig, string svc, int? index = null)
        {
            CreateServices(tw, twitterConfig);

            SetImageServiceCombo();
            SetImagePageCombo();

            SelectImageServiceComboItem(svc, index);
        }

        /// <summary>
        /// 投稿先サービスを再作成する。
        /// </summary>
        public void Reset(Twitter tw, TwitterConfiguration twitterConfig)
        {
            CreateServices(tw, twitterConfig);

            SetImageServiceCombo();
        }

        /// <summary>
        /// 指定されたファイルの投稿に対応した投稿先があるかどうかを示す値を取得する。
        /// </summary>
        public bool HasUploadableService(string fileName, bool ignoreSize)
        {
            FileInfo fl = new FileInfo(fileName);
            string ext = fl.Extension;
            long? size = ignoreSize ? (long?)null : fl.Length;

            if (IsUploadable(this.ServiceName, ext, size))
                return true;

            foreach (string svc in ImageServiceCombo.Items)
            {
                if (IsUploadable(svc, ext, size))
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
            if (!string.IsNullOrEmpty(serviceName))
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
                BeginSelection();
                return;
            }

            var service = this.SelectedService;
            if (service == null) return;

            var count = Math.Min(items.Length, service.MaxMediaCount);
            if (!this.Visible || count > 1)
            {
                // 非表示時または複数のファイル指定は新規選択として扱う
                SetImagePageCombo();

                this.BeginSelecting?.Invoke(this, EventArgs.Empty);

                this.Visible = true;
            }
            this.Enabled = true;

            if (count == 1)
            {
                ImagefilePathText.Text = items[0].Path;
                ImageFromSelectedFile(items[0], false);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    var index = ImagePageCombo.Items.Count - 1;
                    if (index == 0) ImagefilePathText.Text = items[i].Path;
                    ImageFromSelectedFile(index, items[i], false);
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
                BeginSelection();
                return;
            }

            var items = fileNames.Select(x => CreateFileMediaItem(x, false)).OfType<IMediaItem>().ToArray();
            BeginSelection(items);
        }

        /// <summary>
        /// 投稿するファイルとその投稿先を選択するためのコントロールを表示する。
        /// </summary>
        public void BeginSelection(Image image)
        {
            if (image == null)
            {
                BeginSelection();
                return;
            }

            var items = new [] { CreateMemoryImageMediaItem(image, false) }.OfType<IMediaItem>().ToArray();
            BeginSelection(items);
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

                var media = (SelectedMedia)ImagePageCombo.SelectedItem;
                ImageFromSelectedFile(media.Item, true);
                ImagefilePathText.Focus();
            }
        }

        /// <summary>
        /// 選択処理を終了してコントロールを隠す。
        /// </summary>
        public void EndSelection()
        {
            if (this.Visible)
            {
                ImagefilePathText.CausesValidation = false;

                this.EndSelecting?.Invoke(this, EventArgs.Empty);

                this.Visible = false;
                this.Enabled = false;
                ClearImageSelectedPicture();

                ImagefilePathText.CausesValidation = true;
            }
        }

        /// <summary>
        /// 選択された投稿先名と投稿する MediaItem を取得する。MediaItem は不要になったら呼び出し側にて破棄すること。
        /// </summary>
        public bool TryGetSelectedMedia(out string imageService, out IMediaItem[] mediaItems)
        {
            var validItems = ImagePageCombo.Items.Cast<SelectedMedia>()
                             .Where(x => x.IsValid).Select(x => x.Item).OfType<IMediaItem>().ToArray();

            if (validItems.Length > 0 &&
                ImageServiceCombo.SelectedIndex > -1)
            {
                var serviceName = this.ServiceName;
                if (MessageBox.Show(string.Format(Properties.Resources.PostPictureConfirm1, serviceName, validItems.Length),
                                   Properties.Resources.PostPictureConfirm2,
                                   MessageBoxButtons.OKCancel,
                                   MessageBoxIcon.Question,
                                   MessageBoxDefaultButton.Button1)
                               == DialogResult.OK)
                {
                    //収集した MediaItem が破棄されないように、予め null を代入しておく
                    foreach (SelectedMedia media in ImagePageCombo.Items)
                    {
                        if (media != null) media.Item = null;
                    }

                    imageService = serviceName;
                    mediaItems = validItems;
                    EndSelection();
                    SetImagePageCombo();
                    return true;
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.PostPictureWarn1, Properties.Resources.PostPictureWarn2);
            }

            EndSelection();
            imageService = null;
            mediaItems = null;
            return false;
        }

        private MemoryImageMediaItem CreateMemoryImageMediaItem(Image image, bool noMsgBox)
        {
            if (image == null) return null;

            MemoryImage memoryImage = null;
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

        private IMediaItem CreateFileMediaItem(string path, bool noMsgBox)
        {
            if (string.IsNullOrEmpty(path)) return null;

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

        private void ValidateNewFileMediaItem(string path, bool noMsgBox)
        {
            var media = (SelectedMedia)ImagePageCombo.SelectedItem;
            var item = media.Item;

            if (path != media.Path)
            {
                DisposeMediaItem(media.Item);
                media.Item = null;

                item = CreateFileMediaItem(path, noMsgBox);
            }

            ImagefilePathText.Text = path;
            ImageFromSelectedFile(item, noMsgBox);
        }

        private void DisposeMediaItem(IMediaItem item)
        {
            var disposableItem = item as IDisposable;
            disposableItem?.Dispose();
        }

        private void FilePickButton_Click(object sender, EventArgs e)
        {
            var service = this.SelectedService;

            if (FilePickDialog == null || service == null) return;
            FilePickDialog.Filter = service.SupportedFormatsStrForDialog;
            FilePickDialog.Title = Properties.Resources.PickPictureDialog1;
            FilePickDialog.FileName = "";

            this.FilePickDialogOpening?.Invoke(this, EventArgs.Empty);

            try
            {
                if (FilePickDialog.ShowDialog() == DialogResult.Cancel) return;
            }
            finally
            {
                this.FilePickDialogClosed?.Invoke(this, EventArgs.Empty);
            }

            ValidateNewFileMediaItem(FilePickDialog.FileName, false);
        }

        private void ImagefilePathText_Validating(object sender, CancelEventArgs e)
        {
            if (ImageCancelButton.Focused)
            {
                ImagefilePathText.CausesValidation = false;
                return;
            }

            ValidateNewFileMediaItem(ImagefilePathText.Text.Trim(), false);
        }

        private void ImageFromSelectedFile(IMediaItem item, bool noMsgBox)
        {
            ImageFromSelectedFile(-1, item, noMsgBox);
        }

        private void ImageFromSelectedFile(int index, IMediaItem item, bool noMsgBox)
        {
            var valid = false;

            try
            {
                var imageService = this.SelectedService;
                if (imageService == null) return;

                var selectedIndex = ImagePageCombo.SelectedIndex;
                if (index < 0) index = selectedIndex;

                if (index >= ImagePageCombo.Items.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                var isSelectedPage = (index == selectedIndex);

                if (isSelectedPage)
                    this.ClearImageSelectedPicture();

                if (item == null || string.IsNullOrEmpty(item.Path)) return;

                try
                {
                    var ext = item.Extension;
                    var size = item.Size;

                    if (!imageService.CheckFileExtension(ext))
                    {
                        //画像以外の形式
                        if (!noMsgBox)
                        {
                            MessageBox.Show(
                                string.Format(Properties.Resources.PostPictureWarn3, this.ServiceName, MakeAvailableServiceText(ext, size), ext, item.Name),
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
                                string.Format(Properties.Resources.PostPictureWarn5, this.ServiceName, MakeAvailableServiceText(ext, size), item.Name),
                                Properties.Resources.PostPictureWarn4,
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                        return;
                    }

                    if (item.IsImage)
                    {
                        if (isSelectedPage)
                            ImageSelectedPicture.Image = item.CreateImage();
                        SetImagePage(index, item, MyCommon.UploadFileType.Picture);
                    }
                    else
                    {
                        SetImagePage(index, item, MyCommon.UploadFileType.MultiMedia);
                    }

                    valid = true;  //正常終了
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
                    ClearImagePage(index);
                    DisposeMediaItem(item);
                }
            }
        }

        private string MakeAvailableServiceText(string ext, long fileSize)
        {
            var text = string.Join(", ",
                ImageServiceCombo.Items.Cast<string>()
                    .Where(serviceName =>
                        !string.IsNullOrEmpty(serviceName) &&
                        this.pictureService[serviceName].CheckFileExtension(ext) &&
                        this.pictureService[serviceName].CheckFileSize(ext, fileSize)));

            if (string.IsNullOrEmpty(text))
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
        {
            EndSelection();
        }

        private void ImageSelection_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                EndSelection();
            }
        }

        private void ImageSelection_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Convert.ToInt32(e.KeyChar) == 0x1B)
            {
                ImagefilePathText.CausesValidation = false;
                e.Handled = true;
            }
        }

        private void ImageSelection_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ImagefilePathText.CausesValidation = false;
            }
        }

        private void SetImageServiceCombo()
        {
            using (ControlTransaction.Update(ImageServiceCombo))
            {
                string svc = "";
                if (ImageServiceCombo.SelectedIndex > -1) svc = ImageServiceCombo.Text;
                ImageServiceCombo.Items.Clear();

                // Add service names to combobox
                foreach (var key in pictureService.Keys)
                {
                    ImageServiceCombo.Items.Add(key);
                }

                SelectImageServiceComboItem(svc);
            }
        }

        private void SelectImageServiceComboItem(string svc, int? index = null)
        {
            int idx;
            if (string.IsNullOrEmpty(svc))
            {
                idx = index ?? 0;
            }
            else
            {
                idx = ImageServiceCombo.Items.IndexOf(svc);
                if (idx == -1) idx = index ?? 0;
            }

            try
            {
                ImageServiceCombo.SelectedIndex = idx;
            }
            catch (ArgumentOutOfRangeException)
            {
                ImageServiceCombo.SelectedIndex = 0;
            }
        }

        private void ImageServiceCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                var imageService = this.SelectedService;
                if (imageService != null)
                {
                    if (ImagePageCombo.Items.Count > 0)
                    {
                        // 画像が選択された投稿先に対応しているかをチェックする
                        // TODO: 複数の選択済み画像があるなら、できれば全てを再チェックしたほうがいい
                        if (this.ServiceName == "Twitter")
                        {
                            ValidateSelectedImagePage();
                        }
                        else
                        {
                            if (ImagePageCombo.Items.Count > 1)
                            {
                                // 複数の選択済み画像のうち、1枚目のみを残す
                                SetImagePageCombo((SelectedMedia)ImagePageCombo.Items[0]);
                            }
                            else
                            {
                                ImagePageCombo.Enabled = false;
                                var valid = false;

                                try
                                {
                                    var item = ((SelectedMedia)ImagePageCombo.Items[0]).Item;
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
                                        ClearImageSelectedPicture();
                                        ClearSelectedImagePage();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            this.SelectedServiceChanged?.Invoke(this, EventArgs.Empty);
        }

        private void SetImagePageCombo(SelectedMedia media = null)
        {
            using (ControlTransaction.Update(ImagePageCombo))
            {
                ImagePageCombo.Enabled = false;

                foreach (SelectedMedia oldMedia in ImagePageCombo.Items)
                {
                    if (oldMedia == null || oldMedia == media) continue;
                    DisposeMediaItem(oldMedia.Item);
                }
                ImagePageCombo.Items.Clear();

                if (media == null)
                    media = new SelectedMedia("1");

                ImagePageCombo.Items.Add(media);
                ImagefilePathText.Text = media.Path;

                ImagePageCombo.SelectedIndex = 0;
            }
        }

        private void AddNewImagePage(int selectedIndex)
        {
            var service = this.SelectedService;
            if (service == null) return;

            if (selectedIndex < service.MaxMediaCount - 1)
            {
                // 投稿先の投稿可能枚数まで選択できるようにする
                var count = ImagePageCombo.Items.Count;
                if (selectedIndex == count - 1)
                {
                    count++;
                    ImagePageCombo.Items.Add(new SelectedMedia(count.ToString()));
                    ImagePageCombo.Enabled = true;
                }
            }
        }

        private void SetSelectedImagePage(IMediaItem item, MyCommon.UploadFileType type)
        {
            SetImagePage(-1, item, type);
        }

        private void SetImagePage(int index, IMediaItem item, MyCommon.UploadFileType type)
        {
            var selectedIndex = ImagePageCombo.SelectedIndex;
            if (index < 0) index = selectedIndex;

            var media = (SelectedMedia)ImagePageCombo.Items[index];
            if (media.Item != item)
            {
                DisposeMediaItem(media.Item);
                media.Item = item;
            }
            media.Type = type;

            AddNewImagePage(index);
        }

        private void ClearSelectedImagePage()
        {
            ClearImagePage(-1);
        }

        private void ClearImagePage(int index)
        {
            var selectedIndex = ImagePageCombo.SelectedIndex;
            if (index < 0) index = selectedIndex;

            var media = (SelectedMedia)ImagePageCombo.Items[index];
            DisposeMediaItem(media.Item);
            media.Item = null;
            media.Type = MyCommon.UploadFileType.Invalid;

            if (index == selectedIndex) ImagefilePathText.Text = "";
        }

        private void ValidateSelectedImagePage()
        {
            var idx = ImagePageCombo.SelectedIndex;
            var media = (SelectedMedia)ImagePageCombo.Items[idx];
            ImageServiceCombo.Enabled = (idx == 0);  // idx == 0 以外では投稿先サービスを選べないようにする
            ImagefilePathText.Text = media.Path;
            ImageFromSelectedFile(media.Item, true);
        }

        private void ImagePageCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateSelectedImagePage();
        }
    }
}
