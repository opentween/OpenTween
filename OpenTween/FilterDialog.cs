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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Specialized;
using OpenTween.Models;

namespace OpenTween
{
    public partial class FilterDialog : OTBaseForm
    {
        private EDITMODE _mode;
        private bool _directAdd;
        private MultiSelectionState _multiSelState = MultiSelectionState.None;
        private readonly TabInformations _sts;

        private List<TabModel> tabs = new List<TabModel>();
        private int selectedTabIndex = -1;

        private readonly List<string> idlist = new List<string>();

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
            get => this._ruleEnableButtonMode;
            set
            {
                this._ruleEnableButtonMode = value;

                this.buttonRuleToggleEnabled.Text = value == FilterDialog.EnableButtonMode.Enable
                    ? Properties.Resources.EnableButtonCaption
                    : Properties.Resources.DisableButtonCaption;
                this.buttonRuleToggleEnabled.Enabled = value != EnableButtonMode.NotSelected;
            }
        }
        private EnableButtonMode _ruleEnableButtonMode = FilterDialog.EnableButtonMode.NotSelected;

        public TabModel? SelectedTab
            => this.selectedTabIndex != -1 ? this.tabs[this.selectedTabIndex] : null;

        public FilterDialog()
        {
            this.InitializeComponent();

            this._sts = TabInformations.GetInstance();
            this.RefreshListTabs();
        }

        private void RefreshListTabs()
        {
            this.tabs = this._sts.Tabs.Append(this._sts.MuteTab).ToList();

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
            if (ListTabs.Items.Count == 0) return;

            ListFilters.Items.Clear();

            if (tab is FilterTabModel filterTab)
                ListFilters.Items.AddRange(filterTab.GetFilters());

            if (ListFilters.Items.Count > 0)
                ListFilters.SelectedIndex = 0;
            else
                ShowDetail();

            if (tab.IsDefaultTabType)
            {
                CheckProtected.Checked = true;
                CheckProtected.Enabled = false;
            }
            else
            {
                CheckProtected.Checked = tab.Protected;
                CheckProtected.Enabled = true;
            }

            CheckManageRead.CheckedChanged -= this.CheckManageRead_CheckedChanged;
            CheckManageRead.Checked = tab.UnreadManage;
            CheckManageRead.CheckedChanged += this.CheckManageRead_CheckedChanged;

            CheckNotifyNew.CheckedChanged -= this.CheckNotifyNew_CheckedChanged;
            CheckNotifyNew.Checked = tab.Notify;
            CheckNotifyNew.CheckedChanged += this.CheckNotifyNew_CheckedChanged;

            var idx = ComboSound.Items.IndexOf(tab.SoundFile);
            if (idx == -1) idx = 0;
            ComboSound.SelectedIndex = idx;

            if (_directAdd) return;

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

            ListTabs.Enabled = true;
            GroupTab.Enabled = true;
            ListFilters.Enabled = true;
            EditFilterGroup.Enabled = false;

            if (tab.IsDistributableTabType)
            {
                ButtonNew.Enabled = true;
                if (ListFilters.SelectedIndex > -1)
                {
                    ButtonEdit.Enabled = true;
                    ButtonDelete.Enabled = true;
                    ButtonRuleUp.Enabled = true;
                    ButtonRuleDown.Enabled = true;
                    ButtonRuleCopy.Enabled = true;
                    ButtonRuleMove.Enabled = true;
                    buttonRuleToggleEnabled.Enabled = true;
                }
                else
                {
                    ButtonEdit.Enabled = false;
                    ButtonDelete.Enabled = false;
                    ButtonRuleUp.Enabled = false;
                    ButtonRuleDown.Enabled = false;
                    ButtonRuleCopy.Enabled = false;
                    ButtonRuleMove.Enabled = false;
                    buttonRuleToggleEnabled.Enabled = false;
                }
            }
            else
            {
                ButtonNew.Enabled = false;
                ButtonEdit.Enabled = false;
                ButtonDelete.Enabled = false;
                ButtonRuleUp.Enabled = false;
                ButtonRuleDown.Enabled = false;
                ButtonRuleCopy.Enabled = false;
                ButtonRuleMove.Enabled = false;
                buttonRuleToggleEnabled.Enabled = false;
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
                ButtonDeleteTab.Enabled = false;
            }
            else
            {
                ButtonDeleteTab.Enabled = true;
            }
            ButtonClose.Enabled = true;
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
            ButtonNew.Enabled = false;
            ButtonEdit.Enabled = false;
            ButtonRuleUp.Enabled = false;
            ButtonRuleDown.Enabled = false;
            ButtonRuleCopy.Enabled = false;
            ButtonRuleMove.Enabled = false;
            buttonRuleToggleEnabled.Enabled = false;
            ButtonDelete.Enabled = false;
            ButtonClose.Enabled = false;
            EditFilterGroup.Enabled = true;
            ListTabs.Enabled = false;
            GroupTab.Enabled = false;
            ListFilters.Enabled = false;

            RadioAND.Checked = true;
            RadioPLUS.Checked = false;
            UID.Text = id;
            UID.SelectAll();
            MSG1.Text = msg;
            MSG1.SelectAll();
            MSG2.Text = id + msg;
            MSG2.SelectAll();
            TextSource.Text = "";
            UID.Enabled = true;
            MSG1.Enabled = true;
            MSG2.Enabled = false;
            CheckRegex.Checked = false;
            CheckURL.Checked = false;
            CheckCaseSensitive.Checked = false;
            CheckRetweet.Checked = false;
            CheckLambda.Checked = false;

            RadioExAnd.Checked = true;
            RadioExPLUS.Checked = false;
            ExUID.Text = "";
            ExUID.SelectAll();
            ExMSG1.Text = "";
            ExMSG1.SelectAll();
            ExMSG2.Text = "";
            ExMSG2.SelectAll();
            TextExSource.Text = "";
            ExUID.Enabled = true;
            ExMSG1.Enabled = true;
            ExMSG2.Enabled = false;
            CheckExRegex.Checked = false;
            CheckExURL.Checked = false;
            CheckExCaseSensitive.Checked = false;
            CheckExRetweet.Checked = false;
            CheckExLambDa.Checked = false;

            OptCopy.Checked = true;
            CheckMark.Checked = true;
            UID.Focus();
            _mode = EDITMODE.AddNew;
            _directAdd = true;
        }

