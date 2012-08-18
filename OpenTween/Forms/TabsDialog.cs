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
using System.Collections.Specialized;

namespace OpenTween
{
    public partial class TabsDialog : Form
    {
        private bool _multiSelect = false;
        private string _newtabItem = Properties.Resources.AddNewTabText1;

        public TabsDialog()
        {
            InitializeComponent();
        }

        public TabsDialog(bool multiselect)
        {
            // この呼び出しはデザイナーで必要です。
            InitializeComponent();

            // InitializeComponent() 呼び出しの後で初期化を追加します。

            this.MultiSelect = true;

        }

        private void TabsDialog_Load(object sender, EventArgs e)
        {
            if (_multiSelect)
            {
                TabList.SelectedIndex = -1;
            }
            else
            {
                if (TabList.SelectedIndex == -1) TabList.SelectedIndex = 0;
            }
        }

        public void AddTab(string tabName)
        {
            foreach (string obj in TabList.Items)
            {
                if (obj == tabName) return;
            }
            TabList.Items.Add(tabName);
        }

        public void RemoveTab(string tabName)
        {
            for (int i = 0; i < TabList.Items.Count; i++)
            {
                if (((string)TabList.Items[i]) == tabName)
                {
                    TabList.Items.RemoveAt(i);
                    return;
                }
            }
        }

        public void ClearTab()
        {
            int startidx = 1;

            if (_multiSelect)
            {
                startidx = 0;
            }
            for (int i = startidx; i < TabList.Items.Count; i++)
            {
                TabList.Items.RemoveAt(0);
            }
        }

        public string SelectedTabName
        {
            get
            {
                if (TabList.SelectedIndex == -1)
                    return "";
                else
                    return (string)TabList.SelectedItem;
            }
        }

        public StringCollection SelectedTabNames
        {
            get
            {
                if (TabList.SelectedIndex == -1)
                {
                    return null;
                }
                else
                {
                    StringCollection ret = new StringCollection();
                    foreach (object selitem in TabList.SelectedItems)
                    {
                        ret.Add((string)selitem);
                    }
                    return ret;
                }
            }
        }

        public bool MultiSelect
        {
            get { return _multiSelect; }
            set
            {
                _multiSelect = value;
                if (value)
                {
                    this.TabList.SelectionMode = SelectionMode.MultiExtended;
                    if (this.TabList.Items[0].ToString() == Properties.Resources.AddNewTabText1)
                    {
                        this.TabList.Items.RemoveAt(0);
                    }
                }
                else
                {
                    this.TabList.SelectionMode = SelectionMode.One;
                    if (this.TabList.Items[0].ToString() != Properties.Resources.AddNewTabText1)
                    {
                        this.TabList.Items.Insert(0, Properties.Resources.AddNewTabText1);
                    }
                }
            }
        }

        private void TabList_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void TabList_DoubleClick(object sender, EventArgs e)
        {
            if (TabList.SelectedItem == null)
            {
                return;
            }

            if (TabList.IndexFromPoint(TabList.PointToClient(Control.MousePosition)) == ListBox.NoMatches)
            {
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void TabsDialog_Shown(object sender, EventArgs e)
        {
            TabList.Focus();
        }
    }
}
