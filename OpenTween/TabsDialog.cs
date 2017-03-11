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
using OpenTween.Models;

namespace OpenTween
{
    public partial class TabsDialog : OTBaseForm
    {
        private readonly TabInformations TabInfo;

        private bool _MultiSelect = false;
        public bool MultiSelect
        {
            get => this._MultiSelect;
            set { this._MultiSelect = value; this.UpdateTabList(); }
        }

        protected internal class TabListItem
        {
            public TabModel Tab { get; set; }
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

            foreach (var (name, tab) in this.TabInfo.Tabs)
            {
                if (!tab.IsDistributableTabType) continue;

                this.TabList.Items.Add(new TabListItem
                {
                    Label = name,
                    Tab = tab,
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

        public TabModel SelectedTab
        {
            get
            {
                var item = this.TabList.SelectedItem as TabListItem;
                if (item == null) return null;

                return item.Tab;
            }
        }

        public TabModel[] SelectedTabs
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
