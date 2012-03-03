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
