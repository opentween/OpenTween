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

namespace OpenTween
{
    public partial class AppendSettingDialog : Form
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
        private long InitialUserId;
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
            Panel pnl = (Panel)this.TreeViewSetting.SelectedNode.Tag;
            if (pnl == null) return;
            pnl.Enabled = false;
            pnl.Visible = false;
        }

        private void TreeViewSetting_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null) return;
            Panel pnl = (Panel)e.Node.Tag;
            if (pnl == null) return;
            pnl.Enabled = true;
            pnl.Visible = true;

            if (pnl.Name == "PreviewPanel")
            {
                if (GrowlHelper.IsDllExists)
                {
                    IsNotifyUseGrowlCheckBox.Enabled = true;
                }
                else
                {
                    IsNotifyUseGrowlCheckBox.Enabled = false;
                }
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            if (MyCommon.IsNetworkAvailable() &&
                (ComboBoxAutoShortUrlFirst.SelectedIndex == (int)MyCommon.UrlConverter.Bitly || ComboBoxAutoShortUrlFirst.SelectedIndex == (int)MyCommon.UrlConverter.Jmp))
            {
                // bit.ly 短縮機能実装のプライバシー問題の暫定対応
                // bit.ly 使用時はログインIDとAPIキーの指定を必須とする
                // 参照: http://sourceforge.jp/projects/opentween/lists/archive/dev/2012-January/000020.html
                if (string.IsNullOrEmpty(TextBitlyId.Text) || string.IsNullOrEmpty(TextBitlyPw.Text))
                {
                    MessageBox.Show("bit.ly のログイン名とAPIキーの指定は必須項目です。", Application.ProductName);
                    _ValidationError = true;
                    TreeViewSetting.SelectedNode = TreeViewSetting.Nodes["ConnectionNode"].Nodes["ShortUrlNode"]; // 動作タブを選択
                    TreeViewSetting.Select();
                    TextBitlyId.Focus();
                    return;
                }

                if (!BitlyValidation(TextBitlyId.Text, TextBitlyPw.Text))
                {
                    MessageBox.Show(Properties.Resources.SettingSave_ClickText1);
                    _ValidationError = true;
                    TreeViewSetting.SelectedNode = TreeViewSetting.Nodes["ConnectionNode"].Nodes["ShortUrlNode"]; // 動作タブを選択
                    TreeViewSetting.Select();
                    TextBitlyId.Focus();
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
            foreach (object u in this.AuthUserCombo.Items)
            {
                this.UserAccounts.Add((UserAccount)u);
            }
            if (this.AuthUserCombo.SelectedIndex > -1)
            {
                foreach (UserAccount u in this.UserAccounts)
                {
                    if (u.Username.ToLower() == ((UserAccount)this.AuthUserCombo.SelectedItem).Username.ToLower())
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
                UserstreamStartup = this.StartupUserstreamCheck.Checked;

                if (UserstreamPeriodInt != int.Parse(UserstreamPeriod.Text))
                {
                    UserstreamPeriodInt = int.Parse(UserstreamPeriod.Text);
                    arg.UserStream = true;
                    isIntervalChanged = true;
                }
                if (TimelinePeriodInt != int.Parse(TimelinePeriod.Text))
                {
                    TimelinePeriodInt = int.Parse(TimelinePeriod.Text);
                    arg.Timeline = true;
                    isIntervalChanged = true;
                }
                if (DMPeriodInt != int.Parse(DMPeriod.Text))
                {
                    DMPeriodInt = int.Parse(DMPeriod.Text);
                    arg.DirectMessage = true;
                    isIntervalChanged = true;
                }
                if (PubSearchPeriodInt != int.Parse(PubSearchPeriod.Text))
                {
                    PubSearchPeriodInt = int.Parse(PubSearchPeriod.Text);
                    arg.PublicSearch = true;
                    isIntervalChanged = true;
                }

                if (ListsPeriodInt != int.Parse(ListsPeriod.Text))
                {
                    ListsPeriodInt = int.Parse(ListsPeriod.Text);
                    arg.Lists = true;
                    isIntervalChanged = true;
                }
                if (ReplyPeriodInt != int.Parse(ReplyPeriod.Text))
                {
                    ReplyPeriodInt = int.Parse(ReplyPeriod.Text);
                    arg.Reply = true;
                    isIntervalChanged = true;
                }
                if (UserTimelinePeriodInt != int.Parse(UserTimelinePeriod.Text))
                {
                    UserTimelinePeriodInt = int.Parse(UserTimelinePeriod.Text);
                    arg.UserTimeline = true;
                    isIntervalChanged = true;
                }

                if (isIntervalChanged && IntervalChanged != null)
                {
                    IntervalChanged(this, arg);
                }

                Readed = StartupReaded.Checked;
                switch (IconSize.SelectedIndex)
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
                Status = StatusText.Text;
                PlaySound = PlaySnd.Checked;
                UnreadManage = UReadMng.Checked;
                OneWayLove = OneWayLv.Checked;

                FontUnread = lblUnread.Font;     //未使用
                ColorUnread = lblUnread.ForeColor;
                FontReaded = lblListFont.Font;     //リストフォントとして使用
                ColorReaded = lblListFont.ForeColor;
                ColorFav = lblFav.ForeColor;
                ColorOWL = lblOWL.ForeColor;
                ColorRetweet = lblRetweet.ForeColor;
                FontDetail = lblDetail.Font;
                ColorSelf = lblSelf.BackColor;
                ColorAtSelf = lblAtSelf.BackColor;
                ColorTarget = lblTarget.BackColor;
                ColorAtTarget = lblAtTarget.BackColor;
                ColorAtFromTarget = lblAtFromTarget.BackColor;
                ColorAtTo = lblAtTo.BackColor;
                ColorInputBackcolor = lblInputBackcolor.BackColor;
                ColorInputFont = lblInputFont.ForeColor;
                ColorListBackcolor = lblListBackcolor.BackColor;
                ColorDetailBackcolor = lblDetailBackcolor.BackColor;
                ColorDetail = lblDetail.ForeColor;
                ColorDetailLink = lblDetailLink.ForeColor;
                FontInputFont = lblInputFont.Font;
                switch (cmbNameBalloon.SelectedIndex)
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

                switch (ComboBoxPostKeySelect.SelectedIndex)
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
                CountApi = int.Parse(TextCountApi.Text);
                CountApiReply = int.Parse(TextCountApiReply.Text);
                BrowserPath = BrowserPathText.Text.Trim();
                PostAndGet = CheckPostAndGet.Checked;
                UseRecommendStatus = CheckUseRecommendStatus.Checked;
                DispUsername = CheckDispUsername.Checked;
                CloseToExit = CheckCloseToExit.Checked;
                MinimizeToTray = CheckMinimizeToTray.Checked;
                switch (ComboDispTitle.SelectedIndex)
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
                SortOrderLock = CheckSortOrderLock.Checked;
                ViewTabBottom = CheckViewTabBottom.Checked;
                TinyUrlResolve = CheckTinyURL.Checked;
                ShortUrlForceResolve = CheckForceResolve.Checked;
                ShortUrl.IsResolve = TinyUrlResolve;
                ShortUrl.IsForceResolve = ShortUrlForceResolve;
                if (RadioProxyNone.Checked)
                {
                    _MyProxyType = HttpConnection.ProxyType.None;
                }
                else if (RadioProxyIE.Checked)
                {
                    _MyProxyType = HttpConnection.ProxyType.IE;
                }
                else
                {
                    _MyProxyType = HttpConnection.ProxyType.Specified;
                }
                ProxyAddress = TextProxyAddress.Text.Trim();
                ProxyPort = int.Parse(TextProxyPort.Text.Trim());
                ProxyUser = TextProxyUser.Text.Trim();
                ProxyPassword = TextProxyPassword.Text.Trim();
                PeriodAdjust = CheckPeriodAdjust.Checked;
                StartupVersion = CheckStartupVersion.Checked;
                StartupFollowers = CheckStartupFollowers.Checked;
                RestrictFavCheck = CheckFavRestrict.Checked;
                AlwaysTop = CheckAlwaysTop.Checked;
                UrlConvertAuto = CheckAutoConvertUrl.Checked;
                ShortenTco = ShortenTcoCheck.Checked;
                OutputzEnabled = CheckOutputz.Checked;
                OutputzKey = TextBoxOutputzKey.Text.Trim();

                switch (ComboBoxOutputzUrlmode.SelectedIndex)
                {
                    case 0:
                        OutputzUrlmode = MyCommon.OutputzUrlmode.twittercom;
                        break;
                    case 1:
                        OutputzUrlmode = MyCommon.OutputzUrlmode.twittercomWithUsername;
                        break;
                }

                Nicoms = CheckNicoms.Checked;
                UseUnreadStyle = chkUnreadStyle.Checked;
                DateTimeFormat = CmbDateTimeFormat.Text;
                DefaultTimeOut = int.Parse(ConnectionTimeOut.Text);
                RetweetNoConfirm = CheckRetweetNoConfirm.Checked;
                LimitBalloon = CheckBalloonLimit.Checked;
                EventNotifyEnabled = CheckEventNotify.Checked;
                GetEventNotifyFlag(ref _MyEventNotifyFlag, ref _isMyEventNotifyFlag);
                ForceEventNotify = CheckForceEventNotify.Checked;
                FavEventUnread = CheckFavEventUnread.Checked;
                TranslateLanguage = (new Bing()).GetLanguageEnumFromIndex(ComboBoxTranslateLanguage.SelectedIndex);
                EventSoundFile = (string)ComboBoxEventNotifySound.SelectedItem;
                AutoShortUrlFirst = (MyCommon.UrlConverter)ComboBoxAutoShortUrlFirst.SelectedIndex;
                TabIconDisp = chkTabIconDisp.Checked;
                ReadOwnPost = chkReadOwnPost.Checked;
                GetFav = chkGetFav.Checked;
                IsMonospace = CheckMonospace.Checked;
                ReadOldPosts = CheckReadOldPosts.Checked;
                UseSsl = CheckUseSsl.Checked;
                BitlyUser = TextBitlyId.Text;
                BitlyPwd = TextBitlyPw.Text;
                ShowGrid = CheckShowGrid.Checked;
                UseAtIdSupplement = CheckAtIdSupple.Checked;
                UseHashSupplement = CheckHashSupple.Checked;
                PreviewEnable = CheckPreviewEnable.Checked;
                TwitterApiUrl = TwitterAPIText.Text.Trim();
                TwitterSearchApiUrl = TwitterSearchAPIText.Text.Trim();
                switch (ReplyIconStateCombo.SelectedIndex)
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
                switch (LanguageCombo.SelectedIndex)
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
                HotkeyEnabled = this.HotkeyCheck.Checked;
                HotkeyMod = Keys.None;
                if (this.HotkeyAlt.Checked) HotkeyMod = HotkeyMod | Keys.Alt;
                if (this.HotkeyShift.Checked) HotkeyMod = HotkeyMod | Keys.Shift;
                if (this.HotkeyCtrl.Checked) HotkeyMod = HotkeyMod | Keys.Control;
                if (this.HotkeyWin.Checked) HotkeyMod = HotkeyMod | Keys.LWin;
                int.TryParse(HotkeyCode.Text, out HotkeyValue);
                HotkeyKey = (Keys)HotkeyText.Tag;
                BlinkNewMentions = ChkNewMentionsBlink.Checked;
                UseAdditionalCount = UseChangeGetCount.Checked;
                MoreCountApi = int.Parse(GetMoreTextCountApi.Text);
                FirstCountApi = int.Parse(FirstTextCountApi.Text);
                SearchCountApi = int.Parse(SearchTextCountApi.Text);
                FavoritesCountApi = int.Parse(FavoritesTextCountApi.Text);
                UserTimelineCountApi = int.Parse(UserTimelineTextCountApi.Text);
                ListCountApi = int.Parse(ListTextCountApi.Text);
                OpenUserTimeline = CheckOpenUserTimeline.Checked;
                ListDoubleClickAction = ListDoubleClickActionComboBox.SelectedIndex;
                UserAppointUrl = UserAppointUrlText.Text;
                this.HideDuplicatedRetweets = this.HideDuplicatedRetweetsCheck.Checked;
                this.IsPreviewFoursquare = this.IsPreviewFoursquareCheckBox.Checked;
                this.MapThumbnailProvider = (MapProvider)this.MapThumbnailProviderComboBox.SelectedIndex;
                this.MapThumbnailHeight = int.Parse(this.MapThumbnailHeightTextBox.Text);
                this.MapThumbnailWidth = int.Parse(this.MapThumbnailWidthTextBox.Text);
                this.MapThumbnailZoom = int.Parse(this.MapThumbnailZoomTextBox.Text);
                this.IsListStatusesIncludeRts = this.IsListsIncludeRtsCheckBox.Checked;
                this.TabMouseLock = this.TabMouseLockCheck.Checked;
                this.IsRemoveSameEvent = this.IsRemoveSameFavEventCheckBox.Checked;
                this.IsNotifyUseGrowl = this.IsNotifyUseGrowlCheckBox.Checked;
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
                foreach (object u in this.AuthUserCombo.Items)
                {
                    this.UserAccounts.Add((UserAccount)u);
                }
                //アクティブユーザーを起動時のアカウントに戻す（起動時アカウントなければ何もしない）
                bool userSet = false;
                if (this.InitialUserId > 0)
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
                Panel curPanel = (Panel)TreeViewSetting.SelectedNode.Tag;
                curPanel.Visible = false;
                curPanel.Enabled = false;
            }
        }

        private void Setting_Load(object sender, EventArgs e)
        {
#if UA
            this.FollowCheckBox.Text = string.Format(this.FollowCheckBox.Text, ApplicationSettings.FeedbackTwitterName);
            this.GroupBox2.Visible = true;
#else
            this.GroupBox2.Visible = false;
#endif
            tw = ((TweenMain)this.Owner).TwitterInstance;
            string uname = tw.Username;
            string pw = tw.Password;
            string tk = tw.AccessToken;
            string tks = tw.AccessTokenSecret;
            //this.AuthStateLabel.Enabled = true;
            //this.AuthUserLabel.Enabled = true;
            this.AuthClearButton.Enabled = true;

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

            this.AuthUserCombo.Items.Clear();
            if (this.UserAccounts.Count > 0)
            {
                this.AuthUserCombo.Items.AddRange(this.UserAccounts.ToArray());
                foreach (UserAccount u in this.UserAccounts)
                {
                    if (u.UserId == tw.UserId)
                    {
                        this.AuthUserCombo.SelectedItem = u;
                        this.InitialUserId = u.UserId;
                        break;
                    }
                }
            }

            this.StartupUserstreamCheck.Checked = UserstreamStartup;
            UserstreamPeriod.Text = UserstreamPeriodInt.ToString();
            TimelinePeriod.Text = TimelinePeriodInt.ToString();
            ReplyPeriod.Text = ReplyPeriodInt.ToString();
            DMPeriod.Text = DMPeriodInt.ToString();
            PubSearchPeriod.Text = PubSearchPeriodInt.ToString();
            ListsPeriod.Text = ListsPeriodInt.ToString();
            UserTimelinePeriod.Text = UserTimelinePeriodInt.ToString();

            StartupReaded.Checked = Readed;
            switch (IconSz)
            {
                case MyCommon.IconSizes.IconNone:
                    IconSize.SelectedIndex = 0;
                    break;
                case MyCommon.IconSizes.Icon16:
                    IconSize.SelectedIndex = 1;
                    break;
                case MyCommon.IconSizes.Icon24:
                    IconSize.SelectedIndex = 2;
                    break;
                case MyCommon.IconSizes.Icon48:
                    IconSize.SelectedIndex = 3;
                    break;
                case MyCommon.IconSizes.Icon48_2:
                    IconSize.SelectedIndex = 4;
                    break;
            }
            StatusText.Text = Status;
            UReadMng.Checked = UnreadManage;
            if (UnreadManage == false)
            {
                StartupReaded.Enabled = false;
            }
            else
            {
                StartupReaded.Enabled = true;
            }
            PlaySnd.Checked = PlaySound;
            OneWayLv.Checked = OneWayLove;

            lblListFont.Font = FontReaded;
            lblUnread.Font = FontUnread;
            lblUnread.ForeColor = ColorUnread;
            lblListFont.ForeColor = ColorReaded;
            lblFav.ForeColor = ColorFav;
            lblOWL.ForeColor = ColorOWL;
            lblRetweet.ForeColor = ColorRetweet;
            lblDetail.Font = FontDetail;
            lblSelf.BackColor = ColorSelf;
            lblAtSelf.BackColor = ColorAtSelf;
            lblTarget.BackColor = ColorTarget;
            lblAtTarget.BackColor = ColorAtTarget;
            lblAtFromTarget.BackColor = ColorAtFromTarget;
            lblAtTo.BackColor = ColorAtTo;
            lblInputBackcolor.BackColor = ColorInputBackcolor;
            lblInputFont.ForeColor = ColorInputFont;
            lblInputFont.Font = FontInputFont;
            lblListBackcolor.BackColor = ColorListBackcolor;
            lblDetailBackcolor.BackColor = ColorDetailBackcolor;
            lblDetail.ForeColor = ColorDetail;
            lblDetailLink.ForeColor = ColorDetailLink;

            switch (NameBalloon)
            {
                case MyCommon.NameBalloonEnum.None:
                    cmbNameBalloon.SelectedIndex = 0;
                    break;
                case MyCommon.NameBalloonEnum.UserID:
                    cmbNameBalloon.SelectedIndex = 1;
                    break;
                case MyCommon.NameBalloonEnum.NickName:
                    cmbNameBalloon.SelectedIndex = 2;
                    break;
            }

            if (PostCtrlEnter)
            {
                ComboBoxPostKeySelect.SelectedIndex = 1;
            }
            else if (PostShiftEnter)
            {
                ComboBoxPostKeySelect.SelectedIndex = 2;
            }
            else
            {
                ComboBoxPostKeySelect.SelectedIndex = 0;
            }

            TextCountApi.Text = CountApi.ToString();
            TextCountApiReply.Text = CountApiReply.ToString();
            BrowserPathText.Text = BrowserPath;
            CheckPostAndGet.Checked = PostAndGet;
            CheckUseRecommendStatus.Checked = UseRecommendStatus;
            CheckDispUsername.Checked = DispUsername;
            CheckCloseToExit.Checked = CloseToExit;
            CheckMinimizeToTray.Checked = MinimizeToTray;
            switch (DispLatestPost)
            {
                case MyCommon.DispTitleEnum.None:
                    ComboDispTitle.SelectedIndex = 0;
                    break;
                case MyCommon.DispTitleEnum.Ver:
                    ComboDispTitle.SelectedIndex = 1;
                    break;
                case MyCommon.DispTitleEnum.Post:
                    ComboDispTitle.SelectedIndex = 2;
                    break;
                case MyCommon.DispTitleEnum.UnreadRepCount:
                    ComboDispTitle.SelectedIndex = 3;
                    break;
                case MyCommon.DispTitleEnum.UnreadAllCount:
                    ComboDispTitle.SelectedIndex = 4;
                    break;
                case MyCommon.DispTitleEnum.UnreadAllRepCount:
                    ComboDispTitle.SelectedIndex = 5;
                    break;
                case MyCommon.DispTitleEnum.UnreadCountAllCount:
                    ComboDispTitle.SelectedIndex = 6;
                    break;
                case MyCommon.DispTitleEnum.OwnStatus:
                    ComboDispTitle.SelectedIndex = 7;
                    break;
            }
            CheckSortOrderLock.Checked = SortOrderLock;
            CheckViewTabBottom.Checked = ViewTabBottom;
            CheckTinyURL.Checked = TinyUrlResolve;
            CheckForceResolve.Checked = ShortUrlForceResolve;
            switch (_MyProxyType)
            {
                case HttpConnection.ProxyType.None:
                    RadioProxyNone.Checked = true;
                    break;
                case HttpConnection.ProxyType.IE:
                    RadioProxyIE.Checked = true;
                    break;
                default:
                    RadioProxySpecified.Checked = true;
                    break;
            }
            bool chk = RadioProxySpecified.Checked;
            LabelProxyAddress.Enabled = chk;
            TextProxyAddress.Enabled = chk;
            LabelProxyPort.Enabled = chk;
            TextProxyPort.Enabled = chk;
            LabelProxyUser.Enabled = chk;
            TextProxyUser.Enabled = chk;
            LabelProxyPassword.Enabled = chk;
            TextProxyPassword.Enabled = chk;

            TextProxyAddress.Text = ProxyAddress;
            TextProxyPort.Text = ProxyPort.ToString();
            TextProxyUser.Text = ProxyUser;
            TextProxyPassword.Text = ProxyPassword;

            CheckPeriodAdjust.Checked = PeriodAdjust;
            CheckStartupVersion.Checked = StartupVersion;
            if (ApplicationSettings.VersionInfoUrl == null)
                CheckStartupVersion.Enabled = false; // 更新チェック無効化
            CheckStartupFollowers.Checked = StartupFollowers;
            CheckFavRestrict.Checked = RestrictFavCheck;
            CheckAlwaysTop.Checked = AlwaysTop;
            CheckAutoConvertUrl.Checked = UrlConvertAuto;
            ShortenTcoCheck.Checked = ShortenTco;
            ShortenTcoCheck.Enabled = CheckAutoConvertUrl.Checked;
            CheckOutputz.Checked = OutputzEnabled;
            TextBoxOutputzKey.Text = OutputzKey;

            switch (OutputzUrlmode)
            {
                case MyCommon.OutputzUrlmode.twittercom:
                    ComboBoxOutputzUrlmode.SelectedIndex = 0;
                    break;
                case MyCommon.OutputzUrlmode.twittercomWithUsername:
                    ComboBoxOutputzUrlmode.SelectedIndex = 1;
                    break;
            }

            CheckNicoms.Checked = Nicoms;
            chkUnreadStyle.Checked = UseUnreadStyle;
            CmbDateTimeFormat.Text = DateTimeFormat;
            ConnectionTimeOut.Text = DefaultTimeOut.ToString();
            CheckRetweetNoConfirm.Checked = RetweetNoConfirm;
            CheckBalloonLimit.Checked = LimitBalloon;

            ApplyEventNotifyFlag(EventNotifyEnabled, EventNotifyFlag, IsMyEventNotifyFlag);
            CheckForceEventNotify.Checked = ForceEventNotify;
            CheckFavEventUnread.Checked = FavEventUnread;
            ComboBoxTranslateLanguage.SelectedIndex = (new Bing()).GetIndexFromLanguageEnum(TranslateLanguage);
            SoundFileListup();
            ComboBoxAutoShortUrlFirst.SelectedIndex = (int)AutoShortUrlFirst;
            chkTabIconDisp.Checked = TabIconDisp;
            chkReadOwnPost.Checked = ReadOwnPost;
            chkGetFav.Checked = GetFav;
            CheckMonospace.Checked = IsMonospace;
            CheckReadOldPosts.Checked = ReadOldPosts;
            CheckUseSsl.Checked = UseSsl;
            TextBitlyId.Text = BitlyUser;
            TextBitlyPw.Text = BitlyPwd;
            TextBitlyId.Modified = false;
            TextBitlyPw.Modified = false;
            CheckShowGrid.Checked = ShowGrid;
            CheckAtIdSupple.Checked = UseAtIdSupplement;
            CheckHashSupple.Checked = UseHashSupplement;
            CheckPreviewEnable.Checked = PreviewEnable;
            TwitterAPIText.Text = TwitterApiUrl;
            TwitterSearchAPIText.Text = TwitterSearchApiUrl;
            switch (ReplyIconState)
            {
                case MyCommon.REPLY_ICONSTATE.None:
                    ReplyIconStateCombo.SelectedIndex = 0;
                    break;
                case MyCommon.REPLY_ICONSTATE.StaticIcon:
                    ReplyIconStateCombo.SelectedIndex = 1;
                    break;
                case MyCommon.REPLY_ICONSTATE.BlinkIcon:
                    ReplyIconStateCombo.SelectedIndex = 2;
                    break;
            }
            switch (Language)
            {
                case "OS":
                    LanguageCombo.SelectedIndex = 0;
                    break;
                case "ja":
                    LanguageCombo.SelectedIndex = 1;
                    break;
                case "en":
                    LanguageCombo.SelectedIndex = 2;
                    break;
                case "zh-CN":
                    LanguageCombo.SelectedIndex = 3;
                    break;
                default:
                    LanguageCombo.SelectedIndex = 0;
                    break;
            }
            HotkeyCheck.Checked = HotkeyEnabled;
            HotkeyAlt.Checked = ((HotkeyMod & Keys.Alt) == Keys.Alt);
            HotkeyCtrl.Checked = ((HotkeyMod & Keys.Control) == Keys.Control);
            HotkeyShift.Checked = ((HotkeyMod & Keys.Shift) == Keys.Shift);
            HotkeyWin.Checked = ((HotkeyMod & Keys.LWin) == Keys.LWin);
            HotkeyCode.Text = HotkeyValue.ToString();
            HotkeyText.Text = HotkeyKey.ToString();
            HotkeyText.Tag = HotkeyKey;
            HotkeyAlt.Enabled = HotkeyEnabled;
            HotkeyShift.Enabled = HotkeyEnabled;
            HotkeyCtrl.Enabled = HotkeyEnabled;
            HotkeyWin.Enabled = HotkeyEnabled;
            HotkeyText.Enabled = HotkeyEnabled;
            HotkeyCode.Enabled = HotkeyEnabled;
            ChkNewMentionsBlink.Checked = BlinkNewMentions;

            CheckOutputz_CheckedChanged(sender, e);

            GetMoreTextCountApi.Text = MoreCountApi.ToString();
            FirstTextCountApi.Text = FirstCountApi.ToString();
            SearchTextCountApi.Text = SearchCountApi.ToString();
            FavoritesTextCountApi.Text = FavoritesCountApi.ToString();
            UserTimelineTextCountApi.Text = UserTimelineCountApi.ToString();
            ListTextCountApi.Text = ListCountApi.ToString();
            UseChangeGetCount.Checked = UseAdditionalCount;
            Label28.Enabled = UseChangeGetCount.Checked;
            Label30.Enabled = UseChangeGetCount.Checked;
            Label53.Enabled = UseChangeGetCount.Checked;
            Label66.Enabled = UseChangeGetCount.Checked;
            Label17.Enabled = UseChangeGetCount.Checked;
            Label25.Enabled = UseChangeGetCount.Checked;
            GetMoreTextCountApi.Enabled = UseChangeGetCount.Checked;
            FirstTextCountApi.Enabled = UseChangeGetCount.Checked;
            SearchTextCountApi.Enabled = UseChangeGetCount.Checked;
            FavoritesTextCountApi.Enabled = UseChangeGetCount.Checked;
            UserTimelineTextCountApi.Enabled = UseChangeGetCount.Checked;
            ListTextCountApi.Enabled = UseChangeGetCount.Checked;
            CheckOpenUserTimeline.Checked = OpenUserTimeline;
            ListDoubleClickActionComboBox.SelectedIndex = ListDoubleClickAction;
            UserAppointUrlText.Text = UserAppointUrl;
            this.HideDuplicatedRetweetsCheck.Checked = this.HideDuplicatedRetweets;
            this.IsPreviewFoursquareCheckBox.Checked = this.IsPreviewFoursquare;
            this.MapThumbnailProviderComboBox.SelectedIndex = (int)this.MapThumbnailProvider;
            this.MapThumbnailHeightTextBox.Text = this.MapThumbnailHeight.ToString();
            this.MapThumbnailWidthTextBox.Text = this.MapThumbnailWidth.ToString();
            this.MapThumbnailZoomTextBox.Text = this.MapThumbnailZoom.ToString();
            this.IsListsIncludeRtsCheckBox.Checked = this.IsListStatusesIncludeRts;
            this.TabMouseLockCheck.Checked = this.TabMouseLock;
            this.IsRemoveSameFavEventCheckBox.Checked = this.IsRemoveSameEvent;
            this.IsNotifyUseGrowlCheckBox.Checked = this.IsNotifyUseGrowl;

            if (GrowlHelper.IsDllExists)
            {
                IsNotifyUseGrowlCheckBox.Enabled = true;
            }
            else
            {
                IsNotifyUseGrowlCheckBox.Enabled = false;
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
            ActiveControl = StartAuthButton;
        }

        private void UserstreamPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd;
            try
            {
                prd = int.Parse(UserstreamPeriod.Text);
            }
            catch(Exception)
            {
                MessageBox.Show(Properties.Resources.UserstreamPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd < 0 || prd > 60)
            {
                MessageBox.Show(Properties.Resources.UserstreamPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }
            CalcApiUsing();
        }

        private void TimelinePeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd;
            try
            {
                prd = int.Parse(TimelinePeriod.Text);
            }
            catch(Exception)
            {
                MessageBox.Show(Properties.Resources.TimelinePeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Properties.Resources.TimelinePeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            CalcApiUsing();
        }

        private void ReplyPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd;
            try
            {
                prd = int.Parse(ReplyPeriod.Text);
            }
            catch(Exception)
            {
                MessageBox.Show(Properties.Resources.TimelinePeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Properties.Resources.TimelinePeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            CalcApiUsing();
        }

        private void DMPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd;
            try
            {
                prd = int.Parse(DMPeriod.Text);
            }
            catch(Exception)
            {
                MessageBox.Show(Properties.Resources.DMPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Properties.Resources.DMPeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            CalcApiUsing();
        }

        private void PubSearchPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd;
            try
            {
                prd = int.Parse(PubSearchPeriod.Text);
            }
            catch(Exception)
            {
                MessageBox.Show(Properties.Resources.PubSearchPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 30 || prd > 6000))
            {
                MessageBox.Show(Properties.Resources.PubSearchPeriod_ValidatingText2);
                e.Cancel = true;
            }
        }

        private void ListsPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd;
            try
            {
                prd = int.Parse(ListsPeriod.Text);
            }
            catch(Exception)
            {
                MessageBox.Show(Properties.Resources.DMPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Properties.Resources.DMPeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            CalcApiUsing();
        }

        private void UserTimeline_Validating(object sender, CancelEventArgs e)
        {
            int prd;
            try
            {
                prd = int.Parse(UserTimelinePeriod.Text);
            }
            catch(Exception)
            {
                MessageBox.Show(Properties.Resources.DMPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Properties.Resources.DMPeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
            CalcApiUsing();
        }

        private void UReadMng_CheckedChanged(object sender, EventArgs e)
        {
            if (UReadMng.Checked == true)
            {
                StartupReaded.Enabled = true;
            }
            else
            {
                StartupReaded.Enabled = false;
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
                    FontDialog1.Color = lblUnread.ForeColor;
                    FontDialog1.Font = lblUnread.Font;
                    break;
                case "btnDetail":
                    FontDialog1.Color = lblDetail.ForeColor;
                    FontDialog1.Font = lblDetail.Font;
                    break;
                case "btnListFont":
                    FontDialog1.Color = lblListFont.ForeColor;
                    FontDialog1.Font = lblListFont.Font;
                    break;
                case "btnInputFont":
                    FontDialog1.Color = lblInputFont.ForeColor;
                    FontDialog1.Font = lblInputFont.Font;
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
                    lblUnread.ForeColor = FontDialog1.Color;
                    lblUnread.Font = FontDialog1.Font;
                    break;
                case "btnDetail":
                    lblDetail.ForeColor = FontDialog1.Color;
                    lblDetail.Font = FontDialog1.Font;
                    break;
                case "btnListFont":
                    lblListFont.ForeColor = FontDialog1.Color;
                    lblListFont.Font = FontDialog1.Font;
                    break;
                case "btnInputFont":
                    lblInputFont.ForeColor = FontDialog1.Color;
                    lblInputFont.Font = FontDialog1.Font;
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
                    ColorDialog1.Color = lblSelf.BackColor;
                    break;
                case "btnAtSelf":
                    ColorDialog1.Color = lblAtSelf.BackColor;
                    break;
                case "btnTarget":
                    ColorDialog1.Color = lblTarget.BackColor;
                    break;
                case "btnAtTarget":
                    ColorDialog1.Color = lblAtTarget.BackColor;
                    break;
                case "btnAtFromTarget":
                    ColorDialog1.Color = lblAtFromTarget.BackColor;
                    break;
                case "btnFav":
                    ColorDialog1.Color = lblFav.ForeColor;
                    break;
                case "btnOWL":
                    ColorDialog1.Color = lblOWL.ForeColor;
                    break;
                case "btnRetweet":
                    ColorDialog1.Color = lblRetweet.ForeColor;
                    break;
                case "btnInputBackcolor":
                    ColorDialog1.Color = lblInputBackcolor.BackColor;
                    break;
                case "btnAtTo":
                    ColorDialog1.Color = lblAtTo.BackColor;
                    break;
                case "btnListBack":
                    ColorDialog1.Color = lblListBackcolor.BackColor;
                    break;
                case "btnDetailBack":
                    ColorDialog1.Color = lblDetailBackcolor.BackColor;
                    break;
                case "btnDetailLink":
                    ColorDialog1.Color = lblDetailLink.ForeColor;
                    break;
            }

            rtn = ColorDialog1.ShowDialog();

            if (rtn == DialogResult.Cancel) return;

            switch (Btn.Name)
            {
                case "btnSelf":
                    lblSelf.BackColor = ColorDialog1.Color;
                    break;
                case "btnAtSelf":
                    lblAtSelf.BackColor = ColorDialog1.Color;
                    break;
                case "btnTarget":
                    lblTarget.BackColor = ColorDialog1.Color;
                    break;
                case "btnAtTarget":
                    lblAtTarget.BackColor = ColorDialog1.Color;
                    break;
                case "btnAtFromTarget":
                    lblAtFromTarget.BackColor = ColorDialog1.Color;
                    break;
                case "btnFav":
                    lblFav.ForeColor = ColorDialog1.Color;
                    break;
                case "btnOWL":
                    lblOWL.ForeColor = ColorDialog1.Color;
                    break;
                case "btnRetweet":
                    lblRetweet.ForeColor = ColorDialog1.Color;
                    break;
                case "btnInputBackcolor":
                    lblInputBackcolor.BackColor = ColorDialog1.Color;
                    break;
                case "btnAtTo":
                    lblAtTo.BackColor = ColorDialog1.Color;
                    break;
                case "btnListBack":
                    lblListBackcolor.BackColor = ColorDialog1.Color;
                    break;
                case "btnDetailBack":
                    lblDetailBackcolor.BackColor = ColorDialog1.Color;
                    break;
                case "btnDetailLink":
                    lblDetailLink.ForeColor = ColorDialog1.Color;
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
        public bool ShortUrlForceResolve { get; set; }

        private void CheckUseRecommendStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckUseRecommendStatus.Checked == true)
            {
                StatusText.Enabled = false;
            }
            else
            {
                StatusText.Enabled = true;
            }
        }

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
        public bool PeriodAdjust { get; set; }
        public bool StartupVersion { get; set; }
        public bool StartupFollowers { get; set; }
        public bool RestrictFavCheck { get; set; }
        public bool AlwaysTop { get; set; }
        public bool UrlConvertAuto { get; set; }
        public bool ShortenTco { get; set; }
        public bool OutputzEnabled { get; set; }
        public string OutputzKey { get; set; }
        public MyCommon.OutputzUrlmode OutputzUrlmode { get; set; }
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
        public string TwitterSearchApiUrl { get; set; }
        public string Language { get; set; }

        private void Button3_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog filedlg = new OpenFileDialog())
            {
                filedlg.Filter = Properties.Resources.Button3_ClickText1;
                filedlg.FilterIndex = 1;
                filedlg.Title = Properties.Resources.Button3_ClickText2;
                filedlg.RestoreDirectory = true;

                if (filedlg.ShowDialog() == DialogResult.OK)
                {
                    BrowserPathText.Text = filedlg.FileName;
                }
            }
        }

        private void RadioProxySpecified_CheckedChanged(object sender, EventArgs e)
        {
            bool chk = RadioProxySpecified.Checked;
            LabelProxyAddress.Enabled = chk;
            TextProxyAddress.Enabled = chk;
            LabelProxyPort.Enabled = chk;
            TextProxyPort.Enabled = chk;
            LabelProxyUser.Enabled = chk;
            TextProxyUser.Enabled = chk;
            LabelProxyPassword.Enabled = chk;
            TextProxyPassword.Enabled = chk;
        }

        private void TextProxyPort_Validating(object sender, CancelEventArgs e)
        {
            int port;
            if (string.IsNullOrWhiteSpace(TextProxyPort.Text)) TextProxyPort.Text = "0";
            if (int.TryParse(TextProxyPort.Text.Trim(), out port) == false)
            {
                MessageBox.Show(Properties.Resources.TextProxyPort_ValidatingText1);
                e.Cancel = true;
                return;
            }
            if (port < 0 || port > 65535)
            {
                MessageBox.Show(Properties.Resources.TextProxyPort_ValidatingText2);
                e.Cancel = true;
                return;
            }
        }

        private void CheckOutputz_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckOutputz.Checked == true)
            {
                Label59.Enabled = true;
                Label60.Enabled = true;
                TextBoxOutputzKey.Enabled = true;
                ComboBoxOutputzUrlmode.Enabled = true;
            }
            else
            {
                Label59.Enabled = false;
                Label60.Enabled = false;
                TextBoxOutputzKey.Enabled = false;
                ComboBoxOutputzUrlmode.Enabled = false;
            }
        }

        private void TextBoxOutputzKey_Validating(object sender, CancelEventArgs e)
        {
            if (CheckOutputz.Checked)
            {
                TextBoxOutputzKey.Text = TextBoxOutputzKey.Text.Trim();
                if (TextBoxOutputzKey.Text.Length == 0)
                {
                    MessageBox.Show(Properties.Resources.TextBoxOutputzKey_Validating);
                    e.Cancel = true;
                    return;
                }
            }
        }

        private bool CreateDateTimeFormatSample()
        {
            try
            {
                LabelDateTimeFormatApplied.Text = DateTime.Now.ToString(CmbDateTimeFormat.Text);
            }
            catch(FormatException)
            {
                LabelDateTimeFormatApplied.Text = Properties.Resources.CreateDateTimeFormatSampleText1;
                return false;
            }
            return true;
        }

        private void CmbDateTimeFormat_TextUpdate(object sender, EventArgs e)
        {
            CreateDateTimeFormatSample();
        }

        private void CmbDateTimeFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateDateTimeFormatSample();
        }

        private void CmbDateTimeFormat_Validating(object sender, CancelEventArgs e)
        {
            if (!CreateDateTimeFormatSample())
            {
                MessageBox.Show(Properties.Resources.CmbDateTimeFormat_Validating);
                e.Cancel = true;
            }
        }

        private void ConnectionTimeOut_Validating(object sender, CancelEventArgs e)
        {
            int tm;
            try
            {
                tm = int.Parse(ConnectionTimeOut.Text);
            }
            catch(Exception)
            {
                MessageBox.Show(Properties.Resources.ConnectionTimeOut_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (tm < (int)MyCommon.HttpTimeOut.MinValue || tm > (int)MyCommon.HttpTimeOut.MaxValue)
            {
                MessageBox.Show(Properties.Resources.ConnectionTimeOut_ValidatingText1);
                e.Cancel = true;
            }
        }

        private void LabelDateTimeFormatApplied_VisibleChanged(object sender, EventArgs e)
        {
            CreateDateTimeFormatSample();
        }

        private void TextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(TextCountApi.Text);
            }
            catch(Exception)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt < 20 || cnt > 200)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void TextCountApiReply_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(TextCountApiReply.Text);
            }
            catch(Exception)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt < 20 || cnt > 200)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

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
                ComboBoxTranslateLanguage.SelectedIndex = (new Bing()).GetIndexFromLanguageEnum(value);
            }
        }

        public string EventSoundFile { get; set; }
        public int ListDoubleClickAction { get; set; }
        public string UserAppointUrl { get; set; }

        private void ComboBoxAutoShortUrlFirst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxAutoShortUrlFirst.SelectedIndex == (int)MyCommon.UrlConverter.Bitly ||
               ComboBoxAutoShortUrlFirst.SelectedIndex == (int)MyCommon.UrlConverter.Jmp)
            {
                Label76.Enabled = true;
                Label77.Enabled = true;
                TextBitlyId.Enabled = true;
                TextBitlyPw.Enabled = true;
            }
            else
            {
                Label76.Enabled = false;
                Label77.Enabled = false;
                TextBitlyId.Enabled = false;
                TextBitlyPw.Enabled = false;
            }
        }

        private void ButtonBackToDefaultFontColor_Click(object sender, EventArgs e) //Handles ButtonBackToDefaultFontColor.Click, ButtonBackToDefaultFontColor2.Click
        {
            lblUnread.ForeColor = SystemColors.ControlText;
            lblUnread.Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold | FontStyle.Underline);

            lblListFont.ForeColor = System.Drawing.SystemColors.ControlText;
            lblListFont.Font = System.Drawing.SystemFonts.DefaultFont;

            lblDetail.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.ControlText);
            lblDetail.Font = System.Drawing.SystemFonts.DefaultFont;

            lblInputFont.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.ControlText);
            lblInputFont.Font = System.Drawing.SystemFonts.DefaultFont;

            lblSelf.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.AliceBlue);

            lblAtSelf.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.AntiqueWhite);

            lblTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LemonChiffon);

            lblAtTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LavenderBlush);

            lblAtFromTarget.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Honeydew);

            lblFav.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Red);

            lblOWL.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Blue);

            lblInputBackcolor.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.LemonChiffon);

            lblAtTo.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Pink);

            lblListBackcolor.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Window);

            lblDetailBackcolor.BackColor = Color.FromKnownColor(System.Drawing.KnownColor.Window);

            lblDetailLink.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Blue);

            lblRetweet.ForeColor = Color.FromKnownColor(System.Drawing.KnownColor.Green);
        }

        private bool StartAuth()
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

            //通信基底クラス初期化
            HttpConnection.InitializeConnection(20, ptype, padr, pport, pusr, ppw);
            HttpTwitter.TwitterUrl = TwitterAPIText.Text.Trim();
            HttpTwitter.TwitterSearchUrl = TwitterSearchAPIText.Text.Trim();
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

                foreach (object u in this.AuthUserCombo.Items)
                {
                    if (((UserAccount)u).Username.ToLower() == tw.Username.ToLower())
                    {
                        idx = this.AuthUserCombo.Items.IndexOf(u);
                        break;
                    }
                }
                if (idx > -1)
                {
                    this.AuthUserCombo.Items.RemoveAt(idx);
                    this.AuthUserCombo.Items.Insert(idx, user);
                    this.AuthUserCombo.SelectedIndex = idx;
                }
                else
                {
                    this.AuthUserCombo.SelectedIndex = this.AuthUserCombo.Items.Add(user);
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
                    CalcApiUsing();
                    //this.Save.Enabled = true;
                }
            }
        }

        private void AuthClearButton_Click(object sender, EventArgs e)
        {
            //tw.ClearAuthInfo();
            //this.AuthStateLabel.Text = Properties.Resources.AuthorizeButton_Click4;
            //this.AuthUserLabel.Text = "";
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
            //this.Save.Enabled = false;
            CalcApiUsing();
        }

        private void DisplayApiMaxCount()
        {
            var limit = MyCommon.TwitterApiInfo.AccessLimit;
            if (limit != null)
            {
                LabelApiUsing.Text = string.Format(Properties.Resources.SettingAPIUse1, limit.AccessLimitCount - limit.AccessLimitRemain, limit.AccessLimitCount);
            }
            else
            {
                LabelApiUsing.Text = string.Format(Properties.Resources.SettingAPIUse1, "???", "???");
            }
        }

        private void CalcApiUsing()
        {
            int UsingApi = 0;
            int tmp;
            int ListsTabNum = 0;
            int UserTimelineTabNum = 0;
            int ApiLists = 0;
            int ApiUserTimeline = 0;

            try
            {
                // 初回起動時などにnullの場合あり
                ListsTabNum = TabInformations.GetInstance().GetTabsByType(MyCommon.TabUsageType.Lists).Count;
            }
            catch(Exception)
            {
                return;
            }

            try
            {
                // 初回起動時などにnullの場合あり
                UserTimelineTabNum = TabInformations.GetInstance().GetTabsByType(MyCommon.TabUsageType.UserTimeline).Count;
            }
            catch(Exception)
            {
                return;
            }

            // Recent計算 0は手動更新
            if (int.TryParse(TimelinePeriod.Text, out tmp))
            {
                if (tmp != 0)
                {
                    UsingApi += 3600 / tmp;
                }
            }

            // Reply計算 0は手動更新
            if (int.TryParse(ReplyPeriod.Text, out tmp))
            {
                if (tmp != 0)
                {
                    UsingApi += 3600 / tmp;
                }
            }

            // DM計算 0は手動更新 送受信両方
            if (int.TryParse(DMPeriod.Text, out tmp))
            {
                if (tmp != 0)
                {
                    UsingApi += (3600 / tmp) * 2;
                }
            }

            // Listsタブ計算 0は手動更新
            if (int.TryParse(ListsPeriod.Text, out tmp))
            {
                if (tmp != 0)
                {
                    ApiLists = (3600 / tmp) * ListsTabNum;
                    UsingApi += ApiLists;
                }
            }

            // UserTimelineタブ計算 0は手動更新
            if (int.TryParse(UserTimelinePeriod.Text, out tmp))
            {
                if (tmp != 0)
                {
                    ApiUserTimeline = (3600 / tmp) * UserTimelineTabNum;
                    UsingApi += ApiUserTimeline;
                }
            }

            if (tw != null)
            {
                var limit = MyCommon.TwitterApiInfo.AccessLimit;
                if (limit == null)
                {
                    if (Twitter.AccountState == MyCommon.ACCOUNT_STATE.Valid)
                    {
                        Task.Factory.StartNew(() => tw.GetInfoApi()) //取得エラー時はinfoCountは初期状態（値：-1）
                            .ContinueWith(t =>
                            {
                                if (this.IsHandleCreated && !this.IsDisposed)
                                {
                                    this.DisplayApiMaxCount();
                                }
                            }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                    else
                    {
                        LabelApiUsing.Text = string.Format(Properties.Resources.SettingAPIUse1, UsingApi, "???");
                    }
                }
                else
                {
                    LabelApiUsing.Text = string.Format(Properties.Resources.SettingAPIUse1, UsingApi, limit.AccessLimitCount);
                }
            }


            LabelPostAndGet.Visible = CheckPostAndGet.Checked && !tw.UserStreamEnabled;
            LabelUserStreamActive.Visible = tw.UserStreamEnabled;

            LabelApiUsingUserStreamEnabled.Text = string.Format(Properties.Resources.SettingAPIUse2, (ApiLists + ApiUserTimeline).ToString());
            LabelApiUsingUserStreamEnabled.Visible = tw.UserStreamEnabled;
        }

        private void CheckPostAndGet_CheckedChanged(object sender, EventArgs e)
        {
            CalcApiUsing();
        }

        private void Setting_Shown(object sender, EventArgs e)
        {
            do
            {
                Thread.Sleep(10);
                if (this.Disposing || this.IsDisposed) return;
            } while (!this.IsHandleCreated);
            this.TopMost = this.AlwaysTop;
            CalcApiUsing();
        }

        private void ButtonApiCalc_Click(object sender, EventArgs e)
        {
            CalcApiUsing();
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

        private void HotkeyText_KeyDown(object sender, KeyEventArgs e)
        {
            //KeyValueで判定する。
            //表示文字とのテーブルを用意すること
            HotkeyText.Text = e.KeyCode.ToString();
            HotkeyCode.Text = e.KeyValue.ToString();
            HotkeyText.Tag = e.KeyCode;
            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void HotkeyCheck_CheckedChanged(object sender, EventArgs e)
        {
            HotkeyCtrl.Enabled = HotkeyCheck.Checked;
            HotkeyAlt.Enabled = HotkeyCheck.Checked;
            HotkeyShift.Enabled = HotkeyCheck.Checked;
            HotkeyWin.Enabled = HotkeyCheck.Checked;
            HotkeyText.Enabled = HotkeyCheck.Checked;
            HotkeyCode.Enabled = HotkeyCheck.Checked;
        }

        public bool BlinkNewMentions;

        private void GetMoreTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(GetMoreTextCountApi.Text);
            }
            catch(Exception)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt != 0 && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void UseChangeGetCount_CheckedChanged(object sender, EventArgs e)
        {
            GetMoreTextCountApi.Enabled = UseChangeGetCount.Checked;
            FirstTextCountApi.Enabled = UseChangeGetCount.Checked;
            Label28.Enabled = UseChangeGetCount.Checked;
            Label30.Enabled = UseChangeGetCount.Checked;
            Label53.Enabled = UseChangeGetCount.Checked;
            Label66.Enabled = UseChangeGetCount.Checked;
            Label17.Enabled = UseChangeGetCount.Checked;
            Label25.Enabled = UseChangeGetCount.Checked;
            SearchTextCountApi.Enabled = UseChangeGetCount.Checked;
            FavoritesTextCountApi.Enabled = UseChangeGetCount.Checked;
            UserTimelineTextCountApi.Enabled = UseChangeGetCount.Checked;
            ListTextCountApi.Enabled = UseChangeGetCount.Checked;
        }

        private void FirstTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(FirstTextCountApi.Text);
            }
            catch(Exception)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt != 0 && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void SearchTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(SearchTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.TextSearchCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt != 0 && (cnt < 20 || cnt > 100))
            {
                MessageBox.Show(Properties.Resources.TextSearchCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void FavoritesTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(FavoritesTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt != 0 && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void UserTimelineTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(UserTimelineTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt != 0 && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void ListTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(ListTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt != 0 && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
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
            _eventCheckboxTable[0].CheckBox = CheckFavoritesEvent;
            _eventCheckboxTable[0].Type = MyCommon.EVENTTYPE.Favorite;

            _eventCheckboxTable[1] = new EventCheckboxTblElement();
            _eventCheckboxTable[1].CheckBox = CheckUnfavoritesEvent;
            _eventCheckboxTable[1].Type = MyCommon.EVENTTYPE.Unfavorite;

            _eventCheckboxTable[2] = new EventCheckboxTblElement();
            _eventCheckboxTable[2].CheckBox = CheckFollowEvent;
            _eventCheckboxTable[2].Type = MyCommon.EVENTTYPE.Follow;

            _eventCheckboxTable[3] = new EventCheckboxTblElement();
            _eventCheckboxTable[3].CheckBox = CheckListMemberAddedEvent;
            _eventCheckboxTable[3].Type = MyCommon.EVENTTYPE.ListMemberAdded;

            _eventCheckboxTable[4] = new EventCheckboxTblElement();
            _eventCheckboxTable[4].CheckBox = CheckListMemberRemovedEvent;
            _eventCheckboxTable[4].Type = MyCommon.EVENTTYPE.ListMemberRemoved;

            _eventCheckboxTable[5] = new EventCheckboxTblElement();
            _eventCheckboxTable[5].CheckBox = CheckBlockEvent;
            _eventCheckboxTable[5].Type = MyCommon.EVENTTYPE.Block;

            _eventCheckboxTable[6] = new EventCheckboxTblElement();
            _eventCheckboxTable[6].CheckBox = CheckUserUpdateEvent;
            _eventCheckboxTable[6].Type = MyCommon.EVENTTYPE.UserUpdate;

            _eventCheckboxTable[7] = new EventCheckboxTblElement();
            _eventCheckboxTable[7].CheckBox = CheckListCreatedEvent;
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

            CheckEventNotify.Checked = rootEnabled;

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
                tbl.CheckBox.Enabled = CheckEventNotify.Checked;
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
            ComboBoxEventNotifySound.Items.Clear();
            ComboBoxEventNotifySound.Items.Add("");
            DirectoryInfo oDir = new DirectoryInfo(Application.StartupPath + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(Application.StartupPath, "Sounds")))
            {
                oDir = oDir.GetDirectories("Sounds")[0];
            }
            foreach (FileInfo oFile in oDir.GetFiles("*.wav"))
            {
                ComboBoxEventNotifySound.Items.Add(oFile.Name);
            }
            int idx = ComboBoxEventNotifySound.Items.IndexOf(EventSoundFile);
            if (idx == -1) idx = 0;
            ComboBoxEventNotifySound.SelectedIndex = idx;
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

        private void UserAppointUrlText_Validating(object sender, CancelEventArgs e)
        {
            if (!UserAppointUrlText.Text.StartsWith("http") && !string.IsNullOrEmpty(UserAppointUrlText.Text))
            {
                MessageBox.Show("Text Error:正しいURLではありません");
            }
        }

        private void OpenUrl(string url)
        {
            string myPath = url;
            string path = this.BrowserPathText.Text;
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

        private void CheckAutoConvertUrl_CheckedChanged(object sender, EventArgs e)
        {
            ShortenTcoCheck.Enabled = CheckAutoConvertUrl.Checked;
        }

        public AppendSettingDialog()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.MIcon;
        }
    }
}
