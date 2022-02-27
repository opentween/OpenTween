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

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Api;

namespace OpenTween.Setting.Panel
{
    public partial class ShortUrlPanel : SettingPanelBase
    {
        public ShortUrlPanel()
            => this.InitializeComponent();

        public void LoadConfig(SettingCommon settingCommon)
        {
            this.CheckTinyURL.Checked = settingCommon.TinyUrlResolve;

            // 使われていない設定項目 (Tween v1.0.5.0)
            this.CheckAutoConvertUrl.Checked = false;
            this.ShortenTcoCheck.Enabled = this.CheckAutoConvertUrl.Checked;

            this.ComboBoxAutoShortUrlFirst.SelectedIndex = (int)settingCommon.AutoShortUrlFirst;
            this.TextBitlyAccessToken.Text = settingCommon.BitlyAccessToken;
            this.TextBitlyAccessToken.Modified = false;
        }

        public void SaveConfig(SettingCommon settingCommon)
        {
            settingCommon.TinyUrlResolve = this.CheckTinyURL.Checked;
            settingCommon.UrlConvertAuto = this.CheckAutoConvertUrl.Checked;
            settingCommon.AutoShortUrlFirst = (MyCommon.UrlConverter)this.ComboBoxAutoShortUrlFirst.SelectedIndex;
            settingCommon.BitlyAccessToken = this.TextBitlyAccessToken.Text;
        }

        private void ComboBoxAutoShortUrlFirst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ComboBoxAutoShortUrlFirst.SelectedIndex == (int)MyCommon.UrlConverter.Bitly ||
               this.ComboBoxAutoShortUrlFirst.SelectedIndex == (int)MyCommon.UrlConverter.Jmp)
            {
                this.Label77.Enabled = true;
                this.TextBitlyAccessToken.Enabled = true;
                this.ButtonBitlyAuthorize.Enabled = true;
            }
            else
            {
                this.Label77.Enabled = false;
                this.TextBitlyAccessToken.Enabled = false;
                this.ButtonBitlyAuthorize.Enabled = false;
            }
        }

        private void CheckAutoConvertUrl_CheckedChanged(object sender, EventArgs e)
            => this.ShortenTcoCheck.Enabled = this.CheckAutoConvertUrl.Checked;

        private void ButtonBitlyAuthorize_Click(object sender, EventArgs e)
        {
            using var dialog = new LoginDialog();

            const string DialogText = "Bitly Login";
            dialog.Text = DialogText;

            string? accessToken = null;
            dialog.LoginCallback = async () =>
            {
                try
                {
                    var bitly = new BitlyApi();
                    accessToken = await bitly.GetAccessTokenAsync(dialog.LoginName, dialog.Password);
                    return true;
                }
                catch (WebApiException ex)
                {
                    var text = string.Format(Properties.Resources.BitlyAuthorize_ErrorText, ex.Message);
                    MessageBox.Show(dialog, text, DialogText, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            };

            if (dialog.ShowDialog(this.ParentForm) == DialogResult.OK)
            {
                this.TextBitlyAccessToken.Text = accessToken;
            }
        }
    }
}
