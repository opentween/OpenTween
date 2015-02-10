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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Resources;
using OpenTween.Api;
using OpenTween.Connection;
using OpenTween.Thumbnail;
using System.Threading.Tasks;
using OpenTween.Setting.Panel;

namespace OpenTween
{
    public partial class AppendSettingDialog : OTBaseForm
    {
        public event EventHandler<IntervalChangedEventArgs> IntervalChanged;

        internal Twitter tw;

        private bool _ValidationError = false;
        private long? InitialUserId;

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

            var activeUser = settingCommon.UserAccounts.FirstOrDefault(x => x.UserId == this.tw.UserId);
            if (activeUser != null)
            {
                this.BasedPanel.AuthUserCombo.SelectedItem = activeUser;
                this.InitialUserId = activeUser.UserId;
            }
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

            var userAccountIdx = this.BasedPanel.AuthUserCombo.SelectedIndex;
            if (userAccountIdx != -1)
            {
                var u = settingCommon.UserAccounts[userAccountIdx];
                this.tw.Initialize(u.Token, u.TokenSecret, u.Username, u.UserId);

                if (u.UserId == 0)
                {
                    this.tw.VerifyCredentials();
                    u.UserId = this.tw.UserId;
                }
            }
            else
            {
                this.tw.ClearAuthInfo();
                this.tw.Initialize("", "", "", 0);
            }
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

            if (pnl.Name == "PreviewPanel")
            {
                if (GrowlHelper.IsDllExists)
                {
                    this.PreviewPanel.IsNotifyUseGrowlCheckBox.Enabled = true;
                }
                else
                {
                    this.PreviewPanel.IsNotifyUseGrowlCheckBox.Enabled = false;
                }
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (MyCommon.IsNetworkAvailable() &&
                (this.ShortUrlPanel.ComboBoxAutoShortUrlFirst.SelectedIndex == (int)MyCommon.UrlConverter.Bitly || this.ShortUrlPanel.ComboBoxAutoShortUrlFirst.SelectedIndex == (int)MyCommon.UrlConverter.Jmp))
            {
                // bit.ly 短縮機能実装のプライバシー問題の暫定対応
                // bit.ly 使用時はログインIDとAPIキーの指定を必須とする
                // 参照: http://sourceforge.jp/projects/opentween/lists/archive/dev/2012-January/000020.html
                if (string.IsNullOrEmpty(this.ShortUrlPanel.TextBitlyId.Text) || string.IsNullOrEmpty(this.ShortUrlPanel.TextBitlyPw.Text))
                {
                    MessageBox.Show("bit.ly のログイン名とAPIキーの指定は必須項目です。", Application.ProductName);
                    _ValidationError = true;
                    TreeViewSetting.SelectedNode = TreeViewSetting.Nodes["ConnectionNode"].Nodes["ShortUrlNode"]; // 動作タブを選択
                    TreeViewSetting.Select();
                    this.ShortUrlPanel.TextBitlyId.Focus();
                    return;
                }

                if (!BitlyValidation(this.ShortUrlPanel.TextBitlyId.Text, this.ShortUrlPanel.TextBitlyPw.Text))
                {
                    MessageBox.Show(Properties.Resources.SettingSave_ClickText1);
                    _ValidationError = true;
                    TreeViewSetting.SelectedNode = TreeViewSetting.Nodes["ConnectionNode"].Nodes["ShortUrlNode"]; // 動作タブを選択
                    TreeViewSetting.Select();
                    this.ShortUrlPanel.TextBitlyId.Focus();
                    return;
                }
                else
                {
                    _ValidationError = false;
                }
            }
            else
            {
                _ValidationError = false;
            }
        }

        private void Setting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MyCommon._endingFlag) return;

