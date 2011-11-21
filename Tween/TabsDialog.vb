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

Imports System.Windows.Forms
Imports System.Collections.Specialized

Public Class TabsDialog

    Private _multiSelect As Boolean = False
    Private _newtabItem As String = My.Resources.AddNewTabText1

    Public Sub New()

        ' この呼び出しはデザイナーで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

    End Sub


    Public Sub New(ByVal multiselect As Boolean)

        ' この呼び出しはデザイナーで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

        Me.MultiSelect = True

    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub TabsDialog_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If _multiSelect Then
            TabList.SelectedIndex = -1
        Else
            If TabList.SelectedIndex = -1 Then TabList.SelectedIndex = 0
        End If
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

    Public Sub ClearTab()
        Dim startidx As Integer = 1

        If _multiSelect Then
            startidx = 0
        End If
        For i As Integer = startidx To TabList.Items.Count - 1
            TabList.Items.RemoveAt(0)
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

    Public ReadOnly Property SelectedTabNames() As StringCollection
        Get
            If TabList.SelectedIndex = -1 Then
                Return Nothing
            Else
                Dim ret As New StringCollection
                For Each selitem As Object In TabList.SelectedItems
                    ret.Add(CStr(selitem))
                Next
                Return ret
            End If
        End Get
    End Property

    Public Property MultiSelect() As Boolean
        Get
            Return _multiSelect
        End Get
        Set(ByVal value As Boolean)
            _multiSelect = value
            If value Then
                Me.TabList.SelectionMode = SelectionMode.MultiExtended
                If Me.TabList.Items(0).ToString = My.Resources.AddNewTabText1 Then
                    Me.TabList.Items.RemoveAt(0)
                End If
            Else
                Me.TabList.SelectionMode = SelectionMode.One
                If Me.TabList.Items(0).ToString <> My.Resources.AddNewTabText1 Then
                    Me.TabList.Items.Insert(0, My.Resources.AddNewTabText1)
                End If
            End If
        End Set
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
