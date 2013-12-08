using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenTween.Setting.Panel
{
    public partial class BasedPanel : SettingPanelBase
    {
        public BasedPanel()
        {
            InitializeComponent();
        }

        private void AuthClearButton_Click(object sender, EventArgs e)
        {
            //tw.ClearAuthInfo();
            //this.AuthStateLabel.Text = Properties.Resources.AuthorizeButton_Click4;
            //this.AuthUserLabel.Text = "";
            if (this.AuthUserCombo.SelectedIndex > -1)
            {
                this.AuthUserCombo.Items.RemoveAt(this.AuthUserCombo.SelectedIndex);
                if (this.AuthUserCombo.Items.Count > 0)
                {
                    this.AuthUserCombo.SelectedIndex = 0;
                }
                else
                {
                    this.AuthUserCombo.SelectedIndex = -1;
                }
            }
            //this.Save.Enabled = false;
        }
    }
}
