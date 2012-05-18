// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
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
    public partial class DialogAsShieldIcon : Form
    {

        private DialogResult dResult = System.Windows.Forms.DialogResult.None;

        public DialogAsShieldIcon()
        {
            InitializeComponent();
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            this.dResult = System.Windows.Forms.DialogResult.OK;
            this.Hide();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.dResult = System.Windows.Forms.DialogResult.Cancel;
            this.Hide();
        }

        private void DialogAsShieldIcon_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.dResult == System.Windows.Forms.DialogResult.None)
            {
                e.Cancel = true;
                this.dResult = System.Windows.Forms.DialogResult.Cancel;
                this.Hide();
            }
        }

        private void DialogAsShieldIcon_Load(object sender, EventArgs e)
        {
            this.PictureBox1.Image = System.Drawing.SystemIcons.Question.ToBitmap();
        }

        public System.Windows.Forms.DialogResult ShowDialog(IWin32Window owner,
            string text, string detail = "", string caption = "DialogAsShieldIcon",
            System.Windows.Forms.MessageBoxButtons Buttons = System.Windows.Forms.MessageBoxButtons.OKCancel,
            System.Windows.Forms.MessageBoxIcon icon = MessageBoxIcon.Question)
        {
            this.Label1.Text = text;
            this.Text = caption;
            this.TextDetail.Text = detail;

            switch (Buttons)
            {
                case MessageBoxButtons.OKCancel:
                    OK_Button.Text = "OK";
                    Cancel_Button.Text = "Cancel";
                    break;
                case MessageBoxButtons.YesNo:
                    OK_Button.Text = "Yes";
                    Cancel_Button.Text = "No";
                    break;
                default:
                    OK_Button.Text = "OK";
                    Cancel_Button.Text = "Cancel";
                    break;
            }
            // とりあえずアイコンは処理しない（互換性のためパラメータだけ指定できる）

            base.ShowDialog(this.Owner);
            while (this.dResult == System.Windows.Forms.DialogResult.None)
            {
                System.Threading.Thread.Sleep(200);
                Application.DoEvents();
            }
            if (Buttons == MessageBoxButtons.YesNo)
            {
                switch (dResult)
                {
                    case System.Windows.Forms.DialogResult.OK:
                        return System.Windows.Forms.DialogResult.Yes;
                    case System.Windows.Forms.DialogResult.Cancel:
                        return System.Windows.Forms.DialogResult.No;
                }
            }
            else
            {
                return dResult;
            }

            return dResult;
        }
    }
}
