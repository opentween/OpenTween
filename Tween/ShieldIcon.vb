' Tween - Client of Twitter
' Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
'           (c) 2008-2011 Moz (@syo68k)
'           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
'           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
'           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
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
Imports System


Public Class ShieldIcon


    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Unicode)> _
    Private Structure SHSTOCKICONINFO
        Public cbSize As Integer
        Public hIcon As IntPtr
        Public iSysImageIndex As Integer
        Public iIcon As Integer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=260)> _
        Public szPath As String
    End Structure

    Private Declare Function SHGetStockIconInfo Lib "shell32.dll" (ByVal siid As Integer, ByVal uFlags As UInteger, ByRef psii As SHSTOCKICONINFO) As Integer

    Private Declare Function DestroyIcon Lib "shell32.dll" (ByVal hIcon As IntPtr) As Boolean


    Const SIID_SHIELD As Integer = 77
    Const SHGFI_ICON As UInteger = &H100
    Const SHGFI_SMALLICON As UInteger = &H1


    Private icondata As Image = Nothing
    Private sii As SHSTOCKICONINFO


    Public Sub New()
        'NT6 kernelかどうか検査
        If Not IsNT6() Then
            icondata = Nothing
            Exit Sub
        End If

        Try
            sii = New SHSTOCKICONINFO
            sii.cbSize = Marshal.SizeOf(sii)
            sii.hIcon = IntPtr.Zero
            SHGetStockIconInfo(SIID_SHIELD, SHGFI_ICON Or SHGFI_SMALLICON, sii)
            icondata = Bitmap.FromHicon(sii.hIcon)
        Catch ex As Exception
            icondata = Nothing
        End Try
    End Sub

    Public Sub Dispose()
        If icondata IsNot Nothing Then
            icondata.Dispose()
        End If
    End Sub

    Public ReadOnly Property Icon() As Image
        Get
            'Return icondata
            'シールドアイコンのデータを返さないように　あとでどうにかする
            Return Nothing
        End Get
    End Property

End Class
