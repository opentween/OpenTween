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

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenTween.Setting.Panel
{
    public partial class GetPeriodPanel : SettingPanelBase
    {
        public event EventHandler<IntervalChangedEventArgs>? IntervalChanged;

        public GetPeriodPanel()
            => this.InitializeComponent();

        public void LoadConfig(SettingCommon settingCommon)
        {
            this.CheckPostAndGet.Checked = settingCommon.PostAndGet;
            this.TimelinePeriod.Text = settingCommon.TimelinePeriod.ToString();
            this.ReplyPeriod.Text = settingCommon.ReplyPeriod.ToString();
            this.DMPeriod.Text = settingCommon.DMPeriod.ToString();
            this.PubSearchPeriod.Text = settingCommon.PubSearchPeriod.ToString();
            this.ListsPeriod.Text = settingCommon.ListsPeriod.ToString();
            this.UserTimelinePeriod.Text = settingCommon.UserTimelinePeriod.ToString();
        }

        public void SaveConfig(SettingCommon settingCommon)
        {
            settingCommon.PostAndGet = this.CheckPostAndGet.Checked;

            var arg = new IntervalChangedEventArgs();
            var isIntervalChanged = false;

            var timelinePeriod = int.Parse(this.TimelinePeriod.Text);
            if (settingCommon.TimelinePeriod != timelinePeriod)
            {
                settingCommon.TimelinePeriod = timelinePeriod;
                arg.Timeline = true;
                isIntervalChanged = true;
            }

            var dmPeriod = int.Parse(this.DMPeriod.Text);
            if (settingCommon.DMPeriod != dmPeriod)
            {
                settingCommon.DMPeriod = dmPeriod;
                arg.DirectMessage = true;
                isIntervalChanged = true;
            }

            var pubSearchPeriod = int.Parse(this.PubSearchPeriod.Text);
            if (settingCommon.PubSearchPeriod != pubSearchPeriod)
            {
                settingCommon.PubSearchPeriod = pubSearchPeriod;
                arg.PublicSearch = true;
                isIntervalChanged = true;
            }

            var listsPeriod = int.Parse(this.ListsPeriod.Text);
            if (settingCommon.ListsPeriod != listsPeriod)
            {
                settingCommon.ListsPeriod = listsPeriod;
                arg.Lists = true;
                isIntervalChanged = true;
            }

            var replyPeriod = int.Parse(this.ReplyPeriod.Text);
            if (settingCommon.ReplyPeriod != replyPeriod)
            {
                settingCommon.ReplyPeriod = replyPeriod;
                arg.Reply = true;
                isIntervalChanged = true;
            }

            var userTimelinePeriod = int.Parse(this.UserTimelinePeriod.Text);
            if (settingCommon.UserTimelinePeriod != userTimelinePeriod)
            {
                settingCommon.UserTimelinePeriod = userTimelinePeriod;
                arg.UserTimeline = true;
                isIntervalChanged = true;
            }

            if (isIntervalChanged)
                this.IntervalChanged?.Invoke(this, arg);
        }

        private void TimelinePeriod_Validating(object sender, CancelEventArgs e)
        {
            if (!this.ValidateIntervalStr(this.TimelinePeriod.Text))
            {
                MessageBox.Show(Properties.Resources.TimelinePeriod_ValidatingText1);
                e.Cancel = true;
            }
        }

        private void ReplyPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (!this.ValidateIntervalStr(this.ReplyPeriod.Text))
            {
                MessageBox.Show(Properties.Resources.TimelinePeriod_ValidatingText1);
                e.Cancel = true;
            }
        }

        private void DMPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (!this.ValidateIntervalStr(this.DMPeriod.Text))
            {
                MessageBox.Show(Properties.Resources.TimelinePeriod_ValidatingText1);
                e.Cancel = true;
            }
        }

        private void PubSearchPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (!this.ValidateIntervalStr(this.PubSearchPeriod.Text))
            {
                MessageBox.Show(Properties.Resources.TimelinePeriod_ValidatingText1);
                e.Cancel = true;
            }
        }

        private void ListsPeriod_Validating(object sender, CancelEventArgs e)
        {
            if (!this.ValidateIntervalStr(this.ListsPeriod.Text))
            {
                MessageBox.Show(Properties.Resources.TimelinePeriod_ValidatingText1);
                e.Cancel = true;
            }
        }

        private void UserTimeline_Validating(object sender, CancelEventArgs e)
        {
            if (!this.ValidateIntervalStr(this.UserTimelinePeriod.Text))
            {
                MessageBox.Show(Properties.Resources.TimelinePeriod_ValidatingText1);
                e.Cancel = true;
            }
        }

        private bool ValidateIntervalStr(string str)
        {
            if (!int.TryParse(str, out var value))
                return false;

            if (value < 0)
                return false;

            return true;
        }
    }
}
