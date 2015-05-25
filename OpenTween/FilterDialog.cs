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

namespace OpenTween
{
    public partial class FilterDialog : OTBaseForm
    {
        private EDITMODE _mode;
        private bool _directAdd;
        private MultiSelectionState _multiSelState = MultiSelectionState.None;
        private TabInformations _sts;
        private string _cur;
        private List<string> idlist = new List<string>();

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
            get { return this._ruleEnableButtonMode; }
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

        public FilterDialog()
        {
            InitializeComponent();
        }

        private void SetFilters(string tabName)
        {
            if (ListTabs.Items.Count == 0) return;

            var tab = _sts.Tabs[tabName];

            ListFilters.Items.Clear();
            ListFilters.Items.AddRange(tab.GetFilters());

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

            CheckManageRead.Checked = tab.UnreadManage;
            CheckNotifyNew.Checked = tab.Notify;

            int idx = ComboSound.Items.IndexOf(tab.SoundFile);
            if (idx == -1) idx = 0;
            ComboSound.SelectedIndex = idx;

            if (_directAdd) return;

            if (tab.TabType == MyCommon.TabUsageType.Mute)
            {
                this.CheckManageRead.Enabled = false;
                this.CheckNotifyNew.Enabled = false;
                this.ComboSound.Enabled = false;

                this.GroupBox1.Visible = false;
                this.labelMuteTab.Visible = true;
            }
            else
            {
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
            switch (tab.TabType)
            {
                case MyCommon.TabUsageType.Home:
                case MyCommon.TabUsageType.DirectMessage:
                case MyCommon.TabUsageType.Favorites:
                case MyCommon.TabUsageType.PublicSearch:
                case MyCommon.TabUsageType.Lists:
                case MyCommon.TabUsageType.Related:
                case MyCommon.TabUsageType.UserTimeline:
                    ButtonNew.Enabled = false;
                    ButtonEdit.Enabled = false;
                    ButtonDelete.Enabled = false;
                    ButtonRuleUp.Enabled = false;
                    ButtonRuleDown.Enabled = false;
                    ButtonRuleCopy.Enabled = false;
                    ButtonRuleMove.Enabled = false;
                    buttonRuleToggleEnabled.Enabled = false;
                    break;
                default:
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
                    break;
            }
            switch (tab.TabType)
            {
                case MyCommon.TabUsageType.Home:
                    LabelTabType.Text = Properties.Resources.TabUsageTypeName_Home;
                    break;
                case MyCommon.TabUsageType.Mentions:
                    LabelTabType.Text = Properties.Resources.TabUsageTypeName_Mentions;
                    break;
                case MyCommon.TabUsageType.DirectMessage:
                    LabelTabType.Text = Properties.Resources.TabUsageTypeName_DirectMessage;
                    break;
                case MyCommon.TabUsageType.Favorites:
                    LabelTabType.Text = Properties.Resources.TabUsageTypeName_Favorites;
                    break;
                case MyCommon.TabUsageType.UserDefined:
                    LabelTabType.Text = Properties.Resources.TabUsageTypeName_UserDefined;
                    break;
                case MyCommon.TabUsageType.PublicSearch:
                    LabelTabType.Text = Properties.Resources.TabUsageTypeName_PublicSearch;
                    break;
                case MyCommon.TabUsageType.Lists:
                    LabelTabType.Text = Properties.Resources.TabUsageTypeName_Lists;
                    break;
                case MyCommon.TabUsageType.Related:
                    LabelTabType.Text = Properties.Resources.TabUsageTypeName_Related;
                    break;
                case MyCommon.TabUsageType.UserTimeline:
                    LabelTabType.Text = Properties.Resources.TabUsageTypeName_UserTimeline;
                    break;
                case MyCommon.TabUsageType.Mute:
                    LabelTabType.Text = "Mute";
                    break;
                default:
                    LabelTabType.Text = "UNKNOWN";
                    break;
            }
            ButtonRenameTab.Enabled = true;
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

        public void SetCurrent(string TabName)
        {
            _cur = TabName;
        }

        public void AddNewFilter(string id, string msg)
        {
            //元フォームから直接呼ばれる
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

            int idx = ListFilters.SelectedIndex;
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
                tmp = string.Format(Properties.Resources.ButtonDelete_ClickText1, Environment.NewLine, ListFilters.SelectedItem.ToString());
            }
            else
            {
                tmp = string.Format(Properties.Resources.ButtonDelete_ClickText3, selectedCount.ToString());
            }

            var rslt = MessageBox.Show(tmp, Properties.Resources.ButtonDelete_ClickText2, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (rslt == DialogResult.Cancel) return;

            var indices = ListFilters.SelectedIndices.Cast<int>().Reverse().ToArray();  // 後ろの要素から削除
            var tab = _sts.Tabs[ListTabs.SelectedItem.ToString()];

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
                PostFilterRule fc = (PostFilterRule)ListFilters.SelectedItem;
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
            bool flg = RadioAND.Checked;
            UID.Enabled = flg;
            MSG1.Enabled = flg;
            MSG2.Enabled = !flg;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            bool isBlankMatch = false;
            bool isBlankExclude = false;

            //入力チェック
            if (!CheckMatchRule(out isBlankMatch) || !CheckExcludeRule(out isBlankExclude))
            {
                return;
            }
            if (isBlankMatch && isBlankExclude)
            {
                MessageBox.Show(Properties.Resources.ButtonOK_ClickText1, Properties.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var tab = this._sts.Tabs[(string)this.ListTabs.SelectedItem];
            int i = ListFilters.SelectedIndex;

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

            string bdy = "";
            if (RadioAND.Checked)
            {
                ft.FilterName = UID.Text;
                TweenMain owner = (TweenMain)this.Owner;
                int cnt = owner.AtIdSupl.ItemCount;
                owner.AtIdSupl.AddItem("@" + ft.FilterName);
                if (cnt != owner.AtIdSupl.ItemCount)
                {
                    owner.ModifySettingAtId = true;
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
                    .Where(x => !string.IsNullOrEmpty(x))
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
                    .Where(x => !string.IsNullOrEmpty(x))
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

            SetFilters(tab.TabName);
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
        {
            return false;
            // TODO DynamicQuery相当のGPLv3互換なライブラリで置換する
        }

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
                if (string.IsNullOrEmpty(UID.Text) && string.IsNullOrEmpty(MSG1.Text) && string.IsNullOrEmpty(TextSource.Text) && CheckRetweet.Checked == false)
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
                if (string.IsNullOrEmpty(MSG2.Text) && string.IsNullOrEmpty(TextSource.Text) && CheckRetweet.Checked == false)
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
                if (string.IsNullOrEmpty(ExUID.Text) && string.IsNullOrEmpty(ExMSG1.Text) && string.IsNullOrEmpty(TextExSource.Text) && CheckExRetweet.Checked == false)
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
                if (string.IsNullOrEmpty(ExMSG2.Text) && string.IsNullOrEmpty(TextExSource.Text) && CheckExRetweet.Checked == false)
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
            if (_multiSelState != MultiSelectionState.None)  //複数選択処理中は無視する
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
        {
            this.Close();
        }

        private void FilterDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            _directAdd = false;
        }

        private void FilterDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (EditFilterGroup.Enabled)
                    ButtonCancel_Click(null, null);
                else
                    ButtonClose_Click(null, null);
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
            _sts = TabInformations.GetInstance();
            ListTabs.Items.Clear();
            foreach (var tab in this._sts.Tabs.Values)
            {
                if (tab.TabType == MyCommon.TabUsageType.Mute)
                    continue;

                this.ListTabs.Items.Add(tab.TabName);
            }

            var muteTab = this._sts.GetTabByType(MyCommon.TabUsageType.Mute);
            if (muteTab != null)
                this.ListTabs.Items.Add(muteTab.TabName);

            ComboSound.Items.Clear();
            ComboSound.Items.Add("");
            DirectoryInfo oDir = new DirectoryInfo(Application.StartupPath + Path.DirectorySeparatorChar);
            if (Directory.Exists(Path.Combine(Application.StartupPath, "Sounds")))
            {
                oDir = oDir.GetDirectories("Sounds")[0];
            }
            foreach (FileInfo oFile in oDir.GetFiles("*.wav"))
            {
                ComboSound.Items.Add(oFile.Name);
            }

            idlist.Clear();
            foreach (string tmp in ((TweenMain)this.Owner).AtIdSupl.GetItemList())
            {
                idlist.Add(tmp.Remove(0, 1));  // @文字削除
            }
            UID.AutoCompleteCustomSource.Clear();
            UID.AutoCompleteCustomSource.AddRange(idlist.ToArray());

            ExUID.AutoCompleteCustomSource.Clear();
            ExUID.AutoCompleteCustomSource.AddRange(idlist.ToArray());

            //選択タブ変更
            if (ListTabs.Items.Count > 0)
            {
                if (_cur.Length > 0)
                {
                    for (int i = 0; i < ListTabs.Items.Count; i++)
                    {
                        if (_cur == ListTabs.Items[i].ToString())
                        {
                            ListTabs.SelectedIndex = i;
                            //tabdialog.TabList.Items.Remove(_cur);
                            break;
                        }
                    }
                }
            }
        }

        private void ListTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1)
                SetFilters(ListTabs.SelectedItem.ToString());
            else
                ListFilters.Items.Clear();
        }

        private void ButtonAddTab_Click(object sender, EventArgs e)
        {
            string tabName = null;
            MyCommon.TabUsageType tabType;
            using (InputTabName inputName = new InputTabName())
            {
                inputName.TabName = _sts.GetUniqueTabName();
                inputName.IsShowUsage = true;
                inputName.ShowDialog();
                if (inputName.DialogResult == DialogResult.Cancel) return;
                tabName = inputName.TabName;
                tabType = inputName.Usage;
            }
            if (!string.IsNullOrEmpty(tabName))
            {
                //List対応
                ListElement list = null;
                if (tabType == MyCommon.TabUsageType.Lists)
                {
                    string rslt = ((TweenMain)this.Owner).TwitterInstance.GetListsApi();
                    if (!string.IsNullOrEmpty(rslt))
                    {
                        MessageBox.Show("Failed to get lists. (" + rslt + ")");
                    }
                    using (ListAvailable listAvail = new ListAvailable())
                    {
                        if (listAvail.ShowDialog(this) == DialogResult.Cancel) return;
                        if (listAvail.SelectedList == null) return;
                        list = listAvail.SelectedList;
                    }
                }
                if (!_sts.AddTab(tabName, tabType, list) || !((TweenMain)this.Owner).AddNewTab(tabName, false, tabType, list))
                {
                    string tmp = string.Format(Properties.Resources.AddTabMenuItem_ClickText1, tabName);
                    MessageBox.Show(tmp, Properties.Resources.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                else
                {
                    // タブ作成成功

                    // 末尾のタブを取得する
                    var lastIdx = this.ListTabs.Items.Count - 1;
                    var lastTab = lastIdx != -1
                        ? this._sts.Tabs[(string)this.ListTabs.Items[lastIdx]]
                        : null;

                    // 末尾がミュートタブであればその手前に追加する
                    if (lastTab != null && lastTab.TabType == MyCommon.TabUsageType.Mute)
                        this.ListTabs.Items.Insert(lastIdx, tabName);
                    else
                        this.ListTabs.Items.Add(tabName);
                }
            }
        }

        private void ButtonDeleteTab_Click(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string tb = ListTabs.SelectedItem.ToString();
                int idx = ListTabs.SelectedIndex;
                if (((TweenMain)this.Owner).RemoveSpecifiedTab(tb, true))
                {
                    ListTabs.Items.RemoveAt(idx);
                    idx -= 1;
                    if (idx < 0) idx = 0;
                    ListTabs.SelectedIndex = idx;
                }
            }
        }

        private void ButtonRenameTab_Click(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string tb = ListTabs.SelectedItem.ToString();
                int idx = ListTabs.SelectedIndex;
                if (((TweenMain)this.Owner).TabRename(ref tb))
                {
                    ListTabs.Items.RemoveAt(idx);
                    ListTabs.Items.Insert(idx, tb);
                    ListTabs.SelectedIndex = idx;
                }
            }
        }

        private void CheckManageRead_CheckedChanged(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                ((TweenMain)this.Owner).ChangeTabUnreadManage(
                    ListTabs.SelectedItem.ToString(),
                    CheckManageRead.Checked);
            }
        }

        private void ButtonUp_Click(object sender, EventArgs e)
        {
            if (this.ListTabs.SelectedIndex == -1 || this.ListTabs.SelectedIndex == 0)
                return;

            var selectedTabName = (string)this.ListTabs.SelectedItem;
            var selectedTab = this._sts.Tabs[selectedTabName];

            var targetTabName = (string)this.ListTabs.Items[this.ListTabs.SelectedIndex - 1];
            var targetTab = this._sts.Tabs[targetTabName];

            // ミュートタブは移動禁止
            if (selectedTab.TabType == MyCommon.TabUsageType.Mute || targetTab.TabType == MyCommon.TabUsageType.Mute)
                return;

            var tweenMain = (TweenMain)this.Owner;
            tweenMain.ReOrderTab(selectedTabName, targetTabName, true);

            // ListTab のアイテム並び替え
            // 選択が解除されてしまうのを防ぐため SelectedIndex のアイテムは操作せず前後のアイテムを移動する
            var idx = this.ListTabs.SelectedIndex;
            this.ListTabs.Items.RemoveAt(idx - 1);
            this.ListTabs.Items.Insert(idx, targetTabName);
        }

        private void ButtonDown_Click(object sender, EventArgs e)
        {
            if (this.ListTabs.SelectedIndex == -1 || this.ListTabs.SelectedIndex == this.ListTabs.Items.Count - 1)
                return;

            var selectedTabName = (string)this.ListTabs.SelectedItem;
            var selectedTab = this._sts.Tabs[selectedTabName];

            var targetTabName = (string)this.ListTabs.Items[this.ListTabs.SelectedIndex + 1];
            var targetTab = this._sts.Tabs[targetTabName];

            // ミュートタブは移動禁止
            if (selectedTab.TabType == MyCommon.TabUsageType.Mute || targetTab.TabType == MyCommon.TabUsageType.Mute)
                return;

            var tweenMain = (TweenMain)this.Owner;
            tweenMain.ReOrderTab(selectedTabName, targetTabName, false);

            // ListTab のアイテム並び替え
            // 選択が解除されてしまうのを防ぐため SelectedIndex のアイテムは操作せず前後のアイテムを移動する
            var idx = this.ListTabs.SelectedIndex;
            this.ListTabs.Items.RemoveAt(idx + 1);
            this.ListTabs.Items.Insert(idx, targetTabName);
        }

        private void CheckLocked_CheckedChanged(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                _sts.Tabs[ListTabs.SelectedItem.ToString()].Protected = CheckProtected.Checked;
                ButtonDeleteTab.Enabled = !CheckProtected.Checked;
            }
        }

        private void CheckNotifyNew_CheckedChanged(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && !string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                _sts.Tabs[ListTabs.SelectedItem.ToString()].Notify = CheckNotifyNew.Checked;
            }
        }

