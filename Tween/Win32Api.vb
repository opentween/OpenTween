' Tween - Client of Twitter
' Copyright (c) 2007-2010 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
'           (c) 2008-2010 Moz (@syo68k) <http://iddy.jp/profile/moz/>
'           (c) 2008-2010 takeshik (@takeshik) <http://www.takeshik.org/>
' All rights reserved.
' 
' This file is part of Tween.
' 
' This program is free software; you can redistribute it and/or modify it
' under the terms of the GNU General Public License as published by the Free
' Software Foundation; either version 3 of the License, or (at your option)
' any later version.
' 
' This program is distributed in the hope that it will be useful, but
' WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
' or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
' for more details. 
' 
' You should have received a copy of the GNU General Public License along
' with this program. If not, see <http://www.gnu.org/licenses/>, or write to
' the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
' Boston, MA 02110-1301, USA.

Imports System.Runtime.InteropServices
Imports System.Diagnostics
Imports System.Threading

Module Win32Api
#Region "先行起動プロセスをアクティブにする"
    ' 外部プロセスのウィンドウを起動する
    Public Sub WakeupWindow(ByVal hWnd As IntPtr)
        ' メイン・ウィンドウが最小化されていれば元に戻す
        If IsIconic(hWnd) Then
            ShowWindowAsync(hWnd, SW_RESTORE)
        End If

        ' メイン・ウィンドウを最前面に表示する
        SetForegroundWindow(hWnd)
    End Sub

    ' 外部プロセスのメイン・ウィンドウを起動するためのWin32 API
    <DllImport("user32.dll")> _
    Private Function SetForegroundWindow( _
        ByVal hWnd As IntPtr) As Boolean
    End Function
    ' ウィンドウの表示状態を設定
    <DllImport("user32.dll")> _
    Private Function ShowWindowAsync( _
        ByVal hWnd As IntPtr, _
        ByVal nCmdShow As Integer) As Boolean
    End Function
    ' 指定されたウィンドウが最小化（ アイコン化）されているかどうかを調べる
    <DllImport("user32.dll")> _
    Private Function IsIconic( _
        ByVal hWnd As IntPtr) As Boolean
    End Function
    ' ShowWindowAsync関数のパラメータに渡す定義値
    Private Const SW_RESTORE As Integer = 9 ' 画面を元の大きさに戻す

    ' 実行中の同じアプリケーションのプロセスを取得する
    Public Function GetPreviousProcess() As Process
        Dim curProcess As Process = Process.GetCurrentProcess()
        Dim allProcesses() As Process = Process.GetProcessesByName(curProcess.ProcessName)

        Dim checkProcess As Process
        For Each checkProcess In allProcesses
            ' 自分自身のプロセスIDは無視する
            If checkProcess.Id <> curProcess.Id Then
                ' プロセスのフルパス名を比較して同じアプリケーションか検証
                If String.Compare( _
                        checkProcess.MainModule.FileName, _
                        curProcess.MainModule.FileName, True) = 0 Then
                    ' 同じフルパス名のプロセスを取得
                    Return checkProcess
                End If
            End If
        Next

        ' 同じアプリケーションのプロセスが見つからない！  
        Return Nothing
    End Function
