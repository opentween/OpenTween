// OpenTween - Client of Twitter
// Copyright (c) 2017 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTween.Connection;
using OpenTween.Thumbnail;

namespace OpenTween.Setting
{
    public class SettingManager
    {
        public static SettingManager Instance { get; } = new();

        public SettingCommon Common { get; internal set; } = new();

        public SettingLocal Local { get; internal set; } = new();

        public SettingTabs Tabs { get; internal set; } = new();

        public SettingAtIdList AtIdList { get; internal set; } = new();

        /// <summary>ユーザによる設定が必要な項目が残っているか</summary>
        public bool IsIncomplete
            => MyCommon.IsNullOrEmpty(this.Common.UserName);

        public bool IsFirstRun { get; private set; } = false;

        public void LoadAll()
        {
            this.LoadCommon();
            this.LoadLocal();
            this.LoadTabs();
            this.LoadAtIdList();

            this.IsFirstRun = this.IsIncomplete;
        }

        public void LoadCommon()
        {
            var settings = SettingCommon.Load();

            if (settings.UserAccounts == null || settings.UserAccounts.Count == 0)
            {
                settings.UserAccounts = new List<UserAccount>();
                if (!MyCommon.IsNullOrEmpty(settings.UserName))
                {
                    var account = new UserAccount
                    {
                        Username = settings.UserName,
                        UserId = settings.UserId,
                        Token = settings.Token,
                        TokenSecret = settings.TokenSecret,
                    };

                    settings.UserAccounts.Add(account);
                }
            }

            this.Common = settings;
        }

        public void LoadLocal()
            => this.Local = SettingLocal.Load();

        public void LoadTabs()
            => this.Tabs = SettingTabs.Load();

        public void LoadAtIdList()
            => this.AtIdList = SettingAtIdList.Load();

        public void SaveAll()
        {
            this.SaveCommon();
            this.SaveLocal();
            this.SaveTabs();
            this.SaveAtIdList();
        }

        public void SaveCommon()
            => this.Common.Save();

        public void SaveLocal()
            => this.Local.Save();

        public void SaveTabs()
            => this.Tabs.Save();

        public void SaveAtIdList()
            => this.AtIdList.Save();

        public void ApplySettings()
        {
            // 静的フィールドにセットする必要のある設定値を更新
            Networking.DefaultTimeout = TimeSpan.FromSeconds(this.Common.DefaultTimeOut);
            Networking.UploadImageTimeout = TimeSpan.FromSeconds(this.Common.UploadImageTimeout);
            Networking.ForceIPv4 = this.Common.ForceIPv4;
            Networking.SetWebProxy(
                this.Local.ProxyType,
                this.Local.ProxyAddress,
                this.Local.ProxyPort,
                this.Local.ProxyUser,
                this.Local.ProxyPassword);

            TwitterApiConnection.RestApiHost = this.Common.TwitterApiHost;

            ShortUrl.Instance.DisableExpanding = !this.Common.TinyUrlResolve;
            ShortUrl.Instance.BitlyAccessToken = this.Common.BitlyAccessToken;
            ShortUrl.Instance.BitlyId = this.Common.BilyUser;
            ShortUrl.Instance.BitlyKey = this.Common.BitlyPwd;
        }
    }
}
