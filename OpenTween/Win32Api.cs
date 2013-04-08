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

namespace OpenTween
{
    public static class Win32Api
    {
        #region "先行起動プロセスをアクティブにする"
        // 外部プロセスのウィンドウを起動する
        public static void WakeupWindow(IntPtr hWnd)
        {
            // メイン・ウィンドウが最小化されていれば元に戻す
            if (IsIconic(hWnd))
            {
                ShowWindowAsync(hWnd, SW_RESTORE);
            }

            // メイン・ウィンドウを最前面に表示する
            SetForegroundWindow(hWnd);
        }

        // 外部プロセスのメイン・ウィンドウを起動するためのWin32 API
        [DllImport("user32.dll")]
        private extern static bool SetForegroundWindow(
            IntPtr hWnd);

        // ウィンドウの表示状態を設定
        [DllImport("user32.dll")]
        private extern static bool ShowWindowAsync(
            IntPtr hWnd,
            int nCmdShow);

        // 指定されたウィンドウが最小化（ アイコン化）されているかどうかを調べる
        [DllImport("user32.dll")]
        private extern static bool IsIconic(
            IntPtr hWnd);

        // ShowWindowAsync関数のパラメータに渡す定義値
        private const int SW_RESTORE = 9; // 画面を元の大きさに戻す