#End Region
#Region "タスクトレイアイコンのクリック"
    ' 指定されたクラス名およびウィンドウ名と一致するトップレベルウィンドウのハンドルを取得します
    <DllImport("user32.dll")> _
    Private Function FindWindow( _
        ByVal lpClassName As String, _
        ByVal lpWindowName As String) As IntPtr
    End Function
    ' 指定された文字列と一致するクラス名とウィンドウ名文字列を持つウィンドウのハンドルを返します
    <DllImport("user32.dll")> _
    Private Function FindWindowEx( _
        ByVal hWnd1 As IntPtr, _
        ByVal hWnd2 As IntPtr, _
        ByVal lpsz1 As String, _
        ByVal lpsz2 As String) As IntPtr
    End Function
    ' 指定されたウィンドウへ、指定されたメッセージを送信します
    <DllImport("user32.dll")> _
    Private Function SendMessage( _
        ByVal hwnd As IntPtr, _
        ByVal wMsg As Integer, _
        ByVal wParam As IntPtr, _
        ByVal lParam As IntPtr) As Integer
    End Function
    ' SendMessageで送信するメッセージ
    Private Enum Sm_Message As Integer
        WM_USER = &H400                     'ユーザー定義メッセージ
        TB_GETBUTTON = WM_USER + 23         'ツールバーのボタン取得
        TB_BUTTONCOUNT = WM_USER + 24       'ツールバーのボタン（アイコン）数取得
        TB_GETBUTTONINFO = WM_USER + 65     'ツールバーのボタン詳細情報取得
    End Enum
    ' ツールバーボタン構造体
    <StructLayout(LayoutKind.Sequential, Pack:=1)> _
    Private Structure TBBUTTON
        Public iBitmap As Integer
        Public idCommand As IntPtr
        Public fsState As Byte
        Public fsStyle As Byte
        Public bReserved0 As Byte
        Public bReserved1 As Byte
        Public dwData As Integer
        Public iString As Integer
    End Structure
    ' ツールバーボタン詳細情報構造体
    <StructLayout(LayoutKind.Sequential)> _
    Private Structure TBBUTTONINFO
        Public cbSize As Int32
        Public dwMask As Int32
        Public idCommand As Int32
        Public iImage As Int32
        Public fsState As Byte
        Public fsStyle As Byte
        Public cx As Short
        Public lParam As IntPtr
        Public pszText As IntPtr
        Public cchText As Int32
    End Structure
    ' TBBUTTONINFOのlParamでポイントされるアイコン情報（PostMessageで使用）
    <StructLayout(LayoutKind.Sequential)> _
    Private Structure TRAYNOTIFY
        Public hWnd As IntPtr
        Public uID As UInt32
        Public uCallbackMessage As UInt32
        Public dwDummy1 As UInt32
        Public dwDummy2 As UInt32
        Public hIcon As IntPtr
    End Structure
    ' TBBUTTONINFOに指定するマスク情報
    <Flags()> _
    Private Enum ToolbarButtonMask As Int32
        TBIF_COMMAND = &H20
        TBIF_LPARAM = &H10
        TBIF_TEXT = &H2
    End Enum
    ' 指定されたウィンドウを作成したスレッドの ID を取得します
    <DllImport("user32.dll", SetLastError:=True)> _
    Private Function GetWindowThreadProcessId( _
        ByVal hwnd As IntPtr, _
        ByRef lpdwProcessId As Integer) As Integer
    End Function
    ' 指定したプロセスIDに対するプロセスハンドルを取得します
    <DllImport("kernel32.dll")> _
    Private Function OpenProcess( _
        ByVal dwDesiredAccess As ProcessAccess, _
        <MarshalAs(UnmanagedType.Bool)> ByVal bInheritHandle As Boolean, _
        ByVal dwProcessId As Integer) As IntPtr
    End Function
    ' OpenProcessで指定するアクセス権
    <Flags()> _
    Private Enum ProcessAccess As Integer
        ''' <summary>Specifies all possible access flags for the process object.</summary>
        AllAccess = CreateThread Or DuplicateHandle Or QueryInformation Or SetInformation Or Terminate Or VMOperation Or VMRead Or VMWrite Or Synchronize
        ''' <summary>Enables usage of the process handle in the CreateRemoteThread function to create a thread in the process.</summary>
        CreateThread = &H2
        ''' <summary>Enables usage of the process handle as either the source or target process in the DuplicateHandle function to duplicate a handle.</summary>
        DuplicateHandle = &H40
        ''' <summary>Enables usage of the process handle in the GetExitCodeProcess and GetPriorityClass functions to read information from the process object.</summary>
        QueryInformation = &H400
        ''' <summary>Enables usage of the process handle in the SetPriorityClass function to set the priority class of the process.</summary>
        SetInformation = &H200
        ''' <summary>Enables usage of the process handle in the TerminateProcess function to terminate the process.</summary>
        Terminate = &H1
        ''' <summary>Enables usage of the process handle in the VirtualProtectEx and WriteProcessMemory functions to modify the virtual memory of the process.</summary>
        VMOperation = &H8
        ''' <summary>Enables usage of the process handle in the ReadProcessMemory function to' read from the virtual memory of the process.</summary>
        VMRead = &H10
        ''' <summary>Enables usage of the process handle in the WriteProcessMemory function to write to the virtual memory of the process.</summary>
        VMWrite = &H20
        ''' <summary>Enables usage of the process handle in any of the wait functions to wait for the process to terminate.</summary>
        Synchronize = &H100000
    End Enum
    ' 指定したプロセスの仮想アドレス空間にメモリ領域を確保
    <DllImport("kernel32.dll", SetLastError:=True, ExactSpelling:=True)> _
    Private Function VirtualAllocEx( _
        ByVal hProcess As IntPtr, _
        ByVal lpAddress As IntPtr, _
        ByVal dwSize As Integer, _
        ByVal flAllocationType As AllocationTypes, _
        ByVal flProtect As MemoryProtectionTypes) As IntPtr
    End Function
    ' アロケート種類
    <Flags()> _
    Private Enum AllocationTypes As UInteger
        Commit = &H1000
        Reserve = &H2000
        Decommit = &H4000
        Release = &H8000
        Reset = &H80000
        Physical = &H400000
        TopDown = &H100000
        WriteWatch = &H200000
        LargePages = &H20000000
    End Enum
    ' アロケートしたメモリに対する保護レベル
    <Flags()> _
    Private Enum MemoryProtectionTypes As UInteger
        Execute = &H10
        ExecuteRead = &H20
        ExecuteReadWrite = &H40
        ExecuteWriteCopy = &H80
        NoAccess = &H1
        [ReadOnly] = &H2
        ReadWrite = &H4
        WriteCopy = &H8
        GuardModifierflag = &H100
        NoCacheModifierflag = &H200
        WriteCombineModifierflag = &H400
    End Enum
    ' オープンしているカーネルオブジェクトのハンドルをクローズします
    <DllImport("kernel32.dll", SetLastError:=True)> _
    Private Function CloseHandle(ByVal hHandle As IntPtr) As Boolean
    End Function
    ' 指定されたプロセスの仮想アドレス空間内のメモリ領域を解放またはコミット解除します
    <DllImport("kernel32.dll")> _
    Private Function VirtualFreeEx( _
        ByVal hProcess As IntPtr, _
        ByVal lpAddress As IntPtr, _
        ByVal dwSize As Integer, _
        ByVal dwFreeType As Integer) As Boolean
    End Function
    ' メモリ解放種別
    <Flags()> _
    Private Enum MemoryFreeTypes
        Release = &H8000
    End Enum
    '指定したプロセスのメモリ領域にデータをコピーする
    <DllImport("kernel32.dll", SetLastError:=True)> _
    Private Function WriteProcessMemory( _
        ByVal hProcess As IntPtr, _
        ByVal lpBaseAddress As IntPtr, _
        ByRef lpBuffer As TBBUTTONINFO, _
        ByVal nSize As Integer, _
        <Out()> ByRef lpNumberOfBytesWritten As Integer) As Boolean
    End Function
    '指定したプロセスのメモリ領域のデータを呼び出し側プロセスのバッファにコピーする
    <DllImport("kernel32.dll", SetLastError:=True)> _
    Private Function ReadProcessMemory( _
        ByVal hProcess As IntPtr, _
        ByVal lpBaseAddress As IntPtr, _
        ByVal lpBuffer As IntPtr, _
        ByVal iSize As Integer, _
        ByRef lpNumberOfBytesRead As Integer) As Boolean
    End Function
    'メッセージをウィンドウのメッセージ キューに置き、対応するウィンドウがメッセージを処理するのを待たずに戻ります
    <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
    Private Function PostMessage( _
        ByVal hWnd As IntPtr, _
        ByVal Msg As UInteger, _
        ByVal wParam As UInt32, _
        ByVal lParam As UInt32) As Boolean
    End Function
    'PostMessageで送信するメッセージ
    Private Enum PM_Message As UInt32
        WM_LBUTTONDOWN = &H201      '左マウスボタン押し下げ
        WM_LBUTTONUP = &H202        '左マウスボタン離し
    End Enum

    'タスクトレイアイコンのクリック処理
    Public Function ClickTasktrayIcon(ByVal tooltip As String) As Boolean
        Const TRAY_WINDOW As String = "Shell_TrayWnd"
        Const TRAY_NOTIFYWINDOW As String = "TrayNotifyWnd"
        Const TRAY_PAGER As String = "SysPager"
        Const TOOLBAR_CONTROL As String = "ToolbarWindow32"
        'タスクバーのハンドル取得
        Dim taskbarWin As IntPtr = FindWindow(TRAY_WINDOW, Nothing)
        If taskbarWin.Equals(IntPtr.Zero) Then Return False
        '通知領域のハンドル取得
        Dim trayWin As IntPtr = FindWindowEx(taskbarWin, IntPtr.Zero, TRAY_NOTIFYWINDOW, Nothing)
        If trayWin.Equals(IntPtr.Zero) Then Return False
        'SysPagerの有無確認。（XP/2000はSysPagerあり）
        Dim tempWin As IntPtr = FindWindowEx(trayWin, IntPtr.Zero, TRAY_PAGER, Nothing)
        If tempWin.Equals(IntPtr.Zero) Then tempWin = trayWin
        'タスクトレイがツールバーで出来ているか確認
        '　→　ツールバーでなければ終了
        Dim toolWin As IntPtr = FindWindowEx(tempWin, IntPtr.Zero, TOOLBAR_CONTROL, Nothing)
        If toolWin.Equals(IntPtr.Zero) Then Return False
        'タスクトレイのプロセス（Explorer）を取得し、外部から参照するために開く
        Dim expPid As Integer = 0
        GetWindowThreadProcessId(toolWin, expPid)
        Dim hProc As IntPtr = OpenProcess(ProcessAccess.VMOperation Or ProcessAccess.VMRead Or ProcessAccess.VMWrite, False, expPid)
        If hProc.Equals(IntPtr.Zero) Then Return False

        'プロセスを閉じるためにTry-Finally
        Try
            Dim tbButtonLocal As New TBBUTTON   '本プロセス内のタスクバーボタン情報作成（サイズ特定でのみ使用）
            'Explorer内のタスクバーボタン格納メモリ確保
            Dim ptbSysButton As IntPtr = VirtualAllocEx(hProc, IntPtr.Zero, Marshal.SizeOf(tbButtonLocal), AllocationTypes.Reserve Or AllocationTypes.Commit, MemoryProtectionTypes.ReadWrite)
            If ptbSysButton.Equals(IntPtr.Zero) Then Return False 'メモリ確保失敗
            Try
                Dim tbButtonInfoLocal As New TBBUTTONINFO   '本プロセス内ツールバーボタン詳細情報作成
                'Explorer内のタスクバーボタン詳細情報格納メモリ確保
                Dim ptbSysInfo As IntPtr = VirtualAllocEx(hProc, IntPtr.Zero, Marshal.SizeOf(tbButtonInfoLocal), AllocationTypes.Reserve Or AllocationTypes.Commit, MemoryProtectionTypes.ReadWrite)
                If ptbSysInfo.Equals(IntPtr.Zero) Then Return False 'メモリ確保失敗
                Try
                    Const titleSize As Integer = 256    'Tooltip文字列長
                    Dim title As String = ""            'Tooltip文字列
                    '共有メモリにTooltip読込メモリ確保
                    Dim pszTitle As IntPtr = Marshal.AllocCoTaskMem(titleSize)
                    If pszTitle.Equals(IntPtr.Zero) Then Return False 'メモリ確保失敗
                    Try
                        'Explorer内にTooltip読込メモリ確保
                        Dim pszSysTitle As IntPtr = VirtualAllocEx(hProc, IntPtr.Zero, titleSize, AllocationTypes.Reserve Or AllocationTypes.Commit, MemoryProtectionTypes.ReadWrite)
                        If pszSysTitle.Equals(IntPtr.Zero) Then Return False 'メモリ確保失敗
                        Try
                            '通知領域ボタン数取得
                            Dim iCount As Integer = SendMessage(toolWin, Sm_Message.TB_BUTTONCOUNT, New IntPtr(0), New IntPtr(0))
                            '左から順に情報取得
                            For i As Integer = 0 To iCount - 1
                                Dim dwBytes As Integer = 0  '読み書きバイト数
                                Dim tbButtonLocal2 As TBBUTTON  'ボタン情報
                                Dim tbButtonInfoLocal2 As TBBUTTONINFO  'ボタン詳細情報
                                '共有メモリにボタン情報読込メモリ確保
                                Dim ptrLocal As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(tbButtonLocal))
                                If ptrLocal.Equals(IntPtr.Zero) Then Return False 'メモリ確保失敗
                                Try
                                    Marshal.StructureToPtr(tbButtonLocal, ptrLocal, True)   '共有メモリ初期化
                                    'ボタン情報取得（idCommandを取得するため）
                                    SendMessage( _
                                        toolWin, _
                                        Sm_Message.TB_GETBUTTON, _
                                        New IntPtr(i), _
                                        ptbSysButton)
                                    'Explorer内のメモリを共有メモリに読み込み
                                    ReadProcessMemory( _
                                        hProc, _
                                        ptbSysButton, _
                                        ptrLocal, _
                                        Marshal.SizeOf(tbButtonLocal), _
                                        dwBytes)
                                    '共有メモリの内容を構造体に変換
                                    tbButtonLocal2 = DirectCast( _
                                                        Marshal.PtrToStructure( _
                                                            ptrLocal, _
                                                            GetType(TBBUTTON)), _
                                                        TBBUTTON)
                                Finally
                                    Marshal.FreeCoTaskMem(ptrLocal) '共有メモリ解放
                                End Try

                                'ボタン詳細情報を取得するためのマスク等を設定
                                tbButtonInfoLocal.cbSize = Marshal.SizeOf(tbButtonInfoLocal)
                                tbButtonInfoLocal.dwMask = ToolbarButtonMask.TBIF_COMMAND Or ToolbarButtonMask.TBIF_LPARAM Or ToolbarButtonMask.TBIF_TEXT
                                tbButtonInfoLocal.pszText = pszSysTitle     'Tooltip書き込み先領域
                                tbButtonInfoLocal.cchText = titleSize
                                'マスク設定等をExplorerのメモリへ書き込み
                                WriteProcessMemory( _
                                    hProc, _
                                    ptbSysInfo, _
                                    tbButtonInfoLocal, _
                                    Marshal.SizeOf(tbButtonInfoLocal), _
                                    dwBytes)
                                'ボタン詳細情報取得
                                SendMessage( _
                                    toolWin, _
                                    Sm_Message.TB_GETBUTTONINFO, _
                                    tbButtonLocal2.idCommand, _
                                    ptbSysInfo)
                                '共有メモリにボタン詳細情報を読み込む領域確保
                                Dim ptrInfo As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(tbButtonInfoLocal))
                                If ptrInfo.Equals(IntPtr.Zero) Then Return False '共有メモリ確保失敗
                                Try
                                    Marshal.StructureToPtr(tbButtonInfoLocal, ptrInfo, True)    '共有メモリ初期化
                                    'Explorer内のメモリを共有メモリに読み込み
                                    ReadProcessMemory( _
                                        hProc, _
                                        ptbSysInfo, _
                                        ptrInfo, _
                                        Marshal.SizeOf(tbButtonInfoLocal), _
                                        dwBytes)
                                    '共有メモリの内容を構造体に変換
                                    tbButtonInfoLocal2 = DirectCast( _
                                                            Marshal.PtrToStructure( _
                                                                ptrInfo, _
                                                                GetType(TBBUTTONINFO)), _
                                                            TBBUTTONINFO)
                                Finally
                                    Marshal.FreeCoTaskMem(ptrInfo)  '共有メモリ解放
                                End Try
                                'Tooltipの内容をExplorer内のメモリから共有メモリへ読込
                                ReadProcessMemory(hProc, pszSysTitle, pszTitle, titleSize, dwBytes)
                                'ローカル変数へ変換
                                title = Marshal.PtrToStringAnsi(pszTitle, titleSize)

                                'Tooltipが指定文字列を含んでいればクリック
                                If title.Contains(tooltip) Then
                                    'PostMessageでクリックを送るために、ボタン詳細情報のlParamでポイントされているTRAYNOTIFY情報が必要
                                    Dim tNotify As New TRAYNOTIFY
                                    Dim tNotify2 As TRAYNOTIFY
                                    '共有メモリ確保
                                    Dim ptNotify As IntPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(tNotify))
                                    If ptNotify.Equals(IntPtr.Zero) Then Return False 'メモリ確保失敗
                                    Try
                                        Marshal.StructureToPtr(tNotify, ptNotify, True) '初期化
                                        'lParamのメモリを読込
                                        ReadProcessMemory( _
                                            hProc, _
                                            tbButtonInfoLocal2.lParam, _
                                            ptNotify, _
                                            Marshal.SizeOf(tNotify), _
                                            dwBytes)
                                        '構造体へ変換
                                        tNotify2 = DirectCast( _
                                                        Marshal.PtrToStructure( _
                                                            ptNotify, _
                                                            GetType(TRAYNOTIFY)), _
                                                        TRAYNOTIFY)
                                    Finally
                                        Marshal.FreeCoTaskMem(ptNotify) '共有メモリ解放
                                    End Try
                                    'クリックするためには通知領域がアクティブでなければならない
                                    SetForegroundWindow(tNotify2.hWnd)
                                    '左クリック
                                    PostMessage(tNotify2.hWnd, tNotify2.uCallbackMessage, tNotify2.uID, PM_Message.WM_LBUTTONDOWN)
                                    PostMessage(tNotify2.hWnd, tNotify2.uCallbackMessage, tNotify2.uID, PM_Message.WM_LBUTTONUP)
                                    Return True
                                End If
                            Next
                            Return False    '該当なし
                        Finally
                            VirtualFreeEx(hProc, pszSysTitle, titleSize, MemoryFreeTypes.Release)   'メモリ解放
                        End Try
                    Finally
                        Marshal.FreeCoTaskMem(pszTitle)     '共有メモリ解放
                    End Try
                Finally
                    VirtualFreeEx(hProc, ptbSysInfo, Marshal.SizeOf(tbButtonInfoLocal), MemoryFreeTypes.Release)    'メモリ解放
                End Try
            Finally
                VirtualFreeEx(hProc, ptbSysButton, Marshal.SizeOf(tbButtonLocal), MemoryFreeTypes.Release)      'メモリ解放
            End Try
        Finally
            CloseHandle(hProc)  'Explorerのプロセス閉じる
        End Try
    End Function
