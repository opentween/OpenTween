// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
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
    public partial class AtIdSupplement : Form
    {
        public string inputText = "";
        public bool isBack = false;
        private string startChar = "";
        //    private bool tabkeyFix = false;

        private string _StartsWith = "";

        public void AddItem(string id)
        {
            if (!this.TextId.AutoCompleteCustomSource.Contains(id))
            {
                this.TextId.AutoCompleteCustomSource.Add(id);
            }
        }

        public void AddRangeItem(string[] ids)
        {
            foreach (var id in ids)
            {
                this.AddItem(id);
            }
        }

        public List<string> GetItemList()
        {
            var ids = new List<string>();
            for (int i = 0; i < this.TextId.AutoCompleteCustomSource.Count; ++i)
            {
                ids.Add(this.TextId.AutoCompleteCustomSource[i]);
            }
            return ids;
        }

        public int ItemCount
        {
            get
            {
                return this.TextId.AutoCompleteCustomSource.Count;
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e) /*Handles ButtonOK.Click*/
        {
            inputText = this.TextId.Text;
            isBack = false;
        }

        private void ButtonCancel_Click(object sender, EventArgs e) /*Handles ButtonCancel.Click*/
        {
            inputText = "";
            isBack = false;
        }

        private void TextId_KeyDown(object sender, KeyEventArgs e) /*Handles TextId.KeyDown*/
        {
            if (e.KeyCode == Keys.Back && string.IsNullOrEmpty(this.TextId.Text))
            {
                inputText = "";
                isBack = true;
                this.Close();
            }
            else if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Tab)
            {
                inputText = this.TextId.Text + " ";
                isBack = false;
                this.Close();
            }
            else if (e.Control && e.KeyCode == Keys.Delete)
            {
                if (!string.IsNullOrEmpty(this.TextId.Text))
                {
                    var idx = this.TextId.AutoCompleteCustomSource.IndexOf(this.TextId.Text);
                    if (idx > -1)
                    {
                        this.TextId.Text = "";
                        this.TextId.AutoCompleteCustomSource.RemoveAt(idx);
                    }
                }
            }
        }

        private void AtIdSupplement_Load(object sender, EventArgs e) /*Handles this.Load*/
        {
            if (startChar == "#")
            {
                this.ClientSize = new Size(this.TextId.Width, this.TextId.Height); //プロパティで切り替えできるように
                this.TextId.ImeMode = ImeMode.Inherit;
            }
        }

        private void AtIdSupplement_Shown(object sender, EventArgs e) /*Handles this.Shown*/
        {
            TextId.Text = startChar;
            if (!string.IsNullOrEmpty(_StartsWith))
            {
                TextId.Text += _StartsWith.Substring(0, _StartsWith.Length);
            }
            TextId.SelectionStart = TextId.Text.Length;
            TextId.Focus();
        }

        public AtIdSupplement()
        {
            InitializeComponent();
        }

        public AtIdSupplement(List<string> ItemList, string startCharacter)
        {
            InitializeComponent();

            for (int i = 0; i < ItemList.Count; ++i)
            {
                this.TextId.AutoCompleteCustomSource.Add(ItemList[i]);
            }
            startChar = startCharacter;
        }

        private void TextId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) /*Handles TextId.PreviewKeyDown*/
        {
            if (e.KeyCode == Keys.Tab)
            {
                inputText = this.TextId.Text + " ";
                isBack = false;
                this.Close();
            }
        }

        public string StartsWith
        {
            get
            {
                return _StartsWith;
            }
            set
            {
                _StartsWith = value;
            }
        }

        private void AtIdSupplement_FormClosed(object sender, FormClosedEventArgs e) /*Handles MyBase.FormClosed*/
        {
            _StartsWith = "";
            if (isBack)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}
