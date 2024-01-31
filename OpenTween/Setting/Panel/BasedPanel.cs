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
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace OpenTween.Setting.Panel
{
    public partial class BasedPanel : SettingPanelBase
    {
        internal BindingList<AccountListBoxItem> AccountsList { get; } = new();

        internal record AccountListBoxItem(UserAccount AccountSettings, bool IsPrimary)
        {
            public string DisplayText
                => this.ComposeDisplayText();

            private string ComposeDisplayText()
            {
                var authTypeText = this.AccountSettings.TwitterAuthType switch
                {
                    APIAuthType.OAuth1 => "Twitter / OAuth",
                    APIAuthType.TwitterComCookie => "Twitter / Cookie",
                    _ => "Twitter / unknown",
                };
                var accountName = $"@{this.AccountSettings.Username}";
                var suffix = this.IsPrimary ? Properties.Resources.AccountListBoxItem_Primary : "";

                return $"[{authTypeText}] {accountName} {suffix}";
            }
        }

        public BasedPanel()
        {
            this.InitializeComponent();
            this.InitializeBinding();
        }

        private void InitializeBinding()
        {
            this.AccountsListBox.DataSource = this.AccountsList;
            this.AccountsListBox.DisplayMember = nameof(AccountListBoxItem.DisplayText);
        }

        public void LoadConfig(SettingCommon settingCommon)
        {
            using (ControlTransaction.Update(this.AccountsListBox))
            {
                this.AccountsList.Clear();

                var primaryAccountKey = settingCommon.SelectedAccountKey;
                foreach (var accountSettings in settingCommon.UserAccounts)
                {
                    var isPrimary = accountSettings.UniqueKey == primaryAccountKey;
                    var item = new AccountListBoxItem(accountSettings, isPrimary);
                    this.AccountsList.Add(item);
                }
            }
        }

        public void SaveConfig(SettingCommon settingCommon)
        {
            var accounts = this.AccountsList.Select(x => x.AccountSettings).ToList();
            settingCommon.UserAccounts = accounts;

            var primaryItem = this.AccountsList.FirstOrDefault(x => x.IsPrimary);
            if (primaryItem != null)
            {
                var primaryAccountSettings = primaryItem.AccountSettings;
                settingCommon.UserId = primaryAccountSettings.UserId;
                settingCommon.UserName = primaryAccountSettings.Username;
                settingCommon.Token = primaryAccountSettings.Token;
                settingCommon.TokenSecret = primaryAccountSettings.TokenSecret;
                settingCommon.SelectedAccountKey = primaryAccountSettings.UniqueKey;
            }
            else
            {
                settingCommon.UserId = 0;
                settingCommon.UserName = "";
                settingCommon.Token = "";
                settingCommon.TokenSecret = "";
                settingCommon.SelectedAccountKey = null;
            }
        }

        internal void AddAccount(UserAccount accountSettings)
        {
            var duplicatedAccountIndex =
                this.AccountsList.FindIndex(x => x.AccountSettings.UserId == accountSettings.UserId);

            int addIndex;
            if (duplicatedAccountIndex != -1)
            {
                addIndex = duplicatedAccountIndex;
                this.AccountsList[addIndex] = this.AccountsList[addIndex] with { AccountSettings = accountSettings };
            }
            else
            {
                addIndex = this.AccountsList.Count;
                this.AccountsList.Add(new(accountSettings, IsPrimary: addIndex == 0));
            }

            this.AccountsListBox.SelectedIndex = addIndex;
        }

        private void RemoveAccountAt(int index)
        {
            var removingPrimaryAccount = this.AccountsList[index].IsPrimary;
            this.AccountsList.RemoveAt(index);

            if (removingPrimaryAccount && this.AccountsList.Count >= 1)
                this.MakeAccountPrimaryAt(0);
        }

        private void MakeAccountPrimaryAt(int index)
        {
            var oldPrimaryIndex = this.AccountsList.FindIndex(x => x.IsPrimary);
            if (oldPrimaryIndex != -1)
            {
                if (oldPrimaryIndex == index)
                    return;

                this.AccountsList[oldPrimaryIndex] =
                    this.AccountsList[oldPrimaryIndex] with { IsPrimary = false };
            }

            this.AccountsList[index] =
                this.AccountsList[index] with { IsPrimary = true };
        }

        private void RemoveAccountButton_Click(object sender, EventArgs e)
        {
            var selectedIndex = this.AccountsListBox.SelectedIndex;
            if (selectedIndex == -1)
                return;

            using (ControlTransaction.Update(this.AccountsListBox))
                this.RemoveAccountAt(selectedIndex);
        }

        private void MakePrimaryButton_Click(object sender, EventArgs e)
        {
            var selectedIndex = this.AccountsListBox.SelectedIndex;
            if (selectedIndex == -1)
                return;

            using (ControlTransaction.Update(this.AccountsListBox))
                this.MakeAccountPrimaryAt(selectedIndex);
        }
    }
}
