﻿// OpenTween - Client of Twitter
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
                new PostClass { StatusId = new TwitterStatusId("100") },
                new PostClass { StatusId = new TwitterStatusId("110") },
            };
            tab.AddPostQueue(posts[0]);
            tab.AddPostQueue(posts[1]);
            tab.AddSubmit();

            Assert.Null(tab.AnchorStatusId);
            Assert.Null(tab.AnchorPost);

            tab.AnchorPost = posts[1];

            Assert.Equal(new TwitterStatusId("110"), tab.AnchorStatusId);
            Assert.Equal(new TwitterStatusId("110"), tab.AnchorPost.StatusId);
        }

        [Fact]
        public void AnchorPost_DeletedTest()
        {
            var tab = new PublicSearchTabModel("search");

            var posts = new[]
            {
                new PostClass { StatusId = new TwitterStatusId("100") },
            };
            tab.AddPostQueue(posts[0]);
            tab.AddSubmit();
            tab.AnchorPost = posts[0];

            Assert.Equal(new TwitterStatusId("100"), tab.AnchorPost.StatusId);

            tab.EnqueueRemovePost(new TwitterStatusId("100"), setIsDeleted: true);
            tab.RemoveSubmit();

            Assert.Null(tab.AnchorPost);
        }

        [Fact]
        public void ClearAnchor_Test()
        {
            var tab = new PublicSearchTabModel("search");

            var posts = new[]
            {
                new PostClass { StatusId = new TwitterStatusId("100") },
            };
            tab.AddPostQueue(posts[0]);
            tab.AddSubmit();
            tab.AnchorPost = posts[0];

            Assert.Equal(new TwitterStatusId("100"), tab.AnchorPost.StatusId);
            tab.ClearAnchor();
            Assert.Null(tab.AnchorPost);
        }

        [Fact]
        public void SelectPosts_Test()
        {
            var tab = new PublicSearchTabModel("search");

            var posts = new[]
            {
                new PostClass { StatusId = new TwitterStatusId("100") },
                new PostClass { StatusId = new TwitterStatusId("110") },
                new PostClass { StatusId = new TwitterStatusId("120") },
            };
            tab.AddPostQueue(posts[0]);
            tab.AddPostQueue(posts[1]);
            tab.AddPostQueue(posts[2]);
            tab.AddSubmit();

            tab.SelectPosts(new[] { 0, 2 });

            Assert.Equal(new[] { new TwitterStatusId("100"), new TwitterStatusId("120") }, tab.SelectedStatusIds);
            Assert.Equal(new TwitterStatusId("100"), tab.SelectedStatusId);
            Assert.Equal(new[] { posts[0], posts[2] }, tab.SelectedPosts);
            Assert.Equal(posts[0], tab.SelectedPost);
            Assert.Equal(0, tab.SelectedIndex);
        }

        [Fact]
        public void SelectPosts_EmptyTest()
        {
            var tab = new PublicSearchTabModel("search");
            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("100") });
            tab.AddSubmit();

            tab.SelectPosts(Array.Empty<int>());

            Assert.Empty(tab.SelectedStatusIds);
            Assert.Null(tab.SelectedStatusId);
            Assert.Empty(tab.SelectedPosts);
            Assert.Null(tab.SelectedPost);
            Assert.Equal(-1, tab.SelectedIndex);
        }

        [Fact]
        public void SelectPosts_InvalidIndexTest()
        {
            var tab = new PublicSearchTabModel("search");
            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("100") });
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

            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("100"), IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("110"), IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("120"), IsRead = false });

            tab.AddSubmit();

            Assert.Equal(3, tab.AllCount);
            Assert.Equal(3, tab.UnreadCount);

            tab.EnqueueRemovePost(new TwitterStatusId("100"), setIsDeleted: false);

            // この時点では削除は行われない
            Assert.Equal(3, tab.AllCount);
            Assert.Equal(3, tab.UnreadCount);

            var removedIds = tab.RemoveSubmit();

            Assert.Equal(2, tab.AllCount);
            Assert.Equal(2, tab.UnreadCount);
            Assert.Equal(new[] { new TwitterStatusId("110"), new TwitterStatusId("120") }, tab.StatusIds);
            Assert.Equal(new[] { new TwitterStatusId("100") }, removedIds.AsEnumerable());
        }

        [Fact]
        public void EnqueueRemovePost_SetIsDeletedTest()
        {
            var tab = new PublicSearchTabModel("search")
            {
                UnreadManage = true,
            };

            var post = new PostClass { StatusId = new TwitterStatusId("100"), IsRead = false };
            tab.AddPostQueue(post);
            tab.AddSubmit();

            Assert.Equal(1, tab.AllCount);
            Assert.Equal(1, tab.UnreadCount);

            tab.EnqueueRemovePost(new TwitterStatusId("100"), setIsDeleted: true);

            // この時点ではタブからの削除は行われないが、PostClass.IsDeleted は true にセットされる
            Assert.Equal(1, tab.AllCount);
            Assert.Equal(1, tab.UnreadCount);
            Assert.True(post.IsDeleted);

            var removedIds = tab.RemoveSubmit();

            Assert.Equal(0, tab.AllCount);
            Assert.Equal(0, tab.UnreadCount);
            Assert.Equal(new[] { new TwitterStatusId("100") }, removedIds.AsEnumerable());
        }

        [Fact]
        public void EnqueueRemovePost_UnknownIdTest()
        {
            var tab = new PublicSearchTabModel("search")
            {
                UnreadManage = true,
            };

            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("100"), IsRead = false });
            tab.AddSubmit();

            Assert.Equal(1, tab.AllCount);
            Assert.Equal(1, tab.UnreadCount);

            // StatusId = 999 は存在しない
            tab.EnqueueRemovePost(new TwitterStatusId("999"), setIsDeleted: false);

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
            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("100") });
            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("110") });
            tab.AddSubmit();
            tab.SelectPosts(new[] { 0, 1 });

            Assert.Equal(2, tab.AllCount);
            Assert.Equal(new[] { new TwitterStatusId("100"), new TwitterStatusId("110") }, tab.SelectedStatusIds);

            tab.EnqueueRemovePost(new TwitterStatusId("100"), setIsDeleted: false);

            // この時点では変化しない
            Assert.Equal(2, tab.AllCount);
            Assert.Equal(new[] { new TwitterStatusId("100"), new TwitterStatusId("110") }, tab.SelectedStatusIds);

            tab.RemoveSubmit();

            // 削除された発言の選択が解除される
            Assert.Equal(1, tab.AllCount);
            Assert.Equal(new[] { new TwitterStatusId("110") }, tab.SelectedStatusIds);
        }

        [Fact]
        public void ReplacePost_SuccessTest()
        {
            var tab = new PublicSearchTabModel("search");
            var origPost = new PostClass { StatusId = new TwitterStatusId("100") };
            tab.AddPostQueue(origPost);
            tab.AddSubmit();

            Assert.Same(origPost, tab.Posts[new TwitterStatusId("100")]);

            var newPost = new PostClass { StatusId = new TwitterStatusId("100"), InReplyToStatusId = new TwitterStatusId("200") };
            Assert.True(tab.ReplacePost(newPost));
            Assert.Same(newPost, tab.Posts[new TwitterStatusId("100")]);
        }

        [Fact]
        public void ReplacePost_FailedTest()
        {
            var tab = new PublicSearchTabModel("search");
            Assert.False(tab.Contains(new TwitterStatusId("100")));

            var newPost = new PostClass { StatusId = new TwitterStatusId("100"), InReplyToStatusId = new TwitterStatusId("200") };
            Assert.False(tab.ReplacePost(newPost));
        }

        [Fact]
        public void NextUnreadId_Test()
        {
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            // 未読なし
            Assert.Null(tab.NextUnreadId);

            tab.AddPostQueue(new PostClass
            {
                StatusId = new TwitterStatusId("100"),
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(new TwitterStatusId("100"), tab.NextUnreadId);

            tab.AddPostQueue(new PostClass
            {
                StatusId = new TwitterStatusId("50"),
                IsRead = true, // 既読
            });
            tab.AddSubmit();

            Assert.Equal(new TwitterStatusId("100"), tab.NextUnreadId);
        }

        [Fact]
        public void NextUnreadId_DisabledTest()
        {
            var tab = new PublicSearchTabModel("search");

            // 未読表示無効
            tab.UnreadManage = false;

            tab.AddPostQueue(new PostClass
            {
                StatusId = new TwitterStatusId("100"),
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Null(tab.NextUnreadId);
        }

        [Fact]
        public void NextUnreadId_SortByIdAscTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            // ID (CreatedAtForSorting) の昇順でソート
            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);

            // 画面には上から 100 → 200 → 300 の順に並ぶ
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 1),
                IsRead = false,
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("200"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 2),
                IsRead = false,
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("300"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 3),
                IsRead = false,
            });
            tab.AddSubmit();

            // 昇順/降順に関わらず、CreatedAtForSorting の小さい順に未読の ID を返す
            Assert.Equal(new TwitterStatusId("100"), tab.NextUnreadId);
        }

        [Fact]
        public void NextUnreadId_SortByIdDescTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            // ID (CreatedAtForSorting) の降順でソート
            tab.SetSortMode(ComparerMode.Id, SortOrder.Descending);

            // 画面には上から 300 → 200 → 100 の順に並ぶ
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 1),
                IsRead = false,
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("200"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 2),
                IsRead = false,
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("300"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 3),
                IsRead = false,
            });
            tab.AddSubmit();

            // 昇順/降順に関わらず、CreatedAtForSorting の小さい順に未読の ID を返す
            Assert.Equal(new TwitterStatusId("100"), tab.NextUnreadId);
        }

        [Fact]
        public void NextUnreadId_SortByScreenNameAscTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            // ScreenName の昇順でソート
            tab.SetSortMode(ComparerMode.Name, SortOrder.Ascending);

            // 画面には上から 200 → 100 → 300 の順に並ぶ
            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("100"), ScreenName = "bbb", IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("200"), ScreenName = "aaa", IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("300"), ScreenName = "ccc", IsRead = false });
            tab.AddSubmit();

            // 昇順/降順に関わらず、ScreenName の辞書順で小さい順に未読の ID を返す
            Assert.Equal(new TwitterStatusId("200"), tab.NextUnreadId);
        }

        [Fact]
        public void NextUnreadId_SortByScreenNameDescTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            // ScreenName の降順でソート
            tab.SetSortMode(ComparerMode.Name, SortOrder.Descending);

            // 画面には上から 300 → 100 → 200 の順に並ぶ
            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("100"), ScreenName = "bbb", IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("200"), ScreenName = "aaa", IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("300"), ScreenName = "ccc", IsRead = false });
            tab.AddSubmit();

            // 昇順/降順に関わらず、ScreenName の辞書順で小さい順に未読の ID を返す
            Assert.Equal(new TwitterStatusId("200"), tab.NextUnreadId);
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
                StatusId = new TwitterStatusId("100"),
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(1, tab.UnreadCount);

            tab.AddPostQueue(new PostClass
            {
                StatusId = new TwitterStatusId("50"),
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
                StatusId = new TwitterStatusId("100"),
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
                StatusId = new TwitterStatusId("50"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 1),
                IsRead = true, // 既読
            });
            tab.AddPostQueue(new PostClass
            {
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 2),
                IsRead = false, // 未読
            });
            tab.AddPostQueue(new PostClass
            {
                StatusId = new TwitterStatusId("150"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 3),
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
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 1),
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

            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("100"), IsRead = false });
            tab.AddPostQueue(new PostClass { StatusId = new TwitterStatusId("200"), IsRead = true });
            tab.AddSubmit();

            Assert.Equal(new[] { new TwitterStatusId("100") }, tab.GetUnreadIds());

            tab.SetReadState(new TwitterStatusId("100"), true); // 既読にする

            Assert.Empty(tab.GetUnreadIds());
        }

        [Fact]
        public void SetReadState_MarkAsReadTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            tab.AddPostQueue(new PostClass
            {
                StatusId = new TwitterStatusId("100"),
                IsRead = false, // 未読
            });
            tab.AddSubmit();

            Assert.Equal(1, tab.UnreadCount);

            tab.SetReadState(new TwitterStatusId("100"), true); // 既読にする

            Assert.Equal(0, tab.UnreadCount);
        }

        [Fact]
        public void SetReadState_MarkAsUnreadTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.UnreadManage = true;

            tab.AddPostQueue(new PostClass
            {
                StatusId = new TwitterStatusId("100"),
                IsRead = true, // 既読
            });
            tab.AddSubmit();

            Assert.Equal(0, tab.UnreadCount);

            tab.SetReadState(new TwitterStatusId("100"), false); // 未読にする

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
        public void IndexOf_SingleFoundTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 0),
                TextFromApi = "aaa",
            });
            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
            tab.AddSubmit();

            Assert.Equal(0, tab.IndexOf(new TwitterStatusId("100")));
        }

        [Fact]
        public void IndexOf_SingleNotFoundTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 0),
                TextFromApi = "aaa",
            });
            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
            tab.AddSubmit();

            Assert.Equal(-1, tab.IndexOf(new TwitterStatusId("200")));
        }

        [Fact]
        public void IndexOf_MultipleFoundTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 0),
                TextFromApi = "aaa",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("200"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 1),
                TextFromApi = "bbb",
            });
            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
            tab.AddSubmit();

            var actual = tab.IndexOf(new[] { new TwitterStatusId("200"), new TwitterStatusId("100") });
            Assert.Equal(new[] { 1, 0 }, actual);
        }

        [Fact]
        public void IndexOf_MultiplePartiallyFoundTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 0),
                TextFromApi = "aaa",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("200"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 1),
                TextFromApi = "bbb",
            });
            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
            tab.AddSubmit();

            var actual = tab.IndexOf(new[] { new TwitterStatusId("100"), new TwitterStatusId("999") });
            Assert.Equal(new[] { 0, -1 }, actual);
        }

        [Fact]
        public void SearchPostsAll_Test()
        {
            var tab = new PublicSearchTabModel("search");

            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 0), // 0
                TextFromApi = "abcd",
                ScreenName = "",
                Nickname = "",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("110"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 1), // 1
                TextFromApi = "efgh",
                ScreenName = "",
                Nickname = "",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("120"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 2), // 2
                TextFromApi = "ijkl",
                ScreenName = "",
                Nickname = "",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("130"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 3), // 3
                TextFromApi = "abc",
                ScreenName = "",
                Nickname = "",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("140"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 4), // 4
                TextFromApi = "def",
                ScreenName = "",
                Nickname = "",
            });

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

            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 0), // 0
                TextFromApi = "abcd",
                ScreenName = "",
                Nickname = "",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("110"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 1), // 1
                TextFromApi = "efgh",
                ScreenName = "",
                Nickname = "",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("120"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 2), // 2
                TextFromApi = "ijkl",
                ScreenName = "",
                Nickname = "",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("130"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 3), // 3
                TextFromApi = "abc",
                ScreenName = "",
                Nickname = "",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("140"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 4), // 4
                TextFromApi = "def",
                ScreenName = "",
                Nickname = "",
            });

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

            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 0), // 0
                TextFromApi = "abcd",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("110"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 1), // 1
                TextFromApi = "efgh",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("120"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 2), // 2
                TextFromApi = "ijkl",
            });

            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
            tab.AddSubmit();

            Assert.Equal(new TwitterStatusId("100"), tab[0].StatusId);
            Assert.Equal(new TwitterStatusId("120"), tab[2].StatusId);
        }

        [Fact]
        public void GetterSingle_ErrorTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 0), // 0
                TextFromApi = "abcd",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("110"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 1), // 1
                TextFromApi = "efgh",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("120"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 2), // 2
                TextFromApi = "ijkl",
            });

            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
            tab.AddSubmit();

            Assert.Throws<ArgumentOutOfRangeException>(() => tab[-1]);
            Assert.Throws<ArgumentOutOfRangeException>(() => tab[3]);
        }

        [Fact]
        public void GetterSlice_Test()
        {
            var tab = new PublicSearchTabModel("search");

            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 0), // 0
                TextFromApi = "abcd",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("110"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 1), // 1
                TextFromApi = "efgh",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("120"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 2), // 2
                TextFromApi = "ijkl",
            });

            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
            tab.AddSubmit();

            Assert.Equal(new[] { new TwitterStatusId("100"), new TwitterStatusId("110"), new TwitterStatusId("120") }, tab[0, 2].Select(x => x.StatusId));
            Assert.Equal(new[] { new TwitterStatusId("100") }, tab[0, 0].Select(x => x.StatusId));
            Assert.Equal(new[] { new TwitterStatusId("120") }, tab[2, 2].Select(x => x.StatusId));
        }

        [Fact]
        public void GetterSlice_ErrorTest()
        {
            var tab = new PublicSearchTabModel("search");

            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("100"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 0), // 0
                TextFromApi = "abcd",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("110"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 1), // 1
                TextFromApi = "efgh",
            });
            tab.AddPostQueue(new()
            {
                StatusId = new TwitterStatusId("120"),
                CreatedAtForSorting = new(2023, 1, 1, 0, 0, 2), // 2
                TextFromApi = "ijkl",
            });

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
