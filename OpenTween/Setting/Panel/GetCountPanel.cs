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
