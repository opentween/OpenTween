Imports System.Net
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.IO
Imports System.Text
Imports System.Security

Public Class HttpConnectionOAuth
    Inherits HttpConnection

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
    Private Shared token As String = ""

    '''<summary>
    '''OAuthの署名作成用秘密アクセストークン。永続化可能（ユーザー取り消しの可能性はある）。
    '''</summary>
    Private Shared tokenSecret As String = ""

    '''<summary>
    '''OAuthのコンシューマー鍵
    '''</summary>
    Private Shared consumerKey As String

    '''<summary>
    '''OAuthの署名作成用秘密コンシューマーデータ
    '''</summary>
    Private Shared consumerSecret As String

    '''<summary>
    '''HTTP通信してコンテンツを取得する（文字列コンテンツ）
    '''</summary>
    '''<remarks>
    '''通信タイムアウトなどWebExceptionをハンドルしていないため、呼び出し元で処理が必要。
    '''タイムアウト指定やレスポンスヘッダ取得は省略している。
    '''レスポンスのボディストリームを文字列に変換してcontent引数に格納して戻す。文字エンコードは未指定
    '''</remarks>
    '''<param name="method">HTTPのメソッド</param>
    '''<param name="requestUri">URI</param>
    '''<param name="param">key=valueに展開されて、クエリ（GET時）・ボディ（POST時）に付加される送信情報</param>
    '''<param name="content">[IN/OUT]HTTPレスポンスのボディ部データ返却用。呼び出し元で初期化が必要</param>
    '''<param name="headerInfo">[IN/OUT]HTTP応答のヘッダ情報</param>
    '''<returns>通信結果のHttpStatusCode</returns>
    Public Function GetContent(ByVal method As RequestMethod, _
            ByVal requestUri As Uri, _
            ByVal param As SortedList(Of String, String), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String)) As HttpStatusCode
        'contentがインスタンスされているかチェック
        If content Is Nothing Then Throw New ArgumentNullException("content")
        '認証済かチェック
        If String.IsNullOrEmpty(token) Then Throw New Exception("Sequence error. (Token is blank.)")

        Dim webReq As HttpWebRequest = CreateRequest(method, _
                                                    requestUri, _
                                                    param, _
                                                    False)
        'OAuth認証ヘッダを付加
        AppendOAuthInfo(webReq, param, token, tokenSecret)

        Return GetResponse(webReq, content, headerInfo, False)
    End Function

#Region "認証処理"
    Public Function AuthorizePinFlow(ByVal requestTokenUrl As String, _
                                        ByVal accessTokenUrl As String, _
                                        ByVal authorizeUrl As String) As Boolean
        'PIN-based flow
        Dim requestToken As String = ""
        Dim authUri As Uri = GetAuthorizePageUri(requestTokenUrl, authorizeUrl, requestToken)
        If authUri Is Nothing Then Return False
        System.Diagnostics.Process.Start(authUri.PathAndQuery) 'ブラウザで表示
        Dim inputForm As New InputTabName
        inputForm.FormTitle = "Input PIN code"
        inputForm.FormDescription = "Input the PIN code shown in the browser after you accept OAuth request."
        If inputForm.ShowDialog() = DialogResult.OK AndAlso Not String.IsNullOrEmpty(inputForm.TabName) Then
            Return GetAccessToken(accessTokenUrl, inputForm.TabName, requestToken)
        Else
            Return False
        End If
    End Function

    Public Function AuthorizeXAuth(ByVal url As String, ByVal username As String, ByVal password As String) As Boolean
        Dim webReq As HttpWebRequest = Nothing
        If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            Throw New Exception("Sequence error.(username or password is blank)")
        End If
        Dim reqUri As New Uri(url)
        Dim parameter As New SortedList(Of String, String)
        parameter.Add("x_auth_mode", "client_auth")
        parameter.Add("x_auth_username", username)
        parameter.Add("x_auth_password", password)

        webReq = HttpConnection.CreateRequest(RequestMethod.ReqPost, reqUri, parameter, False)

        AppendOAuthInfo(webReq, parameter, "", "")

        Try
            Dim status As HttpStatusCode
            Dim contentText As String = ""
            status = HttpConnection.GetResponse(webReq, contentText, Nothing, False)
            If status = HttpStatusCode.OK Then
                Dim tokenData As NameValueCollection = ParseQueryString(contentText)
                token = tokenData.Item("oauth_token")
                tokenSecret = tokenData.Item("oauth_token_secret")
                ''' TODO:その他情報も格納するか検討（user_idなど）
                If token = "" Then Return False
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function GetAuthorizePageUri(ByVal requestTokenUrl As String, _
                                        ByVal authorizeUrl As String, _
                                        ByRef requestToken As String) As Uri
        Const tokenKey As String = "oauth_token"
        Dim reqTokenData As NameValueCollection = GetOAuthToken(New Uri(requestTokenUrl), "", "")
        If reqTokenData IsNot Nothing Then
            requestToken = reqTokenData.Item(tokenKey)
            Dim ub As New UriBuilder(authorizeUrl)
            ub.Query = String.Format("{0}={1}", tokenKey, requestToken)
            Return ub.Uri
        Else
            Return Nothing
        End If
    End Function

    Private Function GetAccessToken(ByVal accessTokenUrl As String, ByVal pinCode As String, ByVal requestToken As String) As Boolean
        If String.IsNullOrEmpty(requestToken) Then Throw New Exception("Sequence error.(requestToken is blank)")

        Dim accessTokenData As NameValueCollection = GetOAuthToken(New Uri(accessTokenUrl), pinCode, requestToken)

        If accessTokenData IsNot Nothing Then
            token = accessTokenData.Item("oauth_token")
            tokenSecret = accessTokenData.Item("oauth_token_secret")
            If token = "" Then Return False
            Return True
        Else
            Return False
        End If
    End Function

    Private Function GetOAuthToken(ByVal requestUri As Uri, ByVal pinCode As String, ByVal requestToken As String) As NameValueCollection
        Dim webReq As HttpWebRequest = Nothing
        If String.IsNullOrEmpty(pinCode) Then
            webReq = HttpConnection.CreateRequest(RequestMethod.ReqGet, requestUri, Nothing, False)
        Else
            webReq = HttpConnection.CreateRequest(RequestMethod.ReqPost, requestUri, Nothing, False)
        End If
        Dim query As New SortedList(Of String, String)
        If Not String.IsNullOrEmpty(pinCode) Then query.Add("oauth_verifier", pinCode)
        AppendOAuthInfo(webReq, query, requestToken, "")
        Try
            Dim status As HttpStatusCode
            Dim contentText As String = ""
            status = HttpConnection.GetResponse(webReq, contentText, Nothing, False)
            If status = HttpStatusCode.OK Then
                Return ParseQueryString(contentText)
            Else
                Return Nothing
            End If
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
#End Region

