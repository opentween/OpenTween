// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011-2012 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenTween.Models;

namespace OpenTween
{
    public partial class FilterDialog : OTBaseForm
    {
        private EDITMODE mode;
        private bool directAdd;
        private MultiSelectionState multiSelState = MultiSelectionState.None;
        private readonly TabInformations sts;

        private List<TabModel> tabs = new();
        private int selectedTabIndex = -1;

        private readonly List<string> idlist = new();

        private enum EDITMODE
        {
            AddNew,
            Edit,
            None,
        }

        private enum EnableButtonMode
        {
            NotSelected,
            Enable,
            Disable,
        }

        [Flags]
        private enum MultiSelectionState
        {
            None = 0,
            MoveSelected = 1,
            SelectAll = 2,
        }

        private EnableButtonMode RuleEnableButtonMode
        {
            get => this.ruleEnableButtonMode;
            set
            {
                this.ruleEnableButtonMode = value;

                this.buttonRuleToggleEnabled.Text = value == FilterDialog.EnableButtonMode.Enable
                    ? Properties.Resources.EnableButtonCaption
                    : Properties.Resources.DisableButtonCaption;
                this.buttonRuleToggleEnabled.Enabled = value != EnableButtonMode.NotSelected;
            }
        }

        private EnableButtonMode ruleEnableButtonMode = FilterDialog.EnableButtonMode.NotSelected;

        public TabModel? SelectedTab
            => this.selectedTabIndex != -1 ? this.tabs[this.selectedTabIndex] : null;

        public FilterDialog()
        {
            this.InitializeComponent();

            this.sts = TabInformations.GetInstance();
            this.RefreshListTabs();
        }

        private void RefreshListTabs()
        {
            this.tabs = this.sts.Tabs.Append(this.sts.MuteTab).ToList();

            using (ControlTransaction.Update(this.ListTabs))
            {
                var selectedTab = this.ListTabs.SelectedItem;
                this.ListTabs.Items.Clear();
                this.ListTabs.Items.AddRange(this.tabs.ToArray());
                this.ListTabs.SelectedIndex = this.tabs.FindIndex(x => x == selectedTab);
            }
        }

        private void SetFilters(TabModel tab)
        {
            if (this.ListTabs.Items.Count == 0) return;

            this.ListFilters.Items.Clear();

            if (tab is FilterTabModel filterTab)
                this.ListFilters.Items.AddRange(filterTab.GetFilters());

            if (this.ListFilters.Items.Count > 0)
                this.ListFilters.SelectedIndex = 0;
            else
                this.ShowDetail();

            if (tab.IsDefaultTabType)
            {
                this.CheckProtected.Checked = true;
                this.CheckProtected.Enabled = false;
            }
            else
            {
                this.CheckProtected.Checked = tab.Protected;
                this.CheckProtected.Enabled = true;
            }

            this.CheckManageRead.CheckedChanged -= this.CheckManageRead_CheckedChanged;
            this.CheckManageRead.Checked = tab.UnreadManage;
            this.CheckManageRead.CheckedChanged += this.CheckManageRead_CheckedChanged;

            this.CheckNotifyNew.CheckedChanged -= this.CheckNotifyNew_CheckedChanged;
            this.CheckNotifyNew.Checked = tab.Notify;
            this.CheckNotifyNew.CheckedChanged += this.CheckNotifyNew_CheckedChanged;

            var idx = this.ComboSound.Items.IndexOf(tab.SoundFile);
            if (idx == -1) idx = 0;
            this.ComboSound.SelectedIndex = idx;

            if (this.directAdd) return;

            if (tab.TabType == MyCommon.TabUsageType.Mute)
            {
                this.ButtonRenameTab.Enabled = false;
                this.CheckManageRead.Enabled = false;
                this.CheckNotifyNew.Enabled = false;
                this.ComboSound.Enabled = false;

                this.GroupBox1.Visible = false;
                this.labelMuteTab.Visible = true;
            }
            else
            {
                this.ButtonRenameTab.Enabled = true;
                this.CheckManageRead.Enabled = true;
                this.CheckNotifyNew.Enabled = true;
                this.ComboSound.Enabled = true;

                this.GroupBox1.Visible = true;
                this.labelMuteTab.Visible = false;
            }

            this.ListTabs.Enabled = true;
            this.GroupTab.Enabled = true;
            this.ListFilters.Enabled = true;
            this.EditFilterGroup.Enabled = false;

            if (tab.IsDistributableTabType)
            {
                this.ButtonNew.Enabled = true;
                if (this.ListFilters.SelectedIndex > -1)
                {
                    this.ButtonEdit.Enabled = true;
                    this.ButtonDelete.Enabled = true;
                    this.ButtonRuleUp.Enabled = true;
                    this.ButtonRuleDown.Enabled = true;
                    this.ButtonRuleCopy.Enabled = true;
                    this.ButtonRuleMove.Enabled = true;
                    this.buttonRuleToggleEnabled.Enabled = true;
                }
                else
                {
                    this.ButtonEdit.Enabled = false;
                    this.ButtonDelete.Enabled = false;
                    this.ButtonRuleUp.Enabled = false;
                    this.ButtonRuleDown.Enabled = false;
                    this.ButtonRuleCopy.Enabled = false;
                    this.ButtonRuleMove.Enabled = false;
                    this.buttonRuleToggleEnabled.Enabled = false;
                }
            }
            else
            {
                this.ButtonNew.Enabled = false;
                this.ButtonEdit.Enabled = false;
                this.ButtonDelete.Enabled = false;
                this.ButtonRuleUp.Enabled = false;
                this.ButtonRuleDown.Enabled = false;
                this.ButtonRuleCopy.Enabled = false;
                this.ButtonRuleMove.Enabled = false;
                this.buttonRuleToggleEnabled.Enabled = false;
            }

            this.LabelTabType.Text = tab.TabType switch
            {
                MyCommon.TabUsageType.Home => Properties.Resources.TabUsageTypeName_Home,
                MyCommon.TabUsageType.Mentions => Properties.Resources.TabUsageTypeName_Mentions,
                MyCommon.TabUsageType.DirectMessage => Properties.Resources.TabUsageTypeName_DirectMessage,
                MyCommon.TabUsageType.Favorites => Properties.Resources.TabUsageTypeName_Favorites,
                MyCommon.TabUsageType.UserDefined => Properties.Resources.TabUsageTypeName_UserDefined,
                MyCommon.TabUsageType.PublicSearch => Properties.Resources.TabUsageTypeName_PublicSearch,
                MyCommon.TabUsageType.Lists => Properties.Resources.TabUsageTypeName_Lists,
                MyCommon.TabUsageType.Related => Properties.Resources.TabUsageTypeName_Related,
                MyCommon.TabUsageType.UserTimeline => Properties.Resources.TabUsageTypeName_UserTimeline,
                MyCommon.TabUsageType.Mute => "Mute",
                MyCommon.TabUsageType.SearchResults => "SearchResults",
                _ => "UNKNOWN",
            };

            if (tab.IsDefaultTabType || tab.Protected)
            {
                this.ButtonDeleteTab.Enabled = false;
            }
            else
            {
                this.ButtonDeleteTab.Enabled = true;
            }
            this.ButtonClose.Enabled = true;
        }

