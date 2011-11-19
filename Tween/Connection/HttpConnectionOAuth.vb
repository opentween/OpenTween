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

Imports System.Collections.Specialized
Imports System.Diagnostics
Imports System.IO
Imports System.Net
Imports System.Security
Imports System.Text

'''<summary>
'''OAuth認証を使用するHTTP通信。HMAC-SHA1固定
'''</summary>
'''<remarks>
'''使用前に認証情報を設定する。認証確認を伴う場合はAuthenticate系のメソッドを、認証不要な場合はInitializeを呼ぶこと。
'''</remarks>
Public Class HttpConnectionOAuth
    Inherits HttpConnection
    Implements IHttpConnection

    '''<summary>
    '''OAuth署名のoauth_timestamp算出用基準日付（1970/1/1 00:00:00）
    '''</summary>
    Private Shared ReadOnly UnixEpoch As New DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified)

    '''<summary>
    '''OAuth署名のoauth_nonce算出用乱数クラス
    '''</summary>
    Private Shared ReadOnly NonceRandom As New Random

    '''<summary>
    '''OAuthのアクセストークン。永続化可能（ユーザー取り消しの可能性はある）。
    '''</summary>
    Private token As String = ""

    '''<summary>
    '''OAuthの署名作成用秘密アクセストークン。永続化可能（ユーザー取り消しの可能性はある）。
    '''</summary>
    Private tokenSecret As String = ""

    '''<summary>
    '''OAuthのコンシューマー鍵
    '''</summary>
    Private consumerKey As String

    '''<summary>
    '''OAuthの署名作成用秘密コンシューマーデータ
    '''</summary>
    Protected consumerSecret As String

    '''<summary>
    '''認証成功時の応答でユーザー情報を取得する場合のキー。設定しない場合は、AuthUsernameもブランクのままとなる
    '''</summary>
    Private userIdentKey As String = ""

    '''<summary>
    '''認証成功時の応答でユーザーID情報を取得する場合のキー。設定しない場合は、AuthUserIdもブランクのままとなる
    '''</summary>
    Private userIdIdentKey As String = ""

    '''<summary>
    '''認証完了時の応答からuserIdentKey情報に基づいて取得するユーザー情報
    '''</summary>
    Private authorizedUsername As String = ""

    '''<summary>
    '''認証完了時の応答からuserIdentKey情報に基づいて取得するユーザー情報
    '''</summary>
    Private authorizedUserId As Long

    '''<summary>
    '''Stream用のHttpWebRequest
    '''</summary>
    Private streamReq As HttpWebRequest = Nothing

    '''<summary>
    '''OAuth認証で指定のURLとHTTP通信を行い、結果を返す
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
        If String.IsNullOrEmpty(token) Then Return HttpStatusCode.Unauthorized

        Dim webReq As HttpWebRequest = CreateRequest(method, _
                                                    requestUri, _
                                                    param, _
                                                    False)
        'OAuth認証ヘッダを付加
        AppendOAuthInfo(webReq, param, token, tokenSecret)

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
    '''バイナリアップロード
    '''</summary>
    Public Function GetContent(ByVal method As String, _
        ByVal requestUri As Uri, _
        ByVal param As Dictionary(Of String, String), _
        ByVal binary As List(Of KeyValuePair(Of String, FileInfo)), _
        ByRef content As String, _
        ByVal headerInfo As Dictionary(Of String, String), _
        ByVal callback As IHttpConnection.CallbackDelegate) As HttpStatusCode Implements IHttpConnection.GetContent
        '認証済かチェック
        If String.IsNullOrEmpty(token) Then Return HttpStatusCode.Unauthorized

        Dim webReq As HttpWebRequest = CreateRequest(method, _
                                                    requestUri, _
                                                    param, _
                                                    binary, _
                                                    False)
        'OAuth認証ヘッダを付加
        AppendOAuthInfo(webReq, Nothing, token, tokenSecret)

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
    '''<returns>HTTP応答のステータスコード</returns>
    Public Function GetContent(ByVal method As String, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByRef content As Stream,
            ByVal userAgent As String) As HttpStatusCode Implements IHttpConnection.GetContent
        '認証済かチェック
        If String.IsNullOrEmpty(token) Then Return HttpStatusCode.Unauthorized

        Me.RequestAbort()
        streamReq = CreateRequest(method, requestUri, param, False)
        'User-Agent指定がある場合は付加
        If Not String.IsNullOrEmpty(userAgent) Then streamReq.UserAgent = userAgent

        'OAuth認証ヘッダを付加
        AppendOAuthInfo(streamReq, param, token, tokenSecret)

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
                streamReq = Nothing
            End If
        Catch ex As Exception
        End Try
    End Sub


