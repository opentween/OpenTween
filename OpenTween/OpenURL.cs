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
    public partial class OpenURL : OTBaseForm
    {
        private string? selUrl;

        public OpenURL()
            => this.InitializeComponent();

        private void OK_Button_Click(object sender, EventArgs e)
        {
            if (this.UrlList.SelectedItems.Count == 0)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                this.selUrl = this.UrlList.SelectedItem.ToString();
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
            => this.UrlList.Items.Clear();

        public void AddUrl(OpenUrlItem openUrlItem)
            => this.UrlList.Items.Add(openUrlItem);

        public string SelectedUrl
        {
            get
            {
                if (this.UrlList.SelectedItems.Count == 1)
                    return this.selUrl!;
                else
                    return "";
            }
        }

        private void OpenURL_Shown(object sender, EventArgs e)
        {
            this.UrlList.Focus();
            if (this.UrlList.Items.Count > 0)
                this.UrlList.SelectedIndex = 0;
        }

        private void UrlList_DoubleClick(object sender, EventArgs e)
        {
            if (this.UrlList.SelectedItem == null)
                return;

            if (this.UrlList.IndexFromPoint(this.UrlList.PointToClient(Control.MousePosition)) == ListBox.NoMatches)
                return;

            if (this.UrlList.Items[this.UrlList.IndexFromPoint(this.UrlList.PointToClient(Control.MousePosition))] == null)
                return;
            this.OK_Button_Click(sender, e);
        }

        private void UrlList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.J && this.UrlList.SelectedIndex < this.UrlList.Items.Count - 1)
            {
                e.SuppressKeyPress = true;
                this.UrlList.SelectedIndex += 1;
            }
            if (e.KeyCode == Keys.K && this.UrlList.SelectedIndex > 0)
            {
                e.SuppressKeyPress = true;
                this.UrlList.SelectedIndex -= 1;
            }
            if (e.Control && e.KeyCode == Keys.Oem4)
            {
                e.SuppressKeyPress = true;
                this.Cancel_Button_Click(this.Cancel_Button, EventArgs.Empty);
            }
        }
    }

    public readonly record struct OpenUrlItem(
        string LinkText,
        string Url,
        string Href
    )
    {
        public string Text
        {
            get
            {
                if (this.LinkText[0] is '@' or '＠' or '#' or '＃')
                    return this.LinkText;

                if (this.LinkText.TrimEnd('/') == this.Url.TrimEnd('/'))
                    return this.Url;

                return this.LinkText + "  >>>  " + this.Url;
            }
        }

        public override string ToString()
            => this.Href;
    }
}
