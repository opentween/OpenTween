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
        private EventHandlerList _handlers = new EventHandlerList();

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
            this.Items[itemIndex].SubItems[subitemIndex].BackColor = backColor;
        }

        public void ChangeSubItemForeColor(int itemIndex, int subitemIndex, Color foreColor)
        {
            this.Items[itemIndex].SubItems[subitemIndex].ForeColor = foreColor;
        }

        public void ChangeSubItemFont(int itemIndex, int subitemIndex, Font fnt)
        {
            this.Items[itemIndex].SubItems[subitemIndex].Font = fnt;
        }

        public void ChangeSubItemFontAndColor(int itemIndex, int subitemIndex, Color foreColor, Font fnt)
        {
            this.Items[itemIndex].SubItems[subitemIndex].ForeColor = foreColor;
            this.Items[itemIndex].SubItems[subitemIndex].Font = fnt;
        }

        public void ChangeSubItemStyles(int itemIndex, int subitemIndex, Color backColor, Color foreColor, Font fnt)
        {
            this.Items[itemIndex].SubItems[subitemIndex].BackColor = backColor;
            this.Items[itemIndex].SubItems[subitemIndex].ForeColor = foreColor;
            this.Items[itemIndex].SubItems[subitemIndex].Font = fnt;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SCROLLINFO
        {
            public int cbSize;
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        private enum ScrollBarDirection
        {
            SB_HORZ = 0,
            SB_VERT = 1,
            SB_CTL = 2,
            SB_BOTH = 3,
        }

        private enum ScrollInfoMask
        {
            SIF_RANGE = 0x1,
            SIF_PAGE = 0x2,
            SIF_POS = 0x4,
            SIF_DISABLENOSCROLL = 0x8,
            SIF_TRACKPOS = 0x10,
            SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS),
        }

        [DllImport("user32.dll")]
        private static extern int GetScrollInfo(IntPtr hWnd, ScrollBarDirection fnBar, ref SCROLLINFO lpsi);

        private SCROLLINFO si = new SCROLLINFO {
            cbSize = Marshal.SizeOf(new SCROLLINFO()),
            fMask = (int)ScrollInfoMask.SIF_POS
        };

        [DebuggerStepThrough()]
        protected override void WndProc(ref Message m)
        {
            const int WM_MOUSEWHEEL = 0x20A;
            const int WM_MOUSEHWHEEL = 0x20E;
            const int WM_HSCROLL = 0x114;
            const int WM_VSCROLL = 0x115;
            const int WM_KEYDOWN = 0x100;
            const int LVM_SETITEMCOUNT = 0x102F;
            const long LVSICF_NOSCROLL = 0x2;
            const long LVSICF_NOINVALIDATEALL = 0x1;

            int hPos = -1;
            int vPos = -1;

            switch (m.Msg)
            {
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
                    if (GetScrollInfo(this.Handle, ScrollBarDirection.SB_VERT, ref si) != 0)
                        vPos = si.nPos;
                    if (GetScrollInfo(this.Handle, ScrollBarDirection.SB_HORZ, ref si) != 0)
                        hPos = si.nPos;
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
                if (GetScrollInfo(this.Handle, ScrollBarDirection.SB_VERT, ref si) != 0 && vPos != si.nPos)
                    if (VScrolled != null)
                        VScrolled(this, EventArgs.Empty);
            if (hPos != -1)
                if (GetScrollInfo(this.Handle, ScrollBarDirection.SB_HORZ, ref si) != 0 && hPos != si.nPos)
                    if (HScrolled != null)
                        HScrolled(this, EventArgs.Empty);
        }
   }
}
