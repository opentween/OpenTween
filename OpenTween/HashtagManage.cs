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

namespace OpenTween
{
    public partial class HashtagManage : Form
    {
        //入力補助画面
        private AtIdSupplement _hashSupl;
        //I/F用
        private string _useHash = "";
        private bool _isPermanent = false;
        private bool _isHead = false;
        private bool _isNotAddToAtReply = true;
        //編集モード
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
            if (MessageBox.Show(Properties.Resources.DeleteHashtagsMessage1,
                                "Delete Hashtags",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Question) == DialogResult.Cancel)
            {
                return;
            }
            for (int i = 0; i < HistoryHashList.SelectedIndices.Count; i++)
            {
                if (UseHashText.Text == HistoryHashList.SelectedItems[0].ToString()) UseHashText.Text = "";
                HistoryHashList.Items.RemoveAt(HistoryHashList.SelectedIndices[0]);
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
            if (string.IsNullOrEmpty(value)) return -1;

            int idx = 0;

            foreach (object l in list)
            {
                string src = l as string;
                if (string.IsNullOrEmpty(src))
                {
                    idx += 1;
                    continue;
                }
                if (string.Compare(src, value, true) == 0)
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
            if (!string.IsNullOrEmpty(hash))
            {
                if (isIgnorePermanent || !_isPermanent)
                {
                    //無条件に先頭に挿入
                    int idx = GetIndexOf(HistoryHashList.Items, hash);

                    if (idx != -1) HistoryHashList.Items.RemoveAt(idx);
                    HistoryHashList.Items.Insert(0, hash);
                }
                else
                {
                    //固定されていたら2行目に挿入
                    int idx = GetIndexOf(HistoryHashList.Items, hash);
                    if (_isPermanent)
                    {
                        if (idx > 0)
                        {
                            //重複アイテムが2行目以降にあれば2行目へ
                            HistoryHashList.Items.RemoveAt(idx);
                            HistoryHashList.Items.Insert(1, hash);
                        }
                        else if (idx == -1)
                        {
                            //重複アイテムなし
                            if (HistoryHashList.Items.Count == 0)
                            {
                                //リストが空なら追加
                                HistoryHashList.Items.Add(hash);
                            }
                            else
                            {
                                //リストにアイテムがあれば2行目へ
                                HistoryHashList.Items.Insert(1, hash);
                            }
                        }
                    }
                }
            }
        }

        private void HashtagManage_Shown(object sender, EventArgs e)
        {
            //オプション
            this.CheckPermanent.Checked = this._isPermanent;
            this.RadioHead.Checked = this._isHead;
            this.RadioLast.Checked = !this._isHead;
            //リスト選択
            if (this.HistoryHashList.Items.Contains(this._useHash))
            {
                this.HistoryHashList.SelectedItem = this._useHash;
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
            _useHash = permanentHash;
            _isPermanent = IsPermanent;
            _isHead = IsHead;
            _isNotAddToAtReply = IsNotAddToAtReply;
        }

        private void UseHashText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '#')
            {
                _hashSupl.ShowDialog();
                if (!string.IsNullOrEmpty(_hashSupl.inputText))
                {
                    string fHalf = "";
                    string eHalf = "";
                    int selStart = UseHashText.SelectionStart;
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
        {
            this.OK_Button_Click(null, null);
        }

        public void ToggleHash()
        {
            if (string.IsNullOrEmpty(this._useHash))
            {
                if (this.HistoryHashList.Items.Count > 0)
                    this._useHash = this.HistoryHashList.Items[0].ToString();
            }
            else
            {
                this._useHash = "";
            }
        }

        public List<string> HashHistories
        {
            get
            {
                List<string> hash = new List<string>();
                foreach (string item in HistoryHashList.Items)
                {
                    hash.Add(item);
                }
                return hash;
            }
        }

        public string UseHash
        {
            get { return _useHash; }
        }

        public void ClearHashtag()
        {
            this._useHash = "";
        }

        public void SetPermanentHash(string hash)
        {
            //固定ハッシュタグの変更
            _useHash = hash.Trim();
            this.AddHashToHistory(_useHash, false);
            this._isPermanent = true;
        }

        public bool IsPermanent
        {
            get { return _isPermanent; }
        }

        public bool IsHead
        {
            get { return _isHead; }
        }

        public bool IsNotAddToAtReply
        {
            get { return _isNotAddToAtReply; }
        }

        private void PermOK_Button_Click(object sender, EventArgs e)
        {
            //ハッシュタグの整形
            string hashStr = UseHashText.Text;
            if (!this.AdjustHashtags(ref hashStr, true)) return;

            UseHashText.Text = hashStr;
            int idx = 0;
            if (!this._isAdd && this.HistoryHashList.SelectedIndices.Count > 0)
            {
                idx = this.HistoryHashList.SelectedIndices[0];
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
                this.DeleteButton_Click(null, null);
            else if (e.KeyCode == Keys.Insert)
                this.AddButton_Click(null, null);
        }

        private bool AdjustHashtags(ref string hashtag, bool isShowWarn)
        {
            //ハッシュタグの整形
            hashtag = hashtag.Trim();
            if (string.IsNullOrEmpty(hashtag))
            {
                if (isShowWarn) MessageBox.Show("emply hashtag.", "Hashtag warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }
            hashtag = hashtag.Replace("＃", "#");
            hashtag = hashtag.Replace("　", " ");
            string adjust = "";
            foreach (string hash in hashtag.Split(' '))
            {
                if (hash.Length > 0)
                {
                    if (!hash.StartsWith("#"))
                    {
                        if (isShowWarn) MessageBox.Show("Invalid hashtag. -> " + hash, "Hashtag warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return false;
                    }
                    if (hash.Length == 1)
                    {
                        if (isShowWarn) MessageBox.Show("empty hashtag.", "Hashtag warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        return false;
                    }
                    //使用不可の文字チェックはしない
                    adjust += hash + " ";
                }
            }
            hashtag = adjust.Trim();
            return true;
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            string hash = "";
            foreach (string hs in this.HistoryHashList.SelectedItems)
            {
                hash += hs + " ";
            }
            hash = hash.Trim();
            if (!string.IsNullOrEmpty(hash))
            {
                this.AddHashToHistory(hash, true);
                this._isPermanent = this.CheckPermanent.Checked;
            }
            else
            {
                //使用ハッシュが未選択ならば、固定オプション外す
                this._isPermanent = false;
            }
            this._isHead = this.RadioHead.Checked;
            this._useHash = hash;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void HashtagManage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                if (this.GroupDetail.Enabled)
                    this.PermOK_Button_Click(null, null);
                else
                    this.OK_Button_Click(null, null);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                if (this.GroupDetail.Enabled)
                    this.PermCancel_Button_Click(null, null);
                else
                    this.Cancel_Button_Click(null, null);
            }
        }

        private void CheckNotAddToAtReply_CheckedChanged(object sender, EventArgs e)
        {
            _isNotAddToAtReply = CheckNotAddToAtReply.Checked;
        }
    }
}