        // 実行中の同じアプリケーションのプロセスを取得する
        public static Process GetPreviousProcess()
        {
            var curProcess = Process.GetCurrentProcess();
            try
            {
                return Process.GetProcessesByName(curProcess.ProcessName)
                    .Where(p => p.Id != curProcess.Id)
                    .FirstOrDefault(p => string.Compare(p.MainModule.FileName, curProcess.MainModule.FileName, StringComparison.OrdinalIgnoreCase) == 0);
            }
            catch
            {
                return null;
            }
        }
        #endregion
        #region "タスクトレイアイコンのクリック"
        // 指定されたクラス名およびウィンドウ名と一致するトップレベルウィンドウのハンドルを取得します
        [DllImport("user32.dll", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private extern static IntPtr FindWindow(
            [MarshalAs(UnmanagedType.LPTStr)] string lpClassName,
            [MarshalAs(UnmanagedType.LPTStr)] string lpWindowName);

        // 指定された文字列と一致するクラス名とウィンドウ名文字列を持つウィンドウのハンドルを返します
        [DllImport("user32.dll", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        private extern static IntPtr FindWindowEx(
            IntPtr hWnd1,
            IntPtr hWnd2,
            [MarshalAs(UnmanagedType.LPTStr)] string lpsz1,
            [MarshalAs(UnmanagedType.LPTStr)] string lpsz2);

        // 指定されたウィンドウへ、指定されたメッセージを送信します
        [DllImport("user32.dll")]
        private extern static IntPtr SendMessage(
            IntPtr hwnd,
            int wMsg,
            IntPtr wParam,
            IntPtr lParam);

        // SendMessageで送信するメッセージ
        private enum Sm_Message : int
        {
            WM_USER = 0x400,                     //ユーザー定義メッセージ
            TB_GETBUTTON = WM_USER + 23,         //ツールバーのボタン取得
            TB_BUTTONCOUNT = WM_USER + 24,       //ツールバーのボタン（アイコン）数取得
            TB_GETBUTTONINFO = WM_USER + 65,     //ツールバーのボタン詳細情報取得
        }
        // ツールバーボタン構造体
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TBBUTTON
        {
            public int iBitmap;
            public IntPtr idCommand;
            public byte fsState;
            public byte fsStyle;
            public byte bReserved0;
            public byte bReserved1;
            public int dwData;
            public int iString;
        }
        // ツールバーボタン詳細情報構造体
        [StructLayout(LayoutKind.Sequential)]
        private struct TBBUTTONINFO
        {
            public Int32 cbSize;
            public Int32 dwMask;
            public Int32 idCommand;
            public Int32 iImage;
            public Byte fsState;
            public Byte fsStyle;
            public short cx;
            public IntPtr lParam;
            public IntPtr pszText;
            public Int32 cchText;
        }
        // TBBUTTONINFOのlParamでポイントされるアイコン情報（PostMessageで使用）
        [StructLayout(LayoutKind.Sequential)]
        private struct TRAYNOTIFY
        {
            public IntPtr hWnd;
            public UInt32 uID;
            public UInt32 uCallbackMessage;
            public UInt32 dwDummy1;
            public UInt32 dwDummy2;
            public IntPtr hIcon;
        }
        // TBBUTTONINFOに指定するマスク情報
        [Flags()]
        private enum ToolbarButtonMask : int
        {
            TBIF_COMMAND = 0x20,
            TBIF_LPARAM = 0x10,
            TBIF_TEXT = 0x2,
        }
        // 指定されたウィンドウを作成したスレッドの ID を取得します
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(
            IntPtr hwnd,
            out int lpdwProcessId);

        // 指定したプロセスIDに対するプロセスハンドルを取得します
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(
            ProcessAccess dwDesiredAccess,
            [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            int dwProcessId);

        // OpenProcessで指定するアクセス権
        [Flags()]
        private enum ProcessAccess : int
        {
            /// <summary>Specifies all possible access flags for the process object.</summary>
            AllAccess = CreateThread | DuplicateHandle | QueryInformation | SetInformation | Terminate | VMOperation | VMRead | VMWrite | Synchronize,
            /// <summary>Enables usage of the process handle in the CreateRemoteThread function to create a thread in the process.</summary>
            CreateThread = 0x2,
            /// <summary>Enables usage of the process handle as either the source or target process in the DuplicateHandle function to duplicate a handle.</summary>
            DuplicateHandle = 0x40,
            /// <summary>Enables usage of the process handle in the GetExitCodeProcess and GetPriorityClass functions to read information from the process object.</summary>
            QueryInformation = 0x400,
            /// <summary>Enables usage of the process handle in the SetPriorityClass function to set the priority class of the process.</summary>
            SetInformation = 0x200,
            /// <summary>Enables usage of the process handle in the TerminateProcess function to terminate the process.</summary>
            Terminate = 0x1,
            /// <summary>Enables usage of the process handle in the VirtualProtectEx and WriteProcessMemory functions to modify the virtual memory of the process.</summary>
            VMOperation = 0x8,
            /// <summary>Enables usage of the process handle in the ReadProcessMemory function to read from the virtual memory of the process.</summary>
            VMRead = 0x10,
            /// <summary>Enables usage of the process handle in the WriteProcessMemory function to write to the virtual memory of the process.</summary>
            VMWrite = 0x20,
            /// <summary>Enables usage of the process handle in any of the wait functions to wait for the process to terminate.</summary>
            Synchronize = 0x100000,
        }
        // 指定したプロセスの仮想アドレス空間にメモリ領域を確保
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr VirtualAllocEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            IntPtr dwSize,
            AllocationTypes flAllocationType,
            MemoryProtectionTypes flProtect);

        // アロケート種類
        [Flags()]
        private enum AllocationTypes : uint
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000,
        }
        // アロケートしたメモリに対する保護レベル
        [Flags()]
        private enum MemoryProtectionTypes : uint
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x1,
            ReadOnly = 0x2,
            ReadWrite = 0x4,
            WriteCopy = 0x8,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400,
        }
        // オープンしているカーネルオブジェクトのハンドルをクローズします
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hHandle);

        // 指定されたプロセスの仮想アドレス空間内のメモリ領域を解放またはコミット解除します
        [DllImport("kernel32.dll")]
        private static extern bool VirtualFreeEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            IntPtr dwSize,
            MemoryFreeTypes dwFreeType);

        // メモリ解放種別
        [Flags()]
        private enum MemoryFreeTypes
        {
            Release = 0x8000,
        }
        //指定したプロセスのメモリ領域にデータをコピーする
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            ref TBBUTTONINFO lpBuffer,
            IntPtr nSize,
            out int lpNumberOfBytesWritten);

