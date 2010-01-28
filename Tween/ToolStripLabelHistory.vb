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

Namespace TweenCustomControl

    Public NotInheritable Class ToolStripLabelHistory
        Inherits ToolStripStatusLabel

        Private sList As New List(Of String)
        Private Const MAXCNT As Integer = 10
        Private _history As String = ""

        Public Overrides Property Text() As String
            Get
                Return MyBase.Text
            End Get
            Set(ByVal value As String)
                sList.Add(value)
                Do While sList.Count > MAXCNT
                    sList.RemoveAt(0)
                Loop
                _history = ""
                For Each hstr As String In sList
                    If _history <> "" Then _history += System.Environment.NewLine
                    _history += hstr
                Next
                MyBase.Text = value
            End Set
        End Property

        Public ReadOnly Property TextHistory() As String
            Get
                Return _history
            End Get
        End Property
    End Class
End Namespace
