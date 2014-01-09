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
    public partial class TweetActPanel : SettingPanelBase
    {
        public TweetActPanel()
        {
            InitializeComponent();
        }

        private void CheckUseRecommendStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckUseRecommendStatus.Checked == true)
            {
                StatusText.Enabled = false;
            }
            else
            {
                StatusText.Enabled = true;
            }
        }
    }
}
