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
    public partial class FontPanel2 : SettingPanelBase
    {
        private readonly ThemeManager defaultTheme = new(new());
        private ThemeManager currentTheme = new(new());

        public FontPanel2()
            => this.InitializeComponent();

        public void LoadConfig(SettingLocal settingLocal)
        {
            this.UpdateTheme(settingLocal);

            this.lblSelf.BackColor = this.currentTheme.ColorSelf;
            this.lblAtSelf.BackColor = this.currentTheme.ColorAtSelf;
            this.lblTarget.BackColor = this.currentTheme.ColorTarget;
            this.lblAtTarget.BackColor = this.currentTheme.ColorAtTarget;
            this.lblAtFromTarget.BackColor = this.currentTheme.ColorAtFromTarget;
            this.lblAtTo.BackColor = this.currentTheme.ColorAtTo;
            this.lblInputBackcolor.BackColor = this.currentTheme.ColorInputBackcolor;
            this.lblInputFont.ForeColor = this.currentTheme.ColorInputFont;
            this.lblInputFont.Font = this.currentTheme.FontInputFont;
            this.lblListBackcolor.BackColor = this.currentTheme.ColorListBackcolor;
        }

        public void SaveConfig(SettingLocal settingLocal)
        {
            var fontConverter = new FontConverter();
            var colorConverter = new ColorConverter();

            settingLocal.ColorSelfStr = ThemeManager.ConvertColorToString(colorConverter, this.lblSelf.BackColor, this.defaultTheme.ColorSelf);
            settingLocal.ColorAtSelfStr = ThemeManager.ConvertColorToString(colorConverter, this.lblAtSelf.BackColor, this.defaultTheme.ColorAtSelf);
            settingLocal.ColorTargetStr = ThemeManager.ConvertColorToString(colorConverter, this.lblTarget.BackColor, this.defaultTheme.ColorTarget);
            settingLocal.ColorAtTargetStr = ThemeManager.ConvertColorToString(colorConverter, this.lblAtTarget.BackColor, this.defaultTheme.ColorAtTarget);
            settingLocal.ColorAtFromTargetStr = ThemeManager.ConvertColorToString(colorConverter, this.lblAtFromTarget.BackColor, this.defaultTheme.ColorAtFromTarget);
            settingLocal.ColorAtToStr = ThemeManager.ConvertColorToString(colorConverter, this.lblAtTo.BackColor, this.defaultTheme.ColorAtTo);
            settingLocal.ColorInputBackcolorStr = ThemeManager.ConvertColorToString(colorConverter, this.lblInputBackcolor.BackColor, this.defaultTheme.ColorInputBackcolor);
            settingLocal.ColorInputFontStr = ThemeManager.ConvertColorToString(colorConverter, this.lblInputFont.ForeColor, this.defaultTheme.ColorInputFont);
            settingLocal.ColorListBackcolorStr = ThemeManager.ConvertColorToString(colorConverter, this.lblListBackcolor.BackColor, this.defaultTheme.ColorListBackcolor);
            settingLocal.FontInputFontStr = ThemeManager.ConvertFontToString(fontConverter, this.lblInputFont.Font, this.defaultTheme.FontInputFont);
        }

        private void UpdateTheme(SettingLocal settingLocal)
        {
            var newTheme = new ThemeManager(settingLocal);
            (var oldTheme, this.currentTheme) = (this.currentTheme, newTheme);
            oldTheme.Dispose();
        }

        private void ButtonBackToDefaultFontColor2_Click(object sender, EventArgs e)
        {
            this.lblInputFont.ForeColor = this.defaultTheme.ColorInputFont;
            this.lblInputFont.Font = this.defaultTheme.FontInputFont;
            this.lblSelf.BackColor = this.defaultTheme.ColorSelf;
            this.lblAtSelf.BackColor = this.defaultTheme.ColorAtSelf;
            this.lblTarget.BackColor = this.defaultTheme.ColorTarget;
            this.lblAtTarget.BackColor = this.defaultTheme.ColorAtTarget;
            this.lblAtFromTarget.BackColor = this.defaultTheme.ColorAtFromTarget;
            this.lblInputBackcolor.BackColor = this.defaultTheme.ColorInputBackcolor;
            this.lblAtTo.BackColor = this.defaultTheme.ColorAtTo;
            this.lblListBackcolor.BackColor = this.defaultTheme.ColorListBackcolor;
        }

        private void BtnSelf_Click(object sender, EventArgs e)
            => this.ShowBackColorDialog(this.lblSelf);

        private void BtnAtSelf_Click(object sender, EventArgs e)
            => this.ShowBackColorDialog(this.lblAtSelf);

        private void BtnTarget_Click(object sender, EventArgs e)
            => this.ShowBackColorDialog(this.lblTarget);

        private void BtnAtTarget_Click(object sender, EventArgs e)
            => this.ShowBackColorDialog(this.lblAtTarget);

        private void BtnAtFromTarget_Click(object sender, EventArgs e)
            => this.ShowBackColorDialog(this.lblAtFromTarget);

        private void BtnAtTo_Click(object sender, EventArgs e)
            => this.ShowBackColorDialog(this.lblAtTo);

        private void BtnListBack_Click(object sender, EventArgs e)
            => this.ShowBackColorDialog(this.lblListBackcolor);

        private void BtnInputBackcolor_Click(object sender, EventArgs e)
            => this.ShowBackColorDialog(this.lblInputBackcolor);

        private void BtnInputFont_Click(object sender, EventArgs e)
            => this.ShowFontDialog(this.lblInputFont);

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
