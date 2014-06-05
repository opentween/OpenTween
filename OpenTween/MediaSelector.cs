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
        /// 指定された投稿先名から、作成済みの IMultimediaShareService インスタンスを取得する。
        /// </summary>
        public IMultimediaShareService GetService(string serviceName)
        {
            IMultimediaShareService service;
            this.pictureService.TryGetValue(serviceName, out service);
            return service;
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

        private Dictionary<string, IMultimediaShareService> pictureService;

        private void CreateServices(Twitter tw)
        {
            if (this.pictureService != null) this.pictureService.Clear();
            this.pictureService = null;

            this.pictureService = new Dictionary<string, IMultimediaShareService> {
                {"TwitPic", new TwitPic(tw)},
                {"img.ly", new imgly(tw)},
                {"yfrog", new yfrog(tw)},
                {"Twitter", new TwitterPhoto(tw)},
                {"ついっぷるフォト", new TwipplePhoto(tw)},
                {"Imgur", new Imgur(tw)},
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
        public void Initialize(Twitter tw, string svc, int? index = null)
        {
            CreateServices(tw);

            SetImageServiceCombo();
            SetImagePageCombo();

            SelectImageServiceComboItem(svc, index);
        }

        /// <summary>
        /// 投稿先サービスを再作成する。
        /// </summary>
        public void Reset(Twitter tw)
        {
            CreateServices(tw);

            SetImageServiceCombo();
        }

        /// <summary>
        /// 指定されたファイルの投稿に対応した投稿先があるかどうかを示す値を取得する。
        /// </summary>
        public bool HasUploadableService(string fileName)
        {
            FileInfo fl = new FileInfo(fileName);
            string ext = fl.Extension;

            var serviceName = this.ServiceName;
            if (!string.IsNullOrEmpty(serviceName) &&
                this.pictureService[serviceName].CheckValidFilesize(ext, fl.Length))
            {
                return true;
            }

            foreach (string svc in ImageServiceCombo.Items)
            {
                if (!string.IsNullOrEmpty(svc) &&
                    this.pictureService[svc].CheckValidFilesize(ext, fl.Length))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 投稿するファイルとその投稿先を選択するためのコントロールを表示する。
        /// D&Dをサポートする場合は引数にドロップされたファイル名を指定して呼ぶこと。
        /// </summary>
        public void BeginSelection(string fileName = null)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                if (!this.Visible)
                {
                    // 非表示時のファイル指定は新規選択として扱う
                    SetImagePageCombo();

                    if (this.BeginSelecting != null)
                        this.BeginSelecting(this, EventArgs.Empty);

                    this.Visible = true;
                }
                this.Enabled = true;
                ImagefilePathText.Text = fileName;
                ImageFromSelectedFile(false);
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
            FilePickDialog.Filter = this.pictureService[this.ServiceName].GetFileOpenDialogFilter();
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
            this.ClearImageSelectedPicture();

            try
            {
                ImagefilePathText.Text = ImagefilePathText.Text.Trim();
                var fileName = ImagefilePathText.Text;
                var serviceName = this.ServiceName;
                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(serviceName))
                {
                    ClearSelectedImagePage();
                    return;
                }

                FileInfo fl = new FileInfo(fileName);
                string ext = fl.Extension;
                var imageService = this.pictureService[serviceName];

                if (!imageService.CheckValidExtension(ext))
                {
                    //画像以外の形式
                    ClearSelectedImagePage();
                    if (!suppressMsgBox)
                    {
                        MessageBox.Show(
                            string.Format(Properties.Resources.PostPictureWarn3, serviceName, MakeAvailableServiceText(ext, fl.Length), ext),
                            Properties.Resources.PostPictureWarn4,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    return;
                }

                if (!imageService.CheckValidFilesize(ext, fl.Length))
                {
                    // ファイルサイズが大きすぎる
                    ClearSelectedImagePage();
                    if (!suppressMsgBox)
                    {
                        MessageBox.Show(
                            string.Format(Properties.Resources.PostPictureWarn5, serviceName, MakeAvailableServiceText(ext, fl.Length)),
                            Properties.Resources.PostPictureWarn4,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    return;
                }

                switch (imageService.GetFileType(ext))
                {
                    case MyCommon.UploadFileType.Picture:
                        using (var fs = File.OpenRead(fileName))
                        {
                            ImageSelectedPicture.Image = MemoryImage.CopyFromStream(fs);
                        }
                        SetSelectedImagePage(fileName, MyCommon.UploadFileType.Picture);
                        break;
                    case MyCommon.UploadFileType.MultiMedia:
                        SetSelectedImagePage(fileName, MyCommon.UploadFileType.MultiMedia);
                        break;
                    default:
                        ClearSelectedImagePage();
                        break;
                }
            }
            catch (FileNotFoundException)
            {
                ClearSelectedImagePage();
                if (!suppressMsgBox) MessageBox.Show("File not found.");
            }
            catch (Exception)
            {
                ClearSelectedImagePage();
                if (!suppressMsgBox) MessageBox.Show("The type of this file is not image.");
            }
        }

        private string MakeAvailableServiceText(string ext, long fileSize)
        {
            var text = string.Join(", ",
                ImageServiceCombo.Items.Cast<string>()
                    .Where(x => !string.IsNullOrEmpty(x) && this.pictureService[x].CheckValidFilesize(ext, fileSize)));

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
                                    if (!imageService.CheckValidFilesize(ext, fi.Length))
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
            if (this.ServiceName.Equals("Twitter") && selectedIndex < 3)
            {
                // 投稿先が Twitter であれば、最大 4 枚まで選択できるようにする
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
            var idx = ImagePageCombo.SelectedIndex;
            var item = (SelectedMedia)ImagePageCombo.Items[idx];
            item.Path = path;
            item.Type = type;

            AddNewImagePage(idx);
        }

        private void ClearSelectedImagePage()
        {
            var item = (SelectedMedia)ImagePageCombo.SelectedItem;
            item.Path = "";
            item.Type = MyCommon.UploadFileType.Invalid;
            ImagefilePathText.Text = "";
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
