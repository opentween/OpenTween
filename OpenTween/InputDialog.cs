// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
    public partial class InputDialog : OTBaseForm
    {
        protected InputDialog()
            => this.InitializeComponent();

        private void ButtonOK_Click(object sender, EventArgs e)
            => this.DialogResult = DialogResult.OK;

        private void ButtonCancel_Click(object sender, EventArgs e)
            => this.DialogResult = DialogResult.Cancel;

        public static DialogResult Show(string text, out string inputText)
            => Show(null, text, "", out inputText);

        public static DialogResult Show(IWin32Window? owner, string text, out string inputText)
            => Show(owner, text, "", out inputText);

        public static DialogResult Show(string text, string caption, out string inputText)
            => Show(null, text, caption, out inputText);

        public static DialogResult Show(IWin32Window? owner, string text, string caption, out string inputText)
        {
            using var dialog = new InputDialog();
            dialog.labelMain.Text = text;
            dialog.Text = caption;

            var result = dialog.ShowDialog(owner);
            inputText = dialog.textBox.Text;

            return result;
        }
    }
}
