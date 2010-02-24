Imports System.Net
Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.IO
Imports System.Text
Imports System.Security

Public Class HttpConnectionXAuth
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
    Private Const ConsumerKey As String = "EANjQEa5LokuVld682tTDA"

    '''<summary>
    '''OAuthの署名作成用秘密コンシューマーデータ
    '''</summary>
    Private Const ConsumerSecret As String = "zXfwkzmuO6FcHtoikleV3EVgdh5vVAs6ft6ZxtYTYM"

    '''<summary>
    '''OAuthのアクセストークン取得先URI
    '''</summary>
    Private Const AccessTokenUrl As String = "https://api.twitter.com/oauth/access_token"

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
    Protected Function AuthorizeAccount(ByVal username As String, ByVal password As String) As Boolean
        Dim webReq As HttpWebRequest = Nothing
        If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            Throw New Exception("Sequence error.(username or password is blank)")
        End If
        Dim reqUri As New Uri(AccessTokenUrl)
        Dim parameter As New SortedList(Of String, String)
        parameter.Add("oauth_consumer_key", ConsumerKey)
        parameter.Add("oauth_signature_method", "HMAC-SHA1")
        parameter.Add("oauth_timestamp", GetTimestamp())
        parameter.Add("oauth_nonce", GetNonce())
        parameter.Add("oauth_version", "1.0")
        parameter.Add("x_auth_mode", "client_auth")
        parameter.Add("x_auth_username", username)
        parameter.Add("x_auth_password", password)
        parameter.Add("oauth_signature", CreateSignature("", "POST", reqUri, parameter))

        webReq = HttpConnection.CreateRequest(RequestMethod.ReqPost, reqUri, parameter, False)

        Try
            Dim status As HttpStatusCode
            Dim contentText As String = ""
            status = HttpConnection.GetResponse(webReq, contentText, Nothing, False)
            If status = HttpStatusCode.OK Then
                Dim tokenData As NameValueCollection = ParseQueryString(contentText)
                token = tokenData.Item("oauth_token")
                tokenSecret = tokenData.Item("oauth_token_secret")
                If token = "" Then Return False
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            Return False
        End Try
    End Function
#End Region

#Region "OAuth認証用ヘッダ作成・付加処理"
    Private Shared Sub AppendOAuthInfo(ByVal webRequest As HttpWebRequest, _
                                        ByVal query As SortedList(Of String, String), _
                                        ByVal token As String, _
                                        ByVal tokenSecret As String)
        Dim parameter As New SortedList(Of String, String)
        parameter.Add("oauth_consumer_key", ConsumerKey)
        parameter.Add("oauth_signature_method", "HMAC-SHA1")
        parameter.Add("oauth_timestamp", GetTimestamp())
        parameter.Add("oauth_nonce", GetNonce())
        parameter.Add("oauth_version", "1.0")
        If Not String.IsNullOrEmpty(token) Then parameter.Add("oauth_token", token)
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

    Protected Shared Sub Initialize(ByVal token As String, ByVal tokenSecret As String)
        HttpConnectionXAuth.token = token
        HttpConnectionXAuth.tokenSecret = tokenSecret
    End Sub
End Class
