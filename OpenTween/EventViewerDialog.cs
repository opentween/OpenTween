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
// under the terms of the GNU General public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
// 
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General public License
// for more details. 
// 
// You should have received a copy of the GNU General public License along
// with this program. if (not, see <http://www.gnu.org/licenses/>, or write to
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
using System.Text.RegularExpressions;
using System.IO;

namespace OpenTween
{
    public partial class EventViewerDialog : Form
    {
        public List<Twitter.FormattedEvent> EventSource { get; set; }

        private Twitter.FormattedEvent[] _filterdEventSource;

        private ListViewItem[] _ItemCache = null;
        private int _itemCacheIndex;

        private TabPage _curTab = null;

        public EventViewerDialog()
        {
            InitializeComponent();
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private ListViewItem CreateListViewItem(Twitter.FormattedEvent source)
        {
            string[] s = { source.CreatedAt.ToString(), source.Event.ToUpper(), source.Username, source.Target };
            return new ListViewItem(s);
        }

        private void EventViewerDialog_Shown(object sender, EventArgs e)
        {
            EventList.BeginUpdate();
            _curTab = TabEventType.SelectedTab;
            CreateFilterdEventSource();
            EventList.EndUpdate();
            this.TopMost = AppendSettingDialog.Instance.AlwaysTop;
        }

        private void EventList_DoubleClick(object sender, EventArgs e)
        {
            if (EventList.SelectedIndices.Count != 0 && _filterdEventSource[EventList.SelectedIndices[0]] != null)
            {
                ((TweenMain)this.Owner).OpenUriAsync("http://twitter.com/" + _filterdEventSource[EventList.SelectedIndices[0]].Username);
            }
        }

        private MyCommon.EVENTTYPE ParseEventTypeFromTag()
        {
            return (MyCommon.EVENTTYPE)Enum.Parse(typeof(MyCommon.EVENTTYPE), _curTab.Tag.ToString());
        }

        private bool IsFilterMatch(Twitter.FormattedEvent x)
        {
            if (!CheckBoxFilter.Checked || string.IsNullOrEmpty(TextBoxKeyword.Text))
            {
                return true;
            }
            else
            {
                if (CheckRegex.Checked)
                {
                    try
                    {
                        Regex rx = new Regex(TextBoxKeyword.Text);
                        return rx.Match(x.Username).Success || rx.Match(x.Target).Success;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Properties.Resources.ButtonOK_ClickText3 + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                }
                else
                {
                    return x.Username.Contains(TextBoxKeyword.Text) || x.Target.Contains(TextBoxKeyword.Text);
                }
            }
        }

        private void CreateFilterdEventSource()
        {
            if (EventSource != null && EventSource.Count > 0)
            {
                _filterdEventSource = EventSource.FindAll((x) => (CheckExcludeMyEvent.Checked ? !x.IsMe : true) &&
                                                                 (x.Eventtype & ParseEventTypeFromTag()) != 0 &&
                                                                 IsFilterMatch(x)).ToArray();
                _ItemCache = null;
                EventList.VirtualListSize = _filterdEventSource.Count();
                StatusLabelCount.Text = string.Format("{0} / {1}", _filterdEventSource.Count(), EventSource.Count());
            }
            else
            {
                StatusLabelCount.Text = "0 / 0";
            }
        }

        private void CheckExcludeMyEvent_CheckedChanged(object sender, EventArgs e)
        {
            CreateFilterdEventSource();
        }

        private void ButtonRefresh_Click(object sender, EventArgs e)
        {
            CreateFilterdEventSource();
        }

        private void TabEventType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateFilterdEventSource();
        }

        private void TabEventType_Selecting(object sender, TabControlCancelEventArgs e)
        {
            _curTab = e.TabPage;
            if (!e.TabPage.Controls.Contains(EventList))
            {
                e.TabPage.Controls.Add(EventList);
            }
        }

        private void TextBoxKeyword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                CreateFilterdEventSource();
                e.Handled = true;
            }
        }

        private void EventList_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (_ItemCache != null && e.ItemIndex >= _itemCacheIndex && e.ItemIndex < _itemCacheIndex + _ItemCache.Length)
            {
                //キャッシュヒット
                e.Item = _ItemCache[e.ItemIndex - _itemCacheIndex];
            }
            else
            {
                //キャッシュミス
                e.Item = CreateListViewItem(_filterdEventSource[e.ItemIndex]);
            }
        }

        private void EventList_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            CreateCache(e.StartIndex, e.EndIndex);
        }

        private void CreateCache(int StartIndex, int EndIndex)
        {
            //キャッシュ要求（要求範囲±30を作成）
            StartIndex -= 30;
            if (StartIndex < 0) StartIndex = 0;
            EndIndex += 30;
            if (EndIndex > _filterdEventSource.Count() - 1)
            {
                EndIndex = _filterdEventSource.Count() - 1;
            }
            _ItemCache = new ListViewItem[] { };
            Array.Resize(ref _ItemCache, EndIndex - StartIndex + 1);
            _itemCacheIndex = StartIndex;
            for (int i = 0; i < _ItemCache.Length; i++)
            {
                _ItemCache[i] = CreateListViewItem(_filterdEventSource[StartIndex + i]);
            }
        }

        private void SaveLogButton_Click(object sender, EventArgs e)
        {
            DialogResult rslt = MessageBox.Show(string.Format(Properties.Resources.SaveLogMenuItem_ClickText5, Environment.NewLine),
                    Properties.Resources.SaveLogMenuItem_ClickText2,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            switch (rslt)
            {
                case DialogResult.Yes:
                    SaveFileDialog1.FileName = MyCommon.GetAssemblyName() + "Events" + _curTab.Tag.ToString() + DateTime.Now.ToString("yyMMdd-HHmmss") + ".tsv";
                    break;
                case DialogResult.No:
                    SaveFileDialog1.FileName = MyCommon.GetAssemblyName() + "Events" + DateTime.Now.ToString("yyMMdd-HHmmss") + ".tsv";
                    break;
                default:
                    return;
            }

            SaveFileDialog1.InitialDirectory = Application.StartupPath;
            SaveFileDialog1.Filter = Properties.Resources.SaveLogMenuItem_ClickText3;
            SaveFileDialog1.FilterIndex = 0;
            SaveFileDialog1.Title = Properties.Resources.SaveLogMenuItem_ClickText4;
            SaveFileDialog1.RestoreDirectory = true;

            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!SaveFileDialog1.ValidateNames) return;
                using (StreamWriter sw = new StreamWriter(SaveFileDialog1.FileName, false, Encoding.UTF8))
                {
                    switch (rslt)
                    {
                        case DialogResult.Yes:
                            SaveEventLog(_filterdEventSource.ToList(), sw);
                            break;
                        case DialogResult.No:
                            SaveEventLog(EventSource, sw);
                            break;
                        default:
                            //
                            break;
                    }
                }
            }
            this.TopMost = AppendSettingDialog.Instance.AlwaysTop;
        }

        private void SaveEventLog(List<Twitter.FormattedEvent> source, StreamWriter sw)
        {
            foreach (Twitter.FormattedEvent _event in source)
            {
                sw.WriteLine(_event.Eventtype.ToString() + "\t" +
                             "\"" + _event.CreatedAt.ToString() + "\"\t" +
                             _event.Event + "\t" +
                             _event.Username + "\t" +
                             _event.Target + "\t" +
                             _event.Id.ToString());
            }
        }
    }
}
