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
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        public static DialogResult Show(string text, out string inputText)
        {
            return Show(null, text, "", out inputText);
        }

        public static DialogResult Show(IWin32Window owner, string text, out string inputText)
        {
            return Show(owner, text, "", out inputText);
        }

        public static DialogResult Show(string text, string caption, out string inputText)
        {
            return Show(null, text, caption, out inputText);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, out string inputText)
        {
            using (var dialog = new InputDialog())
            {
                dialog.labelMain.Text = text;
                dialog.Text = caption;

                var result = dialog.ShowDialog(owner);
                inputText = dialog.textBox.Text;

                return result;
            }
        }
    }
}
