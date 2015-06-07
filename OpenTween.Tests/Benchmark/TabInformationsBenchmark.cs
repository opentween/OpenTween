// OpenTween - Client of Twitter
// Copyright (c) 2015 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTween.Benchmark
{
    public class TabInformationsBenchmark
    {
        private TabInformations tabinfo;

        public TabInformationsBenchmark()
        {
            this.tabinfo = Activator.CreateInstance(typeof(TabInformations), true) as TabInformations;

            // TabInformation.GetInstance() で取得できるようにする
            var field = typeof(TabInformations).GetField("_instance",
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetField);
            field.SetValue(null, this.tabinfo);

            // 標準のタブを追加
            this.tabinfo.AddTab("Recent", MyCommon.TabUsageType.Home, null);
            this.tabinfo.AddTab("Reply", MyCommon.TabUsageType.Mentions, null);
            this.tabinfo.AddTab("DM", MyCommon.TabUsageType.DirectMessage, null);
            this.tabinfo.AddTab("Favorites", MyCommon.TabUsageType.Favorites, null);
        }

        [Fact]
        [Trait("Type", "Benchmark")]
        public void DistributeBenchmark_MoveMatches()
        {
            // MyTab1: name_000 から name_099 までのスクリーンネームにマッチ
            var filtersQuery =
                from x in Enumerable.Range(0, 100)
                select new PostFilterRule
                {
                    FilterName = "name_" + x.ToString("000"),
                    MoveMatches = true,
                };
            var tab1 = AddFilterTab("MyTab1", filtersQuery.ToArray());

            // 25% の確率で MyTab1 にヒットするツイートを 500 件生成
            var randStatusId = new Random();
            var postsQuery =
                from x in Enumerable.Range(0, 500)
                select new PostClass
                {
                    StatusId = (long)randStatusId.Next() << 32 | (uint)randStatusId.Next(),
                    ScreenName = "name_" + x.ToString("000"),
                };
            // postsQuery を 100 回実行する => 50,000 件
            var posts = Enumerable.Range(0, 100).Select(x => postsQuery).SelectMany(x => x)
                .OrderBy(x => x.StatusId).ToArray();

            long addPostTime, distributeTime, updateTime;
            var watch = new Stopwatch();

            // 測定1: TabInformations.AddPost()
            watch.Start();
            foreach (var post in posts)
                this.tabinfo.AddPost(post);
            watch.Stop();
            addPostTime = watch.ElapsedMilliseconds;

            // 測定2: TabInformation.DistributePosts()
            watch.Restart();
            this.tabinfo.DistributePosts();
            watch.Stop();
            distributeTime = watch.ElapsedMilliseconds;

            // 測定3: TabInformations.SubmitUpdate()
            string _ = null;
            PostClass[] __ = null;
            bool ___ = false;
            watch.Restart();
            this.tabinfo.SubmitUpdate(ref _, ref __, ref ___, ref ___, isUserStream: false);
            watch.Stop();
            updateTime = watch.ElapsedMilliseconds;

            Assert.Equal(50000, this.tabinfo.Posts.Count);
            Assert.Equal(10000, tab1.AllCount);

            Console.WriteLine("AddPost: " + addPostTime);
            Console.WriteLine("DistributePosts: " + distributeTime);
            Console.WriteLine("SubmitUpdate: " + updateTime);
        }

        private TabClass AddFilterTab(string tabName, PostFilterRule[] filterRules)
        {
            this.tabinfo.AddTab(tabName, MyCommon.TabUsageType.UserDefined, null);

            var tab = this.tabinfo.Tabs[tabName];
            tab.FilterArray = filterRules;

            return tab;
        }
    }
}
