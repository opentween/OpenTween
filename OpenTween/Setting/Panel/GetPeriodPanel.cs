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
    public partial class GetPeriodPanel : SettingPanelBase
    {
        public GetPeriodPanel()
        {
            InitializeComponent();
        }

        private void UserstreamPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd;
            try
            {
                prd = int.Parse(UserstreamPeriod.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.UserstreamPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd < 0 || prd > 60)
            {
                MessageBox.Show(Properties.Resources.UserstreamPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }
        }

        private void TimelinePeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd;
            try
            {
                prd = int.Parse(TimelinePeriod.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.TimelinePeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Properties.Resources.TimelinePeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
        }

        private void ReplyPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd;
            try
            {
                prd = int.Parse(ReplyPeriod.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.TimelinePeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Properties.Resources.TimelinePeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
        }

        private void DMPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd;
            try
            {
                prd = int.Parse(DMPeriod.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.DMPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Properties.Resources.DMPeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
        }

        private void PubSearchPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd;
            try
            {
                prd = int.Parse(PubSearchPeriod.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.PubSearchPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 30 || prd > 6000))
            {
                MessageBox.Show(Properties.Resources.PubSearchPeriod_ValidatingText2);
                e.Cancel = true;
            }
        }

        private void ListsPeriod_Validating(object sender, CancelEventArgs e)
        {
            int prd;
            try
            {
                prd = int.Parse(ListsPeriod.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.DMPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Properties.Resources.DMPeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
        }

        private void UserTimeline_Validating(object sender, CancelEventArgs e)
        {
            int prd;
            try
            {
                prd = int.Parse(UserTimelinePeriod.Text);
            }
            catch (Exception)
            {
                MessageBox.Show(Properties.Resources.DMPeriod_ValidatingText1);
                e.Cancel = true;
                return;
            }

            if (prd != 0 && (prd < 15 || prd > 6000))
            {
                MessageBox.Show(Properties.Resources.DMPeriod_ValidatingText2);
                e.Cancel = true;
                return;
            }
        }
    }
}
