﻿' Tween - Client of Twitter
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

Imports System.Diagnostics
Imports System.IO
Imports System.Linq
Imports System.Net
Imports System.Reflection.MethodBase
Imports System.Runtime.Serialization
Imports System.Runtime.Serialization.Json
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Web
Imports System.Xml
Imports System.Xml.Linq

Public Class Twitter
    Implements IDisposable

    'Hashtag用正規表現
    Private Const LATIN_ACCENTS As String = "\xc0-\xd6\xd8-\xf6\xf8-\xff"
    Private Const NON_LATIN_HASHTAG_CHARS As String = "\u0400-\u04ff\u0500-\u0527\u1100-\u11ff\u3130-\u3185\uA960-\uA97F\uAC00-\uD7AF\uD7B0-\uD7FF"
    'Private Const CJ_HASHTAG_CHARACTERS As String = "\u30A1-\u30FA\uFF66-\uFF9F\uFF10-\uFF19\uFF21-\uFF3A\uFF41-\uFF5A\u3041-\u3096\u3400-\u4DBF\u4E00-\u9FFF\u20000-\u2A6DF\u2A700-\u2B73F\u2B740-\u2B81F\u2F800-\u2FA1F"
    Private Const CJ_HASHTAG_CHARACTERS As String = "\u30A1-\u30FA\u30FC\u3005\uFF66-\uFF9F\uFF10-\uFF19\uFF21-\uFF3A\uFF41-\uFF5A\u3041-\u309A\u3400-\u4DBF\p{IsCJKUnifiedIdeographs}"
    Private Const HASHTAG_BOUNDARY As String = "^|$|\s|「|」|。|\.|!"
    Private Const HASHTAG_ALPHA As String = "[a-z_" + LATIN_ACCENTS + NON_LATIN_HASHTAG_CHARS + CJ_HASHTAG_CHARACTERS + "]"
    Private Const HASHTAG_ALPHANUMERIC As String = "[a-z0-9_" + LATIN_ACCENTS + NON_LATIN_HASHTAG_CHARS + CJ_HASHTAG_CHARACTERS + "]"
    Private Const HASHTAG_TERMINATOR As String = "[^a-z0-9_" + LATIN_ACCENTS + NON_LATIN_HASHTAG_CHARS + CJ_HASHTAG_CHARACTERS + "]"
    Public Const HASHTAG As String = "(" + HASHTAG_BOUNDARY + ")(#|＃)(" + HASHTAG_ALPHANUMERIC + "*" + HASHTAG_ALPHA + HASHTAG_ALPHANUMERIC + "*)(?=" + HASHTAG_TERMINATOR + "|" + HASHTAG_BOUNDARY + ")"
    'URL正規表現
    Private Const url_valid_domain As String = "(?<domain>(?:[^\p{P}\s][\.\-_](?=[^\p{P}\s])|[^\p{P}\s]){1,}\.[a-z]{2,}(?::[0-9]+)?)"
    Private Const url_valid_general_path_chars As String = "[a-z0-9!*';:=+$/%#\[\]\-_&,~]"
    Private Const url_balance_parens As String = "(?:\(" + url_valid_general_path_chars + "+\))"
    Private Const url_valid_url_path_ending_chars As String = "(?:[a-z0-9=_#/\-\+]+|" + url_balance_parens + ")"
    Private Const pth As String = "(?:" + url_balance_parens +
        "|@" + url_valid_general_path_chars + "+/" +
        "|[.,]?" + url_valid_general_path_chars + "+" +
        ")"
    Private Const pth2 As String = "(/(?:" +
        pth + "+" + url_valid_url_path_ending_chars + "|" +
        pth + "+" + url_valid_url_path_ending_chars + "?|" +
        url_valid_url_path_ending_chars +
        ")?)?"
    Private Const qry As String = "(?<query>\?[a-z0-9!*'();:&=+$/%#\[\]\-_.,~]*[a-z0-9_&=#])?"
    Public Const rgUrl As String = "(?<before>(?:[^\""':!=#]|^|\:/))" +
                                "(?<url>(?<protocol>https?://)" +
                                url_valid_domain +
                                pth2 +
                                qry +
                                ")"
    Delegate Sub GetIconImageDelegate(ByVal post As PostClass)
    Private ReadOnly LockObj As New Object
    Private followerId As New List(Of Long)
    Private _GetFollowerResult As Boolean = False
    Private noRTId As New List(Of Long)
    Private _GetNoRetweetResult As Boolean = False

    Private _followersCount As Integer = 0
    Private _friendsCount As Integer = 0
    Private _statusesCount As Integer = 0
    Private _location As String = ""
    Private _bio As String = ""
    Private _protocol As String = "https://"

    'プロパティからアクセスされる共通情報
    Private _uname As String
    Private _iconSz As Integer
    Private _getIcon As Boolean
    Private _dIcon As IDictionary(Of String, Image)

    Private _tinyUrlResolve As Boolean
    Private _restrictFavCheck As Boolean

    Private _hubServer As String
    Private _readOwnPost As Boolean
    Private _hashList As New List(Of String)

    '共通で使用する状態
    Private _remainCountApi As Integer = -1

    Private op As New Outputz
    'max_idで古い発言を取得するために保持（lists分は個別タブで管理）
    Private minHomeTimeline As Long = Long.MaxValue
    Private minMentions As Long = Long.MaxValue
    Private minDirectmessage As Long = Long.MaxValue
    Private minDirectmessageSent As Long = Long.MaxValue

    'Private favQueue As FavoriteQueue

    Private twCon As New HttpTwitter

    Public Event UserIdChanged()

    'Private _deletemessages As New List(Of PostClass)

    Public Overloads Function Authenticate(ByVal username As String, ByVal password As String) As String

        Dim res As HttpStatusCode
        Dim content As String = ""

        TwitterApiInfo.Initialize()
        Try
            res = twCon.AuthUserAndPass(username, password, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                _uname = username.ToLower
                If AppendSettingDialog.Instance.UserstreamStartup Then Me.ReconnectUserStream()
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return My.Resources.Unauthorized + Environment.NewLine + content
                Else
                    Return "Auth error:" + errMsg
                End If
            Case HttpStatusCode.Forbidden
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Err:Forbidden"
                Else
                    Return "Err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select

    End Function

    Public Function StartAuthentication(ByRef pinPageUrl As String) As String
        'OAuth PIN Flow
        Dim res As Boolean
        Dim content As String = ""

        TwitterApiInfo.Initialize()
        Try
            res = twCon.AuthGetRequestToken(pinPageUrl)
        Catch ex As Exception
            Return "Err:" + "Failed to access auth server."
        End Try

        Return ""
    End Function

    Public Overloads Function Authenticate(ByVal pinCode As String) As String

        Dim res As HttpStatusCode
        Dim content As String = ""

        TwitterApiInfo.Initialize()
        Try
            res = twCon.AuthGetAccessToken(pinCode)
        Catch ex As Exception
            Return "Err:" + "Failed to access auth acc server."
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                _uname = Username.ToLower
                If AppendSettingDialog.Instance.UserstreamStartup Then Me.ReconnectUserStream()
                Google.GASender.GetInstance().TrackEventWithCategory("post", "authenticate", Me.UserId)
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Check the PIN or retry." + Environment.NewLine + content
                Else
                    Return "Auth error:" + errMsg
                End If
            Case HttpStatusCode.Forbidden
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Err:Forbidden"
                Else
                    Return "Err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select

    End Function

    Public Sub ClearAuthInfo()
        Twitter.AccountState = ACCOUNT_STATE.Invalid
        TwitterApiInfo.Initialize()
        twCon.ClearAuthInfo()
    End Sub

    Public Sub VerifyCredentials()
        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.VerifyCredentials(content)
        Catch ex As Exception
            Exit Sub
        End Try

        If res = HttpStatusCode.OK Then
            Twitter.AccountState = ACCOUNT_STATE.Valid
            Dim user As TwitterDataModel.User
            Try
                user = CreateDataFromJson(Of TwitterDataModel.User)(content)
            Catch ex As SerializationException
                Exit Sub
            End Try
            twCon.AuthenticatedUserId = user.Id
        End If
    End Sub

    Private Function GetErrorMessageJson(ByVal content As String) As String
        Try
            If Not String.IsNullOrEmpty(content) Then
                Using jsonReader As XmlDictionaryReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(content), XmlDictionaryReaderQuotas.Max)
                    Dim xElm As XElement = XElement.Load(jsonReader)
                    If xElm.Element("error") IsNot Nothing Then
                        Return xElm.Element("error").Value
                    Else
                        Return ""
                    End If
                End Using
            Else
                Return ""
            End If
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Sub Initialize(ByVal token As String, ByVal tokenSecret As String, ByVal username As String, ByVal userId As Long)
        'OAuth認証
        If String.IsNullOrEmpty(token) OrElse String.IsNullOrEmpty(tokenSecret) OrElse String.IsNullOrEmpty(username) Then
            Twitter.AccountState = ACCOUNT_STATE.Invalid
        End If
        TwitterApiInfo.Initialize()
        twCon.Initialize(token, tokenSecret, username, userId)
        _uname = username.ToLower
        If AppendSettingDialog.Instance.UserstreamStartup Then Me.ReconnectUserStream()
    End Sub

    Public Function PreProcessUrl(ByVal orgData As String) As String
        Dim posl1 As Integer
        Dim posl2 As Integer = 0
        'Dim IDNConveter As IdnMapping = New IdnMapping()
        Dim href As String = "<a href="""

        Do While True
            If orgData.IndexOf(href, posl2, StringComparison.Ordinal) > -1 Then
                Dim urlStr As String = ""
                ' IDN展開
                posl1 = orgData.IndexOf(href, posl2, StringComparison.Ordinal)
                posl1 += href.Length
                posl2 = orgData.IndexOf("""", posl1, StringComparison.Ordinal)
                urlStr = orgData.Substring(posl1, posl2 - posl1)

                If Not urlStr.StartsWith("http://") AndAlso Not urlStr.StartsWith("https://") AndAlso Not urlStr.StartsWith("ftp://") Then
                    Continue Do
                End If

                Dim replacedUrl As String = IDNDecode(urlStr)
                If replacedUrl Is Nothing Then Continue Do
                If replacedUrl = urlStr Then Continue Do

                orgData = orgData.Replace("<a href=""" + urlStr, "<a href=""" + replacedUrl)
                posl2 = 0
            Else
                Exit Do
            End If
        Loop
        Return orgData
    End Function

    Private Function GetPlainText(ByVal orgData As String) As String
        Return HttpUtility.HtmlDecode(Regex.Replace(orgData, "(?<tagStart><a [^>]+>)(?<text>[^<]+)(?<tagEnd></a>)", "${text}"))
    End Function

    ' htmlの簡易サニタイズ(詳細表示に不要なタグの除去)

    Private Function SanitizeHtml(ByVal orgdata As String) As String
        Dim retdata As String = orgdata

        retdata = Regex.Replace(retdata, "<(script|object|applet|image|frameset|fieldset|legend|style).*" & _
            "</(script|object|applet|image|frameset|fieldset|legend|style)>", "", RegexOptions.IgnoreCase)

        retdata = Regex.Replace(retdata, "<(frame|link|iframe|img)>", "", RegexOptions.IgnoreCase)

        Return retdata
    End Function

    Private Function AdjustHtml(ByVal orgData As String) As String
        Dim retStr As String = orgData
        'Dim m As Match = Regex.Match(retStr, "<a [^>]+>[#|＃](?<1>[a-zA-Z0-9_]+)</a>")
        'While m.Success
        '    SyncLock LockObj
        '        _hashList.Add("#" + m.Groups(1).Value)
        '    End SyncLock
        '    m = m.NextMatch
        'End While
        retStr = Regex.Replace(retStr, "<a [^>]*href=""/", "<a href=""" + _protocol + "twitter.com/")
        retStr = retStr.Replace("<a href=", "<a target=""_self"" href=")
        retStr = retStr.Replace(vbLf, "<br>")

        '半角スペースを置換(Thanks @anis774)
        Dim ret As Boolean = False
        Do
            ret = EscapeSpace(retStr)
        Loop While Not ret

        Return SanitizeHtml(retStr)
    End Function

    Private Function EscapeSpace(ByRef html As String) As Boolean
        '半角スペースを置換(Thanks @anis774)
        Dim isTag As Boolean = False
        For i As Integer = 0 To html.Length - 1
            If html(i) = "<"c Then
                isTag = True
            End If
            If html(i) = ">"c Then
                isTag = False
            End If

            If (Not isTag) AndAlso (html(i) = " "c) Then
                html = html.Remove(i, 1)
                html = html.Insert(i, "&nbsp;")
                Return False
            End If
        Next
        Return True
    End Function

    Private Structure PostInfo
        Public CreatedAt As String
        Public Id As String
        Public Text As String
        Public UserId As String
        Public Sub New(ByVal Created As String, ByVal IdStr As String, ByVal txt As String, ByVal uid As String)
            CreatedAt = Created
            Id = IdStr
            Text = txt
            UserId = uid
        End Sub
        Public Shadows Function Equals(ByVal dst As PostInfo) As Boolean
            If Me.CreatedAt = dst.CreatedAt AndAlso Me.Id = dst.Id AndAlso Me.Text = dst.Text AndAlso Me.UserId = dst.UserId Then
                Return True
            Else
                Return False
            End If
        End Function
    End Structure

    Private Function IsPostRestricted(ByVal status As TwitterDataModel.Status) As Boolean
        Static _prev As New PostInfo("", "", "", "")
        Dim _current As New PostInfo("", "", "", "")

        _current.CreatedAt = status.CreatedAt
        _current.Id = status.IdStr
        If status.Text Is Nothing Then
            _current.Text = ""
        Else
            _current.Text = status.Text
        End If
        _current.UserId = status.User.IdStr

        If _current.Equals(_prev) Then
            Return True
        End If
        _prev.CreatedAt = _current.CreatedAt
        _prev.Id = _current.Id
        _prev.Text = _current.Text
        _prev.UserId = _current.UserId

        Return False
    End Function

    Public Function PostStatus(ByVal postStr As String, ByVal reply_to As Long) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        postStr = postStr.Trim()

        If Regex.Match(postStr, "^DM? +(?<id>[a-zA-Z0-9_]+) +(?<body>.+)", RegexOptions.IgnoreCase Or RegexOptions.Singleline).Success Then
            Return SendDirectMessage(postStr)
        End If

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.UpdateStatus(postStr, reply_to, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "status", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Dim status As TwitterDataModel.Status
                Try
                    status = CreateDataFromJson(Of TwitterDataModel.Status)(content)
                Catch ex As SerializationException
                    TraceOut(ex.Message + Environment.NewLine + content)
                    Return "Err:Json Parse Error(DataContractJsonSerializer)"
                Catch ex As Exception
                    TraceOut(ex, GetCurrentMethod.Name & " " & content)
                    Return "Err:Invalid Json!"
                End Try
                _followersCount = status.User.FollowersCount
                _friendsCount = status.User.FriendsCount
                _statusesCount = status.User.StatusesCount
                _location = status.User.Location
                _bio = status.User.Description

                If IsPostRestricted(status) Then
                    Return "OK:Delaying?"
                End If
                If op.Post(postStr.Length) Then
                    Return ""
                Else
                    Return "Outputz:Failed"
                End If
            Case HttpStatusCode.NotFound
                Return ""
            Case HttpStatusCode.Forbidden, HttpStatusCode.BadRequest
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Warn:" + res.ToString
                Else
                    Return "Warn:" + errMsg
                End If
            Case HttpStatusCode.Conflict, _
                HttpStatusCode.ExpectationFailed, _
                HttpStatusCode.Gone, _
                HttpStatusCode.LengthRequired, _
                HttpStatusCode.MethodNotAllowed, _
                HttpStatusCode.NotAcceptable, _
                HttpStatusCode.NotFound, _
                HttpStatusCode.PaymentRequired, _
                HttpStatusCode.PreconditionFailed, _
                HttpStatusCode.RequestedRangeNotSatisfiable, _
                HttpStatusCode.RequestEntityTooLarge, _
                HttpStatusCode.RequestTimeout, _
                HttpStatusCode.RequestUriTooLong
                '仕様書にない400系エラー。サーバまでは到達しているのでリトライしない
                Return "Warn:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return My.Resources.Unauthorized
                Else
                    Return "Auth err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostStatusWithMedia(ByVal postStr As String, ByVal reply_to As Long, ByVal mediaFile As FileInfo) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        postStr = postStr.Trim()

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.UpdateStatusWithMedia(postStr, reply_to, mediaFile, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "status_with_media", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Dim status As TwitterDataModel.Status
                Try
                    status = CreateDataFromJson(Of TwitterDataModel.Status)(content)
                Catch ex As SerializationException
                    TraceOut(ex.Message + Environment.NewLine + content)
                    Return "Err:Json Parse Error(DataContractJsonSerializer)"
                Catch ex As Exception
                    TraceOut(ex, GetCurrentMethod.Name & " " & content)
                    Return "Err:Invalid Json!"
                End Try
                _followersCount = status.User.FollowersCount
                _friendsCount = status.User.FriendsCount
                _statusesCount = status.User.StatusesCount
                _location = status.User.Location
                _bio = status.User.Description

                If IsPostRestricted(status) Then
                    Return "OK:Delaying?"
                End If
                If op.Post(postStr.Length) Then
                    Return ""
                Else
                    Return "Outputz:Failed"
                End If
            Case HttpStatusCode.NotFound
                Return ""
            Case HttpStatusCode.Forbidden, HttpStatusCode.BadRequest
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Warn:" + res.ToString
                Else
                    Return "Warn:" + errMsg
                End If
            Case HttpStatusCode.Conflict, _
                HttpStatusCode.ExpectationFailed, _
                HttpStatusCode.Gone, _
                HttpStatusCode.LengthRequired, _
                HttpStatusCode.MethodNotAllowed, _
                HttpStatusCode.NotAcceptable, _
                HttpStatusCode.NotFound, _
                HttpStatusCode.PaymentRequired, _
                HttpStatusCode.PreconditionFailed, _
                HttpStatusCode.RequestedRangeNotSatisfiable, _
                HttpStatusCode.RequestEntityTooLarge, _
                HttpStatusCode.RequestTimeout, _
                HttpStatusCode.RequestUriTooLong
                '仕様書にない400系エラー。サーバまでは到達しているのでリトライしない
                Return "Warn:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return My.Resources.Unauthorized
                Else
                    Return "Auth err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function SendDirectMessage(ByVal postStr As String) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""
        If TwitterApiInfo.AccessLevel <> ApiAccessLevel.None Then
            If Not TwitterApiInfo.IsDirectMessagePermission Then Return "Auth Err:try to re-authorization."
        End If

        postStr = postStr.Trim()

        Dim res As HttpStatusCode
        Dim content As String = ""

        Dim mc As Match = Regex.Match(postStr, "^DM? +(?<id>[a-zA-Z0-9_]+) +(?<body>.+)", RegexOptions.IgnoreCase Or RegexOptions.Singleline)

        Try
            res = twCon.SendDirectMessage(mc.Groups("body").Value, mc.Groups("id").Value, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "direct_message", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Dim status As TwitterDataModel.Directmessage
                Try
                    status = CreateDataFromJson(Of TwitterDataModel.Directmessage)(content)
                Catch ex As SerializationException
                    TraceOut(ex.Message + Environment.NewLine + content)
                    Return "Err:Json Parse Error(DataContractJsonSerializer)"
                Catch ex As Exception
                    TraceOut(ex, GetCurrentMethod.Name & " " & content)
                    Return "Err:Invalid Json!"
                End Try
                _followersCount = status.Sender.FollowersCount
                _friendsCount = status.Sender.FriendsCount
                _statusesCount = status.Sender.StatusesCount
                _location = status.Sender.Location
                _bio = status.Sender.Description

                If op.Post(postStr.Length) Then
                    Return ""
                Else
                    Return "Outputz:Failed"
                End If
            Case HttpStatusCode.Forbidden, HttpStatusCode.BadRequest
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Warn:" + res.ToString
                Else
                    Return "Warn:" + errMsg
                End If
            Case HttpStatusCode.Conflict, _
                HttpStatusCode.ExpectationFailed, _
                HttpStatusCode.Gone, _
                HttpStatusCode.LengthRequired, _
                HttpStatusCode.MethodNotAllowed, _
                HttpStatusCode.NotAcceptable, _
                HttpStatusCode.NotFound, _
                HttpStatusCode.PaymentRequired, _
                HttpStatusCode.PreconditionFailed, _
                HttpStatusCode.RequestedRangeNotSatisfiable, _
                HttpStatusCode.RequestEntityTooLarge, _
                HttpStatusCode.RequestTimeout, _
                HttpStatusCode.RequestUriTooLong
                '仕様書にない400系エラー。サーバまでは到達しているのでリトライしない
                Return "Warn:" + res.ToString
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return My.Resources.Unauthorized
                Else
                    Return "Auth err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function RemoveStatus(ByVal id As Long) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode

        Try
            res = twCon.DestroyStatus(id)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "destroy", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.NotFound
                Return ""
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select

    End Function

    Public Function PostRetweet(ByVal id As Long, ByVal read As Boolean) As String
        If _endingFlag Then Return ""
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        'データ部分の生成
        Dim target As Long = id
        Dim post As PostClass = TabInformations.GetInstance.Item(id)
        If post Is Nothing Then
            Return "Err:Target isn't found."
        End If
        If TabInformations.GetInstance.Item(id).RetweetedId > 0 Then
            target = TabInformations.GetInstance.Item(id).RetweetedId '再RTの場合は元発言をRT
        End If

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.RetweetStatus(target, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.Unauthorized
                'Blockユーザーの発言をRTすると認証エラー返る
                'Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized + " or blocked user."
            Case Is <> HttpStatusCode.OK
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Google.GASender.GetInstance().TrackEventWithCategory("post", "retweet", Me.UserId)
        Twitter.AccountState = ACCOUNT_STATE.Valid

        Dim status As TwitterDataModel.Status
        Try
            status = CreateDataFromJson(Of TwitterDataModel.Status)(content)
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Err:Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Err:Invalid Json!"
        End Try

        'ReTweetしたものをTLに追加
        post = CreatePostsFromStatusData(status)
        If post Is Nothing Then Return "Invalid Json!"

        '二重取得回避
        SyncLock LockObj
            If TabInformations.GetInstance.ContainsKey(post.StatusId) Then Return ""
        End SyncLock
        'Retweet判定
        If post.RetweetedId = 0 Then Return "Invalid Json!"
        'ユーザー情報
        post.IsMe = True

        post.IsRead = read
        post.IsOwl = False
        If _readOwnPost Then post.IsRead = True
        post.IsDm = False

        TabInformations.GetInstance.AddPost(post)

        Return ""
    End Function

    Public Function RemoveDirectMessage(ByVal id As Long, ByVal post As PostClass) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""
        If TwitterApiInfo.AccessLevel <> ApiAccessLevel.None Then
            If Not TwitterApiInfo.IsDirectMessagePermission Then Return "Auth Err:try to re-authorization."
        End If

        Dim res As HttpStatusCode

        'If post.IsMe Then
        '    _deletemessages.Add(post)
        'End If
        Try
            res = twCon.DestroyDirectMessage(id)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "destroy_direct_message", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.NotFound
                Return ""
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostFollowCommand(ByVal screenName As String) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.CreateFriendships(screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "follow", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.Forbidden
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Err:Forbidden(" + GetCurrentMethod.Name + ")"
                Else
                    Return "Err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostRemoveCommand(ByVal screenName As String) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.DestroyFriendships(screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "destroy_friendships", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.Forbidden
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Err:Forbidden(" + GetCurrentMethod.Name + ")"
                Else
                    Return "Err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostCreateBlock(ByVal screenName As String) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.CreateBlock(screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "block", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.Forbidden
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Err:Forbidden(" + GetCurrentMethod.Name + ")"
                Else
                    Return "Err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostDestroyBlock(ByVal screenName As String) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.DestroyBlock(screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "destroy_block", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.Forbidden
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Err:Forbidden(" + GetCurrentMethod.Name + ")"
                Else
                    Return "Err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostReportSpam(ByVal screenName As String) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.ReportSpam(screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "spam", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.Forbidden
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Err:Forbidden(" + GetCurrentMethod.Name + ")"
                Else
                    Return "Err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function GetFriendshipInfo(ByVal screenName As String, ByRef isFollowing As Boolean, ByRef isFollowed As Boolean) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Google.GASender.GetInstance().TrackPage("/friendships", Me.UserId)
        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.ShowFriendships(_uname, screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Try
                    Dim relation = CreateDataFromJson(Of TwitterDataModel.Relationship)(content)
                    isFollowing = relation.Relationship.Source.Following
                    isFollowed = relation.Relationship.Source.FollowedBy
                    Return ""
                Catch ex As SerializationException
                    TraceOut(ex.Message + Environment.NewLine + content)
                    Return "Err:Json Parse Error(DataContractJsonSerializer)"
                Catch ex As Exception
                    TraceOut(ex, GetCurrentMethod.Name & " " & content)
                    Return "Err:Invalid Json!"
                End Try
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function GetUserInfo(ByVal screenName As String, ByRef user As TwitterDataModel.User) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Google.GASender.GetInstance().TrackPage("/showuser", Me.UserId)
        Dim res As HttpStatusCode
        Dim content As String = ""
        user = Nothing
        Try
            res = twCon.ShowUserInfo(screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Try
                    user = CreateDataFromJson(Of TwitterDataModel.User)(content)
                Catch ex As SerializationException
                    TraceOut(ex.Message + Environment.NewLine + content)
                    Return "Err:Json Parse Error(DataContractJsonSerializer)"
                Catch ex As Exception
                    TraceOut(ex, GetCurrentMethod.Name & " " & content)
                    Return "Err:Invalid Json!"
                End Try
                Return ""
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return My.Resources.Unauthorized
                Else
                    Return "Auth err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function GetStatus_Retweeted_Count(ByVal StatusId As Long, ByRef retweeted_count As Integer) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Google.GASender.GetInstance().TrackPage("/retweet_count", Me.UserId)
        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim xmlBuf As String = ""

        retweeted_count = 0

        ' 注：dev.twitter.comに記述されているcountパラメータは間違い。100が正しい
        For i As Integer = 1 To 100

            Try
                res = twCon.Statusid_retweeted_by_ids(StatusId, 100, i, content)
            Catch ex As Exception
                Return "Err:" + ex.Message
            End Try

            Select Case res
                Case HttpStatusCode.OK
                    Try
                        Dim ids As Int64() = CreateDataFromJson(Of Int64())(content)
                        retweeted_count += ids.Length
                        If ids.Length < 100 Then Exit For
                    Catch ex As SerializationException
                        retweeted_count = -1
                        TraceOut(ex.Message + Environment.NewLine + content)
                        Return "Err:Json Parse Error(DataContractJsonSerializer)"
                    Catch ex As Exception
                        retweeted_count = -1
                        TraceOut(ex, GetCurrentMethod.Name & " " & content)
                        Return "Err:Invalid Json!"
                    End Try
                Case HttpStatusCode.BadRequest
                    retweeted_count = -1
                    Return "Err:API Limits?"
                Case HttpStatusCode.Unauthorized
                    retweeted_count = -1
                    Twitter.AccountState = ACCOUNT_STATE.Invalid
                    Return My.Resources.Unauthorized
                Case Else
                    retweeted_count = -1
                    Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
            End Select
        Next
        Return ""
    End Function

    Public Function PostFavAdd(ByVal id As Long) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        'If Me.favQueue Is Nothing Then Me.favQueue = New FavoriteQueue(Me)

        'If Me.favQueue.Contains(id) Then Me.favQueue.Remove(id)

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.CreateFavorites(id, content)
        Catch ex As Exception
            'Me.favQueue.Add(id)
            'Return "Err:->FavoriteQueue:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "favorites", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
                'Me.favQueue.FavoriteCacheStart()
                If Not _restrictFavCheck Then Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.Forbidden
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Err:Forbidden(" + GetCurrentMethod.Name + ")"
                Else
                    'If errMsg.Contains("It's great that you like so many updates") Then
                    '    'Me.favQueue.Add(id)
                    '    Return "Err:->FavoriteQueue:" + errMsg
                    'End If
                    Return "Err:" + errMsg
                End If
                'Case HttpStatusCode.BadGateway, HttpStatusCode.ServiceUnavailable, HttpStatusCode.InternalServerError, HttpStatusCode.RequestTimeout
                '    'Me.favQueue.Add(id)
                '    Return "Err:->FavoriteQueue:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select

        'http://twitter.com/statuses/show/id.xml APIを発行して本文を取得

        'Dim content As String = ""
        content = ""
        Try
            res = twCon.ShowStatuses(id, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Dim status As TwitterDataModel.Status
                Try
                    status = CreateDataFromJson(Of TwitterDataModel.Status)(content)
                Catch ex As SerializationException
                    TraceOut(ex.Message + Environment.NewLine + content)
                    Return "Err:Json Parse Error(DataContractJsonSerializer)"
                Catch ex As Exception
                    TraceOut(ex, GetCurrentMethod.Name & " " & content)
                    Return "Err:Invalid Json!"
                End Try
                If status.Favorited Then
                    Return ""
                Else
                    Return "NG(Restricted?)"
                End If
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select

    End Function

    Public Function PostFavRemove(ByVal id As Long) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        'If Me.favQueue Is Nothing Then Me.favQueue = New FavoriteQueue(Me)

        'If Me.favQueue.Contains(id) Then
        '    Me.favQueue.Remove(id)
        '    Return ""
        'End If

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.DestroyFavorites(id, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "destroy_favorites", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.Forbidden
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Err:Forbidden(" + GetCurrentMethod.Name + ")"
                Else
                    Return "Err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostUpdateProfile(ByVal name As String, ByVal url As String, ByVal location As String, ByVal description As String) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.UpdateProfile(name, url, location, description, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "update_profile", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.Forbidden
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Err:Forbidden(" + GetCurrentMethod.Name + ")"
                Else
                    Return "Err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public Function PostUpdateProfileImage(ByVal filename As String) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.UpdateProfileImage(New FileInfo(filename), content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "update_profile_image", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.Forbidden
                Dim errMsg As String = GetErrorMessageJson(content)
                If String.IsNullOrEmpty(errMsg) Then
                    Return "Err:Forbidden(" + GetCurrentMethod.Name + ")"
                Else
                    Return "Err:" + errMsg
                End If
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select
    End Function

    Public ReadOnly Property Username() As String
        Get
            Return twCon.AuthenticatedUsername
        End Get
    End Property

    Public ReadOnly Property UserId As Long
        Get
            Return twCon.AuthenticatedUserId
        End Get
    End Property

    Public ReadOnly Property Password() As String
        Get
            Return twCon.Password
        End Get
    End Property

    Private Shared _accountState As ACCOUNT_STATE = ACCOUNT_STATE.Valid
    Public Shared Property AccountState() As ACCOUNT_STATE
        Get
            Return _accountState
        End Get
        Set(ByVal value As ACCOUNT_STATE)
            _accountState = value
        End Set
    End Property

    Public WriteOnly Property GetIcon() As Boolean
        Set(ByVal value As Boolean)
            _getIcon = value
        End Set
    End Property

    Public WriteOnly Property TinyUrlResolve() As Boolean
        Set(ByVal value As Boolean)
            _tinyUrlResolve = value
        End Set
    End Property

    Public WriteOnly Property RestrictFavCheck() As Boolean
        Set(ByVal value As Boolean)
            _restrictFavCheck = value
        End Set
    End Property

    Public WriteOnly Property IconSize() As Integer
        Set(ByVal value As Integer)
            _iconSz = value
        End Set
    End Property

#Region "バージョンアップ"
    Public Function GetVersionInfo() As String
        Dim content As String = ""
        If Not (New HttpVarious).GetData("http://tween.sourceforge.jp/version.txt?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Nothing, content, GetUserAgentString()) Then
            Throw New Exception("GetVersionInfo Failed")
        End If
        Return content
    End Function

    Public Function GetTweenBinary(ByVal strVer As String) As String
        Google.GASender.GetInstance().TrackPage("/newversion", Me.UserId)
        Try
            '本体
            If Not (New HttpVarious).GetDataToFile("http://tween.sourceforge.jp/Tween" + strVer + ".gz?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), _
                                                Path.Combine(MyCommon.settingPath, "TweenNew.exe")) Then
                Return "Err:Download failed"
            End If
            '英語リソース
            If Not Directory.Exists(Path.Combine(MyCommon.settingPath, "en")) Then
                Directory.CreateDirectory(Path.Combine(MyCommon.settingPath, "en"))
            End If
            If Not (New HttpVarious).GetDataToFile("http://tween.sourceforge.jp/TweenResEn" + strVer + ".gz?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), _
                                                Path.Combine(Path.Combine(MyCommon.settingPath, "en"), "Tween.resourcesNew.dll")) Then
                Return "Err:Download failed"
            End If
            'その他言語圏のリソース。取得失敗しても継続
            'UIの言語圏のリソース
            Dim curCul As String = ""
            If Not Thread.CurrentThread.CurrentUICulture.IsNeutralCulture Then
                Dim idx As Integer = Thread.CurrentThread.CurrentUICulture.Name.LastIndexOf("-"c)
                If idx > -1 Then
                    curCul = Thread.CurrentThread.CurrentUICulture.Name.Substring(0, idx)
                Else
                    curCul = Thread.CurrentThread.CurrentUICulture.Name
                End If
            Else
                curCul = Thread.CurrentThread.CurrentUICulture.Name
            End If
            If Not String.IsNullOrEmpty(curCul) AndAlso curCul <> "en" AndAlso curCul <> "ja" Then
                If Not Directory.Exists(Path.Combine(MyCommon.settingPath, curCul)) Then
                    Directory.CreateDirectory(Path.Combine(MyCommon.settingPath, curCul))
                End If
                If Not (New HttpVarious).GetDataToFile("http://tween.sourceforge.jp/TweenRes" + curCul + strVer + ".gz?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), _
                                                    Path.Combine(Path.Combine(MyCommon.settingPath, curCul), "Tween.resourcesNew.dll")) Then
                    'Return "Err:Download failed"
                End If
            End If
            'スレッドの言語圏のリソース
            Dim curCul2 As String
            If Not Thread.CurrentThread.CurrentCulture.IsNeutralCulture Then
                Dim idx As Integer = Thread.CurrentThread.CurrentCulture.Name.LastIndexOf("-"c)
                If idx > -1 Then
                    curCul2 = Thread.CurrentThread.CurrentCulture.Name.Substring(0, idx)
                Else
                    curCul2 = Thread.CurrentThread.CurrentCulture.Name
                End If
            Else
                curCul2 = Thread.CurrentThread.CurrentCulture.Name
            End If
            If Not String.IsNullOrEmpty(curCul2) AndAlso curCul2 <> "en" AndAlso curCul2 <> curCul Then
                If Not Directory.Exists(Path.Combine(MyCommon.settingPath, curCul2)) Then
                    Directory.CreateDirectory(Path.Combine(MyCommon.settingPath, curCul2))
                End If
                If Not (New HttpVarious).GetDataToFile("http://tween.sourceforge.jp/TweenRes" + curCul2 + strVer + ".gz?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), _
                                                Path.Combine(Path.Combine(MyCommon.settingPath, curCul2), "Tween.resourcesNew.dll")) Then
                    'Return "Err:Download failed"
                End If
            End If

            'アップデータ
            If Not (New HttpVarious).GetDataToFile("http://tween.sourceforge.jp/TweenUp3.gz?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), _
                                                Path.Combine(MyCommon.settingPath, "TweenUp3.exe")) Then
                Return "Err:Download failed"
            End If
            'シリアライザDLL
            If Not (New HttpVarious).GetDataToFile("http://tween.sourceforge.jp/TweenDll" + strVer + ".gz?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), _
                                                Path.Combine(MyCommon.settingPath, "TweenNew.XmlSerializers.dll")) Then
                Return "Err:Download failed"
            End If
            Return ""
        Catch ex As Exception
            Return "Err:Download failed"
        End Try
    End Function
#End Region

    Public Property DetailIcon() As IDictionary(Of String, Image)
        Get
            Return _dIcon
        End Get
        Set(ByVal value As IDictionary(Of String, Image))
            _dIcon = value
        End Set
    End Property

    Public Property ReadOwnPost() As Boolean
        Get
            Return _readOwnPost
        End Get
        Set(ByVal value As Boolean)
            _readOwnPost = value
        End Set
    End Property

    Public ReadOnly Property FollowersCount() As Integer
        Get
            Return _followersCount
        End Get
    End Property

    Public ReadOnly Property FriendsCount() As Integer
        Get
            Return _friendsCount
        End Get
    End Property

    Public ReadOnly Property StatusesCount() As Integer
        Get
            Return _statusesCount
        End Get
    End Property

    Public ReadOnly Property Location() As String
        Get
            Return _location
        End Get
    End Property

    Public ReadOnly Property Bio() As String
        Get
            Return _bio
        End Get
    End Property

    Public WriteOnly Property UseSsl() As Boolean
        Set(ByVal value As Boolean)
            HttpTwitter.UseSsl = value
            If value Then
                _protocol = "https://"
            Else
                _protocol = "http://"
            End If
        End Set
    End Property

    Public Function GetTimelineApi(ByVal read As Boolean, _
                            ByVal gType As WORKERTYPE, _
                            ByVal more As Boolean, _
                            ByVal startup As Boolean) As String

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim count As Integer = AppendSettingDialog.Instance.CountApi
        If gType = WORKERTYPE.Reply Then count = AppendSettingDialog.Instance.CountApiReply()
        If AppendSettingDialog.Instance.UseAdditionalCount Then
            If more AndAlso AppendSettingDialog.Instance.MoreCountApi <> 0 Then
                count = AppendSettingDialog.Instance.MoreCountApi
            ElseIf startup AndAlso AppendSettingDialog.Instance.FirstCountApi <> 0 AndAlso gType = WORKERTYPE.Timeline Then
                count = AppendSettingDialog.Instance.FirstCountApi
            End If
        End If
        Try
            If gType = WORKERTYPE.Timeline Then
                If more Then
                    res = twCon.HomeTimeline(count, Me.minHomeTimeline, 0, content)
                Else
                    res = twCon.HomeTimeline(count, 0, 0, content)
                End If
            Else
                If more Then
                    res = twCon.Mentions(count, Me.minMentions, 0, content)
                Else
                    res = twCon.Mentions(count, 0, 0, content)
                End If
            End If
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        If gType = WORKERTYPE.Timeline Then
            Return CreatePostsFromJson(content, gType, Nothing, read, count, Me.minHomeTimeline)
        Else
            Return CreatePostsFromJson(content, gType, Nothing, read, count, Me.minMentions)
        End If
    End Function

    Public Function GetUserTimelineApi(ByVal read As Boolean,
                                       ByVal count As Integer,
                                       ByVal userName As String,
                                       ByVal tab As TabClass,
                                       ByVal more As Boolean) As String

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        If count = 0 Then count = 20
        Try
            If String.IsNullOrEmpty(userName) Then
                Dim target As String = tab.User
                If String.IsNullOrEmpty(target) Then Return ""
                userName = target
                res = twCon.UserTimeline(0, target, count, 0, 0, content)
            Else
                If more Then
                    res = twCon.UserTimeline(0, userName, count, tab.OldestId, 0, content)
                Else
                    res = twCon.UserTimeline(0, userName, count, 0, 0, content)
                End If
            End If
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return "Err:@" + userName + "'s Tweets are protected."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Dim items As List(Of TwitterDataModel.Status)
        Try
            items = CreateDataFromJson(Of List(Of TwitterDataModel.Status))(content)
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Invalid Json!"
        End Try

        For Each status As TwitterDataModel.Status In items
            Dim item As PostClass = CreatePostsFromStatusData(status)
            If item Is Nothing Then Continue For
            If item.StatusId < tab.OldestId Then tab.OldestId = item.StatusId
            item.IsRead = read
            If item.IsMe AndAlso Not read AndAlso _readOwnPost Then item.IsRead = True
            If tab IsNot Nothing Then item.RelTabName = tab.TabName
            '非同期アイコン取得＆StatusDictionaryに追加
            TabInformations.GetInstance.AddPost(item)
        Next

        Return ""
    End Function

    Public Function GetStatusApi(ByVal read As Boolean,
                                       ByVal id As Int64,
                                       ByRef post As PostClass) As String
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        If _endingFlag Then Return ""

        Google.GASender.GetInstance().TrackPage("/showstatus", Me.UserId)
        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.ShowStatuses(id, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case HttpStatusCode.Forbidden
                Return "Err:Protected user's tweet"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Dim status As TwitterDataModel.Status
        Try
            status = CreateDataFromJson(Of TwitterDataModel.Status)(content)
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Invalid Json!"
        End Try

        Dim item As PostClass = CreatePostsFromStatusData(status)
        If item Is Nothing Then Return "Err:Can't create post"
        item.IsRead = read
        If item.IsMe AndAlso Not read AndAlso _readOwnPost Then item.IsRead = True

        post = item
        Return ""
    End Function

    Public Function GetStatusApi(ByVal read As Boolean,
                                       ByVal id As Int64,
                                       ByVal tab As TabClass) As String
        Dim post As PostClass = Nothing
        Dim r As String = Me.GetStatusApi(read, id, post)

        If r = "" Then
            If tab IsNot Nothing Then post.RelTabName = tab.TabName
            '非同期アイコン取得＆StatusDictionaryに追加
            TabInformations.GetInstance.AddPost(post)
        End If

        Return r
    End Function

    Private Function CreatePostsFromStatusData(ByVal status As TwitterDataModel.Status) As PostClass
        Dim post As New PostClass
        Dim entities As TwitterDataModel.Entities

        post.StatusId = status.Id
        If status.RetweetedStatus IsNot Nothing Then
            Dim retweeted As TwitterDataModel.RetweetedStatus = status.RetweetedStatus

            post.CreatedAt = DateTimeParse(retweeted.CreatedAt)

            'Id
            post.RetweetedId = retweeted.Id
            '本文
            post.TextFromApi = retweeted.Text
            entities = retweeted.Entities
            'Source取得（htmlの場合は、中身を取り出し）
            post.Source = retweeted.Source
            'Reply先
            Long.TryParse(retweeted.InReplyToStatusId, post.InReplyToStatusId)
            post.InReplyToUser = retweeted.InReplyToScreenName
            Long.TryParse(status.InReplyToUserId, post.InReplyToUserId)

            '幻覚fav対策
            Dim tc As TabClass = TabInformations.GetInstance.GetTabByType(TabUsageType.Favorites)
            post.IsFav = tc.Contains(post.RetweetedId)

            If retweeted.Geo IsNot Nothing Then post.PostGeo = New PostClass.StatusGeo() With {.Lat = retweeted.Geo.Coordinates(0), .Lng = retweeted.Geo.Coordinates(1)}

            '以下、ユーザー情報
            Dim user As TwitterDataModel.User = retweeted.User

            If user.ScreenName Is Nothing OrElse status.User.ScreenName Is Nothing Then Return Nothing

            post.UserId = user.Id
            post.ScreenName = user.ScreenName
            post.Nickname = user.Name.Trim()
            post.ImageUrl = user.ProfileImageUrl
            post.IsProtect = user.Protected

            'Retweetした人
            post.RetweetedBy = status.User.ScreenName
            post.RetweetedByUserId = status.User.Id
            post.IsMe = post.RetweetedBy.ToLower.Equals(_uname)
        Else
            post.CreatedAt = DateTimeParse(status.CreatedAt)
            '本文
            post.TextFromApi = status.Text
            entities = status.Entities
            'Source取得（htmlの場合は、中身を取り出し）
            post.Source = status.Source
            Long.TryParse(status.InReplyToStatusId, post.InReplyToStatusId)
            post.InReplyToUser = status.InReplyToScreenName
            Long.TryParse(status.InReplyToUserId, post.InReplyToUserId)

            If status.Geo IsNot Nothing Then post.PostGeo = New PostClass.StatusGeo() With {.Lat = status.Geo.Coordinates(0), .Lng = status.Geo.Coordinates(1)}

            '以下、ユーザー情報
            Dim user As TwitterDataModel.User = status.User

            If user.ScreenName Is Nothing Then Return Nothing

            post.UserId = user.Id
            post.ScreenName = user.ScreenName
            post.Nickname = user.Name.Trim()
            post.ImageUrl = user.ProfileImageUrl
            post.IsProtect = user.Protected
            post.IsMe = post.ScreenName.ToLower.Equals(_uname)

            '幻覚fav対策
            Dim tc As TabClass = TabInformations.GetInstance.GetTabByType(TabUsageType.Favorites)
            post.IsFav = tc.Contains(post.StatusId) AndAlso TabInformations.GetInstance.Item(post.StatusId).IsFav
        End If
        'HTMLに整形
        post.Text = CreateHtmlAnchor(post.TextFromApi, post.ReplyToList, entities, post.Media)
        post.TextFromApi = Me.ReplaceTextFromApi(post.TextFromApi, entities)
        post.TextFromApi = HttpUtility.HtmlDecode(post.TextFromApi)
        post.TextFromApi = post.TextFromApi.Replace("<3", "♡")

        'Source整形
        CreateSource(post)

        post.IsReply = post.ReplyToList.Contains(_uname)
        post.IsExcludeReply = False

        If post.IsMe Then
            post.IsOwl = False
        Else
            If followerId.Count > 0 Then post.IsOwl = Not followerId.Contains(post.UserId)
        End If

        post.IsDm = False
        Return post
    End Function

    Private Function CreatePostsFromJson(ByVal content As String, ByVal gType As WORKERTYPE, ByVal tab As TabClass, ByVal read As Boolean, ByVal count As Integer, ByRef minimumId As Long) As String
        Dim items As List(Of TwitterDataModel.Status)
        Try
            items = CreateDataFromJson(Of List(Of TwitterDataModel.Status))(content)
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Invalid Json!"
        End Try

        For Each status As TwitterDataModel.Status In items
            Dim post As PostClass = Nothing
            post = CreatePostsFromStatusData(status)
            If post Is Nothing Then Continue For

            If minimumId > post.StatusId Then minimumId = post.StatusId
            '二重取得回避
            SyncLock LockObj
                If tab Is Nothing Then
                    If TabInformations.GetInstance.ContainsKey(post.StatusId) Then Continue For
                Else
                    If TabInformations.GetInstance.ContainsKey(post.StatusId, tab.TabName) Then Continue For
                End If
            End SyncLock

            'RT禁止ユーザーによるもの
            If post.RetweetedId > 0 AndAlso Me.noRTId.Contains(post.RetweetedByUserId) Then Continue For

            post.IsRead = read
            If post.IsMe AndAlso Not read AndAlso _readOwnPost Then post.IsRead = True

            If tab IsNot Nothing Then post.RelTabName = tab.TabName
            '非同期アイコン取得＆StatusDictionaryに追加
            TabInformations.GetInstance.AddPost(post)
        Next

        Return ""
    End Function

    Private Function CreatePostsFromPhoenixSearch(ByVal content As String, ByVal gType As WORKERTYPE, ByVal tab As TabClass, ByVal read As Boolean, ByVal count As Integer, ByRef minimumId As Long, ByRef nextPageQuery As String) As String
        Dim items As TwitterDataModel.SearchResult
        Try
            items = CreateDataFromJson(Of TwitterDataModel.SearchResult)(content)
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Invalid Json!"
        End Try

        nextPageQuery = items.NextPage

        For Each status As TwitterDataModel.Status In items.Statuses
            Dim post As PostClass = Nothing
            post = CreatePostsFromStatusData(status)
            If post Is Nothing Then Continue For

            If minimumId > post.StatusId Then minimumId = post.StatusId
            '二重取得回避
            SyncLock LockObj
                If tab Is Nothing Then
                    If TabInformations.GetInstance.ContainsKey(post.StatusId) Then Continue For
                Else
                    If TabInformations.GetInstance.ContainsKey(post.StatusId, tab.TabName) Then Continue For
                End If
            End SyncLock

            post.IsRead = read
            If post.IsMe AndAlso Not read AndAlso _readOwnPost Then post.IsRead = True

            If tab IsNot Nothing Then post.RelTabName = tab.TabName
            '非同期アイコン取得＆StatusDictionaryに追加
            TabInformations.GetInstance.AddPost(post)
        Next

        Return If(String.IsNullOrEmpty(items.ErrMsg), "", "Err:" + items.ErrMsg)
    End Function

    Public Overloads Function GetListStatus(ByVal read As Boolean, _
                            ByVal tab As TabClass, _
                            ByVal more As Boolean, _
                            ByVal startup As Boolean) As String

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim page As Integer = 0
        Dim count As Integer
        If AppendSettingDialog.Instance.UseAdditionalCount Then
            count = AppendSettingDialog.Instance.ListCountApi
            If count = 0 Then
                If more AndAlso AppendSettingDialog.Instance.MoreCountApi <> 0 Then
                    count = AppendSettingDialog.Instance.MoreCountApi
                ElseIf startup AndAlso AppendSettingDialog.Instance.FirstCountApi <> 0 Then
                    count = AppendSettingDialog.Instance.FirstCountApi
                Else
                    count = AppendSettingDialog.Instance.CountApi
                End If
            End If
        Else
            count = AppendSettingDialog.Instance.CountApi
        End If
        Try
            If more Then
                res = twCon.GetListsStatuses(tab.ListInfo.UserId, tab.ListInfo.Id, count, tab.OldestId, 0, AppendSettingDialog.Instance.IsListStatusesIncludeRts, content)
            Else
                res = twCon.GetListsStatuses(tab.ListInfo.UserId, tab.ListInfo.Id, count, 0, 0, AppendSettingDialog.Instance.IsListStatusesIncludeRts, content)
            End If
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Return CreatePostsFromJson(content, WORKERTYPE.List, tab, read, count, tab.OldestId)
    End Function


    Private Function CheckReplyToPost(ByVal relPosts As List(Of PostClass)) As PostClass
        Dim tmpPost As PostClass = relPosts(0)
        Dim lastPost As PostClass = Nothing
        Do While tmpPost IsNot Nothing
            If tmpPost.InReplyToStatusId = 0 Then Return Nothing
            lastPost = tmpPost
            Dim replyToPost = From p In relPosts
                             Where p.StatusId = tmpPost.InReplyToStatusId
                             Select p
            tmpPost = replyToPost.FirstOrDefault()
        Loop
        Return lastPost
    End Function

    Public Function GetRelatedResult(ByVal read As Boolean, ByVal tab As TabClass) As String
        Google.GASender.GetInstance().TrackPage("/related_statuses", Me.UserId)
        Dim rslt As String = ""
        Dim relPosts As New List(Of PostClass)
        If tab.RelationTargetPost.TextFromApi.Contains("@") AndAlso tab.RelationTargetPost.InReplyToStatusId = 0 Then
            '検索結果対応
            Dim p As PostClass = TabInformations.GetInstance.Item(tab.RelationTargetPost.StatusId)
            If p IsNot Nothing AndAlso p.InReplyToStatusId > 0 Then
                tab.RelationTargetPost = p
            Else
                rslt = Me.GetStatusApi(read, tab.RelationTargetPost.StatusId, p)
                If Not String.IsNullOrEmpty(rslt) Then Return rslt
                tab.RelationTargetPost = p
            End If
        End If
        relPosts.Add(tab.RelationTargetPost.Copy)
        Dim tmpPost As PostClass = relPosts(0)
        Do
            rslt = Me.GetRelatedResultsApi(read, tmpPost, tab, relPosts)
            If Not String.IsNullOrEmpty(rslt) Then Exit Do
            tmpPost = CheckReplyToPost(relPosts)
        Loop While tmpPost IsNot Nothing

        relPosts.ForEach(Sub(p) TabInformations.GetInstance.AddPost(p))
        Return rslt
    End Function

    Private Function GetRelatedResultsApi(ByVal read As Boolean,
                                         ByVal post As PostClass,
                                         ByVal tab As TabClass,
                                         ByVal relatedPosts As List(Of PostClass)) As String

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            If post.RetweetedId > 0 Then
                res = twCon.GetRelatedResults(post.RetweetedId, content)
            Else
                res = twCon.GetRelatedResults(post.StatusId, content)
            End If
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Dim items As List(Of TwitterDataModel.RelatedResult)
        Try
            items = CreateDataFromJson(Of List(Of TwitterDataModel.RelatedResult))(content)
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Invalid Json!"
        End Try

        Dim targetItem As PostClass = post
        If targetItem Is Nothing Then
            Return ""
        Else
            targetItem = targetItem.Copy()
        End If
        targetItem.RelTabName = tab.TabName
        TabInformations.GetInstance.AddPost(targetItem)

        Dim replyToItem As PostClass = Nothing
        Dim replyToUserName As String = targetItem.InReplyToUser
        If targetItem.InReplyToStatusId > 0 AndAlso TabInformations.GetInstance.Item(targetItem.InReplyToStatusId) IsNot Nothing Then
            replyToItem = TabInformations.GetInstance.Item(targetItem.InReplyToStatusId).Copy
            replyToItem.IsRead = read
            If replyToItem.IsMe AndAlso Not read AndAlso _readOwnPost Then replyToItem.IsRead = True
            replyToItem.RelTabName = tab.TabName
        End If

        Dim replyAdded As Boolean = False
        For Each relatedData As TwitterDataModel.RelatedResult In items
            For Each result As TwitterDataModel.RelatedTweet In relatedData.Results
                Dim item As PostClass = CreatePostsFromStatusData(result.Status)
                If item Is Nothing Then Continue For
                If targetItem.InReplyToStatusId = item.StatusId Then
                    replyToItem = Nothing
                    replyAdded = True
                End If
                item.IsRead = read
                If item.IsMe AndAlso Not read AndAlso _readOwnPost Then item.IsRead = True
                If tab IsNot Nothing Then item.RelTabName = tab.TabName
                '非同期アイコン取得＆StatusDictionaryに追加
                relatedPosts.Add(item)
            Next
        Next
        If replyToItem IsNot Nothing Then
            relatedPosts.Add(replyToItem)
        ElseIf targetItem.InReplyToStatusId > 0 AndAlso Not replyAdded Then
            Dim p As PostClass = Nothing
            Dim rslt As String = ""
            rslt = GetStatusApi(read, targetItem.InReplyToStatusId, p)
            If String.IsNullOrEmpty(rslt) Then
                p.IsRead = read
                p.RelTabName = tab.TabName
                relatedPosts.Add(p)
            End If
            Return rslt
        End If

        '発言者・返信先ユーザーの直近10発言取得
        'Dim rslt As String = Me.GetUserTimelineApi(read, 10, "", tab)
        'If Not String.IsNullOrEmpty(rslt) Then Return rslt
        'If Not String.IsNullOrEmpty(replyToUserName) Then
        '    rslt = Me.GetUserTimelineApi(read, 10, replyToUserName, tab)
        'End If
        'Return rslt


        'MRTとかに対応のためツイート内にあるツイートを指すURLを取り込む
        Dim ma As MatchCollection = Regex.Matches(tab.RelationTargetPost.Text, "title=""https?://twitter.com/(#!/)?(?<ScreenName>[a-zA-Z0-9_]+)(/status(es)?/(?<StatusId>[0-9]+))""")
        For Each _match As Match In ma
            Dim _statusId As Int64
            If Int64.TryParse(_match.Groups("StatusId").Value, _statusId) Then
                Dim p As PostClass = Nothing
                Dim _post As PostClass = TabInformations.GetInstance.Item(_statusId)
                If _post Is Nothing Then
                    Dim rslt = Me.GetStatusApi(read, _statusId, p)
                Else
                    p = _post.Copy
                End If
                If p IsNot Nothing Then
                    p.IsRead = read
                    p.RelTabName = tab.TabName
                    relatedPosts.Add(p)
                End If
            End If
        Next
        Return ""
    End Function

    Public Function GetSearch(ByVal read As Boolean, _
                        ByVal tab As TabClass, _
                        ByVal more As Boolean) As String

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim page As Integer = 0
        Dim sinceId As Long = 0
        Dim count As Integer = 100
        If AppendSettingDialog.Instance.UseAdditionalCount AndAlso
            AppendSettingDialog.Instance.SearchCountApi <> 0 Then
            count = AppendSettingDialog.Instance.SearchCountApi
        Else
            count = AppendSettingDialog.Instance.CountApi
        End If
        If more Then
            page = tab.GetSearchPage(count)
        Else
            sinceId = tab.SinceId
        End If

        Try
            ' TODO:一時的に40>100件に 件数変更UI作成の必要あり
            res = twCon.Search(tab.SearchWords, tab.SearchLang, count, page, sinceId, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Select Case res
            Case HttpStatusCode.BadRequest
                Return "Invalid query"
            Case HttpStatusCode.NotFound
                Return "Invalid query"
            Case HttpStatusCode.PaymentRequired 'API Documentには420と書いてあるが、該当コードがないので402にしてある
                Return "Search API Limit?"
            Case HttpStatusCode.OK
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select

        If Not TabInformations.GetInstance.ContainsTab(tab) Then Return ""
        content = Regex.Replace(content, "[\x00-\x1f-[\x0a\x0d]]+", " ")
        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Invalid ATOM!"
        End Try
        Dim nsmgr As New XmlNamespaceManager(xdoc.NameTable)
        nsmgr.AddNamespace("search", "http://www.w3.org/2005/Atom")
        nsmgr.AddNamespace("twitter", "http://api.twitter.com/")
        nsmgr.AddNamespace("georss", "http://www.georss.org/georss")
        For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("/search:feed/search:entry", nsmgr)
            Dim xentry As XmlElement = CType(xentryNode, XmlElement)
            Dim post As New PostClass
            Try
                post.StatusId = Long.Parse(xentry.Item("id").InnerText.Split(":"c)(2))
                If TabInformations.GetInstance.ContainsKey(post.StatusId, tab.TabName) Then Continue For
                post.CreatedAt = DateTime.Parse(xentry.Item("published").InnerText)
                '本文
                post.TextFromApi = xentry.Item("title").InnerText
                'Source取得（htmlの場合は、中身を取り出し）
                post.Source = xentry.Item("twitter:source").InnerText
                post.InReplyToStatusId = 0
                post.InReplyToUser = ""
                post.InReplyToUserId = 0
                post.IsFav = False

                ' Geoが勝手に付加されるバグがいっこうに修正されないので暫定的にGeo情報を無視する
                If xentry.Item("twitter:geo").HasChildNodes Then
                    Dim pnt As String() = CType(xentry.SelectSingleNode("twitter:geo/georss:point", nsmgr), XmlElement).InnerText.Split(" "c)
                    post.PostGeo = New PostClass.StatusGeo With {.Lat = Double.Parse(pnt(0)), .Lng = Double.Parse(pnt(1))}
                End If

                '以下、ユーザー情報
                Dim xUentry As XmlElement = CType(xentry.SelectSingleNode("./search:author", nsmgr), XmlElement)
                post.UserId = 0
                post.ScreenName = xUentry.Item("name").InnerText.Split(" "c)(0).Trim
                post.Nickname = xUentry.Item("name").InnerText.Substring(post.ScreenName.Length).Trim
                If post.Nickname.Length > 2 Then
                    post.Nickname = post.Nickname.Substring(1, post.Nickname.Length - 2)
                Else
                    post.Nickname = post.ScreenName
                End If
                post.ImageUrl = CType(xentry.SelectSingleNode("./search:link[@type='image/png']", nsmgr), XmlElement).GetAttribute("href")
                post.IsProtect = False
                post.IsMe = post.ScreenName.ToLower.Equals(_uname)

                'HTMLに整形
                post.Text = CreateHtmlAnchor(HttpUtility.HtmlEncode(post.TextFromApi), post.ReplyToList, post.Media)
                post.TextFromApi = HttpUtility.HtmlDecode(post.TextFromApi)
                'Source整形
                CreateSource(post)

                post.IsRead = read
                post.IsReply = post.ReplyToList.Contains(_uname)
                post.IsExcludeReply = False

                post.IsOwl = False
                If post.IsMe AndAlso Not read AndAlso _readOwnPost Then post.IsRead = True
                post.IsDm = False
                post.RelTabName = tab.TabName
                If Not more AndAlso post.StatusId > tab.SinceId Then tab.SinceId = post.StatusId
            Catch ex As Exception
                TraceOut(ex, GetCurrentMethod.Name & " " & content)
                Continue For
            End Try

            'Me._dIcon.Add(post.ImageUrl, Nothing)
            TabInformations.GetInstance.AddPost(post)

        Next

        '' TODO
        '' 遡るための情報max_idやnext_pageの情報を保持する

#If 0 Then
        Dim xNode As XmlNode = xdoc.DocumentElement.SelectSingleNode("/search:feed/twitter:warning", nsmgr)

        If xNode IsNot Nothing Then
            Return "Warn:" + xNode.InnerText + "(" + GetCurrentMethod.Name + ")"
        End If
#End If

        Return ""
    End Function

    Public Function GetPhoenixSearch(ByVal read As Boolean, _
                            ByVal tab As TabClass, _
                            ByVal more As Boolean) As String

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim page As Integer = 0
        Dim sinceId As Long = 0
        Dim count As Integer = 100
        Dim querystr As String = ""
        If AppendSettingDialog.Instance.UseAdditionalCount AndAlso
            AppendSettingDialog.Instance.SearchCountApi <> 0 Then
            count = AppendSettingDialog.Instance.SearchCountApi
        End If
        If more Then
            page = tab.GetSearchPage(count)
            If Not String.IsNullOrEmpty(tab.NextPageQuery) Then
                querystr = tab.NextPageQuery
            End If
        Else
            sinceId = tab.SinceId
        End If

        Try
            If String.IsNullOrEmpty(querystr) Then
                res = twCon.PhoenixSearch(tab.SearchWords, tab.SearchLang, count, page, sinceId, content)
            Else
                res = twCon.PhoenixSearch(querystr, content)
            End If
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Select Case res
            Case HttpStatusCode.BadRequest
                Return "Invalid query"
            Case HttpStatusCode.NotFound
                Return "Invalid query"
            Case HttpStatusCode.PaymentRequired 'API Documentには420と書いてあるが、該当コードがないので402にしてある
                Return "Search API Limit?"
            Case HttpStatusCode.OK
            Case Else
                Return "Err:" + res.ToString + "(" + GetCurrentMethod.Name + ")"
        End Select

        If Not TabInformations.GetInstance.ContainsTab(tab) Then Return ""

        '' TODO
        '' 遡るための情報max_idやnext_pageの情報を保持する

        Return CreatePostsFromPhoenixSearch(content, WORKERTYPE.PublicSearch, tab, read, count, tab.OldestId, tab.NextPageQuery)
    End Function

    Private Function CreateDirectMessagesFromJson(ByVal content As String, ByVal gType As WORKERTYPE, ByVal read As Boolean) As String
        Dim item As List(Of TwitterDataModel.Directmessage)
        Try
            If gType = WORKERTYPE.UserStream Then
                Dim itm As List(Of TwitterDataModel.DirectmessageEvent) = CreateDataFromJson(Of List(Of TwitterDataModel.DirectmessageEvent))(content)
                item = New List(Of TwitterDataModel.Directmessage)
                For Each dat As TwitterDataModel.DirectmessageEvent In itm
                    item.Add(dat.Directmessage)
                Next
            Else
                item = CreateDataFromJson(Of List(Of TwitterDataModel.Directmessage))(content)
            End If
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Invalid Json!"
        End Try

        For Each message As TwitterDataModel.Directmessage In item
            Dim post As New PostClass
            Try
                post.StatusId = message.Id
                If gType <> WORKERTYPE.UserStream Then
                    If gType = WORKERTYPE.DirectMessegeRcv Then
                        If minDirectmessage > post.StatusId Then minDirectmessage = post.StatusId
                    Else
                        If minDirectmessageSent > post.StatusId Then minDirectmessageSent = post.StatusId
                    End If
                End If

                '二重取得回避
                SyncLock LockObj
                    If TabInformations.GetInstance.GetTabByType(TabUsageType.DirectMessage).Contains(post.StatusId) Then Continue For
                End SyncLock
                'sender_id
                'recipient_id
                post.CreatedAt = DateTimeParse(message.CreatedAt)
                '本文
                post.TextFromApi = message.Text
                'HTMLに整形
                post.Text = CreateHtmlAnchor(post.TextFromApi, post.ReplyToList, post.Media)
                post.TextFromApi = HttpUtility.HtmlDecode(post.TextFromApi)
                post.TextFromApi = post.TextFromApi.Replace("<3", "♡")
                post.IsFav = False

                '以下、ユーザー情報
                Dim user As TwitterDataModel.User
                If gType = WORKERTYPE.UserStream Then
                    If twCon.AuthenticatedUsername.Equals(message.Recipient.ScreenName, StringComparison.CurrentCultureIgnoreCase) Then
                        user = message.Sender
                        post.IsMe = False
                        post.IsOwl = True
                    Else
                        user = message.Recipient
                        post.IsMe = True
                        post.IsOwl = False
                    End If
                Else
                    If gType = WORKERTYPE.DirectMessegeRcv Then
                        user = message.Sender
                        post.IsMe = False
                        post.IsOwl = True
                    Else
                        user = message.Recipient
                        post.IsMe = True
                        post.IsOwl = False
                    End If
                End If

                post.UserId = user.Id
                post.ScreenName = user.ScreenName
                post.Nickname = user.Name.Trim()
                post.ImageUrl = user.ProfileImageUrl
                post.IsProtect = user.Protected
            Catch ex As Exception
                TraceOut(ex, GetCurrentMethod.Name & " " & content)
                MessageBox.Show("Parse Error(CreateDirectMessagesFromJson)")
                Continue For
            End Try

            post.IsRead = read
            If post.IsMe AndAlso Not read AndAlso _readOwnPost Then post.IsRead = True
            post.IsReply = False
            post.IsExcludeReply = False
            post.IsDm = True

            TabInformations.GetInstance.AddPost(post)
        Next

        Return ""

    End Function

    Public Function GetDirectMessageApi(ByVal read As Boolean, _
                            ByVal gType As WORKERTYPE, _
                            ByVal more As Boolean) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""
        If TwitterApiInfo.AccessLevel <> ApiAccessLevel.None Then
            If Not TwitterApiInfo.IsDirectMessagePermission Then Return "Auth Err:try to re-authorization."
        End If

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            If gType = WORKERTYPE.DirectMessegeRcv Then
                If more Then
                    res = twCon.DirectMessages(20, minDirectmessage, 0, content)
                Else
                    res = twCon.DirectMessages(20, 0, 0, content)
                End If
            Else
                If more Then
                    res = twCon.DirectMessagesSent(20, minDirectmessageSent, 0, content)
                Else
                    res = twCon.DirectMessagesSent(20, 0, 0, content)
                End If
            End If
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Return CreateDirectMessagesFromJson(content, gType, read)
    End Function

    Public Function GetFavoritesApi(ByVal read As Boolean,
                        ByVal gType As WORKERTYPE,
                        ByVal more As Boolean) As String

        Static page As Integer = 1

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim count As Integer = AppendSettingDialog.Instance.CountApi
        If AppendSettingDialog.Instance.UseAdditionalCount AndAlso
            AppendSettingDialog.Instance.FavoritesCountApi <> 0 Then
            count = AppendSettingDialog.Instance.FavoritesCountApi
        End If

        ' 前ページ取得の場合はページカウンタをインクリメント、それ以外の場合はページカウンタリセット
        If more Then
            page += 1
        Else
            page = 1
        End If

        Try
            res = twCon.Favorites(count, page, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Dim serializer As New DataContractJsonSerializer(GetType(List(Of TwitterDataModel.Status)))
        Dim item As List(Of TwitterDataModel.Status)

        Try
            item = CreateDataFromJson(Of List(Of TwitterDataModel.Status))(content)
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Invalid Json!"
        End Try

        For Each status As TwitterDataModel.Status In item
            Dim post As New PostClass
            Dim entities As TwitterDataModel.Entities

            Try
                post.StatusId = status.Id
                '二重取得回避
                SyncLock LockObj
                    If TabInformations.GetInstance.GetTabByType(TabUsageType.Favorites).Contains(post.StatusId) Then Continue For
                End SyncLock
                'Retweet判定
                If status.RetweetedStatus IsNot Nothing Then
                    Dim retweeted As TwitterDataModel.RetweetedStatus = status.RetweetedStatus
                    post.CreatedAt = DateTimeParse(retweeted.CreatedAt)

                    'Id
                    post.RetweetedId = post.StatusId
                    '本文
                    post.TextFromApi = retweeted.Text
                    entities = retweeted.Entities
                    'Source取得（htmlの場合は、中身を取り出し）
                    post.Source = retweeted.Source
                    'Reply先
                    Long.TryParse(retweeted.InReplyToStatusId, post.InReplyToStatusId)
                    post.InReplyToUser = retweeted.InReplyToScreenName
                    Long.TryParse(retweeted.InReplyToUserId, post.InReplyToUserId)
                    post.IsFav = True

                    '以下、ユーザー情報
                    Dim user As TwitterDataModel.User = retweeted.User
                    post.UserId = user.Id
                    post.ScreenName = user.ScreenName
                    post.Nickname = user.Name.Trim()
                    post.ImageUrl = user.ProfileImageUrl
                    post.IsProtect = user.Protected

                    'Retweetした人
                    post.RetweetedBy = status.User.ScreenName
                    post.IsMe = post.RetweetedBy.ToLower.Equals(_uname)
                Else
                    post.CreatedAt = DateTimeParse(status.CreatedAt)

                    '本文
                    post.TextFromApi = status.Text
                    entities = status.Entities
                    'Source取得（htmlの場合は、中身を取り出し）
                    post.Source = status.Source
                    Long.TryParse(status.InReplyToStatusId, post.InReplyToStatusId)
                    post.InReplyToUser = status.InReplyToScreenName
                    Long.TryParse(status.InReplyToUserId, post.InReplyToUserId)

                    post.IsFav = True

                    '以下、ユーザー情報
                    Dim user As TwitterDataModel.User = status.User
                    post.UserId = user.Id
                    post.ScreenName = user.ScreenName
                    post.Nickname = user.Name.Trim()
                    post.ImageUrl = user.ProfileImageUrl
                    post.IsProtect = user.Protected
                    post.IsMe = post.ScreenName.ToLower.Equals(_uname)
                End If
                'HTMLに整形
                post.Text = CreateHtmlAnchor(post.TextFromApi, post.ReplyToList, entities, post.Media)
                post.TextFromApi = Me.ReplaceTextFromApi(post.TextFromApi, entities)
                post.TextFromApi = HttpUtility.HtmlDecode(post.TextFromApi)
                post.TextFromApi = post.TextFromApi.Replace("<3", "♡")
                'Source整形
                CreateSource(post)

                post.IsRead = read
                post.IsReply = post.ReplyToList.Contains(_uname)
                post.IsExcludeReply = False

                If post.IsMe Then
                    post.IsOwl = False
                Else
                    If followerId.Count > 0 Then post.IsOwl = Not followerId.Contains(post.UserId)
                End If

                post.IsDm = False
            Catch ex As Exception
                TraceOut(ex, GetCurrentMethod.Name & " " & content)
                Continue For
            End Try

            TabInformations.GetInstance.AddPost(post)

        Next

        Return ""
    End Function

    Private Function ReplaceTextFromApi(ByVal text As String, ByVal entities As TwitterDataModel.Entities) As String
        If entities IsNot Nothing Then
            If entities.Urls IsNot Nothing Then
                For Each m In entities.Urls
                    If Not String.IsNullOrEmpty(m.DisplayUrl) Then text = text.Replace(m.Url, m.DisplayUrl)
                Next
            End If
            If entities.Media IsNot Nothing Then
                For Each m In entities.Media
                    If Not String.IsNullOrEmpty(m.DisplayUrl) Then text = text.Replace(m.Url, m.DisplayUrl)
                Next
            End If
        End If
        Return text
    End Function

    Public Function GetFollowersApi() As String
        If _endingFlag Then Return ""
        Dim cursor As Long = -1
        Dim tmpFollower As New List(Of Long)(followerId)

        followerId.Clear()
        Do
            Dim ret As String = FollowerApi(cursor)
            If Not String.IsNullOrEmpty(ret) Then
                followerId.Clear()
                followerId.AddRange(tmpFollower)
                _GetFollowerResult = False
                Return ret
            End If
        Loop While cursor > 0

        TabInformations.GetInstance.RefreshOwl(followerId)

        _GetFollowerResult = True
        Return ""
    End Function

    Public ReadOnly Property GetFollowersSuccess() As Boolean
        Get
            Return _GetFollowerResult
        End Get
    End Property

    Private Function FollowerApi(ByRef cursor As Long) As String
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.FollowerIds(cursor, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Try
            Dim followers = CreateDataFromJson(Of TwitterDataModel.Ids)(content)
            followerId.AddRange(followers.Id)
            cursor = followers.NextCursor
            Return ""
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Err:Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Err:Invalid Json!"
        End Try
    End Function

    Public Function GetNoRetweetIdsApi() As String
        If _endingFlag Then Return ""
        Dim cursor As Long = -1
        Dim tmpIds As New List(Of Long)(noRTId)

        noRTId.Clear()
        Do
            Dim ret As String = NoRetweetApi(cursor)
            If Not String.IsNullOrEmpty(ret) Then
                noRTId.Clear()
                noRTId.AddRange(tmpIds)
                _GetNoRetweetResult = False
                Return ret
            End If
        Loop While cursor > 0

        _GetNoRetweetResult = True
        Return ""
    End Function

    Private Function NoRetweetApi(ByRef cursor As Long) As String
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.NoRetweetIds(cursor, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Try
            Dim ids = CreateDataFromJson(Of Long())(content)
            noRTId.AddRange(ids)
            cursor = 0  '0より小さければ何でも良い。
            Return ""
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Err:Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Err:Invalid Json!"
        End Try
    End Function

    Public ReadOnly Property GetNoRetweetSuccess() As Boolean
        Get
            Return _GetNoRetweetResult
        End Get
    End Property

    Public Function ConfigurationApi() As String
        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.GetConfiguration(content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Try
            AppendSettingDialog.Instance.TwitterConfiguration = CreateDataFromJson(Of TwitterDataModel.Configuration)(content)
            Return ""
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Err:Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Err:Invalid Json!"
        End Try
    End Function

    Public Function GetListsApi() As String
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim cursor As Long = -1

        Dim lists As New List(Of ListElement)
        Do
            Try
                res = twCon.GetLists(Me.Username, cursor, content)
            Catch ex As Exception
                Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
            End Try

            Select Case res
                Case HttpStatusCode.OK
                    Twitter.AccountState = ACCOUNT_STATE.Valid
                Case HttpStatusCode.Unauthorized
                    Twitter.AccountState = ACCOUNT_STATE.Invalid
                    Return My.Resources.Unauthorized
                Case HttpStatusCode.BadRequest
                    Return "Err:API Limits?"
                Case Else
                    Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
            End Select

            Try
                Dim lst = CreateDataFromJson(Of TwitterDataModel.Lists)(content)
                lists.AddRange(From le In lst.Lists Select New ListElement(le, Me))
                cursor = lst.NextCursor
            Catch ex As SerializationException
                TraceOut(ex.Message + Environment.NewLine + content)
                Return "Err:Json Parse Error(DataContractJsonSerializer)"
            Catch ex As Exception
                TraceOut(ex, GetCurrentMethod.Name & " " & content)
                Return "Err:Invalid Json!"
            End Try
        Loop While cursor <> 0

        cursor = -1
        content = ""
        Do
            Try
                res = twCon.GetListsSubscriptions(Me.Username, cursor, content)
            Catch ex As Exception
                Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
            End Try

            Select Case res
                Case HttpStatusCode.OK
                    Twitter.AccountState = ACCOUNT_STATE.Valid
                Case HttpStatusCode.Unauthorized
                    Twitter.AccountState = ACCOUNT_STATE.Invalid
                    Return My.Resources.Unauthorized
                Case HttpStatusCode.BadRequest
                    Return "Err:API Limits?"
                Case Else
                    Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
            End Select

            Try
                Dim lst = CreateDataFromJson(Of TwitterDataModel.Lists)(content)
                lists.AddRange(From le In lst.Lists Select New ListElement(le, Me))
                cursor = lst.NextCursor
            Catch ex As SerializationException
                TraceOut(ex.Message + Environment.NewLine + content)
                Return "Err:Json Parse Error(DataContractJsonSerializer)"
            Catch ex As Exception
                TraceOut(ex, GetCurrentMethod.Name & " " & content)
                Return "Err:Invalid Json!"
            End Try
        Loop While cursor <> 0

        TabInformations.GetInstance.SubscribableLists = lists
        Return ""
    End Function

    Public Function DeleteList(ByVal list_id As String) As String
        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.DeleteListID(Me.Username, list_id, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "destroy_list", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Return ""
    End Function

    Public Function EditList(ByVal list_id As String, ByVal new_name As String, ByVal isPrivate As Boolean, ByVal description As String, ByRef list As ListElement) As String
        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.UpdateListID(Me.Username, list_id, new_name, isPrivate, description, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("get", "update_list", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Try
            Dim le = CreateDataFromJson(Of TwitterDataModel.ListElementData)(content)
            Dim newList As New ListElement(le, Me)
            list.Description = newList.Description
            list.Id = newList.Id
            list.IsPublic = newList.IsPublic
            list.MemberCount = newList.MemberCount
            list.Name = newList.Name
            list.SubscriberCount = newList.SubscriberCount
            list.Slug = newList.Slug
            list.Nickname = newList.Nickname
            list.Username = newList.Username
            list.UserId = newList.UserId
            Return ""
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Err:Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Err:Invalid Json!"
        End Try

    End Function

    Public Function GetListMembers(ByVal list_id As String, ByVal lists As List(Of UserInfo), ByRef cursor As Long) As String
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        'Do
        Try
            res = twCon.GetListMembers(Me.Username, list_id, cursor, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Try
            Dim users = CreateDataFromJson(Of TwitterDataModel.Users)(content)
            Array.ForEach(Of TwitterDataModel.User)(
                users.users,
                New Action(Of TwitterDataModel.User)(Sub(u)
                                                         lists.Add(New UserInfo(u))
                                                     End Sub))
            cursor = users.NextCursor
            Return ""
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Err:Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Err:Invalid Json!"
        End Try

        Return ""
    End Function

    Public Function CreateListApi(ByVal listName As String, ByVal isPrivate As Boolean, ByVal description As String) As String
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.CreateLists(listName, isPrivate, description, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "create_list", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Try
            Dim le = CreateDataFromJson(Of TwitterDataModel.ListElementData)(content)
            TabInformations.GetInstance().SubscribableLists.Add(New ListElement(le, Me))
            Return ""
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Err:Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Err:Invalid Json!"
        End Try
    End Function

    Public Function ContainsUserAtList(ByVal listId As String, ByVal user As String, ByRef value As Boolean) As String
        value = False

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = Me.twCon.ShowListMember(listId, user, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case HttpStatusCode.NotFound
                value = False
                Return ""
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Try
            Dim u = CreateDataFromJson(Of TwitterDataModel.User)(content)
            value = True
            Return ""
        Catch ex As Exception
            value = False
            Return ""
        End Try
    End Function

    Public Function AddUserToList(ByVal listId As String, ByVal user As String) As String
        Dim content As String = ""
        Dim res As HttpStatusCode

        Try
            res = twCon.CreateListMembers(listId, user, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "add_user_to_list", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:" + GetErrorMessageJson(content)
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Return ""
    End Function

    Public Function RemoveUserToList(ByVal listId As String, ByVal user As String) As String

        Dim content As String = ""
        Dim res As HttpStatusCode

        Try
            res = twCon.DeleteListMembers(listId, user, content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Google.GASender.GetInstance().TrackEventWithCategory("post", "remove_user_from_list", Me.UserId)
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:" + GetErrorMessageJson(content)
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Return ""
    End Function

    Private Class range
        Public Property fromIndex As Integer
        Public Property toIndex As Integer
        Public Sub New(ByVal fromIndex As Integer, ByVal toIndex As Integer)
            Me.fromIndex = fromIndex
            Me.toIndex = toIndex
        End Sub
    End Class
    Public Function CreateHtmlAnchor(ByVal Text As String, ByVal AtList As List(Of String), ByVal media As Dictionary(Of String, String)) As String
        If Text Is Nothing Then Return Nothing
        Dim retStr As String = Text.Replace("&gt;", "<<<<<tweenだいなり>>>>>").Replace("&lt;", "<<<<<tweenしょうなり>>>>>")
        'uriの正規表現
        'Const url_valid_domain As String = "(?<domain>(?:[^\p{P}\s][\.\-_](?=[^\p{P}\s])|[^\p{P}\s]){1,}\.[a-z]{2,}(?::[0-9]+)?)"
        'Const url_valid_general_path_chars As String = "[a-z0-9!*';:=+$/%#\[\]\-_,~]"
        'Const url_balance_parens As String = "(?:\(" + url_valid_general_path_chars + "+\))"
        'Const url_valid_url_path_ending_chars As String = "(?:[a-z0-9=_#/\-\+]+|" + url_balance_parens + ")"
        'Const pth As String = "(?:" + url_balance_parens +
        '    "|@" + url_valid_general_path_chars + "+/" +
        '    "|[.,]?" + url_valid_general_path_chars + "+" +
        '    ")"
        'Const pth2 As String = "(/(?:" +
        '    pth + "+" + url_valid_url_path_ending_chars + "|" +
        '    pth + "+" + url_valid_url_path_ending_chars + "?|" +
        '    url_valid_url_path_ending_chars +
        '    ")?)?"
        'Const qry As String = "(?<query>\?[a-z0-9!*'();:&=+$/%#\[\]\-_.,~]*[a-z0-9_&=#])?"
        'Const rgUrl As String = "(?<before>(?:[^\""':!=#]|^|\:/))" +
        '                            "(?<url>(?<protocol>https?://)" +
        '                            url_valid_domain +
        '                            pth2 +
        '                            qry +
        '                            ")"
        'Const rgUrl As String = "(?<before>(?:[^\""':!=#]|^|\:/))" +
        '                            "(?<url>(?<protocol>https?://|www\.)" +
        '                            url_valid_domain +
        '                            pth2 +
        '                            qry +
        '                            ")"
        '絶対パス表現のUriをリンクに置換
        retStr = Regex.Replace(retStr,
                               rgUrl,
                               New MatchEvaluator(Function(mu As Match)
                                                      Dim sb As New StringBuilder(mu.Result("${before}<a href="""))
                                                      'If mu.Result("${protocol}").StartsWith("w", StringComparison.OrdinalIgnoreCase) Then
                                                      '    sb.Append("http://")
                                                      'End If
                                                      Dim url As String = mu.Result("${url}")
                                                      Dim title As String = ShortUrl.ResolveMedia(url, True)
                                                      If url <> title Then
                                                          title = ShortUrl.ResolveMedia(title, False)
                                                      End If
                                                      sb.Append(url + """ title=""" + title + """>").Append(url).Append("</a>")
                                                      If media IsNot Nothing AndAlso Not media.ContainsKey(url) Then media.Add(url, title)
                                                      Return sb.ToString
                                                  End Function),
                               RegexOptions.IgnoreCase)

        '@先をリンクに置換（リスト）
        retStr = Regex.Replace(retStr,
                               "(^|[^a-zA-Z0-9_/])([@＠]+)([a-zA-Z0-9_]{1,20}/[a-zA-Z][a-zA-Z0-9\p{IsLatin-1Supplement}\-]{0,79})",
                               "$1$2<a href=""/$3"">$3</a>")

        Dim m As Match = Regex.Match(retStr, "(^|[^a-zA-Z0-9_])[@＠]([a-zA-Z0-9_]{1,20})")
        While m.Success
            If Not AtList.Contains(m.Result("$2").ToLower) Then AtList.Add(m.Result("$2").ToLower)
            m = m.NextMatch
        End While
        '@先をリンクに置換
        retStr = Regex.Replace(retStr,
                               "(^|[^a-zA-Z0-9_/])([@＠])([a-zA-Z0-9_]{1,20})",
                               "$1$2<a href=""/$3"">$3</a>")

        'ハッシュタグを抽出し、リンクに置換
        Dim anchorRange As New List(Of range)
        For i As Integer = 0 To retStr.Length - 1
            Dim index As Integer = retStr.IndexOf("<a ", i)
            If index > -1 AndAlso index < retStr.Length Then
                i = index
                Dim toIndex As Integer = retStr.IndexOf("</a>", index)
                If toIndex > -1 Then
                    anchorRange.Add(New range(index, toIndex + 3))
                    i = toIndex
                End If
            End If
        Next
        'retStr = Regex.Replace(retStr,
        '                       "(^|[^a-zA-Z0-9/&])([#＃])([0-9a-zA-Z_]*[a-zA-Z_]+[a-zA-Z0-9_\xc0-\xd6\xd8-\xf6\xf8-\xff]*)",
        '                       New MatchEvaluator(Function(mh As Match)
        '                                              For Each rng As range In anchorRange
        '                                                  If mh.Index >= rng.fromIndex AndAlso
        '                                                   mh.Index <= rng.toIndex Then Return mh.Result("$0")
        '                                              Next
        '                                              If IsNumeric(mh.Result("$3")) Then Return mh.Result("$0")
        '                                              SyncLock LockObj
        '                                                  _hashList.Add("#" + mh.Result("$3"))
        '                                              End SyncLock
        '                                              Return mh.Result("$1") + "<a href=""" & _protocol & "twitter.com/search?q=%23" + mh.Result("$3") + """>" + mh.Result("$2$3") + "</a>"
        '                                          End Function),
        '                                      RegexOptions.IgnoreCase)
        retStr = Regex.Replace(retStr,
                               HASHTAG,
                               New MatchEvaluator(Function(mh As Match)
                                                      For Each rng As range In anchorRange
                                                          If mh.Index >= rng.fromIndex AndAlso
                                                           mh.Index <= rng.toIndex Then Return mh.Result("$0")
                                                      Next
                                                      SyncLock LockObj
                                                          _hashList.Add("#" + mh.Result("$3"))
                                                      End SyncLock
                                                      Return mh.Result("$1") + "<a href=""" & _protocol & "twitter.com/search?q=%23" + mh.Result("$3") + """>" + mh.Result("$2$3") + "</a>"
                                                  End Function),
                                              RegexOptions.IgnoreCase)


        retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9_/&#＃@＠>=.~])(sm|nm)([0-9]{1,10})", "$1<a href=""http://www.nicovideo.jp/watch/$2$3"">$2$3</a>")

        retStr = retStr.Replace("<<<<<tweenだいなり>>>>>", "&gt;").Replace("<<<<<tweenしょうなり>>>>>", "&lt;")

        'retStr = AdjustHtml(ShortUrl.Resolve(PreProcessUrl(retStr), True)) 'IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与
        retStr = AdjustHtml(PreProcessUrl(retStr)) 'IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与
        Return retStr
    End Function

    Private Class EntityInfo
        Public Property StartIndex As Integer
        Public Property EndIndex As Integer
        Public Property Text As String
        Public Property Html As String
        Public Property Display As String
    End Class
    Public Function CreateHtmlAnchor(ByRef Text As String, ByVal AtList As List(Of String), ByVal entities As TwitterDataModel.Entities, ByVal media As Dictionary(Of String, String)) As String
        Dim ret As String = Text

        If entities IsNot Nothing Then
            Dim etInfo As New SortedList(Of Integer, EntityInfo)
            'URL
            If entities.Urls IsNot Nothing Then
                For Each ent In entities.Urls
                    If String.IsNullOrEmpty(ent.DisplayUrl) Then
                        etInfo.Add(ent.Indices(0),
                                   New EntityInfo With {.StartIndex = ent.Indices(0),
                                                        .EndIndex = ent.Indices(1),
                                                        .Text = ent.Url,
                                                        .Html = "<a href=""" + ent.Url + """>" + ent.Url + "</a>"})
                    Else
                        Dim expanded As String = ShortUrl.ResolveMedia(ent.ExpandedUrl, False)
                        etInfo.Add(ent.Indices(0),
                                   New EntityInfo With {.StartIndex = ent.Indices(0),
                                                        .EndIndex = ent.Indices(1),
                                                        .Text = ent.Url,
                                                        .Html = "<a href=""" + ent.Url + """ title=""" + expanded + """>" + ent.DisplayUrl + "</a>",
                                                        .Display = ent.DisplayUrl})
                        If media IsNot Nothing AndAlso Not media.ContainsKey(ent.Url) Then media.Add(ent.Url, expanded)
                    End If
                Next
            End If
            If entities.Hashtags IsNot Nothing Then
                For Each ent In entities.Hashtags
                    Dim hash As String = Text.Substring(ent.Indices(0), ent.Indices(1) - ent.Indices(0))
                    etInfo.Add(ent.Indices(0),
                               New EntityInfo With {.StartIndex = ent.Indices(0),
                                                    .EndIndex = ent.Indices(1),
                                                    .Text = hash,
                                                    .Html = "<a href=""" & _protocol & "twitter.com/search?q=%23" + ent.Text + """>" + hash + "</a>"})
                    SyncLock LockObj
                        _hashList.Add("#" + ent.Text)
                    End SyncLock
                Next
            End If
            If entities.UserMentions IsNot Nothing Then
                For Each ent In entities.UserMentions
                    Dim screenName As String = Text.Substring(ent.Indices(0) + 1, ent.Indices(1) - ent.Indices(0) - 1)
                    etInfo.Add(ent.Indices(0) + 1,
                               New EntityInfo With {.StartIndex = ent.Indices(0) + 1,
                                                    .EndIndex = ent.Indices(1),
                                                    .Text = ent.ScreenName,
                                                    .Html = "<a href=""/" + ent.ScreenName + """>" + screenName + "</a>"})
                    If Not AtList.Contains(ent.ScreenName.ToLower) Then AtList.Add(ent.ScreenName.ToLower)
                Next
            End If
            If entities.Media IsNot Nothing Then
                For Each ent In entities.Media
                    If ent.Type = "photo" Then
                        etInfo.Add(ent.Indices(0),
                                   New EntityInfo With {.StartIndex = ent.Indices(0),
                                                        .EndIndex = ent.Indices(1),
                                                        .Text = ent.Url,
                                                        .Html = "<a href=""" + ent.Url + """ title=""" + ent.ExpandedUrl + """>" + ent.DisplayUrl + "</a>",
                                                        .Display = ent.DisplayUrl})
                        If media IsNot Nothing AndAlso Not media.ContainsKey(ent.Url) Then media.Add(ent.Url, ent.MediaUrl)
                    End If
                Next
            End If
            If etInfo.Count > 0 Then
                Try
                    Dim idx As Integer = 0
                    ret = ""
                    For Each et In etInfo
                        ret += Text.Substring(idx, et.Key - idx) + et.Value.Html
                        idx = et.Value.EndIndex
                    Next
                    ret += Text.Substring(idx)
                Catch ex As ArgumentOutOfRangeException
                    'Twitterのバグで不正なエンティティ（Index指定範囲が重なっている）が返ってくる場合の対応
                    ret = Text
                    entities = Nothing
                    If media IsNot Nothing Then media.Clear()
                End Try
            End If
        End If

        ret = Regex.Replace(ret, "(^|[^a-zA-Z0-9_/&#＃@＠>=.~])(sm|nm)([0-9]{1,10})", "$1<a href=""http://www.nicovideo.jp/watch/$2$3"">$2$3</a>")
        ret = AdjustHtml(ShortUrl.Resolve(PreProcessUrl(ret), False)) 'IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与

        Return ret
    End Function

    'Source整形
    Private Sub CreateSource(ByRef post As PostClass)
        If post.Source.StartsWith("<") Then
            If Not post.Source.Contains("</a>") Then
                post.Source += "</a>"
            End If
            Dim mS As Match = Regex.Match(post.Source, ">(?<source>.+)<")
            If mS.Success Then
                post.SourceHtml = String.Copy(ShortUrl.Resolve(PreProcessUrl(post.Source), False))
                post.Source = HttpUtility.HtmlDecode(mS.Result("${source}"))
            Else
                post.Source = ""
                post.SourceHtml = ""
            End If
        Else
            If post.Source = "web" Then
                post.SourceHtml = My.Resources.WebSourceString
            ElseIf post.Source = "Keitai Mail" Then
                post.SourceHtml = My.Resources.KeitaiMailSourceString
            Else
                post.SourceHtml = String.Copy(post.Source)
            End If
        End If
    End Sub

    Public Function GetInfoApi(ByVal info As ApiInfo) As Boolean
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return True

        If _endingFlag Then Return True

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.RateLimitStatus(content)
        Catch ex As Exception
            TwitterApiInfo.Initialize()
            Return False
        End Try

        If res <> HttpStatusCode.OK Then Return False

        Try
            Dim limit = CreateDataFromJson(Of TwitterDataModel.RateLimitStatus)(content)
            Dim arg As New ApiInformationChangedEventArgs
            arg.ApiInfo.MaxCount = limit.HourlyLimit
            arg.ApiInfo.RemainCount = limit.RemainingHits
            arg.ApiInfo.ResetTime = DateTimeParse(limit.RestTime)
            arg.ApiInfo.ResetTimeInSeconds = limit.RestTimeInSeconds
            If info IsNot Nothing Then
                arg.ApiInfo.UsingCount = info.UsingCount

                info.MaxCount = arg.ApiInfo.MaxCount
                info.RemainCount = arg.ApiInfo.RemainCount
                info.ResetTime = arg.ApiInfo.ResetTime
                info.ResetTimeInSeconds = arg.ApiInfo.ResetTimeInSeconds
            End If

            RaiseEvent ApiInformationChanged(Me, arg)
            TwitterApiInfo.WriteBackEventArgs(arg)
            Return True
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            TwitterApiInfo.Initialize()
            Return False
        End Try
    End Function
    Public Function GetBlockUserIds() As String
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.GetBlockUserIds(content)
        Catch ex As Exception
            Return "Err:" + ex.Message + "(" + GetCurrentMethod.Name + ")"
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return My.Resources.Unauthorized
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString() + "(" + GetCurrentMethod.Name + ")"
        End Select

        Try
            Dim Ids = CreateDataFromJson(Of List(Of Long))(content)
            If Ids.Contains(Me.UserId) Then Ids.Remove(Me.UserId)
            TabInformations.GetInstance.BlockIds.AddRange(Ids)
            Return ("")
        Catch ex As SerializationException
            TraceOut(ex.Message + Environment.NewLine + content)
            Return "Err:Json Parse Error(DataContractJsonSerializer)"
        Catch ex As Exception
            TraceOut(ex, GetCurrentMethod.Name & " " & content)
            Return "Err:Invalid Json!"
        End Try

    End Function

    Public Function GetHashList() As String()
        Dim hashArray As String()
        SyncLock LockObj
            hashArray = _hashList.ToArray
            _hashList.Clear()
        End SyncLock
        Return hashArray
    End Function

    Public ReadOnly Property AccessToken() As String
        Get
            Return twCon.AccessToken
        End Get
    End Property

    Public ReadOnly Property AccessTokenSecret() As String
        Get
            Return twCon.AccessTokenSecret
        End Get
    End Property

    Public Event ApiInformationChanged(ByVal sender As Object, ByVal e As ApiInformationChangedEventArgs)

    Private Sub Twitter_ApiInformationChanged(ByVal sender As Object, ByVal e As ApiInformationChangedEventArgs) Handles Me.ApiInformationChanged
    End Sub

#Region "UserStream"
    Public Property TrackWord As String = ""
    Public Property AllAtReply As Boolean = False

    Public Event NewPostFromStream()
    Public Event UserStreamStarted()
    Public Event UserStreamStopped()
    Public Event UserStreamGetFriendsList()
    Public Event PostDeleted(ByVal id As Long)
    Public Event UserStreamEventReceived(ByVal eventType As FormattedEvent)
    Private _lastUserstreamDataReceived As DateTime
    Private WithEvents userStream As TwitterUserstream

    Public Class FormattedEvent
        Public Property Eventtype As EVENTTYPE
        Public Property CreatedAt As DateTime
        Public Property [Event] As String
        Public Property Username As String
        Public Property Target As String
        Public Property Id As Int64
        Public Property IsMe As Boolean
    End Class

    Public Property StoredEvent As New List(Of FormattedEvent)

    Private Class EventTypeTableElement
        Public Name As String
        Public Type As EVENTTYPE

        Public Sub New(ByVal name As String, ByVal type As EVENTTYPE)
            Me.Name = name
            Me.Type = type
        End Sub
    End Class

    Private EventTable As EventTypeTableElement() = {
        New EventTypeTableElement("favorite", EVENTTYPE.Favorite), _
        New EventTypeTableElement("unfavorite", EVENTTYPE.Unfavorite), _
        New EventTypeTableElement("follow", EVENTTYPE.Follow), _
        New EventTypeTableElement("list_member_added", EVENTTYPE.ListMemberAdded), _
        New EventTypeTableElement("list_member_removed", EVENTTYPE.ListMemberRemoved), _
        New EventTypeTableElement("block", EVENTTYPE.Block), _
        New EventTypeTableElement("unblock", EVENTTYPE.Unblock), _
        New EventTypeTableElement("user_update", EVENTTYPE.UserUpdate), _
        New EventTypeTableElement("deleted", EVENTTYPE.Deleted), _
        New EventTypeTableElement("list_created", EVENTTYPE.ListCreated), _
        New EventTypeTableElement("list_updated", EVENTTYPE.ListUpdated)
    }

    Public Function EventNameToEventType(ByVal EventName As String) As EVENTTYPE
        Return (From tbl In EventTable Where tbl.Name.Equals(EventName) Select tbl.Type).FirstOrDefault()
    End Function

    Public ReadOnly Property IsUserstreamDataReceived As Boolean
        Get
            Return Now.Subtract(Me._lastUserstreamDataReceived).TotalSeconds < 31
        End Get
    End Property

    Private Sub userStream_StatusArrived(ByVal line As String) Handles userStream.StatusArrived
        Me._lastUserstreamDataReceived = Now
        If String.IsNullOrEmpty(line) Then Exit Sub

        Dim isDm As Boolean = False

        Try
            Using jsonReader As XmlDictionaryReader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(line), XmlDictionaryReaderQuotas.Max)
                Dim xElm As XElement = XElement.Load(jsonReader)
                If xElm.Element("friends") IsNot Nothing Then
                    Debug.Print("friends")
                    Exit Sub
                ElseIf xElm.Element("delete") IsNot Nothing Then
                    Debug.Print("delete")
                    Dim id As Int64
                    If xElm.Element("delete").Element("direct_message") IsNot Nothing AndAlso
                        xElm.Element("delete").Element("direct_message").Element("id") IsNot Nothing Then
                        id = CLng(xElm.Element("delete").Element("direct_message").Element("id").Value)
                        RaiseEvent PostDeleted(id)
                    ElseIf xElm.Element("delete").Element("status") IsNot Nothing AndAlso
                        xElm.Element("delete").Element("status").Element("id") IsNot Nothing Then
                        id = CLng(xElm.Element("delete").Element("status").Element("id").Value)
                        RaiseEvent PostDeleted(id)
                    Else
                        TraceOut("delete:" + line)
                        Exit Sub
                    End If
                    For i As Integer = Me.StoredEvent.Count - 1 To 0 Step -1
                        Dim sEvt As FormattedEvent = Me.StoredEvent(i)
                        If sEvt.Id = id AndAlso (sEvt.Event = "favorite" OrElse sEvt.Event = "unfavorite") Then
                            Me.StoredEvent.RemoveAt(i)
                        End If
                    Next
                    Exit Sub
                ElseIf xElm.Element("limit") IsNot Nothing Then
                    Debug.Print(line)
                    Exit Sub
                ElseIf xElm.Element("event") IsNot Nothing Then
                    Debug.Print("event: " + xElm.Element("event").Value)
                    CreateEventFromJson(line)
                    Exit Sub
                ElseIf xElm.Element("direct_message") IsNot Nothing Then
                    Debug.Print("direct_message")
                    isDm = True
                ElseIf xElm.Element("scrub_geo") IsNot Nothing Then
                    Try
                        TabInformations.GetInstance.ScrubGeoReserve(Long.Parse(xElm.Element("scrub_geo").Element("user_id").Value),
                                                                    Long.Parse(xElm.Element("scrub_geo").Element("up_to_status_id").Value))
                    Catch ex As Exception
                        TraceOut("scrub_geo:" + line)
                    End Try
                    Exit Sub
                End If
            End Using

            Dim res As New StringBuilder
            res.Length = 0
            res.Append("[")
            res.Append(line)
            res.Append("]")

            If isDm Then
                CreateDirectMessagesFromJson(res.ToString, WORKERTYPE.UserStream, False)
            Else
                CreatePostsFromJson(res.ToString, WORKERTYPE.Timeline, Nothing, False, Nothing, Nothing)
            End If
        Catch ex As NullReferenceException
            TraceOut("NullRef StatusArrived: " + line)
        End Try

        RaiseEvent NewPostFromStream()
    End Sub

    Private Sub CreateEventFromJson(ByVal content As String)
        Dim eventData As TwitterDataModel.EventData = Nothing
        Try
            eventData = CreateDataFromJson(Of TwitterDataModel.EventData)(content)
        Catch ex As SerializationException
            TraceOut(ex, "Event Serialize Exception!" + Environment.NewLine + content)
        Catch ex As Exception
            TraceOut(ex, "Event Exception!" + Environment.NewLine + content)
        End Try

        Dim evt As New FormattedEvent
        evt.CreatedAt = DateTimeParse(eventData.CreatedAt)
        evt.Event = eventData.Event
        evt.Username = eventData.Source.ScreenName
        evt.IsMe = evt.Username.ToLower().Equals(Me.Username.ToLower())
        evt.Eventtype = EventNameToEventType(evt.Event)
        Select Case eventData.Event
            Case "access_revoked"
                Exit Sub
            Case "follow"
                If eventData.Target.ScreenName.ToLower.Equals(_uname) Then
                    If Not Me.followerId.Contains(eventData.Source.Id) Then Me.followerId.Add(eventData.Source.Id)
                Else
                    Exit Sub    'Block後のUndoをすると、SourceとTargetが逆転したfollowイベントが帰ってくるため。
                End If
                evt.Target = ""
            Case "favorite", "unfavorite"
                evt.Target = "@" + eventData.TargetObject.User.ScreenName + ":" + HttpUtility.HtmlDecode(eventData.TargetObject.Text)
                evt.Id = eventData.TargetObject.Id
                If AppendSettingDialog.Instance.IsRemoveSameEvent Then
                    If StoredEvent.Any(Function(ev As FormattedEvent)
                                           Return ev.Username = evt.Username AndAlso ev.Eventtype = evt.Eventtype AndAlso ev.Target = evt.Target
                                       End Function) Then Exit Sub
                End If
                If TabInformations.GetInstance.ContainsKey(eventData.TargetObject.Id) Then
                    Dim post As PostClass = TabInformations.GetInstance.Item(eventData.TargetObject.Id)
                    If eventData.Event = "favorite" Then
                        If evt.Username.ToLower.Equals(_uname) Then
                            post.IsFav = True
                            TabInformations.GetInstance.GetTabByType(TabUsageType.Favorites).Add(post.StatusId, post.IsRead, False)
                        Else
                            post.FavoritedCount += 1
                            If Not TabInformations.GetInstance.GetTabByType(TabUsageType.Favorites).Contains(post.StatusId) Then
                                If AppendSettingDialog.Instance.FavEventUnread AndAlso post.IsRead Then
                                    post.IsRead = False
                                End If
                                TabInformations.GetInstance.GetTabByType(TabUsageType.Favorites).Add(post.StatusId, post.IsRead, False)
                            Else
                                If AppendSettingDialog.Instance.FavEventUnread Then
                                    TabInformations.GetInstance.SetRead(False, TabInformations.GetInstance.GetTabByType(TabUsageType.Favorites).TabName, TabInformations.GetInstance.GetTabByType(TabUsageType.Favorites).IndexOf(post.StatusId))
                                End If
                            End If
                        End If
                    Else
                        If evt.Username.ToLower.Equals(_uname) Then
                            post.IsFav = False
                        Else
                            post.FavoritedCount -= 1
                            If post.FavoritedCount < 0 Then post.FavoritedCount = 0
                        End If
                    End If
                End If
            Case "list_member_added", "list_member_removed", "list_updated"
                evt.Target = eventData.TargetObject.FullName
            Case "block"
                If Not TabInformations.GetInstance.BlockIds.Contains(eventData.Target.Id) Then TabInformations.GetInstance.BlockIds.Add(eventData.Target.Id)
                evt.Target = ""
            Case "unblock"
                If TabInformations.GetInstance.BlockIds.Contains(eventData.Target.Id) Then TabInformations.GetInstance.BlockIds.Remove(eventData.Target.Id)
                evt.Target = ""
            Case "user_update"
                evt.Target = ""
            Case "list_created"
                evt.Target = ""
            Case Else
                TraceOut("Unknown Event:" + evt.Event + Environment.NewLine + content)
        End Select
        Me.StoredEvent.Insert(0, evt)
        RaiseEvent UserStreamEventReceived(evt)
    End Sub

    Private Sub userStream_Started() Handles userStream.Started
        Google.GASender.GetInstance().TrackPage("/userstream", Me.UserId)
        RaiseEvent UserStreamStarted()
    End Sub

    Private Sub userStream_Stopped() Handles userStream.Stopped
        RaiseEvent UserStreamStopped()
    End Sub

    Public ReadOnly Property UserStreamEnabled As Boolean
        Get
            Return If(userStream Is Nothing, False, userStream.Enabled)
        End Get
    End Property

    Public Sub StartUserStream()
        If userStream IsNot Nothing Then
            StopUserStream()
        End If
        userStream = New TwitterUserstream(twCon)
        userStream.Start(Me.AllAtReply, Me.TrackWord)
    End Sub

    Public Sub StopUserStream()
        If userStream IsNot Nothing Then userStream.Dispose()
        userStream = Nothing
        If Not _endingFlag Then RaiseEvent UserStreamStopped()
    End Sub

    Public Sub ReconnectUserStream()
        If userStream IsNot Nothing Then
            Me.StartUserStream()
        End If
    End Sub

    Private Class TwitterUserstream
        Implements IDisposable

        Public Event StatusArrived(ByVal status As String)
        Public Event Stopped()
        Public Event Started()
        Private twCon As HttpTwitter

        Private _streamThread As Thread
        Private _streamActive As Boolean

        Private _allAtreplies As Boolean = False
        Private _trackwords As String = ""

        Public Sub New(ByVal twitterConnection As HttpTwitter)
            twCon = DirectCast(twitterConnection.Clone(), HttpTwitter)
        End Sub

        Public Sub Start(ByVal allAtReplies As Boolean, ByVal trackwords As String)
            Me.AllAtReplies = allAtReplies
            Me.TrackWords = trackwords
            _streamActive = True
            If _streamThread IsNot Nothing AndAlso _streamThread.IsAlive Then Exit Sub
            _streamThread = New Thread(AddressOf UserStreamLoop)
            _streamThread.Name = "UserStreamReceiver"
            _streamThread.IsBackground = True
            _streamThread.Start()
        End Sub

        Public ReadOnly Property Enabled() As Boolean
            Get
                Return _streamActive
            End Get
        End Property

        Public Property AllAtReplies As Boolean
            Get
                Return _allAtreplies
            End Get
            Set(ByVal value As Boolean)
                _allAtreplies = value
            End Set
        End Property

        Public Property TrackWords As String
            Get
                Return _trackwords
            End Get
            Set(ByVal value As String)
                _trackwords = value
            End Set
        End Property

        Private Sub UserStreamLoop()
            Dim st As Stream = Nothing
            Dim sr As StreamReader = Nothing
            Dim sleepSec As Integer = 0
            Do
                Try
                    If Not MyCommon.IsNetworkAvailable() Then
                        sleepSec = 30
                        Continue Do
                    End If

                    RaiseEvent Started()
                    Dim res As HttpStatusCode = twCon.UserStream(st, _allAtreplies, _trackwords, GetUserAgentString())

                    Select Case res
                        Case HttpStatusCode.OK
                            Twitter.AccountState = ACCOUNT_STATE.Valid
                        Case HttpStatusCode.Unauthorized
                            Twitter.AccountState = ACCOUNT_STATE.Invalid
                            sleepSec = 120
                            Continue Do
                    End Select

                    If st Is Nothing Then
                        sleepSec = 30
                        'TraceOut("Stop:stream is Nothing")
                        Continue Do
                    End If

                    sr = New StreamReader(st)

                    Do While _streamActive AndAlso Not sr.EndOfStream AndAlso Twitter.AccountState = ACCOUNT_STATE.Valid
                        RaiseEvent StatusArrived(sr.ReadLine())
                        'Me.LastTime = Now
                    Loop

                    If sr.EndOfStream OrElse Twitter.AccountState = ACCOUNT_STATE.Invalid Then
                        sleepSec = 30
                        'TraceOut("Stop:EndOfStream")
                        Continue Do
                    End If
                    Exit Do
                Catch ex As WebException
                    If ex.Status = WebExceptionStatus.Timeout Then
                        sleepSec = 30                        'TraceOut("Stop:Timeout")
                    ElseIf ex.Response IsNot Nothing AndAlso CType(ex.Response, HttpWebResponse).StatusCode = 420 Then
                        'TraceOut("Stop:Connection Limit")
                        Exit Do
                    Else
                        sleepSec = 30
                        'TraceOut("Stop:WebException " & ex.Status.ToString)
                    End If
                Catch ex As ThreadAbortException
                    Exit Do
                Catch ex As IOException
                    sleepSec = 30
                    'TraceOut("Stop:IOException with Active." + Environment.NewLine + ex.Message)
                Catch ex As ArgumentException
                    'System.ArgumentException: ストリームを読み取れませんでした。
                    'サーバー側もしくは通信経路上で切断された場合？タイムアウト頻発後発生
                    sleepSec = 30
                    TraceOut(ex, "Stop:ArgumentException")
                Catch ex As Exception
                    TraceOut("Stop:Exception." + Environment.NewLine + ex.Message)
                    ExceptionOut(ex)
                    sleepSec = 30
                Finally
                    If _streamActive Then RaiseEvent Stopped()
                    twCon.RequestAbort()
                    If sr IsNot Nothing Then sr.Close()
                    If st IsNot Nothing Then st.Close()
                    If sleepSec > 0 Then
                        Dim ms As Integer = 0
                        Do While _streamActive AndAlso ms < sleepSec * 1000
                            Thread.Sleep(500)
                            ms += 500
                        Loop
                    End If
                    sleepSec = 0
                End Try
            Loop While Me._streamActive

            If _streamActive Then RaiseEvent Stopped()
            TraceOut("Stop:EndLoop")
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 重複する呼び出しを検出するには

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                    _streamActive = False
                    If _streamThread IsNot Nothing AndAlso _streamThread.IsAlive Then
                        _streamThread.Abort()
                    End If
                End If

                ' TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下の Finalize() をオーバーライドします。
                ' TODO: 大きなフィールドを null に設定します。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: 上の Dispose(ByVal disposing As Boolean) にアンマネージ リソースを解放するコードがある場合にのみ、Finalize() をオーバーライドします。
        'Protected Overrides Sub Finalize()
        '    ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
#End Region

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 重複する呼び出しを検出するには

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: マネージ状態を破棄します (マネージ オブジェクト)。
                Me.StopUserStream()
            End If

            ' TODO: アンマネージ リソース (アンマネージ オブジェクト) を解放し、下の Finalize() をオーバーライドします。
            ' TODO: 大きなフィールドを null に設定します。
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: 上の Dispose(ByVal disposing As Boolean) にアンマネージ リソースを解放するコードがある場合にのみ、Finalize() をオーバーライドします。
    'Protected Overrides Sub Finalize()
    '    ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' このコードは、破棄可能なパターンを正しく実装できるように Visual Basic によって追加されました。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' このコードを変更しないでください。クリーンアップ コードを上の Dispose(ByVal disposing As Boolean) に記述します。
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
