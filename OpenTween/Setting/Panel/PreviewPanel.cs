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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenTween.Setting.Panel
{
    public partial class PreviewPanel : SettingPanelBase
    {
        public PreviewPanel()
            => this.InitializeComponent();

        public void LoadConfig(SettingCommon settingCommon)
        {
            this.CheckDispUsername.Checked = settingCommon.DispUsername;
            this.ComboDispTitle.SelectedIndex = settingCommon.DispLatestPost switch
            {
                MyCommon.DispTitleEnum.None => 0,
                MyCommon.DispTitleEnum.Ver => 1,
                MyCommon.DispTitleEnum.Post => 2,
                MyCommon.DispTitleEnum.UnreadRepCount => 3,
                MyCommon.DispTitleEnum.UnreadAllCount => 4,
                MyCommon.DispTitleEnum.UnreadAllRepCount => 5,
                MyCommon.DispTitleEnum.UnreadCountAllCount => 6,
                MyCommon.DispTitleEnum.OwnStatus => 7,
                _ => 2,
            };
            this.CheckAlwaysTop.Checked = settingCommon.AlwaysTop;
            this.CheckBalloonLimit.Checked = settingCommon.LimitBalloon;
            this.chkTabIconDisp.Checked = settingCommon.TabIconDisp;
            this.CheckMonospace.Checked = settingCommon.IsMonospace;
            this.CheckPreviewEnable.Checked = settingCommon.PreviewEnable;
            this.CheckPreviewWindowEnable.Enabled = this.CheckPreviewEnable.Checked;
            this.CheckPreviewWindowEnable.Checked = settingCommon.PreviewWindowEnable;
            this.CheckStatusAreaAtBottom.Checked = settingCommon.StatusAreaAtBottom;
            this.ReplyIconStateCombo.SelectedIndex = settingCommon.ReplyIconState switch
            {
                MyCommon.REPLY_ICONSTATE.None => 0,
                MyCommon.REPLY_ICONSTATE.StaticIcon => 1,
                MyCommon.REPLY_ICONSTATE.BlinkIcon => 2,
                _ => 1,
            };
            this.LanguageCombo.SelectedIndex = settingCommon.Language switch
            {
                "OS" => 0,
                "ja" => 1,
                "en" => 2,
                _ => 0,
            };
        }

        public void SaveConfig(SettingCommon settingCommon)
        {
            settingCommon.DispUsername = this.CheckDispUsername.Checked;
            settingCommon.DispLatestPost = this.ComboDispTitle.SelectedIndex switch
            {
                0 => MyCommon.DispTitleEnum.None,
                1 => MyCommon.DispTitleEnum.Ver,
                2 => MyCommon.DispTitleEnum.Post,
                3 => MyCommon.DispTitleEnum.UnreadRepCount,
                4 => MyCommon.DispTitleEnum.UnreadAllCount,
                5 => MyCommon.DispTitleEnum.UnreadAllRepCount,
                6 => MyCommon.DispTitleEnum.UnreadCountAllCount,
                7 => MyCommon.DispTitleEnum.OwnStatus,
                _ => throw new IndexOutOfRangeException(),
            };
            settingCommon.AlwaysTop = this.CheckAlwaysTop.Checked;
            settingCommon.LimitBalloon = this.CheckBalloonLimit.Checked;
            settingCommon.TabIconDisp = this.chkTabIconDisp.Checked;
            settingCommon.IsMonospace = this.CheckMonospace.Checked;
            settingCommon.PreviewEnable = this.CheckPreviewEnable.Checked;
            settingCommon.PreviewWindowEnable = this.CheckPreviewWindowEnable.Checked;
            settingCommon.StatusAreaAtBottom = this.CheckStatusAreaAtBottom.Checked;
            settingCommon.ReplyIconState = this.ReplyIconStateCombo.SelectedIndex switch
            {
                0 => MyCommon.REPLY_ICONSTATE.None,
                1 => MyCommon.REPLY_ICONSTATE.StaticIcon,
                2 => MyCommon.REPLY_ICONSTATE.BlinkIcon,
                _ => throw new IndexOutOfRangeException(),
            };
            settingCommon.Language = this.LanguageCombo.SelectedIndex switch
            {
                0 => "OS",
                1 => "ja",
                2 => "en",
                _ => "en",
            };
        }

        private void CheckPreviewEnable_CheckedChanged(object sender, EventArgs e)
        {
            this.CheckPreviewWindowEnable.Enabled = this.CheckPreviewEnable.Checked;
        }
    }
}
