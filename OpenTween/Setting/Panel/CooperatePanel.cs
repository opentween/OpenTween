// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2014      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTween.Thumbnail;

namespace OpenTween.Setting.Panel
{
    public partial class CooperatePanel : SettingPanelBase
    {
        public CooperatePanel()
        {
            InitializeComponent();
        }

        public void LoadConfig(SettingCommon settingCommon)
        {
            this.CheckNicoms.Checked = settingCommon.Nicoms;
            this.ComboBoxTranslateLanguage.SelectedIndex = Bing.GetIndexFromLanguageEnum(settingCommon.TranslateLanguage);
            this.UserAppointUrlText.Text = settingCommon.UserAppointUrl;
            this.EnableImgAzyobuziNetCheckBox.Checked = settingCommon.EnableImgAzyobuziNet;
            this.ImgAzyobuziNetDisabledInDMCheckBox.Checked = settingCommon.ImgAzyobuziNetDisabledInDM;
            this.MapThumbnailProviderComboBox.SelectedIndex = (int)settingCommon.MapThumbnailProvider;
            this.MapThumbnailHeightTextBox.Text = settingCommon.MapThumbnailHeight.ToString();
            this.MapThumbnailWidthTextBox.Text = settingCommon.MapThumbnailWidth.ToString();
            this.MapThumbnailZoomTextBox.Text = settingCommon.MapThumbnailZoom.ToString();
        }

        public void SaveConfig(SettingCommon settingCommon)
        {
            settingCommon.Nicoms = this.CheckNicoms.Checked;
            settingCommon.TranslateLanguage = Bing.GetLanguageEnumFromIndex(this.ComboBoxTranslateLanguage.SelectedIndex);
            settingCommon.UserAppointUrl = this.UserAppointUrlText.Text;
            settingCommon.EnableImgAzyobuziNet = this.EnableImgAzyobuziNetCheckBox.Checked;
            settingCommon.ImgAzyobuziNetDisabledInDM = this.ImgAzyobuziNetDisabledInDMCheckBox.Checked;
            settingCommon.MapThumbnailProvider = (MapProvider)this.MapThumbnailProviderComboBox.SelectedIndex;
            settingCommon.MapThumbnailHeight = int.Parse(this.MapThumbnailHeightTextBox.Text);
            settingCommon.MapThumbnailWidth = int.Parse(this.MapThumbnailWidthTextBox.Text);
            settingCommon.MapThumbnailZoom = int.Parse(this.MapThumbnailZoomTextBox.Text);
        }

        private void UserAppointUrlText_Validating(object sender, CancelEventArgs e)
        {
            if (!UserAppointUrlText.Text.StartsWith("http") && !string.IsNullOrEmpty(UserAppointUrlText.Text))
            {
                MessageBox.Show("Text Error:正しいURLではありません");
            }
        }

        private void EnableImgAzyobuziNetCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            this.ImgAzyobuziNetDisabledInDMCheckBox.Enabled = this.EnableImgAzyobuziNetCheckBox.Checked;
        }
    }
}
