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
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace OpenTween.OpenTweenCustomControl
{
    public sealed class DetailsListView : ListView
    {
        private (int Start, int End)? redrawRange = null;

        [DefaultValue(null)]
        public ContextMenuStrip? ColumnHeaderContextMenuStrip { get; set; }

        public event EventHandler? VScrolled;

        public event EventHandler? HScrolled;

        public DetailsListView()
        {
            this.View = View.Details;
            this.FullRowSelect = true;
            this.HideSelection = false;
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// 複数選択時の起点になるアイテム (selection mark) の位置を取得・設定する
        /// </summary>
        /// <remarks>
        /// Items[idx].Selected の設定では mark が設定されるが、SelectedIndices.Add(idx) では設定されないため、
        /// 主に後者と合わせて使用する
        /// </remarks>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionMark
        {
            get => NativeMethods.ListView_GetSelectionMark(this.Handle);
            set => NativeMethods.ListView_SetSelectionMark(this.Handle, value);
        }

        public void SelectItems(int[] indices)
        {
            var listSize = this.VirtualListSize;
            if (indices.Any(x => x < 0 || x >= listSize))
                throw new ArgumentOutOfRangeException(nameof(indices));

            if (indices.Length == 0)
            {
                this.SelectedIndices.Clear();
            }
            else if (indices.Length == 1)
            {
                this.SelectedIndices.Clear();
                this.SelectedIndices.Add(indices[0]);
            }
            else
            {
                var currentSelectedIndices = this.SelectedIndices.Cast<int>().ToArray();
                var selectIndices = indices.Except(currentSelectedIndices).ToArray();
                var deselectIndices = currentSelectedIndices.Except(indices).ToArray();

                if (selectIndices.Length + deselectIndices.Length > currentSelectedIndices.Length)
                {
                    // Clearして選択し直した方が早い場合
                    this.SelectedIndices.Clear();
                    selectIndices = indices;
                    deselectIndices = Array.Empty<int>();
                }

                foreach (var index in selectIndices)
                    NativeMethods.SelectItem(this, index, selected: true);

                foreach (var index in deselectIndices)
                    NativeMethods.SelectItem(this, index, selected: false);
            }
        }

        public void SelectAllItems()
        {
            NativeMethods.SelectAllItems(this);

            this.OnSelectedIndexChanged(EventArgs.Empty);
        }

        public void RefreshItem(int index)
        {
            this.ValidateAll();
            this.RefreshItemsRange(index, index);
        }

        public void RefreshItems(IEnumerable<int> indices)
        {
            var chunks = MyCommon.ToRangeChunk(indices);
            this.ValidateAll();

            foreach (var (start, end) in chunks)
                this.RefreshItemsRange(start, end);
        }

        private void RefreshItemsRange(int start, int end)
        {
            try
            {
                this.redrawRange = (start, end);
                this.RedrawItems(start, end, invalidateOnly: false);
            }
            finally
            {
                this.redrawRange = null;
            }
        }

        /// <summary>領域を全て有効化する（再描画が必要な領域から除外する）</summary>
        private void ValidateAll()
            => NativeMethods.ValidateRect(this.Handle, IntPtr.Zero);

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (this.redrawRange is (int start, int end))
            {
                var index = e.ItemIndex;
                if (index < start || index > end)
                    return;
            }

            base.OnDrawItem(e);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NMHDR
        {
            public IntPtr HwndFrom;
            public IntPtr IdFrom;
            public int Code;
        }

        [DebuggerStepThrough]
        protected override void WndProc(ref Message m)
        {
            const int WM_ERASEBKGND = 0x14;
            const int WM_MOUSEWHEEL = 0x20A;
            const int WM_MOUSEHWHEEL = 0x20E;
            const int WM_HSCROLL = 0x114;
            const int WM_VSCROLL = 0x115;
            const int WM_KEYDOWN = 0x100;
            const int WM_USER = 0x400;
            const int WM_REFLECT = WM_USER + 0x1C00;
            const int WM_NOTIFY = 0x004E;
            const int WM_CONTEXTMENU = 0x7B;
            const int LVM_SETITEMCOUNT = 0x102F;
            const int LVN_ODSTATECHANGED = 0 - 100 - 15;
            const long LVSICF_NOSCROLL = 0x2;
            const long LVSICF_NOINVALIDATEALL = 0x1;

            var hPos = -1;
            var vPos = -1;

            switch (m.Msg)
            {
                case WM_ERASEBKGND:
                    if (this.redrawRange != null)
                        m.Msg = 0;
                    break;
                case WM_HSCROLL:
                    this.HScrolled?.Invoke(this, EventArgs.Empty);
                    break;
                case WM_VSCROLL:
                    this.VScrolled?.Invoke(this, EventArgs.Empty);
                    break;
                case WM_MOUSEWHEEL:
                case WM_MOUSEHWHEEL:
                case WM_KEYDOWN:
                    vPos = NativeMethods.GetScrollPosition(this, NativeMethods.ScrollBarDirection.SB_VERT);
                    hPos = NativeMethods.GetScrollPosition(this, NativeMethods.ScrollBarDirection.SB_HORZ);
                    break;
                case WM_CONTEXTMENU:
                    if (m.WParam != this.Handle)
                    {
                        // カラムヘッダメニューを表示
                        this.ColumnHeaderContextMenuStrip?.Show(new Point(m.LParam.ToInt32()));
                        return;
                    }
                    break;
                case LVM_SETITEMCOUNT:
                    m.LParam = new IntPtr(LVSICF_NOSCROLL | LVSICF_NOINVALIDATEALL);
                    break;
                case WM_REFLECT + WM_NOTIFY:
                    var nmhdr = Marshal.PtrToStructure<NMHDR>(m.LParam);

                    // Ctrl+クリックで選択状態を変更した場合にイベントが発生しない問題への対処
                    if (nmhdr.Code == LVN_ODSTATECHANGED)
                        this.OnSelectedIndexChanged(EventArgs.Empty);
                    break;
            }

            try
            {
                base.WndProc(ref m);
            }
            catch (ArgumentOutOfRangeException)
            {
                // Substringでlengthが0以下。アイコンサイズが影響？
            }
            catch (AccessViolationException)
            {
                // WndProcのさらに先で発生する。
            }
            if (this.IsDisposed) return;

            if (vPos != -1)
            {
                if (vPos != NativeMethods.GetScrollPosition(this, NativeMethods.ScrollBarDirection.SB_VERT))
                    this.VScrolled?.Invoke(this, EventArgs.Empty);
            }

            if (hPos != -1)
            {
                if (hPos != NativeMethods.GetScrollPosition(this, NativeMethods.ScrollBarDirection.SB_HORZ))
                    this.HScrolled?.Invoke(this, EventArgs.Empty);
            }
        }
   }
}
