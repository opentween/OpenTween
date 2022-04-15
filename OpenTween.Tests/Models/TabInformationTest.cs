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

namespace OpenTween.Models
{
    public class TabInformationTest
    {
        private readonly TabInformations tabinfo;

        public TabInformationTest()
        {
            this.tabinfo = this.CreateInstance();

            // TabInformation.GetInstance() で取得できるようにする
            var field = typeof(TabInformations).GetField("Instance",
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetField);
            field.SetValue(null, this.tabinfo);

            // 標準のタブを追加
            this.tabinfo.AddTab(new HomeTabModel("Recent"));
            this.tabinfo.AddTab(new MentionsTabModel("Reply"));
            this.tabinfo.AddTab(new DirectMessagesTabModel("DM"));
            this.tabinfo.AddTab(new FavoritesTabModel("Favorites"));
        }

        private TabInformations CreateInstance()
            => (TabInformations)Activator.CreateInstance(typeof(TabInformations), true);

        [Fact]
        public void AddTab_Test()
        {
            var tab = new FilterTabModel("MyTab");
            var ret = this.tabinfo.AddTab(tab);

            Assert.True(ret);
            Assert.Same(tab, this.tabinfo.Tabs.Last());
        }

        [Fact]
        public void AddTab_DuplicateTest()
        {
            var ret = this.tabinfo.AddTab(new FilterTabModel("Recent"));

            Assert.False(ret);
        }

        [Fact]
        public void RenameTab_PositionTest()
        {
            var replyTab = this.tabinfo.Tabs["Reply"];
            Assert.Equal(1, this.tabinfo.Tabs.IndexOf(replyTab));

            this.tabinfo.RenameTab("Reply", "Reply12345");

            Assert.Equal("Reply12345", replyTab.TabName);
            Assert.Equal(1, this.tabinfo.Tabs.IndexOf(replyTab));
        }

        [Fact]
        public void RenameTab_SelectedTabTest()
        {
            var replyTab = this.tabinfo.Tabs["Reply"];

            this.tabinfo.SelectTab("Reply");
            this.tabinfo.RenameTab("Reply", "Reply12345");

            Assert.Equal("Reply12345", replyTab.TabName);
            Assert.Equal("Reply12345", this.tabinfo.SelectedTabName);
            Assert.Equal(replyTab, this.tabinfo.SelectedTab);
        }

        [Fact]
        public void SelectTab_Test()
        {
            this.tabinfo.SelectTab("Reply");

            Assert.Equal("Reply", this.tabinfo.SelectedTabName);
            Assert.IsType<MentionsTabModel>(this.tabinfo.SelectedTab);
        }

        [Fact]
        public void SelectTab_NotExistTest()
            => Assert.Throws<ArgumentException>(() => this.tabinfo.SelectTab("INVALID"));

        [Theory]
        [InlineData(MyCommon.TabUsageType.Home, typeof(HomeTabModel))]
        [InlineData(MyCommon.TabUsageType.Mentions, typeof(MentionsTabModel))]
        [InlineData(MyCommon.TabUsageType.DirectMessage, typeof(DirectMessagesTabModel))]
        [InlineData(MyCommon.TabUsageType.Favorites, typeof(FavoritesTabModel))]
        [InlineData(MyCommon.TabUsageType.UserDefined, typeof(FilterTabModel))]
        [InlineData(MyCommon.TabUsageType.UserTimeline, typeof(UserTimelineTabModel))]
        [InlineData(MyCommon.TabUsageType.PublicSearch, typeof(PublicSearchTabModel))]
        [InlineData(MyCommon.TabUsageType.Lists, typeof(ListTimelineTabModel))]
        [InlineData(MyCommon.TabUsageType.Mute, typeof(MuteTabModel))]
        public void CreateTabFromSettings_TabTypeTest(MyCommon.TabUsageType tabType, Type expected)
        {
            var tabSetting = new SettingTabs.SettingTabItem
            {
                TabName = "tab",
                TabType = tabType,
            };
            var tabinfo = this.CreateInstance();
            var tab = tabinfo.CreateTabFromSettings(tabSetting);
            Assert.IsType(expected, tab);
        }

