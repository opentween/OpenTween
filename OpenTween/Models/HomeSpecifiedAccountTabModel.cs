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
using System.Threading.Tasks;
using OpenTween.SocialProtocol;
using OpenTween.SocialProtocol.Twitter;

namespace OpenTween.Models
{
    public class HomeSpecifiedAccountTabModel : InternalStorageTabModel
    {
        public override MyCommon.TabUsageType TabType
            => MyCommon.TabUsageType.Undefined;

        public override bool IsPermanentTabType
            => false;

        public override Guid? SourceAccountId { get; }

        public PostId? OldestId { get; set; }

        public string? CursorTop { get; set; }

        public string? CursorBottom { get; set; }

        public HomeSpecifiedAccountTabModel(string tabName, Guid accountId)
            : base(tabName)
        {
            this.SourceAccountId = accountId;
        }

        public override async Task RefreshAsync(ISocialAccount account, bool backward, IProgress<string> progress)
        {
            if (account is not TwitterAccount twAccount)
                throw new ArgumentException($"Invalid account type: {account.GetType()}", nameof(account));

            progress.Report("Home refreshing...");

            var firstLoad = !this.IsFirstLoadCompleted;

            await twAccount.Legacy.GetHomeSpecifiedAccountTimelineApi(this, backward, firstLoad)
                .ConfigureAwait(false);

            TabInformations.GetInstance().DistributePosts();

            if (firstLoad)
                this.IsFirstLoadCompleted = true;

            progress.Report("Home refreshed");
        }
    }
}
