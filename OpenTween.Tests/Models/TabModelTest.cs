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

namespace OpenTween.Models
{
    public class TabModelTest
    {
        [Fact]
        public void AnchorPost_Test()
        {
            var tab = new PublicSearchTabModel("search");

            var posts = new[]
            {
                new PostClass { StatusId = 100L },
                new PostClass { StatusId = 110L },
            };
            tab.AddPostQueue(posts[0]);
            tab.AddPostQueue(posts[1]);
            tab.AddSubmit();

            Assert.Null(tab.AnchorStatusId);
            Assert.Null(tab.AnchorPost);

            tab.AnchorPost = posts[1];

            Assert.Equal(110L, tab.AnchorStatusId);
            Assert.Equal(110L, tab.AnchorPost.StatusId);
        }

        [Fact]
        public void AnchorPost_DeletedTest()
        {
            var tab = new PublicSearchTabModel("search");

            var posts = new[]
            {
                new PostClass { StatusId = 100L },
            };
            tab.AddPostQueue(posts[0]);
            tab.AddSubmit();
            tab.AnchorPost = posts[0];

            Assert.Equal(100L, tab.AnchorPost.StatusId);

            tab.EnqueueRemovePost(100L, setIsDeleted: true);
            tab.RemoveSubmit();

            Assert.Null(tab.AnchorPost);
        }

        [Fact]
        public void ClearAnchor_Test()
        {
            var tab = new PublicSearchTabModel("search");

            var posts = new[]
            {
                new PostClass { StatusId = 100L },
            };
            tab.AddPostQueue(posts[0]);
            tab.AddSubmit();
            tab.AnchorPost = posts[0];

            Assert.Equal(100L, tab.AnchorPost.StatusId);
            tab.ClearAnchor();
            Assert.Null(tab.AnchorPost);
        }

        [Fact]
        public void SelectPosts_Test()
        {
            var tab = new PublicSearchTabModel("search");

            var posts = new[]
            {
                new PostClass { StatusId = 100L },
                new PostClass { StatusId = 110L },
                new PostClass { StatusId = 120L },
            };
            tab.AddPostQueue(posts[0]);
            tab.AddPostQueue(posts[1]);
            tab.AddPostQueue(posts[2]);
            tab.AddSubmit();

            tab.SelectPosts(new[] { 0, 2 });

            Assert.Equal(new[] { 100L, 120L }, tab.SelectedStatusIds);
            Assert.Equal(100L, tab.SelectedStatusId);
            Assert.Equal(new[] { posts[0], posts[2] }, tab.SelectedPosts);
            Assert.Equal(posts[0], tab.SelectedPost);
            Assert.Equal(0, tab.SelectedIndex);
        }

        [Fact]
        public void SelectPosts_EmptyTest()
        {
            var tab = new PublicSearchTabModel("search");
            tab.AddPostQueue(new PostClass { StatusId = 100L });
            tab.AddSubmit();

            tab.SelectPosts(Array.Empty<int>());

            Assert.Empty(tab.SelectedStatusIds);
            Assert.Equal(-1L, tab.SelectedStatusId);
            Assert.Empty(tab.SelectedPosts);
            Assert.Null(tab.SelectedPost);
            Assert.Equal(-1, tab.SelectedIndex);
        }

        [Fact]
        public void SelectPosts_InvalidIndexTest()
        {
            var tab = new PublicSearchTabModel("search");
            tab.AddPostQueue(new PostClass { StatusId = 100L });
            tab.AddSubmit();

            Assert.Throws<ArgumentOutOfRangeException>(() => tab.SelectPosts(new[] { -1 }));
            Assert.Throws<ArgumentOutOfRangeException>(() => tab.SelectPosts(new[] { 1 }));
        }

        [Fact]
        public void EnqueueRemovePost_Test()
        {
            var tab = new PublicSearchTabModel("search")
            {
                UnreadManage = true,
            };

            tab.AddPostQueue(new PostClass { StatusId = 100L, IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = 110L, IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = 120L, IsRead = false });

            tab.AddSubmit();

            Assert.Equal(3, tab.AllCount);
            Assert.Equal(3, tab.UnreadCount);

            tab.EnqueueRemovePost(100L, setIsDeleted: false);

            // この時点では削除は行われない
            Assert.Equal(3, tab.AllCount);
            Assert.Equal(3, tab.UnreadCount);

            var removedIds = tab.RemoveSubmit();

            Assert.Equal(2, tab.AllCount);
            Assert.Equal(2, tab.UnreadCount);
            Assert.Equal(new[] { 110L, 120L }, tab.StatusIds);
            Assert.Equal(new[] { 100L }, removedIds.AsEnumerable());
        }

