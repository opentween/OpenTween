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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace OpenTween.OpenTweenCustomControl
{
    public sealed class DetailsListView : ListView
    {
        private Rectangle changeBounds;

        public ContextMenuStrip ColumnHeaderContextMenuStrip { get; set; }

        public event EventHandler VScrolled;
        public event EventHandler HScrolled;

        public DetailsListView()
        {
            View = View.Details;
            FullRowSelect = true;
            HideSelection = false;
            DoubleBuffered = true;
        }

        //[System.ComponentModel.DefaultValue(0),
        // System.ComponentModel.RefreshProperties(System.ComponentModel.RefreshProperties.Repaint)]
        //public new int VirtualListSize
        //{
        //    get { return base.VirtualListSize; }
        //    set
        //    {
        //        if (value == base.VirtualListSize) return;
        //        if (base.VirtualListSize > 0 && value > 0)
        //        {
        //            int topIndex = 0;
        //            if (!this.IsDisposed)
        //            {
        //                if (base.VirtualListSize < value)
        //                {
        //                    if (this.TopItem == null)
        //                    {
        //                        topIndex = 0;
        //                    }
        //                    else
        //                    {
        //                        topIndex = this.TopItem.Index;
        //                    }
        //                    topIndex = Math.Min(topIndex, Math.Abs(value - 1));
        //                    this.TopItem = this.Items[topIndex];
        //                }
        //                else
        //                {
        //                    if (this.TopItem == null)
        //                    {
        //                        topIndex = 0;
        //                    }
        //                    else
        //                    {
        //
        //                    }
        //                    this.TopItem = this.Items[0];
        //                }
        //            }
        //        }
        //        base.VirtualListSize = value;
        //    }
        //}

        /// <summary>
        /// 複数選択時の起点になるアイテム (selection mark) の位置を取得・設定する
        /// </summary>
        /// <remarks>
        /// Items[idx].Selected の設定では mark が設定されるが、SelectedIndices.Add(idx) では設定されないため、
        /// 主に後者と合わせて使用する
        /// </remarks>
        public int SelectionMark
        {
            get { return NativeMethods.ListView_GetSelectionMark(this.Handle); }
            set { NativeMethods.ListView_SetSelectionMark(this.Handle, value); }
        }

        public void SelectItems(int[] indices)
        {
            foreach (var index in indices)
            {
                if (index < 0 || index >= this.VirtualListSize)
                    throw new ArgumentOutOfRangeException("indices");

                NativeMethods.SelectItem(this, index);
            }

            this.OnSelectedIndexChanged(EventArgs.Empty);
        }

        public void SelectAllItems()
        {
            NativeMethods.SelectAllItems(this);

            this.OnSelectedIndexChanged(EventArgs.Empty);
        }

        public void ChangeItemBackColor(int index, Color backColor)
        {
            ChangeSubItemBackColor(index, 0, backColor);
        }

        public void ChangeItemForeColor(int index, Color foreColor)
        {
            ChangeSubItemForeColor(index, 0, foreColor);
        }

        public void ChangeItemFont(int index, Font fnt)
        {
            ChangeSubItemFont(index, 0, fnt);
        }

        public void ChangeItemFontAndColor(int index, Color foreColor, Font fnt)
        {
            ChangeSubItemStyles(index, 0, BackColor, foreColor, fnt);
        }

        public void ChangeItemStyles(int index, Color backColor, Color foreColor, Font fnt)
        {
            ChangeSubItemStyles(index, 0, backColor, foreColor, fnt);
        }

        public void ChangeSubItemBackColor(int itemIndex, int subitemIndex, Color backColor)
        {
            var item = this.Items[itemIndex];
            item.SubItems[subitemIndex].BackColor = backColor;
            SetUpdateBounds(item, subitemIndex);
            this.Update();
            this.changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemForeColor(int itemIndex, int subitemIndex, Color foreColor)
        {
            var item = this.Items[itemIndex];
            item.SubItems[subitemIndex].ForeColor = foreColor;
            SetUpdateBounds(item, subitemIndex);
            this.Update();
            this.changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemFont(int itemIndex, int subitemIndex, Font fnt)
        {
            var item = this.Items[itemIndex];
            item.SubItems[subitemIndex].Font = fnt;
            SetUpdateBounds(item, subitemIndex);
            this.Update();
            this.changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemFontAndColor(int itemIndex, int subitemIndex, Color foreColor, Font fnt)
        {
            var item = this.Items[itemIndex];
            var subItem = item.SubItems[subitemIndex];
            subItem.ForeColor = foreColor;
            subItem.Font = fnt;
            SetUpdateBounds(item, subitemIndex);
            this.Update();
            this.changeBounds = Rectangle.Empty;
        }

        public void ChangeSubItemStyles(int itemIndex, int subitemIndex, Color backColor, Color foreColor, Font fnt)
        {
            var item = this.Items[itemIndex];
            var subItem = item.SubItems[subitemIndex];
            subItem.BackColor = backColor;
            subItem.ForeColor = foreColor;
            subItem.Font = fnt;
            SetUpdateBounds(item, subitemIndex);
            this.Update();
            this.changeBounds = Rectangle.Empty;
        }

        private void SetUpdateBounds(ListViewItem item, int subItemIndex)
        {
            try
            {
                if (subItemIndex > this.Columns.Count)
                {
                    throw new ArgumentOutOfRangeException("subItemIndex");
                }
                if (item.UseItemStyleForSubItems)
                {
                    this.changeBounds = item.Bounds;
                }
                else
                {
                    this.changeBounds = this.GetSubItemBounds(item, subItemIndex);
                }
            }
            catch (ArgumentException)
            {
                //タイミングによりBoundsプロパティが取れない？
                this.changeBounds = Rectangle.Empty;
            }
        }

        private Rectangle GetSubItemBounds(ListViewItem item, int subitemIndex)
        {
            if (subitemIndex == 0 && this.Columns.Count > 0)
            {
                Rectangle col0 = item.Bounds;
                return new Rectangle(col0.Left, col0.Top, item.SubItems[1].Bounds.X + 1, col0.Height);
            }
            else
            {
                return item.SubItems[subitemIndex].Bounds;
            }
        }

        [DebuggerStepThrough()]
        protected override void WndProc(ref Message m)
        {
            const int WM_ERASEBKGND = 0x14;
            const int WM_PAINT = 0xF;
            const int WM_MOUSEWHEEL = 0x20A;
            const int WM_MOUSEHWHEEL = 0x20E;
            const int WM_HSCROLL = 0x114;
            const int WM_VSCROLL = 0x115;
            const int WM_KEYDOWN = 0x100;
            const int WM_CONTEXTMENU = 0x7B;
            const int LVM_SETITEMCOUNT = 0x102F;
            const long LVSICF_NOSCROLL = 0x2;
            const long LVSICF_NOINVALIDATEALL = 0x1;

            int hPos = -1;
            int vPos = -1;

            switch (m.Msg)
            {
                case WM_ERASEBKGND:
                    if (this.changeBounds != Rectangle.Empty)
                        m.Msg = 0;
                    break;
                case WM_PAINT:
                    if (this.changeBounds != Rectangle.Empty)
                    {
                        NativeMethods.ValidateRect(this.Handle, IntPtr.Zero);
                        this.Invalidate(this.changeBounds);
                        this.changeBounds = Rectangle.Empty;
                    }
                    break;
                case WM_HSCROLL:
                    if (HScrolled != null)
                        HScrolled(this, EventArgs.Empty);
                    break;
                case WM_VSCROLL:
                    if (VScrolled != null)
                        VScrolled(this, EventArgs.Empty);
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
                        //カラムヘッダメニューを表示
                        if (this.ColumnHeaderContextMenuStrip != null)
                            this.ColumnHeaderContextMenuStrip.Show(new Point(m.LParam.ToInt32()));
                        return;
                    }
                    break;
                case LVM_SETITEMCOUNT:
                    m.LParam = new IntPtr(LVSICF_NOSCROLL | LVSICF_NOINVALIDATEALL);
                    break;
            }

            try
            {
                base.WndProc(ref m);
            }
            catch (ArgumentOutOfRangeException)
            {
                //Substringでlengthが0以下。アイコンサイズが影響？
            }
            catch (AccessViolationException)
            {
                //WndProcのさらに先で発生する。
            }
            if (this.IsDisposed) return;

            if (vPos != -1)
                if (vPos != NativeMethods.GetScrollPosition(this, NativeMethods.ScrollBarDirection.SB_VERT))
                    if (VScrolled != null)
                        VScrolled(this, EventArgs.Empty);
            if (hPos != -1)
                if (hPos != NativeMethods.GetScrollPosition(this, NativeMethods.ScrollBarDirection.SB_HORZ))
                    if (HScrolled != null)
                        HScrolled(this, EventArgs.Empty);
        }
   }
}