        private void ButtonNew_Click(object sender, EventArgs e)
        {
            ButtonNew.Enabled = false;
            ButtonEdit.Enabled = false;
            ButtonClose.Enabled = false;
            ButtonRuleUp.Enabled = false;
            ButtonRuleDown.Enabled = false;
            ButtonRuleCopy.Enabled = false;
            ButtonRuleMove.Enabled = false;
            buttonRuleToggleEnabled.Enabled = false;
            ButtonDelete.Enabled = false;
            ButtonClose.Enabled = false;
            EditFilterGroup.Enabled = true;
            ListTabs.Enabled = false;
            GroupTab.Enabled = false;
            ListFilters.Enabled = false;

            RadioAND.Checked = true;
            RadioPLUS.Checked = false;
            UID.Text = "";
            MSG1.Text = "";
            MSG2.Text = "";
            TextSource.Text = "";
            UID.Enabled = true;
            MSG1.Enabled = true;
            MSG2.Enabled = false;
            CheckRegex.Checked = false;
            CheckURL.Checked = false;
            CheckCaseSensitive.Checked = false;
            CheckRetweet.Checked = false;
            CheckLambda.Checked = false;

            RadioExAnd.Checked = true;
            RadioExPLUS.Checked = false;
            ExUID.Text = "";
            ExMSG1.Text = "";
            ExMSG2.Text = "";
            TextExSource.Text = "";
            ExUID.Enabled = true;
            ExMSG1.Enabled = true;
            ExMSG2.Enabled = false;
            CheckExRegex.Checked = false;
            CheckExURL.Checked = false;
            CheckExCaseSensitive.Checked = false;
            CheckExRetweet.Checked = false;
            CheckExLambDa.Checked = false;

            OptCopy.Checked = true;
            CheckMark.Checked = true;
            UID.Focus();
            _mode = EDITMODE.AddNew;
        }

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (ListFilters.SelectedIndex == -1) return;

