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

Imports System.ComponentModel

Public Class ApiInformationChangedEventArgs
    Inherits EventArgs
    Public ApiInfo As New ApiInfo
End Class

Public MustInherit Class ApiInfoBase
    Protected Shared _MaxCount As Integer = -1
    Protected Shared _RemainCount As Integer = -1
    Protected Shared _ResetTime As New DateTime
    Protected Shared _ResetTimeInSeconds As Integer = -1
    Protected Shared _UsingCount As Integer = -1
    Protected Shared _AccessLevel As ApiAccessLevel = ApiAccessLevel.None
    Protected Shared _MediaMaxCount As Integer = -1
    Protected Shared _MediaResetTime As New DateTime
    Protected Shared _MediaRemainCount As Integer = -1
End Class

Public Enum ApiAccessLevel
    None
    Unknown
    Read
    ReadWrite
    ReadWriteAndDirectMessage
End Enum

Public Class ApiInfo
    Inherits ApiInfoBase
    Public MaxCount As Integer
    Public RemainCount As Integer
    Public ResetTime As DateTime
    Public ResetTimeInSeconds As Integer
    Public UsingCount As Integer
    Public AccessLevel As ApiAccessLevel
    Public MediaMaxCount As Integer
    Public MediaRemainCount As Integer
    Public MediaResetTime As DateTime

    Public Sub New()
        Me.MaxCount = _MaxCount
        Me.RemainCount = _RemainCount
        Me.ResetTime = _ResetTime
        Me.ResetTimeInSeconds = _ResetTimeInSeconds
        Me.UsingCount = _UsingCount
        Me.AccessLevel = _AccessLevel
        Me.MediaMaxCount = _MediaMaxCount
        Me.MediaRemainCount = _MediaRemainCount
        Me.MediaResetTime = _MediaResetTime
    End Sub
End Class

