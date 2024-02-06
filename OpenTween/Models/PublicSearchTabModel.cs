// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
//           (c) 2012      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using OpenTween.Setting;
using OpenTween.SocialProtocol;
using OpenTween.SocialProtocol.Twitter;

namespace OpenTween.Models
{
    public class PublicSearchTabModel : InternalStorageTabModel
    {
        public override MyCommon.TabUsageType TabType
            => MyCommon.TabUsageType.PublicSearch;

        public PostId? OldestId { get; set; }

        public PostId? SinceId { get; set; }

        public string? CursorTop { get; set; }

        public string? CursorBottom { get; set; }

        public string SearchWords
        {
            get => this.searchWords;
            set
            {
                this.searchWords = value;
                this.ResetFetchIds();
            }
        }

        public string SearchLang
        {
            get => this.searchLang;
            set
            {
                this.searchLang = value;
                this.ResetFetchIds();
            }
        }

        private string searchWords = "";
        private string searchLang = "";

        public PublicSearchTabModel(string tabName)
            : base(tabName)
        {
        }

        public override async Task RefreshAsync(ISocialAccount account, bool backward, IProgress<string> progress)
        {
            if (account is not TwitterAccount twAccount)
                throw new ArgumentException($"Invalid account type: {account.GetType()}", nameof(account));

            if (MyCommon.IsNullOrEmpty(this.SearchWords))
                return;

            progress.Report("Search refreshing...");

            var firstLoad = !this.IsFirstLoadCompleted;

            await twAccount.Legacy.GetSearch(this, backward, firstLoad)
                .ConfigureAwait(false);

            TabInformations.GetInstance().DistributePosts();

            if (firstLoad)
                this.IsFirstLoadCompleted = true;

            progress.Report("Search refreshed");
        }

        /// <summary>
        /// 差分更新用の cursor をリセットする（検索条件が変更された時に使用する）
        /// </summary>
        public void ResetFetchIds()
        {
            this.SinceId = null;
            this.OldestId = null;
            this.CursorTop = null;
            this.CursorBottom = null;
            this.IsFirstLoadCompleted = false;
        }
    }
}
