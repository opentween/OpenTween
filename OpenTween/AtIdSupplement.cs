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

#nullable enable

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
    public partial class AtIdSupplement : OTBaseForm
    {
        public string StartsWith { get; set; } = "";

        public string InputText { get; set; } = "";

        private bool isBack = false;
        private readonly string startChar = "";

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
            for (var i = 0; i < this.TextId.AutoCompleteCustomSource.Count; ++i)
            {
                ids.Add(this.TextId.AutoCompleteCustomSource[i]);
            }
            return ids;
        }

        public int ItemCount
            => this.TextId.AutoCompleteCustomSource.Count;

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            this.InputText = this.TextId.Text;
            this.isBack = false;
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.InputText = "";
            this.isBack = false;
        }

        private void TextId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back && MyCommon.IsNullOrEmpty(this.TextId.Text))
            {
                this.InputText = "";
                this.isBack = true;
                this.Close();
            }
            else if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Tab)
            {
                this.InputText = this.TextId.Text + " ";
                this.isBack = false;
                this.Close();
            }
            else if (e.Control && e.KeyCode == Keys.Delete)
            {
                if (!MyCommon.IsNullOrEmpty(this.TextId.Text))
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

        private void AtIdSupplement_Load(object sender, EventArgs e)
        {
            if (this.startChar == "#")
            {
                this.ClientSize = new Size(this.TextId.Width, this.TextId.Height); // プロパティで切り替えできるように
                this.TextId.ImeMode = ImeMode.Inherit;
            }
        }

        private void AtIdSupplement_Shown(object sender, EventArgs e)
        {
            this.TextId.Text = this.startChar;
            if (!MyCommon.IsNullOrEmpty(this.StartsWith))
            {
                this.TextId.Text += this.StartsWith.Substring(0, this.StartsWith.Length);
            }
            this.TextId.SelectionStart = this.TextId.Text.Length;
            this.TextId.Focus();
        }

        public AtIdSupplement()
            => this.InitializeComponent();

        public AtIdSupplement(List<string> itemList, string startCharacter)
        {
            this.InitializeComponent();

            for (var i = 0; i < itemList.Count; ++i)
            {
                this.TextId.AutoCompleteCustomSource.Add(itemList[i]);
            }
            this.startChar = startCharacter;
        }

        private void TextId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                this.InputText = this.TextId.Text + " ";
                this.isBack = false;
                this.Close();
            }
        }

        private void AtIdSupplement_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.StartsWith = "";
            if (this.isBack)
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
