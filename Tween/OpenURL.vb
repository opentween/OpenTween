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
Imports System.Text

Public Class OpenURL

    Private _selUrl As String

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        If UrlList.SelectedItems.Count = 0 Then
            Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Else
            _selUrl = UrlList.SelectedItem.ToString
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
        End If
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Public Sub ClearUrl()
        UrlList.Items.Clear()
    End Sub

    Public Sub AddUrl(ByVal openUrlItem As OpenUrlItem)
        UrlList.Items.Add(openUrlItem)
    End Sub

    Public ReadOnly Property SelectedUrl() As String
        Get
            If UrlList.SelectedItems.Count = 1 Then
                Return _selUrl
            Else
                Return ""
            End If
        End Get
    End Property

    Private Sub OpenURL_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        UrlList.Focus()
        If UrlList.Items.Count > 0 Then
            UrlList.SelectedIndex = 0
        End If
    End Sub

    Private Sub UrlList_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UrlList.DoubleClick
        If UrlList.SelectedItem Is Nothing Then
            Exit Sub
        End If

        If UrlList.IndexFromPoint(UrlList.PointToClient(Control.MousePosition)) = ListBox.NoMatches Then
            Exit Sub
        End If

        If UrlList.Items(UrlList.IndexFromPoint(UrlList.PointToClient(Control.MousePosition))) Is Nothing Then
            Exit Sub
        End If
        OK_Button_Click(sender, e)
    End Sub

    Private Sub UrlList_KeyDown(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles UrlList.KeyDown
        If e.KeyCode = Keys.J AndAlso UrlList.SelectedIndex < UrlList.Items.Count - 1 Then
            e.SuppressKeyPress = True
            UrlList.SelectedIndex += 1
        End If
        If e.KeyCode = Keys.K AndAlso UrlList.SelectedIndex > 0 Then
            e.SuppressKeyPress = True
            UrlList.SelectedIndex -= 1
        End If
        If e.Control AndAlso e.KeyCode = Keys.Oem4 Then
            e.SuppressKeyPress = True
            Cancel_Button_Click(Nothing, Nothing)
        End If
    End Sub
End Class

Public Class OpenUrlItem
    Private _url As String
    Private _linkText As String

    Public Sub New(ByVal linkText As String, ByVal url As String)
        Me._linkText = linkText
        Me._url = url
    End Sub

    Public ReadOnly Property Text() As String
        Get
            If Me._linkText.StartsWith("@") OrElse Me._linkText.StartsWith("＠") OrElse Me._linkText.StartsWith("#") OrElse Me._linkText.StartsWith("＃") Then
                Return Me._linkText
            End If
            If Me._linkText.TrimEnd("/"c) = Me._url.TrimEnd("/"c) Then
                Return Me._url
            Else
                Return Me._linkText + "  >>>  " + Me.Url
            End If
        End Get
    End Property

    Public ReadOnly Property Url() As String
        Get
            Return Me._url
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Me._url
    End Function
End Class