        [Fact]
        public void EnqueueRemovePost_SetIsDeletedTest()
        {
            var tab = new PublicSearchTabModel("search")
            {
                UnreadManage = true,
            };

            var post = new PostClass { StatusId = 100L, IsRead = false };
            tab.AddPostQueue(post);
            tab.AddSubmit();

            Assert.Equal(1, tab.AllCount);
            Assert.Equal(1, tab.UnreadCount);

            tab.EnqueueRemovePost(100L, setIsDeleted: true);

            // この時点ではタブからの削除は行われないが、PostClass.IsDeleted は true にセットされる
            Assert.Equal(1, tab.AllCount);
            Assert.Equal(1, tab.UnreadCount);
            Assert.True(post.IsDeleted);

            var removedIds = tab.RemoveSubmit();

            Assert.Equal(0, tab.AllCount);
            Assert.Equal(0, tab.UnreadCount);
            Assert.Equal(new[] { 100L }, removedIds.AsEnumerable());
        }

        [Fact]
        public void EnqueueRemovePost_UnknownIdTest()
        {
            var tab = new PublicSearchTabModel("search")
            {
                UnreadManage = true,
            };

            tab.AddPostQueue(new PostClass { StatusId = 100L, IsRead = false });
            tab.AddSubmit();

            Assert.Equal(1, tab.AllCount);
            Assert.Equal(1, tab.UnreadCount);

            // StatusId = 999L は存在しない
            tab.EnqueueRemovePost(999L, setIsDeleted: false);

            Assert.Equal(1, tab.AllCount);
            Assert.Equal(1, tab.UnreadCount);

            var removedIds = tab.RemoveSubmit();

            Assert.Equal(1, tab.AllCount);
            Assert.Equal(1, tab.UnreadCount);
            Assert.Empty(removedIds);
        }

        [Fact]
        public void EnqueueRemovePost_SelectedTest()
        {
            var tab = new PublicSearchTabModel("search");
            tab.AddPostQueue(new PostClass { StatusId = 100L });
            tab.AddPostQueue(new PostClass { StatusId = 110L });
            tab.AddSubmit();
            tab.SelectPosts(new[] { 0, 1 });

            Assert.Equal(2, tab.AllCount);
            Assert.Equal(new[] { 100L, 110L }, tab.SelectedStatusIds);

            tab.EnqueueRemovePost(100L, setIsDeleted: false);

            // この時点では変化しない
            Assert.Equal(2, tab.AllCount);
            Assert.Equal(new[] { 100L, 110L }, tab.SelectedStatusIds);

            tab.RemoveSubmit();

            // 削除された発言の選択が解除される
            Assert.Equal(1, tab.AllCount);
            Assert.Equal(new[] { 110L }, tab.SelectedStatusIds);
        }

        [Fact]
        public void NextUnreadId_Test()
        {
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            // 未読なし
            Assert.Equal(-1L, tab.NextUnreadId);

            tab.AddPostQueue(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(100L, tab.NextUnreadId);

            tab.AddPostQueue(new PostClass
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
            var tab = new PublicSearchTabModel("search");

            // 未読表示無効
            tab.UnreadManage = false;

            tab.AddPostQueue(new PostClass
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
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            // ID の昇順でソート
            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);

            // 画面には上から 100 → 200 → 300 の順に並ぶ
            tab.AddPostQueue(new PostClass { StatusId = 100L, IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = 200L, IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = 300L, IsRead = false });
            tab.AddSubmit();

            // 昇順/降順に関わらず、ID の小さい順に未読の ID を返す
            Assert.Equal(100L, tab.NextUnreadId);
        }

        [Fact]
        public void NextUnreadId_SortByIdDescTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            // ID の降順でソート
            tab.SetSortMode(ComparerMode.Id, SortOrder.Descending);

            // 画面には上から 300 → 200 → 100 の順に並ぶ
            tab.AddPostQueue(new PostClass { StatusId = 100L, IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = 200L, IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = 300L, IsRead = false });
            tab.AddSubmit();

            // 昇順/降順に関わらず、ID の小さい順に未読の ID を返す
            Assert.Equal(100L, tab.NextUnreadId);
        }

        [Fact]
        public void NextUnreadId_SortByScreenNameAscTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            // ScreenName の昇順でソート
            tab.SetSortMode(ComparerMode.Name, SortOrder.Ascending);

            // 画面には上から 200 → 100 → 300 の順に並ぶ
            tab.AddPostQueue(new PostClass { StatusId = 100L, ScreenName = "bbb", IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = 200L, ScreenName = "aaa", IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = 300L, ScreenName = "ccc", IsRead = false });
            tab.AddSubmit();

            // 昇順/降順に関わらず、ScreenName の辞書順で小さい順に未読の ID を返す
            Assert.Equal(200L, tab.NextUnreadId);
        }

