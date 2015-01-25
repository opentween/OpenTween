// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.ComponentModel;

namespace OpenTween
{
    /// <summary>
    /// WinformsコントロールのBegin/EndUpdate等のメソッドをusingブロックによって呼び出すためのクラス
    /// </summary>
    public static class ControlTransaction
    {
        public static IDisposable Update(ListView control)
        {
            return new Transaction<ListView>(control, x => x.BeginUpdate(), x => x.EndUpdate());
        }

        public static IDisposable Update(ListBox control)
        {
            return new Transaction<ListBox>(control, x => x.BeginUpdate(), x => x.EndUpdate());
        }

        public static IDisposable Update(ComboBox control)
        {
            return new Transaction<ComboBox>(control, x => x.BeginUpdate(), x => x.EndUpdate());
        }

        public static IDisposable Update(TreeView control)
        {
            return new Transaction<TreeView>(control, x => x.BeginUpdate(), x => x.EndUpdate());
        }

        public static IDisposable Update(Control control)
        {
            // Begin/EndUpdate メソッドを持たないコントロールに対しては、
            // WM_SETREDRAW メッセージを直接コントロールに送信する。
            return new Transaction<Control>(control,
                x => NativeMethods.SetRedrawState(x, false),
                x => { NativeMethods.SetRedrawState(x, true); x.Invalidate(true); });
        }

        public static IDisposable Layout(Control control)
        {
            return Layout(control, performLayout: true);
        }

        public static IDisposable Layout(Control control, bool performLayout)
        {
            return new Transaction<Control>(control, x => x.SuspendLayout(), x => x.ResumeLayout(performLayout));
        }

        public static IDisposable Init(ISupportInitialize control)
        {
            return new Transaction<ISupportInitialize>(control, x => x.BeginInit(), x => x.EndInit());
        }

        public static IDisposable Cursor(Control control, Cursor newCursor)
        {
            var oldCursor = control.Cursor;

            return new Transaction<Control>(control, x => x.Cursor = newCursor, x => x.Cursor = oldCursor);
        }

        public static IDisposable Enabled(Control control)
        {
            var oldState = control.Enabled;

            return new Transaction<Control>(control, x => x.Enabled = true, x => x.Enabled = oldState);
        }

        public static IDisposable Disabled(Control control)
        {
            var oldState = control.Enabled;

            return new Transaction<Control>(control, x => x.Enabled = false, x => x.Enabled = oldState);
        }

        private class Transaction<T> : IDisposable
        {
            private readonly T control;

            private readonly Action<T> beginTransaction;
            private readonly Action<T> endTransaction;

            internal Transaction(T control, Action<T> beginTrans, Action<T> endTrans)
            {
                this.control = control;

                this.beginTransaction = beginTrans;
                this.endTransaction = endTrans;

                this.beginTransaction(this.control);
            }

            public void Dispose()
            {
                this.endTransaction(this.control);
            }
        }
    }
}
