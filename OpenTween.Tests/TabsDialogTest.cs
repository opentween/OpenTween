// OpenTween - Client of Twitter
// Copyright (c) 2012      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Text;
using NUnit.Framework;
using System.Windows.Forms;
using System.Reflection;

namespace OpenTween
{
    class TabsDialogTest
    {
        private TabInformations tabinfo;

        [SetUp]
        public void TabInformationSetUp()
        {
            this.tabinfo = Activator.CreateInstance(typeof(TabInformations), true) as TabInformations;

            // タブを追加
            this.tabinfo.AddTab("Recent", MyCommon.TabUsageType.Home, null);
            this.tabinfo.AddTab("Reply", MyCommon.TabUsageType.Mentions, null);
            this.tabinfo.AddTab("DM", MyCommon.TabUsageType.DirectMessage, null);
            this.tabinfo.AddTab("Favorites", MyCommon.TabUsageType.Favorites, null);
            this.tabinfo.AddTab("MyTab1", MyCommon.TabUsageType.UserDefined, null);

            // 一応 TabInformation.GetInstance() でも取得できるようにする
            var field = typeof(TabInformations).GetField("_instance",
                BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetField);
            field.SetValue(null, this.tabinfo);
        }

        [Test]
        public void OKButtonEnabledTest()
        {
            using (var dialog = new TabsDialog(this.tabinfo))
            {
                Assert.That(dialog.OK_Button.Enabled, Is.False);

                dialog.TabList.SelectedIndex = 0;

                Assert.That(dialog.OK_Button.Enabled, Is.True);

                dialog.TabList.SelectedIndex = -1;

                Assert.That(dialog.OK_Button.Enabled, Is.False);
            }
        }

        [Test]
        public void MultiSelectTest()
        {
            using (var dialog = new TabsDialog(this.tabinfo))
            {
                // MultiSelect = false (default)
                var firstItem = dialog.TabList.Items[0] as TabsDialog.TabListItem;
                Assert.That(firstItem.Tab, Is.Null); // 「(新規タブ)」
                Assert.That(dialog.TabList.SelectionMode, Is.EqualTo(SelectionMode.One));

                dialog.MultiSelect = true;
                firstItem = dialog.TabList.Items[0] as TabsDialog.TabListItem;
                Assert.That(firstItem.Tab, Is.Not.Null);
                Assert.That(dialog.TabList.SelectionMode, Is.EqualTo(SelectionMode.MultiExtended));

                dialog.MultiSelect = false;
                firstItem = dialog.TabList.Items[0] as TabsDialog.TabListItem;
                Assert.That(firstItem.Tab, Is.Null);
                Assert.That(dialog.TabList.SelectionMode, Is.EqualTo(SelectionMode.One));
            }
        }

        [Test]
        public void DoubleClickTest()
        {
            using (var dialog = new TabsDialog(this.tabinfo))
            {
                dialog.TabList.SelectedIndex = -1;
                TestUtils.FireEvent(dialog.TabList, "DoubleClick");

                Assert.That(dialog.DialogResult, Is.EqualTo(DialogResult.None));
                Assert.That(dialog.IsDisposed, Is.False);

                dialog.TabList.SelectedIndex = 1;
                TestUtils.FireEvent(dialog.TabList, "DoubleClick");

                Assert.That(dialog.DialogResult, Is.EqualTo(DialogResult.OK));
                Assert.That(dialog.IsDisposed, Is.True);
            }
        }

        [Test]
        public void SelectableTabTest()
        {
            using (var dialog = new TabsDialog(this.tabinfo))
            {
                dialog.MultiSelect = false;

                var item = dialog.TabList.Items[0] as TabsDialog.TabListItem;
                Assert.That(item.Tab, Is.Null);

                item = dialog.TabList.Items[1] as TabsDialog.TabListItem;
                Assert.That(item.Tab, Is.EqualTo(this.tabinfo.Tabs["Reply"]));

                item = dialog.TabList.Items[2] as TabsDialog.TabListItem;
                Assert.That(item.Tab, Is.EqualTo(this.tabinfo.Tabs["MyTab1"]));
            }
        }

        [Test]
        public void SelectedTabTest()
        {
            using (var dialog = new TabsDialog(this.tabinfo))
            {
                dialog.MultiSelect = false;

                dialog.TabList.SelectedIndex = 0;
                Assert.That(dialog.SelectedTab, Is.Null);

                dialog.TabList.SelectedIndex = 1;
                Assert.That(dialog.SelectedTab, Is.EqualTo(this.tabinfo.Tabs["Reply"]));
            }
        }

        [Test]
        public void SelectedTabsTest()
        {
            using (var dialog = new TabsDialog(this.tabinfo))
            {
                dialog.MultiSelect = true;

                dialog.TabList.SelectedIndices.Clear();
                var selectedTabs = dialog.SelectedTabs;
                Assert.That(selectedTabs, Is.Empty);

                dialog.TabList.SelectedIndices.Add(0);
                selectedTabs = dialog.SelectedTabs;
                Assert.That(selectedTabs, Is.EquivalentTo(new[] { this.tabinfo.Tabs["Reply"] }));

                dialog.TabList.SelectedIndices.Add(1);
                selectedTabs = dialog.SelectedTabs;
                Assert.That(selectedTabs, Is.EquivalentTo(new[] { this.tabinfo.Tabs["Reply"], this.tabinfo.Tabs["MyTab1"] }));
            }
        }
    }
}
