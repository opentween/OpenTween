// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2014      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTween.Connection;

namespace OpenTween.Setting.Panel
{
    public partial class ProxyPanel : SettingPanelBase
    {
        public ProxyPanel()
        {
            InitializeComponent();
        }

        public void LoadConfig(SettingLocal settingLocal)
        {
            switch (settingLocal.ProxyType)
            {
                case ProxyType.None:
                    this.RadioProxyNone.Checked = true;
                    break;
                case ProxyType.IE:
                    this.RadioProxyIE.Checked = true;
                    break;
                default:
                    this.RadioProxySpecified.Checked = true;
                    break;
            }

            var chk = this.RadioProxySpecified.Checked;
            this.LabelProxyAddress.Enabled = chk;
            this.TextProxyAddress.Enabled = chk;
            this.LabelProxyPort.Enabled = chk;
            this.TextProxyPort.Enabled = chk;
            this.LabelProxyUser.Enabled = chk;
            this.TextProxyUser.Enabled = chk;
            this.LabelProxyPassword.Enabled = chk;
            this.TextProxyPassword.Enabled = chk;

            this.TextProxyAddress.Text = settingLocal.ProxyAddress;
            this.TextProxyPort.Text = settingLocal.ProxyPort.ToString();
            this.TextProxyUser.Text = settingLocal.ProxyUser;
            this.TextProxyPassword.Text = settingLocal.ProxyPassword;
        }

        public void SaveConfig(SettingLocal settingLocal)
        {
            if (this.RadioProxyNone.Checked)
            {
                settingLocal.ProxyType = ProxyType.None;
            }
            else if (this.RadioProxyIE.Checked)
            {
                settingLocal.ProxyType = ProxyType.IE;
            }
            else
            {
                settingLocal.ProxyType = ProxyType.Specified;
            }

            settingLocal.ProxyAddress = this.TextProxyAddress.Text.Trim();
            settingLocal.ProxyPort = int.Parse(this.TextProxyPort.Text.Trim());
            settingLocal.ProxyUser = this.TextProxyUser.Text.Trim();
            settingLocal.ProxyPassword = this.TextProxyPassword.Text.Trim();
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
