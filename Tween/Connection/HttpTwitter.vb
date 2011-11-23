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

Imports System.IO
Imports System.Net

Public Class HttpTwitter
    Implements ICloneable

    'OAuth関連
    '''<summary>
    '''OAuthのコンシューマー鍵
    '''</summary>
    Private Const ConsumerKey As String = "ST6eAABKDRKTqbN7pPo2A"

    '''<summary>
    '''OAuthの署名作成用秘密コンシューマーデータ
    '''</summary>
    Private Const ConsumerSecret As String = "BJMEiivrXlqGESzdb8D0bvLfNYf3fifXRDMFjMogXg"
    '''<summary>
    '''OAuthのアクセストークン取得先URI
    '''</summary>
    Private Const AccessTokenUrlXAuth As String = "https://api.twitter.com/oauth/access_token"
    Private Const RequestTokenUrl As String = "https://api.twitter.com/oauth/request_token"
    Private Const AuthorizeUrl As String = "https://api.twitter.com/oauth/authorize"
    Private Const AccessTokenUrl As String = "https://api.twitter.com/oauth/access_token"

    Private Shared _protocol As String = "http://"

    Private Const PostMethod As String = "POST"
    Private Const GetMethod As String = "GET"

    Private httpCon As IHttpConnection 'HttpConnectionApi or HttpConnectionOAuth
    Private httpConVar As New HttpVarious

    Private Enum AuthMethod
        OAuth
        Basic
    End Enum
    Private connectionType As AuthMethod = AuthMethod.Basic

    Private requestToken As String

    Public Sub Initialize(ByVal accessToken As String, _
                                    ByVal accessTokenSecret As String, _
                                    ByVal username As String,
                                    ByVal userId As Long)
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
        End If
        con.Initialize(ConsumerKey, ConsumerSecret, accessToken, accessTokenSecret, username, userId, "screen_name", "user_id")
        httpCon = con
        connectionType = AuthMethod.OAuth
        requestToken = ""
    End Sub

    Public ReadOnly Property AccessToken() As String
        Get
            If httpCon IsNot Nothing Then
                Return DirectCast(httpCon, HttpConnectionOAuth).AccessToken
            Else
                Return ""
            End If
        End Get
    End Property

    Public ReadOnly Property AccessTokenSecret() As String
        Get
            If httpCon IsNot Nothing Then
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

    Public Property AuthenticatedUserId() As Long
        Get
            If httpCon IsNot Nothing Then
                Return httpCon.AuthUserId
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Long)
            If httpCon IsNot Nothing Then
                httpCon.AuthUserId = value
            End If
        End Set
    End Property

    Public ReadOnly Property Password() As String
        Get
            Return ""
        End Get
    End Property

    Public Function AuthGetRequestToken(ByRef content As String) As Boolean
        Dim authUri As Uri = Nothing
        Dim result As Boolean = DirectCast(httpCon, HttpOAuthApiProxy).AuthenticatePinFlowRequest(RequestTokenUrl, AuthorizeUrl, requestToken, authUri)
        content = authUri.ToString
        Return result
    End Function

    Public Function AuthGetAccessToken(ByVal pin As String) As HttpStatusCode
        Return DirectCast(httpCon, HttpOAuthApiProxy).AuthenticatePinFlow(AccessTokenUrl, requestToken, pin)
    End Function

    Public Function AuthUserAndPass(ByVal username As String, ByVal password As String, ByRef content As String) As HttpStatusCode
        Return httpCon.Authenticate(New Uri(AccessTokenUrlXAuth), username, password, content)
    End Function

    Public Sub ClearAuthInfo()
        Me.Initialize("", "", "", 0)
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

    Public Function UpdateStatus(ByVal status As String, ByVal replyToId As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("status", status)
        If replyToId > 0 Then param.Add("in_reply_to_status_id", replyToId.ToString)
        param.Add("include_entities", "true")
        'If AppendSettingDialog.Instance.ShortenTco AndAlso AppendSettingDialog.Instance.UrlConvertAuto Then param.Add("wrap_links", "true")

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/statuses/update.json"), _
                            param, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function UpdateStatusWithMedia(ByVal status As String, ByVal replyToId As Long, ByVal mediaFile As FileInfo, ByRef content As String) As HttpStatusCode
        '画像投稿用エンドポイント
        Dim param As New Dictionary(Of String, String)
        param.Add("status", status)
        If replyToId > 0 Then param.Add("in_reply_to_status_id", replyToId.ToString)
        param.Add("include_entities", "true")
        'If AppendSettingDialog.Instance.ShortenTco AndAlso AppendSettingDialog.Instance.UrlConvertAuto Then param.Add("wrap_links", "true")

        Dim binary As New List(Of KeyValuePair(Of String, FileInfo))
        binary.Add(New KeyValuePair(Of String, FileInfo)("media[]", mediaFile))

        Return httpCon.GetContent(PostMethod, _
                            New Uri("https://upload.twitter.com/1/statuses/update_with_media.json"), _
                            param, _
                            binary, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function DestroyStatus(ByVal id As Long) As HttpStatusCode
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/statuses/destroy/" + id.ToString + ".json"), _
                            Nothing, _
                            Nothing, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function SendDirectMessage(ByVal status As String, ByVal sendto As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("text", status)
        param.Add("screen_name", sendto)
        'If AppendSettingDialog.Instance.ShortenTco AndAlso AppendSettingDialog.Instance.UrlConvertAuto Then param.Add("wrap_links", "true")

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/direct_messages/new.json"), _
                            param, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function DestroyDirectMessage(ByVal id As Long) As HttpStatusCode
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/direct_messages/destroy/" + id.ToString + ".json"), _
                            Nothing, _
                            Nothing, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function RetweetStatus(ByVal id As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("include_entities", "true")

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/statuses/retweet/" + id.ToString() + ".json"), _
                            param, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function ShowUserInfo(ByVal screenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)
        param.Add("include_entities", "true")
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/users/show.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function CreateFriendships(ByVal screenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/friendships/create.json"), _
                            param, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function DestroyFriendships(ByVal screenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/friendships/destroy.json"), _
                            param, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function CreateBlock(ByVal screenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/blocks/create.json"), _
                            param, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function DestroyBlock(ByVal screenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/blocks/destroy.json"), _
                            param, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function ReportSpam(ByVal screenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/report_spam.json"), _
                            param, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function ShowFriendships(ByVal souceScreenName As String, ByVal targetScreenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("source_screen_name", souceScreenName)
        param.Add("target_screen_name", targetScreenName)

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/friendships/show.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function ShowStatuses(ByVal id As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("include_entities", "true")
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/statuses/show/" + id.ToString() + ".json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function CreateFavorites(ByVal id As Long, ByRef content As String) As HttpStatusCode
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/favorites/create/" + id.ToString() + ".json"), _
                            Nothing, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function DestroyFavorites(ByVal id As Long, ByRef content As String) As HttpStatusCode
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/favorites/destroy/" + id.ToString() + ".json"), _
                            Nothing, _
                            content, _
                            Nothing, _
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

        param.Add("include_entities", "true")

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/statuses/home_timeline.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function UserTimeline(ByVal user_id As Long, ByVal screen_name As String, ByVal count As Integer, ByVal max_id As Long, ByVal since_id As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)

        If (user_id = 0 AndAlso String.IsNullOrEmpty(screen_name)) OrElse
            (user_id <> 0 AndAlso Not String.IsNullOrEmpty(screen_name)) Then Return HttpStatusCode.BadRequest

        If user_id > 0 Then
            param.Add("user_id", user_id.ToString())
        End If
        If Not String.IsNullOrEmpty(screen_name) Then
            param.Add("screen_name", screen_name)
        End If
        If count > 0 Then
            param.Add("count", count.ToString())
        End If
        If max_id > 0 Then
            param.Add("max_id", max_id.ToString())
        End If
        If since_id > 0 Then
            param.Add("since_id", since_id.ToString())
        End If

        param.Add("include_rts", "true")
        param.Add("include_entities", "true")

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/statuses/user_timeline.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function PublicTimeline(ByVal count As Integer, ByVal max_id As Long, ByVal since_id As Long, ByRef content As String) As HttpStatusCode
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

        param.Add("include_entities", "true")

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/statuses/public_timeline.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
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

        param.Add("include_entities", "true")

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/statuses/mentions.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
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
        param.Add("include_entities", "true")

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/direct_messages.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
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
        param.Add("include_entities", "true")

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/direct_messages/sent.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function Favorites(ByVal count As Integer, ByVal page As Integer, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        If count <> 20 Then param.Add("count", count.ToString())

        If page > 0 Then
            param.Add("page", page.ToString())
        End If

        param.Add("include_entities", "true")

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/favorites.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Overloads Function PhoenixSearch(ByVal querystr As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        Dim tmp() As String
        Dim paramstr() As String
        If String.IsNullOrEmpty(querystr) Then Return HttpStatusCode.BadRequest

        tmp = querystr.Split(New Char() {"?"c, "&"c}, StringSplitOptions.RemoveEmptyEntries)
        For Each tmp2 As String In tmp
            paramstr = tmp2.Split(New Char() {"="c})
            param.Add(paramstr(0), paramstr(1))
        Next

        Return httpConVar.GetContent(GetMethod, _
                                CreateTwitterUri("/phoenix_search.phoenix"), _
                                param, _
                                content, _
                                Nothing, _
                                "Tween")
    End Function

    Public Overloads Function PhoenixSearch(ByVal words As String, ByVal lang As String, ByVal rpp As Integer, ByVal page As Integer, ByVal sinceId As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        If Not String.IsNullOrEmpty(words) Then param.Add("q", words)
        param.Add("include_entities", "1")
        param.Add("contributor_details", "true")
        If Not String.IsNullOrEmpty(lang) Then param.Add("lang", lang)
        If rpp > 0 Then param.Add("rpp", rpp.ToString())
        If page > 0 Then param.Add("page", page.ToString())
        If sinceId > 0 Then param.Add("since_id", sinceId.ToString)

        If param.Count = 0 Then Return HttpStatusCode.BadRequest

        Return httpConVar.GetContent(GetMethod, _
                                        CreateTwitterUri("/phoenix_search.phoenix"), _
                                        param, _
                                        content, _
                                        Nothing, _
                                        "Tween")
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

    Public Function SavedSearches(ByRef content As String) As HttpStatusCode
        Return httpCon.GetContent(GetMethod, _
                                        CreateTwitterUri("/1/saved_searches.json"), _
                                        Nothing, _
                                        content, _
                                        Nothing, _
                                        AddressOf GetApiCallback)
    End Function

    Public Function FollowerIds(ByVal cursor As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("cursor", cursor.ToString())

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/followers/ids.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function NoRetweetIds(ByVal cursor As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("cursor", cursor.ToString())

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/friendships/no_retweet_ids.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function RateLimitStatus(ByRef content As String) As HttpStatusCode
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/account/rate_limit_status.json"), _
                            Nothing, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

#Region "Lists"
    Public Function GetLists(ByVal user As String, ByVal cursor As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", user)
        param.Add("cursor", cursor.ToString)
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/lists.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function UpdateListID(ByVal user As String, ByVal list_id As String, ByVal name As String, ByVal isPrivate As Boolean, ByVal description As String, ByRef content As String) As HttpStatusCode
        Dim mode As String = "public"
        If isPrivate Then
            mode = "private"
        End If

        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", user)
        param.Add("list_id", list_id)
        If name IsNot Nothing Then param.Add("name", name)
        If mode IsNot Nothing Then param.Add("mode", mode)
        If description IsNot Nothing Then param.Add("description", description)

        Return httpCon.GetContent(PostMethod, _
                                  CreateTwitterUri("/1/lists/update.json"), _
                                  param, _
                                  content, _
                                  Nothing, _
                                  Nothing)
    End Function

    Public Function DeleteListID(ByVal user As String, ByVal list_id As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", user)
        param.Add("list_id", list_id)

        Return httpCon.GetContent(PostMethod, _
                                  CreateTwitterUri("/1/lists/destroy.json"), _
                                  param, _
                                  content, _
                                  Nothing, _
                                  Nothing)
    End Function

    Public Function GetListsSubscriptions(ByVal user As String, ByVal cursor As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", user)
        param.Add("cursor", cursor.ToString)
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/lists/subscriptions.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function GetListsStatuses(ByVal userId As Long, ByVal list_id As Long, ByVal per_page As Integer, ByVal max_id As Long, ByVal since_id As Long, ByVal isRTinclude As Boolean, ByRef content As String) As HttpStatusCode
        '認証なくても取得できるが、protectedユーザー分が抜ける
        Dim param As New Dictionary(Of String, String)
        param.Add("user_id", userId.ToString)
        param.Add("list_id", list_id.ToString)
        If isRTinclude Then
            param.Add("include_rts", "true")
        End If
        If per_page > 0 Then
            param.Add("per_page", per_page.ToString())
        End If
        If max_id > 0 Then
            param.Add("max_id", max_id.ToString())
        End If
        If since_id > 0 Then
            param.Add("since_id", since_id.ToString())
        End If
        param.Add("include_entities", "true")

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/lists/statuses.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function CreateLists(ByVal listname As String, ByVal isPrivate As Boolean, ByVal description As String, ByRef content As String) As HttpStatusCode
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
                            CreateTwitterUri("/1/lists/create.json"), _
                            param, _
                            content, _
                            Nothing,
                            Nothing)
    End Function

    Public Function GetListMembers(ByVal user As String, ByVal list_id As String, ByVal cursor As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", user)
        param.Add("list_id", list_id)
        param.Add("cursor", cursor.ToString())
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/lists/members.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function CreateListMembers(ByVal list_id As String, ByVal memberName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("list_id", list_id)
        param.Add("screen_name", memberName)
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/lists/members/create.json"), _
                            param, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    'Public Function CreateListMembers(ByVal user As String, ByVal list_id As String, ByVal memberName As String, ByRef content As String) As HttpStatusCode
    '    '正常に動かないので旧APIで様子見
    '    'Dim param As New Dictionary(Of String, String)
    '    'param.Add("screen_name", user)
    '    'param.Add("list_id", list_id)
    '    'param.Add("member_screen_name", memberName)
    '    'Return httpCon.GetContent(PostMethod, _
    '    '                    CreateTwitterUri("/1/lists/members/create.json"), _
    '    '                    param, _
    '    '                    content, _
    '    '                    Nothing, _
    '    '                    Nothing)
    '    Dim param As New Dictionary(Of String, String)
    '    param.Add("id", memberName)
    '    Return httpCon.GetContent(PostMethod, _
    '                        CreateTwitterUri("/1/" + user + "/" + list_id + "/members.json"), _
    '                        param, _
    '                        content, _
    '                        Nothing, _
    '                        Nothing)
    'End Function

    Public Function DeleteListMembers(ByVal list_id As String, ByVal memberName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", memberName)
        param.Add("list_id", list_id)
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/lists/members/destroy.json"), _
                            param, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    'Public Function DeleteListMembers(ByVal user As String, ByVal list_id As String, ByVal memberName As String, ByRef content As String) As HttpStatusCode
    '    'Dim param As New Dictionary(Of String, String)
    '    'param.Add("screen_name", user)
    '    'param.Add("list_id", list_id)
    '    'param.Add("member_screen_name", memberName)
    '    'Return httpCon.GetContent(PostMethod, _
    '    '                    CreateTwitterUri("/1/lists/members/destroy.json"), _
    '    '                    param, _
    '    '                    content, _
    '    '                    Nothing, _
    '    '                    Nothing)
    '    Dim param As New Dictionary(Of String, String)
    '    param.Add("id", memberName)
    '    param.Add("_method", "DELETE")
    '    Return httpCon.GetContent(PostMethod, _
    '                        CreateTwitterUri("/1/" + user + "/" + list_id + "/members.json"), _
    '                        param, _
    '                        content, _
    '                        Nothing, _
    '                        Nothing)
    'End Function

    Public Function ShowListMember(ByVal list_id As String, ByVal memberName As String, ByRef content As String) As HttpStatusCode
        '新APIがmember_screen_nameもmember_user_idも無視して、自分のIDを返してくる。
        '正式にドキュメントに反映されるまで旧APIを使用する
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", memberName)
        param.Add("list_id", list_id)
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/lists/members/show.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
        'Return httpCon.GetContent(GetMethod, _
        '                    CreateTwitterUri("/1/" + user + "/" + list_id + "/members/" + id + ".json"), _
        '                    Nothing, _
        '                    content, _
        '                    TwitterApiInfo.HttpHeaders, _
        '                    AddressOf GetApiCallback)
    End Function
#End Region

    Public Function Statusid_retweeted_by_ids(ByVal statusid As Long, ByVal count As Integer, ByVal page As Integer, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        If count > 0 Then
            param.Add("count", count.ToString())
        End If
        If page > 0 Then
            param.Add("page", page.ToString())
        End If

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/statuses/" + statusid.ToString + "/retweeted_by/ids.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function UpdateProfile(ByVal name As String, ByVal url As String, ByVal location As String, ByVal description As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)

        param.Add("name", name)
        param.Add("url", url)
        param.Add("location", location)
        param.Add("description", description)
        param.Add("include_entities", "true")

        Return httpCon.GetContent(PostMethod, _
                    CreateTwitterUri("/1/account/update_profile.json"), _
                    param, _
                    content, _
                    Nothing, _
                    Nothing)
    End Function

    Public Function UpdateProfileImage(ByVal imageFile As FileInfo, ByRef content As String) As HttpStatusCode
        Dim binary As New List(Of KeyValuePair(Of String, FileInfo))
        binary.Add(New KeyValuePair(Of String, FileInfo)("image", imageFile))

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/account/update_profile_image.json"), _
                            Nothing, _
                            binary, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function GetRelatedResults(ByVal id As Long, ByRef content As String) As HttpStatusCode
        '認証なくても取得できるが、protectedユーザー分が抜ける
        Dim param As New Dictionary(Of String, String)

        param.Add("include_entities", "true")

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/related_results/show/" + id.ToString + ".json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function GetBlockUserIds(ByRef content As String) As HttpStatusCode
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/blocks/blocking/ids.json"), _
                            Nothing, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function GetConfiguration(ByRef content As String) As HttpStatusCode
        Return httpCon.GetContent(GetMethod, _
                                        CreateTwitterUri("/1/help/configuration.json"), _
                                        Nothing, _
                                        content, _
                                        TwitterApiInfo.HttpHeaders, _
                                        AddressOf GetApiCallback)
    End Function

    Public Function VerifyCredentials(ByRef content As String) As HttpStatusCode
        Return httpCon.GetContent(GetMethod, _
                                        CreateTwitterUri("/1/account/verify_credentials.json"), _
                                        Nothing, _
                                        content, _
                                        TwitterApiInfo.HttpHeaders, _
                                        AddressOf GetApiCallback)
    End Function

#Region "Proxy API"
    Private Shared _twitterUrl As String = "api.twitter.com"
    Private Shared _TwitterSearchUrl As String = "search.twitter.com"
    Private Shared _twitterUserStreamUrl As String = "userstream.twitter.com"
    Private Shared _twitterStreamUrl As String = "stream.twitter.com"

    Private Function CreateTwitterUri(ByVal path As String) As Uri
        Return New Uri(String.Format("{0}{1}{2}", _protocol, _twitterUrl, path))
    End Function

    Private Function CreateTwitterSearchUri(ByVal path As String) As Uri
        Return New Uri(String.Format("{0}{1}{2}", _protocol, _TwitterSearchUrl, path))
    End Function

    Private Function CreateTwitterUserStreamUri(ByVal path As String) As Uri
        Return New Uri(String.Format("{0}{1}{2}", "https://", _twitterUserStreamUrl, path))
    End Function

    Private Function CreateTwitterStreamUri(ByVal path As String) As Uri
        Return New Uri(String.Format("{0}{1}{2}", "http://", _twitterStreamUrl, path))
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

    Private Sub GetApiCallback(ByVal sender As Object, ByRef code As HttpStatusCode, ByRef content As String)
        If code < HttpStatusCode.InternalServerError Then
            TwitterApiInfo.ParseHttpHeaders(TwitterApiInfo.HttpHeaders)
        End If
    End Sub

    Public Function UserStream(ByRef content As Stream,
                               ByVal allAtReplies As Boolean,
                               ByVal trackwords As String,
                               ByVal userAgent As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)

        If allAtReplies Then
            param.Add("replies", "all")
        End If

        If Not String.IsNullOrEmpty(trackwords) Then
            param.Add("track", trackwords)
        End If

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUserStreamUri("/2/user.json"), _
                            param, _
                            content,
                            userAgent)
    End Function

    Public Function FilterStream(ByRef content As Stream,
                               ByVal trackwords As String,
                               ByVal userAgent As String) As HttpStatusCode
        '文中の日本語キーワードに反応せず、使えない（スペースで分かち書きしてないと反応しない）
        Dim param As New Dictionary(Of String, String)

        If Not String.IsNullOrEmpty(trackwords) Then
            param.Add("track", String.Join(",", trackwords.Split(" ".ToCharArray)))
        End If

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterStreamUri("/1/statuses/filter.json"), _
                            param, _
                            content,
                            userAgent)
    End Function

    Public Sub RequestAbort()
        httpCon.RequestAbort()
    End Sub

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Dim myCopy As New HttpTwitter
        myCopy.Initialize(Me.AccessToken, Me.AccessTokenSecret, Me.AuthenticatedUsername, Me.AuthenticatedUserId)
        Return myCopy
    End Function
End Class
