﻿' Tween - Client of Twitter
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

Public Class AtIdSupplement

    Public inputText As String = ""
    Public isBack As Boolean = False
    Private startChar As String = ""
    '    Private tabkeyFix As Boolean = False

    Private _StartsWith As String = ""

    Public Sub AddItem(ByVal id As String)
        If Not Me.TextId.AutoCompleteCustomSource.Contains(id) Then
            Me.TextId.AutoCompleteCustomSource.Add(id)
        End If
    End Sub

    Public Sub AddRangeItem(ByVal ids As String())
        For Each id As String In ids
            Me.AddItem(id)
        Next
    End Sub

    Public Function GetItemList() As List(Of String)
        Dim ids As New List(Of String)
        For i As Integer = 0 To Me.TextId.AutoCompleteCustomSource.Count - 1
            ids.Add(Me.TextId.AutoCompleteCustomSource(i))
        Next
        Return ids
    End Function

    Public ReadOnly Property ItemCount() As Integer
        Get
            Return Me.TextId.AutoCompleteCustomSource.Count
        End Get
    End Property

    Private Sub ButtonOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonOK.Click
        inputText = Me.TextId.Text
        isBack = False
    End Sub

    Private Sub ButtonCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCancel.Click
        inputText = ""
        isBack = False
    End Sub

    Private Sub TextId_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles TextId.KeyDown
        If e.KeyCode = Keys.Back AndAlso Me.TextId.Text = "" Then
            inputText = ""
            isBack = True
            Me.Close()
        End If
        If e.KeyCode = Keys.Space OrElse e.KeyCode = Keys.Tab Then
            inputText = Me.TextId.Text + " "
            isBack = False
            Me.Close()
        End If
        If e.Control AndAlso e.KeyCode = Keys.Delete Then
            If Me.TextId.Text <> "" Then
                Dim idx As Integer = Me.TextId.AutoCompleteCustomSource.IndexOf(Me.TextId.Text)
                If idx > -1 Then
                    Me.TextId.Text = ""
                    Me.TextId.AutoCompleteCustomSource.RemoveAt(idx)
                End If
            End If
        End If
    End Sub

    Private Sub AtIdSupplement_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If startChar = "#" Then
            Me.ClientSize = New Size(Me.TextId.Width, Me.TextId.Height) 'プロパティで切り替えできるように
            Me.TextId.ImeMode = Windows.Forms.ImeMode.Inherit
        End If
    End Sub

    Private Sub AtIdSupplement_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        TextId.Text = startChar
        If Not String.IsNullOrEmpty(_StartsWith) Then
            TextId.Text += _StartsWith.Substring(0, _StartsWith.Length)
        End If
        TextId.SelectionStart = TextId.Text.Length
        TextId.Focus()
    End Sub

    Public Sub New()

        ' この呼び出しは、Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

    End Sub

    Public Sub New(ByVal ItemList As List(Of String), ByVal startCharacter As String)

        ' この呼び出しは、Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

        For i As Integer = 0 To ItemList.Count - 1
            Me.TextId.AutoCompleteCustomSource.Add(ItemList(i))
        Next
        startChar = startCharacter
    End Sub

    Private Sub TextId_PreviewKeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles TextId.PreviewKeyDown
        If e.KeyCode = Keys.Tab Then
            inputText = Me.TextId.Text + " "
            isBack = False
            Me.Close()
        End If
    End Sub

    Public Property StartsWith() As String
        Get
            Return _StartsWith
        End Get
        Set(ByVal value As String)
            _StartsWith = value
        End Set
    End Property

    Private Sub AtIdSupplement_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        _StartsWith = ""
        If isBack Then
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Else
            Me.DialogResult = Windows.Forms.DialogResult.OK
        End If
    End Sub
End Class
