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
    Private consumerSecret As String

    Private authorizedUsername As String
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
    Protected Function GetContent(ByVal method As RequestMethod, _
            ByVal requestUri As Uri, _
            ByVal param As Dictionary(Of String, String), _
            ByRef content As String, _
            ByVal headerInfo As Dictionary(Of String, String)) As HttpStatusCode
        '認証済かチェック
        If String.IsNullOrEmpty(token) Then Throw New Exception("Sequence error. (Token is blank.)")

        Dim webReq As HttpWebRequest = CreateRequest(method, _
                                                    requestUri, _
                                                    param, _
                                                    False)
        'OAuth認証ヘッダを付加
        AppendOAuthInfo(webReq, param, token, tokenSecret)

        If content Is Nothing Then
            Return GetResponse(webReq, headerInfo, False)
        Else
            Return GetResponse(webReq, content, headerInfo, False)
        End If
    End Function

#Region "認証処理"
    Protected Function AuthorizePinFlowRequest(ByVal requestTokenUrl As String, _
                                        ByVal authorizeUrl As String, _
                                        ByRef requestToken As String, _
                                        ByRef authUri As Uri) As Boolean
        'PIN-based flow
        authUri = GetAuthorizePageUri(requestTokenUrl, authorizeUrl, requestToken)
        If authUri Is Nothing Then Return False
        Return True
    End Function

    Protected Function AuthorizePinFlow(ByVal accessTokenUrl As String, _
                                        ByVal requestToken As String, _
                                        ByVal pinCode As String) As Boolean
        'PIN-based flow
        Return GetAccessToken(accessTokenUrl, pinCode, requestToken)
    End Function

    Protected Function AuthorizeXAuth(ByVal url As String, ByVal username As String, ByVal password As String) As Boolean
        Dim webReq As HttpWebRequest = Nothing
        If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            Throw New Exception("Sequence error.(username or password is blank)")
        End If
        Dim parameter As New Dictionary(Of String, String)
        parameter.Add("x_auth_mode", "client_auth")
        parameter.Add("x_auth_username", username)
        parameter.Add("x_auth_password", password)

        Dim accessTokenData As NameValueCollection = GetOAuthToken(New Uri(url), "", "", parameter)

        If accessTokenData IsNot Nothing Then
            token = accessTokenData.Item("oauth_token")
            tokenSecret = accessTokenData.Item("oauth_token_secret")
            authorizedUsername = accessTokenData.Item("screen_name")
            If token = "" Then Return False
            Return True
        Else
            Return False
        End If
    End Function

    Private Function GetAuthorizePageUri(ByVal requestTokenUrl As String, _
                                        ByVal authorizeUrl As String, _
                                        ByRef requestToken As String) As Uri
        Const tokenKey As String = "oauth_token"
        Dim reqTokenData As NameValueCollection = GetOAuthToken(New Uri(requestTokenUrl), "", "", Nothing)
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

        Dim accessTokenData As NameValueCollection = GetOAuthToken(New Uri(accessTokenUrl), pinCode, requestToken, Nothing)

        If accessTokenData IsNot Nothing Then
            token = accessTokenData.Item("oauth_token")
            tokenSecret = accessTokenData.Item("oauth_token_secret")
            If token = "" Then Return False
            Return True
        Else
            Return False
        End If
    End Function

    Private Function GetOAuthToken(ByVal requestUri As Uri, ByVal pinCode As String, ByVal requestToken As String, ByVal parameter As Dictionary(Of String, String)) As NameValueCollection
        Dim webReq As HttpWebRequest = Nothing
        If String.IsNullOrEmpty(pinCode) AndAlso parameter Is Nothing Then
            webReq = CreateRequest(RequestMethod.ReqGet, requestUri, Nothing, False)
        Else
            webReq = CreateRequest(RequestMethod.ReqPost, requestUri, parameter, False)
        End If
        Dim query As New Dictionary(Of String, String)
        If parameter IsNot Nothing Then
            For Each kvp As KeyValuePair(Of String, String) In parameter
                query.Add(kvp.Key, kvp.Value)
            Next
        End If
        If Not String.IsNullOrEmpty(pinCode) Then query.Add("oauth_verifier", pinCode)
        AppendOAuthInfo(webReq, query, requestToken, "")
        Try
            Dim status As HttpStatusCode
            Dim contentText As String = ""
            status = GetResponse(webReq, contentText, Nothing, False)
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
    Private Sub AppendOAuthInfo(ByVal webRequest As HttpWebRequest, _
                                        ByVal query As Dictionary(Of String, String), _
                                        ByVal token As String, _
                                        ByVal tokenSecret As String)
        Dim parameter As Dictionary(Of String, String) = GetOAuthParameter(token)
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

    Private Function GetOAuthParameter(ByVal token As String) As Dictionary(Of String, String)
        Dim parameter As New Dictionary(Of String, String)
        parameter.Add("oauth_consumer_key", consumerKey)
        parameter.Add("oauth_signature_method", "HMAC-SHA1")
        parameter.Add("oauth_timestamp", GetTimestamp())
        parameter.Add("oauth_nonce", GetNonce())
        parameter.Add("oauth_version", "1.0")
        If Not String.IsNullOrEmpty(token) Then parameter.Add("oauth_token", token)
        Return parameter
    End Function

    Private Function CreateSignature(ByVal tokenSecret As String, _
                                            ByVal method As String, _
                                            ByVal uri As Uri, _
                                            ByVal parameter As Dictionary(Of String, String) _
                                        ) As String
        Dim sorted As New SortedDictionary(Of String, String)(parameter)
        Dim paramString As String = CreateQueryString(sorted)
        Dim url As String = String.Format("{0}://{1}{2}", uri.Scheme, uri.Host, uri.AbsolutePath)
        Dim signatureBase As String = String.Format("{0}&{1}&{2}", method, UrlEncode(url), UrlEncode(paramString))
        Dim key As String = UrlEncode(consumerSecret) + "&"
        If Not String.IsNullOrEmpty(tokenSecret) Then key += UrlEncode(tokenSecret)
        Dim hmac As New Cryptography.HMACSHA1(Encoding.ASCII.GetBytes(key))
        Dim hash As Byte() = hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBase))
        Return Convert.ToBase64String(hash)
    End Function

    Private Function GetTimestamp() As String
        Return Convert.ToInt64((DateTime.UtcNow - UnixEpoch).TotalSeconds).ToString()
    End Function

    Private Function GetNonce() As String
        Return NonceRandom.Next(123400, 9999999).ToString()
    End Function
#End Region

    Protected Sub Initialize(ByVal consumerKeyStr As String, _
                                    ByVal consumerSecretStr As String, _
                                    ByVal accessToken As String, _
                                    ByVal accessTokenSecret As String)
        Me.consumerKey = consumerKeyStr
        Me.consumerSecret = consumerSecretStr
        Me.token = accessToken
        Me.tokenSecret = accessTokenSecret
    End Sub

    Protected Sub Initialize(ByVal consumerKeyStr As String, _
                                ByVal consumerSecretStr As String, _
                                ByVal accessToken As String, _
                                ByVal accessTokenSecret As String, _
                                ByVal username As String)
        Initialize(consumerKeyStr, consumerSecretStr, accessToken, accessTokenSecret)
        authorizedUsername = username
    End Sub

    Protected ReadOnly Property AccessToken() As String
        Get
            Return token
        End Get
    End Property

    Protected ReadOnly Property AccessTokenSecret() As String
        Get
            Return tokenSecret
        End Get
    End Property

    Protected ReadOnly Property AuthUsername() As String
        Get
            Return authorizedUsername
        End Get
    End Property
End Class
