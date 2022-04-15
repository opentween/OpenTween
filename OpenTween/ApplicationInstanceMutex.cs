// OpenTween - Client of Twitter
// Copyright (c) 2007-2012 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2012 Moz (@syo68k)
//           (c) 2008-2012 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2012 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2012 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2012      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
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
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace OpenTween
{
    /// <summary>
    /// アプリケーションの多重起動を抑制するためのクラス
    /// </summary>
    public sealed class ApplicationInstanceMutex : IDisposable
    {
        public bool IsDisposed { get; private set; } = false;

        private readonly Mutex mutex;
        private readonly bool createdNew;

        public ApplicationInstanceMutex(string appName, string settingPath)
        {
            var name = $"{settingPath.Replace(@"\", "/")}/{appName}";
            this.mutex = new Mutex(initiallyOwned: true, name, out this.createdNew);
        }

        /// <summary>
        /// 他のインスタンスが既に起動している場合は true、それ以外は false
        /// </summary>
        public bool InstanceExists
            => !this.createdNew;

        public void TryActivatePreviousInstance()
        {
            // 実行中の同じアプリケーションのウィンドウ・ハンドルの取得
            var prevProcess = this.GetPreviousProcess();
            if (prevProcess == null)
                return;

            var windowHandle = NativeMethods.GetWindowHandle((uint)prevProcess.Id, ApplicationSettings.ApplicationName);
            if (windowHandle != IntPtr.Zero)
                NativeMethods.SetActiveWindow(windowHandle);
        }

        private Process? GetPreviousProcess()
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();

                return Process.GetProcessesByName(currentProcess.ProcessName)
                    .Where(p => p.Id != currentProcess.Id)
                    .FirstOrDefault(p => p.MainModule.FileName.Equals(currentProcess.MainModule.FileName, StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return null;
            }
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;

            this.IsDisposed = true;

            if (this.createdNew)
                this.mutex.ReleaseMutex();

            this.mutex.Dispose();
        }
    }
}
