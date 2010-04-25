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
    '入力補助画面
    Private _hashSupl As AtIdSupplement
    'I/F用
    Private _useHash As String = ""
    Private _isPermanent As Boolean = False
    Private _isHead As Boolean = False
    '編集モード
    Private _isAdd As Boolean = False

    Private Sub ChangeMode(ByVal isEdit As Boolean)
        Me.GroupHashtag.Enabled = Not isEdit
        Me.GroupDetail.Enabled = isEdit
        Me.TableLayoutButtons.Enabled = Not isEdit
        If isEdit Then
            Me.UseHashText.Focus()
        Else
            Me.HistoryHashList.Focus()
        End If
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub AddButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddButton.Click
        Me.UseHashText.Text = ""
        ChangeMode(True)
        _isAdd = True
    End Sub

    Private Sub EditButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EditButton.Click
        If Me.HistoryHashList.SelectedIndices.Count = 0 Then Exit Sub
        Me.UseHashText.Text = Me.HistoryHashList.SelectedItems(0).ToString()
        ChangeMode(True)
        _isAdd = False
    End Sub

    Private Sub DeleteButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteButton.Click
        If Me.HistoryHashList.SelectedIndices.Count = 0 Then Exit Sub
        If MessageBox.Show(My.Resources.DeleteHashtagsMessage1, _
                            "Delete Hashtags", _
                            MessageBoxButtons.OKCancel, _
                            MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If
        For i As Integer = 0 To HistoryHashList.SelectedIndices.Count - 1
            If UseHashText.Text = HistoryHashList.SelectedItems(0).ToString Then UseHashText.Text = ""
            HistoryHashList.Items.RemoveAt(HistoryHashList.SelectedIndices(0))
        Next
        If HistoryHashList.Items.Count > 0 Then
            HistoryHashList.SelectedIndex = 0
        End If
    End Sub

    Private Sub UnSelectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UnSelectButton.Click
        HistoryHashList.SelectedIndices.Clear()
    End Sub

    Public Sub AddHashToHistory(ByVal hash As String, ByVal isIgnorePermanent As Boolean)
        hash = hash.Trim
        If hash <> "" Then
            If isIgnorePermanent OrElse Not _isPermanent Then
                '無条件に先頭に挿入
                If HistoryHashList.Items.Contains(hash) Then HistoryHashList.Items.Remove(hash)
                HistoryHashList.Items.Insert(0, hash)
            Else
                '固定されていたら2行目に挿入
                If _isPermanent Then
                    If HistoryHashList.Items.IndexOf(hash) > 0 Then
                        '重複アイテムが2行目以降にあれば2行目へ
                        HistoryHashList.Items.Remove(hash)
                        HistoryHashList.Items.Insert(1, hash)
                    ElseIf HistoryHashList.Items.IndexOf(hash) = -1 Then
                        '重複アイテムなし
                        If HistoryHashList.Items.Count = 0 Then
                            'リストが空なら追加
                            HistoryHashList.Items.Add(hash)
                        Else
                            'リストにアイテムがあれば2行目へ
                            HistoryHashList.Items.Insert(1, hash)
                        End If
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub HashtagManage_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        'オプション
        Me.CheckPermanent.Checked = Me._isPermanent
        Me.RadioHead.Checked = Me._isHead
        Me.RadioLast.Checked = Not Me._isHead
        'リスト選択
        If Me.HistoryHashList.Items.Contains(Me._useHash) Then
            Me.HistoryHashList.SelectedItem = Me._useHash
        Else
            If Me.HistoryHashList.Items.Count > 0 Then
                Me.HistoryHashList.SelectedIndex = 0
            End If
        End If
        Me.ChangeMode(False)
    End Sub

    Public Sub New(ByVal hashSuplForm As AtIdSupplement, ByVal history() As String, ByVal permanentHash As String, ByVal IsPermanent As Boolean, ByVal IsHead As Boolean)

        ' この呼び出しは、Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後で初期化を追加します。

        _hashSupl = hashSuplForm
        HistoryHashList.Items.AddRange(history)
        _useHash = permanentHash
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
        Me.OK_Button_Click(Nothing, Nothing)
    End Sub

    Public Sub ToggleHash()
        If Me._useHash = "" Then
            If Me.HistoryHashList.Items.Count > 0 Then
                Me._useHash = Me.HistoryHashList.Items(0).ToString
            End If
        Else
            Me._useHash = ""
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

    Public Sub ClearHashtag()
        Me._useHash = ""
    End Sub

    Public Sub SetPermanentHash(ByVal hash As String)
        '固定ハッシュタグの変更
        _useHash = hash.Trim
        Me.AddHashToHistory(_useHash, False)
        Me._isPermanent = True
    End Sub

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

    Private Sub PermOK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PermOK_Button.Click
        'ハッシュタグの整形
        Dim hashStr As String = UseHashText.Text
        If Not Me.AdjustHashtags(hashStr, True) Then Exit Sub

        UseHashText.Text = hashStr
        Dim idx As Integer = 0
        If Not Me._isAdd AndAlso Me.HistoryHashList.SelectedIndices.Count > 0 Then
            idx = Me.HistoryHashList.SelectedIndices(0)
            Me.HistoryHashList.Items.RemoveAt(idx)
            Me.HistoryHashList.SelectedIndices.Clear()
            Me.HistoryHashList.Items.Insert(idx, hashStr)
            Me.HistoryHashList.SelectedIndex = idx
        Else
            Me.AddHashToHistory(hashStr, False)
            Me.HistoryHashList.SelectedIndices.Clear()
            Me.HistoryHashList.SelectedIndex = Me.HistoryHashList.Items.IndexOf(hashStr)
        End If

        ChangeMode(False)
    End Sub

    Private Sub PermCancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PermCancel_Button.Click
        If Me.HistoryHashList.Items.Count > 0 AndAlso Me.HistoryHashList.SelectedIndices.Count > 0 Then
            Me.UseHashText.Text = Me.HistoryHashList.Items(Me.HistoryHashList.SelectedIndices(0)).ToString
        Else
            Me.UseHashText.Text = ""
        End If

        ChangeMode(False)
    End Sub

    Private Sub HistoryHashList_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles HistoryHashList.KeyDown
        If e.KeyCode = Keys.Delete Then
            Me.DeleteButton_Click(Nothing, Nothing)
        ElseIf e.KeyCode = Keys.Insert Then
            Me.AddButton_Click(Nothing, Nothing)
        End If
    End Sub

    Private Function AdjustHashtags(ByRef hashtag As String, ByVal isShowWarn As Boolean) As Boolean
        'ハッシュタグの整形
        hashtag = hashtag.Trim
        If hashtag = "" Then
            If isShowWarn Then MessageBox.Show("emply hashtag.", "Hashtag warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            Return False
        End If
        hashtag = hashtag.Replace("＃", "#")
        hashtag = hashtag.Replace("　", " ")
        Dim adjust As String = ""
        For Each hash As String In hashtag.Split(" "c)
            If hash.Length > 0 Then
                If Not hash.StartsWith("#") Then
                    If isShowWarn Then MessageBox.Show("Invalid hashtag. -> " + hash, "Hashtag warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                    Return False
                End If
                If hash.Length = 1 Then
                    If isShowWarn Then MessageBox.Show("emply hashtag.", "Hashtag warning", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
                    Return False
                End If
                '使用不可の文字チェックはしない
                adjust += hash + " "
            End If
        Next
        hashtag = adjust.Trim
        Return True
    End Function

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Dim hash As String = ""
        For Each hs As String In Me.HistoryHashList.SelectedItems
            hash += hs + " "
        Next
        hash = hash.Trim
        If hash <> "" Then
            Me.AddHashToHistory(hash, True)
            Me._isPermanent = Me.CheckPermanent.Checked
        Else
            '使用ハッシュが未選択ならば、固定オプション外す
            Me._isPermanent = False
        End If
        Me._isHead = Me.RadioHead.Checked
        Me._useHash = hash

        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub HashtagManage_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.Handled = True
            If Me.GroupDetail.Enabled Then
                Me.PermOK_Button_Click(Nothing, Nothing)
            Else
                Me.OK_Button_Click(Nothing, Nothing)
            End If
        ElseIf e.KeyCode = Keys.Escape Then
            e.Handled = True
            If Me.GroupDetail.Enabled Then
                Me.PermCancel_Button_Click(Nothing, Nothing)
            Else
                Me.Cancel_Button_Click(Nothing, Nothing)
            End If
        End If
    End Sub
End Class
