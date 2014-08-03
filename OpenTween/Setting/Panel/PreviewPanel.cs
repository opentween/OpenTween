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
    public partial class PreviewPanel : SettingPanelBase
    {
        public PreviewPanel()
        {
            InitializeComponent();
        }

        public void LoadConfig(SettingCommon settingCommon)
        {
            switch (settingCommon.NameBalloon)
            {
                case MyCommon.NameBalloonEnum.None:
                    this.cmbNameBalloon.SelectedIndex = 0;
                    break;
                case MyCommon.NameBalloonEnum.UserID:
                    this.cmbNameBalloon.SelectedIndex = 1;
                    break;
                case MyCommon.NameBalloonEnum.NickName:
                    this.cmbNameBalloon.SelectedIndex = 2;
                    break;
            }

            this.CheckDispUsername.Checked = settingCommon.DispUsername;

            switch (settingCommon.DispLatestPost)
            {
                case MyCommon.DispTitleEnum.None:
                    this.ComboDispTitle.SelectedIndex = 0;
                    break;
                case MyCommon.DispTitleEnum.Ver:
                    this.ComboDispTitle.SelectedIndex = 1;
                    break;
                case MyCommon.DispTitleEnum.Post:
                    this.ComboDispTitle.SelectedIndex = 2;
                    break;
                case MyCommon.DispTitleEnum.UnreadRepCount:
                    this.ComboDispTitle.SelectedIndex = 3;
                    break;
                case MyCommon.DispTitleEnum.UnreadAllCount:
                    this.ComboDispTitle.SelectedIndex = 4;
                    break;
                case MyCommon.DispTitleEnum.UnreadAllRepCount:
                    this.ComboDispTitle.SelectedIndex = 5;
                    break;
                case MyCommon.DispTitleEnum.UnreadCountAllCount:
                    this.ComboDispTitle.SelectedIndex = 6;
                    break;
                case MyCommon.DispTitleEnum.OwnStatus:
                    this.ComboDispTitle.SelectedIndex = 7;
                    break;
            }

            this.CheckAlwaysTop.Checked = settingCommon.AlwaysTop;
            this.CheckBalloonLimit.Checked = settingCommon.LimitBalloon;
            this.chkTabIconDisp.Checked = settingCommon.TabIconDisp;
            this.CheckMonospace.Checked = settingCommon.IsMonospace;
            this.CheckPreviewEnable.Checked = settingCommon.PreviewEnable;
            this.CheckStatusAreaAtBottom.Checked = settingCommon.StatusAreaAtBottom;

            switch (settingCommon.ReplyIconState)
            {
                case MyCommon.REPLY_ICONSTATE.None:
                    this.ReplyIconStateCombo.SelectedIndex = 0;
                    break;
                case MyCommon.REPLY_ICONSTATE.StaticIcon:
                    this.ReplyIconStateCombo.SelectedIndex = 1;
                    break;
                case MyCommon.REPLY_ICONSTATE.BlinkIcon:
                    this.ReplyIconStateCombo.SelectedIndex = 2;
                    break;
            }

            switch (settingCommon.Language)
            {
                case "OS":
                    this.LanguageCombo.SelectedIndex = 0;
                    break;
                case "ja":
                    this.LanguageCombo.SelectedIndex = 1;
                    break;
                case "en":
                    this.LanguageCombo.SelectedIndex = 2;
                    break;
                case "zh-CN":
                    this.LanguageCombo.SelectedIndex = 3;
                    break;
                default:
                    this.LanguageCombo.SelectedIndex = 0;
                    break;
            }

            this.ChkNewMentionsBlink.Checked = settingCommon.BlinkNewMentions;
            this.IsNotifyUseGrowlCheckBox.Checked = settingCommon.IsUseNotifyGrowl;
            this.IsNotifyUseGrowlCheckBox.Enabled = GrowlHelper.IsDllExists;
        }

        public void SaveConfig(SettingCommon settingCommon)
        {
            switch (this.cmbNameBalloon.SelectedIndex)
            {
                case 0:
                    settingCommon.NameBalloon = MyCommon.NameBalloonEnum.None;
                    break;
                case 1:
                    settingCommon.NameBalloon = MyCommon.NameBalloonEnum.UserID;
                    break;
                case 2:
                    settingCommon.NameBalloon = MyCommon.NameBalloonEnum.NickName;
                    break;
            }

            settingCommon.DispUsername = this.CheckDispUsername.Checked;

            switch (this.ComboDispTitle.SelectedIndex)
            {
                case 0: // None
                    settingCommon.DispLatestPost = MyCommon.DispTitleEnum.None;
                    break;
                case 1: // Ver
                    settingCommon.DispLatestPost = MyCommon.DispTitleEnum.Ver;
                    break;
                case 2: // Post
                    settingCommon.DispLatestPost = MyCommon.DispTitleEnum.Post;
                    break;
                case 3: // RepCount
                    settingCommon.DispLatestPost = MyCommon.DispTitleEnum.UnreadRepCount;
                    break;
                case 4: // AllCount
                    settingCommon.DispLatestPost = MyCommon.DispTitleEnum.UnreadAllCount;
                    break;
                case 5: // Rep+All
                    settingCommon.DispLatestPost = MyCommon.DispTitleEnum.UnreadAllRepCount;
                    break;
                case 6: // Unread/All
                    settingCommon.DispLatestPost = MyCommon.DispTitleEnum.UnreadCountAllCount;
                    break;
                case 7: // Count of Status/Follow/Follower
                    settingCommon.DispLatestPost = MyCommon.DispTitleEnum.OwnStatus;
                    break;
            }

            settingCommon.AlwaysTop = this.CheckAlwaysTop.Checked;
            settingCommon.LimitBalloon = this.CheckBalloonLimit.Checked;
            settingCommon.TabIconDisp = this.chkTabIconDisp.Checked;
            settingCommon.IsMonospace = this.CheckMonospace.Checked;
            settingCommon.PreviewEnable = this.CheckPreviewEnable.Checked;
            settingCommon.StatusAreaAtBottom = this.CheckStatusAreaAtBottom.Checked;

            switch (this.ReplyIconStateCombo.SelectedIndex)
            {
                case 0:
                    settingCommon.ReplyIconState = MyCommon.REPLY_ICONSTATE.None;
                    break;
                case 1:
                    settingCommon.ReplyIconState = MyCommon.REPLY_ICONSTATE.StaticIcon;
                    break;
                case 2:
                    settingCommon.ReplyIconState = MyCommon.REPLY_ICONSTATE.BlinkIcon;
                    break;
            }

            switch (this.LanguageCombo.SelectedIndex)
            {
                case 0:
                    settingCommon.Language = "OS";
                    break;
                case 1:
                    settingCommon.Language = "ja";
                    break;
                case 2:
                    settingCommon.Language = "en";
                    break;
                case 3:
                    settingCommon.Language = "zh-CN";
                    break;
                default:
                    settingCommon.Language = "en";
                    break;
            }

            settingCommon.BlinkNewMentions = this.ChkNewMentionsBlink.Checked;
            settingCommon.IsUseNotifyGrowl = this.IsNotifyUseGrowlCheckBox.Checked;
        }
    }
}
