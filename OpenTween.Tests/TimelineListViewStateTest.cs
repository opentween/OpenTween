// OpenTween - Client of Twitter
// Copyright (c) 2023 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Linq;
using System.Windows.Forms;
using OpenTween.Models;
using OpenTween.OpenTweenCustomControl;
using Xunit;

namespace OpenTween
{
    public class TimelineListViewStateTest
    {
        [WinFormsFact]
        public void Initialize_Test()
        {
            using var listView = new DetailsListView();
            var tab = new PublicSearchTabModel("hoge");
            var listViewState = new TimelineListViewState(listView, tab);
        }

        private void UsingListView(int count, Action<DetailsListView, TabModel> func)
        {
            using var listView = new DetailsListView();
            listView.Columns.Add("col");
            listView.HeaderStyle = ColumnHeaderStyle.None; // 座標計算の邪魔になるため非表示にする

            listView.RetrieveVirtualItem += (s, e) => e.Item = new($"text {e.ItemIndex}");
            listView.VirtualMode = true;
            listView.VirtualListSize = count;

            using var imageList = new ImageList { ImageSize = new(1, 19) };
            listView.SmallImageList = imageList; // Item の高さは 20px
            listView.ClientSize = new(100, 100); // 高さは 5 行分
            listView.CreateControl();

            var tab = new PublicSearchTabModel("hoge");
            tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);

            var firstDateTime = new DateTimeUtc(2024, 1, 1, 0, 0, 0);
            foreach (var i in MyCommon.CountUp(0, count - 1))
            {
                var post = new PostClass
                {
                    StatusId = new TwitterStatusId(i.ToString()),
                    CreatedAtForSorting = firstDateTime + TimeSpan.FromSeconds(i),
                };
                tab.AddPostQueue(post);
            }
            tab.AddSubmit();

            func(listView, tab);
        }

        [WinFormsFact]
        public void GetScrollLockMode_IdAsc_Test()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
                listView.EnsureVisible(0); // 一番上にスクロール

