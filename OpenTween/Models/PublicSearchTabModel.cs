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

namespace OpenTween.Models
{
    public class PublicSearchTabModel : InternalStorageTabModel
    {
        public override MyCommon.TabUsageType TabType
            => MyCommon.TabUsageType.PublicSearch;

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

        public override async Task RefreshAsync(Twitter tw, bool backward, bool startup, IProgress<string> progress)
        {
            if (MyCommon.IsNullOrEmpty(this.SearchWords))
                return;

            bool read;
            if (!SettingManager.Instance.Common.UnreadManage)
                read = true;
            else
                read = startup && SettingManager.Instance.Common.Read;

            progress.Report("Search refreshing...");

            await tw.GetSearch(read, this, backward)
                .ConfigureAwait(false);

            TabInformations.GetInstance().DistributePosts();

            progress.Report("Search refreshed");
        }
    }
}
