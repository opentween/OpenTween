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
// with this program. if not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using OpenTween.Connection;

namespace OpenTween
{
    public static class Win32Api
    {
        // 指定されたウィンドウへ、指定されたメッセージを送信します
        [DllImport("user32.dll")]
        private extern static IntPtr SendMessage(
            IntPtr hwnd,
            SendMessageType wMsg,
            IntPtr wParam,
            IntPtr lParam);

        // SendMessageで送信するメッセージ
        private enum SendMessageType : uint
        {
            WM_SETREDRAW = 0x000B,               //再描画を許可するかを設定
            WM_USER = 0x400,                     //ユーザー定義メッセージ

            TCM_FIRST = 0x1300,                  //タブコントロールメッセージ
            TCM_SETMINTABWIDTH = TCM_FIRST + 49, //タブアイテムの最小幅を設定

            LVM_FIRST = 0x1000,                    //リストビューメッセージ
            LVM_GETSELECTIONMARK = LVM_FIRST + 66, //複数選択時の起点になるアイテムの位置を取得
            LVM_SETSELECTIONMARK = LVM_FIRST + 67, //複数選択時の起点になるアイテムを設定
        }

        /// <summary>
        /// コントロールの再描画を許可するかを設定します
        /// </summary>
        /// <param name="control">対象となるコントロール</param>
        /// <param name="redraw">再描画を許可する場合は true、抑制する場合は false</param>
        /// <returns>このメッセージを処理する場合、アプリケーションは 0 を返します</returns>
        public static int SetRedrawState(Control control, bool redraw)
        {
            var state = redraw ? new IntPtr(1) : IntPtr.Zero;
            return (int)SendMessage(control.Handle, SendMessageType.WM_SETREDRAW, state, IntPtr.Zero);
        }

        /// <summary>
        /// タブコントロールのアイテムの最小幅を設定します
        /// </summary>
        /// <param name="tabControl">対象となるタブコントロール</param>
        /// <param name="width">アイテムの最小幅。-1 を指定するとデフォルトの幅が使用されます</param>
        /// <returns>設定前の最小幅</returns>
        public static int SetMinTabWidth(TabControl tabControl, int width)
        {
            return (int)SendMessage(tabControl.Handle, SendMessageType.TCM_SETMINTABWIDTH, IntPtr.Zero, (IntPtr)width);
        }

        [DllImport("user32")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>
        /// 最小化状態のウィンドウを最小化する前の状態に復元します。
        /// </summary>
        /// <param name="window">復元するウィンドウ。</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="window"/> が null です。</exception>
        public static void RestoreWindow(IWin32Window window)
        {
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }

            ShowWindow(window.Handle, /* SW_RESTORE */ 9);
        }

        #region "画面ブリンク用"
        public static bool FlashMyWindow(IntPtr hwnd,
            FlashSpecification flashType,
            int flashCount)
        {
            var fInfo = new FLASHWINFO();
            fInfo.cbSize = Convert.ToInt32(Marshal.SizeOf(fInfo));
            fInfo.hwnd = hwnd;
            fInfo.dwFlags = (int)FlashSpecification.FlashAll;
            fInfo.uCount = flashCount;
            fInfo.dwTimeout = 0;

            return FlashWindowEx(ref fInfo);
        }

        public enum FlashSpecification : uint
        {
            FlashStop = FLASHW_STOP,
            FlashCaption = FLASHW_CAPTION,
            FlashTray = FLASHW_TRAY,
            FlashAll = FLASHW_ALL,
            FlashTimer = FLASHW_TIMER,
            FlashTimerNoForeground = FLASHW_TIMERNOFG,
        }
        /// http://www.atmarkit.co.jp/fdotnet/dotnettips/723flashwindow/flashwindow.html
        [DllImport("user32.dll")]
        private static extern bool FlashWindowEx(
            ref FLASHWINFO FWInfo);


        private struct FLASHWINFO
        {
            public Int32 cbSize;    // FLASHWINFO構造体のサイズ
            public IntPtr hwnd;     // 点滅対象のウィンドウ・ハンドル
            public Int32 dwFlags;   // 以下の「FLASHW_XXX」のいずれか
            public Int32 uCount;    // 点滅する回数
            public Int32 dwTimeout; // 点滅する間隔（ミリ秒単位）
        }

