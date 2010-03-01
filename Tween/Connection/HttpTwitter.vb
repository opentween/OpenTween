Imports System.Net

Public Class HttpTwitter
    Inherits HttpConnectionOAuth

    'OAuth関連
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
    Private Const AccessTokenUrlXAuth As String = "https://api.twitter.com/oauth/access_token"

    Private Shared _protocol As String = "http://"
    Private Shared _remainCountApi As New Dictionary(Of String, String)

    Public Sub New()
        _remainCountApi.Add("X-RateLimit-Remaining", "-1")
    End Sub

    Public Overloads Shared Sub Initialize(ByVal accessToken As String, _
                                    ByVal accessTokenSecret As String, _
                                    ByVal username As String)
        HttpConnectionOAuth.Initialize(ConsumerKey, ConsumerSecret, accessToken, accessTokenSecret, username)
    End Sub

    Public Overloads Shared ReadOnly Property AccessToken() As String
        Get
            Return HttpConnectionOAuth.AccessToken
        End Get
    End Property

    Public Overloads Shared ReadOnly Property AccessTokenSecret() As String
        Get
            Return HttpConnectionOAuth.AccessTokenSecret
        End Get
    End Property

    Public Overloads Shared ReadOnly Property AuthUsername() As String
        Get
            Return HttpConnectionOAuth.AuthUsername
        End Get
    End Property

    Public Function Auth(ByVal username As String, ByVal password As String) As Boolean
        Return AuthorizeXAuth(AccessTokenUrlXAuth, username, password)
    End Function

    Public Shared Sub ClearAuthInfo()
        HttpConnectionOAuth.Initialize(ConsumerKey, ConsumerSecret, "", "", "")
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

    Public Shared ReadOnly Property RemainCountApi() As Integer
        Get
            Return Integer.Parse(_remainCountApi("X-RateLimit-Remaining"))
        End Get
    End Property

    Public Function UpdateStatus(ByVal status As String, ByVal replyToId As Long, ByRef content As String) As HttpStatusCode
        If HttpConnectionOAuth.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New SortedList(Of String, String)
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
        If HttpConnectionOAuth.AuthUsername = "" Then
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
        If HttpConnectionOAuth.AuthUsername = "" Then
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
        If HttpConnectionOAuth.AuthUsername = "" Then
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
        If HttpConnectionOAuth.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New SortedList(Of String, String)
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
        If HttpConnectionOAuth.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New SortedList(Of String, String)
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
        If HttpConnectionOAuth.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New SortedList(Of String, String)
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
        If HttpConnectionOAuth.AuthUsername = "" Then
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
        If HttpConnectionOAuth.AuthUsername = "" Then
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
        If HttpConnectionOAuth.AuthUsername = "" Then
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
        If HttpConnectionOAuth.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New SortedList(Of String, String)
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
        If HttpConnectionOAuth.AuthUsername = "" Then
            Return HttpStatusCode.Unauthorized
        End If
        Dim param As New SortedList(Of String, String)
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
End Class