        [Fact]
        public void CreateTabFromSettings_FilterTabTest()
        {
            var tabSetting = new SettingTabs.SettingTabItem
            {
                TabName = "tab",
                TabType = MyCommon.TabUsageType.UserDefined,
                FilterArray = new PostFilterRule[]
                {
                    new() { FilterName = "foo" },
                },
            };
            var tabinfo = this.CreateInstance();
            var tab = tabinfo.CreateTabFromSettings(tabSetting);
            Assert.IsType<FilterTabModel>(tab);

            var filterTab = (FilterTabModel)tab!;
            Assert.Equal("foo", filterTab.FilterArray.First().FilterName);
        }

        [Fact]
        public void CreateTabFromSettings_PublicSearchTabTest()
        {
            var tabSetting = new SettingTabs.SettingTabItem
            {
                TabName = "tab",
                TabType = MyCommon.TabUsageType.PublicSearch,
                SearchWords = "foo",
                SearchLang = "ja",
            };
            var tabinfo = this.CreateInstance();
            var tab = tabinfo.CreateTabFromSettings(tabSetting);
            Assert.IsType<PublicSearchTabModel>(tab);

            var searchTab = (PublicSearchTabModel)tab!;
            Assert.Equal("foo", searchTab.SearchWords);
            Assert.Equal("ja", searchTab.SearchLang);
        }

        [Fact]
        public void AddDefaultTabs_Test()
        {
            var tabinfo = this.CreateInstance();
            Assert.Equal(0, tabinfo.Tabs.Count);

            tabinfo.AddDefaultTabs();

            Assert.Equal(4, tabinfo.Tabs.Count);
            Assert.IsType<HomeTabModel>(tabinfo.Tabs[0]);
            Assert.IsType<MentionsTabModel>(tabinfo.Tabs[1]);
            Assert.IsType<DirectMessagesTabModel>(tabinfo.Tabs[2]);
            Assert.IsType<FavoritesTabModel>(tabinfo.Tabs[3]);

            var homeTab = tabinfo.HomeTab;
            Assert.False(homeTab.Notify);

            var mentionsTab = tabinfo.MentionTab;
            Assert.True(mentionsTab.Notify);

            var dmTab = tabinfo.DirectMessageTab;
            Assert.True(dmTab.Notify);

            var favsTab = tabinfo.FavoriteTab;
            Assert.False(favsTab.Notify);
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
            this.tabinfo.AddTab(new FilterTabModel("NewTab"));
            this.tabinfo.AddTab(new FilterTabModel("NewTab2"));

            var baseTabName = "NewTab";
            Assert.Equal("NewTab3", this.tabinfo.MakeTabName(baseTabName, 5));
        }

        [Fact]
        public void MakeTabName_RetryErrorTest()
        {
            this.tabinfo.AddTab(new FilterTabModel("NewTab"));
            this.tabinfo.AddTab(new FilterTabModel("NewTab2"));
            this.tabinfo.AddTab(new FilterTabModel("NewTab3"));
            this.tabinfo.AddTab(new FilterTabModel("NewTab4"));
            this.tabinfo.AddTab(new FilterTabModel("NewTab5"));

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
            Assert.True(this.tabinfo.IsMuted(post, isHomeTimeline: true));
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
            Assert.False(this.tabinfo.IsMuted(post, isHomeTimeline: true));
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
            Assert.True(this.tabinfo.IsMuted(post, isHomeTimeline: true));
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
            Assert.False(this.tabinfo.IsMuted(post, isHomeTimeline: true));
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
            Assert.False(this.tabinfo.IsMuted(post, isHomeTimeline: true));
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
            };
            Assert.False(this.tabinfo.IsMuted(post, isHomeTimeline: false));
        }

