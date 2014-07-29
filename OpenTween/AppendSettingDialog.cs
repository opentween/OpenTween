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
        private static AppendSettingDialog _instance = new AppendSettingDialog();
        private Twitter tw;

        private bool _ValidationError = false;
        private MyCommon.EVENTTYPE _MyEventNotifyFlag;
        private MyCommon.EVENTTYPE _isMyEventNotifyFlag;

        public List<UserAccount> UserAccounts;
        private long? InitialUserId;
        public bool IsRemoveSameEvent;

        public TwitterConfiguration TwitterConfiguration = TwitterConfiguration.DefaultConfiguration();

        private string _pin;

        public event EventHandler<IntervalChangedEventArgs> IntervalChanged;

        public void LoadConfig(SettingCommon settingCommon, SettingLocal settingLocal)
        {
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
        }

        public void SaveConfig(SettingCommon settingCommon, SettingLocal settingLocal)
        {
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

            this.UserAccounts.Clear();
            foreach (object u in this.BasedPanel.AuthUserCombo.Items)
            {
                this.UserAccounts.Add((UserAccount)u);
            }
            if (this.BasedPanel.AuthUserCombo.SelectedIndex > -1)
            {
                foreach (UserAccount u in this.UserAccounts)
                {
                    if (u.Username.ToLower() == ((UserAccount)this.BasedPanel.AuthUserCombo.SelectedItem).Username.ToLower())
                    {
                        tw.Initialize(u.Token, u.TokenSecret, u.Username, u.UserId);
                        if (u.UserId == 0)
                        {
                            tw.VerifyCredentials();
                            u.UserId = tw.UserId;
                        }
                        break;
                    }
                }
            }
            else
            {
                tw.ClearAuthInfo();
                tw.Initialize("", "", "", 0);
            }

#if UA
            //フォロー
            if (this.FollowCheckBox.Checked)
            {
                //現在の設定内容で通信
                HttpConnection.ProxyType ptype;
                if (RadioProxyNone.Checked)
                {
                    ptype = HttpConnection.ProxyType.None;
                }
                else if (RadioProxyIE.Checked)
                {
                    ptype = HttpConnection.ProxyType.IE;
                }
                else
                {
                    ptype = HttpConnection.ProxyType.Specified;
                }
                string padr = TextProxyAddress.Text.Trim();
                int pport = int.Parse(TextProxyPort.Text.Trim());
                string pusr = TextProxyUser.Text.Trim();
                string ppw = TextProxyPassword.Text.Trim();
                HttpConnection.InitializeConnection(20, ptype, padr, pport, pusr, ppw);

                string ret = tw.PostFollowCommand(ApplicationSettings.FeedbackTwitterName);
            }
#endif
            try
            {
                EventNotifyEnabled = this.NotifyPanel.CheckEventNotify.Checked;
                GetEventNotifyFlag(ref _MyEventNotifyFlag, ref _isMyEventNotifyFlag);
                ForceEventNotify = this.NotifyPanel.CheckForceEventNotify.Checked;
                FavEventUnread = this.NotifyPanel.CheckFavEventUnread.Checked;
                EventSoundFile = (string)this.NotifyPanel.ComboBoxEventNotifySound.SelectedItem;
                this.IsRemoveSameEvent = this.NotifyPanel.IsRemoveSameFavEventCheckBox.Checked;
            }
            catch(Exception)
            {
                MessageBox.Show(Properties.Resources.Save_ClickText3);
                this.DialogResult = DialogResult.Cancel;
                return;
            }
        }

        private void Setting_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MyCommon._endingFlag) return;

            if (this.DialogResult == DialogResult.Cancel)
            {
                //キャンセル時は画面表示時のアカウントに戻す
                //キャンセル時でも認証済みアカウント情報は保存する
                this.UserAccounts.Clear();
                foreach (object u in this.BasedPanel.AuthUserCombo.Items)
                {
                    this.UserAccounts.Add((UserAccount)u);
                }
                //アクティブユーザーを起動時のアカウントに戻す（起動時アカウントなければ何もしない）
                bool userSet = false;
                if (this.InitialUserId != null)
                {
                    foreach (UserAccount u in this.UserAccounts)
                    {
                        if (u.UserId == this.InitialUserId)
                        {
                            tw.Initialize(u.Token, u.TokenSecret, u.Username, u.UserId);
                            userSet = true;
                            break;
                        }
                    }
                }
                //認証済みアカウントが削除されていた場合、もしくは起動時アカウントがなかった場合は、
                //アクティブユーザーなしとして初期化
                if (!userSet)
                {
                    tw.ClearAuthInfo();
                    tw.Initialize("", "", "", 0);
                }
            }

            if (tw != null && string.IsNullOrEmpty(tw.Username) && e.CloseReason == CloseReason.None)
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
            tw = ((TweenMain)this.Owner).TwitterInstance;
            //this.AuthStateLabel.Enabled = true;
            //this.AuthUserLabel.Enabled = true;
            this.BasedPanel.AuthClearButton.Enabled = true;

            //if (tw.Username == "")
            //{
            //    //this.AuthStateLabel.Text = Properties.Resources.AuthorizeButton_Click4
            //    //this.AuthUserLabel.Text = ""
            //    //this.Save.Enabled = false
            //}
            //else
            //{
            //    //this.AuthStateLabel.Text = Properties.Resources.AuthorizeButton_Click3;
            //    //if (TwitterApiInfo.AccessLevel == ApiAccessLevel.ReadWrite)
            //    //{
            //    //    this.AuthStateLabel.Text += "(xAuth)";
            //    //}
            //    //else if (TwitterApiInfo.AccessLevel == ApiAccessLevel.ReadWriteAndDirectMessage)
            //    //{
            //    //    this.AuthStateLabel.Text += "(OAuth)";
            //    //}
            //    //this.AuthUserLabel.Text = tw.Username;
            //}

            this.BasedPanel.AuthUserCombo.Items.Clear();
            if (this.UserAccounts.Count > 0)
            {
                this.BasedPanel.AuthUserCombo.Items.AddRange(this.UserAccounts.ToArray());
                foreach (UserAccount u in this.UserAccounts)
                {
                    if (u.UserId == tw.UserId)
                    {
                        this.BasedPanel.AuthUserCombo.SelectedItem = u;
                        this.InitialUserId = u.UserId;
                        break;
                    }
                }
            }

            ApplyEventNotifyFlag(EventNotifyEnabled, EventNotifyFlag, IsMyEventNotifyFlag);
            this.NotifyPanel.CheckForceEventNotify.Checked = ForceEventNotify;
            this.NotifyPanel.CheckFavEventUnread.Checked = FavEventUnread;
            SoundFileListup();

            this.NotifyPanel.IsRemoveSameFavEventCheckBox.Checked = this.IsRemoveSameEvent;

            this.TreeViewSetting.Nodes["BasedNode"].Tag = BasedPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["PeriodNode"].Tag = GetPeriodPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["StartUpNode"].Tag = StartupPanel;
            this.TreeViewSetting.Nodes["BasedNode"].Nodes["GetCountNode"].Tag = GetCountPanel;
            //this.TreeViewSetting.Nodes["BasedNode"].Nodes["UserStreamNode"].Tag = UserStreamPanel;
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

            //TreeViewSetting.SelectedNode = TreeViewSetting.TopNode;
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

        private void btnFontAndColor_Click(object sender, EventArgs e) // Handles btnUnread.Click, btnDetail.Click, btnListFont.Click, btnInputFont.Click
        {
            Button Btn = (Button) sender;
            DialogResult rtn;

            FontDialog1.AllowVerticalFonts = false;
            FontDialog1.AllowScriptChange = true;
            FontDialog1.AllowSimulations = true;
            FontDialog1.AllowVectorFonts = true;
            FontDialog1.FixedPitchOnly = false;
            FontDialog1.FontMustExist = true;
            FontDialog1.ScriptsOnly = false;
            FontDialog1.ShowApply = false;
            FontDialog1.ShowEffects = true;
            FontDialog1.ShowColor = true;

            switch (Btn.Name)
            {
                case "btnUnread":
                    FontDialog1.Color = this.FontPanel.lblUnread.ForeColor;
                    FontDialog1.Font = this.FontPanel.lblUnread.Font;
                    break;
                case "btnDetail":
                    FontDialog1.Color = this.FontPanel.lblDetail.ForeColor;
                    FontDialog1.Font = this.FontPanel.lblDetail.Font;
                    break;
                case "btnListFont":
                    FontDialog1.Color = this.FontPanel.lblListFont.ForeColor;
                    FontDialog1.Font = this.FontPanel.lblListFont.Font;
                    break;
                case "btnInputFont":
                    FontDialog1.Color = this.FontPanel2.lblInputFont.ForeColor;
                    FontDialog1.Font = this.FontPanel2.lblInputFont.Font;
                    break;
            }

            try
            {
                rtn = FontDialog1.ShowDialog();
            }
            catch(ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            if (rtn == DialogResult.Cancel) return;

            switch (Btn.Name)
            {
                case "btnUnread":
                    this.FontPanel.lblUnread.ForeColor = FontDialog1.Color;
                    this.FontPanel.lblUnread.Font = FontDialog1.Font;
                    break;
                case "btnDetail":
                    this.FontPanel.lblDetail.ForeColor = FontDialog1.Color;
                    this.FontPanel.lblDetail.Font = FontDialog1.Font;
                    break;
                case "btnListFont":
                    this.FontPanel.lblListFont.ForeColor = FontDialog1.Color;
                    this.FontPanel.lblListFont.Font = FontDialog1.Font;
                    break;
                case "btnInputFont":
                    this.FontPanel2.lblInputFont.ForeColor = FontDialog1.Color;
                    this.FontPanel2.lblInputFont.Font = FontDialog1.Font;
                    break;
            }

        }

        private void btnColor_Click(object sender, EventArgs e) //Handles btnSelf.Click, btnAtSelf.Click, btnTarget.Click, btnAtTarget.Click, btnAtFromTarget.Click, btnFav.Click, btnOWL.Click, btnInputBackcolor.Click, btnAtTo.Click, btnListBack.Click, btnDetailBack.Click, btnDetailLink.Click, btnRetweet.Click
        {
            Button Btn = (Button)sender;
            DialogResult rtn;

            ColorDialog1.AllowFullOpen = true;
            ColorDialog1.AnyColor = true;
            ColorDialog1.FullOpen = false;
            ColorDialog1.SolidColorOnly = false;

            switch (Btn.Name)
            {
                case "btnSelf":
                    ColorDialog1.Color = this.FontPanel2.lblSelf.BackColor;
                    break;
                case "btnAtSelf":
                    ColorDialog1.Color = this.FontPanel2.lblAtSelf.BackColor;
                    break;
                case "btnTarget":
                    ColorDialog1.Color = this.FontPanel2.lblTarget.BackColor;
                    break;
                case "btnAtTarget":
                    ColorDialog1.Color = this.FontPanel2.lblAtTarget.BackColor;
                    break;
                case "btnAtFromTarget":
                    ColorDialog1.Color = this.FontPanel2.lblAtFromTarget.BackColor;
                    break;
                case "btnFav":
                    ColorDialog1.Color = this.FontPanel.lblFav.ForeColor;
                    break;
                case "btnOWL":
                    ColorDialog1.Color = this.FontPanel.lblOWL.ForeColor;
                    break;
                case "btnRetweet":
                    ColorDialog1.Color = this.FontPanel.lblRetweet.ForeColor;
                    break;
                case "btnInputBackcolor":
                    ColorDialog1.Color = this.FontPanel2.lblInputBackcolor.BackColor;
                    break;
                case "btnAtTo":
                    ColorDialog1.Color = this.FontPanel2.lblAtTo.BackColor;
                    break;
                case "btnListBack":
                    ColorDialog1.Color = this.FontPanel2.lblListBackcolor.BackColor;
                    break;
                case "btnDetailBack":
                    ColorDialog1.Color = this.FontPanel.lblDetailBackcolor.BackColor;
                    break;
                case "btnDetailLink":
                    ColorDialog1.Color = this.FontPanel.lblDetailLink.ForeColor;
                    break;
            }

            rtn = ColorDialog1.ShowDialog();

            if (rtn == DialogResult.Cancel) return;

            switch (Btn.Name)
            {
                case "btnSelf":
                    this.FontPanel2.lblSelf.BackColor = ColorDialog1.Color;
                    break;
                case "btnAtSelf":
                    this.FontPanel2.lblAtSelf.BackColor = ColorDialog1.Color;
                    break;
                case "btnTarget":
                    this.FontPanel2.lblTarget.BackColor = ColorDialog1.Color;
                    break;
                case "btnAtTarget":
                    this.FontPanel2.lblAtTarget.BackColor = ColorDialog1.Color;
                    break;
                case "btnAtFromTarget":
                    this.FontPanel2.lblAtFromTarget.BackColor = ColorDialog1.Color;
                    break;
                case "btnFav":
                    this.FontPanel.lblFav.ForeColor = ColorDialog1.Color;
                    break;
                case "btnOWL":
                    this.FontPanel.lblOWL.ForeColor = ColorDialog1.Color;
                    break;
                case "btnRetweet":
                    this.FontPanel.lblRetweet.ForeColor = ColorDialog1.Color;
                    break;
                case "btnInputBackcolor":
                    this.FontPanel2.lblInputBackcolor.BackColor = ColorDialog1.Color;
                    break;
                case "btnAtTo":
                    this.FontPanel2.lblAtTo.BackColor = ColorDialog1.Color;
                    break;
                case "btnListBack":
                    this.FontPanel2.lblListBackcolor.BackColor = ColorDialog1.Color;
                    break;
                case "btnDetailBack":
                    this.FontPanel.lblDetailBackcolor.BackColor = ColorDialog1.Color;
                    break;
                case "btnDetailLink":
                    this.FontPanel.lblDetailLink.ForeColor = ColorDialog1.Color;
                    break;
            }
        }

        public string RecommendStatusText { get; set; }

        public bool EventNotifyEnabled { get; set; }

        public MyCommon.EVENTTYPE EventNotifyFlag
        {
            get
            {
                return _MyEventNotifyFlag;
            }
            set
            {
                _MyEventNotifyFlag = value;
            }
        }

        public MyCommon.EVENTTYPE IsMyEventNotifyFlag
        {
            get
            {
                return _isMyEventNotifyFlag;
            }
            set
            {
                _isMyEventNotifyFlag = value;
            }
        }

        public bool ForceEventNotify { get; set; }
        public bool FavEventUnread { get; set; }

        public string EventSoundFile { get; set; }

        private bool StartAuth()
        {
            //現在の設定内容で通信
            ProxyType ptype;
            if (this.ProxyPanel.RadioProxyNone.Checked)
            {
                ptype = ProxyType.None;
            }
            else if (this.ProxyPanel.RadioProxyIE.Checked)
            {
                ptype = ProxyType.IE;
            }
            else
            {
                ptype = ProxyType.Specified;
            }
            string padr = this.ProxyPanel.TextProxyAddress.Text.Trim();
            int pport = int.Parse(this.ProxyPanel.TextProxyPort.Text.Trim());
            string pusr = this.ProxyPanel.TextProxyUser.Text.Trim();
            string ppw = this.ProxyPanel.TextProxyPassword.Text.Trim();

            //通信基底クラス初期化
            Networking.DefaultTimeout = TimeSpan.FromSeconds(20);
            Networking.SetWebProxy(ptype, padr, pport, pusr, ppw);
            HttpTwitter.TwitterUrl = this.ConnectionPanel.TwitterAPIText.Text.Trim();
            tw.Initialize("", "", "", 0);
            //this.AuthStateLabel.Text = Properties.Resources.AuthorizeButton_Click4;
            //this.AuthUserLabel.Text = "";
            string pinPageUrl = "";
            string rslt = tw.StartAuthentication(ref pinPageUrl);
            if (string.IsNullOrEmpty(rslt))
            {
                string pin = AuthDialog.DoAuth(this, pinPageUrl);
                if (!string.IsNullOrEmpty(pin))
                {
                    this._pin = pin;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.AuthorizeButton_Click2 + Environment.NewLine + rslt, "Authenticate", MessageBoxButtons.OK);
                return false;
            }
        }

        private bool PinAuth()
        {
            string pin = this._pin;   //PIN Code

            string rslt = tw.Authenticate(pin);
            if (string.IsNullOrEmpty(rslt))
            {
                MessageBox.Show(Properties.Resources.AuthorizeButton_Click1, "Authenticate", MessageBoxButtons.OK);
                //this.AuthStateLabel.Text = Properties.Resources.AuthorizeButton_Click3;
                //this.AuthUserLabel.Text = tw.Username;
                int idx = -1;
                UserAccount user = new UserAccount();
                user.Username = tw.Username;
                user.UserId = tw.UserId;
                user.Token = tw.AccessToken;
                user.TokenSecret = tw.AccessTokenSecret;

                foreach (object u in this.BasedPanel.AuthUserCombo.Items)
                {
                    if (((UserAccount)u).Username.ToLower() == tw.Username.ToLower())
                    {
                        idx = this.BasedPanel.AuthUserCombo.Items.IndexOf(u);
                        break;
                    }
                }
                if (idx > -1)
                {
                    this.BasedPanel.AuthUserCombo.Items.RemoveAt(idx);
                    this.BasedPanel.AuthUserCombo.Items.Insert(idx, user);
                    this.BasedPanel.AuthUserCombo.SelectedIndex = idx;
                }
                else
                {
                    this.BasedPanel.AuthUserCombo.SelectedIndex = this.BasedPanel.AuthUserCombo.Items.Add(user);
                }
                //if (TwitterApiInfo.AccessLevel = ApiAccessLevel.ReadWrite)
                //{
                //    this.AuthStateLabel.Text += "(xAuth)";
                //}
                //else if (TwitterApiInfo.AccessLevel == ApiAccessLevel.ReadWriteAndDirectMessage)
                //{
                //    this.AuthStateLabel.Text += "(OAuth)";
                //}
                return true;
            }
            else
            {
                MessageBox.Show(Properties.Resources.AuthorizeButton_Click2 + Environment.NewLine + rslt, "Authenticate", MessageBoxButtons.OK);
                //this.AuthStateLabel.Text = Properties.Resources.AuthorizeButton_Click4;
                //this.AuthUserLabel.Text = "";
                return false;
            }
        }

        private void StartAuthButton_Click(object sender, EventArgs e)
        {
            //this.Save.Enabled = false;
            if (StartAuth())
            {
                if (PinAuth())
                {
                    //this.Save.Enabled = true;
                }
            }
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

        public static AppendSettingDialog Instance
        {
            get { return _instance; }
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

        //private void CheckEventNotify_CheckedChanged(object sender, EventArgs e)
        //                Handles CheckEventNotify.CheckedChanged, CheckFavoritesEvent.CheckStateChanged,
        //                        CheckUnfavoritesEvent.CheckStateChanged, CheckFollowEvent.CheckStateChanged,
        //                        CheckListMemberAddedEvent.CheckStateChanged, CheckListMemberRemovedEvent.CheckStateChanged,
        //                        CheckListCreatedEvent.CheckStateChanged, CheckUserUpdateEvent.CheckStateChanged
        //{
        //    EventNotifyEnabled = CheckEventNotify.Checked;
        //    GetEventNotifyFlag(EventNotifyFlag, IsMyEventNotifyFlag);
        //    ApplyEventNotifyFlag(EventNotifyEnabled, EventNotifyFlag, IsMyEventNotifyFlag);
        //}

        private class EventCheckboxTblElement
        {
            public CheckBox CheckBox;
            public MyCommon.EVENTTYPE Type;
        }

        private EventCheckboxTblElement[] GetEventCheckboxTable()
        {
            EventCheckboxTblElement[] _eventCheckboxTable = new EventCheckboxTblElement[8];

            _eventCheckboxTable[0] = new EventCheckboxTblElement();
            _eventCheckboxTable[0].CheckBox = this.NotifyPanel.CheckFavoritesEvent;
            _eventCheckboxTable[0].Type = MyCommon.EVENTTYPE.Favorite;

            _eventCheckboxTable[1] = new EventCheckboxTblElement();
            _eventCheckboxTable[1].CheckBox = this.NotifyPanel.CheckUnfavoritesEvent;
            _eventCheckboxTable[1].Type = MyCommon.EVENTTYPE.Unfavorite;

            _eventCheckboxTable[2] = new EventCheckboxTblElement();
            _eventCheckboxTable[2].CheckBox = this.NotifyPanel.CheckFollowEvent;
            _eventCheckboxTable[2].Type = MyCommon.EVENTTYPE.Follow;

            _eventCheckboxTable[3] = new EventCheckboxTblElement();
            _eventCheckboxTable[3].CheckBox = this.NotifyPanel.CheckListMemberAddedEvent;
            _eventCheckboxTable[3].Type = MyCommon.EVENTTYPE.ListMemberAdded;

            _eventCheckboxTable[4] = new EventCheckboxTblElement();
            _eventCheckboxTable[4].CheckBox = this.NotifyPanel.CheckListMemberRemovedEvent;
            _eventCheckboxTable[4].Type = MyCommon.EVENTTYPE.ListMemberRemoved;

            _eventCheckboxTable[5] = new EventCheckboxTblElement();
            _eventCheckboxTable[5].CheckBox = this.NotifyPanel.CheckBlockEvent;
            _eventCheckboxTable[5].Type = MyCommon.EVENTTYPE.Block;

            _eventCheckboxTable[6] = new EventCheckboxTblElement();
            _eventCheckboxTable[6].CheckBox = this.NotifyPanel.CheckUserUpdateEvent;
            _eventCheckboxTable[6].Type = MyCommon.EVENTTYPE.UserUpdate;

            _eventCheckboxTable[7] = new EventCheckboxTblElement();
            _eventCheckboxTable[7].CheckBox = this.NotifyPanel.CheckListCreatedEvent;
            _eventCheckboxTable[7].Type = MyCommon.EVENTTYPE.ListCreated;

            return _eventCheckboxTable;
        }

        private void GetEventNotifyFlag(ref MyCommon.EVENTTYPE eventnotifyflag, ref MyCommon.EVENTTYPE isMyeventnotifyflag)
        {
            MyCommon.EVENTTYPE evt = MyCommon.EVENTTYPE.None;
            MyCommon.EVENTTYPE myevt = MyCommon.EVENTTYPE.None;

            foreach (EventCheckboxTblElement tbl in GetEventCheckboxTable())
            {
                switch (tbl.CheckBox.CheckState)
                {
                    case CheckState.Checked:
                        evt = evt | tbl.Type;
                        myevt = myevt | tbl.Type;
                        break;
                    case CheckState.Indeterminate:
                        evt = evt | tbl.Type;
                        break;
                    case CheckState.Unchecked:
                        break;
                }
            }
            eventnotifyflag = evt;
            isMyeventnotifyflag = myevt;
        }

        private void ApplyEventNotifyFlag(bool rootEnabled, MyCommon.EVENTTYPE eventnotifyflag, MyCommon.EVENTTYPE isMyeventnotifyflag)
        {
            MyCommon.EVENTTYPE evt = eventnotifyflag;
            MyCommon.EVENTTYPE myevt = isMyeventnotifyflag;

            this.NotifyPanel.CheckEventNotify.Checked = rootEnabled;

            foreach (EventCheckboxTblElement tbl in GetEventCheckboxTable())
            {
                if ((evt & tbl.Type) != 0)
                {
                    if ((myevt & tbl.Type) != 0)
                    {
                        tbl.CheckBox.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        tbl.CheckBox.CheckState = CheckState.Indeterminate;
                    }
                }
                else
                {
                    tbl.CheckBox.CheckState = CheckState.Unchecked;
                }
                tbl.CheckBox.Enabled = rootEnabled;
            }

        }

        private void CheckEventNotify_CheckedChanged(object sender, EventArgs e)
        {
            foreach (EventCheckboxTblElement tbl in GetEventCheckboxTable())
            {
                tbl.CheckBox.Enabled = this.NotifyPanel.CheckEventNotify.Checked;
            }
        }

        //private void CheckForceEventNotify_CheckedChanged(object sender, EventArgs e)
        //{
        //    _MyForceEventNotify = CheckEventNotify.Checked;
        //}

        //private void CheckFavEventUnread_CheckedChanged(object sender, EventArgs e)
        //{
        //    _MyFavEventUnread = CheckFavEventUnread.Checked;
        //}

        //private void ComboBoxTranslateLanguage_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    _MyTranslateLanguage = (new Google()).GetLanguageEnumFromIndex(ComboBoxTranslateLanguage.SelectedIndex);
        //}

        private void SoundFileListup()
        {
            if (EventSoundFile == null) EventSoundFile = "";
            this.NotifyPanel.ComboBoxEventNotifySound.Items.Clear();
            this.NotifyPanel.ComboBoxEventNotifySound.Items.Add("");
            DirectoryInfo oDir = new DirectoryInfo(Application.StartupPath + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(Application.StartupPath, "Sounds")))
            {
                oDir = oDir.GetDirectories("Sounds")[0];
            }
            foreach (FileInfo oFile in oDir.GetFiles("*.wav"))
            {
                this.NotifyPanel.ComboBoxEventNotifySound.Items.Add(oFile.Name);
            }
            int idx = this.NotifyPanel.ComboBoxEventNotifySound.Items.IndexOf(EventSoundFile);
            if (idx == -1) idx = 0;
            this.NotifyPanel.ComboBoxEventNotifySound.SelectedIndex = idx;
        }

        //private void ComboBoxEventNotifySound_VisibleChanged(object sender, EventArgs e)
        //{
        //    SoundFileListup();
        //}

        //private void ComboBoxEventNotifySound_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //   if (_soundfileListup) return;

        //    _MyEventSoundFile = (string)ComboBoxEventNotifySound.SelectedItem;
        //}

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
//              MessageBox.Show("ブラウザの起動に失敗、またはタイムアウトしました。" + ex.ToString());
            }
        }

        private void CreateAccountButton_Click(object sender, EventArgs e)
        {
            this.OpenUrl("https://twitter.com/signup");
        }

        public AppendSettingDialog()
        {
            InitializeComponent();

            this.BasedPanel.StartAuthButton.Click += this.StartAuthButton_Click;
            this.BasedPanel.CreateAccountButton.Click += this.CreateAccountButton_Click;
            this.NotifyPanel.CheckEventNotify.CheckedChanged += this.CheckEventNotify_CheckedChanged;
            this.GetPeriodPanel.CheckPostAndGet.CheckedChanged += this.CheckPostAndGet_CheckedChanged;
            this.ActionPanel.UReadMng.CheckedChanged += this.UReadMng_CheckedChanged;
            this.FontPanel.btnUnread.Click += this.btnFontAndColor_Click;
            this.FontPanel.btnDetail.Click += this.btnFontAndColor_Click;
            this.FontPanel.btnListFont.Click += this.btnFontAndColor_Click;
            this.FontPanel.btnFav.Click += this.btnColor_Click;
            this.FontPanel.btnOWL.Click += this.btnColor_Click;
            this.FontPanel.btnRetweet.Click += this.btnColor_Click;
            this.FontPanel.btnDetailBack.Click += this.btnColor_Click;
            this.FontPanel.btnDetailLink.Click += this.btnColor_Click;
            this.FontPanel2.btnInputFont.Click += this.btnFontAndColor_Click;
            this.FontPanel2.btnSelf.Click += this.btnColor_Click;
            this.FontPanel2.btnAtSelf.Click += this.btnColor_Click;
            this.FontPanel2.btnTarget.Click += this.btnColor_Click;
            this.FontPanel2.btnAtTarget.Click += this.btnColor_Click;
            this.FontPanel2.btnAtFromTarget.Click += this.btnColor_Click;
            this.FontPanel2.btnInputBackcolor.Click += this.btnColor_Click;
            this.FontPanel2.btnAtTo.Click += this.btnColor_Click;
            this.FontPanel2.btnListBack.Click += this.btnColor_Click;

            this.Icon = Properties.Resources.MIcon;
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
    }
}