Public Class ApiInformation
    Inherits ApiInfoBase

    'Private ReadOnly _lockobj As New Object 更新時にロックが必要かどうかは様子見

    Public HttpHeaders As New Dictionary(Of String, String)(StringComparer.CurrentCultureIgnoreCase)

    Public Sub Initialize()
        If HttpHeaders.ContainsKey("X-RateLimit-Remaining") Then
            HttpHeaders.Item("X-RateLimit-Remaining") = "-1"
        Else
            HttpHeaders.Add("X-RateLimit-Remaining", "-1")
        End If

        If HttpHeaders.ContainsKey("X-RateLimit-Limit") Then
            HttpHeaders.Item("X-RateLimit-Limit") = "-1"
        Else
            HttpHeaders.Add("X-RateLimit-Limit", "-1")
        End If

        If HttpHeaders.ContainsKey("X-RateLimit-Reset") Then
            HttpHeaders.Item("X-RateLimit-Reset") = "-1"
        Else
            HttpHeaders.Add("X-RateLimit-Reset", "-1")
        End If

        If HttpHeaders.ContainsKey("X-Access-Level") Then
            HttpHeaders.Item("X-Access-Level") = "read-write-directmessages"
        Else
            HttpHeaders.Add("X-Access-Level", "read-write-directmessages")
        End If

        If HttpHeaders.ContainsKey("X-MediaRateLimit-Remaining") Then
            HttpHeaders.Item("X-MediaRateLimit-Remaining") = "-1"
        Else
            HttpHeaders.Add("X-MediaRateLimit-Remaining", "-1")
        End If

        If HttpHeaders.ContainsKey("X-MediaRateLimit-Limit") Then
            HttpHeaders.Item("X-MediaRateLimit-Limit") = "-1"
        Else
            HttpHeaders.Add("X-MediaRateLimit-Limit", "-1")
        End If

        If HttpHeaders.ContainsKey("X-MediaRateLimit-Reset") Then
            HttpHeaders.Item("X-MediaRateLimit-Reset") = "-1"
        Else
            HttpHeaders.Add("X-MediaRateLimit-Reset", "-1")
        End If

        _MaxCount = -1
        _RemainCount = -1
        _ResetTime = New DateTime
        _ResetTimeInSeconds = -1
        AccessLevel = ApiAccessLevel.None
        _MediaMaxCount = -1
        _MediaRemainCount = -1
        _MediaResetTime = New DateTime

        '_UsingCount = -1
        RaiseEvent Changed(Me, New ApiInformationChangedEventArgs)
    End Sub

    Public Function ConvertResetTimeInSecondsToResetTime(ByVal seconds As Integer) As DateTime
        If seconds >= 0 Then
            Return System.TimeZone.CurrentTimeZone.ToLocalTime((New DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(seconds))
        Else
            Return New DateTime
        End If
    End Function

    Public Event Changed(ByVal sender As Object, ByVal e As ApiInformationChangedEventArgs)

    Private Sub Raise_Changed()
        Dim arg As New ApiInformationChangedEventArgs
        RaiseEvent Changed(Me, arg)
        _MaxCount = arg.ApiInfo.MaxCount
        _RemainCount = arg.ApiInfo.RemainCount
        _ResetTime = arg.ApiInfo.ResetTime
        _ResetTimeInSeconds = arg.ApiInfo.ResetTimeInSeconds
        '_UsingCount = arg.ApiInfo.UsingCount
    End Sub

    Public Property MaxCount As Integer
        Get
            Return _MaxCount
        End Get
        Set(ByVal value As Integer)
            If _MaxCount <> value Then
                _MaxCount = value
                Raise_Changed()
            End If
        End Set
    End Property

    Public Property RemainCount As Integer
        Get
            Return _RemainCount
        End Get
        Set(ByVal value As Integer)
            If _RemainCount <> value Then
                _RemainCount = value
                Raise_Changed()
            End If
        End Set
    End Property

    Public Property ResetTime As DateTime
        Get
            Return _ResetTime
        End Get
        Set(ByVal value As DateTime)
            If _ResetTime <> value Then
                _ResetTime = value
                Raise_Changed()
            End If
        End Set
    End Property

    Public Property MediaMaxCount As Integer
        Get
            Return _MediaMaxCount
        End Get
        Set(ByVal value As Integer)
            If _MediaMaxCount <> value Then
                _MediaMaxCount = value
                Raise_Changed()
            End If
        End Set
    End Property

    Public Property MediaRemainCount As Integer
        Get
            Return _MediaRemainCount
        End Get
        Set(ByVal value As Integer)
            If _MediaRemainCount <> value Then
                _MediaRemainCount = value
                Raise_Changed()
            End If
        End Set
    End Property

    Public Property MediaResetTime As DateTime
        Get
            Return _MediaResetTime
        End Get
        Set(ByVal value As DateTime)
            If _MediaResetTime <> value Then
                _MediaResetTime = value
                Raise_Changed()
            End If
        End Set
    End Property

    Public Property ResetTimeInSeconds As Integer
        Get
            Return _ResetTimeInSeconds
        End Get
        Set(ByVal value As Integer)
            If _ResetTimeInSeconds <> value Then
                _ResetTimeInSeconds = value
                Raise_Changed()
            End If
        End Set
    End Property

    Public Property UsingCount As Integer
        Get
            Return _UsingCount
        End Get
        Set(ByVal value As Integer)
            If _UsingCount <> value Then
                _UsingCount = value
                Raise_Changed()
            End If
        End Set
    End Property

    Public Property AccessLevel As ApiAccessLevel
        Get
            Return _AccessLevel
        End Get
        Private Set(ByVal value As ApiAccessLevel)
            If _AccessLevel <> value Then
                _AccessLevel = value
                Raise_Changed()
            End If
        End Set
    End Property

    Public ReadOnly Property IsReadPermission As Boolean
        Get
            Return AccessLevel = ApiAccessLevel.Read OrElse
                AccessLevel = ApiAccessLevel.ReadWrite OrElse
                AccessLevel = ApiAccessLevel.ReadWriteAndDirectMessage
        End Get
    End Property

    Public ReadOnly Property IsWritePermission As Boolean
        Get
            Return AccessLevel = ApiAccessLevel.ReadWrite OrElse
                AccessLevel = ApiAccessLevel.ReadWriteAndDirectMessage
        End Get
    End Property

    Public ReadOnly Property IsDirectMessagePermission As Boolean
        Get
            Return AccessLevel = ApiAccessLevel.ReadWriteAndDirectMessage
        End Get
    End Property

    Private ReadOnly Property RemainCountFromHttpHeader() As Integer
        Get
            Dim result As Integer = 0
            If String.IsNullOrEmpty(HttpHeaders("X-RateLimit-Remaining")) Then Return -1
            If Integer.TryParse(HttpHeaders("X-RateLimit-Remaining"), result) Then
                Return result
            End If
            Return -1
        End Get
    End Property

    Private ReadOnly Property MaxCountFromHttpHeader() As Integer
        Get
            Dim result As Integer = 0
            If String.IsNullOrEmpty(HttpHeaders("X-RateLimit-Limit")) Then Return -1
            If Integer.TryParse(HttpHeaders("X-RateLimit-Limit"), result) Then
                Return result
            End If
            Return -1
        End Get
    End Property

    Private ReadOnly Property ResetTimeFromHttpHeader() As DateTime
        Get
            Dim i As Integer
            If Integer.TryParse(HttpHeaders("X-RateLimit-Reset"), i) Then
                If i >= 0 Then
                    Return System.TimeZone.CurrentTimeZone.ToLocalTime((New DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(i))
                Else
                    Return New DateTime
                End If
            Else
                Return New DateTime
            End If
        End Get
    End Property

    Private ReadOnly Property MediaRemainCountFromHttpHeader() As Integer
        Get
            Dim result As Integer = 0
            If String.IsNullOrEmpty(HttpHeaders("X-MediaRateLimit-Remaining")) Then Return -1
            If Integer.TryParse(HttpHeaders("X-MediaRateLimit-Remaining"), result) Then
                Return result
            End If
            Return -1
        End Get
    End Property

    Private ReadOnly Property MediaMaxCountFromHttpHeader() As Integer
        Get
            Dim result As Integer = 0
            If String.IsNullOrEmpty(HttpHeaders("X-MediaRateLimit-Limit")) Then Return -1
            If Integer.TryParse(HttpHeaders("X-MediaRateLimit-Limit"), result) Then
                Return result
            End If
            Return -1
        End Get
    End Property

    Private ReadOnly Property MediaResetTimeFromHttpHeader() As DateTime
        Get
            Dim i As Integer
            If Integer.TryParse(HttpHeaders("X-MediaRateLimit-Reset"), i) Then
                If i >= 0 Then
                    Return System.TimeZone.CurrentTimeZone.ToLocalTime((New DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(i))
                Else
                    Return New DateTime
                End If
            Else
                Return New DateTime
            End If
        End Get
    End Property

    Private ReadOnly Property ApiAccessLevelFromHttpHeader() As ApiAccessLevel
        Get
            Select Case HttpHeaders("X-Access-Level")
                Case "read"
                    Return ApiAccessLevel.Read
                Case "read-write"
                    Return ApiAccessLevel.ReadWrite
                Case "read-write-directmessages", "read-write-privatemessages"
                    Return ApiAccessLevel.ReadWriteAndDirectMessage
                Case Else
                    TraceOut("Unknown ApiAccessLevel:" + HttpHeaders("X-Access-Level"))
                    Return ApiAccessLevel.ReadWriteAndDirectMessage     '未知のアクセスレベルの場合Read/Write/Dmと仮定して処理継続
            End Select
        End Get
    End Property

    Public Sub ParseHttpHeaders(ByVal headers As Dictionary(Of String, String))
        Dim tmp As Integer
        Dim tmpd As DateTime
        tmp = MaxCountFromHttpHeader
        If tmp <> -1 Then
            _MaxCount = tmp
        End If
        tmp = RemainCountFromHttpHeader
        If tmp <> -1 Then
            _RemainCount = tmp
        End If
        tmpd = ResetTimeFromHttpHeader
        If tmpd <> New DateTime Then
            _ResetTime = tmpd
        End If

        tmp = MediaMaxCountFromHttpHeader
        If tmp <> -1 Then
            _MediaMaxCount = tmp
        End If
        tmp = MediaRemainCountFromHttpHeader
        If tmp <> -1 Then
            _MediaRemainCount = tmp
        End If
        tmpd = MediaResetTimeFromHttpHeader
        If tmpd <> New DateTime Then
            _MediaResetTime = tmpd
        End If

        AccessLevel = ApiAccessLevelFromHttpHeader
        Raise_Changed()
    End Sub

    Public Sub WriteBackEventArgs(ByVal arg As ApiInformationChangedEventArgs)
        _MaxCount = arg.ApiInfo.MaxCount
        _RemainCount = arg.ApiInfo.RemainCount
        _ResetTime = arg.ApiInfo.ResetTime
        _ResetTimeInSeconds = arg.ApiInfo.ResetTimeInSeconds
        _UsingCount = arg.ApiInfo.UsingCount
        Raise_Changed()
    End Sub
End Class


