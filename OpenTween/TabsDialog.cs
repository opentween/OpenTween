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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenTween
{
    public partial class TabsDialog : Form
    {
        private readonly TabInformations TabInfo;

        private bool _MultiSelect = false;
        public bool MultiSelect
        {
            get { return this._MultiSelect; }
            set { this._MultiSelect = value; this.UpdateTabList(); }
        }

        protected internal class TabListItem
        {
            public TabClass Tab { get; set; }
            public string Label { get; set; }

            public override string ToString()
            {
                return this.Label;
            }
        }

        public TabsDialog(TabInformations tabinformation)
        {
            InitializeComponent();

            this.TabInfo = tabinformation;
            UpdateTabList();
        }

        protected void UpdateTabList()
        {
            this.TabList.Items.Clear();

            if (this.MultiSelect)
            {
                this.TabList.SelectionMode = SelectionMode.MultiExtended;
            }
            else
            {
                this.TabList.SelectionMode = SelectionMode.One;

                this.TabList.Items.Add(new TabListItem
                {
                    Label = Properties.Resources.AddNewTabText1,
                    Tab = null,
                });
            }

            foreach (var tab in this.TabInfo.Tabs)
            {
                if (!this.TabInfo.IsDistributableTab(tab.Key)) continue;

                this.TabList.Items.Add(new TabListItem
                {
                    Label = tab.Key,
                    Tab = tab.Value,
                });
            }
        }

        private void TabList_DoubleClick(object sender, EventArgs e)
        {
            if (this.TabList.SelectedIndex == -1) return;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void TabList_SelectedValueChanged(object sender, EventArgs e)
        {
            if (this.TabList.SelectedIndex == -1)
                this.OK_Button.Enabled = false;
            else
                this.OK_Button.Enabled = true;
        }

        public TabClass SelectedTab
        {
            get
            {
                var item = this.TabList.SelectedItem as TabListItem;
                if (item == null) return null;

                return item.Tab;
            }
        }

        public TabClass[] SelectedTabs
        {
            get
            {
                return this.TabList.SelectedItems
                    .Cast<TabListItem>()
                    .Select(x => x.Tab)
                    .ToArray();
            }
        }
    }
}
