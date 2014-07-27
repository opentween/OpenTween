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

namespace OpenTween.Setting.Panel
{
    public partial class GetCountPanel : SettingPanelBase
    {
        public GetCountPanel()
        {
            InitializeComponent();
        }

        public void LoadConfig(SettingCommon settingCommon)
        {
            this.TextCountApi.Text = settingCommon.CountApi.ToString();
            this.TextCountApiReply.Text = settingCommon.CountApiReply.ToString();
            this.GetMoreTextCountApi.Text = settingCommon.MoreCountApi.ToString();
            this.FirstTextCountApi.Text = settingCommon.FirstCountApi.ToString();
            this.SearchTextCountApi.Text = settingCommon.SearchCountApi.ToString();
            this.FavoritesTextCountApi.Text = settingCommon.FavoritesCountApi.ToString();
            this.UserTimelineTextCountApi.Text = settingCommon.UserTimelineCountApi.ToString();
            this.ListTextCountApi.Text = settingCommon.ListCountApi.ToString();
            this.UseChangeGetCount.Checked = settingCommon.UseAdditionalCount;

            this.Label28.Enabled = this.UseChangeGetCount.Checked;
            this.Label30.Enabled = this.UseChangeGetCount.Checked;
            this.Label53.Enabled = this.UseChangeGetCount.Checked;
            this.Label66.Enabled = this.UseChangeGetCount.Checked;
            this.Label17.Enabled = this.UseChangeGetCount.Checked;
            this.Label25.Enabled = this.UseChangeGetCount.Checked;
            this.GetMoreTextCountApi.Enabled = this.UseChangeGetCount.Checked;
            this.FirstTextCountApi.Enabled = this.UseChangeGetCount.Checked;
            this.SearchTextCountApi.Enabled = this.UseChangeGetCount.Checked;
            this.FavoritesTextCountApi.Enabled = this.UseChangeGetCount.Checked;
            this.UserTimelineTextCountApi.Enabled = this.UseChangeGetCount.Checked;
            this.ListTextCountApi.Enabled = this.UseChangeGetCount.Checked;
        }

        public void SaveConfig(SettingCommon settingCommon)
        {
            settingCommon.CountApi = int.Parse(this.TextCountApi.Text);
            settingCommon.CountApiReply = int.Parse(this.TextCountApiReply.Text);
            settingCommon.UseAdditionalCount = this.UseChangeGetCount.Checked;
            settingCommon.MoreCountApi = int.Parse(this.GetMoreTextCountApi.Text);
            settingCommon.FirstCountApi = int.Parse(this.FirstTextCountApi.Text);
            settingCommon.SearchCountApi = int.Parse(this.SearchTextCountApi.Text);
            settingCommon.FavoritesCountApi = int.Parse(this.FavoritesTextCountApi.Text);
            settingCommon.UserTimelineCountApi = int.Parse(this.UserTimelineTextCountApi.Text);
            settingCommon.ListCountApi = int.Parse(this.ListTextCountApi.Text);
        }

        private void TextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(TextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt < 20 || cnt > 200)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void TextCountApiReply_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(TextCountApiReply.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt < 20 || cnt > 200)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void GetMoreTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(GetMoreTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt != 0 && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void UseChangeGetCount_CheckedChanged(object sender, EventArgs e)
        {
            GetMoreTextCountApi.Enabled = UseChangeGetCount.Checked;
            FirstTextCountApi.Enabled = UseChangeGetCount.Checked;
            Label28.Enabled = UseChangeGetCount.Checked;
            Label30.Enabled = UseChangeGetCount.Checked;
            Label53.Enabled = UseChangeGetCount.Checked;
            Label66.Enabled = UseChangeGetCount.Checked;
            Label17.Enabled = UseChangeGetCount.Checked;
            Label25.Enabled = UseChangeGetCount.Checked;
            SearchTextCountApi.Enabled = UseChangeGetCount.Checked;
            FavoritesTextCountApi.Enabled = UseChangeGetCount.Checked;
            UserTimelineTextCountApi.Enabled = UseChangeGetCount.Checked;
            ListTextCountApi.Enabled = UseChangeGetCount.Checked;
        }

        private void FirstTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(FirstTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt != 0 && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void SearchTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(SearchTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.TextSearchCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt != 0 && (cnt < 20 || cnt > 100))
            {
                MessageBox.Show(Properties.Resources.TextSearchCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void FavoritesTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(FavoritesTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt != 0 && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void UserTimelineTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(UserTimelineTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt != 0 && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }

        private void ListTextCountApi_Validating(object sender, CancelEventArgs e)
        {
            int cnt;
            try
            {
                cnt = int.Parse(ListTextCountApi.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }

            if (cnt != 0 && (cnt < 20 || cnt > 200))
            {
                MessageBox.Show(Properties.Resources.TextCountApi_Validating1);
                e.Cancel = true;
                return;
            }
        }
    }
}