            ShowDetail();

            var idx = ListFilters.SelectedIndex;
            ListFilters.SelectedIndex = -1;
            ListFilters.SelectedIndex = idx;
            ListFilters.Enabled = false;

            ButtonNew.Enabled = false;
            ButtonEdit.Enabled = false;
            ButtonDelete.Enabled = false;
            ButtonClose.Enabled = false;
            ButtonRuleUp.Enabled = false;
            ButtonRuleDown.Enabled = false;
            ButtonRuleCopy.Enabled = false;
            ButtonRuleMove.Enabled = false;
            buttonRuleToggleEnabled.Enabled = false;
            EditFilterGroup.Enabled = true;
            ListTabs.Enabled = false;
            GroupTab.Enabled = false;

            _mode = EDITMODE.Edit;
        }

        private void ButtonDelete_Click(object sender, EventArgs e)
        {
            var selectedCount = ListFilters.SelectedIndices.Count;
            if (selectedCount == 0) return;

            string tmp;

            if (selectedCount == 1)
            {
                tmp = string.Format(Properties.Resources.ButtonDelete_ClickText1, Environment.NewLine, ListFilters.SelectedItem);
            }
            else
            {
                tmp = string.Format(Properties.Resources.ButtonDelete_ClickText3, selectedCount);
            }

            var rslt = MessageBox.Show(tmp, Properties.Resources.ButtonDelete_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (rslt == DialogResult.Cancel) return;

            var indices = ListFilters.SelectedIndices.Cast<int>().Reverse().ToArray();  // 後ろの要素から削除
            var tab = (FilterTabModel)this.SelectedTab!;

            using (ControlTransaction.Update(ListFilters))
            {
                foreach (var idx in indices)
                {
                    tab.RemoveFilter((PostFilterRule)ListFilters.Items[idx]);
                    ListFilters.Items.RemoveAt(idx);
                }
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            ListTabs.Enabled = true;
            GroupTab.Enabled = true;
            ListFilters.Enabled = true;
            ListFilters.Focus();
            if (ListFilters.SelectedIndex != -1)
            {
                ShowDetail();
            }
            EditFilterGroup.Enabled = false;
            ButtonNew.Enabled = true;
            if (ListFilters.SelectedIndex > -1)
            {
                ButtonEdit.Enabled = true;
                ButtonDelete.Enabled = true;
                ButtonRuleUp.Enabled = true;
                ButtonRuleDown.Enabled = true;
                ButtonRuleCopy.Enabled = true;
                ButtonRuleMove.Enabled = true;
                buttonRuleToggleEnabled.Enabled = true;
            }
            else
            {
                ButtonEdit.Enabled = false;
                ButtonDelete.Enabled = false;
                ButtonRuleUp.Enabled = false;
                ButtonRuleDown.Enabled = false;
                ButtonRuleCopy.Enabled = false;
                ButtonRuleMove.Enabled = false;
                buttonRuleToggleEnabled.Enabled = false;
            }
            ButtonClose.Enabled = true;
            if (_directAdd)
            {
                this.Close();
            }
        }

        private void ShowDetail()
        {
            if (_directAdd) return;

            if (ListFilters.SelectedIndex > -1)
            {
                var fc = (PostFilterRule)ListFilters.SelectedItem;
                if (fc.UseNameField)
                {
                    RadioAND.Checked = true;
                    RadioPLUS.Checked = false;
                    UID.Enabled = true;
                    MSG1.Enabled = true;
                    MSG2.Enabled = false;
                    UID.Text = fc.FilterName;
                    UID.SelectAll();
                    MSG1.Text = string.Join(" ", fc.FilterBody);
                    MSG1.SelectAll();
                    MSG2.Text = "";
                }
                else
                {
                    RadioPLUS.Checked = true;
                    RadioAND.Checked = false;
                    UID.Enabled = false;
                    MSG1.Enabled = false;
                    MSG2.Enabled = true;
                    UID.Text = "";
                    MSG1.Text = "";
                    MSG2.Text = string.Join(" ", fc.FilterBody);
                    MSG2.SelectAll();
                }
                TextSource.Text = fc.FilterSource;
                CheckRegex.Checked = fc.UseRegex;
                CheckURL.Checked = fc.FilterByUrl;
                CheckCaseSensitive.Checked = fc.CaseSensitive;
                CheckRetweet.Checked = fc.FilterRt;
                CheckLambda.Checked = fc.UseLambda;

                if (fc.ExUseNameField)
                {
                    RadioExAnd.Checked = true;
                    RadioExPLUS.Checked = false;
                    ExUID.Enabled = true;
                    ExMSG1.Enabled = true;
                    ExMSG2.Enabled = false;
                    ExUID.Text = fc.ExFilterName;
                    ExUID.SelectAll();
                    ExMSG1.Text = string.Join(" ", fc.ExFilterBody);
                    ExMSG1.SelectAll();
                    ExMSG2.Text = "";
                }
                else
                {
                    RadioExPLUS.Checked = true;
                    RadioExAnd.Checked = false;
                    ExUID.Enabled = false;
                    ExMSG1.Enabled = false;
                    ExMSG2.Enabled = true;
                    ExUID.Text = "";
                    ExMSG1.Text = "";
                    ExMSG2.Text = string.Join(" ", fc.ExFilterBody);
                    ExMSG2.SelectAll();
                }
                TextExSource.Text = fc.ExFilterSource;
                CheckExRegex.Checked = fc.ExUseRegex;
                CheckExURL.Checked = fc.ExFilterByUrl;
                CheckExCaseSensitive.Checked = fc.ExCaseSensitive;
                CheckExRetweet.Checked = fc.ExFilterRt;
                CheckExLambDa.Checked = fc.ExUseLambda;

                if (fc.MoveMatches)
                {
                    OptMove.Checked = true;
                }
                else
                {
                    OptCopy.Checked = true;
                }
                CheckMark.Checked = fc.MarkMatches;

                ButtonEdit.Enabled = true;
                ButtonDelete.Enabled = true;
                ButtonRuleUp.Enabled = true;
                ButtonRuleDown.Enabled = true;
                ButtonRuleCopy.Enabled = true;
                ButtonRuleMove.Enabled = true;
                buttonRuleToggleEnabled.Enabled = true;
            }
            else
            {
                RadioAND.Checked = true;
                RadioPLUS.Checked = false;
                UID.Enabled = true;
                MSG1.Enabled = true;
                MSG2.Enabled = false;
                UID.Text = "";
                MSG1.Text = "";
                MSG2.Text = "";
                TextSource.Text = "";
                CheckRegex.Checked = false;
                CheckURL.Checked = false;
                CheckCaseSensitive.Checked = false;
                CheckRetweet.Checked = false;
                CheckLambda.Checked = false;

                RadioExAnd.Checked = true;
                RadioExPLUS.Checked = false;
                ExUID.Enabled = true;
                ExMSG1.Enabled = true;
                ExMSG2.Enabled = false;
                ExUID.Text = "";
                ExMSG1.Text = "";
                ExMSG2.Text = "";
                TextExSource.Text = "";
                CheckExRegex.Checked = false;
                CheckExURL.Checked = false;
                CheckExCaseSensitive.Checked = false;
                CheckExRetweet.Checked = false;
                CheckExLambDa.Checked = false;

                OptCopy.Checked = true;
                CheckMark.Checked = true;

                ButtonEdit.Enabled = false;
                ButtonDelete.Enabled = false;
                ButtonRuleUp.Enabled = false;
                ButtonRuleDown.Enabled = false;
                ButtonRuleCopy.Enabled = false;
                ButtonRuleMove.Enabled = false;
                buttonRuleToggleEnabled.Enabled = false;
            }
        }

        private void RadioAND_CheckedChanged(object sender, EventArgs e)
        {
            var flg = RadioAND.Checked;
            UID.Enabled = flg;
            MSG1.Enabled = flg;
            MSG2.Enabled = !flg;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            // 入力チェック
            if (!CheckMatchRule(out var isBlankMatch) || !CheckExcludeRule(out var isBlankExclude))
            {
                return;
            }
            if (isBlankMatch && isBlankExclude)
            {
                MessageBox.Show(Properties.Resources.ButtonOK_ClickText1, Properties.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var tab = (FilterTabModel)this.SelectedTab!;
            var i = ListFilters.SelectedIndex;

            PostFilterRule ft;
            if (_mode == EDITMODE.AddNew)
                ft = new PostFilterRule();
            else
                ft = (PostFilterRule)this.ListFilters.SelectedItem;

            if (tab.TabType != MyCommon.TabUsageType.Mute)
            {
                ft.MoveMatches = OptMove.Checked;
                ft.MarkMatches = CheckMark.Checked;
            }
            else
            {
                ft.MoveMatches = true;
                ft.MarkMatches = false;
            }

            var bdy = "";
            if (RadioAND.Checked)
            {
                ft.FilterName = UID.Text;
                var owner = (TweenMain)this.Owner;
                var cnt = owner.AtIdSupl.ItemCount;
                owner.AtIdSupl.AddItem("@" + ft.FilterName);
                if (cnt != owner.AtIdSupl.ItemCount)
                {
                    owner.MarkSettingAtIdModified();
                }
                ft.UseNameField = true;
                bdy = MSG1.Text;
            }
            else
            {
                ft.FilterName = "";
                ft.UseNameField = false;
                bdy = MSG2.Text;
            }
            ft.FilterSource = TextSource.Text;

            if (CheckRegex.Checked || CheckLambda.Checked)
            {
                ft.FilterBody = new[] { bdy };
            }
            else
            {
                ft.FilterBody = bdy.Split(' ', '　')
                    .Where(x => !MyCommon.IsNullOrEmpty(x))
                    .ToArray();
            }

            ft.UseRegex = CheckRegex.Checked;
            ft.FilterByUrl = CheckURL.Checked;
            ft.CaseSensitive = CheckCaseSensitive.Checked;
            ft.FilterRt = CheckRetweet.Checked;
            ft.UseLambda = CheckLambda.Checked;

            bdy = "";
            if (RadioExAnd.Checked)
            {
                ft.ExFilterName = ExUID.Text;
                ft.ExUseNameField = true;
                bdy = ExMSG1.Text;
            }
            else
            {
                ft.ExFilterName = "";
                ft.ExUseNameField = false;
                bdy = ExMSG2.Text;
            }
            ft.ExFilterSource = TextExSource.Text;

            if (CheckExRegex.Checked || CheckExLambDa.Checked)
            {
                ft.ExFilterBody = new[] { bdy };
            }
            else
            {
                ft.ExFilterBody = bdy.Split(' ', '　')
                    .Where(x => !MyCommon.IsNullOrEmpty(x))
                    .ToArray();
            }

            ft.ExUseRegex = CheckExRegex.Checked;
            ft.ExFilterByUrl = CheckExURL.Checked;
            ft.ExCaseSensitive = CheckExCaseSensitive.Checked;
            ft.ExFilterRt = CheckExRetweet.Checked;
            ft.ExUseLambda = CheckExLambDa.Checked;

            if (_mode == EDITMODE.AddNew)
            {
                if (!tab.AddFilter(ft))
                    MessageBox.Show(Properties.Resources.ButtonOK_ClickText4, Properties.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            SetFilters(tab);
            ListFilters.SelectedIndex = -1;
            if (_mode == EDITMODE.AddNew)
            {
                ListFilters.SelectedIndex = ListFilters.Items.Count - 1;
            }
            else
            {
                ListFilters.SelectedIndex = i;
            }
            _mode = EDITMODE.None;

            if (_directAdd)
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
            if (RadioAND.Checked)
            {
                if (MyCommon.IsNullOrEmpty(UID.Text) && MyCommon.IsNullOrEmpty(MSG1.Text) && MyCommon.IsNullOrEmpty(TextSource.Text) && CheckRetweet.Checked == false)
                {
                    isBlank = true;
                    return true;
                }
                if (CheckLambda.Checked)
                {
                    if (!IsValidLambdaExp(UID.Text))
                    {
                        return false;
                    }
                    if (!IsValidLambdaExp(MSG1.Text))
                    {
                        return false;
                    }
                }
                else if (CheckRegex.Checked)
                {
                    if (!IsValidRegexp(UID.Text))
                    {
                        return false;
                    }
                    if (!IsValidRegexp(MSG1.Text))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (MyCommon.IsNullOrEmpty(MSG2.Text) && MyCommon.IsNullOrEmpty(TextSource.Text) && CheckRetweet.Checked == false)
                {
                    isBlank = true;
                    return true;
                }
                if (CheckLambda.Checked && !IsValidLambdaExp(MSG2.Text))
                {
                    return false;
                }
                else if (CheckRegex.Checked && !IsValidRegexp(MSG2.Text))
                {
                    return false;
                }
            }

            if (CheckRegex.Checked && !IsValidRegexp(TextSource.Text))
            {
                return false;
            }
            return true;
        }

        private bool CheckExcludeRule(out bool isBlank)
        {
            isBlank = false;
            if (RadioExAnd.Checked)
            {
                if (MyCommon.IsNullOrEmpty(ExUID.Text) && MyCommon.IsNullOrEmpty(ExMSG1.Text) && MyCommon.IsNullOrEmpty(TextExSource.Text) && CheckExRetweet.Checked == false)
                {
                    isBlank = true;
                    return true;
                }
                if (CheckExLambDa.Checked)
                {
                    if (!IsValidLambdaExp(ExUID.Text))
                    {
                        return false;
                    }
                    if (!IsValidLambdaExp(ExMSG1.Text))
                    {
                        return false;
                    }
                }
                else if (CheckExRegex.Checked)
                {
                    if (!IsValidRegexp(ExUID.Text))
                    {
                        return false;
                    }
                    if (!IsValidRegexp(ExMSG1.Text))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (MyCommon.IsNullOrEmpty(ExMSG2.Text) && MyCommon.IsNullOrEmpty(TextExSource.Text) && CheckExRetweet.Checked == false)
                {
                    isBlank = true;
                    return true;
                }
                if (CheckExLambDa.Checked && !IsValidLambdaExp(ExMSG2.Text))
                {
                    return false;
                }
                else if (CheckExRegex.Checked && !IsValidRegexp(ExMSG2.Text))
                {
                    return false;
                }
            }

            if (CheckExRegex.Checked && !IsValidRegexp(TextExSource.Text))
            {
                return false;
            }

            return true;
        }

        private void ListFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_multiSelState != MultiSelectionState.None)  // 複数選択処理中は無視する
                return;

            ShowDetail();

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
            => this._directAdd = false;

        private void FilterDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (EditFilterGroup.Enabled)
                    ButtonCancel_Click(this.ButtonCancel, EventArgs.Empty);
                else
                    ButtonClose_Click(this.ButtonClose, EventArgs.Empty);
            }
        }

        private void ListFilters_DoubleClick(object sender, EventArgs e)
        {
            var idx = ListFilters.SelectedIndex;
            if (idx == -1) return;

            var midx = ListFilters.IndexFromPoint(ListFilters.PointToClient(Control.MousePosition));
            if (midx == ListBox.NoMatches || idx != midx) return;

            ButtonEdit_Click(sender, e);
        }

        private void FilterDialog_Shown(object sender, EventArgs e)
        {
            ListTabs.DisplayMember = nameof(TabModel.TabName);

            ComboSound.Items.Clear();
            ComboSound.Items.Add("");
            var oDir = new DirectoryInfo(Application.StartupPath + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(Application.StartupPath, "Sounds")))
            {
                oDir = oDir.GetDirectories("Sounds")[0];
            }
            foreach (var oFile in oDir.GetFiles("*.wav"))
            {
                ComboSound.Items.Add(oFile.Name);
            }

            idlist.Clear();
            foreach (var tmp in ((TweenMain)this.Owner).AtIdSupl.GetItemList())
            {
                idlist.Add(tmp.Remove(0, 1));  // @文字削除
            }
            UID.AutoCompleteCustomSource.Clear();
            UID.AutoCompleteCustomSource.AddRange(idlist.ToArray());

            ExUID.AutoCompleteCustomSource.Clear();
            ExUID.AutoCompleteCustomSource.AddRange(idlist.ToArray());

            // 選択タブ変更
            this.ListTabs.SelectedIndex = this.selectedTabIndex;
        }

        private void ListTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectedTabIndex = this.ListTabs.SelectedIndex;

            var selectedTab = this.SelectedTab;
            if (selectedTab != null)
                SetFilters(selectedTab);
            else
                ListFilters.Items.Clear();
        }

        private async void ButtonAddTab_Click(object sender, EventArgs e)
        {
            string? tabName = null;
            MyCommon.TabUsageType tabType;
            using (var inputName = new InputTabName())
            {
                inputName.TabName = _sts.MakeTabName("MyTab");
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
                    catch (OperationCanceledException) { return; }
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

                if (!_sts.AddTab(tab) || !((TweenMain)this.Owner).AddNewTab(tab, startup: false))
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
                var idx = ListTabs.SelectedIndex;
                if (((TweenMain)this.Owner).RemoveSpecifiedTab(tb, true))
                {
                    this.RefreshListTabs();
                    idx -= 1;
                    if (idx < 0) idx = 0;
                    ListTabs.SelectedIndex = idx;
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
                    CheckManageRead.Checked);
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
                selectedTab.Protected = CheckProtected.Checked;
                ButtonDeleteTab.Enabled = !CheckProtected.Checked;
            }
        }

        private void CheckNotifyNew_CheckedChanged(object sender, EventArgs e)
        {
            var selectedTab = this.SelectedTab;
            if (selectedTab != null)
            {
                selectedTab.Notify = CheckNotifyNew.Checked;
            }
        }

        private void ComboSound_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedTab = this.SelectedTab;
            if (selectedTab != null)
            {
                var filename = "";
                if (ComboSound.SelectedIndex > -1) filename = ComboSound.SelectedItem.ToString();
                selectedTab.SoundFile = filename;
            }
        }

        private void RadioExAnd_CheckedChanged(object sender, EventArgs e)
        {
            var flg = RadioExAnd.Checked;
            ExUID.Enabled = flg;
            ExMSG1.Enabled = flg;
            ExMSG2.Enabled = !flg;
        }

        private void OptMove_CheckedChanged(object sender, EventArgs e)
            => this.CheckMark.Enabled = !OptMove.Checked;

        private void ButtonRuleUp_Click(object sender, EventArgs e)
            => this.MoveSelectedRules(up: true);

        private void ButtonRuleDown_Click(object sender, EventArgs e)
            => this.MoveSelectedRules(up: false);

        private void MoveSelectedRules(bool up)
        {
            var selectedTab = this.SelectedTab;
            if (selectedTab == null || ListFilters.SelectedIndices.Count == 0)
                return;

            var indices = ListFilters.SelectedIndices.Cast<int>().ToArray();

            int diff;
            if (up)
            {
                if (indices[0] <= 0) return;
                diff = -1;
            }
            else
            {
                if (indices[indices.Length - 1] >= ListFilters.Items.Count - 1) return;
                diff = +1;
                Array.Reverse(indices);  // 逆順にして、下にある要素から処理する
            }

            var lastSelIdx = indices[0] + diff;
            var tab = (FilterTabModel)selectedTab;

            try
            {
                _multiSelState |= MultiSelectionState.MoveSelected;

                using (ControlTransaction.Update(ListFilters))
                {
                    ListFilters.SelectedIndices.Clear();

                    foreach (var idx in indices)
                    {
                        var tidx = idx + diff;
                        var target = (PostFilterRule)ListFilters.Items[tidx];

                        // 移動先にある要素と位置を入れ替える
                        ListFilters.Items.RemoveAt(tidx);
                        ListFilters.Items.Insert(idx, target);

                        // 移動方向の先頭要素以外なら選択する
                        if (tidx != lastSelIdx)
                            ListFilters.SelectedIndex = tidx;
                    }

                    tab.FilterArray = ListFilters.Items.Cast<PostFilterRule>().ToArray();

                    // 移動方向の先頭要素は最後に選択する
                    // ※移動方向への自動スクロール目的
                    ListFilters.SelectedIndex = lastSelIdx;
                }
            }
            finally
            {
                _multiSelState &= ~MultiSelectionState.MoveSelected;
            }
        }

        private void buttonRuleToggleEnabled_Click(object sender, EventArgs e)
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
            if (selectedTab != null && ListFilters.SelectedItem != null)
            {
                TabModel[] destinationTabs;
                using (var dialog = new TabsDialog(_sts))
                {
                    dialog.MultiSelect = true;
                    dialog.Text = Properties.Resources.ButtonRuleCopy_ClickText1;

                    if (dialog.ShowDialog(this) == DialogResult.Cancel) return;

                    destinationTabs = dialog.SelectedTabs;
                }

                var currentTab = (FilterTabModel)selectedTab;
                var filters = new List<PostFilterRule>();

                foreach (int idx in ListFilters.SelectedIndices)
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
                SetFilters(selectedTab);
            }
        }

        private void ButtonRuleMove_Click(object sender, EventArgs e)
        {
            var selectedTab = this.SelectedTab;
            if (selectedTab != null && ListFilters.SelectedItem != null)
            {
                TabModel[] destinationTabs;
                using (var dialog = new TabsDialog(_sts))
                {
                    dialog.MultiSelect = true;
                    dialog.Text = Properties.Resources.ButtonRuleMove_ClickText1;

                    if (dialog.ShowDialog(this) == DialogResult.Cancel) return;

                    destinationTabs = dialog.SelectedTabs;
                }
                var currentTab = (FilterTabModel)selectedTab;
                var filters = new List<PostFilterRule>();

                foreach (int idx in ListFilters.SelectedIndices)
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
                for (var idx = ListFilters.Items.Count - 1; idx >= 0; idx--)
                {
                    if (ListFilters.GetSelected(idx))
                    {
                        currentTab.RemoveFilter((PostFilterRule)ListFilters.Items[idx]);
                        ListFilters.Items.RemoveAt(idx);
                    }
                }
                SetFilters(selectedTab);
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
                            _multiSelState |= MultiSelectionState.SelectAll;

                            for (var i = 1; i < itemCount; i++)
                            {
                                this.ListFilters.SetSelected(i, true);
                            }
                        }
                        finally
                        {
                            _multiSelState &= ~MultiSelectionState.SelectAll;
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
