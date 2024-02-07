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
using OpenTween.Models;
using OpenTween.SocialProtocol.Twitter;
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
            Assert.Equal(APIAuthType.None, ((TwitterAccount)accounts.Primary).AuthType);
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

        [Fact]
        public void GetAccountForTab_DefaultTest()
        {
            using var accounts = new AccountCollection();
            accounts.LoadFromSettings(new()
            {
                UserAccounts = new()
                {
                    this.CreateAccountSetting("00000000-0000-4000-8000-000000000000"),
                },
                SelectedAccountKey = new("00000000-0000-4000-8000-000000000000"),
            });

            var tabWithoutAccountId = new PublicSearchTabModel("hoge");

            // SourceAccountId が null のタブに対しては Primary のアカウントを返す
            var actual = accounts.GetAccountForTab(tabWithoutAccountId);
            Assert.Equal(new("00000000-0000-4000-8000-000000000000"), actual?.UniqueKey);
        }

        [Fact]
        public void GetAccountForTab_SpecifiedAccountTest()
        {
            using var accounts = new AccountCollection();
            accounts.LoadFromSettings(new()
            {
                UserAccounts = new()
                {
                    this.CreateAccountSetting("00000000-0000-4000-8000-000000000000"),
                    this.CreateAccountSetting("00000000-0000-4000-8000-111111111111"),
                },
                SelectedAccountKey = new("00000000-0000-4000-8000-000000000000"),
            });

            var tabWithAccountId = new RelatedPostsTabModel("hoge", new("00000000-0000-4000-8000-111111111111"), new());

            // SourceAccountId が設定されているタブに対しては対応するアカウントを返す
            var actual = accounts.GetAccountForTab(tabWithAccountId);
            Assert.Equal(new("00000000-0000-4000-8000-111111111111"), actual?.UniqueKey);
        }

        [Fact]
        public void GetAccountForTab_NotExistsTest()
        {
            using var accounts = new AccountCollection();
            accounts.LoadFromSettings(new()
            {
                UserAccounts = new()
                {
                    this.CreateAccountSetting("00000000-0000-4000-8000-000000000000"),
                },
                SelectedAccountKey = new("00000000-0000-4000-8000-000000000000"),
            });

            var tabWithAccountId = new RelatedPostsTabModel("hoge", new("00000000-0000-4000-8000-999999999999"), new());

            // SourceAccountId に存在しない ID が設定されていた場合は null を返す
            Assert.Null(accounts.GetAccountForTab(tabWithAccountId));
        }
    }
}