            if (this.BasedPanel.AuthUserCombo.SelectedIndex == -1 && e.CloseReason == CloseReason.None)
            {
                if (MessageBox.Show(Properties.Resources.Setting_FormClosing1, "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            if (_ValidationError)
            {
                e.Cancel = true;
            }
            if (e.Cancel == false && TreeViewSetting.SelectedNode != null)
            {
                var curPanel = (SettingPanelBase)TreeViewSetting.SelectedNode.Tag;
                curPanel.Visible = false;
                curPanel.Enabled = false;
            }
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            this.TreeViewSetting.Nodes["BasedNode"].Tag = BasedPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["PeriodNode"].Tag = GetPeriodPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["StartUpNode"].Tag = StartupPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["GetCountNode"].Tag = GetCountPanel;
            this.TreeViewSetting.Nodes["ActionNode"].Tag = ActionPanel;
            this.TreeViewSetting.Nodes["ActionNode"].Nodes["TweetActNode"].Tag = TweetActPanel;
            this.TreeViewSetting.Nodes["PreviewNode"].Tag = PreviewPanel;
            this.TreeViewSetting.Nodes["PreviewNode"].Nodes["TweetPrvNode"].Tag = TweetPrvPanel;
            this.TreeViewSetting.Nodes["PreviewNode"].Nodes["NotifyNode"].Tag = NotifyPanel;
            this.TreeViewSetting.Nodes["FontNode"].Tag = FontPanel;
            this.TreeViewSetting.Nodes["FontNode"].Nodes["FontNode2"].Tag = FontPanel2;
            this.TreeViewSetting.Nodes["ConnectionNode"].Tag = ConnectionPanel;
            this.TreeViewSetting.Nodes["ConnectionNode"].Nodes["ProxyNode"].Tag = ProxyPanel;
            this.TreeViewSetting.Nodes["ConnectionNode"].Nodes["CooperateNode"].Tag = CooperatePanel;
            this.TreeViewSetting.Nodes["ConnectionNode"].Nodes["ShortUrlNode"].Tag = ShortUrlPanel;

            this.TreeViewSetting.SelectedNode = this.TreeViewSetting.Nodes[0];
            this.TreeViewSetting.ExpandAll();

            ActiveControl = BasedPanel.StartAuthButton;
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

        private void StartAuthButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.ApplyNetworkSettings();

                var newAccount = this.PinAuth();
                if (newAccount == null)
                    return;

                var authUserCombo = this.BasedPanel.AuthUserCombo;

                var oldAccount = authUserCombo.Items.Cast<UserAccount>()
                    .FirstOrDefault(x => x.UserId == this.tw.UserId);

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

                MessageBox.Show(this, Properties.Resources.AuthorizeButton_Click1,
                    "Authenticate", MessageBoxButtons.OK);
            }
            catch (WebApiException ex)
            {
                var message = Properties.Resources.AuthorizeButton_Click2 + Environment.NewLine + ex.Message;
                MessageBox.Show(this, message, "Authenticate", MessageBoxButtons.OK);
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

            HttpTwitter.TwitterUrl = this.ConnectionPanel.TwitterAPIText.Text.Trim();
        }

        private UserAccount PinAuth()
        {
            this.tw.Initialize("", "", "", 0);

            var pinPageUrl = "";
            var err = this.tw.StartAuthentication(ref pinPageUrl);
            if (!string.IsNullOrEmpty(err))
                throw new WebApiException(err);

            var pin = AuthDialog.DoAuth(this, pinPageUrl);
            if (string.IsNullOrEmpty(pin))
                return null; // キャンセルされた場合

            var err2 = this.tw.Authenticate(pin);
            if (!string.IsNullOrEmpty(err2))
                throw new WebApiException(err2);

            return new UserAccount
            {
                Username = this.tw.Username,
                UserId = this.tw.UserId,
                Token = this.tw.AccessToken,
                TokenSecret = this.tw.AccessTokenSecret,
            };
        }

        private void CheckPostAndGet_CheckedChanged(object sender, EventArgs e)
        {
            this.GetPeriodPanel.LabelPostAndGet.Visible = this.GetPeriodPanel.CheckPostAndGet.Checked && !tw.UserStreamEnabled;
        }

        private void Setting_Shown(object sender, EventArgs e)
        {
            do
            {
                Thread.Sleep(10);
                if (this.Disposing || this.IsDisposed) return;
            } while (!this.IsHandleCreated);
            this.TopMost = this.PreviewPanel.CheckAlwaysTop.Checked;

            this.GetPeriodPanel.LabelPostAndGet.Visible = this.GetPeriodPanel.CheckPostAndGet.Checked && !tw.UserStreamEnabled;
            this.GetPeriodPanel.LabelUserStreamActive.Visible = tw.UserStreamEnabled;
        }

        private bool BitlyValidation(string id, string apikey)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(apikey))
            {
                return false;
            }

            string req = "http://api.bit.ly/v3/validate";
            string content = "";
            Dictionary<string, string> param = new Dictionary<string, string>();

            param.Add("login", ApplicationSettings.BitlyLoginId);
            param.Add("apiKey", ApplicationSettings.BitlyApiKey);
            param.Add("x_login", id);
            param.Add("x_apiKey", apikey);
            param.Add("format", "txt");

            if (!(new HttpVarious()).PostData(req, param, out content))
            {
                return true;             // 通信エラーの場合はとりあえずチェックを通ったことにする
            }
            else if (content.Trim() == "1")
            {
                return true;             // 検証成功
            }
            else if (content.Trim() == "0")
            {
                return false;            // 検証失敗 APIキーとIDの組み合わせが違う
            }
            else
            {
                return true;             // 規定外応答：通信エラーの可能性があるためとりあえずチェックを通ったことにする
            }
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            _ValidationError = false;
        }

        private void OpenUrl(string url)
        {
            string myPath = url;
            string path = this.ActionPanel.BrowserPathText.Text;
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    if (path.StartsWith("\"") && path.Length > 2 && path.IndexOf("\"", 2) > -1)
                    {
                        int sep = path.IndexOf("\"", 2);
                        string browserPath = path.Substring(1, sep - 1);
                        string arg = "";
                        if (sep < path.Length - 1)
                        {
                            arg = path.Substring(sep + 1);
                        }
                        myPath = arg + " " + myPath;
                        System.Diagnostics.Process.Start(browserPath, myPath);
                    }
                    else
                    {
                        System.Diagnostics.Process.Start(path, myPath);
                    }
                }
                else
                {
                    System.Diagnostics.Process.Start(myPath);
                }
            }
            catch(Exception)
            {
            }
        }

        private void CreateAccountButton_Click(object sender, EventArgs e)
        {
            this.OpenUrl("https://twitter.com/signup");
        }

        private void GetPeriodPanel_IntervalChanged(object sender, IntervalChangedEventArgs e)
        {
            if (this.IntervalChanged != null)
                this.IntervalChanged(sender, e);
        }
    }

    public class IntervalChangedEventArgs : EventArgs
    {
        public bool UserStream;
        public bool Timeline;
        public bool Reply;
        public bool DirectMessage;
        public bool PublicSearch;
        public bool Lists;
        public bool UserTimeline;

        public static IntervalChangedEventArgs ResetAll
        {
            get
            {
                return new IntervalChangedEventArgs()
                {
                    UserStream = true,
                    Timeline = true,
                    Reply = true,
                    DirectMessage = true,
                    PublicSearch = true,
                    Lists = true,
                    UserTimeline = true,
                };
            }
        }
    }
}
