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
    public partial class OpenURL : Form
    {
        private string _selUrl;

        public OpenURL()
        {
            InitializeComponent();
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            if (UrlList.SelectedItems.Count == 0)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                _selUrl = UrlList.SelectedItem.ToString();
                this.DialogResult = DialogResult.OK;
            }
            this.Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void ClearUrl()
        {
            UrlList.Items.Clear();
        }

        public void AddUrl(OpenUrlItem openUrlItem)
        {
            UrlList.Items.Add(openUrlItem);
        }

        public string SelectedUrl
        {
            get
            {
                if (UrlList.SelectedItems.Count == 1)
                    return _selUrl;
                else
                    return "";
            }
        }

        private void OpenURL_Shown(object sender, EventArgs e)
        {
            UrlList.Focus();
            if (UrlList.Items.Count > 0)
                UrlList.SelectedIndex = 0;
        }

        private void UrlList_DoubleClick(object sender, EventArgs e)
        {
            if (UrlList.SelectedItem == null)
                return;

            if (UrlList.IndexFromPoint(UrlList.PointToClient(Control.MousePosition)) == ListBox.NoMatches)
                return;

            if (UrlList.Items[UrlList.IndexFromPoint(UrlList.PointToClient(Control.MousePosition))] == null)
                return;
            OK_Button_Click(sender, e);
        }

        private void UrlList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.J && UrlList.SelectedIndex < UrlList.Items.Count - 1)
            {
                e.SuppressKeyPress = true;
                UrlList.SelectedIndex += 1;
            }
            if (e.KeyCode == Keys.K && UrlList.SelectedIndex > 0)
            {
                e.SuppressKeyPress = true;
                UrlList.SelectedIndex -= 1;
            }
            if (e.Control && e.KeyCode == Keys.Oem4)
            {
                e.SuppressKeyPress = true;
                Cancel_Button_Click(null, null);
            }
        }
    }

    public class OpenUrlItem
    {
        private string _linkText;

        public OpenUrlItem(string linkText, string url, string href)
        {
            this._linkText = linkText;
            this.Url = url;
            this.Href = href;
        }

        public string Text
        {
            get
            {
                if (this._linkText.StartsWith("@") || this._linkText.StartsWith("＠") || this._linkText.StartsWith("#") || this._linkText.StartsWith("＃"))
                    return this._linkText;
                if (this._linkText.TrimEnd('/') == this.Url.TrimEnd('/'))
                    return this.Url;
                else
                    return this._linkText + "  >>>  " + this.Url;
            }
        }

        public override string ToString()
        {
            return this.Href;
        }

        public string Url { get; private set; }
        public string Href { get; private set; }
    }
}
