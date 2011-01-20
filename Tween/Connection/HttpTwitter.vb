' Tween - Client of Twitter
' Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
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

Imports System.Net
Imports System.IO
Imports System.Web
Imports System.Threading

Public Class HttpTwitter
    Implements ICloneable

    'OAuth関連
    '''<summary>
    '''OAuthのコンシューマー鍵
    '''</summary>
    Private Const ConsumerKey As String = "tLbG3uS0BIIE8jm1mKzKOfZ6EgUOmWVM"

    '''<summary>
    '''OAuthの署名作成用秘密コンシューマーデータ
    '''</summary>
    Private Const ConsumerSecret As String = "M0IMsbl2722iWa+CGPVcNeQmE+TFpJk8B/KW9UUTk3eLOl9Ij005r52JNxVukTzM"
    '''<summary>
    '''OAuthのアクセストークン取得先URI
    '''</summary>
    Private Const AccessTokenUrlXAuth As String = "https://api.twitter.com/oauth/access_token"

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
        End If
        con.Initialize(DecryptString(ConsumerKey), DecryptString(ConsumerSecret), accessToken, accessTokenSecret, username, "screen_name")
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

    Public Function AuthUserAndPass(ByVal username As String, ByVal password As String, ByRef content As String) As HttpStatusCode
        If connectionType = AuthMethod.Basic Then
            Return httpCon.Authenticate(CreateTwitterUri("/1/account/verify_credentials.json"), username, password, content)
        Else
            Return httpCon.Authenticate(New Uri(AccessTokenUrlXAuth), username, password, content)
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

    Public Function UpdateStatus(ByVal status As String, ByVal replyToId As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("status", status)
        If connectionType = AuthMethod.Basic Then param.Add("source", "Tween")
        If replyToId > 0 Then param.Add("in_reply_to_status_id", replyToId.ToString)

        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/statuses/update.json"), _
                            param, _
                            content, _
                            Nothing, _
                            Nothing)
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
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/statuses/retweet/" + id.ToString() + ".json"), _
                            Nothing, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function ShowUserInfo(ByVal screenName As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("screen_name", screenName)
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
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/statuses/show/" + id.ToString() + ".json"), _
                            Nothing, _
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

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/direct_messages.json"), _
                            Nothing, _
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

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/direct_messages/sent.json"), _
                            Nothing, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function Favorites(ByVal count As Integer, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        If count <> 20 Then param.Add("count", count.ToString())

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/favorites.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
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
                            CreateTwitterUri("/1/followers/ids.json"), _
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
                            Nothing, _
                            Nothing)
    End Function

    Public Function GetLists(ByVal user As String, ByVal cursor As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("cursor", cursor.ToString)
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/" + user + "/lists.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function PostListID(ByVal user As String, ByVal list_id As String, ByVal name As String, ByVal mode As String, ByVal description As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        If name IsNot Nothing Then param.Add("name", name)
        If mode IsNot Nothing Then param.Add("mode", mode)
        If description IsNot Nothing Then param.Add("description", description)

        Return httpCon.GetContent(PostMethod, _
                                  CreateTwitterUri("/1/" + user + "/lists/" + list_id + ".json"), _
                                  param, _
                                  content, _
                                  Nothing, _
                                  Nothing)
    End Function

    Public Function DeleteListID(ByVal user As String, ByVal list_id As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("_method", "DELETE")

        Return httpCon.GetContent(PostMethod, _
                                  CreateTwitterUri("/1/" + user + "/lists/" + list_id + ".json"), _
                                  param, _
                                  content, _
                                  Nothing, _
                                  Nothing)
    End Function

    Public Function GetListsSubscriptions(ByVal user As String, ByVal cursor As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("cursor", cursor.ToString)
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/" + user + "/lists/subscriptions.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
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
                            CreateTwitterUri("/1/" + user + "/lists/" + list_id + "/statuses.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
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
                            CreateTwitterUri("/1/" + user + "/lists.json"), _
                            param, _
                            content, _
                            Nothing,
                            Nothing)
    End Function

    Public Function GetListMembers(ByVal user As String, ByVal list_id As String, ByVal cursor As Long, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("cursor", cursor.ToString())
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/" + user + "/" + list_id + "/members.json"), _
                            param, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function

    Public Function PostListMembers(ByVal user As String, ByVal list_id As String, ByVal id As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("id", id)
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/" + user + "/" + list_id + "/members.json"), _
                            param, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function DeleteListMembers(ByVal user As String, ByVal list_id As String, ByVal id As String, ByRef content As String) As HttpStatusCode
        Dim param As New Dictionary(Of String, String)
        param.Add("id", id)
        param.Add("_method", "DELETE")
        Return httpCon.GetContent(PostMethod, _
                            CreateTwitterUri("/1/" + user + "/" + list_id + "/members.json"), _
                            param, _
                            content, _
                            Nothing, _
                            Nothing)
    End Function

    Public Function GetListMembersID(ByVal user As String, ByVal list_id As String, ByVal id As String, ByRef content As String) As HttpStatusCode
        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/" + user + "/" + list_id + "/members/" + id + ".json"), _
                            Nothing, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
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

        Return httpCon.GetContent(GetMethod, _
                            CreateTwitterUri("/1/related_results/show/" + id.ToString + ".json"), _
                            Nothing, _
                            content, _
                            TwitterApiInfo.HttpHeaders, _
                            AddressOf GetApiCallback)
    End Function


#Region "Proxy API"
    Private Shared _twitterUrl As String = "api.twitter.com"
    Private Shared _TwitterSearchUrl As String = "search.twitter.com"
    Private Shared _twitterStreamUrl As String = "userstream.twitter.com"

    Private Function CreateTwitterUri(ByVal path As String) As Uri
        Return New Uri(String.Format("{0}{1}{2}", _protocol, _twitterUrl, path))
    End Function

    Private Function CreateTwitterSearchUri(ByVal path As String) As Uri
        Return New Uri(String.Format("{0}{1}{2}", _protocol, _TwitterSearchUrl, path))
    End Function

    Private Function CreateTwitterStreamUri(ByVal path As String) As Uri
        Return New Uri(String.Format("{0}{1}{2}", "https://", _twitterStreamUrl, path))
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
                            CreateTwitterStreamUri("/2/user.json"), _
                            param, _
                            content,
                            userAgent)
    End Function

    Public Sub RequestAbort()
        httpCon.RequestAbort()
    End Sub

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Dim myCopy As New HttpTwitter
        If Me.connectionType = AuthMethod.Basic Then
            myCopy.Initialize(Me.AuthenticatedUsername, Me.Password)
        Else
            myCopy.Initialize(Me.AccessToken, Me.AccessTokenSecret, Me.AuthenticatedUsername)
        End If
        Return myCopy
    End Function
End Class