        // 点滅を止める
        private const Int32 FLASHW_STOP = 0;
        // タイトルバーを点滅させる
        private const Int32 FLASHW_CAPTION = 0x1;
        // タスクバー・ボタンを点滅させる
        private const Int32 FLASHW_TRAY = 0x2;
        // タスクバー・ボタンとタイトルバーを点滅させる
        private const Int32 FLASHW_ALL = 0x3;
        // FLASHW_STOPが指定されるまでずっと点滅させる
        private const Int32 FLASHW_TIMER = 0x4;
        // ウィンドウが最前面に来るまでずっと点滅させる
        private const Int32 FLASHW_TIMERNOFG = 0xC;
        #endregion

        [DllImport("user32.dll")]
        public static extern bool ValidateRect(
            IntPtr hwnd,
            IntPtr rect);

        #region "selection mark"
        // 複数選択時の起点になるアイテム (selection mark) の位置を取得する
        public static int ListView_GetSelectionMark(IntPtr hwndLV)
        {
            return SendMessage(hwndLV, SendMessageType.LVM_GETSELECTIONMARK, IntPtr.Zero, IntPtr.Zero).ToInt32();
        }

        // 複数選択時の起点になるアイテム (selection mark) を設定する
        public static void ListView_SetSelectionMark(IntPtr hwndLV, int itemIndex)
        {
            SendMessage(hwndLV, SendMessageType.LVM_SETSELECTIONMARK, IntPtr.Zero, (IntPtr)itemIndex);
        }
        #endregion

        #region "スクリーンセーバー起動中か判定"
        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(
                    int intAction,
                    int intParam,
                    ref bool bParam,
                    int intWinIniFlag);
        // returns non-zero value if function succeeds

        //スクリーンセーバーが起動中かを取得する定数
        private const int SPI_GETSCREENSAVERRUNNING = 0x0072;

        public static bool IsScreenSaverRunning()
        {
            var isRunning = false;
            SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, ref isRunning, 0);
            return isRunning;
        }
        #endregion

