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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenTween.Setting;

namespace OpenTween.Models
{
    public class HomeTabModel : TabModel
    {
        public override MyCommon.TabUsageType TabType
            => MyCommon.TabUsageType.Home;

        public int TweetsPerHour => this.tweetsPerHour;

        // 流速計測用
        private int tweetsPerHour = 0;
        private readonly ConcurrentDictionary<DateTimeUtc, int> tweetsTimestamps = new();

        public HomeTabModel()
            : this(MyCommon.DEFAULTTAB.RECENT)
        {
        }

        public HomeTabModel(string tabName)
            : base(tabName)
        {
        }

        public override void AddPostQueue(PostClass post)
        {
            base.AddPostQueue(post);
            this.UpdateTimelineSpeed(post.CreatedAt);
        }

        public override async Task RefreshAsync(Twitter tw, bool backward, bool startup, IProgress<string> progress)
        {
            bool read;
            if (!SettingManager.Instance.Common.UnreadManage)
                read = true;
            else
                read = startup && SettingManager.Instance.Common.Read;

            progress.Report(string.Format(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText5, backward ? -1 : 1));

            await tw.GetHomeTimelineApi(read, this, backward, startup)
                .ConfigureAwait(false);

            // 新着時未読クリア
            if (SettingManager.Instance.Common.ReadOldPosts)
                TabInformations.GetInstance().SetReadHomeTab();

            TabInformations.GetInstance().DistributePosts();

            progress.Report(Properties.Resources.GetTimelineWorker_RunWorkerCompletedText1);
        }

        /// <summary>
        /// タイムラインに追加された発言件数を反映し、タイムラインの流速を更新します
        /// </summary>
        private void UpdateTimelineSpeed(DateTimeUtc postCreatedAt)
        {
            var now = DateTimeUtc.Now;

            // 1 時間以上前の時刻は追加しない
            var oneHour = TimeSpan.FromHours(1);
            if (now - postCreatedAt > oneHour)
                return;

            this.tweetsTimestamps.AddOrUpdate(postCreatedAt, 1, (k, v) => v + 1);

            var removeKeys = new List<DateTimeUtc>();
            var tweetsInWindow = 0;
            foreach (var (timestamp, count) in this.tweetsTimestamps)
            {
                if (now - timestamp > oneHour)
                    removeKeys.Add(timestamp);
                else
                    tweetsInWindow += count;
            }
            Interlocked.Exchange(ref this.tweetsPerHour, tweetsInWindow);

            foreach (var key in removeKeys)
                this.tweetsTimestamps.TryRemove(key, out _);
        }
    }
}
