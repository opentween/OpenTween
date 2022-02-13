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
            => this.InitializeComponent();

        public void LoadConfig(SettingCommon settingCommon)
        {
            this.IconSize.SelectedIndex = settingCommon.IconSize switch
            {
                MyCommon.IconSizes.IconNone => 0,
                MyCommon.IconSizes.Icon16 => 1,
                MyCommon.IconSizes.Icon24 => 2,
                MyCommon.IconSizes.Icon48 => 3,
                MyCommon.IconSizes.Icon48_2 => 4,
                _ => 1,
            };
            this.OneWayLv.Checked = settingCommon.OneWayLove;
            this.CheckSortOrderLock.Checked = settingCommon.SortOrderLock;
            this.CheckViewTabBottom.Checked = settingCommon.ViewTabBottom;
            this.chkUnreadStyle.Checked = settingCommon.UseUnreadStyle;

            // 書式指定文字列エラーチェック
            var dateTimeFormat = settingCommon.DateTimeFormat;
            try
            {
                if (DateTimeUtc.Now.ToLocalTimeString(dateTimeFormat).Length == 0)
                {
                    // このブロックは絶対に実行されないはず
                    // 変換が成功した場合にLengthが0にならない
                    dateTimeFormat = "yyyy/MM/dd H:mm:ss";
                }
            }
            catch (FormatException)
            {
                // FormatExceptionが発生したら初期値を設定 (=yyyy/MM/dd H:mm:ssとみなされる)
                dateTimeFormat = "yyyy/MM/dd H:mm:ss";
            }
            this.CmbDateTimeFormat.Text = dateTimeFormat;

            this.CheckShowGrid.Checked = settingCommon.ShowGrid;
            this.HideDuplicatedRetweetsCheck.Checked = settingCommon.HideDuplicatedRetweets;
            this.IsListsIncludeRtsCheckBox.Checked = settingCommon.IsListsIncludeRts;
        }

        public void SaveConfig(SettingCommon settingCommon)
        {
            settingCommon.IconSize = this.IconSize.SelectedIndex switch
            {
                0 => MyCommon.IconSizes.IconNone,
                1 => MyCommon.IconSizes.Icon16,
                2 => MyCommon.IconSizes.Icon24,
                3 => MyCommon.IconSizes.Icon48,
                4 => MyCommon.IconSizes.Icon48_2,
                _ => throw new IndexOutOfRangeException($"Invalid {nameof(this.IconSize)} index: {this.IconSize.SelectedIndex}"),
            };
            settingCommon.OneWayLove = this.OneWayLv.Checked;
            settingCommon.SortOrderLock = this.CheckSortOrderLock.Checked;
            settingCommon.ViewTabBottom = this.CheckViewTabBottom.Checked;
            settingCommon.UseUnreadStyle = this.chkUnreadStyle.Checked;
            settingCommon.DateTimeFormat = this.CmbDateTimeFormat.Text;
            settingCommon.ShowGrid = this.CheckShowGrid.Checked;
            settingCommon.HideDuplicatedRetweets = this.HideDuplicatedRetweetsCheck.Checked;
            settingCommon.IsListsIncludeRts = this.IsListsIncludeRtsCheckBox.Checked;
        }

        private bool CreateDateTimeFormatSample()
        {
            try
            {
                this.LabelDateTimeFormatApplied.Text = DateTimeUtc.Now.ToLocalTimeString(this.CmbDateTimeFormat.Text);
            }
            catch (FormatException)
            {
                this.LabelDateTimeFormatApplied.Text = Properties.Resources.CreateDateTimeFormatSampleText1;
                return false;
            }
            return true;
        }

        private void CmbDateTimeFormat_TextUpdate(object sender, EventArgs e)
            => this.CreateDateTimeFormatSample();

        private void CmbDateTimeFormat_SelectedIndexChanged(object sender, EventArgs e)
            => this.CreateDateTimeFormatSample();

        private void CmbDateTimeFormat_Validating(object sender, CancelEventArgs e)
        {
            if (!this.CreateDateTimeFormatSample())
            {
                MessageBox.Show(Properties.Resources.CmbDateTimeFormat_Validating);
                e.Cancel = true;
            }
        }

        private void LabelDateTimeFormatApplied_VisibleChanged(object sender, EventArgs e)
            => this.CreateDateTimeFormatSample();
    }
}
