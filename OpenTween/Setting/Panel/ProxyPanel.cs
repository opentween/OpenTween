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
    public partial class ProxyPanel : SettingPanelBase
    {
        public ProxyPanel()
        {
            InitializeComponent();
        }

        private void RadioProxySpecified_CheckedChanged(object sender, EventArgs e)
        {
            bool chk = RadioProxySpecified.Checked;
            LabelProxyAddress.Enabled = chk;
            TextProxyAddress.Enabled = chk;
            LabelProxyPort.Enabled = chk;
            TextProxyPort.Enabled = chk;
            LabelProxyUser.Enabled = chk;
            TextProxyUser.Enabled = chk;
            LabelProxyPassword.Enabled = chk;
            TextProxyPassword.Enabled = chk;
        }

        private void TextProxyPort_Validating(object sender, CancelEventArgs e)
        {
            int port;
            if (string.IsNullOrWhiteSpace(TextProxyPort.Text)) TextProxyPort.Text = "0";
            if (int.TryParse(TextProxyPort.Text.Trim(), out port) == false)
            {
                MessageBox.Show(Properties.Resources.TextProxyPort_ValidatingText1);
                e.Cancel = true;
                return;
            }
            if (port < 0 || port > 65535)
            {
                MessageBox.Show(Properties.Resources.TextProxyPort_ValidatingText2);
                e.Cancel = true;
                return;
            }
        }
    }
}
