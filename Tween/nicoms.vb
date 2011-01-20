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

Public Class nicoms
    Private Shared _nicovideo() As String = { _
        "www.nicovideo.jp/watch/", _
        "live.nicovideo.jp/watch/", _
        "live.nicovideo.jp/gate/", _
        "live.nicolive.jp/gate/", _
        "co.nicovideo.jp/community/", _
        "com.nicovideo.jp/community/", _
        "ch.nicovideo.jp/channel/", _
        "nicovideo.jp/watch/", _
        "seiga.nicovideo.jp/bbs/", _
        "www.niconicommons.jp/material/", _
        "niconicommons.jp/material/"}

    Public Shared Function Shorten(ByVal url As String) As String
        '®Œ`ihttp(s)://‚ðíœj
        If url.Length > 7 AndAlso url.Length < 128 AndAlso url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) Then
            url = url.Substring(7)
        Else
            Return url
        End If

        For Each nv As String In _nicovideo
            If url.StartsWith(nv) Then Return String.Format("{0}{1}", "http://nico.ms/", url.Substring(nv.Length))
        Next

        Dim i As Integer
        i = url.IndexOf("nicovideo.jp/user/", StringComparison.OrdinalIgnoreCase)
        If i = 0 OrElse i = 4 Then Return String.Format("{0}{1}", "http://nico.ms/", url.Substring(13 + i))

        i = url.IndexOf("nicovideo.jp/mylist/", StringComparison.OrdinalIgnoreCase)
        If i = 0 OrElse i = 4 Then Return String.Format("{0}{1}", "http://nico.ms/", url.Substring(13 + i))

        i = url.IndexOf("seiga.nicovideo.jp/watch/", StringComparison.OrdinalIgnoreCase)
        If i = 0 Then Return String.Format("{0}{1}", "http://nico.ms/", url.Substring(25))

        Return "http://" + url
    End Function
End Class
