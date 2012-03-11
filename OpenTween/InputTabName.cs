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

namespace OpenTween
{
    public partial class InputTabName : Form
    {
        public InputTabName()
        {
            InitializeComponent();
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            TextTabName.Text = "";
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public string TabName
        {
            get { return this.TextTabName.Text.Trim(); }
            set { TextTabName.Text = value.Trim(); }
        }

        public string FormTitle
        {
            set { this.Text = value; }
        }

        public string FormDescription
        {
            set { this.LabelDescription.Text = value; }
        }

        private bool _isShowUsage;
        public bool IsShowUsage
        {
            set { _isShowUsage = value; }
        }

        private MyCommon.TabUsageType _usage;
        public MyCommon.TabUsageType Usage
        {
            get { return _usage; }
        }

        private void InputTabName_Load(object sender, EventArgs e)
        {
            this.LabelUsage.Visible = false;
            this.ComboUsage.Visible = false;
            this.ComboUsage.Items.Add(Properties.Resources.InputTabName_Load1);
            this.ComboUsage.Items.Add("Lists");
            this.ComboUsage.Items.Add("PublicSearch");
            this.ComboUsage.SelectedIndex = 0;
        }

        private void InputTabName_Shown(object sender, EventArgs e)
        {
            ActiveControl = TextTabName;
            if (_isShowUsage)
            {
                this.LabelUsage.Visible = true;
                this.ComboUsage.Visible = true;
            }
        }

        private void ComboUsage_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (ComboUsage.SelectedIndex)
            {
                case 0:
                    _usage = MyCommon.TabUsageType.UserDefined;
                    break;
                case 1:
                    _usage = MyCommon.TabUsageType.Lists;
                    break;
                case 2:
                    _usage = MyCommon.TabUsageType.PublicSearch;
                    break;
                default:
                    _usage = MyCommon.TabUsageType.Undefined;
                    break;
            }
        }
    }
}
