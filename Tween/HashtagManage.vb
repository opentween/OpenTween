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

Public Class HashtagManage

    Private _useHash As String = ""
    Private _permHash As String = ""
    Private _hashSupl As AtIdSupplement
    Private _isPermanent As Boolean = False
    Private _isHead As Boolean = False
    Private _insert As Boolean = False

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        '履歴にフォーカスがあり、アイテムを選択している場合は、固定ハッシュに設定＆フォーカス移動して終了
        If HistoryHashList.Focused AndAlso HistoryHashList.SelectedIndices.Count > 0 Then
            If Me.CheckPermanent.Checked Then
                Me.ReplaceButton_Click(Nothing, Nothing)
                UseHashText.Focus()
                Exit Sub
            Else
                Me.InsertButton_Click(Nothing, Nothing)
                Exit Sub
            End If
        End If
        'ハッシュタグの整形
        UseHashText.Text = UseHashText.Text.Trim
        UseHashText.Text = UseHashText.Text.Replace("＃", "#")
        UseHashText.Text = UseHashText.Text.Replace("　", " ")
        Dim adjust As String = ""
        For Each hash As String In UseHashText.Text.Split(" "c)
            If hash.Length > 0 Then
                If Not hash.StartsWith("#") Then
                    MessageBox.Show("Invalid hashtag. -> " + hash, "Hashtag warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                    Exit Sub
                End If
                If hash.Length = 1 Then
                    MessageBox.Show("emply hashtag.", "Hashtag warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                    Exit Sub
                End If
                '使用不可の文字チェックはしない
                adjust += hash + " "
            End If
        Next
        adjust = adjust.Trim
        UseHashText.Text = adjust
        _permHash = adjust
        Me.AddHashToHistory(_permHash)
        _useHash = ""
        If adjust = "" Then Me.CheckPermanent.Checked = False
        Me._insert = False
        Me._isPermanent = Me.CheckPermanent.Checked
        Me._isHead = Me.RadioHead.Checked
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub AddButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddButton.Click
        If Me.HistoryHashList.SelectedIndices.Count = 0 Then Exit Sub
        UseHashText.Text = UseHashText.Text.Trim + " "
        For Each hash As String In HistoryHashList.SelectedItems
            If Not UseHashText.Text.Contains(hash + " ") Then UseHashText.Text += hash + " "
        Next
        UseHashText.Text = UseHashText.Text.Trim
    End Sub

    Private Sub ReplaceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReplaceButton.Click
        If Me.HistoryHashList.SelectedIndices.Count = 0 Then Exit Sub
        UseHashText.Text = ""
        For Each hash As String In HistoryHashList.SelectedItems
            If Not UseHashText.Text.Contains(hash + " ") Then UseHashText.Text += hash + " "
        Next
        UseHashText.Text = UseHashText.Text.Trim
    End Sub

    Private Sub InsertButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InsertButton.Click
        If Me.HistoryHashList.SelectedIndex > -1 Then
            _useHash = Me.HistoryHashList.SelectedItem.ToString
            Me.AddHashToHistory(_useHash)
        Else
            _useHash = ""
        End If
        Me._insert = True
        'Me._isPermanent = Me.CheckPermanent.Checked
        'Me._isHead = Me.RadioHead.Checked
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Public Sub AddHashToHistory(ByVal hash As String)
        hash = hash.Trim
        If hash <> "" Then
            If HistoryHashList.Items.Contains(hash) Then HistoryHashList.Items.Remove(hash)
            HistoryHashList.Items.Insert(0, hash)
        End If
    End Sub

    Private Sub DeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteButton.Click
        For i As Integer = 0 To HistoryHashList.SelectedIndices.Count - 1
            If UseHashText.Text = HistoryHashList.SelectedItems(0).ToString Then UseHashText.Text = ""
            HistoryHashList.Items.RemoveAt(HistoryHashList.SelectedIndices(0))
        Next
    End Sub

    Private Sub HashtagManage_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        Me.CheckPermanent.Checked = Me._isPermanent
        Me.GroupPermanent.Enabled = Me._isPermanent
        Me.RadioHead.Checked = Me._isHead
        Me.RadioLast.Checked = Not Me._isHead
        UseHashText.Text = _permHash
        If HistoryHashList.Items.Count > 0 Then
            HistoryHashList.SelectedIndices.Clear()
            HistoryHashList.SelectedIndex = 0
        End If
        HistoryHashList.Focus()
    End Sub

    Public Sub New(ByVal hashSuplForm As AtIdSupplement, ByVal history() As String, ByVal permanentHash As String, ByVal IsPermanent As Boolean, ByVal IsHead As Boolean)

        ' この呼び出しは、Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

        _hashSupl = hashSuplForm
        HistoryHashList.Items.AddRange(history)
        _permHash = permanentHash
        _isPermanent = IsPermanent
        _isHead = IsHead
    End Sub

    Private Sub UseHashText_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles UseHashText.KeyPress
        If e.KeyChar = "#" Then
            _hashSupl.ShowDialog()
            If _hashSupl.inputText <> "" Then
                Dim fHalf As String = ""
                Dim eHalf As String = ""
                Dim selStart As Integer = UseHashText.SelectionStart
                If selStart > 0 Then
                    fHalf = UseHashText.Text.Substring(0, selStart)
                End If
                If selStart < UseHashText.Text.Length Then
                    eHalf = UseHashText.Text.Substring(selStart)
                End If
                UseHashText.Text = fHalf + _hashSupl.inputText + eHalf
                UseHashText.SelectionStart = selStart + _hashSupl.inputText.Length
            End If
            e.Handled = True
        End If
    End Sub

    Private Sub HistoryHashList_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles HistoryHashList.DoubleClick
        If CheckPermanent.Checked Then
            Me.ReplaceButton_Click(Nothing, Nothing)
        Else
            Me.InsertButton_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub CheckPermanent_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckPermanent.CheckedChanged
        Me.GroupPermanent.Enabled = Me.CheckPermanent.Checked
        Me.InsertButton.Enabled = Not Me.CheckPermanent.Checked
    End Sub

    Public Sub ToggleHash()
        If Me._isPermanent Then
            If Me._permHash <> "" Then
                Me._isPermanent = False
            Else
                If Me.HistoryHashList.Items.Count > 0 Then
                    Me._permHash = Me.HistoryHashList.Items(0).ToString
                Else
                    Me._isPermanent = False
                End If
            End If
        Else
            If Me.HistoryHashList.Items.Count > 0 Then
                Me._permHash = Me.HistoryHashList.Items(0).ToString
                Me._isPermanent = True
            Else
                Me._isPermanent = False
            End If
        End If
    End Sub

    Public ReadOnly Property HashHistories() As List(Of String)
        Get
            Dim hash As New List(Of String)
            For Each item As String In HistoryHashList.Items
                hash.Add(item)
            Next
            Return hash
        End Get
    End Property

    Public ReadOnly Property UseHash() As String
        Get
            Return _useHash
        End Get
    End Property

    Public Property PermanentHash() As String
        Get
            Return _permHash
        End Get
        Set(ByVal value As String)
            '固定ハッシュタグの変更
            _permHash = value.Trim
            UseHashText.Text = _permHash
            If _permHash <> "" Then
                Me.AddHashToHistory(_permHash)
                Me._isPermanent = True
            Else
                Me._isPermanent = False
            End If
        End Set
    End Property

    Public ReadOnly Property IsPermanent() As Boolean
        Get
            Return _isPermanent
        End Get
    End Property

    Public ReadOnly Property IsHead() As Boolean
        Get
            Return _isHead
        End Get
    End Property

    Public ReadOnly Property IsInsert() As Boolean
        Get
            Return _insert
        End Get
    End Property
End Class