#Region "OAuth認証用ヘッダ作成・付加処理"
    Private Shared Sub AppendOAuthInfo(ByVal webRequest As HttpWebRequest, _
                                        ByVal query As SortedList(Of String, String), _
                                        ByVal token As String, _
                                        ByVal tokenSecret As String)
        Dim parameter As SortedList(Of String, String) = GetOAuthParameter(token)
        If query IsNot Nothing Then
            For Each item As KeyValuePair(Of String, String) In query
                parameter.Add(item.Key, item.Value)
            Next
        End If
        parameter.Add("oauth_signature", CreateSignature(tokenSecret, webRequest.Method, webRequest.RequestUri, parameter))
        Dim sb As New StringBuilder("OAuth ")
        For Each item As KeyValuePair(Of String, String) In parameter
            If item.Key.StartsWith("oauth_") Then
                sb.AppendFormat("{0}=""{1}"",", item.Key, UrlEncode(item.Value))
            End If
        Next
        webRequest.Headers.Add(HttpRequestHeader.Authorization, sb.ToString)
    End Sub

    Private Shared Function GetOAuthParameter(ByVal token As String) As SortedList(Of String, String)
        Dim parameter As New SortedList(Of String, String)
        parameter.Add("oauth_consumer_key", ConsumerKey)
        parameter.Add("oauth_signature_method", "HMAC-SHA1")
        parameter.Add("oauth_timestamp", GetTimestamp())
        parameter.Add("oauth_nonce", GetNonce())
        parameter.Add("oauth_version", "1.0")
        If Not String.IsNullOrEmpty(token) Then parameter.Add("oauth_token", token)
        Return parameter
    End Function

    Private Shared Function CreateSignature(ByVal tokenSecret As String, _
                                            ByVal method As String, _
                                            ByVal uri As Uri, _
                                            ByVal parameter As SortedList(Of String, String) _
                                        ) As String
        Dim paramString As String = CreateQueryString(parameter)
        Dim url As String = String.Format("{0}://{1}{2}", uri.Scheme, uri.Host, uri.AbsolutePath)
        Dim signatureBase As String = String.Format("{0}&{1}&{2}", method, UrlEncode(url), UrlEncode(paramString))
        Dim key As String = UrlEncode(ConsumerSecret) + "&"
        If Not String.IsNullOrEmpty(tokenSecret) Then key += UrlEncode(tokenSecret)
        Dim hmac As New Cryptography.HMACSHA1(Encoding.ASCII.GetBytes(key))
        Dim hash As Byte() = hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBase))
        Return Convert.ToBase64String(hash)
    End Function

    Private Shared Function GetTimestamp() As String
        Return Convert.ToInt64((DateTime.UtcNow - UnixEpoch).TotalSeconds).ToString()
    End Function

    Private Shared Function GetNonce() As String
        Return NonceRandom.Next(123400, 9999999).ToString()
    End Function
#End Region

    Public Shared Sub Initialize(ByVal consumerKeyStr As String, _
                                    ByVal consumerSecretStr As String, _
                                    ByVal token As String, _
                                    ByVal tokenSecret As String)
        consumerKey = consumerKeyStr
        consumerSecret = consumerSecretStr
        HttpConnectionOAuth.token = token
        HttpConnectionOAuth.tokenSecret = tokenSecret
    End Sub
End Class
