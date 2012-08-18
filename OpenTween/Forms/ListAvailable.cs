// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
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
    public partial class ListAvailable : Form
    {
        private ListElement _selectedList = null;
        public ListElement SelectedList
        {
            get
            {
                return _selectedList;
            }
        }

        public ListAvailable()
        {
            InitializeComponent();
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            if (this.ListsList.SelectedIndex > -1) {
                _selectedList = (ListElement)this.ListsList.SelectedItem;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            _selectedList = null;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void ListAvailable_Shown(object sender, EventArgs e)
        {
            if (TabInformations.GetInstance().SubscribableLists.Count == 0) this.RefreshLists();
            this.ListsList.Items.AddRange(TabInformations.GetInstance().SubscribableLists.ToArray());
            if (this.ListsList.Items.Count > 0)
            {
                this.ListsList.SelectedIndex = 0;
            }
            else
            {
                this.UsernameLabel.Text = "";
                this.NameLabel.Text = "";
                this.StatusLabel.Text = "";
                this.MemberCountLabel.Text = "0";
                this.SubscriberCountLabel.Text = "0";
                this.DescriptionText.Text = "";
            }
        }

        private void ListsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListElement lst;
            if (this.ListsList.SelectedIndex > -1)
            {
                lst = (ListElement)this.ListsList.SelectedItem;
            }
            else
            {
                lst = null;
            }
            if (lst == null)
            {
                this.UsernameLabel.Text = "";
                this.NameLabel.Text = "";
                this.StatusLabel.Text = "";
                this.MemberCountLabel.Text = "0";
                this.SubscriberCountLabel.Text = "0";
                this.DescriptionText.Text = "";
            }
            else
            {
                this.UsernameLabel.Text = lst.Username + " / " + lst.Nickname;
                this.NameLabel.Text = lst.Name;
                if (lst.IsPublic)
                {
                    this.StatusLabel.Text = "Public";
                }
                else
                {
                    this.StatusLabel.Text = "Private";
                }
                this.MemberCountLabel.Text = lst.MemberCount.ToString("#,##0");
                this.SubscriberCountLabel.Text = lst.SubscriberCount.ToString("#,##0");
                this.DescriptionText.Text = lst.Description;
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            this.RefreshLists();
            this.ListsList.Items.Clear();
            this.ListsList.Items.AddRange(TabInformations.GetInstance().SubscribableLists.ToArray());
            if (this.ListsList.Items.Count > 0)
            {
                this.ListsList.SelectedIndex = 0;
            }
        }

        private void RefreshLists()
        {
            using (FormInfo dlg = new FormInfo(this, "Getting Lists...", RefreshLists_DoWork))
            {
                dlg.ShowDialog();
                if (!String.IsNullOrEmpty(dlg.Result as String))
                {
                    MessageBox.Show("Failed to get lists. (" + (String)dlg.Result + ")");
                    return;
                }
            }
        }

        private void RefreshLists_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                e.Result = ((TweenMain)this.Owner).TwitterInstance.GetListsApi();
            }
            catch (InvalidCastException)
            {
                return;
            }
        }
    }
}
