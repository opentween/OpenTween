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
using OpenTween.Api;
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
            public string Path { get; set; }
            public MyCommon.UploadFileType Type { get; set; }
            public string Text { get; set; }

            public SelectedMedia(string path, MyCommon.UploadFileType type, string text)
            {
                this.Path = path;
                this.Type = type;
                this.Text = text;
            }

            public SelectedMedia(string text)
                : this("", MyCommon.UploadFileType.Invalid, text)
            {
            }

            public bool IsValid
            {
                get { return this.Type != MyCommon.UploadFileType.Invalid; }
            }

            public override string ToString()
            {
                return this.Text;
            }
        }

        private Dictionary<string, IMediaUploadService> pictureService;

        private void CreateServices(Twitter tw, TwitterConfiguration twitterConfig)
        {
            if (this.pictureService != null) this.pictureService.Clear();
            this.pictureService = null;

            this.pictureService = new Dictionary<string, IMediaUploadService> {
                {"Twitter", new TwitterPhoto(tw, twitterConfig)},
                {"img.ly", new imgly(tw, twitterConfig)},
                {"yfrog", new yfrog(tw, twitterConfig)},
                {"ついっぷるフォト", new TwipplePhoto(tw, twitterConfig)},
                {"Imgur", new Imgur(tw, twitterConfig)},
                {"Mobypicture", new Mobypicture(tw, twitterConfig)},
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
        /// D&Dをサポートする場合は引数にドロップされたファイル名を指定して呼ぶこと。
        /// </summary>
        public void BeginSelection(string[] fileNames = null)
        {
            if (fileNames != null && fileNames.Length > 0)
            {
                var serviceName = this.ServiceName;
                if (string.IsNullOrEmpty(serviceName)) return;
                var service = this.pictureService[serviceName];

                var count = Math.Min(fileNames.Length, service.MaxMediaCount);
                if (!this.Visible || count > 1)
                {
                    // 非表示時または複数のファイル指定は新規選択として扱う
                    SetImagePageCombo();

                    if (this.BeginSelecting != null)
                        this.BeginSelecting(this, EventArgs.Empty);

                    this.Visible = true;
                }
                this.Enabled = true;

                if (count == 1)
                {
                    ImagefilePathText.Text = fileNames[0];
                    ImageFromSelectedFile(false);
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        var index = ImagePageCombo.Items.Count - 1;
                        if (index == 0) ImagefilePathText.Text = fileNames[i];
                        ImageFromSelectedFile(index, fileNames[i], false);
                    }
                }
            }
            else
            {
                if (!this.Visible)
                {
                    if (this.BeginSelecting != null)
                        this.BeginSelecting(this, EventArgs.Empty);

                    this.Visible = true;
                    this.Enabled = true;
                    ImageFromSelectedFile(true);
                    ImagefilePathText.Focus();
                }
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

                if (this.EndSelecting != null)
                    this.EndSelecting(this, EventArgs.Empty);

                this.Visible = false;
                this.Enabled = false;
                ClearImageSelectedPicture();

                ImagefilePathText.CausesValidation = true;
            }
        }

        /// <summary>
        /// 選択された投稿先名と投稿ファイル名を取得する。
        /// </summary>
        public bool TryGetSelectedMedia(out string imageService, out string[] imagePaths)
        {
            var validPaths = ImagePageCombo.Items.Cast<SelectedMedia>()
                             .Where(x => x.IsValid).Select(x => x.Path).ToArray();

            if (validPaths.Length > 0 &&
                ImageServiceCombo.SelectedIndex > -1)
            {
                var serviceName = this.ServiceName;
                if (MessageBox.Show(string.Format(Properties.Resources.PostPictureConfirm1, serviceName, validPaths.Length),
                                   Properties.Resources.PostPictureConfirm2,
                                   MessageBoxButtons.OKCancel,
                                   MessageBoxIcon.Question,
                                   MessageBoxDefaultButton.Button1)
                               == DialogResult.OK)
                {
                    imageService = serviceName;
                    imagePaths = validPaths;
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
            imagePaths = null;
            return false;
        }

        private void FilePickButton_Click(object sender, EventArgs e)
        {
            if (FilePickDialog == null || string.IsNullOrEmpty(this.ServiceName)) return;
            FilePickDialog.Filter = this.pictureService[this.ServiceName].SupportedFormatsStrForDialog;
            FilePickDialog.Title = Properties.Resources.PickPictureDialog1;
            FilePickDialog.FileName = "";

            if (this.FilePickDialogOpening != null)
                this.FilePickDialogOpening(this, EventArgs.Empty);

            try
            {
                if (FilePickDialog.ShowDialog() == DialogResult.Cancel) return;
            }
            finally
            {
                if (this.FilePickDialogClosed != null)
                    this.FilePickDialogClosed(this, EventArgs.Empty);
            }

            ImagefilePathText.Text = FilePickDialog.FileName;
            ImageFromSelectedFile(false);
        }

        private void ImagefilePathText_Validating(object sender, CancelEventArgs e)
        {
            if (ImageCancelButton.Focused)
            {
                ImagefilePathText.CausesValidation = false;
                return;
            }

            ImageFromSelectedFile(false);
        }

        private void ImageFromSelectedFile(bool suppressMsgBox)
        {
            ImagefilePathText.Text = ImagefilePathText.Text.Trim();
            ImageFromSelectedFile(-1, ImagefilePathText.Text, suppressMsgBox);
        }

        private void ImageFromSelectedFile(int index, string fileName, bool suppressMsgBox)
        {
            var serviceName = this.ServiceName;
            if (string.IsNullOrEmpty(serviceName)) return;

            var selectedIndex = ImagePageCombo.SelectedIndex;
            if (index < 0) index = selectedIndex;

            if (index >= ImagePageCombo.Items.Count)
                throw new ArgumentOutOfRangeException("index");

            var imageService = this.pictureService[serviceName];
            var isSelectedPage = (index == selectedIndex);

            if (isSelectedPage)
                this.ClearImageSelectedPicture();

            if (string.IsNullOrEmpty(fileName))
            {
                ClearImagePage(index);
                return;
            }

            try
            {
                FileInfo fl = new FileInfo(fileName);
                string ext = fl.Extension;

                if (!imageService.CheckFileExtension(ext))
                {
                    //画像以外の形式
                    ClearImagePage(index);
                    if (!suppressMsgBox)
                    {
                        MessageBox.Show(
                            string.Format(Properties.Resources.PostPictureWarn3, serviceName, MakeAvailableServiceText(ext, fl.Length), ext, fl.Name),
                            Properties.Resources.PostPictureWarn4,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    return;
                }

                if (!imageService.CheckFileSize(ext, fl.Length))
                {
                    // ファイルサイズが大きすぎる
                    ClearImagePage(index);
                    if (!suppressMsgBox)
                    {
                        MessageBox.Show(
                            string.Format(Properties.Resources.PostPictureWarn5, serviceName, MakeAvailableServiceText(ext, fl.Length), fl.Name),
                            Properties.Resources.PostPictureWarn4,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    return;
                }

                try
                {
                    using (var fs = File.OpenRead(fileName))
                    {
                        var image = MemoryImage.CopyFromStream(fs);
                        if (isSelectedPage)
                            ImageSelectedPicture.Image = image;
                        else
                            image.Dispose();  //画像チェック後は使わないので破棄する
                    }
                    SetImagePage(index, fileName, MyCommon.UploadFileType.Picture);
                }
                catch (InvalidImageException)
                {
                    SetImagePage(index, fileName, MyCommon.UploadFileType.MultiMedia);
                }
            }
            catch (FileNotFoundException)
            {
                ClearImagePage(index);
                if (!suppressMsgBox) MessageBox.Show("File not found.");
            }
            catch (Exception)
            {
                ClearImagePage(index);
                if (!suppressMsgBox) MessageBox.Show("The type of this file is not image.");
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
            if (oldImage != null)
            {
                this.ImageSelectedPicture.Image = null;
                oldImage.Dispose();
            }

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
                var serviceName = this.ServiceName;
                if (!string.IsNullOrEmpty(serviceName))
                {
                    if (ImagePageCombo.Items.Count > 0)
                    {
                        // 画像が選択された投稿先に対応しているかをチェックする
                        // TODO: 複数の選択済み画像があるなら、できれば全てを再チェックしたほうがいい
                        if (serviceName.Equals("Twitter"))
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

                                try
                                {
                                    FileInfo fi = new FileInfo(ImagefilePathText.Text.Trim());
                                    string ext = fi.Extension;
                                    var imageService = this.pictureService[serviceName];
                                    if (!imageService.CheckFileExtension(ext) ||
                                        !imageService.CheckFileSize(ext, fi.Length))
                                    {
                                        ClearImageSelectedPicture();
                                        ClearSelectedImagePage();
                                    }
                                }
                                catch (Exception)
                                {
                                    ClearImageSelectedPicture();
                                    ClearSelectedImagePage();
                                }
                            }
                        }
                    }
                }
            }

            if (this.SelectedServiceChanged != null)
                this.SelectedServiceChanged(this, EventArgs.Empty);
        }

        private void SetImagePageCombo(SelectedMedia media = null)
        {
            using (ControlTransaction.Update(ImagePageCombo))
            {
                ImagePageCombo.Enabled = false;
                ImagePageCombo.Items.Clear();
                if (media != null)
                {
                    ImagePageCombo.Items.Add(media);
                    ImagefilePathText.Text = media.Path;
                }
                else
                {
                    ImagePageCombo.Items.Add(new SelectedMedia("1"));
                    ImagefilePathText.Text = "";
                }
                ImagePageCombo.SelectedIndex = 0;
            }
        }

        private void AddNewImagePage(int selectedIndex)
        {
            var serviceName = this.ServiceName;
            if (string.IsNullOrEmpty(serviceName)) return;

            if (selectedIndex < this.pictureService[serviceName].MaxMediaCount - 1)
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

        private void SetSelectedImagePage(string path, MyCommon.UploadFileType type)
        {
            SetImagePage(-1, path, type);
        }

        private void SetImagePage(int index, string path, MyCommon.UploadFileType type)
        {
            var selectedIndex = ImagePageCombo.SelectedIndex;
            if (index < 0) index = selectedIndex;

            var item = (SelectedMedia)ImagePageCombo.Items[index];
            item.Path = path;
            item.Type = type;

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

            var item = (SelectedMedia)ImagePageCombo.Items[index];
            item.Path = "";
            item.Type = MyCommon.UploadFileType.Invalid;

            if (index == selectedIndex) ImagefilePathText.Text = "";
        }

        private void ValidateSelectedImagePage()
        {
            var idx = ImagePageCombo.SelectedIndex;
            var item = (SelectedMedia)ImagePageCombo.Items[idx];
            ImageServiceCombo.Enabled = (idx == 0);  // idx == 0 以外では投稿先サービスを選べないようにする
            ImagefilePathText.Text = item.Path;
            ImageFromSelectedFile(true);
        }

        private void ImagePageCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateSelectedImagePage();
        }
    }
}
