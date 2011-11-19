' Tween - Client of Twitter
' Copyright (c) 2007-2010 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
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



Public Class DoubleClickCopyCanceller
    Inherits NativeWindow
    Implements IDisposable

    Const WM_GETTEXTLENGTH As Integer = &HE
    Const WM_GETTEXT As Integer = &HD
    Const WM_LBUTTONDBLCLK As Integer = &H203
    Dim _doubleClick As Boolean = False

    Public Sub New(ByVal control As Control)
        Me.AssignHandle(control.Handle)
    End Sub

    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = WM_LBUTTONDBLCLK Then
            _doubleClick = True
        End If
        If _doubleClick Then
            If m.Msg = WM_GETTEXTLENGTH Then
                _doubleClick = False
                m.Result = CType(0, IntPtr)
                Exit Sub
            End If
        End If
        MyBase.WndProc(m)
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Me.ReleaseHandle()
        GC.SuppressFinalize(Me)
    End Sub
End Class