        public void SetCurrent(string tabName)
        {
            var index = this.tabs.FindIndex(x => x.TabName == tabName);
            if (index == -1)
                throw new ArgumentException($"Unknown tab: {tabName}", nameof(tabName));

            this.selectedTabIndex = index;
        }

        public void AddNewFilter(string id, string msg)
        {
            // 元フォームから直接呼ばれる
            this.ButtonNew.Enabled = false;
            this.ButtonEdit.Enabled = false;
            this.ButtonRuleUp.Enabled = false;
            this.ButtonRuleDown.Enabled = false;
            this.ButtonRuleCopy.Enabled = false;
            this.ButtonRuleMove.Enabled = false;
            this.buttonRuleToggleEnabled.Enabled = false;
            this.ButtonDelete.Enabled = false;
            this.ButtonClose.Enabled = false;
            this.EditFilterGroup.Enabled = true;
            this.ListTabs.Enabled = false;
            this.GroupTab.Enabled = false;
            this.ListFilters.Enabled = false;

            this.RadioAND.Checked = true;
            this.RadioPLUS.Checked = false;
            this.UID.Text = id;
            this.UID.SelectAll();
            this.MSG1.Text = msg;
            this.MSG1.SelectAll();
            this.MSG2.Text = id + msg;
            this.MSG2.SelectAll();
            this.TextSource.Text = "";
            this.UID.Enabled = true;
            this.MSG1.Enabled = true;
            this.MSG2.Enabled = false;
            this.CheckRegex.Checked = false;
            this.CheckURL.Checked = false;
            this.CheckCaseSensitive.Checked = false;
            this.CheckRetweet.Checked = false;
            this.CheckLambda.Checked = false;

            this.RadioExAnd.Checked = true;
            this.RadioExPLUS.Checked = false;
            this.ExUID.Text = "";
            this.ExUID.SelectAll();
            this.ExMSG1.Text = "";
            this.ExMSG1.SelectAll();
            this.ExMSG2.Text = "";
            this.ExMSG2.SelectAll();
            this.TextExSource.Text = "";
            this.ExUID.Enabled = true;
            this.ExMSG1.Enabled = true;
            this.ExMSG2.Enabled = false;
            this.CheckExRegex.Checked = false;
            this.CheckExURL.Checked = false;
            this.CheckExCaseSensitive.Checked = false;
            this.CheckExRetweet.Checked = false;
            this.CheckExLambDa.Checked = false;

            this.OptCopy.Checked = true;
            this.CheckMark.Checked = true;
            this.UID.Focus();
            this.mode = EDITMODE.AddNew;
            this.directAdd = true;
        }

        private void ButtonNew_Click(object sender, EventArgs e)
        {
            this.ButtonNew.Enabled = false;
            this.ButtonEdit.Enabled = false;
            this.ButtonClose.Enabled = false;
            this.ButtonRuleUp.Enabled = false;
            this.ButtonRuleDown.Enabled = false;
            this.ButtonRuleCopy.Enabled = false;
            this.ButtonRuleMove.Enabled = false;
            this.buttonRuleToggleEnabled.Enabled = false;
            this.ButtonDelete.Enabled = false;
            this.ButtonClose.Enabled = false;
            this.EditFilterGroup.Enabled = true;
            this.ListTabs.Enabled = false;
            this.GroupTab.Enabled = false;
            this.ListFilters.Enabled = false;

            this.RadioAND.Checked = true;
            this.RadioPLUS.Checked = false;
            this.UID.Text = "";
            this.MSG1.Text = "";
            this.MSG2.Text = "";
            this.TextSource.Text = "";
            this.UID.Enabled = true;
            this.MSG1.Enabled = true;
            this.MSG2.Enabled = false;
            this.CheckRegex.Checked = false;
            this.CheckURL.Checked = false;
            this.CheckCaseSensitive.Checked = false;
            this.CheckRetweet.Checked = false;
            this.CheckLambda.Checked = false;

            this.RadioExAnd.Checked = true;
            this.RadioExPLUS.Checked = false;
            this.ExUID.Text = "";
            this.ExMSG1.Text = "";
            this.ExMSG2.Text = "";
            this.TextExSource.Text = "";
            this.ExUID.Enabled = true;
            this.ExMSG1.Enabled = true;
            this.ExMSG2.Enabled = false;
            this.CheckExRegex.Checked = false;
            this.CheckExURL.Checked = false;
            this.CheckExCaseSensitive.Checked = false;
            this.CheckExRetweet.Checked = false;
            this.CheckExLambDa.Checked = false;

            this.OptCopy.Checked = true;
            this.CheckMark.Checked = true;
            this.UID.Focus();
            this.mode = EDITMODE.AddNew;
        }

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (this.ListFilters.SelectedIndex == -1) return;