        [Fact]
        public void IsMuted_MuteTabRulesTest()
        {
            this.tabinfo.MuteUserIds = new HashSet<long> { };

            var muteTab = new MuteTabModel();
            muteTab.AddFilter(new PostFilterRule
            {
                FilterName = "foo",
                MoveMatches = true,
            });
            this.tabinfo.AddTab(muteTab);

            var post = new PostClass
            {
                UserId = 12345L,
                ScreenName = "foo",
                Text = "hogehoge",
            };
            Assert.True(this.tabinfo.IsMuted(post, isHomeTimeline: true));
        }

        [Fact]
        public void IsMuted_MuteTabRules_NotInHomeTimelineTest()
        {
            this.tabinfo.MuteUserIds = new HashSet<long> { };

            var muteTab = new MuteTabModel();
            muteTab.AddFilter(new PostFilterRule
            {
                FilterName = "foo",
                MoveMatches = true,
            });
            this.tabinfo.AddTab(muteTab);

            // ミュートタブによるミュートはリプライも対象とする
            var post = new PostClass
            {
                UserId = 12345L,
                ScreenName = "foo",
                Text = "@hoge hogehoge",
                IsReply = true,
            };
            Assert.True(this.tabinfo.IsMuted(post, isHomeTimeline: false));
        }