        [Fact]
        public void NextUnreadId_SortByScreenNameDescTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            // ScreenName の降順でソート
            tab.SetSortMode(ComparerMode.Name, SortOrder.Descending);

            // 画面には上から 300 → 100 → 200 の順に並ぶ
            tab.AddPostQueue(new PostClass { StatusId = 100L, ScreenName = "bbb", IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = 200L, ScreenName = "aaa", IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = 300L, ScreenName = "ccc", IsRead = false });
            tab.AddSubmit();

            // 昇順/降順に関わらず、ScreenName の辞書順で小さい順に未読の ID を返す
            Assert.Equal(200L, tab.NextUnreadId);
        }

        [Fact]
        public void UnreadCount_Test()
        {
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            // 未読なし
            Assert.Equal(0, tab.UnreadCount);

            tab.AddPostQueue(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(1, tab.UnreadCount);

            tab.AddPostQueue(new PostClass
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
            var tab = new PublicSearchTabModel("search");

            // 未読表示無効
            tab.UnreadManage = false;

            tab.AddPostQueue(new PostClass
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
            var tab = new PublicSearchTabModel("search");
            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);

            tab.UnreadManage = true;

            // 未読なし
            Assert.Equal(-1, tab.NextUnreadIndex);

            tab.AddPostQueue(new PostClass
            {
                StatusId = 50L,
                IsRead = true, // 既読
            });
            tab.AddPostQueue(new PostClass
            {
                StatusId = 100L,
                IsRead = false, // 未読
            });
            tab.AddPostQueue(new PostClass
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
            var tab = new PublicSearchTabModel("search");
            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);

            // 未読表示無効
            tab.UnreadManage = false;

            tab.AddPostQueue(new PostClass
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
            var tab = new PublicSearchTabModel("search");
            tab.UnreadManage = true;

            Assert.Empty(tab.GetUnreadIds());

            tab.AddPostQueue(new PostClass { StatusId = 100L, IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = 200L, IsRead = true });
            tab.AddSubmit();

            Assert.Equal(new[] { 100L }, tab.GetUnreadIds());

            tab.SetReadState(100L, true); // 既読にする

            Assert.Empty(tab.GetUnreadIds());
        }

        [Fact]
        public void SetReadState_MarkAsReadTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            tab.AddPostQueue(new PostClass
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
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            tab.AddPostQueue(new PostClass
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
            var tab = new FilterTabModel("MyTab");

            var filter = new PostFilterRule();
            tab.FilterArray = new[] { filter };

            Assert.Equal(new[] { filter }, tab.FilterArray);
            Assert.True(tab.FilterModified);
        }

        [Fact]
        public void AddFilter_Test()
        {
            var tab = new FilterTabModel("MyTab");

            var filter = new PostFilterRule();
            tab.AddFilter(filter);

            Assert.Equal(new[] { filter }, tab.FilterArray);
            Assert.True(tab.FilterModified);
        }

        [Fact]
        public void RemoveFilter_Test()
        {
            var tab = new FilterTabModel("MyTab");

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
            var tab = new FilterTabModel("MyTab");

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
            var tab = new FilterTabModel("MyTab");

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
            var tab = new PublicSearchTabModel("search");

            tab.AddPostQueue(new PostClass { StatusId = 100L, TextFromApi = "abcd", ScreenName = "", Nickname = "" }); // 0
            tab.AddPostQueue(new PostClass { StatusId = 110L, TextFromApi = "efgh", ScreenName = "", Nickname = "" }); // 1
            tab.AddPostQueue(new PostClass { StatusId = 120L, TextFromApi = "ijkl", ScreenName = "", Nickname = "" }); // 2
            tab.AddPostQueue(new PostClass { StatusId = 130L, TextFromApi = "abc", ScreenName = "", Nickname = "" });  // 3
            tab.AddPostQueue(new PostClass { StatusId = 140L, TextFromApi = "def", ScreenName = "", Nickname = "" });  // 4

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
            var tab = new PublicSearchTabModel("search");

            tab.AddPostQueue(new PostClass { StatusId = 100L, TextFromApi = "abcd", ScreenName = "", Nickname = "" }); // 0
            tab.AddPostQueue(new PostClass { StatusId = 110L, TextFromApi = "efgh", ScreenName = "", Nickname = "" }); // 1
            tab.AddPostQueue(new PostClass { StatusId = 120L, TextFromApi = "ijkl", ScreenName = "", Nickname = "" }); // 2
            tab.AddPostQueue(new PostClass { StatusId = 130L, TextFromApi = "abc", ScreenName = "", Nickname = "" });  // 3
            tab.AddPostQueue(new PostClass { StatusId = 140L, TextFromApi = "def", ScreenName = "", Nickname = "" });  // 4

            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
            tab.AddSubmit();

            // インデックス番号 4 から逆順に開始 → 4, 3, 2, 1, 0 の順に検索
            var result = tab.SearchPostsAll(x => x.Contains("a"), startIndex: 4, reverse: true);
            Assert.Equal(new[] { 3, 0 }, result);

            // インデックス番号 2 から逆順に開始 → 2, 1, 0, 4, 3 の順に検索
            result = tab.SearchPostsAll(x => x.Contains("a"), startIndex: 2, reverse: true);
            Assert.Equal(new[] { 0, 3 }, result);
        }

        [Fact]
        public void GetterSingle_Test()
        {
            var tab = new PublicSearchTabModel("search");

            tab.AddPostQueue(new PostClass { StatusId = 100L, TextFromApi = "abcd" }); // 0
            tab.AddPostQueue(new PostClass { StatusId = 110L, TextFromApi = "efgh" }); // 1
            tab.AddPostQueue(new PostClass { StatusId = 120L, TextFromApi = "ijkl" }); // 2

            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
            tab.AddSubmit();

            Assert.Equal(100L, tab[0].StatusId);
            Assert.Equal(120L, tab[2].StatusId);
        }

        [Fact]
        public void GetterSingle_ErrorTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.AddPostQueue(new PostClass { StatusId = 100L, TextFromApi = "abcd" }); // 0
            tab.AddPostQueue(new PostClass { StatusId = 110L, TextFromApi = "efgh" }); // 1
            tab.AddPostQueue(new PostClass { StatusId = 120L, TextFromApi = "ijkl" }); // 2

            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
            tab.AddSubmit();

            Assert.Throws<ArgumentOutOfRangeException>(() => tab[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => tab[3]);
        }

        [Fact]
        public void GetterSlice_Test()
        {
            var tab = new PublicSearchTabModel("search");

            tab.AddPostQueue(new PostClass { StatusId = 100L, TextFromApi = "abcd" }); // 0
            tab.AddPostQueue(new PostClass { StatusId = 110L, TextFromApi = "efgh" }); // 1
            tab.AddPostQueue(new PostClass { StatusId = 120L, TextFromApi = "ijkl" }); // 2

            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
            tab.AddSubmit();

            Assert.Equal(new[] { 100L, 110L, 120L }, tab[0, 2].Select(x => x.StatusId));
            Assert.Equal(new[] { 100L }, tab[0, 0].Select(x => x.StatusId));
            Assert.Equal(new[] { 120L }, tab[2, 2].Select(x => x.StatusId));
        }

        [Fact]
        public void GetterSlice_ErrorTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.AddPostQueue(new PostClass { StatusId = 100L, TextFromApi = "abcd" }); // 0
            tab.AddPostQueue(new PostClass { StatusId = 110L, TextFromApi = "efgh" }); // 1
            tab.AddPostQueue(new PostClass { StatusId = 120L, TextFromApi = "ijkl" }); // 2

            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
            tab.AddSubmit();

            Assert.Throws<ArgumentOutOfRangeException>(() => tab[-2, -1]); // 範囲外 ... 範囲外
            Assert.Throws<ArgumentOutOfRangeException>(() => tab[-1, 1]);  // 範囲外 ... 範囲内
            Assert.Throws<ArgumentOutOfRangeException>(() => tab[2, 3]);   // 範囲内 ... 範囲外
            Assert.Throws<ArgumentOutOfRangeException>(() => tab[3, 4]);   // 範囲外 ... 範囲外
            Assert.Throws<ArgumentOutOfRangeException>(() => tab[-1, 3]);  // 範囲外 ... 範囲外

            Assert.Throws<ArgumentException>(() => tab[2, 0]); // 範囲内だが startIndex > endIndex
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
            => Assert.Equal(expected, tabType.IsDefault());

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
            => Assert.Equal(expected, tabType.IsDistributable());

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
            => Assert.Equal(expected, tabType.IsInnerStorage());
    }
}
