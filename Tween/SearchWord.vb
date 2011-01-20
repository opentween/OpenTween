' Tween - Client of Twitter
' Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
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

Public Class SearchWord

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Public Property SWord() As String
        Get
            Return SWordText.Text
        End Get
        Set(ByVal value As String)
            SWordText.Text = value
        End Set
    End Property

    Public Property CheckCaseSensitive() As Boolean
        Get
            Return CheckSearchCaseSensitive.Checked
        End Get
        Set(ByVal value As Boolean)
            CheckSearchCaseSensitive.Checked = value
        End Set
    End Property

    Public Property CheckRegex() As Boolean
        Get
            Return CheckSearchRegex.Checked
        End Get
        Set(ByVal value As Boolean)
            CheckSearchRegex.Checked = value
        End Set
    End Property

    Private Sub SearchWord_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        SWordText.SelectAll()
        SWordText.Focus()
    End Sub
End Class
