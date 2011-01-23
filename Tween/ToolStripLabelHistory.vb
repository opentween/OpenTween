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

Imports System.Text

Namespace TweenCustomControl

    Public NotInheritable Class ToolStripLabelHistory
        Inherits ToolStripStatusLabel

        Public Enum LogLevel
            Lowest = 0
            Debug = 16
            Info = 32
            Notice = 64
            Warn = 128
            Err = 192
            Fatal = 255
            Highest = 256
        End Enum

        Public Class LogEntry
            Private ReadOnly _logLevel As LogLevel

            Private ReadOnly _timestamp As DateTime

            Private ReadOnly _summary As String

            Private ReadOnly _detail As String

            Public ReadOnly Property LogLevel() As LogLevel
                Get
                    Return _logLevel
                End Get
            End Property

            Public ReadOnly Property Timestamp() As DateTime
                Get
                    Return _timestamp
                End Get
            End Property

            Public ReadOnly Property Summary() As String
                Get
                    Return _summary
                End Get
            End Property

            Public ReadOnly Property Detail() As String
                Get
                    Return _detail
                End Get
            End Property

            Public Sub New(ByVal logLevel As LogLevel, ByVal timestamp As DateTime, ByVal summary As String, ByVal detail As String)
                _logLevel = logLevel
                _timestamp = timestamp
                _summary = summary
                _detail = detail
            End Sub

            Public Sub New(ByVal timestamp As DateTime, ByVal summary As String)
                Me.New(LogLevel.Debug, timestamp, summary, summary)
            End Sub

            Public Overrides Function ToString() As String
                Return Timestamp.ToString("T") + ": " + Summary
            End Function
        End Class

        Private _logs As LinkedList(Of LogEntry)

        Private Const MAXCNT As Integer = 20

        Public Overrides Property Text() As String
            Get
                Return MyBase.Text
            End Get
            Set(ByVal value As String)
                _logs.AddLast(New LogEntry(DateTime.Now, value))
                Do While _logs.Count > MAXCNT
                    _logs.RemoveFirst()
                Loop
                MyBase.Text = value
            End Set
        End Property

        Public ReadOnly Property TextHistory() As String
            Get
                Dim sb As StringBuilder = New StringBuilder()
                For Each e As LogEntry In _logs
                    sb.AppendLine(e.ToString())
                Next
                Return sb.ToString()
            End Get
        End Property

        Public Sub New()
            _logs = New LinkedList(Of LogEntry)()
        End Sub
    End Class
End Namespace
