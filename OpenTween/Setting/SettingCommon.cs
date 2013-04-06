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
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenTween.Thumbnail;

namespace OpenTween
{
    [Serializable]
    public class SettingCommon : SettingBase<SettingCommon>
    {

        #region "Settingクラス基本"
        public static SettingCommon Load()
        {
            return LoadSettings();
        }

        public void Save()
        {
            SaveSettings(this);
        }
        #endregion

        public List<UserAccount> UserAccounts;
        public string UserName = "";

        [XmlIgnore]
        public string Password = "";
        public string EncryptPassword
        {
            get { return Encrypt(Password); }
            set { Password = Decrypt(value); }
        }

        public string Token = "";
        [XmlIgnore]
        public string TokenSecret = "";
        public string EncryptTokenSecret
        {
            get { return Encrypt(TokenSecret); }
            set { TokenSecret = Decrypt(value); }
        }

        private string Encrypt(string password)
        {
            if (String.IsNullOrEmpty(password)) password = "";
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
            if (String.IsNullOrEmpty(password)) password = "";
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
        public List<string> TabList;
        public int TimelinePeriod = 90;
        public int ReplyPeriod = 180;
        public int DMPeriod = 600;
        public int PubSearchPeriod = 180;
        public int ListsPeriod = 180;
        public bool Read = true;
        public bool ListLock = false;
        public MyCommon.IconSizes IconSize = MyCommon.IconSizes.Icon16;
        public bool NewAllPop = true;
        public bool EventNotifyEnabled = true;
        public MyCommon.EVENTTYPE EventNotifyFlag = MyCommon.EVENTTYPE.All;
        public MyCommon.EVENTTYPE IsMyEventNotifyFlag = MyCommon.EVENTTYPE.All;
        public bool ForceEventNotify = false;
        public bool FavEventUnread = true;
        public string TranslateLanguage = Properties.Resources.TranslateDefaultLanguage;
        public string EventSoundFile = "";
        public bool PlaySound = false;
        public bool UnreadManage = true;
        public bool OneWayLove = true;
        public MyCommon.NameBalloonEnum NameBalloon = MyCommon.NameBalloonEnum.NickName;
        public bool PostCtrlEnter = false;
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
        public bool ShortUrlForceResolve = false;
        public bool PeriodAdjust = true;
        public bool StartupVersion = true;
        public bool StartupFollowers = true;
        public bool RestrictFavCheck = false;
        public bool AlwaysTop = false;
        public string CultureCode = "";
        public bool UrlConvertAuto = false;
        public bool Outputz = false;
        public int SortColumn = 3;
        public int SortOrder = 1;
        public bool IsMonospace = false;
        public bool ReadOldPosts = false;
        public bool UseSsl = true;
        public string Language = "OS";
        public bool Nicoms = false;
        public List<string> HashTags = new List<string>();
        public string HashSelected = "";
        public bool HashIsPermanent = false;
        public bool HashIsHead = false;
        public bool HashIsNotAddToAtReply = true;
        public bool PreviewEnable = true;

        [XmlIgnore]
        public string OutputzKey = "";
        public string EncryptOutputzKey
        {
            get { return Encrypt(OutputzKey); }
            set { OutputzKey = Decrypt(value); }
        }

        public MyCommon.OutputzUrlmode OutputzUrlMode = MyCommon.OutputzUrlmode.twittercom;
        public MyCommon.UrlConverter AutoShortUrlFirst = MyCommon.UrlConverter.Uxnu;
        public bool UseUnreadStyle = true;
        public string DateTimeFormat = "yyyy/MM/dd H:mm:ss";
        public int DefaultTimeOut = 20;
        public bool RetweetNoConfirm = false;
        public bool LimitBalloon = false;
        public bool TabIconDisp = true;
        public MyCommon.REPLY_ICONSTATE ReplyIconState = MyCommon.REPLY_ICONSTATE.StaticIcon;
        public bool WideSpaceConvert = true;
        public bool ReadOwnPost = false;
        public bool GetFav = true;
        public string BilyUser = "";
        public string BitlyPwd = "";
        public bool ShowGrid = false;
        public bool UseAtIdSupplement = true;
        public bool UseHashSupplement = true;
        public string TwitterUrl = "api.twitter.com";
        public string TwitterSearchUrl = "search.twitter.com";
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
        public string TrackWord = "";
        public bool AllAtReply = false;
        public bool UserstreamStartup = true;
        public int UserstreamPeriod = 0;
        public int UserTimelineCountApi = 20;
        public int UserTimelinePeriod = 600;
        public bool OpenUserTimeline = true;
        public int ListCountApi = 100;
        public int UseImageService = 0;
        public int ListDoubleClickAction = 0;
        public string UserAppointUrl = "";
        public bool HideDuplicatedRetweets = false;
        public bool IsPreviewFoursquare = false;
        public int MapThumbnailHeight = 200;
        public int MapThumbnailWidth = 200;
        public int MapThumbnailZoom = 15;
        public MapProvider MapThumbnailProvider = MapProvider.OpenStreetMap;
        public bool IsListsIncludeRts = false;
        public long GAFirst = 0;
        public long GALast = 0;
        public bool TabMouseLock = false;
        public bool IsRemoveSameEvent = false;
        public bool IsUseNotifyGrowl = false;
    }

    public class UserAccount
    {
        public string Username = "";
        public long UserId = 0;
        public long GAFirst = 0;
        public long GALast = 0;
        public string Token = "";
        [XmlIgnore]
        public string TokenSecret = "";
        public string EncryptTokenSecret
        {
            get { return Encrypt(TokenSecret); }
            set { TokenSecret = Decrypt(value); }
        }
        private string Encrypt(string password)
        {
            if (String.IsNullOrEmpty(password)) password = "";
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
            if (String.IsNullOrEmpty(password)) password = "";
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
        {
            return this.Username;
        }
    }
}