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
    public partial class ShortUrlPanel : SettingPanelBase
    {
        public ShortUrlPanel()
        {
            InitializeComponent();
        }

        private void ComboBoxAutoShortUrlFirst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxAutoShortUrlFirst.SelectedIndex == (int)MyCommon.UrlConverter.Bitly ||
               ComboBoxAutoShortUrlFirst.SelectedIndex == (int)MyCommon.UrlConverter.Jmp)
            {
                Label76.Enabled = true;
                Label77.Enabled = true;
                TextBitlyId.Enabled = true;
                TextBitlyPw.Enabled = true;
            }
            else
            {
                Label76.Enabled = false;
                Label77.Enabled = false;
                TextBitlyId.Enabled = false;
                TextBitlyPw.Enabled = false;
            }
        }

        private void CheckAutoConvertUrl_CheckedChanged(object sender, EventArgs e)
        {
            ShortenTcoCheck.Enabled = CheckAutoConvertUrl.Checked;
        }
    }
}
