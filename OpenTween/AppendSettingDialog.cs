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
using OpenTween.Thumbnail;
using System.Threading.Tasks;
using OpenTween.Setting.Panel;

namespace OpenTween
{
    public partial class AppendSettingDialog : OTBaseForm
    {
        private static AppendSettingDialog _instance = new AppendSettingDialog();
        private Twitter tw;
        private HttpConnection.ProxyType _MyProxyType;

        private bool _ValidationError = false;
        private MyCommon.EVENTTYPE _MyEventNotifyFlag;
        private MyCommon.EVENTTYPE _isMyEventNotifyFlag;
        private string _MyTranslateLanguage;

        public bool HideDuplicatedRetweets;

        public bool IsPreviewFoursquare;
        public MapProvider MapThumbnailProvider;
        public int MapThumbnailHeight;
        public int MapThumbnailWidth;
        public int MapThumbnailZoom;
        public bool IsListStatusesIncludeRts;
        public List<UserAccount> UserAccounts;
        private long? InitialUserId;
        public bool TabMouseLock;
        public bool IsRemoveSameEvent;
        public bool IsNotifyUseGrowl;

        public TwitterDataModel.Configuration TwitterConfiguration = new TwitterDataModel.Configuration();

        private string _pin;

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

        public delegate void IntervalChangedEventHandler(object sender, IntervalChangedEventArgs e);
        public event IntervalChangedEventHandler IntervalChanged;

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
            IntervalChangedEventArgs arg = new IntervalChangedEventArgs();
            bool isIntervalChanged = false;