        private void ComboSound_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && string.IsNullOrEmpty(ListTabs.SelectedItem.ToString()))
            {
                string filename = "";
                if (ComboSound.SelectedIndex > -1) filename = ComboSound.SelectedItem.ToString();
                _sts.Tabs[ListTabs.SelectedItem.ToString()].SoundFile = filename;
            }
        }

        private void RadioExAnd_CheckedChanged(object sender, EventArgs e)
        {
            bool flg = RadioExAnd.Checked;
            ExUID.Enabled = flg;
            ExMSG1.Enabled = flg;
            ExMSG2.Enabled = !flg;
        }

        private void OptMove_CheckedChanged(object sender, EventArgs e)
        {
            CheckMark.Enabled = !OptMove.Checked;
        }

        private void ButtonRuleUp_Click(object sender, EventArgs e)
        {
            MoveSelectedRules(up: true);
        }

        private void ButtonRuleDown_Click(object sender, EventArgs e)
        {
            MoveSelectedRules(up: false);
        }

        private void MoveSelectedRules(bool up)
        {
            var tabIdx = ListTabs.SelectedIndex;
            if (tabIdx == -1 ||
                ListFilters.SelectedIndices.Count == 0) return;

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
            var tab = _sts.Tabs[ListTabs.Items[tabIdx].ToString()];

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
            if (ListTabs.SelectedIndex > -1 && ListFilters.SelectedItem != null)
            {
                TabClass[] selectedTabs;
                using (TabsDialog dialog = new TabsDialog(_sts))
                {
                    dialog.MultiSelect = true;
                    dialog.Text = Properties.Resources.ButtonRuleCopy_ClickText1;

                    if (dialog.ShowDialog(this) == DialogResult.Cancel) return;

                    selectedTabs = dialog.SelectedTabs;
                }

                string tabname = ListTabs.SelectedItem.ToString();
                List<PostFilterRule> filters = new List<PostFilterRule>();

                foreach (int idx in ListFilters.SelectedIndices)
                {
                    filters.Add(_sts.Tabs[tabname].FilterArray[idx].Clone());
                }
                foreach (var tb in selectedTabs)
                {
                    if (tb.TabName == tabname) continue;

                    foreach (PostFilterRule flt in filters)
                    {
                        if (!tb.FilterArray.Contains(flt))
                            tb.AddFilter(flt.Clone());
                    }
                }
                SetFilters(tabname);
            }
        }

        private void ButtonRuleMove_Click(object sender, EventArgs e)
        {
            if (ListTabs.SelectedIndex > -1 && ListFilters.SelectedItem != null)
            {
                TabClass[] selectedTabs;
                using (var dialog = new TabsDialog(_sts))
                {
                    dialog.MultiSelect = true;
                    dialog.Text = Properties.Resources.ButtonRuleMove_ClickText1;

                    if (dialog.ShowDialog(this) == DialogResult.Cancel) return;

                    selectedTabs = dialog.SelectedTabs;
                }
                string tabname = ListTabs.SelectedItem.ToString();
                List<PostFilterRule> filters = new List<PostFilterRule>();

                foreach (int idx in ListFilters.SelectedIndices)
                {
                    filters.Add(_sts.Tabs[tabname].FilterArray[idx].Clone());
                }
                if (selectedTabs.Length == 1 && selectedTabs[0].TabName == tabname) return;
                foreach (var tb in selectedTabs)
                {
                    if (tb.TabName == tabname) continue;

                    foreach (PostFilterRule flt in filters)
                    {
                        if (!tb.FilterArray.Contains(flt))
                            tb.AddFilter(flt.Clone());
                    }
                }
                for (int idx = ListFilters.Items.Count - 1; idx >= 0; idx--)
                {
                    if (ListFilters.GetSelected(idx))
                    {
                        _sts.Tabs[ListTabs.SelectedItem.ToString()].RemoveFilter((PostFilterRule)ListFilters.Items[idx]);
                        ListFilters.Items.RemoveAt(idx);
                    }
                }
                SetFilters(tabname);
            }
        }

        private void FilterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && e.Modifiers == (Keys.Shift | Keys.Control))
            {
                TweenMain main = (TweenMain)this.Owner;
                TextBox tbox = (TextBox)sender;
                if (tbox.SelectionStart > 0)
                {
                    int endidx = tbox.SelectionStart - 1;
                    string startstr = "";
                    for (int i = tbox.SelectionStart - 1; i >= 0; i--)
                    {
                        char c = tbox.Text[i];
                        if (Char.IsLetterOrDigit(c) || c == '_')
                        {
                            continue;
                        }
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
            TweenMain main = (TweenMain)this.Owner;
            TextBox tbox = (TextBox)sender;
            if (e.KeyChar == '@')
            {
                //if (!SettingDialog.UseAtIdSupplement) return;
                //@マーク
                main.ShowSuplDialog(tbox, main.AtIdSupl);
                e.Handled = true;
            }
            else if (e.KeyChar == '#')
            {
                //if (!SettingDialog.UseHashSupplement) return;
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

                            for (int i = 1; i < itemCount; i++)
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

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            this.ListFilters.ItemHeight = this.ListFilters.Font.Height;
        }
    }
}
