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

Public Class EventViewerDialog
    Public Property EventSource As List(Of Twitter.FormattedEvent)

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Function CreateListViewItemArray(ByVal source As Generic.List(Of Twitter.FormattedEvent)) As ListViewItem()
        Dim items As New Generic.List(Of ListViewItem)
        For Each x As Twitter.FormattedEvent In source
            Dim s() As String = {x.CreatedAt.ToString, x.Event.ToUpper, x.Username, x.Target}
            items.Add(New ListViewItem(s))
        Next
        Return items.ToArray()
    End Function

    Private Sub EventViewerDialog_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        If EventSource IsNot Nothing AndAlso EventSource.Count > 0 Then
            EventList.BeginUpdate()
            EventList.Items.AddRange(CreateListViewItemArray(EventSource))
            Me.EventList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
            EventList.EndUpdate()
        End If
    End Sub

    Private Sub EventList_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles EventList.DoubleClick
        If EventSource IsNot Nothing AndAlso EventSource.Count > 0 Then
            If EventSource(EventList.SelectedIndices(0)) IsNot Nothing Then
                If Me.Owner IsNot Nothing Then
                    DirectCast(Me.Owner, TweenMain).OpenUriAsync("http://twitter.com/" + EventSource(EventList.SelectedIndices(0)).Username)
                End If
            End If
        End If
    End Sub

    Private Sub CheckExcludeMyEvent_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckExcludeMyEvent.CheckedChanged
        EventList.BeginUpdate()
        EventList.Items.Clear()
        If EventSource IsNot Nothing AndAlso EventSource.Count > 0 Then
            EventList.Items.AddRange(
                CreateListViewItemArray((From x As Twitter.FormattedEvent In EventSource
                                        Where If(CheckExcludeMyEvent.Checked, Not x.IsMe, True) Select x).ToList()))
        End If
        EventList.EndUpdate()
    End Sub
End Class
