﻿// OpenTween - Client of Twitter
// Copyright (c) 2022 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details.
//
// You should have received a copy of the GNU General public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using OpenTween.Models;
using OpenTween.OpenTweenCustomControl;
using Xunit;

namespace OpenTween
{
    public class TimelineListViewCacheTest
    {
        private readonly Random random = new();

        private PostClass CreatePost()
        {
            return new()
            {
                StatusId = this.random.Next(10000),
                UserId = this.random.Next(10000),
                ScreenName = "test",
                Nickname = "てすと",
                AccessibleText = "foo",
                Source = "OpenTween",
                FavoritedCount = 0,
                CreatedAt = TestUtils.LocalTime(2022, 1, 1, 0, 0, 0),
                IsRead = true,
            };
        }

        [Fact]
        public void UpdateListSize_Test()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            Assert.Equal(0, listView.VirtualListSize);
            Assert.False(cache.IsListSizeMismatched);

            tab.AddPostQueue(this.CreatePost());
            tab.AddSubmit();

            Assert.True(cache.IsListSizeMismatched);

            cache.UpdateListSize();

            Assert.Equal(1, listView.VirtualListSize);
            Assert.False(cache.IsListSizeMismatched);
        }

        [Fact]
        public void CreateItem_Test()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var post = this.CreatePost();
            var item = cache.CreateItem(post);