            try
            {
                UserstreamStartup = this.GetPeriodPanel.StartupUserstreamCheck.Checked;

                if (UserstreamPeriodInt != int.Parse(this.GetPeriodPanel.UserstreamPeriod.Text))
                {
                    UserstreamPeriodInt = int.Parse(this.GetPeriodPanel.UserstreamPeriod.Text);
                    arg.UserStream = true;
                    isIntervalChanged = true;
                }
                if (TimelinePeriodInt != int.Parse(this.GetPeriodPanel.TimelinePeriod.Text))
                {
                    TimelinePeriodInt = int.Parse(this.GetPeriodPanel.TimelinePeriod.Text);
                    arg.Timeline = true;
                    isIntervalChanged = true;
                }
                if (DMPeriodInt != int.Parse(this.GetPeriodPanel.DMPeriod.Text))
                {
                    DMPeriodInt = int.Parse(this.GetPeriodPanel.DMPeriod.Text);
                    arg.DirectMessage = true;
                    isIntervalChanged = true;
                }
                if (PubSearchPeriodInt != int.Parse(this.GetPeriodPanel.PubSearchPeriod.Text))
                {
                    PubSearchPeriodInt = int.Parse(this.GetPeriodPanel.PubSearchPeriod.Text);
                    arg.PublicSearch = true;
                    isIntervalChanged = true;
                }

                if (ListsPeriodInt != int.Parse(this.GetPeriodPanel.ListsPeriod.Text))
                {
                    ListsPeriodInt = int.Parse(this.GetPeriodPanel.ListsPeriod.Text);
                    arg.Lists = true;
                    isIntervalChanged = true;
                }
                if (ReplyPeriodInt != int.Parse(this.GetPeriodPanel.ReplyPeriod.Text))
                {
                    ReplyPeriodInt = int.Parse(this.GetPeriodPanel.ReplyPeriod.Text);
                    arg.Reply = true;
                    isIntervalChanged = true;
                }
                if (UserTimelinePeriodInt != int.Parse(this.GetPeriodPanel.UserTimelinePeriod.Text))
                {
                    UserTimelinePeriodInt = int.Parse(this.GetPeriodPanel.UserTimelinePeriod.Text);
                    arg.UserTimeline = true;
                    isIntervalChanged = true;
                }

                if (isIntervalChanged && IntervalChanged != null)
                {
                    IntervalChanged(this, arg);
                }

                Readed = this.StartupPanel.StartupReaded.Checked;
                switch (this.TweetPrvPanel.IconSize.SelectedIndex)
                {
                    case 0:
                        IconSz = MyCommon.IconSizes.IconNone;
                        break;
                    case 1:
                        IconSz = MyCommon.IconSizes.Icon16;
                        break;
                    case 2:
                        IconSz = MyCommon.IconSizes.Icon24;
                        break;
                    case 3:
                        IconSz = MyCommon.IconSizes.Icon48;
                        break;
                    case 4:
                        IconSz = MyCommon.IconSizes.Icon48_2;
                        break;
                }
                Status = this.TweetActPanel.StatusText.Text;
                PlaySound = this.ActionPanel.PlaySnd.Checked;
                UnreadManage = this.ActionPanel.UReadMng.Checked;
                OneWayLove = this.TweetPrvPanel.OneWayLv.Checked;

                FontUnread = this.FontPanel.lblUnread.Font;     //未使用
                ColorUnread = this.FontPanel.lblUnread.ForeColor;
                FontReaded = this.FontPanel.lblListFont.Font;     //リストフォントとして使用
                ColorReaded = this.FontPanel.lblListFont.ForeColor;
                ColorFav = this.FontPanel.lblFav.ForeColor;
                ColorOWL = this.FontPanel.lblOWL.ForeColor;
                ColorRetweet = this.FontPanel.lblRetweet.ForeColor;
                FontDetail = this.FontPanel.lblDetail.Font;
                ColorSelf = this.FontPanel2.lblSelf.BackColor;
                ColorAtSelf = this.FontPanel2.lblAtSelf.BackColor;
                ColorTarget = this.FontPanel2.lblTarget.BackColor;
                ColorAtTarget = this.FontPanel2.lblAtTarget.BackColor;
                ColorAtFromTarget = this.FontPanel2.lblAtFromTarget.BackColor;
                ColorAtTo = this.FontPanel2.lblAtTo.BackColor;
                ColorInputBackcolor = this.FontPanel2.lblInputBackcolor.BackColor;
                ColorInputFont = this.FontPanel2.lblInputFont.ForeColor;
                ColorListBackcolor = this.FontPanel2.lblListBackcolor.BackColor;
                ColorDetailBackcolor = this.FontPanel.lblDetailBackcolor.BackColor;
                ColorDetail = this.FontPanel.lblDetail.ForeColor;
                ColorDetailLink = this.FontPanel.lblDetailLink.ForeColor;
                FontInputFont = this.FontPanel2.lblInputFont.Font;
                switch (this.PreviewPanel.cmbNameBalloon.SelectedIndex)
                {
                    case 0:
                        NameBalloon = MyCommon.NameBalloonEnum.None;
                        break;
                    case 1:
                        NameBalloon = MyCommon.NameBalloonEnum.UserID;
                        break;
                    case 2:
                        NameBalloon = MyCommon.NameBalloonEnum.NickName;
                        break;
                }

                switch (this.TweetActPanel.ComboBoxPostKeySelect.SelectedIndex)
                {
                    case 2:
                        PostShiftEnter = true;
                        PostCtrlEnter = false;
                        break;
                    case 1:
                        PostCtrlEnter = true;
                        PostShiftEnter = false;
                        break;
                    case 0:
                        PostCtrlEnter = false;
                        PostShiftEnter = false;
                        break;
                }
                CountApi = int.Parse(this.GetCountPanel.TextCountApi.Text);
                CountApiReply = int.Parse(this.GetCountPanel.TextCountApiReply.Text);
                BrowserPath = this.ActionPanel.BrowserPathText.Text.Trim();
                PostAndGet = this.GetPeriodPanel.CheckPostAndGet.Checked;
                UseRecommendStatus = this.TweetActPanel.CheckUseRecommendStatus.Checked;
                DispUsername = this.PreviewPanel.CheckDispUsername.Checked;
                CloseToExit = this.ActionPanel.CheckCloseToExit.Checked;
                MinimizeToTray = this.ActionPanel.CheckMinimizeToTray.Checked;
                switch (this.PreviewPanel.ComboDispTitle.SelectedIndex)
                {
                    case 0:  //None
                        DispLatestPost = MyCommon.DispTitleEnum.None;
                        break;
                    case 1:  //Ver
                        DispLatestPost = MyCommon.DispTitleEnum.Ver;
                        break;
                    case 2:  //Post
                        DispLatestPost = MyCommon.DispTitleEnum.Post;
                        break;
                    case 3:  //RepCount
                        DispLatestPost = MyCommon.DispTitleEnum.UnreadRepCount;
                        break;
                    case 4:  //AllCount
                        DispLatestPost = MyCommon.DispTitleEnum.UnreadAllCount;
                        break;
                    case 5:  //Rep+All
                        DispLatestPost = MyCommon.DispTitleEnum.UnreadAllRepCount;
                        break;
                    case 6:  //Unread/All
                        DispLatestPost = MyCommon.DispTitleEnum.UnreadCountAllCount;
                        break;
                    case 7: //Count of Status/Follow/Follower
                        DispLatestPost = MyCommon.DispTitleEnum.OwnStatus;
                        break;
                }
                SortOrderLock = this.TweetPrvPanel.CheckSortOrderLock.Checked;
                ViewTabBottom = this.TweetPrvPanel.CheckViewTabBottom.Checked;
                TinyUrlResolve = this.ShortUrlPanel.CheckTinyURL.Checked;
                ShortUrl.IsResolve = TinyUrlResolve;
                if (this.ProxyPanel.RadioProxyNone.Checked)
                {
                    _MyProxyType = HttpConnection.ProxyType.None;
                }
                else if (this.ProxyPanel.RadioProxyIE.Checked)
                {
                    _MyProxyType = HttpConnection.ProxyType.IE;
                }
                else
                {
                    _MyProxyType = HttpConnection.ProxyType.Specified;
                }
                ProxyAddress = this.ProxyPanel.TextProxyAddress.Text.Trim();
                ProxyPort = int.Parse(this.ProxyPanel.TextProxyPort.Text.Trim());
                ProxyUser = this.ProxyPanel.TextProxyUser.Text.Trim();
                ProxyPassword = this.ProxyPanel.TextProxyPassword.Text.Trim();
                StartupVersion = this.StartupPanel.CheckStartupVersion.Checked;
                StartupFollowers = this.StartupPanel.CheckStartupFollowers.Checked;
                RestrictFavCheck = this.ActionPanel.CheckFavRestrict.Checked;
                AlwaysTop = this.PreviewPanel.CheckAlwaysTop.Checked;
                UrlConvertAuto = this.ShortUrlPanel.CheckAutoConvertUrl.Checked;
                ShortenTco = this.ShortUrlPanel.ShortenTcoCheck.Checked;

                Nicoms = this.CooperatePanel.CheckNicoms.Checked;
                UseUnreadStyle = this.TweetPrvPanel.chkUnreadStyle.Checked;
                DateTimeFormat = this.TweetPrvPanel.CmbDateTimeFormat.Text;
                DefaultTimeOut = int.Parse(this.ConnectionPanel.ConnectionTimeOut.Text);
                RetweetNoConfirm = this.TweetActPanel.CheckRetweetNoConfirm.Checked;
                LimitBalloon = this.PreviewPanel.CheckBalloonLimit.Checked;
                EventNotifyEnabled = this.NotifyPanel.CheckEventNotify.Checked;
                GetEventNotifyFlag(ref _MyEventNotifyFlag, ref _isMyEventNotifyFlag);
                ForceEventNotify = this.NotifyPanel.CheckForceEventNotify.Checked;
                FavEventUnread = this.NotifyPanel.CheckFavEventUnread.Checked;
                TranslateLanguage = (new Bing()).GetLanguageEnumFromIndex(this.CooperatePanel.ComboBoxTranslateLanguage.SelectedIndex);
                EventSoundFile = (string)this.NotifyPanel.ComboBoxEventNotifySound.SelectedItem;
                AutoShortUrlFirst = (MyCommon.UrlConverter)this.ShortUrlPanel.ComboBoxAutoShortUrlFirst.SelectedIndex;
                TabIconDisp = this.PreviewPanel.chkTabIconDisp.Checked;
                ReadOwnPost = this.ActionPanel.chkReadOwnPost.Checked;
                GetFav = this.StartupPanel.chkGetFav.Checked;
                IsMonospace = this.PreviewPanel.CheckMonospace.Checked;
                ReadOldPosts = this.ActionPanel.CheckReadOldPosts.Checked;
                UseSsl = this.ConnectionPanel.CheckUseSsl.Checked;
                BitlyUser = this.ShortUrlPanel.TextBitlyId.Text;
                BitlyPwd = this.ShortUrlPanel.TextBitlyPw.Text;
                ShowGrid = this.TweetPrvPanel.CheckShowGrid.Checked;
                UseAtIdSupplement = this.TweetActPanel.CheckAtIdSupple.Checked;
                UseHashSupplement = this.TweetActPanel.CheckHashSupple.Checked;
                PreviewEnable = this.PreviewPanel.CheckPreviewEnable.Checked;
                TwitterApiUrl = this.ConnectionPanel.TwitterAPIText.Text.Trim();
                switch (this.PreviewPanel.ReplyIconStateCombo.SelectedIndex)
                {
                    case 0:
                        ReplyIconState = MyCommon.REPLY_ICONSTATE.None;
                        break;
                    case 1:
                        ReplyIconState = MyCommon.REPLY_ICONSTATE.StaticIcon;
                        break;
                    case 2:
                        ReplyIconState = MyCommon.REPLY_ICONSTATE.BlinkIcon;
                        break;
                }
                switch (this.PreviewPanel.LanguageCombo.SelectedIndex)
                {
                    case 0:
                        Language = "OS";
                        break;
                    case 1:
                        Language = "ja";
                        break;
                    case 2:
                        Language = "en";
                        break;
                    case 3:
                        Language = "zh-CN";
                        break;
                    default:
                        Language = "en";
                        break;
                }
                HotkeyEnabled = this.ActionPanel.HotkeyCheck.Checked;
                HotkeyMod = Keys.None;
                if (this.ActionPanel.HotkeyAlt.Checked) HotkeyMod = HotkeyMod | Keys.Alt;
                if (this.ActionPanel.HotkeyShift.Checked) HotkeyMod = HotkeyMod | Keys.Shift;
                if (this.ActionPanel.HotkeyCtrl.Checked) HotkeyMod = HotkeyMod | Keys.Control;
                if (this.ActionPanel.HotkeyWin.Checked) HotkeyMod = HotkeyMod | Keys.LWin;
                int.TryParse(this.ActionPanel.HotkeyCode.Text, out HotkeyValue);
                HotkeyKey = (Keys)this.ActionPanel.HotkeyText.Tag;
                BlinkNewMentions = this.PreviewPanel.ChkNewMentionsBlink.Checked;
                UseAdditionalCount = this.GetCountPanel.UseChangeGetCount.Checked;
                MoreCountApi = int.Parse(this.GetCountPanel.GetMoreTextCountApi.Text);
                FirstCountApi = int.Parse(this.GetCountPanel.FirstTextCountApi.Text);
                SearchCountApi = int.Parse(this.GetCountPanel.SearchTextCountApi.Text);
                FavoritesCountApi = int.Parse(this.GetCountPanel.FavoritesTextCountApi.Text);
                UserTimelineCountApi = int.Parse(this.GetCountPanel.UserTimelineTextCountApi.Text);
                ListCountApi = int.Parse(this.GetCountPanel.ListTextCountApi.Text);
                OpenUserTimeline = this.ActionPanel.CheckOpenUserTimeline.Checked;
                ListDoubleClickAction = this.ActionPanel.ListDoubleClickActionComboBox.SelectedIndex;
                UserAppointUrl = this.CooperatePanel.UserAppointUrlText.Text;
                this.HideDuplicatedRetweets = this.TweetPrvPanel.HideDuplicatedRetweetsCheck.Checked;
                this.IsPreviewFoursquare = this.CooperatePanel.IsPreviewFoursquareCheckBox.Checked;
                this.MapThumbnailProvider = (MapProvider)this.CooperatePanel.MapThumbnailProviderComboBox.SelectedIndex;
                this.MapThumbnailHeight = int.Parse(this.CooperatePanel.MapThumbnailHeightTextBox.Text);
                this.MapThumbnailWidth = int.Parse(this.CooperatePanel.MapThumbnailWidthTextBox.Text);
                this.MapThumbnailZoom = int.Parse(this.CooperatePanel.MapThumbnailZoomTextBox.Text);
                this.IsListStatusesIncludeRts = this.TweetPrvPanel.IsListsIncludeRtsCheckBox.Checked;
                this.TabMouseLock = this.ActionPanel.TabMouseLockCheck.Checked;
                this.IsRemoveSameEvent = this.NotifyPanel.IsRemoveSameFavEventCheckBox.Checked;
                this.IsNotifyUseGrowl = this.PreviewPanel.IsNotifyUseGrowlCheckBox.Checked;
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
            string uname = tw.Username;
            string pw = tw.Password;
            string tk = tw.AccessToken;
            string tks = tw.AccessTokenSecret;
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

            this.GetPeriodPanel.StartupUserstreamCheck.Checked = UserstreamStartup;
            this.GetPeriodPanel.UserstreamPeriod.Text = UserstreamPeriodInt.ToString();
            this.GetPeriodPanel.TimelinePeriod.Text = TimelinePeriodInt.ToString();
            this.GetPeriodPanel.ReplyPeriod.Text = ReplyPeriodInt.ToString();
            this.GetPeriodPanel.DMPeriod.Text = DMPeriodInt.ToString();
            this.GetPeriodPanel.PubSearchPeriod.Text = PubSearchPeriodInt.ToString();
            this.GetPeriodPanel.ListsPeriod.Text = ListsPeriodInt.ToString();
            this.GetPeriodPanel.UserTimelinePeriod.Text = UserTimelinePeriodInt.ToString();

            this.StartupPanel.StartupReaded.Checked = Readed;
            switch (IconSz)
            {
                case MyCommon.IconSizes.IconNone:
                    this.TweetPrvPanel.IconSize.SelectedIndex = 0;
                    break;
                case MyCommon.IconSizes.Icon16:
                    this.TweetPrvPanel.IconSize.SelectedIndex = 1;
                    break;
                case MyCommon.IconSizes.Icon24:
                    this.TweetPrvPanel.IconSize.SelectedIndex = 2;
                    break;
                case MyCommon.IconSizes.Icon48:
                    this.TweetPrvPanel.IconSize.SelectedIndex = 3;
                    break;
                case MyCommon.IconSizes.Icon48_2:
                    this.TweetPrvPanel.IconSize.SelectedIndex = 4;
                    break;
            }
            this.TweetActPanel.StatusText.Text = Status;
            this.ActionPanel.UReadMng.Checked = UnreadManage;
            if (UnreadManage == false)
            {
                this.StartupPanel.StartupReaded.Enabled = false;
            }
            else
            {
                this.StartupPanel.StartupReaded.Enabled = true;
            }
            this.ActionPanel.PlaySnd.Checked = PlaySound;
            this.TweetPrvPanel.OneWayLv.Checked = OneWayLove;

            this.FontPanel.lblListFont.Font = FontReaded;
            this.FontPanel.lblUnread.Font = FontUnread;
            this.FontPanel.lblUnread.ForeColor = ColorUnread;
            this.FontPanel.lblListFont.ForeColor = ColorReaded;
            this.FontPanel.lblFav.ForeColor = ColorFav;
            this.FontPanel.lblOWL.ForeColor = ColorOWL;
            this.FontPanel.lblRetweet.ForeColor = ColorRetweet;
            this.FontPanel.lblDetail.Font = FontDetail;
            this.FontPanel2.lblSelf.BackColor = ColorSelf;
            this.FontPanel2.lblAtSelf.BackColor = ColorAtSelf;
            this.FontPanel2.lblTarget.BackColor = ColorTarget;
            this.FontPanel2.lblAtTarget.BackColor = ColorAtTarget;
            this.FontPanel2.lblAtFromTarget.BackColor = ColorAtFromTarget;
            this.FontPanel2.lblAtTo.BackColor = ColorAtTo;
            this.FontPanel2.lblInputBackcolor.BackColor = ColorInputBackcolor;
            this.FontPanel2.lblInputFont.ForeColor = ColorInputFont;
            this.FontPanel2.lblInputFont.Font = FontInputFont;
            this.FontPanel2.lblListBackcolor.BackColor = ColorListBackcolor;
            this.FontPanel.lblDetailBackcolor.BackColor = ColorDetailBackcolor;
            this.FontPanel.lblDetail.ForeColor = ColorDetail;
            this.FontPanel.lblDetailLink.ForeColor = ColorDetailLink;

            switch (NameBalloon)
            {
                case MyCommon.NameBalloonEnum.None:
                    this.PreviewPanel.cmbNameBalloon.SelectedIndex = 0;
                    break;
                case MyCommon.NameBalloonEnum.UserID:
                    this.PreviewPanel.cmbNameBalloon.SelectedIndex = 1;
                    break;
                case MyCommon.NameBalloonEnum.NickName:
                    this.PreviewPanel.cmbNameBalloon.SelectedIndex = 2;
                    break;
            }

            if (PostCtrlEnter)
            {
                this.TweetActPanel.ComboBoxPostKeySelect.SelectedIndex = 1;
            }
            else if (PostShiftEnter)
            {
                this.TweetActPanel.ComboBoxPostKeySelect.SelectedIndex = 2;
            }
            else
            {
                this.TweetActPanel.ComboBoxPostKeySelect.SelectedIndex = 0;
            }

            this.GetCountPanel.TextCountApi.Text = CountApi.ToString();
            this.GetCountPanel.TextCountApiReply.Text = CountApiReply.ToString();
            this.ActionPanel.BrowserPathText.Text = BrowserPath;
            this.GetPeriodPanel.CheckPostAndGet.Checked = PostAndGet;
            this.TweetActPanel.CheckUseRecommendStatus.Checked = UseRecommendStatus;
            this.PreviewPanel.CheckDispUsername.Checked = DispUsername;
            this.ActionPanel.CheckCloseToExit.Checked = CloseToExit;
            this.ActionPanel.CheckMinimizeToTray.Checked = MinimizeToTray;
            switch (DispLatestPost)
            {
                case MyCommon.DispTitleEnum.None:
                    this.PreviewPanel.ComboDispTitle.SelectedIndex = 0;
                    break;
                case MyCommon.DispTitleEnum.Ver:
                    this.PreviewPanel.ComboDispTitle.SelectedIndex = 1;
                    break;
                case MyCommon.DispTitleEnum.Post:
                    this.PreviewPanel.ComboDispTitle.SelectedIndex = 2;
                    break;
                case MyCommon.DispTitleEnum.UnreadRepCount:
                    this.PreviewPanel.ComboDispTitle.SelectedIndex = 3;
                    break;
                case MyCommon.DispTitleEnum.UnreadAllCount:
                    this.PreviewPanel.ComboDispTitle.SelectedIndex = 4;
                    break;
                case MyCommon.DispTitleEnum.UnreadAllRepCount:
                    this.PreviewPanel.ComboDispTitle.SelectedIndex = 5;
                    break;
                case MyCommon.DispTitleEnum.UnreadCountAllCount:
                    this.PreviewPanel.ComboDispTitle.SelectedIndex = 6;
                    break;
                case MyCommon.DispTitleEnum.OwnStatus:
                    this.PreviewPanel.ComboDispTitle.SelectedIndex = 7;
                    break;
            }
            this.TweetPrvPanel.CheckSortOrderLock.Checked = SortOrderLock;
            this.TweetPrvPanel.CheckViewTabBottom.Checked = ViewTabBottom;
            this.ShortUrlPanel.CheckTinyURL.Checked = TinyUrlResolve;
            switch (_MyProxyType)
            {
                case HttpConnection.ProxyType.None:
                    this.ProxyPanel.RadioProxyNone.Checked = true;
                    break;
                case HttpConnection.ProxyType.IE:
                    this.ProxyPanel.RadioProxyIE.Checked = true;
                    break;
                default:
                    this.ProxyPanel.RadioProxySpecified.Checked = true;
                    break;
            }
            bool chk = this.ProxyPanel.RadioProxySpecified.Checked;
            this.ProxyPanel.LabelProxyAddress.Enabled = chk;
            this.ProxyPanel.TextProxyAddress.Enabled = chk;
            this.ProxyPanel.LabelProxyPort.Enabled = chk;
            this.ProxyPanel.TextProxyPort.Enabled = chk;
            this.ProxyPanel.LabelProxyUser.Enabled = chk;
            this.ProxyPanel.TextProxyUser.Enabled = chk;
            this.ProxyPanel.LabelProxyPassword.Enabled = chk;
            this.ProxyPanel.TextProxyPassword.Enabled = chk;

            this.ProxyPanel.TextProxyAddress.Text = ProxyAddress;
            this.ProxyPanel.TextProxyPort.Text = ProxyPort.ToString();
            this.ProxyPanel.TextProxyUser.Text = ProxyUser;
            this.ProxyPanel.TextProxyPassword.Text = ProxyPassword;

            this.StartupPanel.CheckStartupVersion.Checked = StartupVersion;
            if (ApplicationSettings.VersionInfoUrl == null)
                this.StartupPanel.CheckStartupVersion.Enabled = false; // 更新チェック無効化
            this.StartupPanel.CheckStartupFollowers.Checked = StartupFollowers;
            this.ActionPanel.CheckFavRestrict.Checked = RestrictFavCheck;
            this.PreviewPanel.CheckAlwaysTop.Checked = AlwaysTop;
            this.ShortUrlPanel.CheckAutoConvertUrl.Checked = UrlConvertAuto;
            this.ShortUrlPanel.ShortenTcoCheck.Checked = ShortenTco;
            this.ShortUrlPanel.ShortenTcoCheck.Enabled = this.ShortUrlPanel.CheckAutoConvertUrl.Checked;

            this.CooperatePanel.CheckNicoms.Checked = Nicoms;
            this.TweetPrvPanel.chkUnreadStyle.Checked = UseUnreadStyle;
            this.TweetPrvPanel.CmbDateTimeFormat.Text = DateTimeFormat;
            this.ConnectionPanel.ConnectionTimeOut.Text = DefaultTimeOut.ToString();
            this.TweetActPanel.CheckRetweetNoConfirm.Checked = RetweetNoConfirm;
            this.PreviewPanel.CheckBalloonLimit.Checked = LimitBalloon;

            ApplyEventNotifyFlag(EventNotifyEnabled, EventNotifyFlag, IsMyEventNotifyFlag);
            this.NotifyPanel.CheckForceEventNotify.Checked = ForceEventNotify;
            this.NotifyPanel.CheckFavEventUnread.Checked = FavEventUnread;
            this.CooperatePanel.ComboBoxTranslateLanguage.SelectedIndex = (new Bing()).GetIndexFromLanguageEnum(TranslateLanguage);
            SoundFileListup();
            this.ShortUrlPanel.ComboBoxAutoShortUrlFirst.SelectedIndex = (int)AutoShortUrlFirst;
            this.PreviewPanel.chkTabIconDisp.Checked = TabIconDisp;
            this.ActionPanel.chkReadOwnPost.Checked = ReadOwnPost;
            this.StartupPanel.chkGetFav.Checked = GetFav;
            this.PreviewPanel.CheckMonospace.Checked = IsMonospace;
            this.ActionPanel.CheckReadOldPosts.Checked = ReadOldPosts;
            this.ConnectionPanel.CheckUseSsl.Checked = UseSsl;
            this.ShortUrlPanel.TextBitlyId.Text = BitlyUser;
            this.ShortUrlPanel.TextBitlyPw.Text = BitlyPwd;
            this.ShortUrlPanel.TextBitlyId.Modified = false;
            this.ShortUrlPanel.TextBitlyPw.Modified = false;
            this.TweetPrvPanel.CheckShowGrid.Checked = ShowGrid;
            this.TweetActPanel.CheckAtIdSupple.Checked = UseAtIdSupplement;
            this.TweetActPanel.CheckHashSupple.Checked = UseHashSupplement;
            this.PreviewPanel.CheckPreviewEnable.Checked = PreviewEnable;
            this.ConnectionPanel.TwitterAPIText.Text = TwitterApiUrl;
            switch (ReplyIconState)
            {
                case MyCommon.REPLY_ICONSTATE.None:
                    this.PreviewPanel.ReplyIconStateCombo.SelectedIndex = 0;
                    break;
                case MyCommon.REPLY_ICONSTATE.StaticIcon:
                    this.PreviewPanel.ReplyIconStateCombo.SelectedIndex = 1;
                    break;
                case MyCommon.REPLY_ICONSTATE.BlinkIcon:
                    this.PreviewPanel.ReplyIconStateCombo.SelectedIndex = 2;
                    break;
            }
            switch (Language)
            {
                case "OS":
                    this.PreviewPanel.LanguageCombo.SelectedIndex = 0;
                    break;
                case "ja":
                    this.PreviewPanel.LanguageCombo.SelectedIndex = 1;
                    break;
                case "en":
                    this.PreviewPanel.LanguageCombo.SelectedIndex = 2;
                    break;
                case "zh-CN":
                    this.PreviewPanel.LanguageCombo.SelectedIndex = 3;
                    break;
                default:
                    this.PreviewPanel.LanguageCombo.SelectedIndex = 0;
                    break;
            }
            this.ActionPanel.HotkeyCheck.Checked = HotkeyEnabled;
            this.ActionPanel.HotkeyAlt.Checked = ((HotkeyMod & Keys.Alt) == Keys.Alt);
            this.ActionPanel.HotkeyCtrl.Checked = ((HotkeyMod & Keys.Control) == Keys.Control);
            this.ActionPanel.HotkeyShift.Checked = ((HotkeyMod & Keys.Shift) == Keys.Shift);
            this.ActionPanel.HotkeyWin.Checked = ((HotkeyMod & Keys.LWin) == Keys.LWin);
            this.ActionPanel.HotkeyCode.Text = HotkeyValue.ToString();
            this.ActionPanel.HotkeyText.Text = HotkeyKey.ToString();
            this.ActionPanel.HotkeyText.Tag = HotkeyKey;
            this.ActionPanel.HotkeyAlt.Enabled = HotkeyEnabled;
            this.ActionPanel.HotkeyShift.Enabled = HotkeyEnabled;
            this.ActionPanel.HotkeyCtrl.Enabled = HotkeyEnabled;
            this.ActionPanel.HotkeyWin.Enabled = HotkeyEnabled;
            this.ActionPanel.HotkeyText.Enabled = HotkeyEnabled;
            this.ActionPanel.HotkeyCode.Enabled = HotkeyEnabled;
            this.PreviewPanel.ChkNewMentionsBlink.Checked = BlinkNewMentions;

            this.GetCountPanel.GetMoreTextCountApi.Text = MoreCountApi.ToString();
            this.GetCountPanel.FirstTextCountApi.Text = FirstCountApi.ToString();
            this.GetCountPanel.SearchTextCountApi.Text = SearchCountApi.ToString();
            this.GetCountPanel.FavoritesTextCountApi.Text = FavoritesCountApi.ToString();
            this.GetCountPanel.UserTimelineTextCountApi.Text = UserTimelineCountApi.ToString();
            this.GetCountPanel.ListTextCountApi.Text = ListCountApi.ToString();
            this.GetCountPanel.UseChangeGetCount.Checked = UseAdditionalCount;
            this.GetCountPanel.Label28.Enabled = this.GetCountPanel.UseChangeGetCount.Checked;
            this.GetCountPanel.Label30.Enabled = this.GetCountPanel.UseChangeGetCount.Checked;
            this.GetCountPanel.Label53.Enabled = this.GetCountPanel.UseChangeGetCount.Checked;
            this.GetCountPanel.Label66.Enabled = this.GetCountPanel.UseChangeGetCount.Checked;
            this.GetCountPanel.Label17.Enabled = this.GetCountPanel.UseChangeGetCount.Checked;
            this.GetCountPanel.Label25.Enabled = this.GetCountPanel.UseChangeGetCount.Checked;
            this.GetCountPanel.GetMoreTextCountApi.Enabled = this.GetCountPanel.UseChangeGetCount.Checked;
            this.GetCountPanel.FirstTextCountApi.Enabled = this.GetCountPanel.UseChangeGetCount.Checked;
            this.GetCountPanel.SearchTextCountApi.Enabled = this.GetCountPanel.UseChangeGetCount.Checked;
            this.GetCountPanel.FavoritesTextCountApi.Enabled = this.GetCountPanel.UseChangeGetCount.Checked;
            this.GetCountPanel.UserTimelineTextCountApi.Enabled = this.GetCountPanel.UseChangeGetCount.Checked;
            this.GetCountPanel.ListTextCountApi.Enabled = this.GetCountPanel.UseChangeGetCount.Checked;
            this.ActionPanel.CheckOpenUserTimeline.Checked = OpenUserTimeline;
            this.ActionPanel.ListDoubleClickActionComboBox.SelectedIndex = ListDoubleClickAction;
            this.CooperatePanel.UserAppointUrlText.Text = UserAppointUrl;
            this.TweetPrvPanel.HideDuplicatedRetweetsCheck.Checked = this.HideDuplicatedRetweets;
            this.CooperatePanel.IsPreviewFoursquareCheckBox.Checked = this.IsPreviewFoursquare;
            this.CooperatePanel.MapThumbnailProviderComboBox.SelectedIndex = (int)this.MapThumbnailProvider;
            this.CooperatePanel.MapThumbnailHeightTextBox.Text = this.MapThumbnailHeight.ToString();
            this.CooperatePanel.MapThumbnailWidthTextBox.Text = this.MapThumbnailWidth.ToString();
            this.CooperatePanel.MapThumbnailZoomTextBox.Text = this.MapThumbnailZoom.ToString();
            this.TweetPrvPanel.IsListsIncludeRtsCheckBox.Checked = this.IsListStatusesIncludeRts;
            this.ActionPanel.TabMouseLockCheck.Checked = this.TabMouseLock;
            this.NotifyPanel.IsRemoveSameFavEventCheckBox.Checked = this.IsRemoveSameEvent;
            this.PreviewPanel.IsNotifyUseGrowlCheckBox.Checked = this.IsNotifyUseGrowl;

            if (GrowlHelper.IsDllExists)
            {
                this.PreviewPanel.IsNotifyUseGrowlCheckBox.Enabled = true;
            }
            else
            {
                this.PreviewPanel.IsNotifyUseGrowlCheckBox.Enabled = false;
            }

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

        public int UserstreamPeriodInt { get; set; }
        public bool UserstreamStartup { get; set; }
        public int TimelinePeriodInt { get; set; }
        public int ReplyPeriodInt { get; set; }
        public int DMPeriodInt { get; set; }
        public int PubSearchPeriodInt { get; set; }
        public int ListsPeriodInt { get; set; }
        public int UserTimelinePeriodInt { get; set; }
        public bool Readed { get; set; }
        public MyCommon.IconSizes IconSz { get; set; }
        public string Status { get; set; }
        public bool UnreadManage { get; set; }
        public bool PlaySound { get; set; }
        public bool OneWayLove { get; set; }
        public Font FontUnread { get; set; } /////未使用
        public Color ColorUnread { get; set; }
        public Font FontReaded { get; set; } /////リストフォントとして使用
        public Color ColorReaded { get; set; }
        public Color ColorFav { get; set; }
        public Color ColorOWL { get; set; }
        public Color ColorRetweet { get; set; }
        public Font FontDetail { get; set; }
        public Color ColorDetail { get; set; }
        public Color ColorDetailLink { get; set; }
        public Color ColorSelf { get; set; }
        public Color ColorAtSelf { get; set; }
        public Color ColorTarget { get; set; }
        public Color ColorAtTarget { get; set; }
        public Color ColorAtFromTarget { get; set; }
        public Color ColorAtTo { get; set; }
        public Color ColorInputBackcolor { get; set; }
        public Color ColorInputFont { get; set; }
        public Font FontInputFont { get; set; }
        public Color ColorListBackcolor { get; set; }
        public Color ColorDetailBackcolor { get; set; }
        public MyCommon.NameBalloonEnum NameBalloon { get; set; }
        public bool PostCtrlEnter { get; set; }
        public bool PostShiftEnter { get; set; }
        public int CountApi { get; set; }
        public int CountApiReply { get; set; }
        public int MoreCountApi { get; set; }
        public int FirstCountApi { get; set; }
        public int SearchCountApi { get; set; }
        public int FavoritesCountApi { get; set; }
        public int UserTimelineCountApi { get; set; }
        public int ListCountApi { get; set; }
        public bool PostAndGet { get; set; }
        public bool UseRecommendStatus { get; set; }
        public string RecommendStatusText { get; set; }
        public bool DispUsername { get; set; }
        public bool CloseToExit { get; set; }
        public bool MinimizeToTray { get; set; }
        public MyCommon.DispTitleEnum DispLatestPost { get; set; }
        public string BrowserPath { get; set; }
        public bool TinyUrlResolve { get; set; }

        public bool SortOrderLock { get; set; }
        public HttpConnection.ProxyType SelectedProxyType
        {
            get {
                return _MyProxyType;
            }
            set {
                _MyProxyType = value;
            }
        }
        /// <summary>
        /// タブを下部に表示するかどうかを取得または設定する
        /// </summary>
        public bool ViewTabBottom { get; set; }

        public string ProxyAddress { get; set; }
        public int ProxyPort { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPassword { get; set; }
        public bool StartupVersion { get; set; }
        public bool StartupFollowers { get; set; }
        public bool RestrictFavCheck { get; set; }
        public bool AlwaysTop { get; set; }
        public bool UrlConvertAuto { get; set; }
        public bool ShortenTco { get; set; }
        public bool Nicoms { get; set; }
        public MyCommon.UrlConverter AutoShortUrlFirst { get; set; }
        public bool UseUnreadStyle { get; set; }
        public string DateTimeFormat { get; set; }
        public int DefaultTimeOut { get; set; }
        public bool RetweetNoConfirm { get; set; }
        public bool TabIconDisp { get; set; }
        public MyCommon.REPLY_ICONSTATE ReplyIconState { get; set; }
        public bool ReadOwnPost { get; set; }
        public bool GetFav { get; set; }
        public bool IsMonospace { get; set; }
        public bool ReadOldPosts { get; set; }
        public bool UseSsl { get; set; }
        public string BitlyUser { get; set; }
        public string BitlyPwd { get; set; }
        public bool ShowGrid { get; set; }
        public bool UseAtIdSupplement { get; set; }
        public bool UseHashSupplement { get; set; }
        public bool PreviewEnable { get; set; }
        public bool UseAdditionalCount { get; set; }
        public bool OpenUserTimeline { get; set; }
        public string TwitterApiUrl { get; set; }
        public string Language { get; set; }

        public bool LimitBalloon { get; set; }
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

        public string TranslateLanguage
        {
            get
            {
                return _MyTranslateLanguage;
            }
            set
            {
                _MyTranslateLanguage = value;
                this.CooperatePanel.ComboBoxTranslateLanguage.SelectedIndex = (new Bing()).GetIndexFromLanguageEnum(value);
            }
        }

        public string EventSoundFile { get; set; }
        public int ListDoubleClickAction { get; set; }
        public string UserAppointUrl { get; set; }

        private bool StartAuth()
        {
            //現在の設定内容で通信
            HttpConnection.ProxyType ptype;
            if (this.ProxyPanel.RadioProxyNone.Checked)
            {
                ptype = HttpConnection.ProxyType.None;
            }
            else if (this.ProxyPanel.RadioProxyIE.Checked)
            {
                ptype = HttpConnection.ProxyType.IE;
            }
            else
            {
                ptype = HttpConnection.ProxyType.Specified;
            }
            string padr = this.ProxyPanel.TextProxyAddress.Text.Trim();
            int pport = int.Parse(this.ProxyPanel.TextProxyPort.Text.Trim());
            string pusr = this.ProxyPanel.TextProxyUser.Text.Trim();
            string ppw = this.ProxyPanel.TextProxyPassword.Text.Trim();

            //通信基底クラス初期化
            HttpConnection.InitializeConnection(20, ptype, padr, pport, pusr, ppw);
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
            this.TopMost = this.AlwaysTop;

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

        public bool HotkeyEnabled;
        public Keys HotkeyKey;
        public int HotkeyValue;
        public Keys HotkeyMod;

        public bool BlinkNewMentions;

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
                if (!string.IsNullOrEmpty(BrowserPath))
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
    }
}
