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

Imports System.Windows.Forms

Public Class TabsDialog

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub TabsDialog_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If TabList.SelectedIndex = -1 Then TabList.SelectedIndex = 0
    End Sub

    Public Sub AddTab(ByVal tabName As String)
        For Each obj As String In TabList.Items
            If obj = tabName Then Exit Sub
        Next
        TabList.Items.Add(tabName)
    End Sub

    Public Sub RemoveTab(ByVal tabName As String)
        For i As Integer = 0 To TabList.Items.Count - 1
            If CType(TabList.Items.Item(i), String) = tabName Then
                TabList.Items.RemoveAt(i)
                Exit Sub
            End If
        Next
    End Sub

    Public ReadOnly Property SelectedTabName() As String
        Get
            If TabList.SelectedIndex = -1 Then
                Return ""
            Else
                Return CStr(TabList.SelectedItem)
            End If
        End Get
    End Property

    Private Sub TabList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TabList.SelectedIndexChanged

    End Sub

    Private Sub TabList_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TabList.DoubleClick
        If TabList.SelectedItem Is Nothing Then
            Exit Sub
        End If

        If TabList.IndexFromPoint(TabList.PointToClient(Control.MousePosition)) = ListBox.NoMatches Then
            Exit Sub
        End If

        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub TabsDialog_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        TabList.Focus()
    End Sub
End Class
