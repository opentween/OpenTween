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
    public partial class ConnectionPanel : SettingPanelBase
    {
        public ConnectionPanel()
        {
            InitializeComponent();
        }

        private void ConnectionTimeOut_Validating(object sender, CancelEventArgs e)
        {
            int tm;
            try
            {
                tm = int.Parse(ConnectionTimeOut.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.ConnectionTimeOut_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (tm < (int)MyCommon.HttpTimeOut.MinValue || tm > (int)MyCommon.HttpTimeOut.MaxValue)
            {
                MessageBox.Show(Properties.Resources.ConnectionTimeOut_ValidatingText1);
                e.Cancel = true;
            }
        }
    }
}
