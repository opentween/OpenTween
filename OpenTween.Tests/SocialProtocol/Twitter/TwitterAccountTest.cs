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

namespace OpenTween.SocialProtocol.Twitter
{
    public class TwitterAccountTest
    {
        [Fact]
        public void Initialize_Test()
        {
            var accountKey = Guid.NewGuid();
            using var account = new TwitterAccount(accountKey);

            var accountSettings = new UserAccount
            {
                UniqueKey = accountKey,
                TwitterAuthType = APIAuthType.OAuth1,
                Token = "aaaaa",
                TokenSecret = "aaaaa",
                UserId = 11111L,
                Username = "tetete",
            };
            var settingCommon = new SettingCommon();
            account.Initialize(accountSettings, settingCommon);
            Assert.Equal(11111L, account.UserId);
            Assert.Equal("tetete", account.UserName);
            Assert.Equal(APIAuthType.OAuth1, account.AuthType);
            Assert.Same(account.Legacy.Api.Connection, account.Connection);
        }

        [Fact]
        public void Initialize_ReconfigureTest()
        {
            var accountKey = Guid.NewGuid();
            using var account = new TwitterAccount(accountKey);

            var accountSettings1 = new UserAccount
            {
                UniqueKey = accountKey,
                TwitterAuthType = APIAuthType.OAuth1,
                Token = "aaaaa",
                TokenSecret = "aaaaa",
                UserId = 11111L,
                Username = "tetete",
            };
            var settingCommon1 = new SettingCommon();
            account.Initialize(accountSettings1, settingCommon1);
            Assert.Equal(11111L, account.UserId);

            var accountSettings2 = new UserAccount
            {
                UniqueKey = accountKey,
                TwitterAuthType = APIAuthType.OAuth1,
                Token = "bbbbb",
                TokenSecret = "bbbbb",
                UserId = 22222L,
                Username = "hoge",
            };
            var settingCommon2 = new SettingCommon();
            account.Initialize(accountSettings2, settingCommon2);
            Assert.Equal(22222L, account.UserId);
        }
    }
}
