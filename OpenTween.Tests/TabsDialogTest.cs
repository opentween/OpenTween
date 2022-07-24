// OpenTween - Client of Twitter
// Copyright (c) 2012 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Windows.Forms;
using OpenTween.Models;
using Xunit;
using Xunit.Extensions;

namespace OpenTween
{
    public class TabsDialogTest
    {
        private readonly TabInformations tabinfo;

        public TabsDialogTest()
        {
            this.tabinfo = (TabInformations)Activator.CreateInstance(typeof(TabInformations), true);

            // タブを追加
            this.tabinfo.AddTab(new HomeTabModel("Recent"));
            this.tabinfo.AddTab(new MentionsTabModel("Reply"));
            this.tabinfo.AddTab(new DirectMessagesTabModel("DM"));
            this.tabinfo.AddTab(new FavoritesTabModel("Favorites"));
            this.tabinfo.AddTab(new FilterTabModel("MyTab1"));

            // 一応 TabInformation.GetInstance() でも取得できるようにする
            var field = typeof(TabInformations).GetField("Instance",
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetField);
            field.SetValue(null, this.tabinfo);
        }

        [Fact]
        public void OKButtonEnabledTest()
        {
            using var dialog = new TabsDialog(this.tabinfo);
            Assert.False(dialog.OK_Button.Enabled);

            dialog.TabList.SelectedIndex = 0;

            Assert.True(dialog.OK_Button.Enabled);

            dialog.TabList.SelectedIndex = -1;

            Assert.False(dialog.OK_Button.Enabled);
        }

        [Fact]
        public void MultiSelectTest()
        {
            using var dialog = new TabsDialog(this.tabinfo);

            // MultiSelect = false (default)
            var firstItem = (TabsDialog.TabListItem)dialog.TabList.Items[0];
            Assert.Null(firstItem.Tab); // 「(新規タブ)」
            Assert.Equal(SelectionMode.One, dialog.TabList.SelectionMode);

            dialog.MultiSelect = true;
            firstItem = (TabsDialog.TabListItem)dialog.TabList.Items[0];
            Assert.NotNull(firstItem.Tab);
            Assert.Equal(SelectionMode.MultiExtended, dialog.TabList.SelectionMode);

            dialog.MultiSelect = false;
            firstItem = (TabsDialog.TabListItem)dialog.TabList.Items[0];
            Assert.Null(firstItem.Tab);
            Assert.Equal(SelectionMode.One, dialog.TabList.SelectionMode);
        }

        [Fact]
        public void DoubleClickTest()
        {
            using var dialog = new TabsDialog(this.tabinfo);

            dialog.TabList.SelectedIndex = -1;
            TestUtils.FireEvent(dialog.TabList, "DoubleClick");

            Assert.Equal(DialogResult.None, dialog.DialogResult);
            Assert.False(dialog.IsDisposed);

            dialog.TabList.SelectedIndex = 1;
            TestUtils.FireEvent(dialog.TabList, "DoubleClick");

            Assert.Equal(DialogResult.OK, dialog.DialogResult);
            Assert.True(dialog.IsDisposed);
        }

        [Fact]
        public void SelectableTabTest()
        {
            using var dialog = new TabsDialog(this.tabinfo);
            dialog.MultiSelect = false;

            var item = (TabsDialog.TabListItem)dialog.TabList.Items[0];
            Assert.Null(item.Tab);

            item = (TabsDialog.TabListItem)dialog.TabList.Items[1];
            Assert.Equal(this.tabinfo.Tabs["Reply"], item.Tab);

            item = (TabsDialog.TabListItem)dialog.TabList.Items[2];
            Assert.Equal(this.tabinfo.Tabs["MyTab1"], item.Tab);
        }

        [Fact]
        public void SelectedTabTest()
        {
            using var dialog = new TabsDialog(this.tabinfo);
            dialog.MultiSelect = false;

            dialog.TabList.SelectedIndex = 0;
            Assert.Null(dialog.SelectedTab);

            dialog.TabList.SelectedIndex = 1;
            Assert.Equal(this.tabinfo.Tabs["Reply"], dialog.SelectedTab);
        }

        [Fact]
        public void SelectedTabsTest()
        {
            using var dialog = new TabsDialog(this.tabinfo);
            dialog.MultiSelect = true;

            dialog.TabList.SelectedIndices.Clear();
            var selectedTabs = dialog.SelectedTabs;
            Assert.Empty(selectedTabs);

            dialog.TabList.SelectedIndices.Add(0);
            selectedTabs = dialog.SelectedTabs;
            Assert.Equal(new[] { this.tabinfo.Tabs["Reply"] }, selectedTabs);

            dialog.TabList.SelectedIndices.Add(1);
            selectedTabs = dialog.SelectedTabs;
            Assert.Equal(new[] { this.tabinfo.Tabs["Reply"], this.tabinfo.Tabs["MyTab1"] }, selectedTabs);
        }
    }
}
