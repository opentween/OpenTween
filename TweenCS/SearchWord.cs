using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tween
{
    public partial class SearchWord : Form
    {
        public SearchWord()
        {
            InitializeComponent();
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public string SWord
        {
            get { return SWordText.Text; }
            set { SWordText.Text = value; }
        }

        public bool CheckCaseSensitive
        {
            get { return CheckSearchCaseSensitive.Checked; }
            set { CheckSearchCaseSensitive.Checked = value; }
        }

        public bool CheckRegex
        {
            get { return CheckSearchRegex.Checked; }
            set { CheckSearchRegex.Checked = value; }
        }

        private void SearchWord_Shown(object sender, EventArgs e)
        {
            SWordText.SelectAll();
            SWordText.Focus();
        }
    }
}
