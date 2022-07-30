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
    public partial class ActionPanel : SettingPanelBase
    {
        public ActionPanel()
            => this.InitializeComponent();

        public void LoadConfig(SettingCommon settingCommon, SettingLocal settingLocal)
        {
            this.UReadMng.Checked = settingCommon.UnreadManage;
            this.BrowserPathText.Text = settingLocal.BrowserPath;
            this.CheckCloseToExit.Checked = settingCommon.CloseToExit;
            this.CheckMinimizeToTray.Checked = settingCommon.MinimizeToTray;
            this.CheckEnableTwitterV2Api.Checked = settingCommon.EnableTwitterV2Api;
            this.CheckFavRestrict.Checked = settingCommon.RestrictFavCheck;
            this.chkReadOwnPost.Checked = settingCommon.ReadOwnPost;
            this.CheckReadOldPosts.Checked = settingCommon.ReadOldPosts;

            this.HotkeyCheck.Checked = settingCommon.HotkeyEnabled;
            this.HotkeyAlt.Checked = (settingCommon.HotkeyModifier & Keys.Alt) == Keys.Alt;
            this.HotkeyCtrl.Checked = (settingCommon.HotkeyModifier & Keys.Control) == Keys.Control;
            this.HotkeyShift.Checked = (settingCommon.HotkeyModifier & Keys.Shift) == Keys.Shift;
            this.HotkeyWin.Checked = (settingCommon.HotkeyModifier & Keys.LWin) == Keys.LWin;
            this.HotkeyCode.Text = settingCommon.HotkeyValue.ToString();
            this.HotkeyText.Text = settingCommon.HotkeyKey.ToString();
            this.HotkeyText.Tag = settingCommon.HotkeyKey;
            this.HotkeyAlt.Enabled = settingCommon.HotkeyEnabled;
            this.HotkeyShift.Enabled = settingCommon.HotkeyEnabled;
            this.HotkeyCtrl.Enabled = settingCommon.HotkeyEnabled;
            this.HotkeyWin.Enabled = settingCommon.HotkeyEnabled;
            this.HotkeyText.Enabled = settingCommon.HotkeyEnabled;
            this.HotkeyCode.Enabled = settingCommon.HotkeyEnabled;

            this.CheckOpenUserTimeline.Checked = settingCommon.OpenUserTimeline;
            this.ListDoubleClickActionComboBox.SelectedIndex = settingCommon.ListDoubleClickAction switch
            {
                MyCommon.ListItemDoubleClickActionType.None => 0,
                MyCommon.ListItemDoubleClickActionType.Reply => 1,
                MyCommon.ListItemDoubleClickActionType.ReplyAll => 2,
                MyCommon.ListItemDoubleClickActionType.Favorite => 3,
                MyCommon.ListItemDoubleClickActionType.ShowProfile => 4,
                MyCommon.ListItemDoubleClickActionType.ShowTimeline => 5,
                MyCommon.ListItemDoubleClickActionType.ShowRelated => 6,
                MyCommon.ListItemDoubleClickActionType.OpenHomeInBrowser => 7,
                MyCommon.ListItemDoubleClickActionType.OpenStatusInBrowser => 8,
                _ => 1,
            };
            this.TabMouseLockCheck.Checked = settingCommon.TabMouseLock;
        }

        public void SaveConfig(SettingCommon settingCommon, SettingLocal settingLocal)
        {
            settingCommon.UnreadManage = this.UReadMng.Checked;
            settingLocal.BrowserPath = this.BrowserPathText.Text.Trim();
            settingCommon.CloseToExit = this.CheckCloseToExit.Checked;
            settingCommon.MinimizeToTray = this.CheckMinimizeToTray.Checked;
            settingCommon.EnableTwitterV2Api = this.CheckEnableTwitterV2Api.Checked;
            settingCommon.RestrictFavCheck = this.CheckFavRestrict.Checked;
            settingCommon.ReadOwnPost = this.chkReadOwnPost.Checked;
            settingCommon.ReadOldPosts = this.CheckReadOldPosts.Checked;

            settingCommon.HotkeyEnabled = this.HotkeyCheck.Checked;
            settingCommon.HotkeyModifier = Keys.None;
            if (this.HotkeyAlt.Checked)
                settingCommon.HotkeyModifier |= Keys.Alt;
            if (this.HotkeyShift.Checked)
                settingCommon.HotkeyModifier |= Keys.Shift;
            if (this.HotkeyCtrl.Checked)
                settingCommon.HotkeyModifier |= Keys.Control;
            if (this.HotkeyWin.Checked)
                settingCommon.HotkeyModifier |= Keys.LWin;
            int.TryParse(this.HotkeyCode.Text, out settingCommon.HotkeyValue);
            settingCommon.HotkeyKey = (Keys)this.HotkeyText.Tag;

            settingCommon.OpenUserTimeline = this.CheckOpenUserTimeline.Checked;
            settingCommon.ListDoubleClickAction = this.ListDoubleClickActionComboBox.SelectedIndex switch
            {
                0 => MyCommon.ListItemDoubleClickActionType.None,
                1 => MyCommon.ListItemDoubleClickActionType.Reply,
                2 => MyCommon.ListItemDoubleClickActionType.ReplyAll,
                3 => MyCommon.ListItemDoubleClickActionType.Favorite,
                4 => MyCommon.ListItemDoubleClickActionType.ShowProfile,
                5 => MyCommon.ListItemDoubleClickActionType.ShowTimeline,
                6 => MyCommon.ListItemDoubleClickActionType.ShowRelated,
                7 => MyCommon.ListItemDoubleClickActionType.OpenHomeInBrowser,
                8 => MyCommon.ListItemDoubleClickActionType.OpenStatusInBrowser,
                _ => MyCommon.ListItemDoubleClickActionType.Reply,
            };
            settingCommon.TabMouseLock = this.TabMouseLockCheck.Checked;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            using var filedlg = new OpenFileDialog();

            filedlg.Filter = Properties.Resources.Button3_ClickText1;
            filedlg.FilterIndex = 1;
            filedlg.Title = Properties.Resources.Button3_ClickText2;
            filedlg.RestoreDirectory = true;

            if (filedlg.ShowDialog() == DialogResult.OK)
            {
                this.BrowserPathText.Text = filedlg.FileName;
            }
        }

        private void HotkeyText_KeyDown(object sender, KeyEventArgs e)
        {
            // KeyValueで判定する。
            // 表示文字とのテーブルを用意すること
            this.HotkeyText.Text = e.KeyCode.ToString();
            this.HotkeyCode.Text = e.KeyValue.ToString();
            this.HotkeyText.Tag = e.KeyCode;
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void HotkeyCheck_CheckedChanged(object sender, EventArgs e)
        {
            this.HotkeyCtrl.Enabled = this.HotkeyCheck.Checked;
            this.HotkeyAlt.Enabled = this.HotkeyCheck.Checked;
            this.HotkeyShift.Enabled = this.HotkeyCheck.Checked;
            this.HotkeyWin.Enabled = this.HotkeyCheck.Checked;
            this.HotkeyText.Enabled = this.HotkeyCheck.Checked;
            this.HotkeyCode.Enabled = this.HotkeyCheck.Checked;
        }
    }
}
