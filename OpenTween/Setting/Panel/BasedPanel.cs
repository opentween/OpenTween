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
using OpenTween;

namespace OpenTween.Setting.Panel
{
    public partial class BasedPanel : SettingPanelBase
    {
        private MastodonCredential? mastodonCredential = null;

        public BasedPanel()
        {
            this.InitializeComponent();
            this.RefreshMastodonCredential();
        }

        public void LoadConfig(SettingCommon settingCommon)
        {
            var accounts = settingCommon.UserAccounts;

            using (ControlTransaction.Update(this.AuthUserCombo))
            {
                this.AuthUserCombo.Items.Clear();
                this.AuthUserCombo.Items.AddRange(accounts.ToArray());

                var primaryIndex = accounts.FindIndex(x => x.Primary);
                if (primaryIndex != -1)
                    this.AuthUserCombo.SelectedIndex = primaryIndex;
            }

            this.mastodonCredential = settingCommon.MastodonPrimaryAccount;
            this.RefreshMastodonCredential();
        }

        public void SaveConfig(SettingCommon settingCommon)
        {
            var accounts = this.AuthUserCombo.Items.Cast<UserAccount>().ToList();
            settingCommon.UserAccounts = accounts;

            var selectedIndex = this.AuthUserCombo.SelectedIndex;
            if (selectedIndex != -1)
            {
                foreach (var (account, index) in accounts.WithIndex())
                    account.Primary = selectedIndex == index;

                var selectedAccount = accounts[selectedIndex];
                settingCommon.UserId = selectedAccount.UserId;
                settingCommon.UserName = selectedAccount.Username;
                settingCommon.Token = selectedAccount.AccessToken;
                settingCommon.TokenSecret = selectedAccount.AccessSecretPlain;
            }
            else
            {
                settingCommon.UserId = 0;
                settingCommon.UserName = "";
                settingCommon.Token = "";
                settingCommon.TokenSecret = "";
            }

            var mastodonCredential = this.mastodonCredential;
            if (mastodonCredential != null)
            {
                mastodonCredential.Primary = true;
                settingCommon.MastodonAccounts = new[] { mastodonCredential };
            }
            else
            {
                settingCommon.MastodonAccounts = new MastodonCredential[0];
            }
        }

        private void RefreshMastodonCredential()
        {
            if (this.mastodonCredential != null)
                this.labelMastodonAccount.Text = this.mastodonCredential.Username;
            else
                this.labelMastodonAccount.Text = "(未設定)";
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

        private async void ButtonMastodonAuth_Click(object sender, EventArgs e)
        {
            var ret = InputDialog.Show(this, "インスタンスのURL (例: https://mstdn.jp/)", ApplicationSettings.ApplicationName, out var instanceUriStr);
            if (ret != DialogResult.OK)
                return;

            if (!Uri.TryCreate(instanceUriStr, UriKind.Absolute, out var instanceUri))
                return;

            try
            {
                var application = await Mastodon.RegisterClientAsync(instanceUri);

                var authorizeUri = Mastodon.GetAuthorizeUri(instanceUri, application.ClientId);

                var code = ((AppendSettingDialog)this.ParentForm).ShowAuthDialog(authorizeUri);
                if (MyCommon.IsNullOrEmpty(code))
                    return;

                var accessToken = await Mastodon.GetAccessTokenAsync(instanceUri, application.ClientId, application.ClientSecret, code);

                this.mastodonCredential = await Mastodon.VerifyCredentialAsync(instanceUri, accessToken);

                this.RefreshMastodonCredential();
            }
            catch (WebApiException ex)
            {
                var message = Properties.Resources.AuthorizeButton_Click2 + Environment.NewLine + ex.Message;
                MessageBox.Show(this, message, "Authenticate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