        [Fact]
        public void SetReadAllTab_MarkAsReadTest()
        {
            var tab1 = new PublicSearchTabModel("search1");
            var tab2 = new PublicSearchTabModel("search2");

            this.tabinfo.AddTab(tab1);
            this.tabinfo.AddTab(tab2);

            // search1 に追加するツイート (StatusId: 100, 150, 200; すべて未読)
            tab1.UnreadManage = true;
            tab1.AddPostQueue(new PostClass { StatusId = 100L, IsRead = false });
            tab1.AddPostQueue(new PostClass { StatusId = 150L, IsRead = false });
            tab1.AddPostQueue(new PostClass { StatusId = 200L, IsRead = false });

            // search2 に追加するツイート (StatusId: 150, 200, 250; すべて未読)
            tab2.UnreadManage = true;
            tab2.AddPostQueue(new PostClass { StatusId = 150L, IsRead = false });
            tab2.AddPostQueue(new PostClass { StatusId = 200L, IsRead = false });
            tab2.AddPostQueue(new PostClass { StatusId = 250L, IsRead = false });

            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

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
            var tab1 = new PublicSearchTabModel("search1");
            var tab2 = new PublicSearchTabModel("search2");

            this.tabinfo.AddTab(tab1);
            this.tabinfo.AddTab(tab2);

            // search1 に追加するツイート (StatusId: 100, 150, 200; すべて既読)
            tab1.UnreadManage = true;
            tab1.AddPostQueue(new PostClass { StatusId = 100L, IsRead = true });
            tab1.AddPostQueue(new PostClass { StatusId = 150L, IsRead = true });
            tab1.AddPostQueue(new PostClass { StatusId = 200L, IsRead = true });

            // search2 に追加するツイート (StatusId: 150, 200, 250; すべて既読)
            tab2.UnreadManage = true;
            tab2.AddPostQueue(new PostClass { StatusId = 150L, IsRead = true });
            tab2.AddPostQueue(new PostClass { StatusId = 200L, IsRead = true });
            tab2.AddPostQueue(new PostClass { StatusId = 250L, IsRead = true });

            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

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
            this.tabinfo.AddPost(new PostClass { StatusId = 100L, IsRead = false });
            this.tabinfo.AddPost(new PostClass { StatusId = 150L, IsRead = false });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, IsRead = false });

            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

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
            this.tabinfo.AddPost(new PostClass { StatusId = 100L, IsRead = false });
            this.tabinfo.AddPost(new PostClass { StatusId = 150L, IsRead = false, IsReply = true });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, IsRead = false });

            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            // この時点でのHomeタブの未読件数
            Assert.Equal(3, homeTab.UnreadCount);

            // Recent タブのツイートをすべて未読にする
            this.tabinfo.SetReadHomeTab();

            // リプライである StatusId: 150 を除いてすべて未読になっている
            Assert.Equal(1, homeTab.UnreadCount);
            Assert.Equal(150L, homeTab.NextUnreadId);
        }

        [Fact]
        public void SetReadHomeTab_ContainsFilterHitTest()
        {
            var homeTab = this.tabinfo.Tabs["Recent"];

            // Recent に追加するツイート (StatusId: 100, 150, 200; すべて未読)
            homeTab.UnreadManage = true;
            this.tabinfo.AddPost(new PostClass { StatusId = 100L, IsRead = false });
            this.tabinfo.AddPost(new PostClass { StatusId = 150L, IsRead = false });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, IsRead = false });

            // StatusId: 150 だけ FilterTab の振り分けルールにヒットする (PostClass.FilterHit が true になる)
            var filterTab = new FilterTabModel("FilterTab");
            filterTab.AddFilter(TestPostFilterRule.Create(x =>
                x.StatusId == 150L ? MyCommon.HITRESULT.Copy : MyCommon.HITRESULT.None));
            this.tabinfo.AddTab(filterTab);

            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            // この時点でのHomeタブの未読件数
            Assert.Equal(3, homeTab.UnreadCount);

            // Recent タブのツイートをすべて未読にする
            this.tabinfo.SetReadHomeTab();

            // FilterHit が true である StatusId: 150 を除いてすべて未読になっている
            Assert.Equal(1, homeTab.UnreadCount);
            Assert.Equal(150L, homeTab.NextUnreadId);
        }

        [Fact]
        public void SubmitUpdate_RemoveSubmit_Test()
        {
            var homeTab = this.tabinfo.HomeTab;

            this.tabinfo.AddPost(new PostClass { StatusId = 100L });
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            Assert.Equal(1, homeTab.AllCount);

            this.tabinfo.RemovePostFromAllTabs(100L, setIsDeleted: true);

            // この時点ではまだ削除されない
            Assert.Equal(1, homeTab.AllCount);

            this.tabinfo.SubmitUpdate(out _, out _, out _, out var isDeletePost);

            Assert.True(isDeletePost);
            Assert.Equal(0, homeTab.AllCount);
            Assert.False(this.tabinfo.Posts.ContainsKey(100L));
        }

        [Fact]
        public void SubmitUpdate_RemoveSubmit_NotOrphaned_Test()
        {
            var homeTab = this.tabinfo.HomeTab;
            var favTab = this.tabinfo.FavoriteTab;

            this.tabinfo.AddPost(new PostClass { StatusId = 100L, IsFav = true });
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            Assert.Equal(1, homeTab.AllCount);
            Assert.Equal(1, favTab.AllCount);

            // favTab のみ発言を除去 (homeTab には残ったまま)
            favTab.EnqueueRemovePost(100L, setIsDeleted: false);

            // この時点ではまだ削除されない
            Assert.Equal(1, homeTab.AllCount);
            Assert.Equal(1, favTab.AllCount);

            this.tabinfo.SubmitUpdate(out _, out _, out _, out var isDeletePost);

            Assert.True(isDeletePost);
            Assert.Equal(1, homeTab.AllCount);
            Assert.Equal(0, favTab.AllCount);

            // homeTab には発言が残っているので Posts からは削除されない
            Assert.True(this.tabinfo.Posts.ContainsKey(100L));
        }

        [Fact]
        public void SubmitUpdate_NotifyPriorityTest()
        {
            var homeTab = this.tabinfo.HomeTab;
            homeTab.UnreadManage = true;
            homeTab.Notify = true;
            homeTab.SoundFile = "home.wav";

            var replyTab = this.tabinfo.MentionTab;
            replyTab.UnreadManage = true;
            replyTab.Notify = true;
            replyTab.SoundFile = "reply.wav";

            var dmTab = this.tabinfo.DirectMessageTab;
            dmTab.UnreadManage = true;
            dmTab.Notify = true;
            dmTab.SoundFile = "dm.wav";

            // 通常ツイート
            this.tabinfo.AddPost(new PostClass { StatusId = 100L, IsRead = false });

            // リプライ
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, IsReply = true, IsRead = false });

            // DM
            dmTab.AddPostQueue(new PostClass { StatusId = 300L, IsDm = true, IsRead = false });

            this.tabinfo.DistributePosts();

            this.tabinfo.SubmitUpdate(out var soundFile, out var notifyPosts, out _, out _);

            // DM が最も優先度が高いため DM の通知音が再生される
            Assert.Equal("dm.wav", soundFile);

            // 通知対象のツイートは 3 件
            Assert.Equal(3, notifyPosts.Length);
        }

        [Fact]
        public void SubmitUpdate_IgnoreEmptySoundPath_Test()
        {
            var homeTab = this.tabinfo.HomeTab;
            homeTab.UnreadManage = true;
            homeTab.Notify = true;
            homeTab.SoundFile = "home.wav";

            var replyTab = this.tabinfo.MentionTab;
            replyTab.UnreadManage = true;
            replyTab.Notify = true;
            replyTab.SoundFile = "";

            // 通常ツイート
            this.tabinfo.AddPost(new PostClass { StatusId = 100L, IsRead = false });

            // リプライ
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, IsReply = true, IsRead = false });

            this.tabinfo.DistributePosts();

            this.tabinfo.SubmitUpdate(out var soundFile, out var notifyPosts, out _, out _);

            // リプライの方が通知音の優先度が高いが、replyTab.SoundFile が空文字列なので次点の Recent の通知音を鳴らす
            Assert.Equal("home.wav", soundFile);

            // 通知対象のツイートは 2 件
            Assert.Equal(2, notifyPosts.Length);
        }

        [Fact]
        public void FilterAll_CopyFilterTest()
        {
            var homeTab = this.tabinfo.HomeTab;

            var myTab1 = new FilterTabModel("MyTab1");
            this.tabinfo.AddTab(myTab1);

            var filter = new PostFilterRule
            {
                FilterName = "aaa",

                // コピーのみ
                MoveMatches = false,
                MarkMatches = false,
            };
            myTab1.AddFilter(filter);
            myTab1.FilterModified = false;

            this.tabinfo.AddPost(new PostClass { StatusId = 100L, ScreenName = "aaa" });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, ScreenName = "bbb" });
            this.tabinfo.AddPost(new PostClass { StatusId = 300L, ScreenName = "ccc" });
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            // この時点での振り分け状態
            Assert.Equal(new[] { 100L, 200L, 300L }, homeTab.StatusIds, AnyOrderComparer<long>.Instance);
            Assert.Equal(new[] { 100L }, myTab1.StatusIds);

            // フィルタを変更する
            filter.FilterName = "bbb";

            // フィルタの変更を反映
            this.tabinfo.FilterAll();
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            // 期待する動作:
            //   [statusId: 100] は MyTab1 から取り除かれる
            //   [statusId: 200] は Recent から MyTab1 にコピーされる

            // 変更後の振り分け状態
            Assert.Equal(new[] { 100L, 200L, 300L }, homeTab.StatusIds, AnyOrderComparer<long>.Instance);
            Assert.Equal(new[] { 200L }, myTab1.StatusIds);
        }

        [Fact]
        public void FilterAll_CopyAndMarkFilterTest()
        {
            var homeTab = this.tabinfo.HomeTab;

            var myTab1 = new FilterTabModel("MyTab1");
            this.tabinfo.AddTab(myTab1);

            var filter = new PostFilterRule
            {
                FilterName = "aaa",

                // コピー+マーク
                MoveMatches = false,
                MarkMatches = true,
            };
            myTab1.AddFilter(filter);
            myTab1.FilterModified = false;

            this.tabinfo.AddPost(new PostClass { StatusId = 100L, ScreenName = "aaa" });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, ScreenName = "bbb" });
            this.tabinfo.AddPost(new PostClass { StatusId = 300L, ScreenName = "ccc" });
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            // この時点での振り分け状態
            Assert.Equal(new[] { 100L, 200L, 300L }, homeTab.StatusIds, AnyOrderComparer<long>.Instance);
            Assert.Equal(new[] { 100L }, myTab1.StatusIds);

            // フィルタを変更する
            filter.FilterName = "bbb";

            // フィルタの変更を反映
            this.tabinfo.FilterAll();
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            // 期待する動作:
            //   [statusId: 100] は MyTab1 から取り除かれる
            //   [statusId: 200] は Recent から MyTab1 にコピーされ、マークが付与される

            // 変更後の振り分け状態
            Assert.Equal(new[] { 100L, 200L, 300L }, homeTab.StatusIds, AnyOrderComparer<long>.Instance);
            Assert.Equal(new[] { 200L }, myTab1.StatusIds);

            // [statusId: 200] は IsMark が true の状態になる
            Assert.True(this.tabinfo[200L]!.IsMark);
        }

        [Fact]
        public void FilterAll_MoveFilterTest()
        {
            var homeTab = this.tabinfo.HomeTab;

            var myTab1 = new FilterTabModel("MyTab1");
            this.tabinfo.AddTab(myTab1);

            var filter = new PostFilterRule
            {
                FilterName = "aaa",

                // マッチしたら移動
                MoveMatches = true,
            };
            myTab1.AddFilter(filter);
            myTab1.FilterModified = false;

            this.tabinfo.AddPost(new PostClass { StatusId = 100L, ScreenName = "aaa" });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, ScreenName = "bbb" });
            this.tabinfo.AddPost(new PostClass { StatusId = 300L, ScreenName = "ccc" });
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            // この時点での振り分け状態
            Assert.Equal(new[] { 200L, 300L }, homeTab.StatusIds, AnyOrderComparer<long>.Instance);
            Assert.Equal(new[] { 100L }, myTab1.StatusIds);

            // フィルタを変更する
            filter.FilterName = "bbb";

            // フィルタの変更を反映
            this.tabinfo.FilterAll();
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            // 期待する動作:
            //   [statusId: 100] は MyTab1 から取り除かれて Recent に戻される
            //   [statusId: 200] は Recent から MyTab1 に移動される

            // 変更後の振り分け状態
            Assert.Equal(new[] { 100L, 300L }, homeTab.StatusIds, AnyOrderComparer<long>.Instance);
            Assert.Equal(new[] { 200L }, myTab1.StatusIds);
        }

        [Fact]
        public void FilterAll_MoveFilterTest2()
        {
            var homeTab = this.tabinfo.HomeTab;

            var myTab1 = new FilterTabModel("MyTab1");
            var myTab2 = new FilterTabModel("MyTab2");
            this.tabinfo.AddTab(myTab1);
            this.tabinfo.AddTab(myTab2);

            var filter1 = new PostFilterRule
            {
                FilterName = "aaa",

                // マッチしたら移動
                MoveMatches = true,
            };
            myTab1.AddFilter(filter1);
            myTab1.FilterModified = false;

            var filter2 = new PostFilterRule
            {
                FilterName = "bbb",

                // マッチしたら移動
                MoveMatches = true,
            };
            myTab2.AddFilter(filter2);
            myTab2.FilterModified = false;

            this.tabinfo.AddPost(new PostClass { StatusId = 100L, ScreenName = "aaa" });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, ScreenName = "bbb" });
            this.tabinfo.AddPost(new PostClass { StatusId = 300L, ScreenName = "ccc" });
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            // この時点での振り分け状態
            Assert.Equal(new[] { 300L }, homeTab.StatusIds);
            Assert.Equal(new[] { 100L }, myTab1.StatusIds);
            Assert.Equal(new[] { 200L }, myTab2.StatusIds);

            // MyTab1 のフィルタを変更する
            filter1.FilterName = "bbb";

            // MyTab2 のフィルタを変更する
            filter2.FilterName = "ccc";

            // フィルタの変更を反映
            this.tabinfo.FilterAll();
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            // 期待する動作:
            //   [statusId: 100] は MyTab1 から取り除かれて Recent に戻される
            //   [statusId: 200] は MyTab1 に移動される
            //   [statusId: 200] は MyTab2 から取り除かれるが MyTab1 に移動されているので Recent には戻さない
            //   [statusId: 300] は Recent から MyTab2 に移動される

            // 変更後の振り分け状態
            Assert.Equal(new[] { 100L }, homeTab.StatusIds);
            Assert.Equal(new[] { 200L }, myTab1.StatusIds);
            Assert.Equal(new[] { 300L }, myTab2.StatusIds);
        }

        [Fact]
        public void FilterAll_ExcludeReplyFilterTest()
        {
            var homeTab = this.tabinfo.HomeTab;
            var replyTab = this.tabinfo.MentionTab;

            var filter = new PostFilterRule
            {
                // @aaa からのリプライは Reply タブに振り分けない
                ExFilterName = "aaa",
            };
            replyTab.AddFilter(filter);
            replyTab.FilterModified = false;

            this.tabinfo.AddPost(new PostClass { StatusId = 100L, ScreenName = "aaa", IsReply = true });
            this.tabinfo.AddPost(new PostClass { StatusId = 200L, ScreenName = "bbb", IsReply = true });
            this.tabinfo.AddPost(new PostClass { StatusId = 300L, ScreenName = "ccc", IsReply = true });
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            // この時点での振り分け状態
            Assert.Equal(new[] { 100L, 200L, 300L }, homeTab.StatusIds, AnyOrderComparer<long>.Instance);
            Assert.Equal(new[] { 200L, 300L }, replyTab.StatusIds, AnyOrderComparer<long>.Instance);

            // [statusId: 100] は IsExcludeReply が true の状態になっている
            Assert.True(this.tabinfo[100L]!.IsExcludeReply);

            // Reply のフィルタを変更する
            filter.ExFilterName = "bbb";

            // フィルタの変更を反映
            this.tabinfo.FilterAll();
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            // 期待する動作:
            //   [statusId: 100] は Reply にコピーされ、IsExcludeReply が false になる
            //   [statusId: 200] は Reply から取り除かれ、IsExcludeReply が true になる

            // 変更後の振り分け状態
            Assert.Equal(new[] { 100L, 200L, 300L }, homeTab.StatusIds, AnyOrderComparer<long>.Instance);
            Assert.Equal(new[] { 100L, 300L }, replyTab.StatusIds, AnyOrderComparer<long>.Instance);

            // [statusId: 100] は IsExcludeReply が false の状態になる
            Assert.False(this.tabinfo[100L]!.IsExcludeReply);

            // [statusId: 200] は IsExcludeReply が true の状態になる
            Assert.True(this.tabinfo[200L]!.IsExcludeReply);
        }

        [Fact]
        public void RefreshOwl_HomeTabTest()
        {
            var post = new PostClass
            {
                StatusId = 100L,
                ScreenName = "aaa",
                UserId = 123L,
                IsOwl = true,
            };
            this.tabinfo.AddPost(post);
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            var followerIds = new HashSet<long> { 123L };
            this.tabinfo.RefreshOwl(followerIds);

            Assert.False(post.IsOwl);
        }

        [Fact]
        public void RefreshOwl_InnerStoregeTabTest()
        {
            var tab = new PublicSearchTabModel("search");
            this.tabinfo.AddTab(tab);

            var post = new PostClass
            {
                StatusId = 100L,
                ScreenName = "aaa",
                UserId = 123L,
                IsOwl = true,
            };
            tab.AddPostQueue(post);
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            var followerIds = new HashSet<long> { 123L };
            this.tabinfo.RefreshOwl(followerIds);

            Assert.False(post.IsOwl);
        }

        [Fact]
        public void RefreshOwl_UnfollowedTest()
        {
            var post = new PostClass
            {
                StatusId = 100L,
                ScreenName = "aaa",
                UserId = 123L,
                IsOwl = false,
            };
            this.tabinfo.AddPost(post);
            this.tabinfo.DistributePosts();
            this.tabinfo.SubmitUpdate();

            var followerIds = new HashSet<long> { 456L };
            this.tabinfo.RefreshOwl(followerIds);

            Assert.True(post.IsOwl);
        }

        private class TestPostFilterRule : PostFilterRule
        {
            public static PostFilterRule Create(Func<PostClass, MyCommon.HITRESULT> filterDelegate)
            {
                return new TestPostFilterRule
                {
                    filterDelegate = filterDelegate,
                    IsDirty = false,
                };
            }
        }
    }
}
