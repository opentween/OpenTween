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

        private void UserAppointUrlText_Validating(object sender, CancelEventArgs e)
        {
            if (!UserAppointUrlText.Text.StartsWith("http") && !string.IsNullOrEmpty(UserAppointUrlText.Text))
            {
                MessageBox.Show("Text Error:正しいURLではありません");
            }
        }
    }
}
