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

Imports System.Collections.Specialized

Public Class FilterDialog

    Private _mode As EDITMODE
    Private _directAdd As Boolean
    Private _sts As TabInformations
    Private _cur As String

    Private tabdialog As New TabsDialog(True)

    Private Enum EDITMODE
        AddNew
        Edit
        None
    End Enum

    Private Sub SetFilters(ByVal tabName As String)
        If ListTabs.Items.Count = 0 Then Exit Sub

        ListFilters.Items.Clear()
        ListFilters.Items.AddRange(_sts.Tabs(tabName).GetFilters())
        If ListFilters.Items.Count > 0 Then ListFilters.SelectedIndex = 0

        CheckManageRead.Checked = _sts.Tabs(tabName).UnreadManage
        CheckNotifyNew.Checked = _sts.Tabs(tabName).Notify

        Dim idx As Integer = ComboSound.Items.IndexOf(_sts.Tabs(tabName).SoundFile)
        If idx = -1 Then idx = 0
        ComboSound.SelectedIndex = idx

        If _directAdd Then Exit Sub

        ListTabs.Enabled = True
        GroupTab.Enabled = True
        ListFilters.Enabled = True
        'ListFilters.Focus()
        If ListFilters.SelectedIndex <> -1 Then
            ShowDetail()
        End If
        EditFilterGroup.Enabled = False
        Select Case TabInformations.GetInstance.Tabs(tabName).TabType
            Case TabUsageType.Home, TabUsageType.DirectMessage, TabUsageType.Favorites, TabUsageType.PublicSearch, TabUsageType.Lists
                ButtonNew.Enabled = False
                ButtonEdit.Enabled = False
                ButtonDelete.Enabled = False
            Case Else
                ButtonNew.Enabled = True
                ButtonEdit.Enabled = True
                ButtonDelete.Enabled = True
        End Select
        ButtonRenameTab.Enabled = True
        If TabInformations.GetInstance.IsDefaultTab(tabName) Then
            'ButtonRenameTab.Enabled = False
            ButtonDeleteTab.Enabled = False
        Else
            'ButtonRenameTab.Enabled = True
            ButtonDeleteTab.Enabled = True
        End If
        ButtonClose.Enabled = True
    End Sub

    Public Sub SetCurrent(ByVal TabName As String)
        _cur = TabName
    End Sub

    Public Sub AddNewFilter(ByVal id As String, ByVal msg As String)
        '元フォームから直接呼ばれる
        ButtonNew.Enabled = False
        ButtonEdit.Enabled = False
        ButtonDelete.Enabled = False
        ButtonClose.Enabled = False
        EditFilterGroup.Enabled = True
        ListTabs.Enabled = False
        GroupTab.Enabled = False
        ListFilters.Enabled = False

        RadioAND.Checked = True
        RadioPLUS.Checked = False
        UID.Text = id
        UID.SelectAll()
        MSG1.Text = msg
        MSG1.SelectAll()
        MSG2.Text = id + msg
        MSG2.SelectAll()
        TextSource.Text = ""
        UID.Enabled = True
        MSG1.Enabled = True
        MSG2.Enabled = False
        CheckRegex.Checked = False
        CheckURL.Checked = False
        CheckCaseSensitive.Checked = False
        CheckRetweet.Checked = False

        RadioExAnd.Checked = True
        RadioExPLUS.Checked = False
        ExUID.Text = ""
        ExUID.SelectAll()
        ExMSG1.Text = ""
        ExMSG1.SelectAll()
        ExMSG2.Text = ""
        ExMSG2.SelectAll()
        TextExSource.Text = ""
        ExUID.Enabled = True
        ExMSG1.Enabled = True
        ExMSG2.Enabled = False
        CheckExRegex.Checked = False
        CheckExURL.Checked = False
        CheckExCaseSensitive.Checked = False
        CheckExRetweet.Checked = False

        'OptNone.Checked = True
        OptCopy.Checked = True
        CheckMark.Checked = True
        UID.Focus()
        _mode = EDITMODE.AddNew
        _directAdd = True
    End Sub

    Private Sub ButtonNew_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonNew.Click
        ButtonNew.Enabled = False
        ButtonEdit.Enabled = False
        ButtonDelete.Enabled = False
        ButtonClose.Enabled = False
        EditFilterGroup.Enabled = True
        ListTabs.Enabled = False
        GroupTab.Enabled = False
        ListFilters.Enabled = False

        RadioAND.Checked = True
        RadioPLUS.Checked = False
        UID.Text = ""
        MSG1.Text = ""
        MSG2.Text = ""
        TextSource.Text = ""
        UID.Enabled = True
        MSG1.Enabled = True
        MSG2.Enabled = False
        CheckRegex.Checked = False
        CheckURL.Checked = False
        CheckCaseSensitive.Checked = False
        CheckRetweet.Checked = False

        RadioExAnd.Checked = True
        RadioExPLUS.Checked = False
        ExUID.Text = ""
        ExMSG1.Text = ""
        ExMSG2.Text = ""
        TextExSource.Text = ""
        ExUID.Enabled = True
        ExMSG1.Enabled = True
        ExMSG2.Enabled = False
        CheckExRegex.Checked = False
        CheckExURL.Checked = False
        CheckExCaseSensitive.Checked = False
        CheckExRetweet.Checked = False

        'OptNone.Checked = True
        OptCopy.Checked = True
        CheckMark.Checked = True
        UID.Focus()
        _mode = EDITMODE.AddNew
    End Sub

    Private Sub ButtonEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonEdit.Click
        If ListFilters.SelectedIndex = -1 Then Exit Sub

        ButtonNew.Enabled = False
        ButtonEdit.Enabled = False
        ButtonDelete.Enabled = False
        ButtonClose.Enabled = False
        EditFilterGroup.Enabled = True
        ListTabs.Enabled = False
        GroupTab.Enabled = False
        ListFilters.Enabled = False

        ShowDetail()
        _mode = EDITMODE.Edit
    End Sub

    Private Sub ButtonDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonDelete.Click
        If ListFilters.SelectedIndex = -1 Then Exit Sub
        Dim tmp As String = String.Format(My.Resources.ButtonDelete_ClickText1, vbCrLf, ListFilters.SelectedItem.ToString)

        If MessageBox.Show(tmp, My.Resources.ButtonDelete_ClickText2, _
            MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Cancel Then Exit Sub

        Dim i As Integer = ListFilters.SelectedIndex

        _sts.Tabs(ListTabs.SelectedItem.ToString()).RemoveFilter(DirectCast(ListFilters.SelectedItem, FiltersClass))
        ListFilters.Items.RemoveAt(i)
    End Sub

    Private Sub ButtonCancel_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonCancel.Click
        ListTabs.Enabled = True
        GroupTab.Enabled = True
        ListFilters.Enabled = True
        ListFilters.Focus()
        If ListFilters.SelectedIndex <> -1 Then
            ShowDetail()
        End If
        EditFilterGroup.Enabled = False
        ButtonNew.Enabled = True
        ButtonEdit.Enabled = True
        ButtonDelete.Enabled = True
        ButtonClose.Enabled = True
        If _directAdd Then
            Me.Close()
        End If
    End Sub

    Private Sub ShowDetail()

        If _directAdd Then Exit Sub

        If ListFilters.SelectedIndex > -1 Then
            Dim fc As FiltersClass = DirectCast(ListFilters.SelectedItem, FiltersClass)
            If fc.SearchBoth Then
                RadioAND.Checked = True
                RadioPLUS.Checked = False
                UID.Enabled = True
                MSG1.Enabled = True
                MSG2.Enabled = False
                UID.Text = fc.NameFilter
                UID.SelectAll()
                MSG1.Text = ""
                MSG2.Text = ""
                For Each bf As String In fc.BodyFilter
                    MSG1.Text += bf + " "
                Next
                MSG1.Text = MSG1.Text.Trim
                MSG1.SelectAll()
            Else
                RadioPLUS.Checked = True
                RadioAND.Checked = False
                UID.Enabled = False
                MSG1.Enabled = False
                MSG2.Enabled = True
                UID.Text = ""
                MSG1.Text = ""
                MSG2.Text = ""
                For Each bf As String In fc.BodyFilter
                    MSG2.Text += bf + " "
                Next
                MSG2.Text = MSG2.Text.Trim
                MSG2.SelectAll()
            End If
            TextSource.Text = fc.Source
            CheckRegex.Checked = fc.UseRegex
            CheckURL.Checked = fc.SearchUrl
            CheckCaseSensitive.Checked = fc.CaseSensitive
            CheckRetweet.Checked = fc.IsRt

            If fc.ExSearchBoth Then
                RadioExAnd.Checked = True
                RadioExPLUS.Checked = False
                ExUID.Enabled = True
                ExMSG1.Enabled = True
                ExMSG2.Enabled = False
                ExUID.Text = fc.ExNameFilter
                ExUID.SelectAll()
                ExMSG1.Text = ""
                ExMSG2.Text = ""
                For Each bf As String In fc.ExBodyFilter
                    ExMSG1.Text += bf + " "
                Next
                ExMSG1.Text = ExMSG1.Text.Trim
                ExMSG1.SelectAll()
            Else
                RadioExPLUS.Checked = True
                RadioExAnd.Checked = False
                ExUID.Enabled = False
                ExMSG1.Enabled = False
                ExMSG2.Enabled = True
                ExUID.Text = ""
                ExMSG1.Text = ""
                ExMSG2.Text = ""
                For Each bf As String In fc.ExBodyFilter
                    ExMSG2.Text += bf + " "
                Next
                ExMSG2.Text = ExMSG2.Text.Trim
                ExMSG2.SelectAll()
            End If
            TextExSource.Text = fc.ExSource
            CheckExRegex.Checked = fc.ExUseRegex
            CheckExURL.Checked = fc.ExSearchUrl
            CheckExCaseSensitive.Checked = fc.ExCaseSensitive
            CheckExRetweet.Checked = fc.IsExRt

            'If fc.moveFrom Then
            '    OptMove.Checked = True
            'ElseIf fc.SetMark Then
            '    OptMark.Checked = True
            'Else
            '    OptNone.Checked = True
            'End If
            If fc.MoveFrom Then
                OptMove.Checked = True
            Else
                OptCopy.Checked = True
            End If
            CheckMark.Checked = fc.SetMark
        Else
            RadioAND.Checked = True
            RadioPLUS.Checked = False
            UID.Enabled = True
            MSG1.Enabled = True
            MSG2.Enabled = False
            UID.Text = ""
            MSG1.Text = ""
            MSG2.Text = ""
            TextSource.Text = ""
            CheckRegex.Checked = False
            CheckURL.Checked = False
            CheckCaseSensitive.Checked = False
            CheckRetweet.Checked = False

            RadioExAnd.Checked = True
            RadioExPLUS.Checked = False
            ExUID.Enabled = True
            ExMSG1.Enabled = True
            ExMSG2.Enabled = False
            ExUID.Text = ""
            ExMSG1.Text = ""
            ExMSG2.Text = ""
            TextExSource.Text = ""
            CheckExRegex.Checked = False
            CheckExURL.Checked = False
            CheckExCaseSensitive.Checked = False
            CheckExRetweet.Checked = False

            OptCopy.Checked = True
            CheckMark.Checked = True
        End If
    End Sub

    Private Sub RadioAND_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioAND.CheckedChanged
        Dim flg As Boolean = RadioAND.Checked
        UID.Enabled = flg
        MSG1.Enabled = flg
        MSG2.Enabled = Not flg
    End Sub

    Private Sub ButtonOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonOK.Click
        Dim isBlankMatch As Boolean = False
        Dim isBlankExclude As Boolean = False

        '入力チェック
        If Not CheckMatchRule(isBlankMatch) OrElse _
           Not CheckExcludeRule(isBlankExclude) Then
            Exit Sub
        End If
        If isBlankMatch AndAlso isBlankExclude Then
            MessageBox.Show(My.Resources.ButtonOK_ClickText1, My.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Exit Sub
        End If

        Dim i As Integer = ListFilters.SelectedIndex
        Dim ft As FiltersClass

        ft = New FiltersClass()

        ft.MoveFrom = OptMove.Checked
        ft.SetMark = CheckMark.Checked

        Dim bdy As String = ""
        If RadioAND.Checked Then
            ft.NameFilter = UID.Text
            ft.SearchBoth = True
            bdy = MSG1.Text
        Else
            ft.NameFilter = ""
            ft.SearchBoth = False
            bdy = MSG2.Text
        End If
        ft.Source = TextSource.Text.Trim

        If CheckRegex.Checked Then
            ft.BodyFilter.Add(bdy)
        Else
            Dim bf() As String = bdy.Trim.Split(Chr(32))
            For Each bfs As String In bf
                If bfs <> "" Then ft.BodyFilter.Add(bfs.Trim)
            Next
        End If

        ft.UseRegex = CheckRegex.Checked
        ft.SearchUrl = CheckURL.Checked
        ft.CaseSensitive = CheckCaseSensitive.Checked
        ft.IsRt = CheckRetweet.Checked

        bdy = ""
        If RadioExAnd.Checked Then
            ft.ExNameFilter = ExUID.Text
            ft.ExSearchBoth = True
            bdy = ExMSG1.Text
        Else
            ft.ExNameFilter = ""
            ft.ExSearchBoth = False
            bdy = ExMSG2.Text
        End If
        ft.ExSource = TextExSource.Text.Trim

        If CheckExRegex.Checked Then
            ft.ExBodyFilter.Add(bdy)
        Else
            Dim bf() As String = bdy.Trim.Split(Chr(32))
            For Each bfs As String In bf
                If bfs <> "" Then ft.ExBodyFilter.Add(bfs.Trim)
            Next
        End If

        ft.ExUseRegex = CheckExRegex.Checked
        ft.ExSearchUrl = CheckExURL.Checked
        ft.ExCaseSensitive = CheckExCaseSensitive.Checked
        ft.IsExRt = CheckExRetweet.Checked

        If _mode = EDITMODE.AddNew Then
            If Not _sts.Tabs(ListTabs.SelectedItem.ToString()).AddFilter(ft) Then
                MessageBox.Show(My.Resources.ButtonOK_ClickText4, My.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            _sts.Tabs(ListTabs.SelectedItem.ToString()).EditFilter(DirectCast(ListFilters.SelectedItem, FiltersClass), ft)
        End If

        SetFilters(ListTabs.SelectedItem.ToString)
        If _mode = EDITMODE.AddNew Then
            ListFilters.SelectedIndex = ListFilters.Items.Count - 1
        Else
            ListFilters.SelectedIndex = i
        End If
        _mode = EDITMODE.None


        If _directAdd Then
            Me.Close()
        End If
    End Sub

    Private Function CheckMatchRule(ByRef isBlank As Boolean) As Boolean
        isBlank = False
        TextSource.Text = TextSource.Text.Trim()
        If RadioAND.Checked Then
            MSG1.Text = MSG1.Text.Trim
            UID.Text = UID.Text.Trim()
            If Not CheckRegex.Checked Then MSG1.Text = MSG1.Text.Replace("　", " ")

            If UID.Text = "" AndAlso MSG1.Text = "" AndAlso TextSource.Text = "" AndAlso CheckRetweet.Checked = False Then
                isBlank = True
                Return True
            End If
            If CheckRegex.Checked Then
                If UID.Text <> "" Then
                    Try
                        Dim rgx As New System.Text.RegularExpressions.Regex(UID.Text)
                    Catch ex As Exception
                        MessageBox.Show(My.Resources.ButtonOK_ClickText3 + ex.Message, My.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Return False
                    End Try
                End If
                If MSG1.Text <> "" Then
                    Try
                        Dim rgx As New System.Text.RegularExpressions.Regex(MSG1.Text)
                    Catch ex As Exception
                        MessageBox.Show(My.Resources.ButtonOK_ClickText3 + ex.Message, My.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Return False
                    End Try
                End If
            End If
        Else
            MSG2.Text = MSG2.Text.Trim
            If Not CheckRegex.Checked Then MSG2.Text = MSG2.Text.Replace("　", " ")
            If MSG2.Text = "" AndAlso TextSource.Text = "" AndAlso CheckRetweet.Checked = False Then
                isBlank = True
                Return True
            End If
            If CheckRegex.Checked AndAlso MSG2.Text <> "" Then
                Try
                    Dim rgx As New System.Text.RegularExpressions.Regex(MSG2.Text)
                Catch ex As Exception
                    MessageBox.Show(My.Resources.ButtonOK_ClickText3 + ex.Message, My.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Return False
                End Try
            End If
        End If

        If CheckRegex.Checked AndAlso TextSource.Text <> "" Then
            Try
                Dim rgx As New System.Text.RegularExpressions.Regex(TextSource.Text)
            Catch ex As Exception
                MessageBox.Show(My.Resources.ButtonOK_ClickText3 + ex.Message, My.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Return False
            End Try
        End If
        Return True
    End Function

    Private Function CheckExcludeRule(ByRef isBlank As Boolean) As Boolean
        isBlank = False
        TextExSource.Text = TextExSource.Text.Trim
        If RadioExAnd.Checked Then
            ExMSG1.Text = ExMSG1.Text.Trim
            If Not CheckExRegex.Checked Then ExMSG1.Text = ExMSG1.Text.Replace("　", " ")
            ExUID.Text = ExUID.Text.Trim()
            If ExUID.Text = "" AndAlso ExMSG1.Text = "" AndAlso TextExSource.Text = "" AndAlso CheckExRetweet.Checked = False Then
                isBlank = True
                Return True
            End If
            If CheckExRegex.Checked Then
                If ExUID.Text <> "" Then
                    Try
                        Dim rgx As New System.Text.RegularExpressions.Regex(ExUID.Text)
                    Catch ex As Exception
                        MessageBox.Show(My.Resources.ButtonOK_ClickText3 + ex.Message, My.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Return False
                    End Try
                End If
                If ExMSG1.Text <> "" Then
                    Try
                        Dim rgx As New System.Text.RegularExpressions.Regex(ExMSG1.Text)
                    Catch ex As Exception
                        MessageBox.Show(My.Resources.ButtonOK_ClickText3 + ex.Message, My.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                        Return False
                    End Try
                End If
            End If
        Else
            ExMSG2.Text = ExMSG2.Text.Trim
            If Not CheckExRegex.Checked Then ExMSG2.Text = ExMSG2.Text.Replace("　", " ")
            If ExMSG2.Text = "" AndAlso TextExSource.Text = "" AndAlso CheckExRetweet.Checked = False Then
                isBlank = True
                Return True
            End If
            If CheckExRegex.Checked AndAlso ExMSG2.Text <> "" Then
                Try
                    Dim rgx As New System.Text.RegularExpressions.Regex(ExMSG2.Text)
                Catch ex As Exception
                    MessageBox.Show(My.Resources.ButtonOK_ClickText3 + ex.Message, My.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Return False
                End Try
            End If
        End If

        If CheckExRegex.Checked AndAlso TextExSource.Text <> "" Then
            Try
                Dim rgx As New System.Text.RegularExpressions.Regex(TextExSource.Text)
            Catch ex As Exception
                MessageBox.Show(My.Resources.ButtonOK_ClickText3 + ex.Message, My.Resources.ButtonOK_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Return False
            End Try
        End If

        Return True
    End Function

    Private Sub ListFilters_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListFilters.SelectedIndexChanged
        ShowDetail()
    End Sub

    Private Sub ButtonClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonClose.Click
        Me.Close()
    End Sub

    Private Sub FilterDialog_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        _directAdd = False
    End Sub

    Private Sub FilterDialog_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Enter Then
            If EditFilterGroup.Enabled Then
                ButtonOK_Click(Nothing, Nothing)
            End If
        End If
        If e.KeyCode = Keys.Escape Then
            If EditFilterGroup.Enabled Then
                ButtonCancel_Click(Nothing, Nothing)
            Else
                ButtonClose_Click(Nothing, Nothing)
            End If
        End If
    End Sub

    Private Sub ListFilters_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListFilters.DoubleClick
        If ListFilters.SelectedItem Is Nothing Then
            Exit Sub
        End If

        If ListFilters.IndexFromPoint(ListFilters.PointToClient(Control.MousePosition)) = ListBox.NoMatches Then
            Exit Sub
        End If

        If ListFilters.Items(ListFilters.IndexFromPoint(ListFilters.PointToClient(Control.MousePosition))) Is Nothing Then
            Exit Sub
        End If
        ButtonEdit_Click(sender, e)
    End Sub

    Private Sub FilterDialog_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        _sts = TabInformations.GetInstance()
        ListTabs.Items.Clear()
        tabdialog.ClearTab()
        For Each key As String In _sts.Tabs.Keys
            ListTabs.Items.Add(key)

            Select Case TabInformations.GetInstance.Tabs(key).TabType
                Case TabUsageType.Home, TabUsageType.DirectMessage, TabUsageType.Favorites, TabUsageType.PublicSearch, TabUsageType.Lists
                    Exit Select
                Case Else
                    tabdialog.AddTab(key)
            End Select
        Next

        ComboSound.Items.Clear()
        ComboSound.Items.Add("")
        Dim oDir As IO.DirectoryInfo = New IO.DirectoryInfo(My.Application.Info.DirectoryPath)
        If IO.Directory.Exists(IO.Path.Combine(My.Application.Info.DirectoryPath, "Sounds")) Then
            oDir = oDir.GetDirectories("Sounds")(0)
        End If
        For Each oFile As IO.FileInfo In oDir.GetFiles("*.wav")
            ComboSound.Items.Add(oFile.Name)
        Next

        '選択タブ変更
        If ListTabs.Items.Count > 0 Then
            If _cur.Length > 0 Then
                For i As Integer = 0 To ListTabs.Items.Count - 1
                    If _cur = ListTabs.Items(i).ToString() Then
                        ListTabs.SelectedIndex = i
                        Exit For
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub ListTabs_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListTabs.SelectedIndexChanged
        If ListTabs.SelectedIndex > -1 Then
            SetFilters(ListTabs.SelectedItem.ToString)
        Else
            ListFilters.Items.Clear()
        End If
    End Sub

    Private Sub ButtonAddTab_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonAddTab.Click
        Dim tabName As String = Nothing
        Dim tabType As TabUsageType
        Using inputName As New InputTabName()
            inputName.TabName = _sts.GetUniqueTabName
            inputName.IsShowUsage = True
            inputName.ShowDialog()
            tabName = inputName.TabName
            tabType = inputName.Usage
        End Using
        If tabName <> "" Then
            'List対応
            Dim list As ListElement = Nothing
            If tabType = TabUsageType.Lists Then
                Dim rslt As String = DirectCast(Me.Owner, TweenMain).TwitterInstance.GetListsApi()
                If rslt <> "" Then
                    MessageBox.Show("Failed to get lists. (" + rslt + ")")
                End If
                Using listAvail As New ListAvailable
                    If listAvail.ShowDialog(Me) = Windows.Forms.DialogResult.Cancel Then Exit Sub
                    If listAvail.SelectedList Is Nothing Then Exit Sub
                    list = listAvail.SelectedList
                End Using
            End If
            If Not DirectCast(Me.Owner, TweenMain).AddNewTab(tabName, False, tabType) Then
                Dim tmp As String = String.Format(My.Resources.AddTabMenuItem_ClickText1, tabName)
                MessageBox.Show(tmp, My.Resources.AddTabMenuItem_ClickText2, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Exit Sub
            Else
                '成功
                _sts.AddTab(tabName, tabType, list)
                ListTabs.Items.Add(tabName)
            End If
        End If
    End Sub

    Private Sub ButtonDeleteTab_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonDeleteTab.Click
        If ListTabs.SelectedIndex > -1 AndAlso ListTabs.SelectedItem.ToString <> "" Then
            Dim tb As String = ListTabs.SelectedItem.ToString
            Dim idx As Integer = ListTabs.SelectedIndex
            If DirectCast(Me.Owner, TweenMain).RemoveSpecifiedTab(tb) Then
                ListTabs.Items.RemoveAt(idx)
                idx -= 1
                If idx < 0 Then idx = 0
                ListTabs.SelectedIndex = idx
            End If
        End If
    End Sub

    Private Sub ButtonRenameTab_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonRenameTab.Click
        If ListTabs.SelectedIndex > -1 AndAlso ListTabs.SelectedItem.ToString <> "" Then
            Dim tb As String = ListTabs.SelectedItem.ToString
            Dim idx As Integer = ListTabs.SelectedIndex
            If DirectCast(Me.Owner, TweenMain).TabRename(tb) Then
                ListTabs.Items.RemoveAt(idx)
                ListTabs.Items.Insert(idx, tb)
                ListTabs.SelectedIndex = idx
            End If
        End If
    End Sub

    Private Sub CheckManageRead_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckManageRead.CheckedChanged
        If ListTabs.SelectedIndex > -1 AndAlso ListTabs.SelectedItem.ToString <> "" Then
            DirectCast(Me.Owner, TweenMain).ChangeTabUnreadManage( _
                ListTabs.SelectedItem.ToString, _
                CheckManageRead.Checked)
        End If
    End Sub

    Private Sub ButtonUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonUp.Click
        If ListTabs.SelectedIndex > 0 AndAlso ListTabs.SelectedItem.ToString <> "" Then
            Dim selName As String = ListTabs.SelectedItem.ToString
            Dim tgtName As String = ListTabs.Items(ListTabs.SelectedIndex - 1).ToString
            DirectCast(Me.Owner, TweenMain).ReOrderTab( _
                selName, _
                tgtName, _
                True)
            Dim idx As Integer = ListTabs.SelectedIndex
            ListTabs.Items.RemoveAt(idx - 1)
            ListTabs.Items.Insert(idx, tgtName)
        End If
    End Sub

    Private Sub ButtonDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonDown.Click
        If ListTabs.SelectedIndex > -1 AndAlso ListTabs.SelectedIndex < ListTabs.Items.Count - 1 AndAlso Not String.IsNullOrEmpty(ListTabs.SelectedItem.ToString) Then
            Dim selName As String = ListTabs.SelectedItem.ToString
            Dim tgtName As String = ListTabs.Items(ListTabs.SelectedIndex + 1).ToString
            DirectCast(Me.Owner, TweenMain).ReOrderTab( _
                selName, _
                tgtName, _
                False)
            Dim idx As Integer = ListTabs.SelectedIndex
            ListTabs.Items.RemoveAt(idx + 1)
            ListTabs.Items.Insert(idx, tgtName)
        End If
    End Sub

    Private Sub CheckNotifyNew_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckNotifyNew.CheckedChanged
        If ListTabs.SelectedIndex > -1 AndAlso ListTabs.SelectedItem.ToString <> "" Then
            _sts.Tabs(ListTabs.SelectedItem.ToString).Notify = CheckNotifyNew.Checked
        End If
    End Sub

    Private Sub ComboSound_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboSound.SelectedIndexChanged
        If ListTabs.SelectedIndex > -1 AndAlso ListTabs.SelectedItem.ToString <> "" Then
            Dim filename As String = ""
            If ComboSound.SelectedIndex > -1 Then filename = ComboSound.SelectedItem.ToString
            _sts.Tabs(ListTabs.SelectedItem.ToString).SoundFile = filename
        End If
    End Sub

    Private Sub RadioExAnd_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadioExAnd.CheckedChanged
        Dim flg As Boolean = RadioExAnd.Checked
        ExUID.Enabled = flg
        ExMSG1.Enabled = flg
        ExMSG2.Enabled = Not flg
    End Sub

    Private Sub OptMove_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OptMove.CheckedChanged
        CheckMark.Enabled = Not OptMove.Checked
    End Sub

    Private Sub ButtonRuleUp_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonRuleUp.Click
        If ListTabs.SelectedIndex > -1 AndAlso ListFilters.SelectedItem IsNot Nothing AndAlso ListFilters.SelectedIndex > 0 Then
            Dim tabname As String = ListTabs.SelectedItem.ToString
            Dim selected As FiltersClass = _sts.Tabs(tabname).Filters(ListFilters.SelectedIndex)
            Dim target As FiltersClass = _sts.Tabs(tabname).Filters(ListFilters.SelectedIndex - 1)
            Dim idx As Integer = ListFilters.SelectedIndex
            ListFilters.Items.RemoveAt(idx - 1)
            ListFilters.Items.Insert(idx, target)
            _sts.Tabs(tabname).Filters.RemoveAt(idx - 1)
            _sts.Tabs(tabname).Filters.Insert(idx, target)
        End If
    End Sub

    Private Sub ButtonRuleDown_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonRuleDown.Click
        If ListTabs.SelectedIndex > -1 AndAlso ListFilters.SelectedItem IsNot Nothing AndAlso ListFilters.SelectedIndex < ListFilters.Items.Count - 1 Then
            Dim tabname As String = ListTabs.SelectedItem.ToString
            Dim selected As FiltersClass = _sts.Tabs(tabname).Filters(ListFilters.SelectedIndex)
            Dim target As FiltersClass = _sts.Tabs(tabname).Filters(ListFilters.SelectedIndex + 1)
            Dim idx As Integer = ListFilters.SelectedIndex
            ListFilters.Items.RemoveAt(idx + 1)
            ListFilters.Items.Insert(idx, target)
            _sts.Tabs(tabname).Filters.RemoveAt(idx + 1)
            _sts.Tabs(tabname).Filters.Insert(idx, target)
        End If
    End Sub

    Private Sub ButtonRuleCopy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonRuleCopy.Click
        tabdialog.Text = "ルールコピー先タブの選択"
        If tabdialog.ShowDialog = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If
        Dim tabname As String = ListTabs.SelectedItem.ToString
        Dim tabs As StringCollection = tabdialog.SelectedTabNames
        Dim filters As New Generic.List(Of FiltersClass)

        For Each idx As Integer In ListFilters.SelectedIndices
            filters.Add(_sts.Tabs(tabname).Filters(idx))
        Next
        For Each tb As String In tabs
            _sts.Tabs(tb).Filters.AddRange(filters)
        Next
    End Sub

    Private Sub ButtonRuleMove_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonRuleMove.Click
        tabdialog.Text = "ルール移動先タブの選択"
        If tabdialog.ShowDialog = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If
        Dim tabname As String = ListTabs.SelectedItem.ToString
        Dim tabs As StringCollection = tabdialog.SelectedTabNames
        Dim filters As New Generic.List(Of FiltersClass)

        ''' TODO ルール移動処理
    End Sub
End Class
