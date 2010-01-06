' Tween - Client of Twitter
' Copyright (c) 2007-2009 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
'           (c) 2008-2009 Moz (@syo68k) <http://iddy.jp/profile/moz/>
'           (c) 2008-2009 takeshik (@takeshik) <http://www.takeshik.org/>
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

Option Strict On

Imports System.Diagnostics
Namespace My

    ' 次のイベントは MyApplication に対して利用できます:
    ' 
    ' Startup: アプリケーションが開始されたとき、スタートアップ フォームが作成される前に発生します。
    ' Shutdown: アプリケーション フォームがすべて閉じられた後に発生します。このイベントは、通常の終了以外の方法でアプリケーションが終了されたときには発生しません。
    ' UnhandledException: ハンドルされていない例外がアプリケーションで発生したときに発生するイベントです。
    ' StartupNextInstance: 単一インスタンス アプリケーションが起動され、それが既にアクティブであるときに発生します。 
    ' NetworkAvailabilityChanged: ネットワーク接続が接続されたとき、または切断されたときに発生します。
    Partial Friend Class MyApplication
        Private Shared mt As System.Threading.Mutex

        Private Sub MyApplication_Shutdown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shutdown
            Try
                If mt IsNot Nothing Then
                    mt.ReleaseMutex()
                    mt.Close()
                    mt = Nothing
                End If
            Catch ex As Exception

            End Try
        End Sub

        Private Sub MyApplication_Startup(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.StartupEventArgs) Handles Me.Startup

            InitCulture()

            Dim pt As String = Application.Info.DirectoryPath.Replace("\", "/") + "/" + Application.Info.ProductName
            mt = New System.Threading.Mutex(False, pt)
            Try
                If Not mt.WaitOne(0, False) Then
                    ' 実行中の同じアプリケーションのウィンドウ・ハンドルの取得
                    Dim prevProcess As Process = GetPreviousProcess()
                    If prevProcess IsNot Nothing AndAlso _
                        IntPtr.op_Inequality(prevProcess.MainWindowHandle, IntPtr.Zero) Then
                        ' 起動中のアプリケーションを最前面に表示
                        WakeupWindow(prevProcess.MainWindowHandle)
                    Else
                        If prevProcess IsNot Nothing Then
                            'プロセス特定は出来たが、ウィンドウハンドルが取得できなかった（アイコン化されている）
                            'タスクトレイアイコンのクリックをエミュレート
                            '注：アイコン特定はTooltipの文字列で行うため、多重起動時は先に見つけた物がアクティブになる
                            Dim rslt As Boolean = ClickTasktrayIcon("Tween")
                            If Not rslt Then
                                ' 警告を表示（見つからない、またはその他の原因で失敗）
                                MessageBox.Show(My.Resources.StartupText1, My.Resources.StartupText2, MessageBoxButtons.OK, MessageBoxIcon.Information)
                            End If
                        Else
                            ' 警告を表示（プロセス見つからない場合）
                            MessageBox.Show(My.Resources.StartupText1, My.Resources.StartupText2, MessageBoxButtons.OK, MessageBoxIcon.Information)
                            'MessageBox.Show("すでに起動しています。2つ同時には起動できません。", "多重起動禁止")
                        End If

                    End If
                    '起動キャンセル
                    e.Cancel = True
                    Try
                        mt.ReleaseMutex()
                        mt.Close()
                        mt = Nothing
                    Catch ex As Exception

                    End Try
                    Exit Sub
                End If
            Catch ex As Exception
            End Try

        End Sub

        Private Sub MyApplication_UnhandledException(ByVal sender As Object, ByVal e As Microsoft.VisualBasic.ApplicationServices.UnhandledExceptionEventArgs) Handles Me.UnhandledException
            'GDI+のエラー原因を特定したい
            If e.Exception.Message <> "A generic error occurred in GDI+." AndAlso _
               e.Exception.Message <> "GDI+ で汎用エラーが発生しました。" Then
                e.ExitApplication = ExceptionOut(e.Exception)
            Else
                e.ExitApplication = False
            End If
        End Sub

        Public ReadOnly Property CultureCode() As String
            Get
                'Static _ccode As String = Nothing
                If cultureStr Is Nothing Then
                    Dim cfgCommon As SettingCommon = SettingCommon.Load()
                    cultureStr = cfgCommon.Language
                    If cultureStr = "OS" Then
                        If Not Threading.Thread.CurrentThread.CurrentUICulture.Name.StartsWith("ja") AndAlso _
                           Not Threading.Thread.CurrentThread.CurrentUICulture.Name.StartsWith("en") Then
                            cultureStr = "en"
                        End If
                    End If
                End If
                Return cultureStr
            End Get
        End Property

        Public Overloads Sub InitCulture(ByVal code As String)
            Try
                ChangeUICulture(code)
            Catch ex As Exception

            End Try
        End Sub
        Public Overloads Sub InitCulture()
            Try
                ChangeUICulture(Me.CultureCode)
            Catch ex As Exception

            End Try
        End Sub

    End Class

End Namespace

