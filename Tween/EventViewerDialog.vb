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
Imports System.Linq
Imports System.Text.RegularExpressions
Imports System
Imports System.Runtime.CompilerServices

Public Class EventViewerDialog
    Public Property EventSource As List(Of Twitter.FormattedEvent)

    Private _filterdEventSource() As Twitter.FormattedEvent

    Private _ItemCache() As ListViewItem = Nothing
    Private _itemCacheIndex As Integer

    Private _curTab As TabPage = Nothing

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Function CreateListViewItem(ByVal source As Twitter.FormattedEvent) As ListViewItem
        Dim s() As String = {source.CreatedAt.ToString, source.Event.ToUpper, source.Username, source.Target}
        Return New ListViewItem(s)
    End Function

    Private Sub EventViewerDialog_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        EventList.BeginUpdate()
        _curTab = TabEventType.SelectedTab
        CreateFilterdEventSource()
        EventList.EndUpdate()
    End Sub

    Private Sub EventList_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EventList.DoubleClick
        If _filterdEventSource(EventList.SelectedIndices(0)) IsNot Nothing Then
            TweenMain.OpenUriAsync("http://twitter.com/" + EventSource(EventList.SelectedIndices(0)).Username)
        End If
    End Sub

    Private Function ParseEventTypeFromTag() As EVENTTYPE
        Return DirectCast([Enum].Parse(GetType(EVENTTYPE), _curTab.Tag.ToString()), EVENTTYPE)
    End Function

    Private Function IsFilterMatch(ByVal x As Twitter.FormattedEvent) As Boolean
        If Not CheckBoxFilter.Checked OrElse String.IsNullOrEmpty(TextBoxKeyword.Text) Then
            Return True
        Else
            If CheckRegex.Checked Then
                Try
                    Dim rx As New Regex(TextBoxKeyword.Text)
                    Return rx.Match(x.Username).Success OrElse rx.Match(x.Target).Success
                Catch ex As Exception
                    MessageBox.Show(My.Resources.ButtonOK_ClickText3 + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                    Return False
                End Try
            Else
                Return x.Username.Contains(TextBoxKeyword.Text) OrElse x.Target.Contains(TextBoxKeyword.Text)
            End If
        End If
    End Function

    Private Sub CreateFilterdEventSource()
        If EventSource IsNot Nothing AndAlso EventSource.Count > 0 Then
            Dim matchDelegate As New Func(Of Twitter.FormattedEvent, Boolean)(AddressOf IsFilterMatch)
            _filterdEventSource = (From x As Twitter.FormattedEvent In EventSource.AsParallel
                                    Where If(CheckExcludeMyEvent.Checked, Not x.IsMe, True)
                                    Where CBool(x.Eventtype And ParseEventTypeFromTag())
                                    Where matchDelegate(x)
                                    Order By x.CreatedAt Descending
                                    Select x).ToArray()
            _ItemCache = Nothing
            EventList.VirtualListSize = _filterdEventSource.Count
            StatusLabelCount.Text = String.Format("{0} / {1}", _filterdEventSource.Count, EventSource.Count)
        Else
            StatusLabelCount.Text = "0 / 0"
        End If
    End Sub

    Private Sub CheckExcludeMyEvent_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckExcludeMyEvent.CheckedChanged
        CreateFilterdEventSource()
    End Sub

    Private Sub ButtonRefresh_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonRefresh.Click
        CreateFilterdEventSource()
    End Sub

    Private Sub TabEventType_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TabEventType.SelectedIndexChanged, CheckBoxFilter.CheckedChanged
        CreateFilterdEventSource()
    End Sub

    Private Sub TabEventType_Selecting(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TabControlCancelEventArgs) Handles TabEventType.Selecting
        _curTab = e.TabPage
        If Not e.TabPage.Controls.Contains(EventList) Then
            e.TabPage.Controls.Add(EventList)
        End If
    End Sub

    Private Sub TextBoxKeyword_KeyPress(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TextBoxKeyword.KeyPress
        If e.KeyChar = ChrW(Keys.Enter) Then
            CreateFilterdEventSource()
            e.Handled = True
        End If
    End Sub

    Private Sub EventList_RetrieveVirtualItem(ByVal sender As System.Object, ByVal e As System.Windows.Forms.RetrieveVirtualItemEventArgs) Handles EventList.RetrieveVirtualItem
        If _ItemCache IsNot Nothing AndAlso e.ItemIndex >= _itemCacheIndex AndAlso e.ItemIndex < _itemCacheIndex + _ItemCache.Length Then
            'キャッシュヒット
            e.Item = _ItemCache(e.ItemIndex - _itemCacheIndex)
        Else
            'キャッシュミス
            e.Item = CreateListViewItem(_filterdEventSource(e.ItemIndex))
        End If
    End Sub

    Private Sub EventList_CacheVirtualItems(ByVal sender As System.Object, ByVal e As System.Windows.Forms.CacheVirtualItemsEventArgs) Handles EventList.CacheVirtualItems
        CreateCache(e.StartIndex, e.EndIndex)
    End Sub

    Private Sub CreateCache(ByVal StartIndex As Integer, ByVal EndIndex As Integer)
        'キャッシュ要求（要求範囲±30を作成）
        StartIndex -= 30
        If StartIndex < 0 Then StartIndex = 0
        EndIndex += 30
        If EndIndex > _filterdEventSource.Count() - 1 Then
            EndIndex = _filterdEventSource.Count() - 1
        End If
        _ItemCache = New ListViewItem(EndIndex - StartIndex) {}
        _itemCacheIndex = StartIndex
        For i As Integer = 0 To EndIndex - StartIndex
            _ItemCache(i) = CreateListViewItem(_filterdEventSource(StartIndex + i))
        Next
    End Sub
End Class
