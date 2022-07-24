// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTween.Connection;
using OpenTween.Setting.Panel;
using OpenTween.Thumbnail;

namespace OpenTween
{
    public partial class AppendSettingDialog : OTBaseForm
    {
        public event EventHandler<IntervalChangedEventArgs>? IntervalChanged;

        public AppendSettingDialog()
        {
            this.InitializeComponent();

            this.BasedPanel.StartAuthButton.Click += this.StartAuthButton_Click;
            this.BasedPanel.CreateAccountButton.Click += this.CreateAccountButton_Click;
            this.GetPeriodPanel.CheckPostAndGet.CheckedChanged += this.CheckPostAndGet_CheckedChanged;
            this.ActionPanel.UReadMng.CheckedChanged += this.UReadMng_CheckedChanged;

            this.Icon = Properties.Resources.MIcon;
        }

        public void LoadConfig(SettingCommon settingCommon, SettingLocal settingLocal)
        {
            this.BasedPanel.LoadConfig(settingCommon);
            this.GetPeriodPanel.LoadConfig(settingCommon);
            this.StartupPanel.LoadConfig(settingCommon);
            this.TweetPrvPanel.LoadConfig(settingCommon);
            this.TweetActPanel.LoadConfig(settingCommon, settingLocal);
            this.ActionPanel.LoadConfig(settingCommon, settingLocal);
            this.FontPanel.LoadConfig(settingLocal);
            this.FontPanel2.LoadConfig(settingLocal);
            this.PreviewPanel.LoadConfig(settingCommon);
            this.GetCountPanel.LoadConfig(settingCommon);
            this.ShortUrlPanel.LoadConfig(settingCommon);
            this.ProxyPanel.LoadConfig(settingLocal);
            this.CooperatePanel.LoadConfig(settingCommon);
            this.ConnectionPanel.LoadConfig(settingCommon);
            this.NotifyPanel.LoadConfig(settingCommon);
        }

        public void SaveConfig(SettingCommon settingCommon, SettingLocal settingLocal)
        {
            this.BasedPanel.SaveConfig(settingCommon);
            this.GetPeriodPanel.SaveConfig(settingCommon);
            this.StartupPanel.SaveConfig(settingCommon);
            this.TweetPrvPanel.SaveConfig(settingCommon);
            this.TweetActPanel.SaveConfig(settingCommon, settingLocal);
            this.ActionPanel.SaveConfig(settingCommon, settingLocal);
            this.FontPanel.SaveConfig(settingLocal);
            this.FontPanel2.SaveConfig(settingLocal);
            this.PreviewPanel.SaveConfig(settingCommon);
            this.GetCountPanel.SaveConfig(settingCommon);
            this.ShortUrlPanel.SaveConfig(settingCommon);
            this.ProxyPanel.SaveConfig(settingLocal);
            this.CooperatePanel.SaveConfig(settingCommon);
            this.ConnectionPanel.SaveConfig(settingCommon);
            this.NotifyPanel.SaveConfig(settingCommon);
        }

        private void TreeViewSetting_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (this.TreeViewSetting.SelectedNode == null) return;
            var pnl = (SettingPanelBase)this.TreeViewSetting.SelectedNode.Tag;
            if (pnl == null) return;
            pnl.Enabled = false;
            pnl.Visible = false;
        }