            Assert.Equal("", item.SubItems[0].Text);
            Assert.Equal("てすと", item.SubItems[1].Text);
            Assert.Equal("foo", item.SubItems[2].Text);
            Assert.Equal("2022/01/01 0:00:00", item.SubItems[3].Text);
            Assert.Equal("test", item.SubItems[4].Text);
            Assert.Equal("", item.SubItems[5].Text);
            Assert.Equal("", item.SubItems[6].Text);
            Assert.Equal("OpenTween", item.SubItems[7].Text);
        }

        [Fact]
        public void CreateItem_UnreadTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var post = this.CreatePost();
            post.IsRead = false;

            var item = cache.CreateItem(post);
            Assert.Equal("★", item.SubItems[5].Text);
        }

        [Fact]
        public void CreateItem_UnreadManageDisabledTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var post = this.CreatePost();
            post.IsRead = false;

            // 未読管理が無効な場合は未読状態に関わらず未読マークを表示しない
            tab.UnreadManage = false;

            var item = cache.CreateItem(post);
            Assert.Equal("", item.SubItems[5].Text);
        }

        [Fact]
        public void CreateItem_FavoritesTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var post = this.CreatePost();
            post.FavoritedCount = 1;

            var item = cache.CreateItem(post);
            Assert.Equal("+1", item.SubItems[6].Text);
        }

        [Fact]
        public void CreateItem_RetweetTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var post = this.CreatePost();
            post.RetweetedId = 50L;
            post.RetweetedBy = "hoge";

            var item = cache.CreateItem(post);
            Assert.Equal($"test{Environment.NewLine}(RT:hoge)", item.SubItems[4].Text);
        }

        [Fact]
        public void CreateItem_DeletedTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var post = this.CreatePost();
            post.IsDeleted = true;

            var item = cache.CreateItem(post);
            Assert.Equal("(DELETED)", item.SubItems[2].Text);
        }

        [Fact]
        public void CreateItem_Font_ReadedTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var post = this.CreatePost();
            post.IsRead = true;

            var item = cache.CreateItem(post);
            Assert.Equal(theme.FontReaded, item.Font);
        }

        [Fact]
        public void CreateItem_Font_UnreadTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var post = this.CreatePost();
            post.IsRead = false;

            var item = cache.CreateItem(post);
            Assert.Equal(theme.FontUnread, item.Font);
        }

        [Fact]
        public void CreateItem_Font_UnreadStyleDisabledTest()
        {
            var tab = new PublicSearchTabModel("tab");
            var settingCommon = new SettingCommon();
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, settingCommon, theme);

            var post = this.CreatePost();
            post.IsRead = false;

            settingCommon.UseUnreadStyle = false;

            var item = cache.CreateItem(post);
            Assert.Equal(theme.FontReaded, item.Font);
        }

        [Fact]
        public void CreateItem_Font_UnreadManageDisabledTest()
        {
            var tab = new PublicSearchTabModel("tab");
            var settingCommon = new SettingCommon();
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, settingCommon, theme);

            var post = this.CreatePost();
            post.IsRead = false;

            tab.UnreadManage = false;

            var item = cache.CreateItem(post);
            Assert.Equal(theme.FontReaded, item.Font);
        }

        [Fact]
        public void CreateItem_ForeColor_Test()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var post = this.CreatePost();

            var item = cache.CreateItem(post);
            Assert.Equal(theme.ColorRead, item.ForeColor);
        }

        [Fact]
        public void CreateItem_ForeColor_FavoritedTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var post = this.CreatePost();
            post.IsFav = true;

            var item = cache.CreateItem(post);
            Assert.Equal(theme.ColorFav, item.ForeColor);
        }

        [Fact]
        public void CreateItem_ForeColor_RetweetTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var post = this.CreatePost();
            post.RetweetedId = 100L;

            var item = cache.CreateItem(post);
            Assert.Equal(theme.ColorRetweet, item.ForeColor);
        }

        [Fact]
        public void CreateItem_ForeColor_OWLTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var post = this.CreatePost();
            post.IsOwl = true;

            var item = cache.CreateItem(post);
            Assert.Equal(theme.ColorOWL, item.ForeColor);
        }

        [Fact]
        public void CreateItem_ForeColor_OWLStyleDisabledTest()
        {
            var tab = new PublicSearchTabModel("tab");
            var settingCommon = new SettingCommon();
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, settingCommon, theme);

            var post = this.CreatePost();
            post.IsOwl = true;

            settingCommon.OneWayLove = false;

            var item = cache.CreateItem(post);
            Assert.Equal(theme.ColorRead, item.ForeColor);
        }

        [Fact]
        public void CreateItem_ForeColor_DMTest()
        {
            var tab = new PublicSearchTabModel("tab");
            var settingCommon = new SettingCommon();
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, settingCommon, theme);

            var post = this.CreatePost();
            post.IsDm = true;
            post.IsOwl = true;

            // DM の場合は設定に関わらず ColorOWL を使う
            settingCommon.OneWayLove = false;

            var item = cache.CreateItem(post);
            Assert.Equal(theme.ColorOWL, item.ForeColor);
        }

        [Fact]
        public void JudgeColor_AtToTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var targetPost = this.CreatePost();

            var basePost = this.CreatePost();
            basePost.InReplyToStatusId = targetPost.StatusId;

            Assert.Equal(theme.ColorAtTo, cache.JudgeColor(basePost, targetPost));
        }

        [Fact]
        public void JudgeColor_SelfTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var targetPost = this.CreatePost();
            targetPost.IsMe = true;

            var basePost = this.CreatePost();

            Assert.Equal(theme.ColorSelf, cache.JudgeColor(basePost, targetPost));
        }

        [Fact]
        public void JudgeColor_AtSelfTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var targetPost = this.CreatePost();
            targetPost.IsReply = true;

            var basePost = this.CreatePost();

            Assert.Equal(theme.ColorAtSelf, cache.JudgeColor(basePost, targetPost));
        }

        [Fact]
        public void JudgeColor_AtFromTargetTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var targetPost = this.CreatePost();

            var basePost = this.CreatePost();
            basePost.ReplyToList = new() { (targetPost.UserId, targetPost.ScreenName) };

            Assert.Equal(theme.ColorAtFromTarget, cache.JudgeColor(basePost, targetPost));
        }

        [Fact]
        public void JudgeColor_AtTargetTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var basePost = this.CreatePost();

            var targetPost = this.CreatePost();
            targetPost.ReplyToList = new() { (basePost.UserId, basePost.ScreenName) };

            Assert.Equal(theme.ColorAtTarget, cache.JudgeColor(basePost, targetPost));
        }

        [Fact]
        public void JudgeColor_TargetTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var targetPost = this.CreatePost();

            var basePost = this.CreatePost();
            basePost.UserId = targetPost.UserId;

            Assert.Equal(theme.ColorTarget, cache.JudgeColor(basePost, targetPost));
        }

        [Fact]
        public void JudgeColor_NormalTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var theme = new ThemeManager(new());
            using var cache = new TimelineListViewCache(listView, tab, new(), theme);

            var targetPost = this.CreatePost();
            var basePost = this.CreatePost();

            Assert.Equal(theme.ColorListBackcolor, cache.JudgeColor(basePost, targetPost));
        }
    }
}