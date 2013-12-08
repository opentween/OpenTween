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
    public partial class CooperatePanel : SettingPanelBase
    {
        public CooperatePanel()
        {
            InitializeComponent();
        }

        private void CheckOutputz_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckOutputz.Checked == true)
            {
                Label59.Enabled = true;
                Label60.Enabled = true;
                TextBoxOutputzKey.Enabled = true;
                ComboBoxOutputzUrlmode.Enabled = true;
            }
            else
            {
                Label59.Enabled = false;
                Label60.Enabled = false;
                TextBoxOutputzKey.Enabled = false;
                ComboBoxOutputzUrlmode.Enabled = false;
            }
        }

        private void TextBoxOutputzKey_Validating(object sender, CancelEventArgs e)
        {
            if (CheckOutputz.Checked)
            {
                TextBoxOutputzKey.Text = TextBoxOutputzKey.Text.Trim();
                if (TextBoxOutputzKey.Text.Length == 0)
                {
                    MessageBox.Show(Properties.Resources.TextBoxOutputzKey_Validating);
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void UserAppointUrlText_Validating(object sender, CancelEventArgs e)
        {
            if (!UserAppointUrlText.Text.StartsWith("http") && !string.IsNullOrEmpty(UserAppointUrlText.Text))
            {
                MessageBox.Show("Text Error:正しいURLではありません");
            }
        }
    }
}
