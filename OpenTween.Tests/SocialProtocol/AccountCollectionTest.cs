// OpenTween - Client of Twitter
// Copyright (c) 2024 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using Xunit;

namespace OpenTween.SocialProtocol
{
    public class AccountCollectionTest
    {
        private readonly Random random = new();

        private UserAccount CreateAccountSetting(string key)
        {
            return new()
            {
                UniqueKey = new(key),
                TwitterAuthType = APIAuthType.OAuth1,
                Token = "aaaaa",
                TokenSecret = "bbbbb",
                UserId = this.random.Next(),
                Username = "tetete",
            };
        }

        [Fact]
        public void LoadFromSettings_Test()
        {
            using var accounts = new AccountCollection();

            var settingCommon = new SettingCommon
            {
                UserAccounts = new()
                {
                    this.CreateAccountSetting("00000000-0000-4000-8000-000000000000"),
                },
                SelectedAccountKey = new("00000000-0000-4000-8000-000000000000"),
            };
            accounts.LoadFromSettings(settingCommon);

            Assert.Single(accounts.Items);
            Assert.Equal(settingCommon.UserAccounts[0].UserId, accounts.Primary.UserId);
        }

        [Fact]
        public void LoadFromSettings_RemoveTest()
        {
            using var accounts = new AccountCollection();

            var settingCommon1 = new SettingCommon
            {
                UserAccounts = new()
                {
                    this.CreateAccountSetting("00000000-0000-4000-8000-000000000000"),
                },
                SelectedAccountKey = new("00000000-0000-4000-8000-000000000000"),
            };
            accounts.LoadFromSettings(settingCommon1);

            var accountItem1 = Assert.Single(accounts.Items);
            Assert.Equal(settingCommon1.UserAccounts[0].UserId, accounts.Primary.UserId);

            var settingCommon2 = new SettingCommon
            {
                UserAccounts = new()
                {
                    // 00000000-0000-4000-8000-000000000000 は削除
                },
                SelectedAccountKey = null,
            };
            accounts.LoadFromSettings(settingCommon2);

            // 欠けている ID は削除される
            Assert.Empty(accounts.Items);
            Assert.Equal(APIAuthType.None, accounts.Primary.Api.AuthType);
            Assert.True(accountItem1.IsDisposed);
        }

        [Fact]
        public void LoadFromSettings_ReconfigureTest()
        {
            using var accounts = new AccountCollection();

            var settingCommon1 = new SettingCommon
            {
                UserAccounts = new()
                {
                    this.CreateAccountSetting("00000000-0000-4000-8000-000000000000"),
                },
                SelectedAccountKey = new("00000000-0000-4000-8000-000000000000"),
            };
            accounts.LoadFromSettings(settingCommon1);

            var accountItem1 = Assert.Single(accounts.Items);
            Assert.Equal(settingCommon1.UserAccounts[0].UserId, accounts.Primary.UserId);

            var settingCommon2 = new SettingCommon
            {
                UserAccounts = new()
                {
                    // 同一の ID だが再認証により UserId が変わっている
                    this.CreateAccountSetting("00000000-0000-4000-8000-000000000000"),
                },
                SelectedAccountKey = new("00000000-0000-4000-8000-000000000000"),
            };
            accounts.LoadFromSettings(settingCommon2);

            var accountItem2 = Assert.Single(accounts.Items);
            Assert.Equal(settingCommon2.UserAccounts[0].UserId, accounts.Primary.UserId);

            // 同一の ID は同じインスタンスを使用
            Assert.Same(accountItem1, accountItem2);
            Assert.NotEqual(
                settingCommon1.UserAccounts[0].UserId,
                accountItem2.UserId
            );
            Assert.Equal(
                settingCommon2.UserAccounts[0].UserId,
                accountItem2.UserId
            );
            Assert.False(accountItem2.IsDisposed);
        }
    }
}
