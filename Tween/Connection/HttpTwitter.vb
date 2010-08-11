Imports System.Net
Imports System.IO

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
    Private _remainCountApi As New Dictionary(Of String, String)(StringComparer.CurrentCultureIgnoreCase)

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
        InitializeCount()
    End Sub

    Private Sub InitializeCount()
        If _remainCountApi.ContainsKey("X-RateLimit-Remaining") Then
            _remainCountApi.Item("X-RateLimit-Remaining") = "-1"
        Else
            _remainCountApi.Add("X-RateLimit-Remaining", "-1")
        End If

        If _remainCountApi.ContainsKey("X-RateLimit-Limit") Then
            _remainCountApi.Item("X-RateLimit-Limit") = "-1"
        Else
            _remainCountApi.Add("X-RateLimit-Limit", "-1")
        End If

        If _remainCountApi.ContainsKey("X-RateLimit-Reset") Then
            _remainCountApi.Item("X-RateLimit-Reset") = "-1"
        Else
            _remainCountApi.Add("X-RateLimit-Reset", "-1")
        End If
    End Sub

    Public Sub Initialize(ByVal accessToken As String, _
                                    ByVal accessTokenSecret As String, _
                                    ByVal username As String)
        'for OAuth
        Dim con As New HttpOAuthApiProxy
        Static tk As String = ""
        Static tks As String = ""
        Static un As String = ""
        If tk <> accessToken OrElse tks <> accessTokenSecret OrElse _
                un <> username OrElse connectionType <> AuthMethod.OAuth Then
            ' 以前の認証状態よりひとつでも変化があったらhttpヘッダより読み取ったカウントは初期化
            tk = accessToken
            tks = accessTokenSecret
            un = username
            InitializeCount()
        End If
        con.Initialize(ConsumerKey, ConsumerSecret, accessToken, accessTokenSecret, username, "screen_name")
        httpCon = con
        connectionType = AuthMethod.OAuth
    End Sub

    Public Sub Initialize(ByVal username As String, _
                                    ByVal password As String)
        'for BASIC auth
        Dim con As New HttpConnectionBasic
        Static un As String = ""
        Static pw As String = ""
        If un <> username OrElse pw <> password OrElse connectionType <> AuthMethod.Basic Then
            ' 以前の認証状態よりひとつでも変化があったらhttpヘッダより読み取ったカウントは初期化
            un = username
            pw = password
            InitializeCount()
        End If
        con.Initialize(username, password)
        httpCon = con
        connectionType = AuthMethod.Basic
    End Sub

    Public ReadOnly Property AccessToken() As String
        Get
            If httpCon IsNot Nothing Then
                If connectionType = AuthMethod.Basic Then Return ""
                Return DirectCast(httpCon, HttpConnectionOAuth).AccessToken
            Else
                Return ""
            End If
        End Get
    End Property

    Public ReadOnly Property AccessTokenSecret() As String
        Get
            If httpCon IsNot Nothing Then
                If connectionType = AuthMethod.Basic Then Return ""
                Return DirectCast(httpCon, HttpConnectionOAuth).AccessTokenSecret
            Else
                Return ""
            End If
        End Get
    End Property

    Public ReadOnly Property AuthenticatedUsername() As String
        Get
            If httpCon IsNot Nothing Then
                Return httpCon.AuthUsername
            Else
                Return ""
            End If
        End Get
    End Property

    Public ReadOnly Property Password() As String
        Get
            If httpCon IsNot Nothing Then
                'OAuthではパスワード取得させない
                If connectionType = AuthMethod.Basic Then Return DirectCast(httpCon, HttpConnectionBasic).Password
                Return ""
            Else
                Return ""
            End If
        End Get
    End Property

    Public Function AuthUserAndPass(ByVal username As String, ByVal password As String) As HttpStatusCode
        If connectionType = AuthMethod.Basic Then
            Return httpCon.Authenticate(CreateTwitterUri("/1/account/verify_credentials.xml"), username, password)
        Else
            Return httpCon.Authenticate(New Uri(AccessTokenUrlXAuth), username, password)
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
            Dim result As Integer = 0
            If _remainCountApi("X-RateLimit-Remaining") = "" Then Return -1
            If Integer.TryParse(_remainCountApi("X-RateLimit-Remaining"), result) Then
                Return result
            End If
            Return -1
        End Get
    End Property

    Public ReadOnly Property UpperCountApi() As Integer
        Get
            Dim result As Integer = 0
            If _remainCountApi("X-RateLimit-Limit") = "" Then Return -1
            If Integer.TryParse(_remainCountApi("X-RateLimit-Limit"), result) Then
                Return result
            End If
            Return -1
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
        Dim param As New Dictionary(Of String, String)
        param.Add("status", status)
        If connectionType = AuthMethod.Basic Then param.Add("source", "Tween")
        If replyToId > 0 Then param.Add("in_reply_to_status_id", replyToId.ToString)

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/statuses/update.xml"), _
                            param, _
                            content, _
                            Nothing)
    End Function

    Public Function DestroyStatus(ByVal id As Long) As HttpStatusCode
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/statuses/destroy/" + id.ToString + ".xml"), _
                            Nothing, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function SendDirectMessage(ByVal status As String, ByVal sendto As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("text", status)
        param.Add("screen_name", sendto)

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/direct_messages/new.xml"), _
                            param, _
                            content, _
                            Nothing)
    End Function

    Public Function DestroyDirectMessage(ByVal id As Long) As HttpStatusCode
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/direct_messages/destroy/" + id.ToString + ".xml"), _
                            Nothing, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function RetweetStatus(ByVal id As Long, ByRef content As String) As HttpStatusCode
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/statuses/retweet/" + id.ToString() + ".xml"), _
                            Nothing, _
                            content, _
                            Nothing)
    End Function

    Public Function ShowUserInfo(ByVal screenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/users/show.xml"), _
                            param, _
                            content, _
                            Nothing)
    End Function
    Public Function CreateFriendships(ByVal screenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/friendships/create.xml"), _
                            param, _
                            content, _
                            Nothing)
    End Function

    Public Function DestroyFriendships(ByVal screenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/friendships/destroy.xml"), _
                            param, _
                            content, _
                            Nothing)
    End Function

    Public Function CreateBlock(ByVal screenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/blocks/create.xml"), _
                            param, _
                            content, _
                            Nothing)
    End Function

    Public Function DestroyBlock(ByVal screenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/blocks/destroy.xml"), _
                            param, _
                            content, _
                            Nothing)
    End Function

    Public Function ReportSpam(ByVal screenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/report_spam.xml"), _
                            param, _
                            content, _
                            Nothing)
    End Function

    Public Function ShowFriendships(ByVal souceScreenName As String, ByVal targetScreenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("source_screen_name", souceScreenName)
        param.Add("target_screen_name", targetScreenName)

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/friendships/show.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
    End Function

    Public Function ShowStatuses(ByVal id As Long, ByRef content As String) As HttpStatusCode
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/statuses/show/" + id.ToString() + ".xml"), _
                            Nothing, _
                            content, _
                            _remainCountApi)
    End Function

    Public Function CreateFavorites(ByVal id As Long, ByRef content As String) As HttpStatusCode
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/favorites/create/" + id.ToString() + ".xml"), _
                            Nothing, _
                            content, _
                            Nothing)
    End Function

    Public Function DestroyFavorites(ByVal id As Long, ByRef content As String) As HttpStatusCode
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/favorites/destroy/" + id.ToString() + ".xml"), _
                            Nothing, _
                            content, _
                            Nothing)
    End Function

    Public Function HomeTimeline(ByVal count As Integer, ByVal max_id As Long, ByVal since_id As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        If count > 0 Then
            param.Add("count", count.ToString())
        End If
        If max_id > 0 Then
            param.Add("max_id", max_id.ToString())
        End If
        If since_id > 0 Then
            param.Add("since_id", since_id.ToString())
        End If

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/statuses/home_timeline.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
    End Function

    Public Function Mentions(ByVal count As Integer, ByVal max_id As Long, ByVal since_id As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        If count > 0 Then
            param.Add("count", count.ToString())
        End If
        If max_id > 0 Then
            param.Add("max_id", max_id.ToString())
        End If
        If since_id > 0 Then
            param.Add("since_id", since_id.ToString())
        End If

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/statuses/mentions.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
    End Function

    Public Function DirectMessages(ByVal count As Integer, ByVal max_id As Long, ByVal since_id As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        If count > 0 Then
            param.Add("count", count.ToString())
        End If
        If max_id > 0 Then
            param.Add("max_id", max_id.ToString())
        End If
        If since_id > 0 Then
            param.Add("since_id", since_id.ToString())
        End If

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/direct_messages.xml"), _
                            Nothing, _
                            content, _
                            _remainCountApi)
    End Function

    Public Function DirectMessagesSent(ByVal count As Integer, ByVal max_id As Long, ByVal since_id As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        If count > 0 Then
            param.Add("count", count.ToString())
        End If
        If max_id > 0 Then
            param.Add("max_id", max_id.ToString())
        End If
        If since_id > 0 Then
            param.Add("since_id", since_id.ToString())
        End If

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/direct_messages/sent.xml"), _
                            Nothing, _
                            content, _
                            _remainCountApi)
    End Function

    Public Function Favorites(ByVal count As Integer, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        If count <> 20 Then param.Add("count", count.ToString())

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/favorites.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
    End Function

    Public Function Search(ByVal words As String, ByVal lang As String, ByVal rpp As Integer, ByVal page As Integer, ByVal sinceId As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        If Not String.IsNullOrEmpty(words) Then param.Add("q", words)
        If Not String.IsNullOrEmpty(lang) Then param.Add("lang", lang)
        If rpp > 0 Then param.Add("rpp", rpp.ToString())
        If page > 0 Then param.Add("page", page.ToString())
        If sinceId > 0 Then param.Add("since_id", sinceId.ToString)

        If param.Count = 0 Then Return HttpStatusCode.BadRequest

        Return httpConVar.GetContent(GetMethod, _
                                        CreateTwitterSearchUri("/search.atom"), _
                                        param, _
                                        content, _
                                        Nothing, _
                                        "Tween")
    End Function

    Public Function FollowerIds(ByVal cursor As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("cursor", cursor.ToString())

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/followers/ids.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
    End Function

    Public Function RateLimitStatus(ByRef content As String) As HttpStatusCode
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/account/rate_limit_status.xml"), _
                            Nothing, _
                            content, _
                            Nothing)
    End Function

    Public Function GetLists(ByVal user As String, ByVal cursor As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("cursor", cursor.ToString)
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/" + user + "/lists.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
    End Function

    Public Function PostListID(ByVal user As String, ByVal list_id As String, ByVal name As String, ByVal mode As String, ByVal description As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        If name IsNot Nothing Then param.Add("name", name)
        If mode IsNot Nothing Then param.Add("mode", mode)
        If description IsNot Nothing Then param.Add("description", description)

        Return httpCon.GetContent(PostMethod, _
                                  CreateTwitterUri("/1/" + user + "/lists/" + list_id + ".xml"), _
                                  param, _
                                  content, _
                                  Nothing)
    End Function

    Public Function DeleteListID(ByVal user As String, ByVal list_id As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("_method", "DELETE")

        Return httpCon.GetContent(PostMethod, _
                                  CreateTwitterUri("/1/" + user + "/lists/" + list_id + ".xml"), _
                                  param, _
                                  content, _
                                  Nothing)
    End Function

    Public Function GetListsSubscriptions(ByVal user As String, ByVal cursor As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("cursor", cursor.ToString)
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/" + user + "/lists/subscriptions.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
    End Function

    Public Function GetListsStatuses(ByVal user As String, ByVal list_id As String, ByVal per_page As Integer, ByVal max_id As Long, ByVal since_id As Long, ByRef content As String) As HttpStatusCode
        '認証なくても取得できるが、protectedユーザー分が抜ける
        Dim param As New Dictionary(Of String, String)
        If per_page > 0 Then
            param.Add("per_page", per_page.ToString())
        End If
        If max_id > 0 Then
            param.Add("max_id", max_id.ToString())
        End If
        If since_id > 0 Then
            param.Add("since_id", since_id.ToString())
        End If

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/" + user + "/lists/" + list_id + "/statuses.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
    End Function

    Public Function PostLists(ByVal user As String, ByVal listname As String, ByVal isPrivate As Boolean, ByVal description As String, ByRef content As String) As HttpStatusCode
        Dim mode As String = "public"
        If isPrivate Then
            mode = "private"
        End If

        Dim param As New Dictionary(Of String, String)
        param.Add("name", listname)
        param.Add("mode", mode)
        If Not String.IsNullOrEmpty(description) Then
            param.Add("description", description)
        End If
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/" + user + "/lists.xml"), _
                            param, _
                            content, _
                            Nothing)
    End Function

    Public Function GetListNembers(ByVal user As String, ByVal list_id As String, ByVal cursor As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("cursor", cursor.ToString())
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/" + user + "/" + list_id + "/members.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
    End Function

    Public Function PostListMembers(ByVal user As String, ByVal list_id As String, ByVal id As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("id", id)
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/" + user + "/" + list_id + "/members.xml"), _
                            param, _
                            content, _
                            Nothing)
    End Function

    Public Function DeleteListMembers(ByVal user As String, ByVal list_id As String, ByVal id As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("id", id)
        param.Add("_method", "DELETE")
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/" + user + "/" + list_id + "/members.xml"), _
                            param, _
                            content, _
                            Nothing)
    End Function

    Public Function GetListMembersID(ByVal user As String, ByVal list_id As String, ByVal id As String, ByRef content As String) As HttpStatusCode
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/" + user + "/" + list_id + "/members/" + id + ".xml"), _
                            Nothing, _
                            content, _
                            _remainCountApi)
    End Function

    Public Function Statusid_retweeted_by_ids(ByVal statusid As Long, ByVal count As Integer, ByVal page As Integer, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        If count > 0 Then
            param.Add("count", count.ToString())
        End If
        If page > 0 Then
            param.Add("page", page.ToString())
        End If

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/statuses/" + statusid.ToString + "/retweeted_by/ids.xml"), _
                            param, _
                            content, _
                            _remainCountApi)
    End Function

    Public Function UpdateProfile(ByVal name As String, ByVal url As String, ByVal location As String, ByVal description As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)

        param.Add("name", name)
        param.Add("url", url)
        param.Add("location", location)
        param.Add("description", description)

        Return httpCon.GetContent(PostMethod, _
                    CreateTwitterUri("/1/account/update_profile.xml"), _
                    param, _
                    content, _
                    Nothing)
    End Function

    Public Function UpdateProfileImage(ByVal imageFile As FileInfo, ByRef content As String) As HttpStatusCode
        Dim binary As New List(Of KeyValuePair(Of String, FileInfo))
        binary.Add(New KeyValuePair(Of String, FileInfo)("image", imageFile))

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/account/update_profile_image.xml"), _
                            Nothing, _
                            binary, _
                            content, _
                            Nothing)
    End Function

#Region "Proxy API"
    Private Shared _twitterUrl As String = "api.twitter.com"
    'Private TwitterUrl As String = "sorayukigtap.appspot.com/api"
    Private Shared _TwitterSearchUrl As String = "search.twitter.com"
    'Private TwitterSearchUrl As String = "sorayukigtap.appspot.com/search"

    Private Function CreateTwitterUri(ByVal path As String) As Uri
        Return New Uri(String.Format("{0}{1}{2}", _protocol, _twitterUrl, path))
    End Function

    Private Function CreateTwitterSearchUri(ByVal path As String) As Uri
        Return New Uri(String.Format("{0}{1}{2}", _protocol, _TwitterSearchUrl, path))
    End Function

    Public Shared WriteOnly Property TwitterUrl() As String
        Set(ByVal value As String)
            _twitterUrl = value
            HttpOAuthApiProxy.ProxyHost = value
        End Set
    End Property

    Public Shared WriteOnly Property TwitterSearchUrl() As String
        Set(ByVal value As String)
            _TwitterSearchUrl = value
        End Set
    End Property
#End Region

End Class
