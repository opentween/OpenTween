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

Imports System.Net
Imports System.Text

Public Class HttpConnectionOAuthEcho
    Inherits HttpConnectionOAuth

    Private _realm As Uri
    Private _serviceProvider As Uri
    Private _token As String
    Private _tokenSecret As String

    Public WriteOnly Property Realm As Uri
        Set(ByVal value As Uri)
            _realm = value
        End Set
    End Property

    Public WriteOnly Property ServiceProvider As Uri
        Set(ByVal value As Uri)
            _serviceProvider = value
        End Set
    End Property

    Protected Overrides Sub AppendOAuthInfo(ByVal webRequest As HttpWebRequest, _
                                ByVal query As Dictionary(Of String, String), _
                                ByVal token As String, _
                                ByVal tokenSecret As String)
        'OAuth共通情報取得
        Dim parameter As Dictionary(Of String, String) = GetOAuthParameter(token)
        'OAuth共通情報にquery情報を追加
        If query IsNot Nothing Then
            For Each item As KeyValuePair(Of String, String) In query
                parameter.Add(item.Key, item.Value)
            Next
        End If
        '署名の作成・追加(GETメソッド固定。ServiceProvider呼び出し用の署名作成)
        parameter.Add("oauth_signature", CreateSignature(tokenSecret, GetMethod, _serviceProvider, parameter))
        'HTTPリクエストのヘッダに追加
        Dim sb As New StringBuilder("OAuth ")
        sb.AppendFormat("realm=""{0}://{1}{2}"",", _realm.Scheme, _realm.Host, _realm.AbsolutePath)
        For Each item As KeyValuePair(Of String, String) In parameter
            '各種情報のうち、oauth_で始まる情報のみ、ヘッダに追加する。各情報はカンマ区切り、データはダブルクォーテーションで括る
            If item.Key.StartsWith("oauth_") Then
                sb.AppendFormat("{0}=""{1}"",", item.Key, UrlEncode(item.Value))
            End If
        Next
        webRequest.Headers.Add("X-Verify-Credentials-Authorization", sb.ToString)
        webRequest.Headers.Add("X-Auth-Service-Provider", String.Format("{0}://{1}{2}", _serviceProvider.Scheme, _serviceProvider.Host, _serviceProvider.AbsolutePath))
    End Sub


    Public Sub New(ByVal realm As Uri, ByVal serviceProvider As Uri)
        _realm = realm
        _serviceProvider = serviceProvider
    End Sub
End Class
