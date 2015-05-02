// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class TabInformationTest
    {
        private TabInformations tabinfo;

        public TabInformationTest()
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
        public void MakeTabName_Test()
        {
            var baseTabName = "NewTab";
            Assert.Equal("NewTab", this.tabinfo.MakeTabName(baseTabName, 5));
        }

        [Fact]
        public void MakeTabName_RetryTest()
        {
            this.tabinfo.AddTab("NewTab", MyCommon.TabUsageType.UserDefined, null);
            this.tabinfo.AddTab("NewTab2", MyCommon.TabUsageType.UserDefined, null);

            var baseTabName = "NewTab";
            Assert.Equal("NewTab3", this.tabinfo.MakeTabName(baseTabName, 5));
        }

        [Fact]
        public void MakeTabName_RetryErrorTest()
        {
            this.tabinfo.AddTab("NewTab", MyCommon.TabUsageType.UserDefined, null);
            this.tabinfo.AddTab("NewTab2", MyCommon.TabUsageType.UserDefined, null);
            this.tabinfo.AddTab("NewTab3", MyCommon.TabUsageType.UserDefined, null);
            this.tabinfo.AddTab("NewTab4", MyCommon.TabUsageType.UserDefined, null);
            this.tabinfo.AddTab("NewTab5", MyCommon.TabUsageType.UserDefined, null);

            var baseTabName = "NewTab";
            Assert.Throws<TabException>(() => this.tabinfo.MakeTabName(baseTabName, 5));
        }

        [Fact]
        public void IsMuted_Test()
        {
            this.tabinfo.MuteUserIds = new HashSet<long> { 12345L };

            var post = new PostClass
            {
                UserId = 12345L,
                Text = "hogehoge",
            };
            Assert.True(this.tabinfo.IsMuted(post));
        }

        [Fact]
        public void IsMuted_NotMutingTest()
        {
            this.tabinfo.MuteUserIds = new HashSet<long> { 12345L };

            var post = new PostClass
            {
                UserId = 11111L,
                Text = "hogehoge",
            };
            Assert.False(this.tabinfo.IsMuted(post));
        }

        [Fact]
        public void IsMuted_RetweetTest()
        {
            this.tabinfo.MuteUserIds = new HashSet<long> { 12345L };

            var post = new PostClass
            {
                UserId = 11111L,
                RetweetedByUserId = 12345L,
                Text = "hogehoge",
            };
            Assert.True(this.tabinfo.IsMuted(post));
        }

        [Fact]
        public void IsMuted_RetweetNotMutingTest()
        {
            this.tabinfo.MuteUserIds = new HashSet<long> { 12345L };

            var post = new PostClass
            {
                UserId = 11111L,
                RetweetedByUserId = 22222L,
                Text = "hogehoge",
            };
            Assert.False(this.tabinfo.IsMuted(post));
        }

        [Fact]
        public void IsMuted_ReplyTest()
        {
            this.tabinfo.MuteUserIds = new HashSet<long> { 12345L };

            // ミュート対象のユーザーであってもリプライの場合は対象外とする
            var post = new PostClass
            {
                UserId = 12345L,
                Text = "@foo hogehoge",
                IsReply = true,
            };
            Assert.False(this.tabinfo.IsMuted(post));
        }

        [Fact]
        public void IsMuted_NotInHomeTimelineTest()
        {
            this.tabinfo.MuteUserIds = new HashSet<long> { 12345L };

            // Recent以外のタブ（検索など）の場合は対象外とする
            var post = new PostClass
            {
                UserId = 12345L,
                Text = "hogehoge",
                RelTabName = "Search",
            };
            Assert.False(this.tabinfo.IsMuted(post));
        }

        [Fact]
        public void IsMuted_MuteTabRulesTest()
        {
            this.tabinfo.MuteUserIds = new HashSet<long> { };

            this.tabinfo.AddTab("Mute", MyCommon.TabUsageType.Mute, null);
            var muteTab = this.tabinfo.Tabs["Mute"];
            muteTab.AddFilter(new PostFilterRule
            {
                FilterName = "foo",
                MoveMatches = true,
            });

            var post = new PostClass
            {
                UserId = 12345L,
                ScreenName = "foo",
                Text = "hogehoge",
            };
            Assert.True(this.tabinfo.IsMuted(post));
        }

        [Fact]
        public void IsMuted_MuteTabRules_NotInHomeTimelineTest()
        {
            this.tabinfo.MuteUserIds = new HashSet<long> { };

            this.tabinfo.AddTab("Mute", MyCommon.TabUsageType.Mute, null);
            var muteTab = this.tabinfo.Tabs["Mute"];
            muteTab.AddFilter(new PostFilterRule
            {
                FilterName = "foo",
                MoveMatches = true,
            });

            // ミュートタブによるミュートは Recent 以外のタブも対象とする
            var post = new PostClass
            {
                UserId = 12345L,
                ScreenName = "foo",
                Text = "hogehoge",
                RelTabName = "Search",
            };
            Assert.True(this.tabinfo.IsMuted(post));
        }

        [Fact]
        public void SetReadAllTab_MarkAsReadTest()
        {
            this.tabinfo.AddTab("search1", MyCommon.TabUsageType.PublicSearch, null);
            this.tabinfo.AddTab("search2", MyCommon.TabUsageType.PublicSearch, null);

            var tab1 = this.tabinfo.Tabs["search1"];
            var tab2 = this.tabinfo.Tabs["search2"];

            // search1 に追加するツイート (StatusId: 100, 150, 200; すべて未読)
            tab1.UnreadManage = true;
            this.tabinfo.AddPost(new PostClass { StatusId = 100L, IsRead = false, RelTabName = "search1" });
            this.tabinfo.AddPost(new PostClass { StatusId = 150L, IsRead = false, RelTabName = "search1" });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, IsRead = false, RelTabName = "search1" });

            // search2 に追加するツイート (StatusId: 150, 200, 250; すべて未読)
            tab2.UnreadManage = true;
            this.tabinfo.AddPost(new PostClass { StatusId = 150L, IsRead = false, RelTabName = "search2" });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, IsRead = false, RelTabName = "search2" });
            this.tabinfo.AddPost(new PostClass { StatusId = 250L, IsRead = false, RelTabName = "search2" });

            this.tabinfo.DistributePosts();

            string soundFile = null;
            PostClass[] notifyPosts = null;
            bool isMentionIncluded = false, isDeletePost = false;
            this.tabinfo.SubmitUpdate(ref soundFile, ref notifyPosts, ref isMentionIncluded, ref isDeletePost, false);

            // この時点での各タブの未読件数
            Assert.Equal(3, tab1.UnreadCount);
            Assert.Equal(3, tab2.UnreadCount);

            // ... ここまで長い前置き

            // StatusId: 200 を既読にする (search1, search2 両方に含まれる)
            this.tabinfo.SetReadAllTab(200L, read: true);
            Assert.Equal(2, tab1.UnreadCount);
            Assert.Equal(2, tab2.UnreadCount);

            // StatusId: 100 を既読にする (search1 のみに含まれる)
            this.tabinfo.SetReadAllTab(100L, read: true);
            Assert.Equal(1, tab1.UnreadCount);
            Assert.Equal(2, tab2.UnreadCount);
        }

        [Fact]
        public void SetReadAllTab_MarkAsUnreadTest()
        {
            this.tabinfo.AddTab("search1", MyCommon.TabUsageType.PublicSearch, null);
            this.tabinfo.AddTab("search2", MyCommon.TabUsageType.PublicSearch, null);

            var tab1 = this.tabinfo.Tabs["search1"];
            var tab2 = this.tabinfo.Tabs["search2"];

            // search1 に追加するツイート (StatusId: 100, 150, 200; すべて既読)
            tab1.UnreadManage = true;
            this.tabinfo.AddPost(new PostClass { StatusId = 100L, IsRead = true, RelTabName = "search1" });
            this.tabinfo.AddPost(new PostClass { StatusId = 150L, IsRead = true, RelTabName = "search1" });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, IsRead = true, RelTabName = "search1" });

            // search2 に追加するツイート (StatusId: 150, 200, 250; すべて既読)
            tab2.UnreadManage = true;
            this.tabinfo.AddPost(new PostClass { StatusId = 150L, IsRead = true, RelTabName = "search2" });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, IsRead = true, RelTabName = "search2" });
            this.tabinfo.AddPost(new PostClass { StatusId = 250L, IsRead = true, RelTabName = "search2" });

            this.tabinfo.DistributePosts();

            string soundFile = null;
            PostClass[] notifyPosts = null;
            bool isMentionIncluded = false, isDeletePost = false;
            this.tabinfo.SubmitUpdate(ref soundFile, ref notifyPosts, ref isMentionIncluded, ref isDeletePost, false);

            // この時点での各タブの未読件数
            Assert.Equal(0, tab1.UnreadCount);
            Assert.Equal(0, tab2.UnreadCount);

            // ... ここまで長い前置き

            // StatusId: 200 を未読にする (search1, search2 両方に含まれる)
            this.tabinfo.SetReadAllTab(200L, read: false);
            Assert.Equal(1, tab1.UnreadCount);
            Assert.Equal(1, tab2.UnreadCount);

            // StatusId: 100 を未読にする (search1 のみに含まれる)
            this.tabinfo.SetReadAllTab(100L, read: false);
            Assert.Equal(2, tab1.UnreadCount);
            Assert.Equal(1, tab2.UnreadCount);
        }

        [Fact]
        public void SetReadHomeTab_Test()
        {
            var homeTab = this.tabinfo.Tabs["Recent"];

            // Recent に追加するツイート (StatusId: 100, 150, 200; すべて未読)
            homeTab.UnreadManage = true;
            this.tabinfo.AddPost(new PostClass { StatusId = 100L, IsRead = false, RelTabName = "" });
            this.tabinfo.AddPost(new PostClass { StatusId = 150L, IsRead = false, RelTabName = "" });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, IsRead = false, RelTabName = "" });

            this.tabinfo.DistributePosts();

            string soundFile = null;
            PostClass[] notifyPosts = null;
            bool isMentionIncluded = false, isDeletePost = false;
            this.tabinfo.SubmitUpdate(ref soundFile, ref notifyPosts, ref isMentionIncluded, ref isDeletePost, false);

            // この時点でのHomeタブの未読件数
            Assert.Equal(3, homeTab.UnreadCount);

            // Recent タブのツイートをすべて未読にする
            this.tabinfo.SetReadHomeTab();
            Assert.Equal(0, homeTab.UnreadCount);
        }

        [Fact]
        public void SetReadHomeTab_ContainsReplyTest()
        {
            var homeTab = this.tabinfo.Tabs["Recent"];

            // Recent に追加するツイート (StatusId: 100, 150, 200; すべて未読)
            // StatusId: 150 は未読だがリプライ属性が付いている
            homeTab.UnreadManage = true;
            this.tabinfo.AddPost(new PostClass { StatusId = 100L, IsRead = false, RelTabName = "" });
            this.tabinfo.AddPost(new PostClass { StatusId = 150L, IsRead = false, RelTabName = "", IsReply = true });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, IsRead = false, RelTabName = "" });

            this.tabinfo.DistributePosts();

            string soundFile = null;
            PostClass[] notifyPosts = null;
            bool isMentionIncluded = false, isDeletePost = false;
            this.tabinfo.SubmitUpdate(ref soundFile, ref notifyPosts, ref isMentionIncluded, ref isDeletePost, false);

            // この時点でのHomeタブの未読件数
            Assert.Equal(3, homeTab.UnreadCount);

            // Recent タブのツイートをすべて未読にする
            this.tabinfo.SetReadHomeTab();

            // リプライである StatusId: 150 を除いてすべて未読になっている
            Assert.Equal(1, homeTab.UnreadCount);
            Assert.Equal(150L, homeTab.OldestUnreadId);
        }

        [Fact]
        public void SetReadHomeTab_ContainsFilterHitTest()
        {
            var homeTab = this.tabinfo.Tabs["Recent"];

            // Recent に追加するツイート (StatusId: 100, 150, 200; すべて未読)
            homeTab.UnreadManage = true;
            this.tabinfo.AddPost(new PostClass { StatusId = 100L, IsRead = false, RelTabName = "" });
            this.tabinfo.AddPost(new PostClass { StatusId = 150L, IsRead = false, RelTabName = "" });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, IsRead = false, RelTabName = "" });

            // StatusId: 150 だけ FilterTab の振り分けルールにヒットする (PostClass.FilterHit が true になる)
            this.tabinfo.AddTab("FilterTab", MyCommon.TabUsageType.UserDefined, null);
            var filterTab = this.tabinfo.Tabs["FilterTab"];
            filterTab.AddFilter(TestPostFilterRule.Create(x =>
                x.StatusId == 150L ? MyCommon.HITRESULT.Copy : MyCommon.HITRESULT.None));

            this.tabinfo.DistributePosts();

            string soundFile = null;
            PostClass[] notifyPosts = null;
            bool isMentionIncluded = false, isDeletePost = false;
            this.tabinfo.SubmitUpdate(ref soundFile, ref notifyPosts, ref isMentionIncluded, ref isDeletePost, false);

            // この時点でのHomeタブの未読件数
            Assert.Equal(3, homeTab.UnreadCount);

            // Recent タブのツイートをすべて未読にする
            this.tabinfo.SetReadHomeTab();

            // FilterHit が true である StatusId: 150 を除いてすべて未読になっている
            Assert.Equal(1, homeTab.UnreadCount);
            Assert.Equal(150L, homeTab.OldestUnreadId);
        }

        class TestPostFilterRule : PostFilterRule
        {
            public static PostFilterRule Create(Func<PostClass, MyCommon.HITRESULT> filterDelegate)
            {
                return new TestPostFilterRule
                {
                    FilterDelegate = filterDelegate,
                    IsDirty = false,
                };
            }
        }
    }
}
