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
    public partial class InputTabName : Form
    {
        public InputTabName()
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
            TextTabName.Text = "";
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public string TabName
        {
            get { return this.TextTabName.Text.Trim(); }
            set { TextTabName.Text = value.Trim(); }
        }

        public string FormTitle
        {
            set { this.Text = value; }
        }

        public string FormDescription
        {
            set { this.LabelDescription.Text = value; }
        }

        private bool _isShowUsage;
        public bool IsShowUsage
        {
            set { _isShowUsage = value; }
        }

        private MyCommon.TabUsageType _usage;
        public MyCommon.TabUsageType Usage
        {
            get { return _usage; }
        }

        private void InputTabName_Load(object sender, EventArgs e)
        {
            this.LabelUsage.Visible = false;
            this.ComboUsage.Visible = false;
            this.ComboUsage.Items.Add(Properties.Resources.InputTabName_Load1);
            this.ComboUsage.Items.Add("Lists");
            this.ComboUsage.Items.Add("PublicSearch");
            this.ComboUsage.SelectedIndex = 0;
        }

        private void InputTabName_Shown(object sender, EventArgs e)
        {
            ActiveControl = TextTabName;
            if (_isShowUsage)
            {
                this.LabelUsage.Visible = true;
                this.ComboUsage.Visible = true;
            }
        }

        private void ComboUsage_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (ComboUsage.SelectedIndex)
            {
                case 0:
                    _usage = MyCommon.TabUsageType.UserDefined;
                    break;
                case 1:
                    _usage = MyCommon.TabUsageType.Lists;
                    break;
                case 2:
                    _usage = MyCommon.TabUsageType.PublicSearch;
                    break;
                default:
                    _usage = MyCommon.TabUsageType.Undefined;
                    break;
            }
        }
    }
}