        private void TreeViewSetting_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            var pnl = (SettingPanelBase)e.Node.Tag;
            if (pnl == null) return;
            pnl.Enabled = true;
            pnl.Visible = true;
        }

        private void Setting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MyCommon.EndingFlag) return;

            if (this.BasedPanel.AuthUserCombo.SelectedIndex == -1 && e.CloseReason == CloseReason.None)
            {
                if (MessageBox.Show(Properties.Resources.Setting_FormClosing1, "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            if (e.Cancel == false && this.TreeViewSetting.SelectedNode != null)
            {
                var curPanel = (SettingPanelBase)this.TreeViewSetting.SelectedNode.Tag;
                curPanel.Visible = false;
                curPanel.Enabled = false;
            }
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            this.TreeViewSetting.Nodes["BasedNode"].Tag = this.BasedPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["PeriodNode"].Tag = this.GetPeriodPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["StartUpNode"].Tag = this.StartupPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["GetCountNode"].Tag = this.GetCountPanel;
            this.TreeViewSetting.Nodes["ActionNode"].Tag = this.ActionPanel;
            this.TreeViewSetting.Nodes["ActionNode"].Nodes["TweetActNode"].Tag = this.TweetActPanel;
            this.TreeViewSetting.Nodes["PreviewNode"].Tag = this.PreviewPanel;
            this.TreeViewSetting.Nodes["PreviewNode"].Nodes["TweetPrvNode"].Tag = this.TweetPrvPanel;
            this.TreeViewSetting.Nodes["PreviewNode"].Nodes["NotifyNode"].Tag = this.NotifyPanel;
            this.TreeViewSetting.Nodes["FontNode"].Tag = this.FontPanel;
            this.TreeViewSetting.Nodes["FontNode"].Nodes["FontNode2"].Tag = this.FontPanel2;
            this.TreeViewSetting.Nodes["ConnectionNode"].Tag = this.ConnectionPanel;
            this.TreeViewSetting.Nodes["ConnectionNode"].Nodes["ProxyNode"].Tag = this.ProxyPanel;
            this.TreeViewSetting.Nodes["ConnectionNode"].Nodes["CooperateNode"].Tag = this.CooperatePanel;
            this.TreeViewSetting.Nodes["ConnectionNode"].Nodes["ShortUrlNode"].Tag = this.ShortUrlPanel;

            this.TreeViewSetting.SelectedNode = this.TreeViewSetting.Nodes[0];
            this.TreeViewSetting.ExpandAll();

            this.ActiveControl = this.BasedPanel.StartAuthButton;
        }

        private void UReadMng_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ActionPanel.UReadMng.Checked == true)
            {
                this.StartupPanel.StartupReaded.Enabled = true;
            }
            else
            {
                this.StartupPanel.StartupReaded.Enabled = false;
            }
        }

        private async void StartAuthButton_Click(object sender, EventArgs e)
        {
            using (ControlTransaction.Disabled(this.BasedPanel.StartAuthButton))
            {
                try
                {
                    this.ApplyNetworkSettings();

                    var newAccount = await this.PinAuth();
                    if (newAccount == null)
                        return;

                    var authUserCombo = this.BasedPanel.AuthUserCombo;

                    var oldAccount = authUserCombo.Items.Cast<UserAccount>()
                        .FirstOrDefault(x => x.UserId == newAccount.UserId);

                    int idx;
                    if (oldAccount != null)
                    {
                        idx = authUserCombo.Items.IndexOf(oldAccount);
                        authUserCombo.Items[idx] = newAccount;
                    }
                    else
                    {
                        idx = authUserCombo.Items.Add(newAccount);
                    }

                    authUserCombo.SelectedIndex = idx;

                    MessageBox.Show(
                        this,
                        Properties.Resources.AuthorizeButton_Click1,
                        "Authenticate",
                        MessageBoxButtons.OK);
                }
                catch (WebApiException ex)
                {
                    var message = Properties.Resources.AuthorizeButton_Click2 + Environment.NewLine + ex.Message;
                    MessageBox.Show(this, message, "Authenticate", MessageBoxButtons.OK);
                }
            }
        }

        /// <summary>
        /// 現在設定画面に入力されているネットワーク関係の設定を適用します
        /// </summary>
        public void ApplyNetworkSettings()
        {
            ProxyType proxyType;
            if (this.ProxyPanel.RadioProxyNone.Checked)
                proxyType = ProxyType.None;
            else if (this.ProxyPanel.RadioProxyIE.Checked)
                proxyType = ProxyType.IE;
            else
                proxyType = ProxyType.Specified;

            var proxyAddress = this.ProxyPanel.TextProxyAddress.Text.Trim();
            var proxyPort = int.Parse(this.ProxyPanel.TextProxyPort.Text.Trim());
            var proxyUser = this.ProxyPanel.TextProxyUser.Text.Trim();
            var proxyPassword = this.ProxyPanel.TextProxyPassword.Text.Trim();
            Networking.SetWebProxy(proxyType, proxyAddress, proxyPort, proxyUser, proxyPassword);

            var timeout = int.Parse(this.ConnectionPanel.ConnectionTimeOut.Text.Trim());
            Networking.DefaultTimeout = TimeSpan.FromSeconds(timeout);

            var uploadImageTimeout = int.Parse(this.ConnectionPanel.UploadImageTimeout.Text.Trim());
            Networking.UploadImageTimeout = TimeSpan.FromSeconds(uploadImageTimeout);

            Networking.ForceIPv4 = this.ConnectionPanel.checkBoxForceIPv4.Checked;

            TwitterApiConnection.RestApiHost = this.ConnectionPanel.TwitterAPIText.Text.Trim();
        }

        private async Task<UserAccount?> PinAuth()
        {
            var requestToken = await TwitterApiConnection.GetRequestTokenAsync();

            var pinPageUrl = TwitterApiConnection.GetAuthorizeUri(requestToken);

            var browserPath = this.ActionPanel.BrowserPathText.Text;
            var pin = AuthDialog.DoAuth(this, pinPageUrl, browserPath);
            if (MyCommon.IsNullOrEmpty(pin))
                return null; // キャンセルされた場合

            var accessTokenResponse = await TwitterApiConnection.GetAccessTokenAsync(requestToken, pin);

            return new UserAccount
            {
                Username = accessTokenResponse["screen_name"],
                UserId = long.Parse(accessTokenResponse["user_id"]),
                Token = accessTokenResponse["oauth_token"],
                TokenSecret = accessTokenResponse["oauth_token_secret"],
            };
        }

        private void CheckPostAndGet_CheckedChanged(object sender, EventArgs e)
            => this.GetPeriodPanel.LabelPostAndGet.Visible = this.GetPeriodPanel.CheckPostAndGet.Checked;

        private void Setting_Shown(object sender, EventArgs e)
        {
            do
            {
                Thread.Sleep(10);
                if (this.Disposing || this.IsDisposed) return;
            }
            while (!this.IsHandleCreated);
            this.TopMost = this.PreviewPanel.CheckAlwaysTop.Checked;

            this.GetPeriodPanel.LabelPostAndGet.Visible = this.GetPeriodPanel.CheckPostAndGet.Checked;
        }

        private async Task OpenUrl(string url)
        {
            var browserPathWithArgs = this.ActionPanel.BrowserPathText.Text;
            await MyCommon.OpenInBrowserAsync(this, browserPathWithArgs, url);
        }

        private async void CreateAccountButton_Click(object sender, EventArgs e)
            => await this.OpenUrl("https://twitter.com/signup");

        private void GetPeriodPanel_IntervalChanged(object sender, IntervalChangedEventArgs e)
            => this.IntervalChanged?.Invoke(sender, e);
    }

    public class IntervalChangedEventArgs : EventArgs
    {
        public bool Timeline;
        public bool Reply;
        public bool DirectMessage;
        public bool PublicSearch;
        public bool Lists;
        public bool UserTimeline;

        public static IntervalChangedEventArgs ResetAll => new()
        {
            Timeline = true,
            Reply = true,
            DirectMessage = true,
            PublicSearch = true,
            Lists = true,
            UserTimeline = true,
        };
    }
}
