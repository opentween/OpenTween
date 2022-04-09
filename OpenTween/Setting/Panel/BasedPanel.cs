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
    public partial class BasedPanel : SettingPanelBase
    {
        public BasedPanel()
            => this.InitializeComponent();

        public void LoadConfig(SettingCommon settingCommon)
        {
            using (ControlTransaction.Update(this.AuthUserCombo))
            {
                this.AuthUserCombo.Items.Clear();
                this.AuthUserCombo.Items.AddRange(settingCommon.UserAccounts.ToArray());

                var selectedUserId = settingCommon.UserId;
                var selectedAccount = settingCommon.UserAccounts.FirstOrDefault(x => x.UserId == selectedUserId);
                if (selectedAccount != null)
                    this.AuthUserCombo.SelectedItem = selectedAccount;
            }
        }

        public void SaveConfig(SettingCommon settingCommon)
        {
            var accounts = this.AuthUserCombo.Items.Cast<UserAccount>().ToList();
            settingCommon.UserAccounts = accounts;

            var selectedIndex = this.AuthUserCombo.SelectedIndex;
            if (selectedIndex != -1)
            {
                var selectedAccount = accounts[selectedIndex];
                settingCommon.UserId = selectedAccount.UserId;
                settingCommon.UserName = selectedAccount.Username;
                settingCommon.Token = selectedAccount.Token;
                settingCommon.TokenSecret = selectedAccount.TokenSecret;
            }
            else
            {
                settingCommon.UserId = 0;
                settingCommon.UserName = "";
                settingCommon.Token = "";
                settingCommon.TokenSecret = "";
            }
        }

        private void AuthClearButton_Click(object sender, EventArgs e)
        {
            if (this.AuthUserCombo.SelectedIndex > -1)
            {
                this.AuthUserCombo.Items.RemoveAt(this.AuthUserCombo.SelectedIndex);
                if (this.AuthUserCombo.Items.Count > 0)
                {
                    this.AuthUserCombo.SelectedIndex = 0;
                }
                else
                {
                    this.AuthUserCombo.SelectedIndex = -1;
                }
            }
        }
    }
}
