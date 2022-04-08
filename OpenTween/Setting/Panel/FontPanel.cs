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
using System.Windows.Forms;

namespace OpenTween.Setting.Panel
{
    public partial class FontPanel : SettingPanelBase
    {
        private readonly ThemeManager defaultTheme = new(new());
        private ThemeManager currentTheme = new(new());

        public FontPanel()
            => this.InitializeComponent();

        public void LoadConfig(SettingLocal settingLocal)
        {
            this.UpdateTheme(settingLocal);

            this.lblListFont.Font = this.currentTheme.FontReaded;
            this.lblUnread.Font = this.currentTheme.FontUnread;
            this.lblUnread.ForeColor = this.currentTheme.ColorUnread;
            this.lblListFont.ForeColor = this.currentTheme.ColorRead;
            this.lblFav.ForeColor = this.currentTheme.ColorFav;
            this.lblOWL.ForeColor = this.currentTheme.ColorOWL;
            this.lblRetweet.ForeColor = this.currentTheme.ColorRetweet;
            this.lblDetail.Font = this.currentTheme.FontDetail;
            this.lblDetailBackcolor.BackColor = this.currentTheme.ColorDetailBackcolor;
            this.lblDetail.ForeColor = this.currentTheme.ColorDetail;
            this.lblDetailLink.ForeColor = this.currentTheme.ColorDetailLink;
            this.checkBoxUseTwemoji.Checked = settingLocal.UseTwemoji;
        }

        public void SaveConfig(SettingLocal settingLocal)
        {
            var fontConverter = new FontConverter();
            var colorConverter = new ColorConverter();

            settingLocal.FontUnreadStr = ThemeManager.ConvertFontToString(fontConverter, this.lblUnread.Font, this.defaultTheme.FontUnread); // 未使用
            settingLocal.ColorUnreadStr = ThemeManager.ConvertColorToString(colorConverter, this.lblUnread.ForeColor, this.defaultTheme.ColorUnread);
            settingLocal.FontReadStr = ThemeManager.ConvertFontToString(fontConverter, this.lblListFont.Font, this.defaultTheme.FontReaded); // リストフォントとして使用
            settingLocal.ColorReadStr = ThemeManager.ConvertColorToString(colorConverter, this.lblListFont.ForeColor, this.defaultTheme.ColorRead);
            settingLocal.ColorFavStr = ThemeManager.ConvertColorToString(colorConverter, this.lblFav.ForeColor, this.defaultTheme.ColorFav);
            settingLocal.ColorOWLStr = ThemeManager.ConvertColorToString(colorConverter, this.lblOWL.ForeColor, this.defaultTheme.ColorOWL);
            settingLocal.ColorRetweetStr = ThemeManager.ConvertColorToString(colorConverter, this.lblRetweet.ForeColor, this.defaultTheme.ColorRetweet);
            settingLocal.FontDetailStr = ThemeManager.ConvertFontToString(fontConverter, this.lblDetail.Font, this.defaultTheme.FontDetail);
            settingLocal.ColorDetailBackcolorStr = ThemeManager.ConvertColorToString(colorConverter, this.lblDetailBackcolor.BackColor, this.defaultTheme.ColorDetailBackcolor);
            settingLocal.ColorDetailStr = ThemeManager.ConvertColorToString(colorConverter, this.lblDetail.ForeColor, this.defaultTheme.ColorDetail);
            settingLocal.ColorDetailLinkStr = ThemeManager.ConvertColorToString(colorConverter, this.lblDetailLink.ForeColor, this.defaultTheme.ColorDetailLink);
            settingLocal.UseTwemoji = this.checkBoxUseTwemoji.Checked;
        }

        private void UpdateTheme(SettingLocal settingLocal)
        {
            var newTheme = new ThemeManager(settingLocal);
            (var oldTheme, this.currentTheme) = (this.currentTheme, newTheme);
            oldTheme.Dispose();
        }

        private void ButtonBackToDefaultFontColor_Click(object sender, EventArgs e)
        {
            this.lblUnread.ForeColor = this.defaultTheme.ColorUnread;
            this.lblUnread.Font = this.defaultTheme.FontUnread;
            this.lblListFont.ForeColor = this.defaultTheme.ColorRead;
            this.lblListFont.Font = this.defaultTheme.FontReaded;
            this.lblDetail.ForeColor = this.defaultTheme.ColorDetail;
            this.lblDetail.Font = this.defaultTheme.FontDetail;
            this.checkBoxUseTwemoji.Checked = true;
            this.lblFav.ForeColor = this.defaultTheme.ColorFav;
            this.lblOWL.ForeColor = this.defaultTheme.ColorOWL;
            this.lblDetailBackcolor.BackColor = this.defaultTheme.ColorDetailBackcolor;
            this.lblDetailLink.ForeColor = this.defaultTheme.ColorDetailLink;
            this.lblRetweet.ForeColor = this.defaultTheme.ColorRetweet;
        }

        private void BtnListFont_Click(object sender, EventArgs e)
            => this.ShowFontDialog(this.lblListFont);

        private void BtnUnread_Click(object sender, EventArgs e)
            => this.ShowFontDialog(this.lblUnread);

        private void BtnFav_Click(object sender, EventArgs e)
            => this.ShowForeColorDialog(this.lblFav);

        private void BtnOWL_Click(object sender, EventArgs e)
            => this.ShowForeColorDialog(this.lblOWL);

        private void BtnRetweet_Click(object sender, EventArgs e)
            => this.ShowForeColorDialog(this.lblRetweet);

        private void BtnDetail_Click(object sender, EventArgs e)
            => this.ShowFontDialog(this.lblDetail);

        private void BtnDetailLink_Click(object sender, EventArgs e)
            => this.ShowForeColorDialog(this.lblDetailLink);

        private void BtnDetailBack_Click(object sender, EventArgs e)
            => this.ShowBackColorDialog(this.lblDetailBackcolor);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.components?.Dispose();
                this.defaultTheme.Dispose();
                this.currentTheme.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
