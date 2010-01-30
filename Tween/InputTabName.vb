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

Public Class InputTabName

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        TextTabName.Text = ""
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Public Property TabName() As String
        Get
            Return Me.TextTabName.Text.Trim()
        End Get
        Set(ByVal value As String)
            TextTabName.Text = value.Trim()
        End Set
    End Property

    Public WriteOnly Property FormTitle() As String
        Set(ByVal value As String)
            Me.Text = value
        End Set
    End Property

    Public WriteOnly Property FormDescription() As String
        Set(ByVal value As String)
            Me.LabelDescription.Text = value
        End Set
    End Property

    Private _isShowUsage As Boolean
    Public WriteOnly Property IsShowUsage() As Boolean
        Set(ByVal value As Boolean)
            _isShowUsage = value
        End Set
    End Property

    Private _usage As TabUsageType
    Public ReadOnly Property Usage() As TabUsageType
        Get
            Return _usage
        End Get
    End Property

    Private Sub InputTabName_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.LabelUsage.Visible = False
        Me.ComboUsage.Visible = False
        Me.ComboUsage.Items.Add(My.Resources.InputTabName_Load1)
        Me.ComboUsage.Items.Add("PublicSearch")
        Me.ComboUsage.SelectedIndex = 0
    End Sub

    Private Sub InputTabName_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        ActiveControl = TextTabName
        If _isShowUsage Then
            Me.LabelUsage.Visible = True
            Me.ComboUsage.Visible = True
        End If
    End Sub

    Private Sub ComboUsage_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComboUsage.SelectedIndexChanged
        Select Case ComboUsage.SelectedIndex
            Case 0
                _usage = TabUsageType.UserDefined
            Case 1
                _usage = TabUsageType.PublicSearch
            Case Else
                _usage = TabUsageType.Undefined
        End Select
    End Sub
End Class
