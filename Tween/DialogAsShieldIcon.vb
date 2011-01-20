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

Public Class DialogAsShieldIcon
    'Private shield As New ShieldIcon
    Private dResult As DialogResult = Windows.Forms.DialogResult.None

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.dResult = System.Windows.Forms.DialogResult.OK
        Me.Hide()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.dResult = System.Windows.Forms.DialogResult.Cancel
        Me.Hide()
    End Sub

    Private Sub DialogAsShieldIcon_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If dResult = Windows.Forms.DialogResult.None Then
            e.Cancel = True
            dResult = Windows.Forms.DialogResult.Cancel
            Me.Hide()
        End If
    End Sub

    Private Sub DialogAsShieldIcon_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Me.Load
        'OK_Button.Image = shield.Icon
        PictureBox1.Image = System.Drawing.SystemIcons.Question.ToBitmap()
    End Sub

    Public Shadows Function Show(ByVal text As String, Optional ByVal detail As String = "", Optional ByVal caption As String = "DialogAsShieldIcon", _
                                 Optional ByVal Buttons As Windows.Forms.MessageBoxButtons = MessageBoxButtons.OKCancel, _
                                 Optional ByVal icon As Windows.Forms.MessageBoxIcon = MessageBoxIcon.Question _
                                ) As System.Windows.Forms.DialogResult
        Label1.Text = text
        Me.Text = caption
        Me.TextDetail.Text = detail
        Select Case Buttons
            Case MessageBoxButtons.OKCancel
                OK_Button.Text = "OK"
                Cancel_Button.Text = "Cancel"
            Case MessageBoxButtons.YesNo
                OK_Button.Text = "Yes"
                Cancel_Button.Text = "No"
            Case Else
                OK_Button.Text = "OK"
                Cancel_Button.Text = "Cancel"
        End Select
        ' とりあえずアイコンは処理しない（互換性のためパラメータだけ指定できる）

        MyBase.Show()
        Do While Me.dResult = Windows.Forms.DialogResult.None
            System.Threading.Thread.Sleep(200)
            Application.DoEvents()
        Loop
        If Buttons = MessageBoxButtons.YesNo Then
            Select Case dResult
                Case Windows.Forms.DialogResult.OK
                    Return Windows.Forms.DialogResult.Yes
                Case Windows.Forms.DialogResult.Cancel
                    Return Windows.Forms.DialogResult.No
            End Select
        Else
            Return dResult
        End If
    End Function
End Class
