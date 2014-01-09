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
    public partial class TweetPrvPanel : SettingPanelBase
    {
        public TweetPrvPanel()
        {
            InitializeComponent();
        }

        private bool CreateDateTimeFormatSample()
        {
            try
            {
                LabelDateTimeFormatApplied.Text = DateTime.Now.ToString(CmbDateTimeFormat.Text);
            }
            catch (FormatException)
            {
                LabelDateTimeFormatApplied.Text = Properties.Resources.CreateDateTimeFormatSampleText1;
                return false;
            }
            return true;
        }

        private void CmbDateTimeFormat_TextUpdate(object sender, EventArgs e)
        {
            CreateDateTimeFormatSample();
        }

        private void CmbDateTimeFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateDateTimeFormatSample();
        }

        private void CmbDateTimeFormat_Validating(object sender, CancelEventArgs e)
        {
            if (!CreateDateTimeFormatSample())
            {
                MessageBox.Show(Properties.Resources.CmbDateTimeFormat_Validating);
                e.Cancel = true;
            }
        }

        private void LabelDateTimeFormatApplied_VisibleChanged(object sender, EventArgs e)
        {
            CreateDateTimeFormatSample();
        }
    }
}
