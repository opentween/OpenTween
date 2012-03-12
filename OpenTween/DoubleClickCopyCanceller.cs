// OpenTween - Client of Twitter
// Copyright (c) 2007-2010 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2010 Moz (@syo68k) <http://iddy.jp/profile/moz/>
//           (c) 2008-2010 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
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
using System.Windows.Forms;

namespace OpenTween
{
    public class DoubleClickCopyCanceller : NativeWindow, IDisposable
    {
        const int WM_GETTEXTLENGTH = 0xE;
        const int WM_GETTEXT = 0xD;
        const int WM_LBUTTONDBLCLK = 0x203;
        bool _doubleClick = false;

        public DoubleClickCopyCanceller(Control control)
        {
            this.AssignHandle(control.Handle);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDBLCLK)
            {
                _doubleClick = true;
            }
            if (_doubleClick)
            {
                if (m.Msg == WM_GETTEXTLENGTH)
                {
                    _doubleClick = false;
                    m.Result = IntPtr.Zero;
                    return;
                }
            }
            base.WndProc(ref m);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ReleaseHandle();
            }
        }
    }
}
