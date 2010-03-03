Imports System.Net

Public Class HttpTwitter
    Inherits HttpConnectionOAuth

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

    Public Sub New()
        _remainCountApi.Add("X-RateLimit-Remaining", "-1")
    End Sub

    Public Overloads Sub Initialize(ByVal accessToken As String, _
                                    ByVal accessTokenSecret As String, _
                                    ByVal username As String)
        Initialize(ConsumerKey, ConsumerSecret, accessToken, accessTokenSecret, username)
    End Sub

    Public Overloads ReadOnly Property AccessToken() As String
        Get
            Return MyBase.AccessToken
        End Get
    End Property

    Public Overloads ReadOnly Property AccessTokenSecret() As String
        Get
            Return MyBase.AccessTokenSecret
        End Get
    End Property

    Public Overloads ReadOnly Property AuthUsername() As String
        Get
            Return MyBase.AuthUsername
        End Get
    End Property

    Public Function Auth(ByVal username As String, ByVal password As String) As Boolean
        Return AuthorizeXAuth(AccessTokenUrlXAuth, username, password)
    End Function

    Public Sub ClearAuthInfo()
        MyBase.Initialize(ConsumerKey, ConsumerSecret, "", "", "")
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

    Public Function UpdateStatus(ByVal status As String, ByVal replyToId As Long, ByRef content As String) As HttpStatusCode
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        param.Add("status", status)
        If replyToId > 0 Then param.Add("in_reply_to_status_id", replyToId.ToString)
        Try
            Return GetContent(RequestMethod.ReqPost, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return GetContent(RequestMethod.ReqPost, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return GetContent(RequestMethod.ReqPost, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return GetContent(RequestMethod.ReqPost, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)
        Try
            Return GetContent(RequestMethod.ReqPost, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)
        Try
            Return GetContent(RequestMethod.ReqPost, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        param.Add("source_screen_name", souceScreenName)
        param.Add("target_screen_name", targetScreenName)
        Try
            Return GetContent(RequestMethod.ReqGet, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return GetContent(RequestMethod.ReqGet, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return GetContent(RequestMethod.ReqPost, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return GetContent(RequestMethod.ReqPost, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        If count > 0 Then
            param.Add("count", count.ToString())
        End If
        Try
            Return GetContent(RequestMethod.ReqGet, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        If count > 0 Then
            param.Add("count", count.ToString())
        End If
        Try
            Return GetContent(RequestMethod.ReqGet, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return GetContent(RequestMethod.ReqGet, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return GetContent(RequestMethod.ReqGet, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        If count <> 20 Then param.Add("count", count.ToString())
        Try
            Return GetContent(RequestMethod.ReqGet, _
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

        Dim req As HttpWebRequest = CreateRequest(RequestMethod.ReqGet, _
                                                New Uri(_protocol + "search.twitter.com/search.atom"), _
                                                param, _
                                                False)
        req.UserAgent = "Tween"
        Try
            Return Me.GetResponse(req, content, Nothing, False)
        Catch ex As WebException
            If ex.Status = WebExceptionStatus.ProtocolError Then
                Dim res As HttpWebResponse = DirectCast(ex.Response, HttpWebResponse)
                Return res.StatusCode
            End If
            Throw ex
        End Try
    End Function

    Public Function FollowerIds(ByVal cursor As Long, ByRef content As String) As HttpStatusCode
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New Dictionary(Of String, String)
        param.Add("cursor", cursor.ToString())
        Try
            Return GetContent(RequestMethod.ReqGet, _
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
        If Me.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Try
            Return GetContent(RequestMethod.ReqGet, _
                            New Uri(_protocol + "api.twitter.com/1/rate_limit_status.xml"), _
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
