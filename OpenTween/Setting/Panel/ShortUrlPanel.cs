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

namespace OpenTween.Setting.Panel
{
    public partial class ShortUrlPanel : SettingPanelBase
    {
        public ShortUrlPanel()
        {
            InitializeComponent();
        }

        public void LoadConfig(SettingCommon settingCommon)
        {
            this.CheckTinyURL.Checked = settingCommon.TinyUrlResolve;

            // 使われていない設定項目 (Tween v1.0.5.0)
            this.CheckAutoConvertUrl.Checked = false;
            //this.CheckAutoConvertUrl.Checked = settingCommon.UrlConvertAuto;
            //this.ShortenTcoCheck.Checked = settingCommon.ShortenTco;
            this.ShortenTcoCheck.Enabled = this.CheckAutoConvertUrl.Checked;

            this.ComboBoxAutoShortUrlFirst.SelectedIndex = (int)settingCommon.AutoShortUrlFirst;
            this.TextBitlyId.Text = settingCommon.BilyUser;
            this.TextBitlyPw.Text = settingCommon.BitlyPwd;
            this.TextBitlyId.Modified = false;
            this.TextBitlyPw.Modified = false;
        }

        public void SaveConfig(SettingCommon settingCommon)
        {
            settingCommon.TinyUrlResolve = this.CheckTinyURL.Checked;
            settingCommon.UrlConvertAuto = this.CheckAutoConvertUrl.Checked;
            //settingCommon.ShortenTco = this.ShortenTcoCheck.Checked;
            settingCommon.AutoShortUrlFirst = (MyCommon.UrlConverter)this.ComboBoxAutoShortUrlFirst.SelectedIndex;
            settingCommon.BilyUser = this.TextBitlyId.Text;
            settingCommon.BitlyPwd = this.TextBitlyPw.Text;
        }

        private void ComboBoxAutoShortUrlFirst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxAutoShortUrlFirst.SelectedIndex == (int)MyCommon.UrlConverter.Bitly ||
               ComboBoxAutoShortUrlFirst.SelectedIndex == (int)MyCommon.UrlConverter.Jmp)
            {
                Label76.Enabled = true;
                Label77.Enabled = true;
                TextBitlyId.Enabled = true;
                TextBitlyPw.Enabled = true;
            }
            else
            {
                Label76.Enabled = false;
                Label77.Enabled = false;
                TextBitlyId.Enabled = false;
                TextBitlyPw.Enabled = false;
            }
        }

        private void CheckAutoConvertUrl_CheckedChanged(object sender, EventArgs e)
        {
            ShortenTcoCheck.Enabled = CheckAutoConvertUrl.Checked;
        }
    }
}
