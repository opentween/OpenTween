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
    public partial class FontPanel2 : SettingPanelBase
    {
        public FontPanel2()
        {
            InitializeComponent();
        }

        public void LoadConfig(SettingLocal settingLocal)
        {
            this.lblSelf.BackColor = settingLocal.ColorSelf;
            this.lblAtSelf.BackColor = settingLocal.ColorAtSelf;
            this.lblTarget.BackColor = settingLocal.ColorTarget;
            this.lblAtTarget.BackColor = settingLocal.ColorAtTarget;
            this.lblAtFromTarget.BackColor = settingLocal.ColorAtFromTarget;
            this.lblAtTo.BackColor = settingLocal.ColorAtTo;
            this.lblInputBackcolor.BackColor = settingLocal.ColorInputBackcolor;
            this.lblInputFont.ForeColor = settingLocal.ColorInputFont;
            this.lblInputFont.Font = settingLocal.FontInputFont;
            this.lblListBackcolor.BackColor = settingLocal.ColorListBackcolor;
        }

        public void SaveConfig(SettingLocal settingLocal)
        {
            settingLocal.ColorSelf = this.lblSelf.BackColor;
            settingLocal.ColorAtSelf = this.lblAtSelf.BackColor;
            settingLocal.ColorTarget = this.lblTarget.BackColor;
            settingLocal.ColorAtTarget = this.lblAtTarget.BackColor;
            settingLocal.ColorAtFromTarget = this.lblAtFromTarget.BackColor;
            settingLocal.ColorAtTo = this.lblAtTo.BackColor;
            settingLocal.ColorInputBackcolor = this.lblInputBackcolor.BackColor;
            settingLocal.ColorInputFont = this.lblInputFont.ForeColor;
            settingLocal.ColorListBackcolor = this.lblListBackcolor.BackColor;
            settingLocal.FontInputFont = this.lblInputFont.Font;
        }

        private void ButtonBackToDefaultFontColor2_Click(object sender, EventArgs e)
        {
            lblInputFont.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.ControlText);
            lblInputFont.Font = System.Drawing.SystemFonts.DefaultFont;

            lblSelf.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.AliceBlue);

            lblAtSelf.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.AntiqueWhite);

            lblTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LemonChiffon);

            lblAtTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LavenderBlush);

            lblAtFromTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Honeydew);

            lblInputBackcolor.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LemonChiffon);

            lblAtTo.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Pink);

            lblListBackcolor.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Window);
        }

        private void btnSelf_Click(object sender, EventArgs e)
        {
            this.ShowBackColorDialog(this.lblSelf);
        }

        private void btnAtSelf_Click(object sender, EventArgs e)
        {
            this.ShowBackColorDialog(this.lblAtSelf);
        }

        private void btnTarget_Click(object sender, EventArgs e)
        {
            this.ShowBackColorDialog(this.lblTarget);
        }

        private void btnAtTarget_Click(object sender, EventArgs e)
        {
            this.ShowBackColorDialog(this.lblAtTarget);
        }

        private void btnAtFromTarget_Click(object sender, EventArgs e)
        {
            this.ShowBackColorDialog(this.lblAtFromTarget);
        }

        private void btnAtTo_Click(object sender, EventArgs e)
        {
            this.ShowBackColorDialog(this.lblAtTo);
        }

        private void btnListBack_Click(object sender, EventArgs e)
        {
            this.ShowBackColorDialog(this.lblListBackcolor);
        }

        private void btnInputBackcolor_Click(object sender, EventArgs e)
        {
            this.ShowBackColorDialog(this.lblInputBackcolor);
        }

        private void btnInputFont_Click(object sender, EventArgs e)
        {
            this.ShowFontDialog(this.lblInputFont);
        }
    }
}
