// OpenTween - Client of Twitter
// Copyright (c) 2011      Moz (@syo68k)
//           (c) 2012      tigree4th <crerish@gmail.com>
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
    public partial class MessageForm : Form
    {
        public MessageForm()
        {
            InitializeComponent();
        }

        public System.Windows.Forms.DialogResult ShowDialog(string message) {
            this.Label1.Text = message;

            // ラベルコントロールをセンタリング
            this.Label1.Left = (this.ClientSize.Width - this.Label1.Size.Width) / 2;

            this.Label1.Refresh();
            this.Label1.Visible = true;
            return base.ShowDialog();
        }
    }
}