#End Region

    <DllImport("user32.dll")> _
    Public Function ValidateRect( _
        ByVal hwnd As IntPtr, _
        ByVal rect As IntPtr) As Boolean
    End Function

#Region "スクリーンセーバー起動中か判定"
    <DllImport("user32", CharSet:=CharSet.Auto)> _
    Private Function SystemParametersInfo( _
                ByVal intAction As Integer, _
                ByVal intParam As Integer, _
                ByRef bParam As Boolean, _
                ByVal intWinIniFlag As Integer) As Integer
        ' returns non-zero value if function succeeds
    End Function
    'スクリーンセーバーが起動中かを取得する定数
    Private Const SPI_GETSCREENSAVERRUNNING As Integer = &H61

    Public Function IsScreenSaverRunning() As Boolean
        Dim ret As Integer = 0
        Dim isRunning As Boolean = False

        ret = SystemParametersInfo(SPI_GETSCREENSAVERRUNNING, 0, isRunning, 0)
        Return isRunning
    End Function
#End Region

#Region "グローバルフック"
    Private Declare Function RegisterHotKey Lib "user32" (ByVal hwnd As IntPtr, ByVal id As Integer, _
        ByVal fsModifiers As Integer, ByVal vk As Integer) As Integer
    Private Declare Function UnregisterHotKey Lib "user32" (ByVal hwnd As IntPtr, ByVal id As Integer) _
       As Integer
    Private Declare Function GlobalAddAtom Lib "kernel32" Alias "GlobalAddAtomA" (ByVal lpString As _
       String) As Integer
    Private Declare Function GlobalDeleteAtom Lib "kernel32" (ByVal nAtom As Integer) As Integer

    ' register a global hot key
    Public Function RegisterGlobalHotKey(ByVal hotkey As Keys, ByVal modifiers As Integer, ByVal targetForm As Form) As Integer
        Dim hotkeyID As Integer = 0
        Try
            ' use the GlobalAddAtom API to get a unique ID (as suggested by MSDN docs)
            Static count As Integer = 0
            count += 1
            Dim atomName As String = Thread.CurrentThread.ManagedThreadId.ToString("X8") & targetForm.Name & count.ToString()
            hotkeyID = GlobalAddAtom(atomName)
            If hotkeyID = 0 Then
                Throw New Exception("Unable to generate unique hotkey ID. Error code: " & _
                   Marshal.GetLastWin32Error().ToString)
            End If

            ' register the hotkey, throw if any error
            If RegisterHotKey(targetForm.Handle, hotkeyID, modifiers, CInt(hotkey)) = 0 Then
                Throw New Exception("Unable to register hotkey. Error code: " & _
                   Marshal.GetLastWin32Error.ToString)
            End If
            Return hotkeyID
        Catch ex As Exception
            ' clean up if hotkey registration failed
            UnregisterGlobalHotKey(hotkeyID, targetForm)
            Return 0
        End Try
    End Function

    ' unregister a global hotkey
    Public Sub UnregisterGlobalHotKey(ByVal hotkeyID As Integer, ByVal targetForm As Form)
        If hotkeyID <> 0 Then
            UnregisterHotKey(targetForm.Handle, hotkeyID)
            ' clean up the atom list
            GlobalDeleteAtom(hotkeyID)
            hotkeyID = 0
        End If
    End Sub
#End Region
End Module