            this.ShowDetail();

            var idx = this.ListFilters.SelectedIndex;
            this.ListFilters.SelectedIndex = -1;
            this.ListFilters.SelectedIndex = idx;
            this.ListFilters.Enabled = false;

            this.ButtonNew.Enabled = false;
            this.ButtonEdit.Enabled = false;
            this.ButtonDelete.Enabled = false;
            this.ButtonClose.Enabled = false;
            this.ButtonRuleUp.Enabled = false;
            this.ButtonRuleDown.Enabled = false;
            this.ButtonRuleCopy.Enabled = false;
            this.ButtonRuleMove.Enabled = false;
            this.buttonRuleToggleEnabled.Enabled = false;
            this.EditFilterGroup.Enabled = true;
            this.ListTabs.Enabled = false;
            this.GroupTab.Enabled = false;

            this.mode = EDITMODE.Edit;
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            var selectedCount = this.ListFilters.SelectedIndices.Count;
            if (selectedCount == 0) return;

            string tmp;

            if (selectedCount == 1)
            {
                tmp = string.Format(Properties.Resources.ButtonDelete_ClickText1, Environment.NewLine, this.ListFilters.SelectedItem);
            }
            else
            {
                tmp = string.Format(Properties.Resources.ButtonDelete_ClickText3, selectedCount);
            }

