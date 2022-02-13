// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2012      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

#nullable enable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenTween
{
    public partial class HashtagManage : OTBaseForm
    {
        public string UseHash { get; private set; } = "";
        public bool IsPermanent { get; private set; } = false;
        public bool IsHead { get; private set; } = false;
        public bool IsNotAddToAtReply { get; private set; } = true;

        /// <summary>
        /// エラー時にダイアログを表示させない (ユニットテストなどで使用)
        /// </summary>
        public bool RunSilent { get; set; }

        // 入力補助画面
        private readonly AtIdSupplement _hashSupl;

        // 編集モード
        private bool _isAdd = false;

        private void ChangeMode(bool isEdit)
        {
            this.GroupHashtag.Enabled = !isEdit;
            this.GroupDetail.Enabled = isEdit;
            this.TableLayoutButtons.Enabled = !isEdit;
            if (isEdit)
                this.UseHashText.Focus();
            else
                this.HistoryHashList.Focus();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            this.UseHashText.Text = "";
            ChangeMode(true);
            _isAdd = true;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (this.HistoryHashList.SelectedIndices.Count == 0) return;
            this.UseHashText.Text = this.HistoryHashList.SelectedItems[0].ToString();
            ChangeMode(true);
            _isAdd = false;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (this.HistoryHashList.SelectedIndices.Count == 0) return;
            if (!this.RunSilent &&
                MessageBox.Show(Properties.Resources.DeleteHashtagsMessage1,
                                "Delete Hashtags",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Question) == DialogResult.Cancel)
            {
                return;
            }

            // 削除によってインデックス番号が変わらないように逆順に処理する
            var selectedIndices = this.HistoryHashList.SelectedIndices.Cast<int>()
                .OrderByDescending(x => x).ToArray();

            foreach (var idx in selectedIndices)
            {
                if (UseHashText.Text == HistoryHashList.Items[idx].ToString()) UseHashText.Text = "";
                HistoryHashList.Items.RemoveAt(idx);
            }
            if (HistoryHashList.Items.Count > 0)
            {
                HistoryHashList.SelectedIndex = 0;
            }
        }

        private void UnSelectButton_Click(object sender, EventArgs e)
        {
            do
            {
                HistoryHashList.SelectedIndices.Clear();
            } while (HistoryHashList.SelectedIndices.Count > 0);
        }

        private int GetIndexOf(ListBox.ObjectCollection list, string value)
        {
            if (MyCommon.IsNullOrEmpty(value)) return -1;

            var idx = 0;

            foreach (var l in list)
            {
                var src = (string)l;
                if (MyCommon.IsNullOrEmpty(src))
                {
                    idx += 1;
                    continue;
                }
                if (string.Compare(src, value, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return idx;
                }
                idx += 1;
            }

            // Not Found
            return -1;
        }

        public void AddHashToHistory(string hash, bool isIgnorePermanent)
        {
            hash = hash.Trim();
            if (!MyCommon.IsNullOrEmpty(hash))
            {
                if (isIgnorePermanent || !this.IsPermanent)
                {
                    // 無条件に先頭に挿入
                    var idx = GetIndexOf(HistoryHashList.Items, hash);

                    if (idx != -1) HistoryHashList.Items.RemoveAt(idx);
                    HistoryHashList.Items.Insert(0, hash);
                }
                else
                {
                    // 固定されていたら2行目に挿入
                    var idx = GetIndexOf(HistoryHashList.Items, hash);
                    if (this.IsPermanent)
                    {
                        if (idx > 0)
                        {
                            // 重複アイテムが2行目以降にあれば2行目へ
                            HistoryHashList.Items.RemoveAt(idx);
                            HistoryHashList.Items.Insert(1, hash);
                        }
                        else if (idx == -1)
                        {
                            // 重複アイテムなし
                            if (HistoryHashList.Items.Count == 0)
                            {
                                // リストが空なら追加
                                HistoryHashList.Items.Add(hash);
                            }
                            else
                            {
                                // リストにアイテムがあれば2行目へ
                                HistoryHashList.Items.Insert(1, hash);
                            }
                        }
                    }
                }
            }
        }

        private void HashtagManage_Shown(object sender, EventArgs e)
        {
            // オプション
            this.CheckPermanent.Checked = this.IsPermanent;
            this.RadioHead.Checked = this.IsHead;
            this.RadioLast.Checked = !this.IsHead;
            // リスト選択
            if (this.HistoryHashList.Items.Contains(this.UseHash))
            {
                this.HistoryHashList.SelectedItem = this.UseHash;
            }
            else
            {
                if (this.HistoryHashList.Items.Count > 0)
                    this.HistoryHashList.SelectedIndex = 0;
            }
            this.ChangeMode(false);
        }

        public HashtagManage(AtIdSupplement hashSuplForm, string[] history, string permanentHash, bool IsPermanent, bool IsHead, bool IsNotAddToAtReply)
        {
            // この呼び出しは、Windows フォーム デザイナで必要です。
            InitializeComponent();

            // InitializeComponent() 呼び出しの後で初期化を追加します。

            _hashSupl = hashSuplForm;
            HistoryHashList.Items.AddRange(history);
            this.UseHash = permanentHash;
            this.IsPermanent = IsPermanent;
            this.IsHead = IsHead;
            this.IsNotAddToAtReply = IsNotAddToAtReply;
        }

        private void UseHashText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '#')
            {
                _hashSupl.ShowDialog();
                if (!MyCommon.IsNullOrEmpty(_hashSupl.inputText))
                {
                    var fHalf = "";
                    var eHalf = "";
                    var selStart = UseHashText.SelectionStart;
                    if (selStart > 0)
                    {
                        fHalf = UseHashText.Text.Substring(0, selStart);
                    }
                    if (selStart < UseHashText.Text.Length)
                    {
                        eHalf = UseHashText.Text.Substring(selStart);
                    }
                    UseHashText.Text = fHalf + _hashSupl.inputText + eHalf;
                    UseHashText.SelectionStart = selStart + _hashSupl.inputText.Length;
                }
                e.Handled = true;
            }
        }

        private void HistoryHashList_DoubleClick(object sender, EventArgs e)
            => this.OK_Button_Click(this.OK_Button, EventArgs.Empty);

        public void ToggleHash()
        {
            if (MyCommon.IsNullOrEmpty(this.UseHash))
            {
                if (this.HistoryHashList.Items.Count > 0)
                    this.UseHash = this.HistoryHashList.Items[0].ToString();
            }
            else
            {
                this.UseHash = "";
            }
        }

        public List<string> HashHistories
        {
            get
            {
                var hash = new List<string>();
                foreach (string item in HistoryHashList.Items)
                {
                    hash.Add(item);
                }
                return hash;
            }
        }

        public void ClearHashtag()
            => this.UseHash = "";

        public void SetPermanentHash(string hash)
        {
            // 固定ハッシュタグの変更
            this.UseHash = hash.Trim();
            this.AddHashToHistory(UseHash, false);
            this.IsPermanent = true;
        }

        private void PermOK_Button_Click(object sender, EventArgs e)
        {
            // ハッシュタグの整形
            var hashStr = UseHashText.Text;
            if (!this.AdjustHashtags(ref hashStr, !this.RunSilent)) return;

            UseHashText.Text = hashStr;
            if (!this._isAdd && this.HistoryHashList.SelectedIndices.Count > 0)
            {
                var idx = this.HistoryHashList.SelectedIndices[0];
                this.HistoryHashList.Items.RemoveAt(idx);
                do
                {
                    this.HistoryHashList.SelectedIndices.Clear();
                } while (this.HistoryHashList.SelectedIndices.Count > 0);
                this.HistoryHashList.Items.Insert(idx, hashStr);
                this.HistoryHashList.SelectedIndex = idx;
            }
            else
            {
                this.AddHashToHistory(hashStr, false);
                do
                {
                    this.HistoryHashList.SelectedIndices.Clear();
                } while (this.HistoryHashList.SelectedIndices.Count > 0);
                this.HistoryHashList.SelectedIndex = this.HistoryHashList.Items.IndexOf(hashStr);
            }

            ChangeMode(false);
        }

        private void PermCancel_Button_Click(object sender, EventArgs e)
        {
            if (this.HistoryHashList.Items.Count > 0 && this.HistoryHashList.SelectedIndices.Count > 0)
                this.UseHashText.Text = this.HistoryHashList.Items[this.HistoryHashList.SelectedIndices[0]].ToString();
            else
                this.UseHashText.Text = "";

            ChangeMode(false);
        }

        private void HistoryHashList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                this.DeleteButton_Click(this.DeleteButton, EventArgs.Empty);
            else if (e.KeyCode == Keys.Insert)
                this.AddButton_Click(this.AddButton, EventArgs.Empty);
        }

        private bool AdjustHashtags(ref string hashtag, bool isShowWarn)
        {
            // ハッシュタグの整形
            hashtag = hashtag.Trim();
            if (MyCommon.IsNullOrEmpty(hashtag))
            {
                if (isShowWarn) MessageBox.Show("emply hashtag.", "Hashtag warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            hashtag = hashtag.Replace("＃", "#");
            hashtag = hashtag.Replace("　", " ");
            var adjust = "";
            foreach (var hash in hashtag.Split(' '))
            {
                if (hash.Length > 0)
                {
                    if (!hash.StartsWith("#", StringComparison.Ordinal))
                    {
                        if (isShowWarn) MessageBox.Show("Invalid hashtag. -> " + hash, "Hashtag warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return false;
                    }
                    if (hash.Length == 1)
                    {
                        if (isShowWarn) MessageBox.Show("empty hashtag.", "Hashtag warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return false;
                    }
                    // 使用不可の文字チェックはしない
                    adjust += hash + " ";
                }
            }
            hashtag = adjust.Trim();
            return true;
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            var hash = "";
            foreach (string hs in this.HistoryHashList.SelectedItems)
            {
                hash += hs + " ";
            }
            hash = hash.Trim();
            if (!MyCommon.IsNullOrEmpty(hash))
            {
                this.AddHashToHistory(hash, true);
                this.IsPermanent = this.CheckPermanent.Checked;
            }
            else
            {
                // 使用ハッシュが未選択ならば、固定オプション外す
                this.IsPermanent = false;
            }
            this.IsHead = this.RadioHead.Checked;
            this.UseHash = hash;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void HashtagManage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (this.GroupDetail.Enabled)
                    this.PermOK_Button_Click(this.PermOK_Button, EventArgs.Empty);
                else
                    this.OK_Button_Click(this.OK_Button, EventArgs.Empty);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                if (this.GroupDetail.Enabled)
                    this.PermCancel_Button_Click(this.PermCancel_Button, EventArgs.Empty);
                else
                    this.Cancel_Button_Click(this.Cancel_Button, EventArgs.Empty);
            }
        }

        private void CheckNotAddToAtReply_CheckedChanged(object sender, EventArgs e)
            => this.IsNotAddToAtReply = CheckNotAddToAtReply.Checked;
    }
}
