// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
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

#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenTween
{
    public class HookGlobalHotkey : NativeWindow, IDisposable
    {
        private readonly Form targetForm;

        private readonly record struct KeyEventValue(
            KeyEventArgs KeyEvent,
            int Value
        );

        private readonly Dictionary<int, KeyEventValue> hotkeyID;

        [Flags]
        public enum ModKeys
        {
            None = 0,
            Alt = 0x1,
            Ctrl = 0x2,
            Shift = 0x4,
            Win = 0x8,
        }

        public event KeyEventHandler? HotkeyPressed;

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x312;
            if (m.Msg == WM_HOTKEY)
            {
                if (this.hotkeyID.ContainsKey(m.WParam.ToInt32()))
                {
                    this.HotkeyPressed?.Invoke(this, this.hotkeyID[m.WParam.ToInt32()].KeyEvent);
                }
                return;
            }
            base.WndProc(ref m);
        }

        public HookGlobalHotkey(Form targetForm)
        {
            this.targetForm = targetForm;
            this.hotkeyID = new Dictionary<int, KeyEventValue>();

            this.targetForm.HandleCreated += this.OnHandleCreated;
            this.targetForm.HandleDestroyed += this.OnHandleDestroyed;
        }

        public void OnHandleCreated(object sender, EventArgs e)
            => this.AssignHandle(this.targetForm.Handle);

        public void OnHandleDestroyed(object sender, EventArgs e)
            => this.ReleaseHandle();

        public bool RegisterOriginalHotkey(Keys hotkey, int hotkeyValue, ModKeys modifiers)
        {
            var modKey = Keys.None;
            if ((modifiers & ModKeys.Alt) == ModKeys.Alt) modKey |= Keys.Alt;
            if ((modifiers & ModKeys.Ctrl) == ModKeys.Ctrl) modKey |= Keys.Control;
            if ((modifiers & ModKeys.Shift) == ModKeys.Shift) modKey |= Keys.Shift;
            if ((modifiers & ModKeys.Win) == ModKeys.Win) modKey |= Keys.LWin;
            var key = new KeyEventArgs(hotkey | modKey);
            foreach (var (_, value) in this.hotkeyID)
            {
                if (value.KeyEvent.KeyData == key.KeyData && value.Value == hotkeyValue) return true; // 登録済みなら正常終了
            }
            var hotkeyId = NativeMethods.RegisterGlobalHotKey(hotkeyValue, (int)modifiers, this.targetForm);
            if (hotkeyId > 0)
            {
                this.hotkeyID.Add(hotkeyId, new KeyEventValue(key, hotkeyValue));
                return true;
            }
            return false;
        }

        public void UnregisterAllOriginalHotkey()
        {
            foreach (ushort hotkeyId in this.hotkeyID.Keys)
            {
                NativeMethods.UnregisterGlobalHotKey(hotkeyId, this.targetForm);
            }
            this.hotkeyID.Clear();
        }

        private bool disposedValue = false;        // 重複する呼び出しを検出するには

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                }

                if (this.targetForm != null && !this.targetForm.IsDisposed)
                {
                    this.UnregisterAllOriginalHotkey();
                    this.targetForm.HandleCreated -= this.OnHandleCreated;
                    this.targetForm.HandleDestroyed -= this.OnHandleDestroyed;
                }
            }
            this.disposedValue = true;
        }

#region " IDisposable Support "
        // このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
#endregion

        ~HookGlobalHotkey()
            => this.Dispose(false);
    }
}
