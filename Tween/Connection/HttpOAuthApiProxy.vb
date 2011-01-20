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

Imports System.Security
Imports System.Text

Public Class HttpOAuthApiProxy
    Inherits HttpConnectionOAuth

    Private Const _apiHost As String = "api.twitter.com"
    Private Shared _proxyHost As String = ""

    Friend Shared WriteOnly Property ProxyHost As String
        Set(ByVal value As String)
            If String.IsNullOrEmpty(value) OrElse value = _apiHost Then
                _proxyHost = ""
            Else
                _proxyHost = value
            End If
        End Set
    End Property

    Protected Overrides Function CreateSignature(ByVal tokenSecret As String, _
                                            ByVal method As String, _
                                            ByVal uri As Uri, _
                                            ByVal parameter As Dictionary(Of String, String) _
                                        ) As String
        'パラメタをソート済みディクショナリに詰替（OAuthの仕様）
        Dim sorted As New SortedDictionary(Of String, String)(parameter)
        'URLエンコード済みのクエリ形式文字列に変換
        Dim paramString As String = CreateQueryString(sorted)
        'アクセス先URLの整形
        Dim url As String = String.Format("{0}://{1}{2}", uri.Scheme, uri.Host, uri.AbsolutePath)
        '本来のアクセス先URLに再設定（api.twitter.com固定）
        If Not String.IsNullOrEmpty(_proxyHost) AndAlso url.StartsWith(uri.Scheme + "://" + _proxyHost) Then
            url = url.Replace(uri.Scheme + "://" + _proxyHost, uri.Scheme + "://" + _apiHost)
        End If
        '署名のベース文字列生成（&区切り）。クエリ形式文字列は再エンコードする
        Dim signatureBase As String = String.Format("{0}&{1}&{2}", method, UrlEncode(url), UrlEncode(paramString))
        '署名鍵の文字列をコンシューマー秘密鍵とアクセストークン秘密鍵から生成（&区切り。アクセストークン秘密鍵なくても&残すこと）
        Dim key As String = UrlEncode(consumerSecret) + "&"
        If Not String.IsNullOrEmpty(tokenSecret) Then key += UrlEncode(tokenSecret)
        '鍵生成＆署名生成
        Dim hmac As New Cryptography.HMACSHA1(Encoding.ASCII.GetBytes(key))
        Dim hash As Byte() = hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBase))
        Return Convert.ToBase64String(hash)
    End Function

End Class
