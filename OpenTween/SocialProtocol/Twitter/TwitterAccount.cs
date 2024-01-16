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

#nullable enable

using System;
using System.Diagnostics;
using OpenTween.Connection;

namespace OpenTween.SocialProtocol.Twitter
{
    public class TwitterAccount : ISocialAccount
    {
        private readonly OpenTween.Twitter twLegacy = new(new());

        public Guid UniqueKey { get; }

        public bool IsDisposed { get; private set; }

        public OpenTween.Twitter Legacy
            => this.twLegacy;

        public long UserId
            => this.Legacy.UserId;

        public string UserName
            => this.Legacy.Username;

        public APIAuthType AuthType
            => this.Legacy.Api.AuthType;

        public IApiConnection Connection
            => this.Legacy.Api.Connection;

        public TwitterAccount(Guid uniqueKey)
            => this.UniqueKey = uniqueKey;

        public void Initialize(UserAccount accountSettings, SettingCommon settingCommon)
        {
            Debug.Assert(accountSettings.UniqueKey == this.UniqueKey, "UniqueKey must be same as current value.");

            var credential = accountSettings.GetTwitterCredential();
            this.twLegacy.Initialize(credential, accountSettings.Username, accountSettings.UserId);
            this.twLegacy.RestrictFavCheck = settingCommon.RestrictFavCheck;
            this.twLegacy.ReadOwnPost = settingCommon.ReadOwnPost;
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.twLegacy.Dispose();
            this.IsDisposed = true;
        }
    }
}
