// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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

namespace OpenTween
{
    public partial class SearchWordDialog : OTBaseForm
    {
        public enum SearchType
        {
            /// <summary>
            /// タイムライン内を検索
            /// </summary>
            Timeline,

            /// <summary>
            /// Twitter検索
            /// </summary>
            Public,
        }

        public record SearchOptions(
            SearchType Type,
            string Query,
            bool NewTab, // タイムライン内検索のみで使用する
            bool CaseSensitive,
            bool UseRegex
        );

        private SearchOptions? resultOptions = null;

        public SearchOptions? ResultOptions
        {
            get => this.resultOptions;
            set
            {
                this.resultOptions = value;

                if (value == null)
                {
                    this.Reset();
                    return;
                }

                switch (value.Type)
                {
                    case SearchType.Timeline:
                        this.tabControl.SelectedTab = this.tabPageTimeline;
                        this.textSearchTimeline.Text = value.Query;
                        this.checkTimelineCaseSensitive.Checked = value.CaseSensitive;
                        this.checkTimelineRegex.Checked = value.UseRegex;
                        break;
                    case SearchType.Public:
                        this.tabControl.SelectedTab = this.tabPagePublic;
                        this.textSearchTimeline.Text = value.Query;
                        break;
                    default:
                        throw new InvalidEnumArgumentException("value", (int)value.Type, typeof(SearchType));
                }
            }
        }

        private bool disableNetTabButton = false;

        public bool DisableNewTabButton
        {
            get => this.disableNetTabButton;
            set
            {
                this.disableNetTabButton = value;
                this.buttonSearchTimelineNew.Enabled = !value;
            }
        }

        public SearchWordDialog()
            => this.InitializeComponent();

        public void Reset()
        {
            this.tabControl.SelectedTab = this.tabPageTimeline;

            this.textSearchTimeline.ResetText();
            this.checkTimelineCaseSensitive.Checked = false;
            this.checkTimelineRegex.Checked = false;

            this.textSearchPublic.ResetText();
        }

        private void SearchWordDialog_Shown(object sender, EventArgs e)
            => this.ActivateSelectedTabPage();

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
            => this.ActivateSelectedTabPage();

        private void ActivateSelectedTabPage()
        {
            if (this.tabControl.SelectedTab == this.tabPageTimeline)
            {
                this.AcceptButton = this.buttonSearchTimeline;
                this.textSearchTimeline.SelectAll();
                this.textSearchTimeline.Focus();
            }
            else
            {
                this.AcceptButton = this.buttonSearchPublic;
                this.textSearchPublic.SelectAll();
                this.textSearchPublic.Focus();
            }
        }

        private void ButtonSearchTimeline_Click(object sender, EventArgs e)
        {
            if (MyCommon.IsNullOrEmpty(this.textSearchTimeline.Text))
            {
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            this.DialogResult = DialogResult.OK;

            this.ResultOptions = new SearchOptions(
                SearchType.Timeline,
                this.textSearchTimeline.Text,
                false,
                this.checkTimelineCaseSensitive.Checked,
                this.checkTimelineRegex.Checked
            );
        }

        private void ButtonSearchTimelineNew_Click(object sender, EventArgs e)
        {
            if (MyCommon.IsNullOrEmpty(this.textSearchTimeline.Text))
            {
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            this.DialogResult = DialogResult.OK;

            this.ResultOptions = new SearchOptions(
                SearchType.Timeline,
                this.textSearchTimeline.Text,
                true,
                this.checkTimelineCaseSensitive.Checked,
                this.checkTimelineRegex.Checked
            );
        }

        private void ButtonSearchPublic_Click(object sender, EventArgs e)
        {
            if (MyCommon.IsNullOrEmpty(this.textSearchPublic.Text))
            {
                this.DialogResult = DialogResult.Cancel;
                return;
            }

            this.DialogResult = DialogResult.OK;

            this.ResultOptions = new SearchOptions(
                SearchType.Public,
                this.textSearchPublic.Text,
                true,
                false,
                false
            );
        }

        private async void LinkLabelSearchHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 「検索オプションの使い方」ページのURL
            const string PublicSearchHelpUrl = "https://support.twitter.com/articles/249059";
            await MyCommon.OpenInBrowserAsync(this, PublicSearchHelpUrl);
        }

        private void SearchWordDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.DialogResult = DialogResult.Cancel;
        }
    }
}
