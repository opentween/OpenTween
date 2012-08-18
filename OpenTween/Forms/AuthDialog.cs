// OpenTween - Client of Twitter
// Copyright (c) 2012      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
// 
// This file is part of OpenTween.
// 
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
// 
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details. 
// 
// You should have received a copy of the GNU General public License along
// with this program. if (not, see <http://www.gnu.org/licenses/>, or write to
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
    /// <summary>
    /// OAuth認証のPINコードの入力を求めるダイアログ
    /// </summary>
    public partial class AuthDialog : Form
    {
        public AuthDialog()
        {
            InitializeComponent();
        }

        public string AuthUrl
        {
            get { return AuthLinkLabel.Text; }
            set { AuthLinkLabel.Text = value; }
        }

        public string Pin
        {
            get { return PinTextBox.Text.Trim(); }
            set { PinTextBox.Text = value; }
        }

        private void AuthLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AuthLinkLabel.LinkVisited = true;

            try
            {
                System.Diagnostics.Process.Start(AuthUrl);
            }
            catch (Win32Exception ex)
            {
                MessageBox.Show(this, string.Format(Properties.Resources.BrowserStartFailed, ex.ErrorCode), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// 指定されたURLにユーザーがアクセスするように指示してPINを入力させるだけ
        /// </summary>
        /// <param name="owner">親ウィンドウ</param>
        /// <param name="authUrl">認証URL</param>
        /// <returns>PIN文字列</returns>
        public static string DoAuth(IWin32Window owner, string authUrl)
        {
            using (var dialog = new AuthDialog())
            {
                dialog.AuthUrl = authUrl;

                dialog.ShowDialog(owner);

                if (dialog.DialogResult == DialogResult.OK)
                    return dialog.Pin;
                else
                    return null;
            }
        }
    }
}
