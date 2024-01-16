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
using System.Collections.Generic;
using System.Linq;

namespace OpenTween.SocialProtocol
{
    public sealed class AccountCollection : IDisposable
    {
        private Dictionary<Guid, Twitter> accounts = new();
        private Guid? primaryId;
        private readonly Twitter emptyAccount = new(new());

        public bool IsDisposed { get; private set; }

        public Twitter Primary
            => this.primaryId != null ? this.accounts[this.primaryId.Value] : this.emptyAccount;

        public Twitter[] Items
            => this.accounts.Values.ToArray();

        public void LoadFromSettings(SettingCommon settingCommon)
        {
            var oldAccounts = this.accounts;
            var newAccounts = new Dictionary<Guid, Twitter>();

            foreach (var accountSettings in settingCommon.UserAccounts)
            {
                var accountKey = accountSettings.UniqueKey;
                if (!oldAccounts.TryGetValue(accountKey, out var account))
                    account = new(new());

                var credential = accountSettings.GetTwitterCredential();
                account.Initialize(credential, accountSettings.Username, accountSettings.UserId);

                account.RestrictFavCheck = settingCommon.RestrictFavCheck;
                account.ReadOwnPost = settingCommon.ReadOwnPost;

                newAccounts[accountKey] = account;
            }

            this.accounts = newAccounts;
            this.primaryId = settingCommon.SelectedAccountKey;

            var removedAccounts = oldAccounts
                .Where(x => !newAccounts.ContainsKey(x.Key))
                .Select(x => x.Value);

            this.DisposeAccounts(removedAccounts);
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.emptyAccount.Dispose();
            this.DisposeAccounts(this.accounts.Values);

            this.IsDisposed = true;
        }

        private void DisposeAccounts(IEnumerable<Twitter> accounts)
        {
            foreach (var account in accounts)
                account.Dispose();
        }
    }
}
