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
using System.Windows.Forms;
using System.Xml.Serialization;
using OpenTween.Thumbnail;

namespace OpenTween
{
    public class SettingCommon : SettingBase<SettingCommon>
    {
        #region "Settingクラス基本"
        public static SettingCommon Load(string settingsPath)
            => LoadSettings(settingsPath);

        public void Save(string settingsPath)
            => SaveSettings(this, settingsPath);
        #endregion

        public List<UserAccount> UserAccounts = new();
        public string UserName = "";

        [XmlIgnore]
        public string Password = "";

        public string EncryptPassword
        {
            get => this.Encrypt(this.Password);
            set => this.Password = this.Decrypt(value);
        }

        public string Token = "";
        [XmlIgnore]
        public string TokenSecret = "";

        public string EncryptTokenSecret
        {
            get => this.Encrypt(this.TokenSecret);
            set => this.TokenSecret = this.Decrypt(value);
        }

        private string Encrypt(string password)
        {
            if (MyCommon.IsNullOrEmpty(password)) password = "";
            if (password.Length > 0)
            {
                try
                {
                    return MyCommon.EncryptString(password);
                }
                catch (Exception)
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        private string Decrypt(string password)
        {
            if (MyCommon.IsNullOrEmpty(password)) password = "";
            if (password.Length > 0)
            {
                try
                {
                    password = MyCommon.DecryptString(password);
                }
                catch (Exception)
                {
                    password = "";
                }
            }
            return password;
        }

        public long UserId = 0;
        public List<string> TabList = new();
        public int TimelinePeriod = 90;
        public int ReplyPeriod = 180;
        public int DMPeriod = 600;
        public int PubSearchPeriod = 180;
        public int ListsPeriod = 180;

        /// <summary>
        /// 起動時読み込み分を既読にするか。trueなら既読として処理
        /// </summary>
        public bool Read = true;

        public bool ListLock = false;
        public MyCommon.IconSizes IconSize = MyCommon.IconSizes.Icon16;
        public bool NewAllPop = true;
        public string TranslateLanguage = Properties.Resources.TranslateDefaultLanguage;

        /// <summary>
        /// サウンド再生（タブ別設定より優先）
        /// </summary>
        public bool PlaySound = false;

        /// <summary>
        /// 未読管理。trueなら未読管理する
        /// </summary>
        public bool UnreadManage = true;

        /// <summary>
        /// 片思い表示。trueなら片思い表示する
        /// </summary>
        public bool OneWayLove = true;

        public MyCommon.NameBalloonEnum NameBalloon = MyCommon.NameBalloonEnum.NickName;
        public bool PostCtrlEnter = true;
        public bool PostShiftEnter = false;
        public int CountApi = 60;
        public int CountApiReply = 40;
        public bool PostAndGet = true;
        public bool DispUsername = false;
        public bool MinimizeToTray = false;
        public bool CloseToExit = false;
        public MyCommon.DispTitleEnum DispLatestPost = MyCommon.DispTitleEnum.Post;
        public bool SortOrderLock = false;

        /// <summary>
        /// タブを下部に表示するかどうか
        /// </summary>
        public bool ViewTabBottom = true;

        public bool TinyUrlResolve = true;
        public bool StartupVersion = true;
        public bool StartupFollowers = true;

        /// <summary>
        /// Twitter API v2 の使用を有効にする
        /// </summary>
        public bool EnableTwitterV2Api { get; set; } = true;

        public bool RestrictFavCheck = false;
        public bool AlwaysTop = false;
        public string CultureCode = "";
        public bool UrlConvertAuto = false;
        public int SortColumn = 3;
        public int SortOrder = 1;
        public bool IsMonospace = false;
        public bool ReadOldPosts = false;
        public string Language = "OS";
        public bool Nicoms = false;
        public List<string> HashTags = new();
        public string HashSelected = "";
        public bool HashIsPermanent = false;
        public bool HashIsHead = false;
        public bool HashIsNotAddToAtReply = true;
        public bool PreviewEnable = true;
        public bool StatusAreaAtBottom = true;

        public MyCommon.UrlConverter AutoShortUrlFirst = MyCommon.UrlConverter.Uxnu;
        public bool UseUnreadStyle = true;
        public string DateTimeFormat = "yyyy/MM/dd H:mm:ss";
        public int DefaultTimeOut = 20;

        /// <summary>画像アップロードのタイムアウト設定 (秒)</summary>
        public int UploadImageTimeout { get; set; } = 60;

        public bool RetweetNoConfirm = false;
        public bool LimitBalloon = false;
        public bool TabIconDisp = true;
        public MyCommon.REPLY_ICONSTATE ReplyIconState = MyCommon.REPLY_ICONSTATE.StaticIcon;
        public bool WideSpaceConvert = true;
        public bool ReadOwnPost = false;
        public bool GetFav = true;
        public string BilyUser = "";
        public string BitlyPwd = "";

        /// <summary>Bitly API アクセストークン</summary>
        public string BitlyAccessToken { get; set; } = "";

        public bool ShowGrid = false;
        public bool UseAtIdSupplement = true;
        public bool UseHashSupplement = true;

        [XmlElement(ElementName = "TwitterUrl")]
        public string TwitterApiHost = "api.twitter.com";

        public bool HotkeyEnabled = false;
        public Keys HotkeyModifier = Keys.None;
        public Keys HotkeyKey = Keys.None;
        public int HotkeyValue = 0;
        public bool BlinkNewMentions = false;
        public bool FocusLockToStatusText = false;
        public bool UseAdditionalCount = false;
        public int MoreCountApi = 200;
        public int FirstCountApi = 100;
        public int SearchCountApi = 100;
        public int FavoritesCountApi = 40;
        public int UserTimelineCountApi = 20;
        public int UserTimelinePeriod = 600;
        public bool OpenUserTimeline = true;
        public int ListCountApi = 100;
        public int UseImageService = 0;
        public string UseImageServiceName = "";

        [XmlIgnore]
        public MyCommon.ListItemDoubleClickActionType ListDoubleClickAction { get; set; } = MyCommon.ListItemDoubleClickActionType.Reply;

        [XmlElement(ElementName = nameof(ListDoubleClickAction))]
        public int ListDoubleClickActionNumeric
        {
            get => (int)this.ListDoubleClickAction;
            set => this.ListDoubleClickAction = (MyCommon.ListItemDoubleClickActionType)value;
        }

        public string UserAppointUrl = "";
        public bool HideDuplicatedRetweets = false;
        public bool EnableImgAzyobuziNet = true;
        public bool ImgAzyobuziNetDisabledInDM = true;
        public int MapThumbnailHeight = 200;
        public int MapThumbnailWidth = 200;
        public int MapThumbnailZoom = 15;
        public MapProvider MapThumbnailProvider = MapProvider.OpenStreetMap;

        /// <summary>Listの発言取得に公式RTを含める</summary>
        public bool IsListsIncludeRts = true;

        public bool TabMouseLock = false;
        public bool IsUseNotifyGrowl = false;
        public bool ForceIPv4 = false;
        public bool ErrorReportAnonymous = true;

        /// <summary>pic.twitter.com への画像アップロード時に JPEG への変換を回避する</summary>
        public bool AlphaPNGWorkaround { get; set; } = false;

        /// <summary>アップデート通知を無視するバージョン番号</summary>
        [XmlIgnore]
        public Version? SkipUpdateVersion
        {
            get => MyCommon.IsNullOrEmpty(this.SkipUpdateVersionStr) ? null : Version.Parse(this.SkipUpdateVersionStr);
            set => this.SkipUpdateVersionStr = value == null ? "" : value.ToString();
        }

        [XmlElement(ElementName = nameof(SkipUpdateVersion))]
        public string SkipUpdateVersionStr { get; set; } = "";

        public void Validate()
        {
            if (this.TimelinePeriod < 0)
                this.TimelinePeriod = 15;

            if (this.ReplyPeriod < 0)
                this.ReplyPeriod = 15;

            if (this.DMPeriod < 0)
                this.DMPeriod = 15;

            if (this.PubSearchPeriod < 0)
                this.PubSearchPeriod = 30;

            if (this.UserTimelinePeriod < 0)
                this.UserTimelinePeriod = 15;

            if (this.ListsPeriod < 0)
                this.ListsPeriod = 15;

            if (!Twitter.VerifyApiResultCount(MyCommon.WORKERTYPE.Timeline, this.CountApi))
                this.CountApi = 60;

            if (!Twitter.VerifyApiResultCount(MyCommon.WORKERTYPE.Reply, this.CountApiReply))
                this.CountApiReply = 40;

            if (this.MoreCountApi != 0 && !Twitter.VerifyMoreApiResultCount(this.MoreCountApi))
                this.MoreCountApi = 200;

            if (this.FirstCountApi != 0 && !Twitter.VerifyFirstApiResultCount(this.FirstCountApi))
                this.FirstCountApi = 100;

            if (this.FavoritesCountApi != 0 && !Twitter.VerifyApiResultCount(MyCommon.WORKERTYPE.Favorites, this.FavoritesCountApi))
                this.FavoritesCountApi = 40;

            if (this.ListCountApi != 0 && !Twitter.VerifyApiResultCount(MyCommon.WORKERTYPE.List, this.ListCountApi))
                this.ListCountApi = 100;

            if (this.SearchCountApi != 0 && !Twitter.VerifyApiResultCount(MyCommon.WORKERTYPE.PublicSearch, this.SearchCountApi))
                this.SearchCountApi = 100;

            if (this.UserTimelineCountApi != 0 && !Twitter.VerifyApiResultCount(MyCommon.WORKERTYPE.UserTimeline, this.UserTimelineCountApi))
                this.UserTimelineCountApi = 20;

            // 廃止サービスが選択されていた場合ux.nuへ読み替え
            if (this.AutoShortUrlFirst < 0)
                this.AutoShortUrlFirst = MyCommon.UrlConverter.Uxnu;

            var selectedAccount = this.UserAccounts.Find(
                x => string.Equals(x.Username, this.UserName, StringComparison.InvariantCultureIgnoreCase)
            );
            if (selectedAccount?.UserId == 0)
                selectedAccount.UserId = this.UserId;

            if (MyCommon.IsNullOrEmpty(this.Token))
                this.UserName = "";
        }
    }

    public class UserAccount
    {
        public string Username = "";
        public long UserId = 0;
        public string Token = "";
        [XmlIgnore]
        public string TokenSecret = "";

        public string EncryptTokenSecret
        {
            get => this.Encrypt(this.TokenSecret);
            set => this.TokenSecret = this.Decrypt(value);
        }

        private string Encrypt(string password)
        {
            if (MyCommon.IsNullOrEmpty(password)) password = "";
            if (password.Length > 0)
            {
                try
                {
                    return MyCommon.EncryptString(password);
                }
                catch (Exception)
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
        }

        private string Decrypt(string password)
        {
            if (MyCommon.IsNullOrEmpty(password)) password = "";
            if (password.Length > 0)
            {
                try
                {
                    password = MyCommon.DecryptString(password);
                }
                catch (Exception)
                {
                    password = "";
                }
            }
            return password;
        }

        public override string ToString()
            => this.Username;
    }
}