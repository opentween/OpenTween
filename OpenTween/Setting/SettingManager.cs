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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween.Setting
{
    public static class SettingManager
    {
        public static SettingCommon Common { get; internal set; } = new SettingCommon();

        public static SettingLocal Local { get; internal set; } = new SettingLocal();

        public static SettingTabs Tabs { get; internal set; } = new SettingTabs();

        public static SettingAtIdList AtIdList { get; internal set; } = new SettingAtIdList();

        public static void LoadAll()
        {
            LoadCommon();
            LoadLocal();
            LoadTabs();
            LoadAtIdList();
        }

        public static void LoadCommon()
        {
            var settings = SettingCommon.Load();

            if (settings.UserAccounts == null || settings.UserAccounts.Count == 0)
            {
                settings.UserAccounts = new List<UserAccount>();
                if (!string.IsNullOrEmpty(settings.UserName))
                {
                    UserAccount account = new UserAccount();
                    account.Username = settings.UserName;
                    account.UserId = settings.UserId;
                    account.Token = settings.Token;
                    account.TokenSecret = settings.TokenSecret;

                    settings.UserAccounts.Add(account);
                }
            }

            SettingManager.Common = settings;
        }

        public static void LoadLocal()
            => SettingManager.Local = SettingLocal.Load();

        public static void LoadTabs()
            => SettingManager.Tabs = SettingTabs.Load();

        public static void LoadAtIdList()
            => SettingManager.AtIdList = SettingAtIdList.Load();

        public static void SaveAll()
        {
            SaveCommon();
            SaveLocal();
            SaveTabs();
            SaveAtIdList();
        }

        public static void SaveCommon()
            => SettingManager.Common.Save();

        public static void SaveLocal()
            => SettingManager.Local.Save();

        public static void SaveTabs()
            => SettingManager.Tabs.Save();

        public static void SaveAtIdList()
            => SettingManager.AtIdList.Save();
    }
}
