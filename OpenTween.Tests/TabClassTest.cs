// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class TabClassTest
    {
        [Fact]
        public void NextUnreadId_Test()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            tab.UnreadManage = true;

            // 未読なし
            Assert.Equal(-1L, tab.NextUnreadId);

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(100L, tab.NextUnreadId);

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 50L,
                IsRead = true, // 既読
            });
            tab.AddSubmit();

            Assert.Equal(100L, tab.NextUnreadId);
        }

        [Fact]
        public void NextUnreadId_DisabledTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            // 未読表示無効
            tab.UnreadManage = false;

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(-1L, tab.NextUnreadId);
        }

        [Fact]
        public void NextUnreadId_SortByIdAscTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            tab.UnreadManage = true;

            // ID の昇順でソート
            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);

            // 画面には上から 100 → 200 → 300 の順に並ぶ
            tab.AddPostToInnerStorage(new PostClass { StatusId = 100L, IsRead = false });
            tab.AddPostToInnerStorage(new PostClass { StatusId = 200L, IsRead = false });
            tab.AddPostToInnerStorage(new PostClass { StatusId = 300L, IsRead = false });
            tab.AddSubmit();

            // 昇順/降順に関わらず、ID の小さい順に未読の ID を返す
            Assert.Equal(100L, tab.NextUnreadId);
        }

        [Fact]
        public void NextUnreadId_SortByIdDescTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            tab.UnreadManage = true;

            // ID の降順でソート
            tab.SetSortMode(ComparerMode.Id, SortOrder.Descending);

            // 画面には上から 300 → 200 → 100 の順に並ぶ
            tab.AddPostToInnerStorage(new PostClass { StatusId = 100L, IsRead = false });
            tab.AddPostToInnerStorage(new PostClass { StatusId = 200L, IsRead = false });
            tab.AddPostToInnerStorage(new PostClass { StatusId = 300L, IsRead = false });
            tab.AddSubmit();

            // 昇順/降順に関わらず、ID の小さい順に未読の ID を返す
            Assert.Equal(100L, tab.NextUnreadId);
        }

        [Fact]
        public void NextUnreadId_SortByScreenNameAscTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            tab.UnreadManage = true;

            // ScreenName の昇順でソート
            tab.SetSortMode(ComparerMode.Name, SortOrder.Ascending);

            // 画面には上から 200 → 100 → 300 の順に並ぶ
            tab.AddPostToInnerStorage(new PostClass { StatusId = 100L, ScreenName = "bbb", IsRead = false });
            tab.AddPostToInnerStorage(new PostClass { StatusId = 200L, ScreenName = "aaa", IsRead = false });
            tab.AddPostToInnerStorage(new PostClass { StatusId = 300L, ScreenName = "ccc", IsRead = false });
            tab.AddSubmit();

            // 昇順/降順に関わらず、ScreenName の辞書順で小さい順に未読の ID を返す
            Assert.Equal(200L, tab.NextUnreadId);
        }

        [Fact]
        public void NextUnreadId_SortByScreenNameDescTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            tab.UnreadManage = true;

            // ScreenName の降順でソート
            tab.SetSortMode(ComparerMode.Name, SortOrder.Descending);

            // 画面には上から 300 → 100 → 200 の順に並ぶ
            tab.AddPostToInnerStorage(new PostClass { StatusId = 100L, ScreenName = "bbb", IsRead = false });
            tab.AddPostToInnerStorage(new PostClass { StatusId = 200L, ScreenName = "aaa", IsRead = false });
            tab.AddPostToInnerStorage(new PostClass { StatusId = 300L, ScreenName = "ccc", IsRead = false });
            tab.AddSubmit();

            // 昇順/降順に関わらず、ScreenName の辞書順で小さい順に未読の ID を返す
            Assert.Equal(200L, tab.NextUnreadId);
        }

        [Fact]
        public void UnreadCount_Test()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            tab.UnreadManage = true;

            // 未読なし
            Assert.Equal(0, tab.UnreadCount);

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(1, tab.UnreadCount);

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 50L,
                IsRead = true, // 既読
            });
            tab.AddSubmit();

            Assert.Equal(1, tab.UnreadCount);
        }

        [Fact]
        public void UnreadCount_DisabledTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            // 未読表示無効
            tab.UnreadManage = false;

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(0, tab.UnreadCount);
        }

        [Fact]
        public void NextUnreadIndex_Test()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };
            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);

            tab.UnreadManage = true;

            // 未読なし
            Assert.Equal(-1, tab.NextUnreadIndex);

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 50L,
                IsRead = true, // 既読
            });
            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 150L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(1, tab.NextUnreadIndex);
        }

        [Fact]
        public void NextUnreadIndex_DisabledTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };
            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);

            // 未読表示無効
            tab.UnreadManage = false;

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(-1, tab.NextUnreadIndex);
        }

        [Fact]
        public void GetUnreadIds_Test()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };
            tab.UnreadManage = true;

            Assert.Empty(tab.GetUnreadIds());

            tab.AddPostToInnerStorage(new PostClass { StatusId = 100L, IsRead = false });
            tab.AddPostToInnerStorage(new PostClass { StatusId = 200L, IsRead = true });
            tab.AddSubmit();

            Assert.Equal(new[] { 100L }, tab.GetUnreadIds());

            tab.SetReadState(100L, true); // 既読にする

            Assert.Empty(tab.GetUnreadIds());
        }

        [Fact]
        public void SetReadState_MarkAsReadTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            tab.UnreadManage = true;

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(1, tab.UnreadCount);

            tab.SetReadState(100L, true); // 既読にする

            Assert.Equal(0, tab.UnreadCount);
        }

        [Fact]
        public void SetReadState_MarkAsUnreadTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.UserTimeline };

            tab.UnreadManage = true;

            tab.AddPostToInnerStorage(new PostClass
            {
                StatusId = 100L,
                IsRead = true, // 既読
            });
            tab.AddSubmit();

            Assert.Equal(0, tab.UnreadCount);

            tab.SetReadState(100L, false); // 未読にする

            Assert.Equal(1, tab.UnreadCount);
        }

        [Fact]
        public void FilterArraySetter_Test()
        {
            var tab = new TabClass();

            var filter = new PostFilterRule();
            tab.FilterArray = new[] { filter };

            Assert.Equal(new[] { filter }, tab.FilterArray);
            Assert.True(tab.FilterModified);
        }

        [Fact]
        public void AddFilter_Test()
        {
            var tab = new TabClass();

            var filter = new PostFilterRule();
            tab.AddFilter(filter);

            Assert.Equal(new[] { filter }, tab.FilterArray);
            Assert.True(tab.FilterModified);
        }

        [Fact]
        public void RemoveFilter_Test()
        {
            var tab = new TabClass();

            var filter = new PostFilterRule();
            tab.FilterArray = new[] { filter };
            tab.FilterModified = false;

            tab.RemoveFilter(filter);

            Assert.Empty(tab.FilterArray);
            Assert.True(tab.FilterModified);
        }

        [Fact]
        public void OnFilterModified_Test()
        {
            var tab = new TabClass();

            var filter = new PostFilterRule();
            tab.FilterArray = new[] { filter };
            tab.FilterModified = false;

            // TabClass に紐付いているフィルタを変更
            filter.FilterSource = "OpenTween";

            Assert.True(tab.FilterModified);
        }

        [Fact]
        public void OnFilterModified_DetachedTest()
        {
            var tab = new TabClass();

            var filter = new PostFilterRule();
            tab.FilterArray = new[] { filter };

            tab.RemoveFilter(filter);
            tab.FilterModified = false;

            // TabClass から既に削除されたフィルタを変更
            filter.FilterSource = "OpenTween";

            Assert.False(tab.FilterModified);
        }

        [Fact]
        public void SearchPostsAll_Test()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.PublicSearch };

            tab.AddPostToInnerStorage(new PostClass { StatusId = 100L, TextFromApi = "abcd", ScreenName = "", Nickname = "" }); // 0
            tab.AddPostToInnerStorage(new PostClass { StatusId = 110L, TextFromApi = "efgh", ScreenName = "", Nickname = "" }); // 1
            tab.AddPostToInnerStorage(new PostClass { StatusId = 120L, TextFromApi = "ijkl", ScreenName = "", Nickname = "" }); // 2
            tab.AddPostToInnerStorage(new PostClass { StatusId = 130L, TextFromApi = "abc", ScreenName = "", Nickname = "" });  // 3
            tab.AddPostToInnerStorage(new PostClass { StatusId = 140L, TextFromApi = "def", ScreenName = "", Nickname = "" });  // 4

            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
            tab.AddSubmit();

            // インデックス番号 0 から開始 → 0, 1, 2, 3, 4 の順に検索
            var result = tab.SearchPostsAll(x => x.Contains("a"), startIndex: 0);
            Assert.Equal(new[] { 0, 3 }, result);

            // インデックス番号 2 から開始 → 2, 3, 4, 0, 1 の順に検索
            result = tab.SearchPostsAll(x => x.Contains("a"), startIndex: 2);
            Assert.Equal(new[] { 3, 0 }, result);
        }

        [Fact]
        public void SearchPostsAll_ReverseOrderTest()
        {
            var tab = new TabClass { TabType = MyCommon.TabUsageType.PublicSearch };

            tab.AddPostToInnerStorage(new PostClass { StatusId = 100L, TextFromApi = "abcd", ScreenName = "", Nickname = "" }); // 0
            tab.AddPostToInnerStorage(new PostClass { StatusId = 110L, TextFromApi = "efgh", ScreenName = "", Nickname = "" }); // 1
            tab.AddPostToInnerStorage(new PostClass { StatusId = 120L, TextFromApi = "ijkl", ScreenName = "", Nickname = "" }); // 2
            tab.AddPostToInnerStorage(new PostClass { StatusId = 130L, TextFromApi = "abc", ScreenName = "", Nickname = "" });  // 3
            tab.AddPostToInnerStorage(new PostClass { StatusId = 140L, TextFromApi = "def", ScreenName = "", Nickname = "" });  // 4

            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
            tab.AddSubmit();

            // インデックス番号 4 から逆順に開始 → 4, 3, 2, 1, 0 の順に検索
            var result = tab.SearchPostsAll(x => x.Contains("a"), startIndex: 4, reverse: true);
            Assert.Equal(new[] { 3, 0 }, result);

            // インデックス番号 2 から逆順に開始 → 2, 1, 0, 4, 3 の順に検索
            result = tab.SearchPostsAll(x => x.Contains("a"), startIndex: 2, reverse: true);
            Assert.Equal(new[] { 0, 3 }, result);
        }
    }

    public class TabUsageTypeExtTest
    {
        [Theory]
        [InlineData(MyCommon.TabUsageType.Home,          true)]
        [InlineData(MyCommon.TabUsageType.Mentions,      true)]
        [InlineData(MyCommon.TabUsageType.DirectMessage, true)]
        [InlineData(MyCommon.TabUsageType.Favorites,     true)]
        [InlineData(MyCommon.TabUsageType.UserDefined,   false)]
        [InlineData(MyCommon.TabUsageType.Lists,         false)]
        [InlineData(MyCommon.TabUsageType.UserTimeline,  false)]
        [InlineData(MyCommon.TabUsageType.PublicSearch,  false)]
        [InlineData(MyCommon.TabUsageType.Related,       false)]
        public void IsDefault_Test(MyCommon.TabUsageType tabType, bool expected)
        {
            Assert.Equal(expected, tabType.IsDefault());
        }

        [Theory]
        [InlineData(MyCommon.TabUsageType.Home,          false)]
        [InlineData(MyCommon.TabUsageType.Mentions,      true)]
        [InlineData(MyCommon.TabUsageType.DirectMessage, false)]
        [InlineData(MyCommon.TabUsageType.Favorites,     false)]
        [InlineData(MyCommon.TabUsageType.UserDefined,   true)]
        [InlineData(MyCommon.TabUsageType.Lists,         false)]
        [InlineData(MyCommon.TabUsageType.UserTimeline,  false)]
        [InlineData(MyCommon.TabUsageType.PublicSearch,  false)]
        [InlineData(MyCommon.TabUsageType.Related,       false)]
        public void IsDistributable_Test(MyCommon.TabUsageType tabType, bool expected)
        {
            Assert.Equal(expected, tabType.IsDistributable());
        }

        [Theory]
        [InlineData(MyCommon.TabUsageType.Home,          false)]
        [InlineData(MyCommon.TabUsageType.Mentions,      false)]
        [InlineData(MyCommon.TabUsageType.DirectMessage, true)]
        [InlineData(MyCommon.TabUsageType.Favorites,     false)]
        [InlineData(MyCommon.TabUsageType.UserDefined,   false)]
        [InlineData(MyCommon.TabUsageType.Lists,         true)]
        [InlineData(MyCommon.TabUsageType.UserTimeline,  true)]
        [InlineData(MyCommon.TabUsageType.PublicSearch,  true)]
        [InlineData(MyCommon.TabUsageType.Related,       true)]
        public void IsInnerStorage_Test(MyCommon.TabUsageType tabType, bool expected)
        {
            Assert.Equal(expected, tabType.IsInnerStorage());
        }
    }
}