                // 投稿日時の昇順に並んでいる場合、新着投稿によってスクロール位置がズレることがないため None を返す
                Assert.Equal(
                    TimelineListViewState.ScrollLockMode.None,
                    listViewState.GetScrollLockMode(lockScroll: false)
                );
            });
        }

        [WinFormsFact]
        public void GetScrollLockMode_IdAsc_BottomTest()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
                listView.EnsureVisible(9); // 一番下までスクロール

                // 最終行が表示されている場合はスクロール位置を一番下に固定する（新着投稿を表示し続ける）
                Assert.Equal(
                    TimelineListViewState.ScrollLockMode.FixedToBottom,
                    listViewState.GetScrollLockMode(lockScroll: false)
                );
            });
        }

        [WinFormsFact]
        public void GetScrollLockMode_IdAsc_BottomLockScrollTest()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
                listView.EnsureVisible(9); // 一番下までスクロール

                // 最終行が表示されていても lockScroll が true の場合は None を返す（新着投稿にスクロールしない）
                Assert.Equal(
                    TimelineListViewState.ScrollLockMode.None,
                    listViewState.GetScrollLockMode(lockScroll: true)
                );
            });
        }

        [WinFormsFact]
        public void GetScrollLockMode_IdDesc_TopTest()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                tab.SetSortMode(ComparerMode.Id, SortOrder.Descending);
                listView.EnsureVisible(0); // 一番上にスクロール

                // 投稿日時の降順に並んでいて 1 行目が表示されている場合はスクロール位置を一番上に固定する（新着投稿を表示し続ける）
                Assert.Equal(
                    TimelineListViewState.ScrollLockMode.FixedToTop,
                    listViewState.GetScrollLockMode(lockScroll: false)
                );
            });
        }

        [WinFormsFact]
        public void GetScrollLockMode_IdDesc_TopLockScrollTest()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                tab.SetSortMode(ComparerMode.Id, SortOrder.Descending);
                listView.EnsureVisible(0); // 一番上にスクロール

                // 先頭行が表示されていても lockScroll が true の場合は FixedToItem を返す（新着投稿にスクロールしない）
                Assert.Equal(
                    TimelineListViewState.ScrollLockMode.FixedToItem,
                    listViewState.GetScrollLockMode(lockScroll: true)
                );
            });
        }

        [WinFormsFact]
        public void GetScrollLockMode_IdDesc_BottomTest()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                tab.SetSortMode(ComparerMode.Id, SortOrder.Descending);
                listView.EnsureVisible(9); // 一番下にスクロール

                // 先頭行が表示されていない場合は FixedToItem を返す
                Assert.Equal(
                    TimelineListViewState.ScrollLockMode.FixedToItem,
                    listViewState.GetScrollLockMode(lockScroll: false)
                );
            });
        }

        [WinFormsFact]
        public void GetScrollLockMode_TextAsc_Test()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                tab.SetSortMode(ComparerMode.Data, SortOrder.Ascending);
                listView.EnsureVisible(0); // 一番上にスクロール

                // ComparerMode.Id 以外の場合は常に FixedToItem を返す
                Assert.Equal(
                    TimelineListViewState.ScrollLockMode.FixedToItem,
                    listViewState.GetScrollLockMode(lockScroll: false)
                );
            });
        }

        [WinFormsFact]
        public void GetListViewScroll_Test()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                tab.SetSortMode(ComparerMode.Id, SortOrder.Descending); // 投稿日時の降順
                listView.TopItem = listView.Items[2]; // 3 行目が一番上になる位置にスクロールされた状態

                var scrollState = listViewState.GetListViewScroll(lockScroll: false);
                Assert.Equal(
                    TimelineListViewState.ScrollLockMode.FixedToItem,
                    scrollState.ScrollLockMode
                );
                Assert.Equal(new TwitterStatusId("7"), scrollState.TopItemStatusId); // 3 行目の StatusId は "7"
            });
        }

        [WinFormsFact]
        public void RestoreListViewScroll_FixedToTopTest()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
                listView.TopItem = listView.Items[2]; // 3 行目が一番上になる位置にスクロールされた状態

                var scrollState = new TimelineListViewState.ListViewScroll(
                    ScrollLockMode: TimelineListViewState.ScrollLockMode.FixedToTop,
                    TopItemStatusId: null
                );
                listViewState.RestoreListViewScroll(scrollState, forceScroll: false);

                // 一番上にスクロールされた状態になる
                Assert.Equal(0, listView.TopItem.Index);
            });
        }

        [WinFormsFact]
        public void RestoreListViewScroll_FixedToBottomTest()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
                listView.TopItem = listView.Items[2]; // 3 行目が一番上になる位置にスクロールされた状態

                var scrollState = new TimelineListViewState.ListViewScroll(
                    ScrollLockMode: TimelineListViewState.ScrollLockMode.FixedToBottom,
                    TopItemStatusId: null
                );
                listViewState.RestoreListViewScroll(scrollState, forceScroll: false);

                // 一番下にスクロールされた状態になる（一番下に余白が生じるため null になる）
                Assert.Null(listView.GetItemAt(0, 82)?.Index);
            });
        }

        [WinFormsFact]
        public void RestoreListViewScroll_FixedToItemTest()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
                listView.TopItem = listView.Items[2]; // 3 行目が一番上になる位置にスクロールされた状態

                var scrollState = new TimelineListViewState.ListViewScroll(
                    ScrollLockMode: TimelineListViewState.ScrollLockMode.FixedToItem,
                    TopItemStatusId: new TwitterStatusId("5")
                );
                listViewState.RestoreListViewScroll(scrollState, forceScroll: false);

                // 6 行目が一番上になる位置にスクロールされた状態になる
                Assert.Equal(5, listView.TopItem.Index);
            });
        }

        [WinFormsFact]
        public void RestoreListViewScroll_NoneTest()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
                listView.TopItem = listView.Items[2]; // 3 行目が一番上になる位置にスクロールされた状態

                var scrollState = new TimelineListViewState.ListViewScroll(
                    ScrollLockMode: TimelineListViewState.ScrollLockMode.None,
                    TopItemStatusId: new TwitterStatusId("5")
                );
                listViewState.RestoreListViewScroll(scrollState, forceScroll: false);

                // ScrollLockMode.None の場合は何もしない
                Assert.Equal(2, listView.TopItem.Index);
            });
        }

        [WinFormsFact]
        public void RestoreListViewScroll_ForceScrollTest()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                tab.SetSortMode(ComparerMode.Id, SortOrder.Ascending);
                listView.TopItem = listView.Items[2]; // 3 行目が一番上になる位置にスクロールされた状態

                var scrollState = new TimelineListViewState.ListViewScroll(
                    ScrollLockMode: TimelineListViewState.ScrollLockMode.None,
                    TopItemStatusId: new TwitterStatusId("5")
                );
                listViewState.RestoreListViewScroll(scrollState, forceScroll: true);

                // ScrollLockMode.None でも forceScroll が true の場合は FixedToItem 相当の動作になる
                Assert.Equal(5, listView.TopItem.Index);
            });
        }

        [WinFormsFact]
        public void GetListViewSelection_EmptyTest()
        {
            this.UsingListView(count: 0, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                var selectionState = listViewState.GetListViewSelection();
                Assert.Empty(selectionState.SelectedStatusIds);
                Assert.Null(selectionState.SelectionMarkStatusId);
                Assert.Null(selectionState.FocusedStatusId);
            });
        }

        [WinFormsFact]
        public void GetListViewSelection_Test()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                listView.SelectedIndices.Add(0);
                listView.SelectedIndices.Add(2);
                listView.SelectedIndices.Add(3);
                tab.SelectPosts(new[] { 0, 2, 3 });
                listView.SelectionMark = 1;
                listView.FocusedItem = listView.Items[3];

                var selectionState = listViewState.GetListViewSelection();
                Assert.Equal(new TwitterStatusId[] { new("0"), new("2"), new("3") }, selectionState.SelectedStatusIds);
                Assert.Equal(new TwitterStatusId("1"), selectionState.SelectionMarkStatusId);
                Assert.Equal(new TwitterStatusId("3"), selectionState.FocusedStatusId);
            });
        }

        [WinFormsFact]
        public void RestoreListViewSelection_Test()
        {
            this.UsingListView(count: 10, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                var selectionState = new TimelineListViewState.ListViewSelection(
                    SelectedStatusIds: new TwitterStatusId[] { new("1"), new("2"), new("3") },
                    SelectionMarkStatusId: new TwitterStatusId("1"),
                    FocusedStatusId: new TwitterStatusId("3")
                );
                listViewState.RestoreListViewSelection(selectionState);

                Assert.Equal(new[] { 1, 2, 3 }, listView.SelectedIndices.Cast<int>());
                Assert.Equal(1, listView.SelectionMark);
                Assert.Equal(3, listView.FocusedItem?.Index);
            });
        }

        [WinFormsFact]
        public void RestoreListViewSelection_EmptyTest()
        {
            this.UsingListView(count: 0, (listView, tab) =>
            {
                var listViewState = new TimelineListViewState(listView, tab);

                var selectionState = new TimelineListViewState.ListViewSelection(
                    SelectedStatusIds: Array.Empty<PostId>(),
                    SelectionMarkStatusId: null,
                    FocusedStatusId: null
                );
                listViewState.RestoreListViewSelection(selectionState);

                Assert.Empty(listView.SelectedIndices);
                Assert.Equal(-1, listView.SelectionMark);
                Assert.Null(listView.FocusedItem?.Index);
            });
        }
    }
}
