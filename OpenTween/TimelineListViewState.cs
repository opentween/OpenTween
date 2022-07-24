// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

#nullable enable

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenTween.Models;
using OpenTween.OpenTweenCustomControl;

namespace OpenTween
{
    public class TimelineListViewState
    {
        private readonly DetailsListView listView;
        private readonly TabModel tab;

        private ListViewScroll savedScrollState;
        private ListViewSelection savedSelectionState;

        internal readonly record struct ListViewScroll(
            ScrollLockMode ScrollLockMode,
            long? TopItemStatusId
        );

        internal enum ScrollLockMode
        {
            /// <summary>固定しない</summary>
            None,

            /// <summary>最上部に固定する</summary>
            FixedToTop,

            /// <summary>最下部に固定する</summary>
            FixedToBottom,

            /// <summary><see cref="ListViewScroll.TopItemStatusId"/> の位置に固定する</summary>
            FixedToItem,
        }

        internal readonly record struct ListViewSelection(
            long[] SelectedStatusIds,
            long? SelectionMarkStatusId,
            long? FocusedStatusId
        );

        public TimelineListViewState(DetailsListView listView, TabModel tab)
        {
            this.listView = listView;
            this.tab = tab;
        }

        public void Save(bool lockScroll)
        {
            this.savedScrollState = this.SaveListViewScroll(lockScroll);
            this.savedSelectionState = this.SaveListViewSelection();
        }

        public void Restore()
        {
            this.RestoreScroll();
            this.RestoreSelection();
        }

        public void RestoreScroll()
            => this.RestoreListViewScroll(this.savedScrollState);

        public void RestoreSelection()
            => this.RestoreListViewSelection(this.savedSelectionState);

        /// <summary>
        /// <see cref="ListView"/> のスクロール位置に関する情報を <see cref="ListViewScroll"/> として返します
        /// </summary>
        private ListViewScroll SaveListViewScroll(bool lockScroll)
        {
            var lockMode = this.GetScrollLockMode(lockScroll);
            long? topItemStatusId = null;

            if (lockMode == ScrollLockMode.FixedToItem)
            {
                var topItemIndex = this.listView.TopItem?.Index ?? -1;
                if (topItemIndex != -1 && topItemIndex < this.tab.AllCount)
                    topItemStatusId = this.tab.GetStatusIdAt(topItemIndex);
            }

            return new ListViewScroll
            {
                ScrollLockMode = lockMode,
                TopItemStatusId = topItemStatusId,
            };
        }

        private ScrollLockMode GetScrollLockMode(bool lockScroll)
        {
            if (this.tab.SortMode == ComparerMode.Id)
            {
                if (this.tab.SortOrder == SortOrder.Ascending)
                {
                    // Id昇順
                    if (lockScroll)
                        return ScrollLockMode.None;

                    // 最下行が表示されていたら、最下行へ強制スクロール。最下行が表示されていなかったら制御しない

                    // 一番下に表示されているアイテム
                    var bottomItem = this.listView.GetItemAt(0, this.listView.ClientSize.Height - 1);
                    if (bottomItem == null || bottomItem.Index == this.listView.VirtualListSize - 1)
                        return ScrollLockMode.FixedToBottom;
                    else
                        return ScrollLockMode.None;
                }
                else
                {
                    // Id降順
                    if (lockScroll)
                        return ScrollLockMode.FixedToItem;

                    // 最上行が表示されていたら、制御しない。最上行が表示されていなかったら、現在表示位置へ強制スクロール
                    var topItem = this.listView.TopItem;
                    if (topItem == null || topItem.Index == 0)
                        return ScrollLockMode.FixedToTop;
                    else
                        return ScrollLockMode.FixedToItem;
                }
            }
            else
            {
                return ScrollLockMode.FixedToItem;
            }
        }

        /// <summary>
        /// <see cref="ListView"/> の選択状態を <see cref="ListViewSelection"/> として返します
        /// </summary>
        private ListViewSelection SaveListViewSelection()
        {
            if (this.listView.VirtualListSize == 0)
            {
                return new ListViewSelection
                {
                    SelectedStatusIds = Array.Empty<long>(),
                    SelectionMarkStatusId = null,
                    FocusedStatusId = null,
                };
            }

            return new ListViewSelection
            {
                SelectedStatusIds = this.tab.SelectedStatusIds,
                FocusedStatusId = this.GetFocusedStatusId(),
                SelectionMarkStatusId = this.GetSelectionMarkStatusId(),
            };
        }

        private long? GetFocusedStatusId()
        {
            var index = this.listView.FocusedItem?.Index ?? -1;

            return index != -1 && index < this.tab.AllCount ? this.tab.GetStatusIdAt(index) : (long?)null;
        }

        private long? GetSelectionMarkStatusId()
        {
            var index = this.listView.SelectionMark;

            return index != -1 && index < this.tab.AllCount ? this.tab.GetStatusIdAt(index) : (long?)null;
        }

        /// <summary>
        /// <see cref="SaveListViewScroll"/> によって保存されたスクロール位置を復元します
        /// </summary>
        private void RestoreListViewScroll(ListViewScroll listScroll)
        {
            if (this.listView.VirtualListSize == 0)
                return;

            switch (listScroll.ScrollLockMode)
            {
                case ScrollLockMode.FixedToTop:
                    this.listView.EnsureVisible(0);
                    break;
                case ScrollLockMode.FixedToBottom:
                    this.listView.EnsureVisible(this.listView.VirtualListSize - 1);
                    break;
                case ScrollLockMode.FixedToItem:
                    var topIndex = listScroll.TopItemStatusId != null ? this.tab.IndexOf(listScroll.TopItemStatusId.Value) : -1;
                    if (topIndex != -1)
                    {
                        var topItem = this.listView.Items[topIndex];
                        try
                        {
                            this.listView.TopItem = topItem;
                        }
                        catch (NullReferenceException)
                        {
                            this.listView.EnsureVisible(this.listView.VirtualListSize - 1);
                            this.listView.EnsureVisible(topIndex);
                        }
                    }
                    break;
                case ScrollLockMode.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// <see cref="SaveListViewSelection"/> によって保存された選択状態を復元します
        /// </summary>
        private void RestoreListViewSelection(ListViewSelection listSelection)
        {
            var invalidateBounds = (Rectangle?)null;

            var previousFocusedItem = this.listView.FocusedItem;
            if (previousFocusedItem != null)
                invalidateBounds = previousFocusedItem.Bounds;

            // status_id から ListView 上のインデックスに変換
            if (listSelection.SelectedStatusIds != null)
            {
                var selectedIndices = this.tab.IndexOf(listSelection.SelectedStatusIds).Where(x => x != -1).ToArray();
                this.listView.SelectItems(selectedIndices);
            }

            if (listSelection.FocusedStatusId != null)
            {
                var focusedIndex = this.tab.IndexOf(listSelection.FocusedStatusId.Value);
                if (focusedIndex != -1)
                    this.listView.Items[focusedIndex].Focused = true;
            }

            if (listSelection.SelectionMarkStatusId != null)
            {
                var selectionMarkIndex = this.tab.IndexOf(listSelection.SelectionMarkStatusId.Value);
                if (selectionMarkIndex != -1)
                    this.listView.SelectionMark = selectionMarkIndex;
            }

            if (invalidateBounds != null)
                this.listView.Invalidate(invalidateBounds.Value);
        }
    }
}
