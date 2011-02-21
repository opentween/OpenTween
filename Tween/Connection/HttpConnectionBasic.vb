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
Imports System.Net
Imports System.IO
Imports System.Diagnostics

'''<summary>
'''BASIC認証を使用するHTTP通信
'''</summary>
'''<remarks>
'''使用前にユーザー／パスワードを設定する。認証確認を伴う場合はAuthenticateを、認証不要な場合はInitializeを呼ぶこと。
'''</remarks>
Public Class HttpConnectionBasic
    Inherits HttpConnection
    Implements IHttpConnection

    '''<summary>
    '''認証用ユーザー名
    '''</summary>
    Private _userName As String = ""
    '''<summary>
    '''認証用パスワード
    '''</summary>
    Private _password As String = ""
    '''<summary>
    '''Authorizationヘッダに設定するエンコード済み文字列
    '''</summary>
    Private credential As String = ""


    '''<summary>
    '''認証完了時の応答からuserIdentKey情報に基づいて取得するユーザー情報
    '''</summary>
    Private streamReq As HttpWebRequest = Nothing

    '''<summary>
    '''BASIC認証で指定のURLとHTTP通信を行い、結果を返す
    '''</summary>
    '''<param name="method">HTTP通信メソッド（GET/HEAD/POST/PUT/DELETE）</param>
    '''<param name="requestUri">通信先URI</param>
    '''<param name="param">GET時のクエリ、またはPOST時のエンティティボディ</param>
    '''<param name="content">[OUT]HTTP応答のボディデータ</param>
    '''<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。必要なヘッダ名を事前に設定しておくこと</param>
    '''<param name="callback">処理終了直前に呼ばれるコールバック関数のデリゲート 不要な場合はNothingを渡すこと</param>
    '''<returns>HTTP応答のステータスコード</returns>
    Public Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String), _
            ByVal callback As IHttpConnection.CallbackDelegate) As HttpStatusCode Implements IHttpConnection.GetContent

        '認証済かチェック
        If String.IsNullOrEmpty(Me.credential) Then Return HttpStatusCode.Unauthorized

        Dim webReq As HttpWebRequest = CreateRequest(method, _
                                                    requestUri, _
                                                    param, _
                                                    False)
        'BASIC認証用ヘッダを付加
        AppendApiInfo(webReq)

        Dim code As HttpStatusCode
        If content Is Nothing Then
            code = GetResponse(webReq, headerInfo, False)
        Else
            code = GetResponse(webReq, content, headerInfo, False)
        End If
        If callback IsNot Nothing Then
            Dim frame As New StackFrame(1)
            callback(frame.GetMethod.Name, code, content)
        End If
        Return code
    End Function

    Public Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByVal binary As List(Of KeyValuePair(Of String, FileInfo)), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String), _
            ByVal callback As IHttpConnection.CallbackDelegate) As HttpStatusCode Implements IHttpConnection.GetContent

        '認証済かチェック
        If String.IsNullOrEmpty(Me.credential) Then Return HttpStatusCode.Unauthorized

        Dim webReq As HttpWebRequest = CreateRequest(method, _
                                                    requestUri, _
                                                    param, _
                                                    binary, _
                                                    False)
        'BASIC認証用ヘッダを付加
        AppendApiInfo(webReq)

        Dim code As HttpStatusCode
        If content Is Nothing Then
            code = GetResponse(webReq, headerInfo, False)
        Else
            code = GetResponse(webReq, content, headerInfo, False)
        End If
        If callback IsNot Nothing Then
            Dim frame As New StackFrame(1)
            callback(frame.GetMethod.Name, code, content)
        End If
        Return code
    End Function

    '''<summary>
    '''OAuth認証で指定のURLとHTTP通信を行い、ストリームを返す
    '''</summary>
    '''<param name="method">HTTP通信メソッド（GET/HEAD/POST/PUT/DELETE）</param>
    '''<param name="requestUri">通信先URI</param>
    '''<param name="param">GET時のクエリ、またはPOST時のエンティティボディ</param>
    '''<param name="content">[OUT]HTTP応答のボディストリーム</param>
    '''<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報。必要なヘッダ名を事前に設定しておくこと</param>
    '''<returns>HTTP応答のステータスコード</returns>
    Public Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByRef content As Stream,
            ByVal userAgent As String) As HttpStatusCode Implements IHttpConnection.GetContent
        '認証済かチェック
        If String.IsNullOrEmpty(Me.credential) Then Return HttpStatusCode.Unauthorized

        streamReq = CreateRequest(method, requestUri, param, False)
        'User-Agent指定がある場合は付加
        If Not String.IsNullOrEmpty(userAgent) Then streamReq.UserAgent = userAgent

        'BASIC認証用ヘッダを付加
        AppendApiInfo(streamReq)

        Try
            Dim webRes As HttpWebResponse = CType(streamReq.GetResponse(), HttpWebResponse)
            content = webRes.GetResponseStream()
            Return webRes.StatusCode
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw
        End Try

    End Function

    Public Sub RequestAbort() Implements IHttpConnection.RequestAbort
        Try
            If streamReq IsNot Nothing Then
                streamReq.Abort()
            End If
        Catch ex As Exception
        End Try
    End Sub

    '''<summary>
    '''BASIC認証とREST APIで必要なヘッダを付加
    '''</summary>
    '''<param name="webRequest">付加対象となるHTTPリクエストオブジェクト</param>
    Private Sub AppendApiInfo(ByVal webRequest As HttpWebRequest)
        webRequest.ContentType = "application/x-www-form-urlencoded"
        webRequest.Accept = "text/html, */*"
        webRequest.Headers.Add(HttpRequestHeader.Authorization, credential)
    End Sub

    '''<summary>
    '''BASIC認証で使用するユーザー名とパスワードを設定。
    '''</summary>
    '''<param name="userName">認証で使用するユーザー名</param>
    '''<param name="password">認証で使用するパスワード</param>
    Public Sub Initialize(ByVal userName As String, ByVal password As String)
        Me._userName = userName
        Me._password = password
        Me.credential = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(userName + ":" + password))
    End Sub

    '''<summary>
    '''設定されているユーザー名
    '''</summary>
    Public ReadOnly Property AuthUsername() As String Implements IHttpConnection.AuthUsername
        Get
            Return _userName
        End Get
    End Property

    '''<summary>
    '''パスワード
    '''</summary>
    Public ReadOnly Property Password() As String
        Get
            Return Me._password
        End Get
    End Property

    '''<summary>
    '''BASIC認証で使用するユーザー名とパスワードを設定。指定のURLにGETリクエストを投げて、OK応答なら認証OKとみなして認証情報を保存する
    '''</summary>
    '''<param name="url">認証先のURL</param>
    '''<param name="userName">認証で使用するユーザー名</param>
    '''<param name="password">認証で使用するパスワード</param>
    Public Function Authenticate(ByVal url As Uri, ByVal username As String, ByVal password As String, ByRef content As String) As HttpStatusCode Implements IHttpConnection.Authenticate
        'urlは認証必要なGETメソッドとする
        Dim orgCre As String = Me.credential
        Me.credential = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(username + ":" + password))
        Try
            Dim httpCode As HttpStatusCode = Me.GetContent("GET", url, Nothing, Nothing, Nothing, Nothing)
            If httpCode = HttpStatusCode.OK Then
                Me._userName = username
                Me._password = password
            Else
                Me.credential = orgCre
            End If
            Return httpCode
        Catch ex As Exception
            Me.credential = orgCre
            Throw
        End Try
    End Function

End Class
