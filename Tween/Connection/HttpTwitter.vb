Imports System.Net

Public Class HttpTwitter

    'OAuth関連
    '''<summary>
    '''OAuthのコンシューマー鍵
    '''</summary>
    Private Const ConsumerKey As String = "iOQHfiCUsyOyamW8JJ8jg"

    '''<summary>
    '''OAuthの署名作成用秘密コンシューマーデータ
    '''</summary>
    Private Const ConsumerSecret As String = "5PS2oa5f2VaKMPrlZa7DTbz0aFULKd3Ojxqgsm142Dw"

    '''<summary>
    '''OAuthのアクセストークン取得先URI
    '''</summary>
    Private Const AccessTokenUrlXAuth As String = "https://api.twitter.com/oauth/access_token"

    Private Shared _protocol As String = "http://"
    Private _remainCountApi As New Dictionary(Of String, String)

    Private Const PostMethod As String = "POST"
    Private Const GetMethod As String = "GET"

    Private httpCon As IHttpConnection 'HttpConnectionApi or HttpConnectionOAuth
    Private httpConVar As New HttpVarious

    Private Enum AuthMethod
        OAuth
        Basic
    End Enum
    Private connectionType As AuthMethod = AuthMethod.Basic

    Public Sub New()
        _remainCountApi.Add("X-RateLimit-Remaining", "-1")
        _remainCountApi.Add("X-RateLimit-Limit", "-1")
        _remainCountApi.Add("X-RateLimit-Reset", "-1")
    End Sub

    Public Sub Initialize(ByVal accessToken As String, _
                                    ByVal accessTokenSecret As String, _
                                    ByVal username As String)
        'for OAuth
        Dim con As New HttpConnectionOAuth
        con.Initialize(ConsumerKey, ConsumerSecret, accessToken, accessTokenSecret, username, "screen_name")
        httpCon = con
        connectionType = AuthMethod.OAuth
    End Sub

    Public Sub Initialize(ByVal username As String, _
                                    ByVal password As String)
        'for BASIC auth
        Dim con As New HttpConnectionBasic
        con.Initialize(username, password)
        httpCon = con
        connectionType = AuthMethod.Basic
    End Sub

    Public ReadOnly Property AccessToken() As String
        Get
            If connectionType = AuthMethod.Basic Then Return ""
            Return DirectCast(httpCon, HttpConnectionOAuth).AccessToken
        End Get
    End Property

    Public ReadOnly Property AccessTokenSecret() As String
        Get
            If connectionType = AuthMethod.Basic Then Return ""
            Return DirectCast(httpCon, HttpConnectionOAuth).AccessTokenSecret
        End Get
    End Property

    Public ReadOnly Property AuthenticatedUsername() As String
        Get
            Return httpCon.AuthUsername
        End Get
    End Property

    Public Function AuthUserAndPass(ByVal username As String, ByVal password As String) As Boolean
        If connectionType = AuthMethod.Basic Then
            Return httpCon.Authenticate("https://api.twitter.com/1/account/verify_credentials.xml", username, password)
        Else
            Return httpCon.Authenticate(AccessTokenUrlXAuth, username, password)
        End If
    End Function

    Public Sub ClearAuthInfo()
        If connectionType = AuthMethod.Basic Then
            Me.Initialize("", "")
        Else
            Me.Initialize("", "", "")
        End If
    End Sub

    Public Shared WriteOnly Property UseSsl() As Boolean
        Set(ByVal value As Boolean)
            If value Then
                _protocol = "https://"
            Else
                _protocol = "http://"
            End If
        End Set
    End Property

    Public ReadOnly Property RemainCountApi() As Integer
        Get
            Return Integer.Parse(_remainCountApi("X-RateLimit-Remaining"))
        End Get
    End Property

    Public ReadOnly Property UpperCountApi() As Integer
        Get
            Return Integer.Parse(_remainCountApi("X-RateLimit-Limit"))
        End Get
    End Property

    Public ReadOnly Property ResetTimeApi() As DateTime
        Get
            Dim i As Integer
            If Integer.TryParse(_remainCountApi("X-RateLimit-Reset"), i) Then
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

    Public Function UpdateStatus(ByVal status As String, ByVal replyToId As Long, ByRef content As String) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        param.Add("status", status)
        If replyToId > 0 Then param.Add("in_reply_to_status_id", replyToId.ToString)
        Try
            Return httpCon.GetContent(PostMethod, _
                            New Uri(_protocol + "api.twitter.com/1/statuses/update.xml"), _
                            param, _
                            content, _
                            Nothing)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function DestroyStatus(ByVal id As Long) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return httpCon.GetContent(PostMethod, _
                            New Uri(_protocol + "api.twitter.com/1/statuses/destroy/" + id.ToString + ".xml"), _
                            Nothing, _
                            Nothing, _
                            Nothing)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function DestroyDirectMessage(ByVal id As Long) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return httpCon.GetContent(PostMethod, _
                            New Uri(_protocol + "api.twitter.com/1/direct_messages/destroy/" + id.ToString + ".xml"), _
                            Nothing, _
                            Nothing, _
                            Nothing)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function RetweetStatus(ByVal id As Long, ByRef content As String) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return httpCon.GetContent(PostMethod, _
                            New Uri(_protocol + "api.twitter.com/1/statuses/retweet/" + id.ToString() + ".xml"), _
                            Nothing, _
                            content, _
                            Nothing)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function CreateFriendships(ByVal screenName As String) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)
        Try
            Return httpCon.GetContent(PostMethod, _
                            New Uri(_protocol + "api.twitter.com/1/friendships/create.xml"), _
                            param, _
                            Nothing, _
                            Nothing)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function DestroyFriendships(ByVal screenName As String) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)
        Try
            Return httpCon.GetContent(PostMethod, _
                            New Uri(_protocol + "api.twitter.com/1/friendships/destroy.xml"), _
                            param, _
                            Nothing, _
                            Nothing)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function ShowFriendships(ByVal souceScreenName As String, ByVal targetScreenName As String, ByRef content As String) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        param.Add("source_screen_name", souceScreenName)
        param.Add("target_screen_name", targetScreenName)
        Try
            Return httpCon.GetContent(GetMethod, _
                            New Uri(_protocol + "api.twitter.com/1/friendships/show.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function ShowStatuses(ByVal id As Long, ByRef content As String) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return httpCon.GetContent(GetMethod, _
                            New Uri(_protocol + "api.twitter.com/1/statuses/show/" + id.ToString() + ".xml"), _
                            Nothing, _
                            content, _
                            _remainCountApi)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function CreateFavorites(ByVal id As Long) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return httpCon.GetContent(PostMethod, _
                            New Uri(_protocol + "api.twitter.com/1/favorites/create/" + id.ToString() + ".xml"), _
                            Nothing, _
                            Nothing, _
                            Nothing)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function DestroyFavorites(ByVal id As Long) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return httpCon.GetContent(PostMethod, _
                            New Uri(_protocol + "api.twitter.com/1/favorites/destroy/" + id.ToString() + ".xml"), _
                            Nothing, _
                            Nothing, _
                            Nothing)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function HomeTimeline(ByVal count As Integer, ByRef content As String) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        If count > 0 Then
            param.Add("count", count.ToString())
        End If
        Try
            Return httpCon.GetContent(GetMethod, _
                            New Uri(_protocol + "api.twitter.com/1/statuses/home_timeline.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function Mentions(ByVal count As Integer, ByRef content As String) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        If count > 0 Then
            param.Add("count", count.ToString())
        End If
        Try
            Return httpCon.GetContent(GetMethod, _
                            New Uri(_protocol + "api.twitter.com/1/statuses/mentions.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function DirectMessages(ByRef content As String) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return httpCon.GetContent(GetMethod, _
                            New Uri(_protocol + "api.twitter.com/1/direct_messages.xml"), _
                            Nothing, _
                            content, _
                            _remainCountApi)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function DirectMessagesSent(ByRef content As String) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return httpCon.GetContent(GetMethod, _
                            New Uri(_protocol + "api.twitter.com/1/direct_messages/sent.xml"), _
                            Nothing, _
                            content, _
                            _remainCountApi)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function Favorites(ByVal count As Integer, ByRef content As String) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        If count <> 20 Then param.Add("count", count.ToString())
        Try
            Return httpCon.GetContent(GetMethod, _
                            New Uri(_protocol + "api.twitter.com/1/favorites.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function Search(ByVal words As String, ByVal lang As String, ByVal rpp As Integer, ByVal page As Integer, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        If Not String.IsNullOrEmpty(words) Then param.Add("q", words)
        If Not String.IsNullOrEmpty(lang) Then param.Add("lang", lang)
        If rpp > 0 Then param.Add("rpp", rpp.ToString())
        If page > 0 Then param.Add("page", page.ToString())

        If param.Count = 0 Then Return HttpStatusCode.BadRequest

        Try
            Return httpConVar.GetContent(GetMethod, _
                                        _protocol + "search.twitter.com/search.atom", _
                                        param, _
                                        content, _
                                        Nothing, _
                                        "Tween")
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function FollowerIds(ByVal cursor As Long, ByRef content As String) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        param.Add("cursor", cursor.ToString())
        Try
            Return httpCon.GetContent(GetMethod, _
                            New Uri(_protocol + "api.twitter.com/1/followers/ids.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function RateLimitStatus(ByRef content As String) As HttpStatusCode
        If Me.AuthenticatedUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return httpCon.GetContent(GetMethod, _
                            New Uri(_protocol + "api.twitter.com/1/account/rate_limit_status.xml"), _
                            Nothing, _
                            content, _
                            Nothing)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

End Class