        //指定したプロセスのメモリ領域のデータを呼び出し側プロセスのバッファにコピーする
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            IntPtr lpBuffer,
            IntPtr nSize,
            out int lpNumberOfBytesRead);

        //メッセージをウィンドウのメッセージ キューに置き、対応するウィンドウがメッセージを処理するのを待たずに戻ります
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool PostMessage(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam);

        //PostMessageで送信するメッセージ
        private enum PM_Message : uint
        {
            WM_LBUTTONDOWN = 0x201,      //左マウスボタン押し下げ
            WM_LBUTTONUP = 0x202,        //左マウスボタン離し
        }

        //タスクトレイアイコンのクリック処理
        public static bool ClickTasktrayIcon(string tooltip)
        {
            const string TRAY_WINDOW = "Shell_TrayWnd";
            const string TRAY_NOTIFYWINDOW = "TrayNotifyWnd";
            const string TRAY_PAGER = "SysPager";
            const string TOOLBAR_CONTROL = "ToolbarWindow32";
            //タスクバーのハンドル取得
            var taskbarWin = FindWindow(TRAY_WINDOW, null);
            if (taskbarWin.Equals(IntPtr.Zero)) return false;
            //通知領域のハンドル取得
            var trayWin = FindWindowEx(taskbarWin, IntPtr.Zero, TRAY_NOTIFYWINDOW, null);
            if (trayWin.Equals(IntPtr.Zero)) return false;
            //SysPagerの有無確認。（XP/2000はSysPagerあり）
            var tempWin = FindWindowEx(trayWin, IntPtr.Zero, TRAY_PAGER, null);
            if (tempWin.Equals(IntPtr.Zero)) tempWin = trayWin;
            //タスクトレイがツールバーで出来ているか確認
            // → ツールバーでなければ終了
            var toolWin = FindWindowEx(tempWin, IntPtr.Zero, TOOLBAR_CONTROL, null);
            if (toolWin.Equals(IntPtr.Zero)) return false;
            //タスクトレイのプロセス（Explorer）を取得し、外部から参照するために開く
            int expPid = 0;
            GetWindowThreadProcessId(toolWin, out expPid);
            var hProc = OpenProcess(ProcessAccess.VMOperation | ProcessAccess.VMRead | ProcessAccess.VMWrite, false, expPid);
            if (hProc.Equals(IntPtr.Zero)) return false;

            //プロセスを閉じるためにtry-finally
            try
            {
                var tbButtonLocal = new TBBUTTON();   //本プロセス内のタスクバーボタン情報作成（サイズ特定でのみ使用）
                //Explorer内のタスクバーボタン格納メモリ確保
                var ptbSysButton = VirtualAllocEx(hProc, IntPtr.Zero, (IntPtr)Marshal.SizeOf(tbButtonLocal), AllocationTypes.Reserve | AllocationTypes.Commit, MemoryProtectionTypes.ReadWrite);
                if (ptbSysButton.Equals(IntPtr.Zero)) return false; //メモリ確保失敗
                try
                {
                    var tbButtonInfoLocal = new TBBUTTONINFO();   //本プロセス内ツールバーボタン詳細情報作成
                    //Explorer内のタスクバーボタン詳細情報格納メモリ確保
                    var ptbSysInfo = VirtualAllocEx(hProc, IntPtr.Zero, (IntPtr)Marshal.SizeOf(tbButtonInfoLocal), AllocationTypes.Reserve | AllocationTypes.Commit, MemoryProtectionTypes.ReadWrite);
                    if (ptbSysInfo.Equals(IntPtr.Zero)) return false; //メモリ確保失敗
                    try
                    {
                        const int titleSize = 256;    //Tooltip文字列長
                        var title = "";            //Tooltip文字列
                        //共有メモリにTooltip読込メモリ確保
                        var pszTitle = Marshal.AllocCoTaskMem(titleSize);
                        if (pszTitle.Equals(IntPtr.Zero)) return false; //メモリ確保失敗
                        try
                        {
                            //Explorer内にTooltip読込メモリ確保
                            var pszSysTitle = VirtualAllocEx(hProc, IntPtr.Zero, (IntPtr)titleSize, AllocationTypes.Reserve | AllocationTypes.Commit, MemoryProtectionTypes.ReadWrite);
                            if (pszSysTitle.Equals(IntPtr.Zero)) return false; //メモリ確保失敗
                            try
                            {
                                //通知領域ボタン数取得
                                var iCount = (int)SendMessage(toolWin, (int)Sm_Message.TB_BUTTONCOUNT, new IntPtr(0), new IntPtr(0));
                                //左から順に情報取得
                                for (var i = 0; i < iCount; i++)
                                {
                                    var dwBytes = 0;  //読み書きバイト数
                                    var tbButtonLocal2 = new TBBUTTON();  //ボタン情報
                                    var tbButtonInfoLocal2 = new TBBUTTONINFO();  //ボタン詳細情報
                                    //共有メモリにボタン情報読込メモリ確保
                                    var ptrLocal = Marshal.AllocCoTaskMem(Marshal.SizeOf(tbButtonLocal));
                                    if (ptrLocal.Equals(IntPtr.Zero)) return false; //メモリ確保失敗
                                    try
                                    {
                                        Marshal.StructureToPtr(tbButtonLocal, ptrLocal, true);   //共有メモリ初期化
                                        //ボタン情報取得（idCommandを取得するため）
                                        SendMessage(
                                            toolWin,
                                            (int)Sm_Message.TB_GETBUTTON,
                                            new IntPtr(i),
                                            ptbSysButton);
                                        //Explorer内のメモリを共有メモリに読み込み
                                        ReadProcessMemory(
                                            hProc,
                                            ptbSysButton,
                                            ptrLocal,
                                            (IntPtr)Marshal.SizeOf(tbButtonLocal),
                                            out dwBytes);
                                        //共有メモリの内容を構造体に変換
                                        tbButtonLocal2 = (TBBUTTON)Marshal.PtrToStructure(
                                                                ptrLocal,
                                                                typeof(TBBUTTON));
                                    }
                                    finally
                                    {
                                        Marshal.FreeCoTaskMem(ptrLocal); //共有メモリ解放
                                    }

                                    //ボタン詳細情報を取得するためのマスク等を設定
                                    tbButtonInfoLocal.cbSize = Marshal.SizeOf(tbButtonInfoLocal);
                                    tbButtonInfoLocal.dwMask = (int)(ToolbarButtonMask.TBIF_COMMAND | ToolbarButtonMask.TBIF_LPARAM | ToolbarButtonMask.TBIF_TEXT);
                                    tbButtonInfoLocal.pszText = pszSysTitle;     //Tooltip書き込み先領域
                                    tbButtonInfoLocal.cchText = titleSize;
                                    //マスク設定等をExplorerのメモリへ書き込み
                                    WriteProcessMemory(
                                        hProc,
                                        ptbSysInfo,
                                        ref tbButtonInfoLocal,
                                        (IntPtr)Marshal.SizeOf(tbButtonInfoLocal),
                                        out dwBytes);
                                    //ボタン詳細情報取得
                                    SendMessage(
                                        toolWin,
                                        (int)Sm_Message.TB_GETBUTTONINFO,
                                        tbButtonLocal2.idCommand,
                                        ptbSysInfo);
                                    //共有メモリにボタン詳細情報を読み込む領域確保
                                    var ptrInfo = Marshal.AllocCoTaskMem(Marshal.SizeOf(tbButtonInfoLocal));
                                    if (ptrInfo.Equals(IntPtr.Zero)) return false; //共有メモリ確保失敗
                                    try
                                    {
                                        Marshal.StructureToPtr(tbButtonInfoLocal, ptrInfo, true);    //共有メモリ初期化
                                        //Explorer内のメモリを共有メモリに読み込み
                                        ReadProcessMemory(
                                            hProc,
                                            ptbSysInfo,
                                            ptrInfo,
                                            (IntPtr)Marshal.SizeOf(tbButtonInfoLocal),
                                            out dwBytes);
                                        //共有メモリの内容を構造体に変換
                                        tbButtonInfoLocal2 = (TBBUTTONINFO)Marshal.PtrToStructure(
                                                                    ptrInfo,
                                                                    typeof(TBBUTTONINFO));
                                    }
                                    finally
                                    {
                                        Marshal.FreeCoTaskMem(ptrInfo);  //共有メモリ解放
                                    }
                                    //Tooltipの内容をExplorer内のメモリから共有メモリへ読込
                                    ReadProcessMemory(hProc, pszSysTitle, pszTitle, (IntPtr)titleSize, out dwBytes);
                                    //ローカル変数へ変換
                                    title = Marshal.PtrToStringAnsi(pszTitle, titleSize);

                                    //Tooltipが指定文字列を含んでいればクリック
                                    if (title.Contains(tooltip))
                                    {
                                        //PostMessageでクリックを送るために、ボタン詳細情報のlParamでポイントされているTRAYNOTIFY情報が必要
                                        var tNotify = new TRAYNOTIFY();
                                        var tNotify2 = new TRAYNOTIFY();
                                        //共有メモリ確保
                                        var ptNotify = Marshal.AllocCoTaskMem(Marshal.SizeOf(tNotify));
                                        if (ptNotify.Equals(IntPtr.Zero)) return false; //メモリ確保失敗
                                        try
                                        {
                                            Marshal.StructureToPtr(tNotify, ptNotify, true); //初期化
                                            //lParamのメモリを読込
                                            ReadProcessMemory(
                                                hProc,
                                                tbButtonInfoLocal2.lParam,
                                                ptNotify,
                                                (IntPtr)Marshal.SizeOf(tNotify),
                                                out dwBytes);
                                            //構造体へ変換
                                            tNotify2 = (TRAYNOTIFY)
                                                            Marshal.PtrToStructure(
                                                                ptNotify,
                                                                typeof(TRAYNOTIFY));
                                        }
                                        finally
                                        {
                                            Marshal.FreeCoTaskMem(ptNotify); //共有メモリ解放
                                        }
                                        //クリックするためには通知領域がアクティブでなければならない
                                        SetForegroundWindow(tNotify2.hWnd);
                                        //左クリック
                                        PostMessage(tNotify2.hWnd, tNotify2.uCallbackMessage, (IntPtr)tNotify2.uID, (IntPtr)PM_Message.WM_LBUTTONDOWN);
                                        PostMessage(tNotify2.hWnd, tNotify2.uCallbackMessage, (IntPtr)tNotify2.uID, (IntPtr)PM_Message.WM_LBUTTONUP);
                                        return true;
                                    }
                                }
                                return false;    //該当なし
                            }
                            finally
                            {
                                VirtualFreeEx(hProc, pszSysTitle, (IntPtr)titleSize, MemoryFreeTypes.Release);   //メモリ解放
                            }
                        }
                        finally
                        {
                            Marshal.FreeCoTaskMem(pszTitle);     //共有メモリ解放
                        }
                    }
                    finally
                    {
                        VirtualFreeEx(hProc, ptbSysInfo, (IntPtr)Marshal.SizeOf(tbButtonInfoLocal), MemoryFreeTypes.Release);    //メモリ解放
                    }
                }
                finally
                {
                    VirtualFreeEx(hProc, ptbSysButton, (IntPtr)Marshal.SizeOf(tbButtonLocal), MemoryFreeTypes.Release);      //メモリ解放
                }
            }
            finally
            {
                CloseHandle(hProc);  //Explorerのプロセス閉じる
            }
        }
        #endregion

        //画面をブリンクするためのWin32API。起動時に10ページ読み取りごとに継続確認メッセージを表示する際の通知強調用
        [DllImport("user32.dll")]
        private static extern int FlashWindow(
            IntPtr hwnd,
            bool bInvert);

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

        #region "スクリーンセーバー起動中か判定"
        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(
                    int intAction,
                    int intParam,
                    ref bool bParam,
                    int intWinIniFlag);
        // returns non-zero value if function succeeds

        //スクリーンセーバーが起動中かを取得する定数
        private const int SPI_GETSCREENSAVERRUNNING = 0x61;

        public static bool IsScreenSaverRunning()
        {
            var ret = 0;
            var isRunning = false;

            ret = SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, ref isRunning, 0);
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

        public static void SetProxy(HttpConnection.ProxyType pType, string host, int port, string username, string password)
        {
            string proxy = null;
            switch (pType)
            {
            case HttpConnection.ProxyType.IE:
                proxy = null;
                break;
            case HttpConnection.ProxyType.None:
                proxy = "";
                break;
            case HttpConnection.ProxyType.Specified:
                proxy = host + (port > 0 ? ":" + port.ToString() : "");
                break;
            }
            RefreshProxySettings(proxy);
            RefreshProxyAccount(username, password);
        }
#endregion
    }
}
