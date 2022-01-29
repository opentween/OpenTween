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
using System.Linq;
using System.Text;

namespace OpenTween.Setting.Panel
{
    public partial class NotifyPanel : SettingPanelBase
    {
        public NotifyPanel()
            => this.InitializeComponent();

        public void LoadConfig(SettingCommon settingCommon)
        {
            this.CheckBoxNotificationPopup.Checked = settingCommon.NewAllPop;
            this.ComboBoxNameInPopup.SelectedIndex = settingCommon.NameBalloon switch
            {
                MyCommon.NameBalloonEnum.None => 0,
                MyCommon.NameBalloonEnum.UserID => 1,
                MyCommon.NameBalloonEnum.NickName => 2,
                _ => 2,
            };
            this.CheckBoxUseGrowlForNotification.Checked = settingCommon.IsUseNotifyGrowl;
            this.CheckBoxUseGrowlForNotification.Enabled = GrowlHelper.IsDllExists;
            this.CheckBoxEnableNotificationSound.Checked = settingCommon.PlaySound;
            this.CheckBoxEnableBlinkOnReply.Checked = settingCommon.BlinkNewMentions;
        }

        public void SaveConfig(SettingCommon settingCommon)
        {
            settingCommon.NewAllPop = this.CheckBoxNotificationPopup.Checked;
            settingCommon.NameBalloon = this.ComboBoxNameInPopup.SelectedIndex switch
            {
                0 => MyCommon.NameBalloonEnum.None,
                1 => MyCommon.NameBalloonEnum.UserID,
                2 => MyCommon.NameBalloonEnum.NickName,
                _ => throw new IndexOutOfRangeException(),
            };
            settingCommon.IsUseNotifyGrowl = this.CheckBoxUseGrowlForNotification.Checked;
            settingCommon.PlaySound = this.CheckBoxEnableNotificationSound.Checked;
            settingCommon.BlinkNewMentions = this.CheckBoxEnableBlinkOnReply.Checked;
        }
    }
}
