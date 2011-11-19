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
Imports System.ComponentModel

Public Class ListAvailable

    Private _selectedList As ListElement = Nothing
    Public ReadOnly Property SelectedList() As ListElement
        Get
            Return _selectedList
        End Get
    End Property

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        If Me.ListsList.SelectedIndex > -1 Then
            _selectedList = DirectCast(Me.ListsList.SelectedItem, ListElement)
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End If
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        _selectedList = Nothing
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub ListAvailable_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        If TabInformations.GetInstance().SubscribableLists.Count = 0 Then Me.RefreshLists()
        Me.ListsList.Items.AddRange(TabInformations.GetInstance.SubscribableLists.ToArray)
        If Me.ListsList.Items.Count > 0 Then
            Me.ListsList.SelectedIndex = 0
        Else
            Me.UsernameLabel.Text = ""
            Me.NameLabel.Text = ""
            Me.StatusLabel.Text = ""
            Me.MemberCountLabel.Text = "0"
            Me.SubscriberCountLabel.Text = "0"
            Me.DescriptionText.Text = ""
        End If
    End Sub

    Private Sub ListsList_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListsList.SelectedIndexChanged
        Dim lst As ListElement
        If Me.ListsList.SelectedIndex > -1 Then
            lst = DirectCast(Me.ListsList.SelectedItem, ListElement)
        Else
            lst = Nothing
        End If
        If lst Is Nothing Then
            Me.UsernameLabel.Text = ""
            Me.NameLabel.Text = ""
            Me.StatusLabel.Text = ""
            Me.MemberCountLabel.Text = "0"
            Me.SubscriberCountLabel.Text = "0"
            Me.DescriptionText.Text = ""
        Else
            Me.UsernameLabel.Text = lst.Username + " / " + lst.Nickname
            Me.NameLabel.Text = lst.Name
            If lst.IsPublic Then
                Me.StatusLabel.Text = "Public"
            Else
                Me.StatusLabel.Text = "Private"
            End If
            Me.MemberCountLabel.Text = lst.MemberCount.ToString("#,##0")
            Me.SubscriberCountLabel.Text = lst.SubscriberCount.ToString("#,##0")
            Me.DescriptionText.Text = lst.Description
        End If
    End Sub

    Private Sub RefreshButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshButton.Click
        Me.RefreshLists()
        Me.ListsList.Items.Clear()
        Me.ListsList.Items.AddRange(TabInformations.GetInstance.SubscribableLists.ToArray)
        If Me.ListsList.Items.Count > 0 Then
            Me.ListsList.SelectedIndex = 0
        End If
    End Sub

    Private Sub RefreshLists()
        Using dlg As New FormInfo(Me, "Getting Lists...", AddressOf RefreshLists_DoWork)
            dlg.ShowDialog()
            If Not String.IsNullOrEmpty(DirectCast(dlg.Result, String)) Then
                MessageBox.Show("Failed to get lists. (" + DirectCast(dlg.Result, String) + ")")
                Exit Sub
            End If
        End Using
    End Sub

    Private Sub RefreshLists_DoWork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        Try
            e.Result = DirectCast(Me.Owner, TweenMain).TwitterInstance.GetListsApi()
        Catch ex As InvalidCastException
            Exit Sub
        End Try
    End Sub
End Class
