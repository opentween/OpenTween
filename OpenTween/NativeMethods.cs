// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
//           (c) 2014      kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using OpenTween.Connection;

namespace OpenTween
{
    internal static class NativeMethods
    {
        // 指定されたウィンドウへ、指定されたメッセージを送信します
        [DllImport("user32.dll")]
        private extern static IntPtr SendMessage(
            IntPtr hwnd,
            SendMessageType wMsg,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("user32.dll")]
        private extern static IntPtr SendMessage(
            IntPtr hwnd,
            SendMessageType wMsg,
            IntPtr wParam,
            ref LVITEM lParam);

        // SendMessageで送信するメッセージ
        private enum SendMessageType : uint
        {
            WM_SETREDRAW = 0x000B,               //再描画を許可するかを設定
            WM_USER = 0x400,                     //ユーザー定義メッセージ

            TCM_FIRST = 0x1300,                  //タブコントロールメッセージ
            TCM_SETMINTABWIDTH = TCM_FIRST + 49, //タブアイテムの最小幅を設定

            LVM_FIRST = 0x1000,                    //リストビューメッセージ
            LVM_SETITEMSTATE = LVM_FIRST + 43,     //アイテムの状態を設定
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

        // 参照: LVITEM structure (Windows)
        // http://msdn.microsoft.com/en-us/library/windows/desktop/bb774760%28v=vs.85%29.aspx
        [StructLayout(LayoutKind.Sequential)]
        [BestFitMapping(false, ThrowOnUnmappableChar = true)]
        private struct LVITEM
        {
            public uint mask;
            public int iItem;
            public int iSubItem;
            public LVIS state;
            public LVIS stateMask;
            public string pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public int iGroupId;
            public uint cColumns;
            public uint puColumns;
            public int piColFmt;
            public int iGroup;
        }

        // 参照: List-View Item States (Windows)
        // http://msdn.microsoft.com/en-us/library/windows/desktop/bb774733%28v=vs.85%29.aspx
        [Flags]
        private enum LVIS : uint
        {
            SELECTED = 0x02,
        }

        /// <summary>
        /// ListView のアイテムを選択された状態にします
        /// </summary>
        /// <param name="listView">対象となる ListView</param>
        /// <param name="index">選択するアイテムのインデックス</param>
        /// <returns>成功した場合は true、それ以外の場合は false</returns>
        public static bool SelectItem(ListView listView, int index)
        {
            // LVM_SETITEMSTATE では stateMask, state 以外のメンバーは無視される
            var lvitem = new LVITEM
            {
                stateMask = LVIS.SELECTED,
                state = LVIS.SELECTED,
            };

            var ret = (int)SendMessage(listView.Handle, SendMessageType.LVM_SETITEMSTATE, (IntPtr)index, ref lvitem);
            return ret != 0;
        }

        /// <summary>
        /// ListView の全アイテムを選択された状態にします
        /// </summary>
        /// <param name="listView">対象となる ListView</param>
        /// <returns>成功した場合は true、それ以外の場合は false</returns>
        public static bool SelectAllItems(ListView listView)
        {
            return SelectItem(listView, -1 /* all items */);
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
                    throw new Win32Exception();
                }

                // register the hotkey, throw if any error
                if (RegisterHotKey(targetForm.Handle, hotkeyID, modifiers, hotkeyValue) == 0)
                {
                    throw new Win32Exception();
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

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet,
            InternetOption dwOption,
            [In] ref InternetProxyInfo lpBuffer,
            int lpdwBufferLength);

        [DllImport("wininet.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private static extern bool InternetSetOption(IntPtr hInternet,
            InternetOption dwOption,
            string lpBuffer,
            int lpdwBufferLength);

        private enum InternetOption
        {
            PROXY = 38,
            PROXY_USERNAME = 43,
            PROXY_PASSWORD = 44,
        }

        [StructLayout(LayoutKind.Sequential)]
        [BestFitMapping(false, ThrowOnUnmappableChar = true)]
        private struct InternetProxyInfo
        {
            public InternetOpenType dwAccessType;
            public string proxy;
            public string proxyBypass;
        }

        private enum InternetOpenType
        {
            //PRECONFIG = 0, // IE setting
            DIRECT = 1, // Direct
            PROXY = 3, // Custom
        }

        private static void RefreshProxySettings(string strProxy)
        {
            InternetProxyInfo ipi;

            // Filling in structure
            if (!string.IsNullOrEmpty(strProxy))
            {
                ipi = new InternetProxyInfo
                {
                    dwAccessType = InternetOpenType.PROXY,
                    proxy = strProxy,
                    proxyBypass = "local",
                };
            }
            else if (strProxy == null)
            {
                //IE Default
                var p = WebRequest.GetSystemWebProxy();
                if (p.IsBypassed(new Uri("http://www.google.com/")))
                {
                    ipi = new InternetProxyInfo
                    {
                        dwAccessType = InternetOpenType.DIRECT,
                        proxy = null,
                        proxyBypass = null,
                    };
                }
                else
                {
                    ipi = new InternetProxyInfo
                    {
                        dwAccessType = InternetOpenType.PROXY,
                        proxy = p.GetProxy(new Uri("http://www.google.com/")).Authority,
                        proxyBypass = "local",
                    };
                }
            }
            else
            {
                ipi = new InternetProxyInfo
                {
                    dwAccessType = InternetOpenType.DIRECT,
                    proxy = null,
                    proxyBypass = null,
                };
            }

            if (!InternetSetOption(IntPtr.Zero, InternetOption.PROXY, ref ipi, Marshal.SizeOf(ipi)))
                throw new Win32Exception();
        }

        private static void RefreshProxyAccount(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
            {
                if (!InternetSetOption(IntPtr.Zero, InternetOption.PROXY_USERNAME, (string)null, 0))
                    throw new Win32Exception();

                if (!InternetSetOption(IntPtr.Zero, InternetOption.PROXY_PASSWORD, (string)null, 0))
                    throw new Win32Exception();
            }
            else
            {
                if (!InternetSetOption(IntPtr.Zero, InternetOption.PROXY_USERNAME, username, username.Length + 1))
                    throw new Win32Exception();

                if (!InternetSetOption(IntPtr.Zero, InternetOption.PROXY_PASSWORD, password, password.Length + 1))
                    throw new Win32Exception();
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
            // グローバルプロキシ (NULL = IntPtr.Zero) にユーザー名・パスワードをセットできないため無効化
            //RefreshProxyAccount(username, password);
        }
#endregion

        [StructLayout(LayoutKind.Sequential)]
        private struct SCROLLINFO
        {
            public int cbSize;
            public ScrollInfoMask fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;
        }

        public enum ScrollBarDirection
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

        public static int GetScrollPosition(Control control, ScrollBarDirection direction)
        {
            var si = new SCROLLINFO
            {
                cbSize = Marshal.SizeOf<SCROLLINFO>(),
                fMask = ScrollInfoMask.SIF_POS,
            };

            if (NativeMethods.GetScrollInfo(control.Handle, direction, ref si) == 0)
                throw new Win32Exception();

            return si.nPos;
        }
    }
}