            var rslt = MessageBox.Show(tmp, Properties.Resources.ButtonDelete_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (rslt == DialogResult.Cancel) return;

            var indices = this.ListFilters.SelectedIndices.Cast<int>().Reverse().ToArray();  // 後ろの要素から削除
            var tab = (FilterTabModel)this.SelectedTab!;

            using (ControlTransaction.Update(this.ListFilters))
            {
                foreach (var idx in indices)
                {
                    tab.RemoveFilter((PostFilterRule)this.ListFilters.Items[idx]);
                    this.ListFilters.Items.RemoveAt(idx);
                }
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.ListTabs.Enabled = true;
            this.GroupTab.Enabled = true;
            this.ListFilters.Enabled = true;
            this.ListFilters.Focus();
            if (this.ListFilters.SelectedIndex != -1)
            {
                this.ShowDetail();
            }
            this.EditFilterGroup.Enabled = false;
            this.ButtonNew.Enabled = true;
            if (this.ListFilters.SelectedIndex > -1)
            {
                this.ButtonEdit.Enabled = true;
                this.ButtonDelete.Enabled = true;
                this.ButtonRuleUp.Enabled = true;
                this.ButtonRuleDown.Enabled = true;
                this.ButtonRuleCopy.Enabled = true;
                this.ButtonRuleMove.Enabled = true;
                this.buttonRuleToggleEnabled.Enabled = true;
            }
            else
            {
                this.ButtonEdit.Enabled = false;
                this.ButtonDelete.Enabled = false;
                this.ButtonRuleUp.Enabled = false;
                this.ButtonRuleDown.Enabled = false;
                this.ButtonRuleCopy.Enabled = false;
                this.ButtonRuleMove.Enabled = false;
                this.buttonRuleToggleEnabled.Enabled = false;
            }
            this.ButtonClose.Enabled = true;
            if (this.directAdd)
            {
                this.Close();
            }
        }

        private void ShowDetail()
        {
            if (this.directAdd) return;

            if (this.ListFilters.SelectedIndex > -1)
            {
                var fc = (PostFilterRule)this.ListFilters.SelectedItem;
                if (fc.UseNameField)
                {
                    this.RadioAND.Checked = true;
                    this.RadioPLUS.Checked = false;
                    this.UID.Enabled = true;
                    this.MSG1.Enabled = true;
                    this.MSG2.Enabled = false;
                    this.UID.Text = fc.FilterName;
                    this.UID.SelectAll();
                    this.MSG1.Text = string.Join(" ", fc.FilterBody);
                    this.MSG1.SelectAll();
                    this.MSG2.Text = "";
                }
                else
                {
                    this.RadioPLUS.Checked = true;
                    this.RadioAND.Checked = false;
                    this.UID.Enabled = false;
                    this.MSG1.Enabled = false;
                    this.MSG2.Enabled = true;
                    this.UID.Text = "";
                    this.MSG1.Text = "";
                    this.MSG2.Text = string.Join(" ", fc.FilterBody);
                    this.MSG2.SelectAll();
                }
                this.TextSource.Text = fc.FilterSource;
                this.CheckRegex.Checked = fc.UseRegex;
                this.CheckURL.Checked = fc.FilterByUrl;
                this.CheckCaseSensitive.Checked = fc.CaseSensitive;
                this.CheckRetweet.Checked = fc.FilterRt;
                this.CheckLambda.Checked = fc.UseLambda;

                if (fc.ExUseNameField)
                {
                    this.RadioExAnd.Checked = true;
                    this.RadioExPLUS.Checked = false;
                    this.ExUID.Enabled = true;
                    this.ExMSG1.Enabled = true;
                    this.ExMSG2.Enabled = false;
                    this.ExUID.Text = fc.ExFilterName;
                    this.ExUID.SelectAll();
                    this.ExMSG1.Text = string.Join(" ", fc.ExFilterBody);
                    this.ExMSG1.SelectAll();
                    this.ExMSG2.Text = "";
                }
                else
                {
                    this.RadioExPLUS.Checked = true;
                    this.RadioExAnd.Checked = false;
                    this.ExUID.Enabled = false;
                    this.ExMSG1.Enabled = false;
                    this.ExMSG2.Enabled = true;
                    this.ExUID.Text = "";
                    this.ExMSG1.Text = "";
                    this.ExMSG2.Text = string.Join(" ", fc.ExFilterBody);
                    this.ExMSG2.SelectAll();
                }
                this.TextExSource.Text = fc.ExFilterSource;
                this.CheckExRegex.Checked = fc.ExUseRegex;
                this.CheckExURL.Checked = fc.ExFilterByUrl;
                this.CheckExCaseSensitive.Checked = fc.ExCaseSensitive;
                this.CheckExRetweet.Checked = fc.ExFilterRt;
                this.CheckExLambDa.Checked = fc.ExUseLambda;

                if (fc.MoveMatches)
                {
                    this.OptMove.Checked = true;
                }
                else
                {
                    this.OptCopy.Checked = true;
                }
                this.CheckMark.Checked = fc.MarkMatches;

                this.ButtonEdit.Enabled = true;
                this.ButtonDelete.Enabled = true;
                this.ButtonRuleUp.Enabled = true;
                this.ButtonRuleDown.Enabled = true;
                this.ButtonRuleCopy.Enabled = true;
                this.ButtonRuleMove.Enabled = true;
                this.buttonRuleToggleEnabled.Enabled = true;
            }
            else
            {
                this.RadioAND.Checked = true;
                this.RadioPLUS.Checked = false;
                this.UID.Enabled = true;
                this.MSG1.Enabled = true;
                this.MSG2.Enabled = false;
                this.UID.Text = "";
                this.MSG1.Text = "";
                this.MSG2.Text = "";
                this.TextSource.Text = "";
                this.CheckRegex.Checked = false;
                this.CheckURL.Checked = false;
                this.CheckCaseSensitive.Checked = false;
                this.CheckRetweet.Checked = false;
                this.CheckLambda.Checked = false;

                this.RadioExAnd.Checked = true;
                this.RadioExPLUS.Checked = false;
                this.ExUID.Enabled = true;
                this.ExMSG1.Enabled = true;
                this.ExMSG2.Enabled = false;
                this.ExUID.Text = "";
                this.ExMSG1.Text = "";
                this.ExMSG2.Text = "";
                this.TextExSource.Text = "";
                this.CheckExRegex.Checked = false;
                this.CheckExURL.Checked = false;
                this.CheckExCaseSensitive.Checked = false;
                this.CheckExRetweet.Checked = false;
                this.CheckExLambDa.Checked = false;

                this.OptCopy.Checked = true;
                this.CheckMark.Checked = true;

                this.ButtonEdit.Enabled = false;
                this.ButtonDelete.Enabled = false;
                this.ButtonRuleUp.Enabled = false;
                this.ButtonRuleDown.Enabled = false;
                this.ButtonRuleCopy.Enabled = false;
                this.ButtonRuleMove.Enabled = false;
                this.buttonRuleToggleEnabled.Enabled = false;
            }
        }

        private void RadioAND_CheckedChanged(object sender, EventArgs e)
        {
            var flg = this.RadioAND.Checked;
            this.UID.Enabled = flg;
            this.MSG1.Enabled = flg;
            this.MSG2.Enabled = !flg;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            // 入力チェック
            if (!this.CheckMatchRule(out var isBlankMatch) || !this.CheckExcludeRule(out var isBlankExclude))
            {
                return;
            }
            if (isBlankMatch && isBlankExclude)
            {
                MessageBox.Show(Properties.Resources.ButtonOK_ClickText1, Properties.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var tab = (FilterTabModel)this.SelectedTab!;
            var i = this.ListFilters.SelectedIndex;

            PostFilterRule ft;
            if (this.mode == EDITMODE.AddNew)
                ft = new PostFilterRule();
            else
                ft = (PostFilterRule)this.ListFilters.SelectedItem;

            if (tab.TabType != MyCommon.TabUsageType.Mute)
            {
                ft.MoveMatches = this.OptMove.Checked;
                ft.MarkMatches = this.CheckMark.Checked;
            }
            else
            {
                ft.MoveMatches = true;
                ft.MarkMatches = false;
            }

            var bdy = "";
            if (this.RadioAND.Checked)
            {
                ft.FilterName = this.UID.Text;
                var owner = (TweenMain)this.Owner;
                var cnt = owner.AtIdSupl.ItemCount;
                owner.AtIdSupl.AddItem("@" + ft.FilterName);
                if (cnt != owner.AtIdSupl.ItemCount)
                {
                    owner.MarkSettingAtIdModified();
                }
                ft.UseNameField = true;
                bdy = this.MSG1.Text;
            }
            else
            {
                ft.FilterName = "";
                ft.UseNameField = false;
                bdy = this.MSG2.Text;
            }
            ft.FilterSource = this.TextSource.Text;

            if (this.CheckRegex.Checked || this.CheckLambda.Checked)
            {
                ft.FilterBody = new[] { bdy };
            }
            else
            {
                ft.FilterBody = bdy.Split(' ', '　')
                    .Where(x => !MyCommon.IsNullOrEmpty(x))
                    .ToArray();
            }

            ft.UseRegex = this.CheckRegex.Checked;
            ft.FilterByUrl = this.CheckURL.Checked;
            ft.CaseSensitive = this.CheckCaseSensitive.Checked;
            ft.FilterRt = this.CheckRetweet.Checked;
            ft.UseLambda = this.CheckLambda.Checked;

            bdy = "";
            if (this.RadioExAnd.Checked)
            {
                ft.ExFilterName = this.ExUID.Text;
                ft.ExUseNameField = true;
                bdy = this.ExMSG1.Text;
            }
            else
            {
                ft.ExFilterName = "";
                ft.ExUseNameField = false;
                bdy = this.ExMSG2.Text;
            }
            ft.ExFilterSource = this.TextExSource.Text;

            if (this.CheckExRegex.Checked || this.CheckExLambDa.Checked)
            {
                ft.ExFilterBody = new[] { bdy };
            }
            else
            {
                ft.ExFilterBody = bdy.Split(' ', '　')
                    .Where(x => !MyCommon.IsNullOrEmpty(x))
                    .ToArray();
            }

            ft.ExUseRegex = this.CheckExRegex.Checked;
            ft.ExFilterByUrl = this.CheckExURL.Checked;
            ft.ExCaseSensitive = this.CheckExCaseSensitive.Checked;
            ft.ExFilterRt = this.CheckExRetweet.Checked;
            ft.ExUseLambda = this.CheckExLambDa.Checked;

            if (this.mode == EDITMODE.AddNew)
            {
                if (!tab.AddFilter(ft))
                    MessageBox.Show(Properties.Resources.ButtonOK_ClickText4, Properties.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.SetFilters(tab);
            this.ListFilters.SelectedIndex = -1;
            if (this.mode == EDITMODE.AddNew)
            {
                this.ListFilters.SelectedIndex = this.ListFilters.Items.Count - 1;
            }
            else
            {
                this.ListFilters.SelectedIndex = i;
            }
            this.mode = EDITMODE.None;

            if (this.directAdd)
            {
                this.Close();
            }
        }

        private bool IsValidLambdaExp(string text)
            => false; // TODO DynamicQuery相当のGPLv3互換なライブラリで置換する

        private bool IsValidRegexp(string text)
        {
            try
            {
                new Regex(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Properties.Resources.ButtonOK_ClickText3 + ex.Message, Properties.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        private bool CheckMatchRule(out bool isBlank)
        {
            isBlank = false;
            if (this.RadioAND.Checked)
            {
                if (MyCommon.IsNullOrEmpty(this.UID.Text) && MyCommon.IsNullOrEmpty(this.MSG1.Text) && MyCommon.IsNullOrEmpty(this.TextSource.Text) && this.CheckRetweet.Checked == false)
                {
                    isBlank = true;
                    return true;
                }
                if (this.CheckLambda.Checked)
                {
                    if (!this.IsValidLambdaExp(this.UID.Text))
                    {
                        return false;
                    }
                    if (!this.IsValidLambdaExp(this.MSG1.Text))
                    {
                        return false;
                    }
                }
                else if (this.CheckRegex.Checked)
                {
                    if (!this.IsValidRegexp(this.UID.Text))
                    {
                        return false;
                    }
                    if (!this.IsValidRegexp(this.MSG1.Text))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (MyCommon.IsNullOrEmpty(this.MSG2.Text) && MyCommon.IsNullOrEmpty(this.TextSource.Text) && this.CheckRetweet.Checked == false)
                {
                    isBlank = true;
                    return true;
                }
                if (this.CheckLambda.Checked && !this.IsValidLambdaExp(this.MSG2.Text))
                {
                    return false;
                }
                else if (this.CheckRegex.Checked && !this.IsValidRegexp(this.MSG2.Text))
                {
                    return false;
                }
            }

            if (this.CheckRegex.Checked && !this.IsValidRegexp(this.TextSource.Text))
            {
                return false;
            }
            return true;
        }

        private bool CheckExcludeRule(out bool isBlank)
        {
            isBlank = false;
            if (this.RadioExAnd.Checked)
            {
                if (MyCommon.IsNullOrEmpty(this.ExUID.Text) && MyCommon.IsNullOrEmpty(this.ExMSG1.Text) && MyCommon.IsNullOrEmpty(this.TextExSource.Text) && this.CheckExRetweet.Checked == false)
                {
                    isBlank = true;
                    return true;
                }
                if (this.CheckExLambDa.Checked)
                {
                    if (!this.IsValidLambdaExp(this.ExUID.Text))
                    {
                        return false;
                    }
                    if (!this.IsValidLambdaExp(this.ExMSG1.Text))
                    {
                        return false;
                    }
                }
                else if (this.CheckExRegex.Checked)
                {
                    if (!this.IsValidRegexp(this.ExUID.Text))
                    {
                        return false;
                    }
                    if (!this.IsValidRegexp(this.ExMSG1.Text))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (MyCommon.IsNullOrEmpty(this.ExMSG2.Text) && MyCommon.IsNullOrEmpty(this.TextExSource.Text) && this.CheckExRetweet.Checked == false)
                {
                    isBlank = true;
                    return true;
                }
                if (this.CheckExLambDa.Checked && !this.IsValidLambdaExp(this.ExMSG2.Text))
                {
                    return false;
                }
                else if (this.CheckExRegex.Checked && !this.IsValidRegexp(this.ExMSG2.Text))
                {
                    return false;
                }
            }

            if (this.CheckExRegex.Checked && !this.IsValidRegexp(this.TextExSource.Text))
            {
                return false;
            }

            return true;
        }

        private void ListFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.multiSelState != MultiSelectionState.None) // 複数選択処理中は無視する
                return;

            this.ShowDetail();

            var selectedCount = this.ListFilters.SelectedIndices.Count;
            if (selectedCount == 0)
            {
                this.RuleEnableButtonMode = EnableButtonMode.NotSelected;
            }
            else
            {
                if (selectedCount == 1 ||
                    this.RuleEnableButtonMode == EnableButtonMode.NotSelected)
                {
                    var topItem = (PostFilterRule)this.ListFilters.SelectedItem;
                    this.RuleEnableButtonMode = topItem.Enabled ? EnableButtonMode.Disable : EnableButtonMode.Enable;
                }
            }
        }

        private void ButtonClose_Click(object sender, EventArgs e)
            => this.Close();

        private void FilterDialog_FormClosed(object sender, FormClosedEventArgs e)
            => this.directAdd = false;

        private void FilterDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (this.EditFilterGroup.Enabled)
                    this.ButtonCancel_Click(this.ButtonCancel, EventArgs.Empty);
                else
                    this.ButtonClose_Click(this.ButtonClose, EventArgs.Empty);
            }
        }

        private void ListFilters_DoubleClick(object sender, EventArgs e)
        {
            var idx = this.ListFilters.SelectedIndex;
            if (idx == -1) return;

            var midx = this.ListFilters.IndexFromPoint(this.ListFilters.PointToClient(Control.MousePosition));
            if (midx == ListBox.NoMatches || idx != midx) return;

            this.ButtonEdit_Click(sender, e);
        }

        private void FilterDialog_Shown(object sender, EventArgs e)
        {
            this.ListTabs.DisplayMember = nameof(TabModel.TabName);

            this.ComboSound.Items.Clear();
            this.ComboSound.Items.Add("");
            var oDir = new DirectoryInfo(Application.StartupPath + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(Application.StartupPath, "Sounds")))
            {
                oDir = oDir.GetDirectories("Sounds")[0];
            }
            foreach (var oFile in oDir.GetFiles("*.wav"))
            {
                this.ComboSound.Items.Add(oFile.Name);
            }

            this.idlist.Clear();
            foreach (var tmp in ((TweenMain)this.Owner).AtIdSupl.GetItemList())
            {
                this.idlist.Add(tmp.Remove(0, 1));  // @文字削除
            }
            this.UID.AutoCompleteCustomSource.Clear();
            this.UID.AutoCompleteCustomSource.AddRange(this.idlist.ToArray());

            this.ExUID.AutoCompleteCustomSource.Clear();
            this.ExUID.AutoCompleteCustomSource.AddRange(this.idlist.ToArray());

            // 選択タブ変更
            this.ListTabs.SelectedIndex = this.selectedTabIndex;
        }

        private void ListTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectedTabIndex = this.ListTabs.SelectedIndex;

            var selectedTab = this.SelectedTab;
            if (selectedTab != null)
                this.SetFilters(selectedTab);
            else
                this.ListFilters.Items.Clear();
        }

        private async void ButtonAddTab_Click(object sender, EventArgs e)
        {
            string? tabName = null;
            MyCommon.TabUsageType tabType;
            using (var inputName = new InputTabName())
            {
                inputName.TabName = this.sts.MakeTabName("MyTab");
                inputName.IsShowUsage = true;
                inputName.ShowDialog();
                if (inputName.DialogResult == DialogResult.Cancel) return;
                tabName = inputName.TabName;
                tabType = inputName.Usage;
            }
            if (!MyCommon.IsNullOrEmpty(tabName))
            {
                // List対応
                ListElement? list = null;
                if (tabType == MyCommon.TabUsageType.Lists)
                {
                    try
                    {
                        using var dialog = new WaitingDialog(Properties.Resources.ListsGetting);
                        var cancellationToken = dialog.EnableCancellation();

                        var task = ((TweenMain)this.Owner).TwitterInstance.GetListsApi();
                        await dialog.WaitForAsync(this, task);

                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    catch (WebApiException ex)
                    {
                        MessageBox.Show("Failed to get lists. (" + ex.Message + ")");
                    }

                    using var listAvail = new ListAvailable();

                    if (listAvail.ShowDialog(this) == DialogResult.Cancel)
                        return;
                    if (listAvail.SelectedList == null)
                        return;
                    list = listAvail.SelectedList;
                }

                TabModel tab;
                switch (tabType)
                {
                    case MyCommon.TabUsageType.UserDefined:
                        tab = new FilterTabModel(tabName);
                        break;
                    case MyCommon.TabUsageType.PublicSearch:
                        tab = new PublicSearchTabModel(tabName);
                        break;
                    case MyCommon.TabUsageType.Lists:
                        tab = new ListTimelineTabModel(tabName, list!);
                        break;
                    default:
                        return;
                }

                if (!this.sts.AddTab(tab) || !((TweenMain)this.Owner).AddNewTab(tab, startup: false))
                {
                    var tmp = string.Format(Properties.Resources.AddTabMenuItem_ClickText1, tabName);
                    MessageBox.Show(tmp, Properties.Resources.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    // タブ作成成功
                    this.RefreshListTabs();
                }
            }
        }

        private void ButtonDeleteTab_Click(object sender, EventArgs e)
        {
            var selectedTab = this.SelectedTab;
            if (selectedTab != null)
            {
                var tb = selectedTab.TabName;
                var idx = this.ListTabs.SelectedIndex;
                if (((TweenMain)this.Owner).RemoveSpecifiedTab(tb, true))
                {
                    this.RefreshListTabs();
                    idx -= 1;
                    if (idx < 0) idx = 0;
                    this.ListTabs.SelectedIndex = idx;
                }
            }
        }

        private void ButtonRenameTab_Click(object sender, EventArgs e)
        {
            var selectedTab = this.SelectedTab;
            if (selectedTab != null)
            {
                var origTabName = selectedTab.TabName;
                if (((TweenMain)this.Owner).TabRename(origTabName, out _))
                    this.RefreshListTabs();
            }
        }

        private void CheckManageRead_CheckedChanged(object sender, EventArgs e)
        {
            var selectedTab = this.SelectedTab;
            if (selectedTab != null)
            {
                ((TweenMain)this.Owner).ChangeTabUnreadManage(
                    selectedTab.TabName,
                    this.CheckManageRead.Checked);
            }
        }

        private void ButtonUp_Click(object sender, EventArgs e)
        {
            var selectedIndex = this.ListTabs.SelectedIndex;

            if (selectedIndex == -1 || selectedIndex == 0)
                return;

            var selectedTab = this.tabs[selectedIndex];
            var selectedTabName = selectedTab.TabName;

            var targetTab = this.tabs[selectedIndex - 1];
            var targetTabName = targetTab.TabName;

            // ミュートタブは移動禁止
            if (selectedTab.TabType == MyCommon.TabUsageType.Mute || targetTab.TabType == MyCommon.TabUsageType.Mute)
                return;

            var tweenMain = (TweenMain)this.Owner;
            tweenMain.ReOrderTab(selectedTabName, targetTabName, true);

            this.RefreshListTabs();
        }

        private void ButtonDown_Click(object sender, EventArgs e)
        {
            var selectedIndex = this.ListTabs.SelectedIndex;

            if (selectedIndex == -1 || selectedIndex == this.ListTabs.Items.Count - 1)
                return;

            var selectedTab = this.tabs[selectedIndex];
            var selectedTabName = selectedTab.TabName;

            var targetTab = this.tabs[selectedIndex + 1];
            var targetTabName = targetTab.TabName;

            // ミュートタブは移動禁止
            if (selectedTab.TabType == MyCommon.TabUsageType.Mute || targetTab.TabType == MyCommon.TabUsageType.Mute)
                return;

            var tweenMain = (TweenMain)this.Owner;
            tweenMain.ReOrderTab(selectedTabName, targetTabName, false);

            this.RefreshListTabs();
        }

        private void CheckLocked_CheckedChanged(object sender, EventArgs e)
        {
            var selectedTab = this.SelectedTab;
            if (selectedTab != null)
            {
                selectedTab.Protected = this.CheckProtected.Checked;
                this.ButtonDeleteTab.Enabled = !this.CheckProtected.Checked;
            }
        }

        private void CheckNotifyNew_CheckedChanged(object sender, EventArgs e)
        {
            var selectedTab = this.SelectedTab;
            if (selectedTab != null)
            {
                selectedTab.Notify = this.CheckNotifyNew.Checked;
            }
        }

        private void ComboSound_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedTab = this.SelectedTab;
            if (selectedTab != null)
            {
                var filename = "";
                if (this.ComboSound.SelectedIndex > -1) filename = this.ComboSound.SelectedItem.ToString();
                selectedTab.SoundFile = filename;
            }
        }

        private void RadioExAnd_CheckedChanged(object sender, EventArgs e)
        {
            var flg = this.RadioExAnd.Checked;
            this.ExUID.Enabled = flg;
            this.ExMSG1.Enabled = flg;
            this.ExMSG2.Enabled = !flg;
        }

        private void OptMove_CheckedChanged(object sender, EventArgs e)
            => this.CheckMark.Enabled = !this.OptMove.Checked;

        private void ButtonRuleUp_Click(object sender, EventArgs e)
            => this.MoveSelectedRules(up: true);

        private void ButtonRuleDown_Click(object sender, EventArgs e)
            => this.MoveSelectedRules(up: false);

        private void MoveSelectedRules(bool up)
        {
            var selectedTab = this.SelectedTab;
            if (selectedTab == null || this.ListFilters.SelectedIndices.Count == 0)
                return;

            var indices = this.ListFilters.SelectedIndices.Cast<int>().ToArray();

            int diff;
            if (up)
            {
                if (indices[0] <= 0) return;
                diff = -1;
            }
            else
            {
                if (indices[indices.Length - 1] >= this.ListFilters.Items.Count - 1) return;
                diff = +1;
                Array.Reverse(indices);  // 逆順にして、下にある要素から処理する
            }

            var lastSelIdx = indices[0] + diff;
            var tab = (FilterTabModel)selectedTab;

            try
            {
                this.multiSelState |= MultiSelectionState.MoveSelected;

                using (ControlTransaction.Update(this.ListFilters))
                {
                    this.ListFilters.SelectedIndices.Clear();

                    foreach (var idx in indices)
                    {
                        var tidx = idx + diff;
                        var target = (PostFilterRule)this.ListFilters.Items[tidx];

                        // 移動先にある要素と位置を入れ替える
                        this.ListFilters.Items.RemoveAt(tidx);
                        this.ListFilters.Items.Insert(idx, target);

                        // 移動方向の先頭要素以外なら選択する
                        if (tidx != lastSelIdx)
                            this.ListFilters.SelectedIndex = tidx;
                    }

                    tab.FilterArray = this.ListFilters.Items.Cast<PostFilterRule>().ToArray();

                    // 移動方向の先頭要素は最後に選択する
                    // ※移動方向への自動スクロール目的
                    this.ListFilters.SelectedIndex = lastSelIdx;
                }
            }
            finally
            {
                this.multiSelState &= ~MultiSelectionState.MoveSelected;
            }
        }

        private void ButtonRuleToggleEnabled_Click(object sender, EventArgs e)
        {
            if (this.RuleEnableButtonMode == EnableButtonMode.NotSelected)
                return;

            var enabled = this.RuleEnableButtonMode == EnableButtonMode.Enable;

            foreach (var idx in this.ListFilters.SelectedIndices.Cast<int>())
            {
                var filter = (PostFilterRule)this.ListFilters.Items[idx];
                if (filter.Enabled != enabled)
                {
                    filter.Enabled = enabled;

                    var itemRect = this.ListFilters.GetItemRectangle(idx);
                    this.ListFilters.Invalidate(itemRect);
                }
            }

            this.RuleEnableButtonMode = enabled ? EnableButtonMode.Disable : EnableButtonMode.Enable;
        }

        private void ButtonRuleCopy_Click(object sender, EventArgs e)
        {
            var selectedTab = this.SelectedTab;
            if (selectedTab != null && this.ListFilters.SelectedItem != null)
            {
                TabModel[] destinationTabs;
                using (var dialog = new TabsDialog(this.sts))
                {
                    dialog.MultiSelect = true;
                    dialog.Text = Properties.Resources.ButtonRuleCopy_ClickText1;

                    if (dialog.ShowDialog(this) == DialogResult.Cancel) return;

                    destinationTabs = dialog.SelectedTabs;
                }

                var currentTab = (FilterTabModel)selectedTab;
                var filters = new List<PostFilterRule>();

                foreach (int idx in this.ListFilters.SelectedIndices)
                {
                    filters.Add(currentTab.FilterArray[idx].Clone());
                }
                foreach (var tb in destinationTabs.Cast<FilterTabModel>())
                {
                    if (tb.TabName == currentTab.TabName) continue;

                    foreach (var flt in filters)
                    {
                        if (!tb.FilterArray.Contains(flt))
                            tb.AddFilter(flt.Clone());
                    }
                }
                this.SetFilters(selectedTab);
            }
        }

        private void ButtonRuleMove_Click(object sender, EventArgs e)
        {
            var selectedTab = this.SelectedTab;
            if (selectedTab != null && this.ListFilters.SelectedItem != null)
            {
                TabModel[] destinationTabs;
                using (var dialog = new TabsDialog(this.sts))
                {
                    dialog.MultiSelect = true;
                    dialog.Text = Properties.Resources.ButtonRuleMove_ClickText1;

                    if (dialog.ShowDialog(this) == DialogResult.Cancel) return;

                    destinationTabs = dialog.SelectedTabs;
                }
                var currentTab = (FilterTabModel)selectedTab;
                var filters = new List<PostFilterRule>();

                foreach (int idx in this.ListFilters.SelectedIndices)
                {
                    filters.Add(currentTab.FilterArray[idx].Clone());
                }
                if (destinationTabs.Length == 1 && destinationTabs[0].TabName == currentTab.TabName) return;
                foreach (var tb in destinationTabs.Cast<FilterTabModel>())
                {
                    if (tb.TabName == currentTab.TabName) continue;

                    foreach (var flt in filters)
                    {
                        if (!tb.FilterArray.Contains(flt))
                            tb.AddFilter(flt.Clone());
                    }
                }
                for (var idx = this.ListFilters.Items.Count - 1; idx >= 0; idx--)
                {
                    if (this.ListFilters.GetSelected(idx))
                    {
                        currentTab.RemoveFilter((PostFilterRule)this.ListFilters.Items[idx]);
                        this.ListFilters.Items.RemoveAt(idx);
                    }
                }
                this.SetFilters(selectedTab);
            }
        }

        private void FilterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && e.Modifiers == (Keys.Shift | Keys.Control))
            {
                var main = (TweenMain)this.Owner;
                var tbox = (TextBox)sender;
                if (tbox.SelectionStart > 0)
                {
                    var endidx = tbox.SelectionStart - 1;
                    for (var i = tbox.SelectionStart - 1; i >= 0; i--)
                    {
                        var c = tbox.Text[i];
                        if (char.IsLetterOrDigit(c) || c == '_')
                        {
                            continue;
                        }
                        string startstr;
                        if (c == '@')
                        {
                            startstr = tbox.Text.Substring(i + 1, endidx - i);
                            main.ShowSuplDialog(tbox, main.AtIdSupl, startstr.Length + 1, startstr);
                        }
                        else if (c == '#')
                        {
                            startstr = tbox.Text.Substring(i + 1, endidx - i);
                            main.ShowSuplDialog(tbox, main.HashSupl, startstr.Length + 1, startstr);
                        }
                        else
                        {
                            break;
                        }
                    }
                    e.Handled = true;
                }
            }
        }

        private void FilterTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            var main = (TweenMain)this.Owner;
            var tbox = (TextBox)sender;
            if (e.KeyChar == '@')
            {
                // @マーク
                main.ShowSuplDialog(tbox, main.AtIdSupl);
                e.Handled = true;
            }
            else if (e.KeyChar == '#')
            {
                main.ShowSuplDialog(tbox, main.HashSupl);
                e.Handled = true;
            }
        }

        private void ListFilters_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index != -1)
            {
                var filter = (PostFilterRule)this.ListFilters.Items[e.Index];
                var isSelected = e.State.HasFlag(DrawItemState.Selected);

                Brush textBrush;
                if (isSelected)
                    textBrush = SystemBrushes.HighlightText;
                else if (filter.Enabled)
                    textBrush = SystemBrushes.WindowText;
                else
                    textBrush = SystemBrushes.GrayText;

                e.Graphics.DrawString(filter.ToString(), e.Font, textBrush, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        private void ListFilters_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                var itemCount = this.ListFilters.Items.Count;
                if (itemCount == 0) return;

                using (ControlTransaction.Update(this.ListFilters))
                {
                    if (itemCount > 1)
                    {
                        try
                        {
                            this.multiSelState |= MultiSelectionState.SelectAll;

                            for (var i = 1; i < itemCount; i++)
                            {
                                this.ListFilters.SetSelected(i, true);
                            }
                        }
                        finally
                        {
                            this.multiSelState &= ~MultiSelectionState.SelectAll;
                        }
                    }

                    this.ListFilters.SetSelected(0, true);
                }
            }
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);
            this.ListFilters.ItemHeight = this.ListFilters.Font.Height;
        }
    }
}
