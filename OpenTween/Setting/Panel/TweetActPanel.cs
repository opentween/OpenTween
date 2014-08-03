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
    public partial class TweetActPanel : SettingPanelBase
    {
        public TweetActPanel()
        {
            InitializeComponent();
        }

        public void LoadConfig(SettingCommon settingCommon, SettingLocal settingLocal)
        {
            this.StatusText.Text = settingLocal.StatusText;

            if (settingCommon.PostCtrlEnter)
            {
                this.ComboBoxPostKeySelect.SelectedIndex = 1;
            }
            else if (settingCommon.PostShiftEnter)
            {
                this.ComboBoxPostKeySelect.SelectedIndex = 2;
            }
            else
            {
                this.ComboBoxPostKeySelect.SelectedIndex = 0;
            }

            this.CheckUseRecommendStatus.Checked = settingLocal.UseRecommendStatus;
            this.CheckRetweetNoConfirm.Checked = settingCommon.RetweetNoConfirm;
            this.CheckAtIdSupple.Checked = settingCommon.UseAtIdSupplement;
            this.CheckHashSupple.Checked = settingCommon.UseHashSupplement;
        }

        public void SaveConfig(SettingCommon settingCommon, SettingLocal settingLocal)
        {
            settingLocal.StatusText = this.StatusText.Text;

            switch (this.ComboBoxPostKeySelect.SelectedIndex)
            {
                case 2:
                    settingCommon.PostShiftEnter = true;
                    settingCommon.PostCtrlEnter = false;
                    break;
                case 1:
                    settingCommon.PostCtrlEnter = true;
                    settingCommon.PostShiftEnter = false;
                    break;
                case 0:
                    settingCommon.PostCtrlEnter = false;
                    settingCommon.PostShiftEnter = false;
                    break;
            }

            settingLocal.UseRecommendStatus = this.CheckUseRecommendStatus.Checked;
            settingCommon.RetweetNoConfirm = this.CheckRetweetNoConfirm.Checked;
            settingCommon.UseAtIdSupplement = this.CheckAtIdSupple.Checked;
            settingCommon.UseHashSupplement = this.CheckHashSupple.Checked;
        }

        private void CheckUseRecommendStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckUseRecommendStatus.Checked == true)
            {
                StatusText.Enabled = false;
            }
            else
            {
                StatusText.Enabled = true;
            }
        }
    }
}