        #region "グローバルフック"
        [DllImport("user32")]
        private static extern int RegisterHotKey(IntPtr hwnd, int id,
            int fsModifiers, int vk);
        [DllImport("user32")]
        private static extern int UnregisterHotKey(IntPtr hwnd, int id);
        [DllImport("kernel32", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private static extern ushort GlobalAddAtom([MarshalAs(UnmanagedType.LPTStr)] string lpString);
        [DllImport("kernel32")]
        private static extern ushort GlobalDeleteAtom(ushort nAtom);

        private static int registerCount = 0;
        // register a global hot key
        public static int RegisterGlobalHotKey(int hotkeyValue, int modifiers, Form targetForm)
        {
            ushort hotkeyID = 0;
            try
            {
                // use the GlobalAddAtom API to get a unique ID (as suggested by MSDN docs)
                registerCount++;
                var atomName = Thread.CurrentThread.ManagedThreadId.ToString("X8") + targetForm.Name + registerCount.ToString();
                hotkeyID = GlobalAddAtom(atomName);
                if (hotkeyID == 0)
                {
                    throw new Exception("Unable to generate unique hotkey ID. Error code: " +
                       Marshal.GetLastWin32Error().ToString());
                }

                // register the hotkey, throw if any error
                if (RegisterHotKey(targetForm.Handle, hotkeyID, modifiers, hotkeyValue) == 0)
                {
                    throw new Exception("Unable to register hotkey. Error code: " +
                       Marshal.GetLastWin32Error().ToString());
                }
                return hotkeyID;
            }
            catch (Exception)
            {
                // clean up if hotkey registration failed
                UnregisterGlobalHotKey(hotkeyID, targetForm);
                return 0;
            }
        }

        // unregister a global hotkey
        public static void UnregisterGlobalHotKey(ushort hotkeyID, Form targetForm)
        {
            if (hotkeyID != 0)
            {
                UnregisterHotKey(targetForm.Handle, hotkeyID);
                // clean up the atom list
                GlobalDeleteAtom(hotkeyID);
                hotkeyID = 0;
            }
        }
        #endregion

        #region "プロセスのProxy設定"
        [DllImport("wininet.dll", SetLastError =true)]
        private static extern bool InternetSetOption(IntPtr hInternet,
                                                     int dwOption,
                                                     IntPtr lpBuffer,
                                                     int lpdwBufferLength);

        private struct INTERNET_PROXY_INFO : IDisposable
        {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;

            public void Dispose()
            {
                Dispose(true);
            }

            private void Dispose(bool disposing)
            {
                if (proxy != IntPtr.Zero) Marshal.FreeHGlobal(proxy);
                if (proxyBypass != IntPtr.Zero) Marshal.FreeHGlobal(proxyBypass);
            }
        }

        private static void RefreshProxySettings(string strProxy)
        {
            const int INTERNET_OPTION_PROXY = 38;
            //const int INTERNET_OPEN_TYPE_PRECONFIG = 0;   //IE setting
            const int INTERNET_OPEN_TYPE_DIRECT = 1;      //Direct
            const int INTERNET_OPEN_TYPE_PROXY = 3;       //Custom

            INTERNET_PROXY_INFO ipi;

            // Filling in structure
            if (!string.IsNullOrEmpty(strProxy))
            {
                ipi.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
                ipi.proxy = Marshal.StringToHGlobalAnsi(strProxy);
                ipi.proxyBypass = Marshal.StringToHGlobalAnsi("local");
            }
            else if (strProxy == null)
            {
                //IE Default
                var p = WebRequest.GetSystemWebProxy();
                if (p.IsBypassed(new Uri("http://www.google.com/")))
                {
                    ipi.dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
                    ipi.proxy = IntPtr.Zero;
                    ipi.proxyBypass = IntPtr.Zero;
                }
                else
                {
                    ipi.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
                    ipi.proxy = Marshal.StringToHGlobalAnsi(p.GetProxy(new Uri("http://www.google.com/")).Authority);
                    ipi.proxyBypass = Marshal.StringToHGlobalAnsi("local");
                }
            }
            else
            {
                ipi.dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
                ipi.proxy = IntPtr.Zero;
                ipi.proxyBypass = IntPtr.Zero;
            }

            try
            {
                // Allocating memory
                var pIpi = Marshal.AllocCoTaskMem(Marshal.SizeOf(ipi));
                if (pIpi.Equals(IntPtr.Zero)) return;
                try
                {
                    // Converting structure to IntPtr
                    Marshal.StructureToPtr(ipi, pIpi, true);
                    var ret = InternetSetOption(IntPtr.Zero,
                                                           INTERNET_OPTION_PROXY,
                                                           pIpi,
                                                           Marshal.SizeOf(ipi));
                }
                finally
                {
                    Marshal.FreeCoTaskMem(pIpi);
                }
            }
            finally
            {
                ipi.Dispose();
            }
        }

        private static void RefreshProxyAccount(string username, string password)
        {
            const int INTERNET_OPTION_PROXY_USERNAME = 43;
            const int INTERNET_OPTION_PROXY_PASSWORD = 44;

            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
            {
                var ret = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY_USERNAME, IntPtr.Zero, 0);
                ret = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY_PASSWORD, IntPtr.Zero, 0);
            }
            else
            {
                var pUser = Marshal.StringToBSTR(username);
                var pPass = Marshal.StringToBSTR(password);
                try
                {
                    var ret = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY_USERNAME, pUser, username.Length + 1);
                    ret = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY_PASSWORD, pPass, password.Length + 1);
                }
                finally
                {
                    Marshal.FreeBSTR(pUser);
                    Marshal.FreeBSTR(pPass);
                }
            }
        }

        public static void SetProxy(ProxyType pType, string host, int port, string username, string password)
        {
            string proxy = null;
            switch (pType)
            {
            case ProxyType.IE:
                proxy = null;
                break;
            case ProxyType.None:
                proxy = "";
                break;
            case ProxyType.Specified:
                proxy = host + (port > 0 ? ":" + port.ToString() : "");
                break;
            }
            RefreshProxySettings(proxy);
            RefreshProxyAccount(username, password);
        }
#endregion
    }
}
