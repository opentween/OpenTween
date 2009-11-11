' Tween - Client of Twitter
' Copyright (c) 2007-2009 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
'           (c) 2008-2009 Moz (@syo68k) <http://iddy.jp/profile/moz/>
'           (c) 2008-2009 takeshik (@takeshik) <http://www.takeshik.org/>
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

    Private Sub InputTabName_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        ActiveControl = TextTabName
    End Sub
End Class