#Region "認証処理"
    '''<summary>
    '''OAuth認証の開始要求（リクエストトークン取得）。PIN入力用の前段
    '''</summary>
    '''<remarks>
    '''呼び出し元では戻されたurlをブラウザで開き、認証完了後PIN入力を受け付けて、リクエストトークンと共にAuthenticatePinFlowを呼び出す
    '''</remarks>
    '''<param name="requestTokenUrl">リクエストトークンの取得先URL</param>
    '''<param name="requestUri">ブラウザで開く認証用URLのベース</param>
    '''<param name="requestToken">[OUT]認証要求で戻されるリクエストトークン。使い捨て</param>
    '''<param name="authUri">[OUT]requestUriを元に生成された認証用URL。通常はリクエストトークンをクエリとして付加したUri</param>
    '''<returns>取得結果真偽値</returns>
    Public Function AuthenticatePinFlowRequest(ByVal requestTokenUrl As String, _
                                        ByVal authorizeUrl As String, _
                                        ByRef requestToken As String, _
                                        ByRef authUri As Uri) As Boolean
        'PIN-based flow
        authUri = GetAuthenticatePageUri(requestTokenUrl, authorizeUrl, requestToken)
        If authUri Is Nothing Then Return False
        Return True
    End Function

    '''<summary>
    '''OAuth認証のアクセストークン取得。PIN入力用の後段
    '''</summary>
    '''<remarks>
    '''事前にAuthenticatePinFlowRequestを呼んで、ブラウザで認証後に表示されるPINを入力してもらい、その値とともに呼び出すこと
    '''</remarks>
    '''<param name="accessTokenUrl">アクセストークンの取得先URL</param>
    '''<param name="requestUri">AuthenticatePinFlowRequestで取得したリクエストトークン</param>
    '''<param name="pinCode">Webで認証後に表示されるPINコード</param>
    '''<returns>取得結果真偽値</returns>
    Public Function AuthenticatePinFlow(ByVal accessTokenUrl As String, _
                                        ByVal requestToken As String, _
                                        ByVal pinCode As String) As HttpStatusCode
        'PIN-based flow
        If String.IsNullOrEmpty(requestToken) Then Throw New Exception("Sequence error.(requestToken is blank)")

        'アクセストークン取得
        Dim content As String = ""
        Dim accessTokenData As NameValueCollection
        Dim httpCode As HttpStatusCode = GetOAuthToken(New Uri(accessTokenUrl), pinCode, requestToken, Nothing, content)
        If httpCode <> HttpStatusCode.OK Then Return httpCode
        accessTokenData = ParseQueryString(content)

        If accessTokenData IsNot Nothing Then
            token = accessTokenData.Item("oauth_token")
            tokenSecret = accessTokenData.Item("oauth_token_secret")
            'サービスごとの独自拡張対応
            If Me.userIdentKey <> "" Then
                authorizedUsername = accessTokenData.Item(Me.userIdentKey)
            Else
                authorizedUsername = ""
            End If
            If Me.userIdIdentKey <> "" Then
                Try
                    authorizedUserId = CLng(accessTokenData.Item(Me.userIdIdentKey))
                Catch ex As Exception
                    authorizedUserId = 0
                End Try
            Else
                authorizedUserId = 0
            End If
            If token = "" Then Throw New InvalidDataException("Token is null.")
            Return HttpStatusCode.OK
        Else
            Throw New InvalidDataException("Return value is null.")
        End If
    End Function

    '''<summary>
    '''OAuth認証のアクセストークン取得。xAuth方式
    '''</summary>
    '''<param name="accessTokenUrl">アクセストークンの取得先URL</param>
    '''<param name="username">認証用ユーザー名</param>
    '''<param name="password">認証用パスワード</param>
    '''<returns>取得結果真偽値</returns>
    Public Function AuthenticateXAuth(ByVal accessTokenUrl As Uri, ByVal username As String, ByVal password As String, ByRef content As String) As HttpStatusCode Implements IHttpConnection.Authenticate
        'ユーザー・パスワードチェック
        If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            Throw New Exception("Sequence error.(username or password is blank)")
        End If
        'xAuthの拡張パラメータ設定
        Dim parameter As New Dictionary(Of String, String)
        parameter.Add("x_auth_mode", "client_auth")
        parameter.Add("x_auth_username", username)
        parameter.Add("x_auth_password", password)

        'アクセストークン取得
        Dim httpCode As HttpStatusCode = GetOAuthToken(accessTokenUrl, "", "", parameter, content)
        If httpCode <> HttpStatusCode.OK Then Return httpCode
        Dim accessTokenData As NameValueCollection = ParseQueryString(content)

        If accessTokenData IsNot Nothing Then
            token = accessTokenData.Item("oauth_token")
            tokenSecret = accessTokenData.Item("oauth_token_secret")
            'サービスごとの独自拡張対応
            If Me.userIdentKey <> "" Then
                authorizedUsername = accessTokenData.Item(Me.userIdentKey)
            Else
                authorizedUsername = ""
            End If
            If Me.userIdIdentKey <> "" Then
                Try
                    authorizedUserId = CLng(accessTokenData.Item(Me.userIdIdentKey))
                Catch ex As Exception
                    authorizedUserId = 0
                End Try
            Else
                authorizedUserId = 0
            End If
            If token = "" Then Throw New InvalidDataException("Token is null.")
            Return HttpStatusCode.OK
        Else
            Throw New InvalidDataException("Return value is null.")
        End If
    End Function

    '''<summary>
    '''OAuth認証のリクエストトークン取得。リクエストトークンと組み合わせた認証用のUriも生成する
    '''</summary>
    '''<param name="accessTokenUrl">リクエストトークンの取得先URL</param>
    '''<param name="authorizeUrl">ブラウザで開く認証用URLのベース</param>
    '''<param name="requestToken">[OUT]取得したリクエストトークン</param>
    '''<returns>取得結果真偽値</returns>
    Private Function GetAuthenticatePageUri(ByVal requestTokenUrl As String, _
                                        ByVal authorizeUrl As String, _
                                        ByRef requestToken As String) As Uri
        Const tokenKey As String = "oauth_token"

        'リクエストトークン取得
        Dim content As String = ""
        Dim reqTokenData As NameValueCollection
        If GetOAuthToken(New Uri(requestTokenUrl), "", "", Nothing, content) <> HttpStatusCode.OK Then Return Nothing
        reqTokenData = ParseQueryString(content)

        If reqTokenData IsNot Nothing Then
            requestToken = reqTokenData.Item(tokenKey)
            'Uri生成
            Dim ub As New UriBuilder(authorizeUrl)
            ub.Query = String.Format("{0}={1}", tokenKey, requestToken)
            Return ub.Uri
        Else
            Return Nothing
        End If
    End Function

    '''<summary>
    '''OAuth認証のトークン取得共通処理
    '''</summary>
    '''<param name="requestUri">各種トークンの取得先URL</param>
    '''<param name="pinCode">PINフロー時のアクセストークン取得時に設定。それ以外は空文字列</param>
    '''<param name="requestToken">PINフロー時のリクエストトークン取得時に設定。それ以外は空文字列</param>
    '''<param name="parameter">追加パラメータ。xAuthで使用</param>
    '''<returns>取得結果のデータ。正しく取得出来なかった場合はNothing</returns>
    Private Function GetOAuthToken(ByVal requestUri As Uri, ByVal pinCode As String, ByVal requestToken As String, ByVal parameter As Dictionary(Of String, String), ByRef content As String) As HttpStatusCode
        Dim webReq As HttpWebRequest = Nothing
        'HTTPリクエスト生成。PINコードもパラメータも未指定の場合はGETメソッドで通信。それ以外はPOST
        If String.IsNullOrEmpty(pinCode) AndAlso parameter Is Nothing Then
            webReq = CreateRequest("GET", requestUri, Nothing, False)
        Else
            webReq = CreateRequest("POST", requestUri, parameter, False) 'ボディに追加パラメータ書き込み
        End If
        'OAuth関連パラメータ準備。追加パラメータがあれば追加
        Dim query As New Dictionary(Of String, String)
        If parameter IsNot Nothing Then
            For Each kvp As KeyValuePair(Of String, String) In parameter
                query.Add(kvp.Key, kvp.Value)
            Next
        End If
        'PINコードが指定されていればパラメータに追加
        If Not String.IsNullOrEmpty(pinCode) Then query.Add("oauth_verifier", pinCode)
        'OAuth関連情報をHTTPリクエストに追加
        AppendOAuthInfo(webReq, query, requestToken, "")
        'HTTP応答取得
        Dim header As New Dictionary(Of String, String) From {{"Date", ""}}
        Dim responseCode As HttpStatusCode = GetResponse(webReq, content, header, False)
        If responseCode = HttpStatusCode.OK Then Return responseCode
        If Not String.IsNullOrEmpty(header("Date")) Then content += Environment.NewLine + "Check the Date & Time of this computer." + Environment.NewLine + "Server:" + CDate(header("Date")).ToString + "  PC:" + Now.ToString
        Return responseCode
    End Function
#End Region

#Region "OAuth認証用ヘッダ作成・付加処理"
    '''<summary>
    '''HTTPリクエストにOAuth関連ヘッダを追加
    '''</summary>
    '''<param name="webRequest">追加対象のHTTPリクエスト</param>
    '''<param name="query">OAuth追加情報＋クエリ or POSTデータ</param>
    '''<param name="token">アクセストークン、もしくはリクエストトークン。未取得なら空文字列</param>
    '''<param name="tokenSecret">アクセストークンシークレット。認証処理では空文字列</param>
    Protected Overridable Sub AppendOAuthInfo(ByVal webRequest As HttpWebRequest, _
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
        '署名の作成・追加
        parameter.Add("oauth_signature", CreateSignature(tokenSecret, webRequest.Method, webRequest.RequestUri, parameter))
        'HTTPリクエストのヘッダに追加
        Dim sb As New StringBuilder("OAuth ")
        For Each item As KeyValuePair(Of String, String) In parameter
            '各種情報のうち、oauth_で始まる情報のみ、ヘッダに追加する。各情報はカンマ区切り、データはダブルクォーテーションで括る
            If item.Key.StartsWith("oauth_") Then
                sb.AppendFormat("{0}=""{1}"",", item.Key, UrlEncode(item.Value))
            End If
        Next
        webRequest.Headers.Add(HttpRequestHeader.Authorization, sb.ToString)
    End Sub

    '''<summary>
    '''OAuthで使用する共通情報を取得する
    '''</summary>
    '''<param name="token">アクセストークン、もしくはリクエストトークン。未取得なら空文字列</param>
    '''<returns>OAuth情報のディクショナリ</returns>
    Protected Function GetOAuthParameter(ByVal token As String) As Dictionary(Of String, String)
        Dim parameter As New Dictionary(Of String, String)
        parameter.Add("oauth_consumer_key", consumerKey)
        parameter.Add("oauth_signature_method", "HMAC-SHA1")
        parameter.Add("oauth_timestamp", Convert.ToInt64((DateTime.UtcNow - UnixEpoch).TotalSeconds).ToString())   'epoch秒
        parameter.Add("oauth_nonce", NonceRandom.Next(123400, 9999999).ToString())
        parameter.Add("oauth_version", "1.0")
        If Not String.IsNullOrEmpty(token) Then parameter.Add("oauth_token", token) 'トークンがあれば追加
        Return parameter
    End Function

    '''<summary>
    '''OAuth認証ヘッダの署名作成
    '''</summary>
    '''<param name="tokenSecret">アクセストークン秘密鍵</param>
    '''<param name="method">HTTPメソッド文字列</param>
    '''<param name="uri">アクセス先Uri</param>
    '''<param name="parameter">クエリ、もしくはPOSTデータ</param>
    '''<returns>署名文字列</returns>
    Protected Overridable Function CreateSignature(ByVal tokenSecret As String, _
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
        '署名のベース文字列生成（&区切り）。クエリ形式文字列は再エンコードする
        Dim signatureBase As String = String.Format("{0}&{1}&{2}", method, UrlEncode(url), UrlEncode(paramString))
        '署名鍵の文字列をコンシューマー秘密鍵とアクセストークン秘密鍵から生成（&区切り。アクセストークン秘密鍵なくても&残すこと）
        Dim key As String = UrlEncode(consumerSecret) + "&"
        If Not String.IsNullOrEmpty(tokenSecret) Then key += UrlEncode(tokenSecret)
        '鍵生成＆署名生成
        Using hmac As New Cryptography.HMACSHA1(Encoding.ASCII.GetBytes(key))
            Dim hash As Byte() = hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBase))
            Return Convert.ToBase64String(hash)
        End Using
    End Function

#End Region

    '''<summary>
    '''初期化。各種トークンの設定とユーザー識別情報設定
    '''</summary>
    '''<param name="consumerKey">コンシューマー鍵</param>
    '''<param name="consumerSecret">コンシューマー秘密鍵</param>
    '''<param name="accessToken">アクセストークン</param>
    '''<param name="accessTokenSecret">アクセストークン秘密鍵</param>
    '''<param name="userIdentifier">アクセストークン取得時に得られるユーザー識別情報。不要なら空文字列</param>
    Public Sub Initialize(ByVal consumerKey As String, _
                                    ByVal consumerSecret As String, _
                                    ByVal accessToken As String, _
                                    ByVal accessTokenSecret As String, _
                                    ByVal userIdentifier As String,
                                    ByVal userIdIdentifier As String)
        Me.consumerKey = consumerKey
        Me.consumerSecret = consumerSecret
        Me.token = accessToken
        Me.tokenSecret = accessTokenSecret
        Me.userIdentKey = userIdentifier
        Me.userIdIdentKey = userIdIdentifier
    End Sub

    '''<summary>
    '''初期化。各種トークンの設定とユーザー識別情報設定
    '''</summary>
    '''<param name="consumerKey">コンシューマー鍵</param>
    '''<param name="consumerSecret">コンシューマー秘密鍵</param>
    '''<param name="accessToken">アクセストークン</param>
    '''<param name="accessTokenSecret">アクセストークン秘密鍵</param>
    '''<param name="username">認証済みユーザー名</param>
    '''<param name="userIdentifier">アクセストークン取得時に得られるユーザー識別情報。不要なら空文字列</param>
    Public Sub Initialize(ByVal consumerKey As String, _
                                ByVal consumerSecret As String, _
                                ByVal accessToken As String, _
                                ByVal accessTokenSecret As String, _
                                ByVal username As String, _
                                ByVal userId As Long,
                                ByVal userIdentifier As String,
                                ByVal userIdIdentifier As String)
        Initialize(consumerKey, consumerSecret, accessToken, accessTokenSecret, userIdentifier, userIdIdentifier)
        authorizedUsername = username
        authorizedUserId = userId
    End Sub

    '''<summary>
    '''アクセストークン
    '''</summary>
    Public ReadOnly Property AccessToken() As String
        Get
            Return token
        End Get
    End Property

    '''<summary>
    '''アクセストークン秘密鍵
    '''</summary>
    Public ReadOnly Property AccessTokenSecret() As String
        Get
            Return tokenSecret
        End Get
    End Property

    '''<summary>
    '''認証済みユーザー名
    '''</summary>
    Public ReadOnly Property AuthUsername() As String Implements IHttpConnection.AuthUsername
        Get
            Return authorizedUsername
        End Get
    End Property

    '''<summary>
    '''認証済みユーザーId
    '''</summary>
    Public Property AuthUserId() As Long Implements IHttpConnection.AuthUserId
        Get
            Return authorizedUserId
        End Get
        Set(ByVal value As Long)
            authorizedUserId = value
        End Set
    End Property

End Class
