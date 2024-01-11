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
using OpenTween.OpenTweenCustomControl;
using Xunit;

namespace OpenTween
{
    public class DetailsListViewTest
    {
        [WinFormsFact]
        public void Initialize_Test()
        {
            using var listView = new DetailsListView();
        }

        [WinFormsFact]
        public void SelectionMark_Test()
        {
            using var listView = new DetailsListView();

            listView.RetrieveVirtualItem += (s, e) => e.Item = new();
            listView.VirtualMode = true;
            listView.VirtualListSize = 10;
            listView.CreateControl();

            listView.SelectionMark = 3;
            Assert.Equal(3, listView.SelectionMark);
        }

        [WinFormsFact]
        public void SelectItems_EmptyTest()
        {
            using var listView = new DetailsListView();

            listView.RetrieveVirtualItem += (s, e) => e.Item = new();
            listView.VirtualMode = true;
            listView.VirtualListSize = 10;
            listView.CreateControl();

            listView.SelectedIndices.Add(1);
            Assert.Single(listView.SelectedIndices);

            listView.SelectItems(Array.Empty<int>());
            Assert.Empty(listView.SelectedIndices);
        }

        [WinFormsFact]
        public void SelectItems_SingleTest()
        {
            using var listView = new DetailsListView();

            listView.RetrieveVirtualItem += (s, e) => e.Item = new();
            listView.VirtualMode = true;
            listView.VirtualListSize = 10;
            listView.CreateControl();

            listView.SelectedIndices.Add(1);
            Assert.Single(listView.SelectedIndices);

            listView.SelectItems(new[] { 2 });
            Assert.Equal(new[] { 2 }, listView.SelectedIndices.Cast<int>());
        }

        [WinFormsFact]
        public void SelectItems_Multiple_ClearAndSelectTest()
        {
            using var listView = new DetailsListView();

            listView.RetrieveVirtualItem += (s, e) => e.Item = new();
            listView.VirtualMode = true;
            listView.VirtualListSize = 10;
            listView.CreateControl();

            listView.SelectedIndices.Add(2);
            listView.SelectedIndices.Add(3);
            Assert.Equal(2, listView.SelectedIndices.Count);

            // Clear して選択し直した方が早いパターン
            listView.SelectItems(new[] { 5, 6 });
            Assert.Equal(new[] { 5, 6 }, listView.SelectedIndices.Cast<int>());
        }

        [WinFormsFact]
        public void SelectItems_Multiple_DeselectAndSelectTest()
        {
            using var listView = new DetailsListView();

            listView.RetrieveVirtualItem += (s, e) => e.Item = new();
            listView.VirtualMode = true;
            listView.VirtualListSize = 10;
            listView.CreateControl();

            listView.SelectedIndices.Add(1);
            listView.SelectedIndices.Add(2);
            listView.SelectedIndices.Add(3);
            Assert.Equal(3, listView.SelectedIndices.Count);

            // 選択範囲の差分だけ更新した方が早いパターン
            listView.SelectItems(new[] { 2, 3, 4 });
            Assert.Equal(new[] { 2, 3, 4 }, listView.SelectedIndices.Cast<int>());
        }

        [WinFormsFact]
        public void SelectItems_OutOfRangeTest()
        {
            using var listView = new DetailsListView();

            listView.RetrieveVirtualItem += (s, e) => e.Item = new();
            listView.VirtualMode = true;
            listView.VirtualListSize = 10;
            listView.CreateControl();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => listView.SelectItems(new[] { -1 })
            );
            Assert.Throws<ArgumentOutOfRangeException>(
                () => listView.SelectItems(new[] { 10 })
            );
        }
    }
}
