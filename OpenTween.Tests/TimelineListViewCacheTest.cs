// OpenTween - Client of Twitter
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
            using var cache = new TimelineListViewCache(listView, tab, new());

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
        public void GetItem_Test()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var post = this.CreatePost();
            tab.AddPostQueue(post);
            tab.AddSubmit();

            var item = cache.GetItem(0);

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
        public void GetItem_UnreadTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var post = this.CreatePost();
            post.IsRead = false;

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var item = cache.GetItem(0);
            Assert.Equal("★", item.SubItems[5].Text);
        }

        [Fact]
        public void GetItem_UnreadManageDisabledTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            // 未読管理が無効な場合は未読状態に関わらず未読マークを表示しない
            tab.UnreadManage = false;

            var post = this.CreatePost();
            post.IsRead = false;

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var item = cache.GetItem(0);
            Assert.Equal("", item.SubItems[5].Text);
        }

        [Fact]
        public void GetItem_FavoritesTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var post = this.CreatePost();
            post.FavoritedCount = 1;

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var item = cache.GetItem(0);
            Assert.Equal("+1", item.SubItems[6].Text);
        }

        [Fact]
        public void GetItem_RetweetTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var post = this.CreatePost();
            post.RetweetedId = 50L;
            post.RetweetedBy = "hoge";

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var item = cache.GetItem(0);
            Assert.Equal($"test{Environment.NewLine}(RT:hoge)", item.SubItems[4].Text);
        }

        [Fact]
        public void GetItem_DeletedTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var post = this.CreatePost();
            post.IsDeleted = true;

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var item = cache.GetItem(0);
            Assert.Equal("(DELETED)", item.SubItems[2].Text);
        }

        [Fact]
        public void GetItem_CachedTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var post = this.CreatePost();
            tab.AddPostQueue(post);
            tab.AddSubmit();

            cache.CreateCache(startIndex: 0, endIndex: 0);

            post.IsRead = false;

            // IsRead の状態はキャッシュに未反映なので既読状態で返るのが正しい
            var item = cache.GetItem(0);
            Assert.Equal("", item.SubItems[5].Text);
        }

        [Fact]
        public void GetStyle_Font_ReadedTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var post = this.CreatePost();
            post.IsRead = true;

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var style = cache.GetStyle(0);
            Assert.Equal(ListItemFont.Readed, style.Font);
        }

        [Fact]
        public void GetStyle_Font_UnreadTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var post = this.CreatePost();
            post.IsRead = false;

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var style = cache.GetStyle(0);
            Assert.Equal(ListItemFont.Unread, style.Font);
        }

        [Fact]
        public void GetStyle_Font_UnreadStyleDisabledTest()
        {
            var tab = new PublicSearchTabModel("tab");
            var settingCommon = new SettingCommon();
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, settingCommon);

            var post = this.CreatePost();
            post.IsRead = false;

            settingCommon.UseUnreadStyle = false;

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var style = cache.GetStyle(0);
            Assert.Equal(ListItemFont.Readed, style.Font);
        }

        [Fact]
        public void GetStyle_Font_UnreadManageDisabledTest()
        {
            var tab = new PublicSearchTabModel("tab");
            var settingCommon = new SettingCommon();
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, settingCommon);

            var post = this.CreatePost();
            post.IsRead = false;

            tab.UnreadManage = false;

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var style = cache.GetStyle(0);
            Assert.Equal(ListItemFont.Readed, style.Font);
        }

        [Fact]
        public void GetStyleItem_ForeColor_Test()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var post = this.CreatePost();

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var style = cache.GetStyle(0);
            Assert.Equal(ListItemForeColor.None, style.ForeColor);
        }

        [Fact]
        public void GetStyle_ForeColor_FavoritedTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var post = this.CreatePost();
            post.IsFav = true;

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var style = cache.GetStyle(0);
            Assert.Equal(ListItemForeColor.Fav, style.ForeColor);
        }

        [Fact]
        public void GetStyle_ForeColor_RetweetTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var post = this.CreatePost();
            post.RetweetedId = 100L;

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var style = cache.GetStyle(0);
            Assert.Equal(ListItemForeColor.Retweet, style.ForeColor);
        }

        [Fact]
        public void GetStyle_ForeColor_OWLTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var post = this.CreatePost();
            post.IsOwl = true;

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var style = cache.GetStyle(0);
            Assert.Equal(ListItemForeColor.OWL, style.ForeColor);
        }

        [Fact]
        public void GetStyle_ForeColor_OWLStyleDisabledTest()
        {
            var tab = new PublicSearchTabModel("tab");
            var settingCommon = new SettingCommon();
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, settingCommon);

            var post = this.CreatePost();
            post.IsOwl = true;

            settingCommon.OneWayLove = false;

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var style = cache.GetStyle(0);
            Assert.Equal(ListItemForeColor.None, style.ForeColor);
        }

        [Fact]
        public void GetStyle_ForeColor_DMTest()
        {
            var tab = new PublicSearchTabModel("tab");
            var settingCommon = new SettingCommon();
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, settingCommon);

            var post = this.CreatePost();
            post.IsDm = true;
            post.IsOwl = true;

            // DM の場合は設定に関わらず ColorOWL を使う
            settingCommon.OneWayLove = false;

            tab.AddPostQueue(post);
            tab.AddSubmit();

            var style = cache.GetStyle(0);
            Assert.Equal(ListItemForeColor.OWL, style.ForeColor);
        }

        [Fact]
        public void GetStyle_BackColor_AtToTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var targetPost = this.CreatePost();
            tab.AddPostQueue(targetPost);

            var basePost = this.CreatePost();
            basePost.InReplyToStatusId = targetPost.StatusId;
            tab.AddPostQueue(basePost);

            tab.AddSubmit();
            tab.SelectPosts(new[] { tab.IndexOf(basePost.StatusId) });

            var style = cache.GetStyle(tab.IndexOf(targetPost.StatusId));
            Assert.Equal(ListItemBackColor.AtTo, style.BackColor);
        }

        [Fact]
        public void GetStyle_BackColor_SelfTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var targetPost = this.CreatePost();
            targetPost.IsMe = true;
            tab.AddPostQueue(targetPost);

            var basePost = this.CreatePost();
            tab.AddPostQueue(basePost);

            tab.AddSubmit();
            tab.SelectPosts(new[] { tab.IndexOf(basePost.StatusId) });

            var style = cache.GetStyle(tab.IndexOf(targetPost.StatusId));
            Assert.Equal(ListItemBackColor.Self, style.BackColor);
        }

        [Fact]
        public void GetStyle_BackColor_AtSelfTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var targetPost = this.CreatePost();
            targetPost.IsReply = true;
            tab.AddPostQueue(targetPost);

            var basePost = this.CreatePost();
            tab.AddPostQueue(basePost);

            tab.AddSubmit();
            tab.SelectPosts(new[] { tab.IndexOf(basePost.StatusId) });

            var style = cache.GetStyle(tab.IndexOf(targetPost.StatusId));
            Assert.Equal(ListItemBackColor.AtSelf, style.BackColor);
        }

        [Fact]
        public void GetStyle_BackColor_AtFromTargetTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var targetPost = this.CreatePost();
            tab.AddPostQueue(targetPost);

            var basePost = this.CreatePost();
            basePost.ReplyToList = new() { (targetPost.UserId, targetPost.ScreenName) };
            tab.AddPostQueue(basePost);

            tab.AddSubmit();
            tab.SelectPosts(new[] { tab.IndexOf(basePost.StatusId) });

            var style = cache.GetStyle(tab.IndexOf(targetPost.StatusId));
            Assert.Equal(ListItemBackColor.AtFromTarget, style.BackColor);
        }

        [Fact]
        public void GetStyle_BackColor_AtTargetTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var basePost = this.CreatePost();
            tab.AddPostQueue(basePost);

            var targetPost = this.CreatePost();
            targetPost.ReplyToList = new() { (basePost.UserId, basePost.ScreenName) };
            tab.AddPostQueue(targetPost);

            tab.AddSubmit();
            tab.SelectPosts(new[] { tab.IndexOf(basePost.StatusId) });

            var style = cache.GetStyle(tab.IndexOf(targetPost.StatusId));
            Assert.Equal(ListItemBackColor.AtTarget, style.BackColor);
        }

        [Fact]
        public void GetStyle_BackColor_TargetTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var targetPost = this.CreatePost();
            tab.AddPostQueue(targetPost);

            var basePost = this.CreatePost();
            basePost.UserId = targetPost.UserId;
            tab.AddPostQueue(basePost);

            tab.AddSubmit();
            tab.SelectPosts(new[] { tab.IndexOf(basePost.StatusId) });

            var style = cache.GetStyle(tab.IndexOf(targetPost.StatusId));
            Assert.Equal(ListItemBackColor.Target, style.BackColor);
        }

        [Fact]
        public void GetStyle_BackColor_NormalTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var targetPost = this.CreatePost();
            tab.AddPostQueue(targetPost);

            var basePost = this.CreatePost();
            tab.AddPostQueue(basePost);

            tab.AddSubmit();
            tab.SelectPosts(new[] { tab.IndexOf(basePost.StatusId) });

            var style = cache.GetStyle(tab.IndexOf(targetPost.StatusId));
            Assert.Equal(ListItemBackColor.None, style.BackColor);
        }

        [Fact]
        public void GetStyle_CachedTest()
        {
            var tab = new PublicSearchTabModel("tab");
            using var listView = new DetailsListView();
            using var cache = new TimelineListViewCache(listView, tab, new());

            var post = this.CreatePost();
            tab.AddPostQueue(post);
            tab.AddSubmit();

            cache.CreateCache(startIndex: 0, endIndex: 0);

            post.IsFav = true;

            // IsFav の状態はキャッシュに未反映なので None が返るのが正しい
            var style = cache.GetStyle(0);
            Assert.Equal(ListItemForeColor.None, style.ForeColor);
        }
    }
}
