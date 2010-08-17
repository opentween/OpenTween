' Tween - Client of Twitter
' Copyright (c) 2007-2010 kiri_feather (@kiri_feather) <kiri_feather@gmail.com>
'           (c) 2008-2010 Moz (@syo68k) <http://iddy.jp/profile/moz/>
'           (c) 2008-2010 takeshik (@takeshik) <http://www.takeshik.org/>
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

Imports System.Web
Imports System.Xml
Imports System.Text
Imports System.Threading
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports System.Diagnostics
Imports System.Net

Public Class Twitter
    Delegate Sub GetIconImageDelegate(ByVal post As PostClass)
    Private ReadOnly LockObj As New Object
    Private followerId As New List(Of Long)
    Private _GetFollowerResult As Boolean = False

    Private _followersCount As Integer = 0
    Private _friendsCount As Integer = 0
    Private _statusesCount As Integer = 0
    Private _location As String = ""
    Private _bio As String = ""
    Private _userinfoxml As String = ""
    Private _protocol As String = "https://"

    'プロパティからアクセスされる共通情報
    Private _uid As String
    Private _iconSz As Integer
    Private _getIcon As Boolean
    Private _lIcon As ImageList
    Private _dIcon As Dictionary(Of String, Image)

    Private _tinyUrlResolve As Boolean
    Private _restrictFavCheck As Boolean

    Private _hubServer As String
    Private _countApi As Integer
    Private _countApiReply As Integer
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
    Private minFavorites As Long = Long.MaxValue

    Private twCon As New HttpTwitter

    Public Function Authenticate(ByVal username As String, ByVal password As String) As String

        Dim res As HttpStatusCode
        Dim content As String = ""

        _infoapi.Initialize()
        Try
            res = twCon.AuthUserAndPass(username, password)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                _uid = username.ToLower
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText
                Catch ex As Exception
                    Return "Err:Forbidden"
                End Try
            Case Else
                Return "Err:" + res.ToString
        End Select

    End Function

    Public Sub ClearAuthInfo()
        Twitter.AccountState = ACCOUNT_STATE.Invalid
        twCon.ClearAuthInfo()
        _UserIdNo = ""
    End Sub

    Public Sub Initialize(ByVal token As String, ByVal tokenSecret As String, ByVal username As String)
        'xAuth認証
        If String.IsNullOrEmpty(token) OrElse String.IsNullOrEmpty(tokenSecret) OrElse String.IsNullOrEmpty(username) Then
            Twitter.AccountState = ACCOUNT_STATE.Invalid
        End If
        _infoapi.Initialize()
        twCon.Initialize(token, tokenSecret, username)
        _uid = username.ToLower
        _UserIdNo = ""
    End Sub

    Public Sub Initialize(ByVal username As String, ByVal password As String)
        'BASIC認証
        If String.IsNullOrEmpty(username) OrElse String.IsNullOrEmpty(password) Then
            Twitter.AccountState = ACCOUNT_STATE.Invalid
        End If
        _infoapi.Initialize()
        twCon.Initialize(username, password)
        _uid = username.ToLower
        _UserIdNo = ""
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
        Dim m As Match = Regex.Match(retStr, "<a [^>]+>[#|＃](?<1>[a-zA-Z0-9_]+)</a>")
        While m.Success
            SyncLock LockObj
                _hashList.Add("#" + m.Groups(1).Value)
            End SyncLock
            m = m.NextMatch
        End While
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

    Private Sub GetIconImage(ByVal post As PostClass)
        Dim img As Image
        Dim bmp2 As Bitmap

        Try
            If Not _getIcon Then
                post.ImageIndex = -1
                TabInformations.GetInstance.AddPost(post)
                Exit Sub
            End If

            SyncLock LockObj
                post.ImageIndex = _lIcon.Images.IndexOfKey(post.ImageUrl)
            End SyncLock

            If post.ImageIndex > -1 Then
                TabInformations.GetInstance.AddPost(post)
                Exit Sub
            End If

            Dim httpVar As New HttpVarious
            img = httpVar.GetImage(post.ImageUrl, 10000)
            If img Is Nothing Then
                post.ImageIndex = -1
                TabInformations.GetInstance.AddPost(post)
                Exit Sub
            End If

            If _endingFlag Then Exit Sub

            SyncLock LockObj
                post.ImageIndex = _lIcon.Images.IndexOfKey(post.ImageUrl)
                If post.ImageIndex = -1 Then
                    Try
                        bmp2 = New Bitmap(_iconSz, _iconSz)
                        Using g As Graphics = Graphics.FromImage(bmp2)
                            g.InterpolationMode = Drawing2D.InterpolationMode.High
                            g.DrawImage(img, 0, 0, _iconSz, _iconSz)
                            g.Dispose()
                        End Using

                        _dIcon.Add(post.ImageUrl, img)
                        _lIcon.Images.Add(post.ImageUrl, bmp2)
                        post.ImageIndex = _lIcon.Images.IndexOfKey(post.ImageUrl)
                    Catch ex As InvalidOperationException
                        'タイミングにより追加できない場合がある？（キー重複ではない）
                        post.ImageIndex = -1
                    Catch ex As System.OverflowException
                        '不正なアイコン？DrawImageに失敗する場合あり
                        post.ImageIndex = -1
                    Catch ex As OutOfMemoryException
                        'DrawImageで発生
                        post.ImageIndex = -1
                    End Try
                End If
            End SyncLock
            TabInformations.GetInstance.AddPost(post)
        Catch ex As ArgumentException
            'タイミングによってはキー重複
        Finally
            img = Nothing
            bmp2 = Nothing
            post = Nothing
        End Try
    End Sub

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

    Private Function IsPostRestricted(ByRef resMsg As String) As Boolean
        Static _prev As New PostInfo("", "", "", "")
        Dim _current As New PostInfo("", "", "", "")


        Dim xd As XmlDocument = New XmlDocument()
        Try
            xd.LoadXml(resMsg)
            _current.CreatedAt = xd.SelectSingleNode("/status/created_at/text()").Value
            _current.Id = xd.SelectSingleNode("/status/id/text()").Value
            _current.Text = xd.SelectSingleNode("/status/text/text()").Value
            _current.UserId = xd.SelectSingleNode("/status/user/id/text()").Value

            If _current.Equals(_prev) Then
                Return True
            End If
            _prev.CreatedAt = _current.CreatedAt
            _prev.Id = _current.Id
            _prev.Text = _current.Text
            _prev.UserId = _current.UserId
        Catch ex As XmlException
            Return False
        End Try

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
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Dim xd As XmlDocument = New XmlDocument()
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/status/user/followers_count/text()")
                    If xNode IsNot Nothing Then _followersCount = Integer.Parse(xNode.Value)
                    xNode = xd.SelectSingleNode("/status/user/friends_count/text()")
                    If xNode IsNot Nothing Then _friendsCount = Integer.Parse(xNode.Value)
                    xNode = xd.SelectSingleNode("/status/user/statuses_count/text()")
                    If xNode IsNot Nothing Then _statusesCount = Integer.Parse(xNode.Value)
                    xNode = xd.SelectSingleNode("/status/user/location/text()")
                    If xNode IsNot Nothing Then _location = xNode.Value
                    xNode = xd.SelectSingleNode("/status/user/description/text()")
                    If xNode IsNot Nothing Then _bio = xNode.Value
                    xNode = xd.SelectSingleNode("/status/user/id/text()")
                    If xNode IsNot Nothing Then _UserIdNo = xNode.Value

                    _userinfoxml = String.Copy(content)
                Catch ex As Exception
                    _userinfoxml = ""
                    Return ""
                End Try

                If Not postStr.StartsWith("D ", StringComparison.OrdinalIgnoreCase) AndAlso _
                        Not postStr.StartsWith("DM ", StringComparison.OrdinalIgnoreCase) AndAlso _
                        IsPostRestricted(content) Then
                    Return "OK:Delaying?"
                End If
                If op.Post(postStr.Length) Then
                    Return ""
                Else
                    Return "Outputz:Failed"
                End If
            Case HttpStatusCode.Forbidden, HttpStatusCode.BadRequest
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Warn:" + xNode.InnerText
                Catch ex As Exception
                End Try
                Return "Warn:" + res.ToString
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
                Return "Check your Username/Password."
            Case Else
                Return "Err:" + res.ToString
        End Select
    End Function

    Public Function SendDirectMessage(ByVal postStr As String) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

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
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Dim xd As XmlDocument = New XmlDocument()
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/status/user/followers_count/text()")
                    If xNode IsNot Nothing Then _followersCount = Integer.Parse(xNode.Value)
                    xNode = xd.SelectSingleNode("/status/user/friends_count/text()")
                    If xNode IsNot Nothing Then _friendsCount = Integer.Parse(xNode.Value)
                    xNode = xd.SelectSingleNode("/status/user/statuses_count/text()")
                    If xNode IsNot Nothing Then _statusesCount = Integer.Parse(xNode.Value)
                    xNode = xd.SelectSingleNode("/status/user/location/text()")
                    If xNode IsNot Nothing Then _location = xNode.Value
                    xNode = xd.SelectSingleNode("/status/user/description/text()")
                    If xNode IsNot Nothing Then _bio = xNode.Value
                    xNode = xd.SelectSingleNode("/status/user/id/text()")
                    If xNode IsNot Nothing Then _userIdNo = xNode.Value
                Catch ex As Exception
                    Return ""
                End Try

                If op.Post(postStr.Length) Then
                    Return ""
                Else
                    Return "Outputz:Failed"
                End If
            Case HttpStatusCode.Forbidden, HttpStatusCode.BadRequest
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Warn:" + xNode.InnerText
                Catch ex As Exception
                End Try
                Return "Warn:" + res.ToString
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
                Return "Check your Username/Password."
            Case Else
                Return "Err:" + res.ToString
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
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.NotFound
                Return ""
            Case Else
                Return "Err:" + res.ToString
        End Select

    End Function

    Public Function PostRetweet(ByVal id As Long, ByVal read As Boolean) As String
        If _endingFlag Then Return ""
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        'データ部分の生成
        Dim target As Long = id
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
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case Is <> HttpStatusCode.OK
                Return "Err:" + res.ToString()
        End Select

        Twitter.AccountState = ACCOUNT_STATE.Valid

        Dim dlgt As GetIconImageDelegate    'countQueryに合わせる
        Dim ar As IAsyncResult              'countQueryに合わせる
        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
        Catch ex As Exception
            TraceOut(content)
            'MessageBox.Show("不正なXMLです。(TL-LoadXml)")
            Return "Invalid XML!"
        End Try

        'ReTweetしたものをTLに追加
        Dim xentryNode As XmlNode = xdoc.DocumentElement.SelectSingleNode("/status")
        If xentryNode Is Nothing Then Return "Invalid XML!"
        Dim xentry As XmlElement = CType(xentryNode, XmlElement)
        Dim post As New PostClass
        Try
            post.Id = Long.Parse(xentry.Item("id").InnerText)
            '二重取得回避
            SyncLock LockObj
                If TabInformations.GetInstance.ContainsKey(post.Id) Then Return ""
            End SyncLock
            'Retweet判定
            Dim xRnode As XmlNode = xentry.SelectSingleNode("./retweeted_status")
            If xRnode Is Nothing Then Return "Invalid XML!"

            Dim xRentry As XmlElement = CType(xRnode, XmlElement)
            post.PDate = DateTime.ParseExact(xRentry.Item("created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
            'Id
            post.RetweetedId = Long.Parse(xRentry.Item("id").InnerText)
            '本文
            post.Data = xRentry.Item("text").InnerText
            'Source取得（htmlの場合は、中身を取り出し）
            post.Source = xRentry.Item("source").InnerText
            'Reply先
            Long.TryParse(xRentry.Item("in_reply_to_status_id").InnerText, post.InReplyToId)
            post.InReplyToUser = xRentry.Item("in_reply_to_screen_name").InnerText
            post.IsFav = TabInformations.GetInstance.GetTabByType(TabUsageType.Favorites).Contains(post.RetweetedId)

            '以下、ユーザー情報
            Dim xRUentry As XmlElement = CType(xRentry.SelectSingleNode("./user"), XmlElement)
            post.Uid = Long.Parse(xRUentry.Item("id").InnerText)
            post.Name = xRUentry.Item("screen_name").InnerText
            post.Nickname = xRUentry.Item("name").InnerText
            post.ImageUrl = xRUentry.Item("profile_image_url").InnerText
            post.IsProtect = Boolean.Parse(xRUentry.Item("protected").InnerText)
            post.IsMe = True

            'Retweetした人(自分のはず)
            Dim xUentry As XmlElement = CType(xentry.SelectSingleNode("./user"), XmlElement)
            post.RetweetedBy = xUentry.Item("screen_name").InnerText

            'HTMLに整形
            post.OriginalData = CreateHtmlAnchor(post.Data, post.ReplyToList)
            post.Data = HttpUtility.HtmlDecode(post.Data)
            post.Data = post.Data.Replace("<3", "♡")
            'Source整形
            If post.Source.StartsWith("<") Then
                'Dim rgS As New Regex(">(?<source>.+)<")
                Dim mS As Match = Regex.Match(post.Source, ">(?<source>.+)<")
                If mS.Success Then
                    post.Source = HttpUtility.HtmlDecode(mS.Result("${source}"))
                End If
            End If

            post.IsRead = read
            post.IsReply = post.ReplyToList.Contains(_uid)

            If post.IsMe Then
                post.IsOwl = False
            Else
                If followerId.Count > 0 Then post.IsOwl = Not followerId.Contains(post.Uid)
            End If
            If post.IsMe AndAlso _readOwnPost Then post.IsRead = True

            post.IsDm = False
        Catch ex As Exception
            TraceOut(content)
            'MessageBox.Show("不正なXMLです。(TL-Parse)")
            Return "Invalid XML!"
        End Try

        '非同期アイコン取得＆StatusDictionaryに追加
        dlgt = New GetIconImageDelegate(AddressOf GetIconImage)
        ar = dlgt.BeginInvoke(post, Nothing, Nothing)

        'アイコン取得完了待ち
        Try
            dlgt.EndInvoke(ar)
        Catch ex As Exception
            '最後までendinvoke回す（ゾンビ化回避）
            ex.Data("IsTerminatePermission") = False
            Throw
        End Try

        Return ""
    End Function

    Public Function RemoveDirectMessage(ByVal id As Long) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode

        Try
            res = twCon.DestroyDirectMessage(id)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.NotFound
                Return ""
            Case Else
                Return "Err:" + res.ToString
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
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText
                Catch ex As Exception
                    Return "Err:Forbidden"
                End Try
            Case Else
                Return "Err:" + res.ToString
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
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText
                Catch ex As Exception
                    Return "Err:Forbidden"
                End Try
            Case Else
                Return "Err:" + res.ToString
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
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText
                Catch ex As Exception
                    Return "Err:Forbidden"
                End Try
            Case Else
                Return "Err:" + res.ToString
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
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText
                Catch ex As Exception
                    Return "Err:Forbidden"
                End Try
            Case Else
                Return "Err:" + res.ToString
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
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText
                Catch ex As Exception
                    Return "Err:Forbidden"
                End Try
            Case Else
                Return "Err:" + res.ToString
        End Select
    End Function

    Public Function GetFriendshipInfo(ByVal screenName As String, ByRef isFollowing As Boolean, ByRef isFollowed As Boolean) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.ShowFriendships(_uid, screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Dim xdoc As New XmlDocument
                Dim result As String = ""
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Try
                    xdoc.LoadXml(content)
                    isFollowing = Boolean.Parse(xdoc.SelectSingleNode("/relationship/source/following").InnerText)
                    isFollowed = Boolean.Parse(xdoc.SelectSingleNode("/relationship/source/followed_by").InnerText)
                Catch ex As Exception
                    result = "Err:Invalid XML."
                End Try
                Return result
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case Else
                Return "Err:" + res.ToString
        End Select
    End Function

    Public Function GetUserInfo(ByVal screenName As String, ByRef xmlBuf As String) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        xmlBuf = Nothing
        Try
            res = twCon.ShowUserInfo(screenName, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Dim xdoc As New XmlDocument
                Dim result As String = ""
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Try
                    xdoc.LoadXml(content)
                    xmlBuf = content
                Catch ex As Exception
                    result = "Err:Invalid XML."
                    xmlBuf = Nothing
                End Try
                Return result
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case Else
                Return "Err:" + res.ToString
        End Select
    End Function

    Public Function GetStatus_Retweeted_Count(ByVal StatusId As Long, ByRef retweeted_count As Integer) As String

        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

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
                    Dim xdoc As New XmlDocument
                    Dim xnode As XmlNodeList
                    Dim result As String = ""
                    Twitter.AccountState = ACCOUNT_STATE.Valid
                    Try
                        xdoc.LoadXml(content)
                        xnode = xdoc.GetElementsByTagName("ids")
                        retweeted_count += xnode.ItemOf(0).ChildNodes.Count
                        If xnode.ItemOf(0).ChildNodes.Count < 100 Then Exit For
                    Catch ex As Exception
                        retweeted_count = -1
                        result = "Err:Invalid XML."
                        xmlBuf = Nothing
                    End Try
                Case HttpStatusCode.BadRequest
                    retweeted_count = -1
                    Return "Err:API Limits?"
                Case HttpStatusCode.Unauthorized
                    retweeted_count = -1
                    Twitter.AccountState = ACCOUNT_STATE.Invalid
                    Return "Check your Username/Password."
                Case Else
                    retweeted_count = -1
                    Return "Err:" + res.ToString
            End Select
        Next
        Return ""
    End Function

    Public Function PostFavAdd(ByVal id As Long) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.CreateFavorites(id, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                If Not _restrictFavCheck Then Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText
                Catch ex As Exception
                    Return "Err:Forbidden"
                End Try
            Case Else
                Return "Err:" + res.ToString
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
                Try
                    Using rd As Xml.XmlTextReader = New Xml.XmlTextReader(New System.IO.StringReader(content))
                        rd.Read()
                        While rd.EOF = False
                            If rd.IsStartElement("favorited") Then
                                If rd.ReadElementContentAsBoolean() = True Then
                                    Return ""  '正常にふぁぼれている
                                Else
                                    Return "NG(Restricted?)"  '正常応答なのにふぁぼれてないので制限っぽい
                                End If
                            Else
                                rd.Read()
                            End If
                        End While
                        rd.Close()
                        Return "Err:Invalid XML!"
                    End Using
                Catch ex As XmlException
                    Return ""
                End Try
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString
        End Select

    End Function

    Public Function PostFavRemove(ByVal id As Long) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.DestroyFavorites(id, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText
                Catch ex As Exception
                    Return "Err:Forbidden"
                End Try
            Case Else
                Return "Err:" + res.ToString
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
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText
                Catch ex As Exception
                    Return "Err:Forbidden"
                End Try
            Case Else
                Return "Err:" + res.ToString
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
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
                Return ""
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.Forbidden
                Dim xd As XmlDocument = New XmlDocument
                Try
                    xd.LoadXml(content)
                    Dim xNode As XmlNode = Nothing
                    xNode = xd.SelectSingleNode("/hash/error")
                    Return "Err:" + xNode.InnerText
                Catch ex As Exception
                    Return "Err:Forbidden"
                End Try
            Case Else
                Return "Err:" + res.ToString
        End Select
    End Function

    Public ReadOnly Property Username() As String
        Get
            Return twCon.AuthenticatedUsername
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
        If Not (New HttpVarious).GetData("http://tween.sourceforge.jp/version2.txt?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), Nothing, content) Then
            Throw New Exception("GetVersionInfo Failed")
        End If
        Return content
    End Function

    Public Function GetTweenBinary(ByVal strVer As String) As String
        Try
            If Not (New HttpVarious).GetDataToFile("http://tween.sourceforge.jp/Tween" + strVer + ".gz?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), _
                                                Path.Combine(Application.StartupPath(), "TweenNew.exe")) Then
                Return "Err:Download failed"
            End If
            If Directory.Exists(Path.Combine(Application.StartupPath(), "en")) = False Then
                Directory.CreateDirectory(Path.Combine(Application.StartupPath(), "en"))
            End If
            If Not (New HttpVarious).GetDataToFile("http://tween.sourceforge.jp/TweenRes" + strVer + ".gz?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), _
                                                Path.Combine(Application.StartupPath(), "en\Tween.resourcesNew.dll")) Then
                Return "Err:Download failed"
            End If
            If Not (New HttpVarious).GetDataToFile("http://tween.sourceforge.jp/TweenUp.gz?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), _
                                                Path.Combine(Application.StartupPath(), "TweenUp.exe")) Then
                Return "Err:Download failed"
            End If
            If Not (New HttpVarious).GetDataToFile("http://tween.sourceforge.jp/TweenDll" + strVer + ".gz?" + Now.ToString("yyMMddHHmmss") + Environment.TickCount.ToString(), _
                                                Path.Combine(Application.StartupPath(), "TweenNew.XmlSerializers.dll")) Then
                Return "Err:Download failed"
            End If
            Return ""
        Catch ex As Exception
            Return "Err:Download failed"
        End Try
    End Function
#End Region

    Public WriteOnly Property ListIcon() As ImageList
        Set(ByVal value As ImageList)
            _lIcon = value
        End Set
    End Property

    Public WriteOnly Property DetailIcon() As Dictionary(Of String, Image)
        Set(ByVal value As Dictionary(Of String, Image))
            _dIcon = value
        End Set
    End Property

    Public WriteOnly Property CountApi() As Integer
        'API時の取得件数
        Set(ByVal value As Integer)
            _countApi = value
        End Set
    End Property

    Public WriteOnly Property CountApiReply() As Integer
        'API時のMentions取得件数
        Set(ByVal value As Integer)
            _countApiReply = value
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

    Public ReadOnly Property UserInfoXml As String
        Get
            Return _userinfoxml
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
                            ByVal more As Boolean) As String

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        If _endingFlag Then Return ""

        Dim countQuery As Integer
        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            If gType = WORKERTYPE.Timeline Then
                If more Then
                    res = twCon.HomeTimeline(_countApi, minHomeTimeline, 0, content)
                Else
                    res = twCon.HomeTimeline(_countApi, 0, 0, content)
                End If
                countQuery = _countApi
            Else
                If more Then
                    res = twCon.Mentions(_countApiReply, minMentions, 0, content)
                Else
                    res = twCon.Mentions(_countApiReply, 0, 0, content)
                End If
                countQuery = _countApiReply
            End If
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString()
        End Select

        If gType = WORKERTYPE.Timeline Then
            Return CreatePostsFromXml(content, gType, Nothing, read, countQuery, Me.minHomeTimeline)
        Else
            Return CreatePostsFromXml(content, gType, Nothing, read, countQuery, Me.minMentions)
        End If
    End Function

    Public Function GetListStatus(ByVal read As Boolean, _
                            ByVal tab As TabClass, _
                            ByVal more As Boolean) As String

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim page As Integer = 0
        Dim countQuery As Integer = 0
        Try
            If more Then
                res = twCon.GetListsStatuses(tab.ListInfo.UserId.ToString, tab.ListInfo.Id.ToString, _countApi, tab.OldestId, 0, content)
            Else
                res = twCon.GetListsStatuses(tab.ListInfo.UserId.ToString, tab.ListInfo.Id.ToString, _countApi, 0, 0, content)
            End If
            countQuery = _countApi
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try
        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString()
        End Select

        Return CreatePostsFromXml(content, WORKERTYPE.List, tab, read, countQuery, tab.OldestId)
    End Function

    Private Function CreatePostsFromXml(ByVal content As String, ByVal gType As WORKERTYPE, ByVal tab As TabClass, ByVal read As Boolean, ByVal count As Integer, ByRef minimumId As Long) As String
        Dim arIdx As Integer = -1
        Dim dlgt(300) As GetIconImageDelegate    'countQueryに合わせる
        Dim ar(300) As IAsyncResult              'countQueryに合わせる
        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
        Catch ex As Exception
            TraceOut(content)
            'MessageBox.Show("不正なXMLです。(TL-LoadXml)")
            Return "Invalid XML!"
        End Try

        For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("./status")
            Dim xentry As XmlElement = CType(xentryNode, XmlElement)
            Dim post As New PostClass
            Try
                post.Id = Long.Parse(xentry.Item("id").InnerText)
                If minimumId > post.Id Then minimumId = post.Id
                '二重取得回避
                SyncLock LockObj
                    If tab Is Nothing Then
                        If TabInformations.GetInstance.ContainsKey(post.Id) Then Continue For
                    Else
                        If TabInformations.GetInstance.ContainsKey(post.Id, tab.TabName) Then Continue For
                    End If
                End SyncLock
                'Retweet判定
                Dim xRnode As XmlNode = xentry.SelectSingleNode("./retweeted_status")
                If xRnode IsNot Nothing Then
                    Dim xRentry As XmlElement = CType(xRnode, XmlElement)
                    post.PDate = DateTime.ParseExact(xRentry.Item("created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
                    'Id
                    post.RetweetedId = Long.Parse(xRentry.Item("id").InnerText)
                    '本文
                    post.Data = xRentry.Item("text").InnerText
                    'Source取得（htmlの場合は、中身を取り出し）
                    post.Source = xRentry.Item("source").InnerText
                    'Reply先
                    Long.TryParse(xRentry.Item("in_reply_to_status_id").InnerText, post.InReplyToId)
                    post.InReplyToUser = xRentry.Item("in_reply_to_screen_name").InnerText
                    post.IsFav = TabInformations.GetInstance.GetTabByType(TabUsageType.Favorites).Contains(post.RetweetedId)
                    'post.IsFav = Boolean.Parse(xentry.Item("favorited").InnerText)

                    '以下、ユーザー情報
                    Dim xRUentry As XmlElement = CType(xRentry.SelectSingleNode("./user"), XmlElement)
                    post.Uid = Long.Parse(xRUentry.Item("id").InnerText)
                    post.Name = xRUentry.Item("screen_name").InnerText
                    post.Nickname = xRUentry.Item("name").InnerText
                    post.ImageUrl = xRUentry.Item("profile_image_url").InnerText
                    post.IsProtect = Boolean.Parse(xRUentry.Item("protected").InnerText)
                    post.IsMe = post.Name.ToLower.Equals(_uid)
                    If post.IsMe Then _userIdNo = post.Uid.ToString()

                    'Retweetした人
                    Dim xUentry As XmlElement = CType(xentry.SelectSingleNode("./user"), XmlElement)
                    post.RetweetedBy = xUentry.Item("screen_name").InnerText
                Else
                    post.PDate = DateTime.ParseExact(xentry.Item("created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
                    '本文
                    post.Data = xentry.Item("text").InnerText
                    'Source取得（htmlの場合は、中身を取り出し）
                    post.Source = xentry.Item("source").InnerText
                    Long.TryParse(xentry.Item("in_reply_to_status_id").InnerText, post.InReplyToId)
                    post.InReplyToUser = xentry.Item("in_reply_to_screen_name").InnerText
                    'in_reply_to_user_idを使うか？
                    post.IsFav = Boolean.Parse(xentry.Item("favorited").InnerText)

                    '以下、ユーザー情報
                    Dim xUentry As XmlElement = CType(xentry.SelectSingleNode("./user"), XmlElement)
                    post.Uid = Long.Parse(xUentry.Item("id").InnerText)
                    post.Name = xUentry.Item("screen_name").InnerText
                    post.Nickname = xUentry.Item("name").InnerText
                    post.ImageUrl = xUentry.Item("profile_image_url").InnerText
                    post.IsProtect = Boolean.Parse(xUentry.Item("protected").InnerText)
                    post.IsMe = post.Name.ToLower.Equals(_uid)
                    If post.IsMe Then _userIdNo = post.Uid.ToString()
                End If
                'HTMLに整形
                post.OriginalData = CreateHtmlAnchor(post.Data, post.ReplyToList)
                post.Data = HttpUtility.HtmlDecode(post.Data)
                post.Data = post.Data.Replace("<3", "♡")
                'Source整形
                If post.Source.StartsWith("<") Then
                    'Dim rgS As New Regex(">(?<source>.+)<")
                    Dim mS As Match = Regex.Match(post.Source, ">(?<source>.+)<")
                    If mS.Success Then
                        post.Source = HttpUtility.HtmlDecode(mS.Result("${source}"))
                    End If
                End If

                post.IsRead = read
                If gType = WORKERTYPE.Timeline OrElse tab IsNot Nothing Then
                    post.IsReply = post.ReplyToList.Contains(_uid)
                Else
                    post.IsReply = True
                End If

                If post.IsMe Then
                    post.IsOwl = False
                Else
                    If followerId.Count > 0 Then post.IsOwl = Not followerId.Contains(post.Uid)
                End If
                If post.IsMe AndAlso Not read AndAlso _readOwnPost Then post.IsRead = True

                post.IsDm = False
                If tab IsNot Nothing Then post.RelTabName = tab.TabName
            Catch ex As Exception
                TraceOut(content)
                'MessageBox.Show("不正なXMLです。(TL-Parse)")
                Continue For
            End Try

            '非同期アイコン取得＆StatusDictionaryに追加
            arIdx += 1
            If arIdx > dlgt.Length - 1 Then
                arIdx -= 1
                Exit For
            End If
            dlgt(arIdx) = New GetIconImageDelegate(AddressOf GetIconImage)
            ar(arIdx) = dlgt(arIdx).BeginInvoke(post, Nothing, Nothing)
        Next

        'アイコン取得完了待ち
        For i As Integer = 0 To arIdx
            Try
                dlgt(i).EndInvoke(ar(i))
            Catch ex As IndexOutOfRangeException
                Throw New IndexOutOfRangeException(String.Format("i={0},dlgt.Length={1},ar.Length={2},arIdx={3}", i, dlgt.Length, ar.Length, arIdx))
            Catch ex As Exception
                '最後までendinvoke回す（ゾンビ化回避）
                ex.Data("IsTerminatePermission") = False
                Throw
            End Try
        Next

        'If _ApiMethod = MySocket.REQ_TYPE.ReqGetAPI Then _remainCountApi = sck.RemainCountApi

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
        If more Then
            page = tab.SearchPage
        Else
            sinceId = tab.SinceId
        End If

        Try
            res = twCon.Search(tab.SearchWords, tab.SearchLang, 40, page, sinceId, content)
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
                Return "Err:" + res.ToString
        End Select

        If Not TabInformations.GetInstance.ContainsTab(tab) Then Return ""

        Dim arIdx As Integer = -1
        Dim dlgt(300) As GetIconImageDelegate    'countQueryに合わせる
        Dim ar(300) As IAsyncResult              'countQueryに合わせる
        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid ATOM!"
        End Try
        Dim nsmgr As New XmlNamespaceManager(xdoc.NameTable)
        nsmgr.AddNamespace("search", "http://www.w3.org/2005/Atom")
        For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("/search:feed/search:entry", nsmgr)
            Dim xentry As XmlElement = CType(xentryNode, XmlElement)
            Dim post As New PostClass
            Try
                post.Id = Long.Parse(xentry.Item("id").InnerText.Split(":"c)(2))
                If TabInformations.GetInstance.ContainsKey(post.Id, tab.TabName) Then Continue For
                post.PDate = DateTime.Parse(xentry.Item("published").InnerText)
                '本文
                post.Data = xentry.Item("title").InnerText
                'Source取得（htmlの場合は、中身を取り出し）
                post.Source = xentry.Item("twitter:source").InnerText
                post.InReplyToId = 0
                post.InReplyToUser = ""
                post.IsFav = False

                '以下、ユーザー情報
                Dim xUentry As XmlElement = CType(xentry.SelectSingleNode("./search:author", nsmgr), XmlElement)
                post.Uid = 0
                post.Name = xUentry.Item("name").InnerText.Split(" "c)(0).Trim
                post.Nickname = xUentry.Item("name").InnerText.Substring(post.Name.Length).Trim
                If post.Nickname.Length > 2 Then
                    post.Nickname = post.Nickname.Substring(1, post.Nickname.Length - 2)
                Else
                    post.Nickname = post.Name
                End If
                post.ImageUrl = CType(xentry.SelectSingleNode("./search:link[@type='image/png']", nsmgr), XmlElement).GetAttribute("href")
                post.IsProtect = False
                post.IsMe = post.Name.ToLower.Equals(_uid)

                'HTMLに整形
                post.OriginalData = CreateHtmlAnchor(HttpUtility.HtmlEncode(post.Data), post.ReplyToList)
                post.Data = HttpUtility.HtmlDecode(post.Data)
                'Source整形
                If post.Source.StartsWith("<") Then
                    'Dim rgS As New Regex(">(?<source>.+)<")
                    Dim mS As Match = Regex.Match(post.Source, ">(?<source>.+)<")
                    If mS.Success Then
                        post.Source = HttpUtility.HtmlDecode(mS.Result("${source}"))
                    End If
                End If

                post.IsRead = read
                post.IsReply = post.ReplyToList.Contains(_uid)

                post.IsOwl = False
                If post.IsMe AndAlso Not read AndAlso _readOwnPost Then post.IsRead = True
                post.IsDm = False
                post.RelTabName = tab.TabName
                If Not more AndAlso post.Id > tab.SinceId Then tab.SinceId = post.Id
            Catch ex As Exception
                TraceOut(content)
                Continue For
            End Try

            '非同期アイコン取得＆StatusDictionaryに追加
            arIdx += 1
            dlgt(arIdx) = New GetIconImageDelegate(AddressOf GetIconImage)
            ar(arIdx) = dlgt(arIdx).BeginInvoke(post, Nothing, Nothing)
        Next

        '' TODO
        '' 遡るための情報max_idやnext_pageの情報を保持する

        'アイコン取得完了待ち
        For i As Integer = 0 To arIdx
            Try
                dlgt(i).EndInvoke(ar(i))
            Catch ex As Exception
                '最後までendinvoke回す（ゾンビ化回避）
                ex.Data("IsTerminatePermission") = False
                Throw
            End Try
        Next

        Return ""
    End Function

    Public Function GetDirectMessageApi(ByVal read As Boolean, _
                            ByVal gType As WORKERTYPE, _
                            ByVal more As Boolean) As String
        If _endingFlag Then Return ""

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

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
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString()
        End Select

        Dim arIdx As Integer = -1
        Dim dlgt(300) As GetIconImageDelegate    'countQueryに合わせる
        Dim ar(300) As IAsyncResult              'countQueryに合わせる
        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
        Catch ex As Exception
            TraceOut(content)
            'MessageBox.Show("不正なXMLです。(DM-LoadXml)")
            Return "Invalid XML!"
        End Try

        For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("./direct_message")
            Dim xentry As XmlElement = CType(xentryNode, XmlElement)
            Dim post As New PostClass
            Try
                post.Id = Long.Parse(xentry.Item("id").InnerText)
                If gType = WORKERTYPE.DirectMessegeRcv Then
                    If minDirectmessage > post.Id Then minDirectmessage = post.Id
                Else
                    If minDirectmessageSent > post.Id Then minDirectmessageSent = post.Id
                End If
                '二重取得回避
                SyncLock LockObj
                    If TabInformations.GetInstance.GetTabByType(TabUsageType.DirectMessage).Contains(post.Id) Then Continue For
                End SyncLock
                'sender_id
                'recipient_id
                post.PDate = DateTime.ParseExact(xentry.Item("created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
                '本文
                post.Data = xentry.Item("text").InnerText
                'HTMLに整形
                post.OriginalData = CreateHtmlAnchor(post.Data, post.ReplyToList)
                post.Data = HttpUtility.HtmlDecode(post.Data)
                post.Data = post.Data.Replace("<3", "♡")
                post.IsFav = False
                '受信ＤＭかの判定で使用
                If gType = WORKERTYPE.DirectMessegeRcv Then
                    post.IsOwl = True
                Else
                    post.IsOwl = False
                End If

                '以下、ユーザー情報
                Dim xUentry As XmlElement
                If gType = WORKERTYPE.DirectMessegeRcv Then
                    xUentry = CType(xentry.SelectSingleNode("./sender"), XmlElement)
                    post.IsMe = False
                Else
                    xUentry = CType(xentry.SelectSingleNode("./recipient"), XmlElement)
                    post.IsMe = True
                End If
                post.Uid = Long.Parse(xUentry.Item("id").InnerText)
                post.Name = xUentry.Item("screen_name").InnerText
                post.Nickname = xUentry.Item("name").InnerText
                post.ImageUrl = xUentry.Item("profile_image_url").InnerText
                post.IsProtect = Boolean.Parse(xUentry.Item("protected").InnerText)
            Catch ex As Exception
                TraceOut(content)
                'MessageBox.Show("不正なXMLです。(DM-Parse)")
                Continue For
            End Try

            post.IsRead = read
            post.IsReply = False
            post.IsDm = True

            '非同期アイコン取得＆StatusDictionaryに追加
            arIdx += 1
            dlgt(arIdx) = New GetIconImageDelegate(AddressOf GetIconImage)
            ar(arIdx) = dlgt(arIdx).BeginInvoke(post, Nothing, Nothing)
        Next

        'アイコン取得完了待ち
        For i As Integer = 0 To arIdx
            Try
                dlgt(i).EndInvoke(ar(i))
            Catch ex As Exception
                '最後までendinvoke回す（ゾンビ化回避）
                ex.Data("IsTerminatePermission") = False
                Throw
            End Try
        Next

        Return ""
    End Function

    Public Function GetFavoritesApi(ByVal read As Boolean, _
                        ByVal gType As WORKERTYPE) As String

        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        If _endingFlag Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.Favorites(_countApi, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString()
        End Select

        Dim arIdx As Integer = -1
        Dim dlgt(300) As GetIconImageDelegate    'countQueryに合わせる
        Dim ar(300) As IAsyncResult              'countQueryに合わせる
        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
        Catch ex As Exception
            TraceOut(content)
            'MessageBox.Show("不正なXMLです。(TL-LoadXml)")
            Return "Invalid XML!"
        End Try

        For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("./status")
            Dim xentry As XmlElement = CType(xentryNode, XmlElement)
            Dim post As New PostClass
            Try
                post.Id = Long.Parse(xentry.Item("id").InnerText)
                If minFavorites > post.Id Then minFavorites = post.Id
                '二重取得回避
                SyncLock LockObj
                    If TabInformations.GetInstance.ContainsKey(post.Id) Then Continue For
                End SyncLock
                'Retweet判定
                Dim xRnode As XmlNode = xentry.SelectSingleNode("./retweeted_status")
                If xRnode IsNot Nothing Then
                    Dim xRentry As XmlElement = CType(xRnode, XmlElement)
                    post.PDate = DateTime.ParseExact(xRentry.Item("created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
                    'Id
                    post.RetweetedId = Long.Parse(xRentry.Item("id").InnerText)
                    '本文
                    post.Data = xRentry.Item("text").InnerText
                    'Source取得（htmlの場合は、中身を取り出し）
                    post.Source = xRentry.Item("source").InnerText
                    'Reply先
                    Long.TryParse(xRentry.Item("in_reply_to_status_id").InnerText, post.InReplyToId)
                    post.InReplyToUser = xRentry.Item("in_reply_to_screen_name").InnerText
                    'in_reply_to_user_idを使うか？
                    post.IsFav = Boolean.Parse(xRentry.Item("favorited").InnerText)

                    '以下、ユーザー情報
                    Dim xRUentry As XmlElement = CType(xRentry.SelectSingleNode("./user"), XmlElement)
                    post.Uid = Long.Parse(xRUentry.Item("id").InnerText)
                    post.Name = xRUentry.Item("screen_name").InnerText
                    post.Nickname = xRUentry.Item("name").InnerText
                    post.ImageUrl = xRUentry.Item("profile_image_url").InnerText
                    post.IsProtect = Boolean.Parse(xRUentry.Item("protected").InnerText)
                    post.IsMe = post.Name.ToLower.Equals(_uid)
                    If post.IsMe Then _userIdNo = post.Uid.ToString()

                    'Retweetした人
                    Dim xUentry As XmlElement = CType(xentry.SelectSingleNode("./user"), XmlElement)
                    post.RetweetedBy = xUentry.Item("screen_name").InnerText
                Else
                    post.PDate = DateTime.ParseExact(xentry.Item("created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
                    '本文
                    post.Data = xentry.Item("text").InnerText
                    'Source取得（htmlの場合は、中身を取り出し）
                    post.Source = xentry.Item("source").InnerText
                    Long.TryParse(xentry.Item("in_reply_to_status_id").InnerText, post.InReplyToId)
                    post.InReplyToUser = xentry.Item("in_reply_to_screen_name").InnerText
                    'in_reply_to_user_idを使うか？
                    post.IsFav = Boolean.Parse(xentry.Item("favorited").InnerText)

                    '以下、ユーザー情報
                    Dim xUentry As XmlElement = CType(xentry.SelectSingleNode("./user"), XmlElement)
                    post.Uid = Long.Parse(xUentry.Item("id").InnerText)
                    post.Name = xUentry.Item("screen_name").InnerText
                    post.Nickname = xUentry.Item("name").InnerText
                    post.ImageUrl = xUentry.Item("profile_image_url").InnerText
                    post.IsProtect = Boolean.Parse(xUentry.Item("protected").InnerText)
                    post.IsMe = post.Name.ToLower.Equals(_uid)
                    If post.IsMe Then _userIdNo = post.Uid.ToString()
                End If
                'HTMLに整形
                post.OriginalData = CreateHtmlAnchor(post.Data, post.ReplyToList)
                post.Data = HttpUtility.HtmlDecode(post.Data)
                post.Data = post.Data.Replace("<3", "♡")
                'Source整形
                If post.Source.StartsWith("<") Then
                    Dim mS As Match = Regex.Match(post.Source, ">(?<source>.+)<")
                    If mS.Success Then
                        post.Source = HttpUtility.HtmlDecode(mS.Result("${source}"))
                    End If
                End If

                post.IsRead = read
                post.IsReply = post.ReplyToList.Contains(_uid)

                If post.IsMe Then
                    post.IsOwl = False
                Else
                    If followerId.Count > 0 Then post.IsOwl = Not followerId.Contains(post.Uid)
                End If

                post.IsDm = False
            Catch ex As Exception
                TraceOut(content)
                'MessageBox.Show("不正なXMLです。(TL-Parse)")
                Continue For
            End Try

            '非同期アイコン取得＆StatusDictionaryに追加
            arIdx += 1
            dlgt(arIdx) = New GetIconImageDelegate(AddressOf GetIconImage)
            ar(arIdx) = dlgt(arIdx).BeginInvoke(post, Nothing, Nothing)
        Next

        'アイコン取得完了待ち
        For i As Integer = 0 To arIdx
            Try
                dlgt(i).EndInvoke(ar(i))
            Catch ex As Exception
                '最後までendinvoke回す（ゾンビ化回避）
                ex.Data("IsTerminatePermission") = False
                Throw
            End Try
        Next

        Return ""
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
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString()
        End Select

        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid XML!"
        End Try

        Try
            For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("/id_list/ids/id")
                followerId.Add(Long.Parse(xentryNode.InnerText))
            Next
            cursor = Long.Parse(xdoc.DocumentElement.SelectSingleNode("/id_list/next_cursor").InnerText)
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid XML!"
        End Try

        Return ""

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
                Return "Err:" + ex.Message
            End Try

            Select Case res
                Case HttpStatusCode.OK
                    Twitter.AccountState = ACCOUNT_STATE.Valid
                Case HttpStatusCode.Unauthorized
                    Twitter.AccountState = ACCOUNT_STATE.Invalid
                    Return "Check your Username/Password."
                Case HttpStatusCode.BadRequest
                    Return "Err:API Limits?"
                Case Else
                    Return "Err:" + res.ToString()
            End Select

            Dim xdoc As New XmlDocument
            Try
                xdoc.LoadXml(content)
            Catch ex As Exception
                TraceOut(content)
                Return "Invalid XML!"
            End Try

            Try
                For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("/lists_list/lists/list")
                    lists.Add(New ListElement(xentryNode, Me))
                Next
                cursor = Long.Parse(xdoc.DocumentElement.SelectSingleNode("/lists_list/next_cursor").InnerText)
            Catch ex As Exception
                TraceOut(content)
                Return "Invalid XML!"
            End Try
        Loop While cursor <> 0

        cursor = -1
        content = ""
        Do
            Try
                res = twCon.GetListsSubscriptions(Me.Username, cursor, content)
            Catch ex As Exception
                Return "Err:" + ex.Message
            End Try

            Select Case res
                Case HttpStatusCode.OK
                    Twitter.AccountState = ACCOUNT_STATE.Valid
                Case HttpStatusCode.Unauthorized
                    Twitter.AccountState = ACCOUNT_STATE.Invalid
                    Return "Check your Username/Password."
                Case HttpStatusCode.BadRequest
                    Return "Err:API Limits?"
                Case Else
                    Return "Err:" + res.ToString()
            End Select

            Dim xdoc As New XmlDocument
            Try
                xdoc.LoadXml(content)
            Catch ex As Exception
                TraceOut(content)
                Return "Invalid XML!"
            End Try

            Try
                For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("/lists_list/lists/list")
                    lists.Add(New ListElement(xentryNode, Me))
                Next
                cursor = Long.Parse(xdoc.DocumentElement.SelectSingleNode("/lists_list/next_cursor").InnerText)
            Catch ex As Exception
                TraceOut(content)
                Return "Invalid XML!"
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
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString()
        End Select

        Return ""
    End Function

    Public Function EditList(ByVal list_id As String, ByVal new_name As String, ByVal isPrivate As Boolean, ByVal description As String, ByRef list As ListElement) As String
        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim modeString As String = "public"
        If isPrivate Then
            modeString = "private"
        End If

        Try
            res = twCon.PostListID(Me.Username, list_id, new_name, modeString, description, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString()
        End Select

        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
            Dim newList As New ListElement(xdoc.DocumentElement, Me)
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
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid XML!"
        End Try

        Return ""
    End Function

    Public Function GetListNembers(ByVal list_id As String, ByVal lists As List(Of UserInfo)) As String
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""
        Dim cursor As Long = -1

        Do
            Try
                res = twCon.GetListNembers(Me.Username, list_id, cursor, content)
            Catch ex As Exception
                Return "Err:" + ex.Message
            End Try

            Select Case res
                Case HttpStatusCode.OK
                    Twitter.AccountState = ACCOUNT_STATE.Valid
                Case HttpStatusCode.Unauthorized
                    Twitter.AccountState = ACCOUNT_STATE.Invalid
                    Return "Check your Username/Password."
                Case HttpStatusCode.BadRequest
                    Return "Err:API Limits?"
                Case Else
                    Return "Err:" + res.ToString()
            End Select

            Dim xdoc As New XmlDocument
            Try
                xdoc.LoadXml(content)
            Catch ex As Exception
                TraceOut(content)
                Return "Invalid XML!"
            End Try

            Try
                For Each xentryNode As XmlNode In xdoc.DocumentElement.SelectNodes("/users_list/users/user")
                    lists.Add(New UserInfo(xentryNode))
                Next
                cursor = Long.Parse(xdoc.DocumentElement.SelectSingleNode("/users_list/next_cursor").InnerText)
            Catch ex As Exception
                TraceOut(content)
                Return "Invalid XML!"
            End Try
        Loop While cursor <> 0

        Return ""
    End Function

    Public Function CreateListApi(ByVal listName As String, ByVal isPrivate As Boolean, ByVal description As String) As String
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return ""

        Dim res As HttpStatusCode
        Dim content As String = ""

        Try
            res = twCon.PostLists(Me.Username, listName, isPrivate, description, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Select Case res
            Case HttpStatusCode.OK
                Twitter.AccountState = ACCOUNT_STATE.Valid
            Case HttpStatusCode.Unauthorized
                Twitter.AccountState = ACCOUNT_STATE.Invalid
                Return "Check your Username/Password."
            Case HttpStatusCode.BadRequest
                Return "Err:API Limits?"
            Case Else
                Return "Err:" + res.ToString()
        End Select

        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)

            TabInformations.GetInstance().SubscribableLists.Add(New ListElement(xdoc.DocumentElement, Me))
        Catch ex As Exception
            TraceOut(content)
            Return "Invalid XML!"
        End Try

        Return ""
    End Function

    Public Function AddUserToList(ByVal list_name As String, ByVal user As String) As String
        Dim content As String = ""
        Dim res As HttpStatusCode

        Try
            res = twCon.PostListMembers(Me.Username, list_name, user, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Return ""
    End Function

    Public Function RemoveUserToList(ByVal list_name As String, ByVal user As String) As String
        Dim content As String = ""
        Dim res As HttpStatusCode

        Try
            res = twCon.DeleteListMembers(Me.Username, list_name, user, content)
        Catch ex As Exception
            Return "Err:" + ex.Message
        End Try

        Return ""
    End Function

    Public Function CreateHtmlAnchor(ByVal Text As String, ByVal AtList As List(Of String)) As String
        Dim retStr As String = Text.Replace("&gt;", "<<<<<tweenだいなり>>>>>").Replace("&lt;", "<<<<<tweenしょうなり>>>>>")
        'uriの正規表現
        Const rgUrl As String = "(?<before>(?:[^\""':!=]|^|\:))" + _
                                    "(?<url>(?<protocol>https?://|www\.)" + _
                                    "(?<domain>(?:[\.-]|[^\p{P}\s])+\.[a-z]{2,}(?::[0-9]+)?)" + _
                                    "(?<path>/[a-z0-9!*'();:&=+$/%#\[\]\-_.,~@^]*[a-z0-9)=#/]?)?" + _
                                    "(?<query>\?[a-z0-9!*'();:&=+$/%#\[\]\-_.,~]*[a-z0-9_&=#])?)"
        '絶対パス表現のUriをリンクに置換
        retStr = Regex.Replace(retStr, rgUrl, New MatchEvaluator(AddressOf AutoLinkUrl), RegexOptions.IgnoreCase)

        '@先をリンクに置換（リスト）
        retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9_/])([@＠]+)([a-zA-Z0-9_]{1,20}/[a-zA-Z][a-zA-Z0-9\p{IsLatin-1Supplement}\-]{0,79})", "$1$2<a href=""/$3"">$3</a>")

        Dim m As Match = Regex.Match(retStr, "(^|[^a-zA-Z0-9_])[@＠]([a-zA-Z0-9_]{1,20})")
        While m.Success
            AtList.Add(m.Result("$2").ToLower)
            m = m.NextMatch

        End While
        '@先をリンクに置換
        retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9_/])([@＠])([a-zA-Z0-9_]{1,20})", "$1$2<a href=""/$3"">$3</a>")

        'ハッシュタグを抽出し、リンクに置換
        Dim mhs As MatchCollection = Regex.Matches(retStr, "(^|[^a-zA-Z0-9/&])[#＃]([0-9a-zA-Z_]*[a-zA-Z_]+[a-zA-Z_\xc0-\xd6\xd8-\xf6\xf8-\xff]*)")
        For Each mt As Match In mhs
            If Not IsNumeric(mt.Result("$2")) Then
                'retStr = retStr.Replace(mt.Result("$1") + mt.Result("$2"), "<a href=""" + _protocol + "twitter.com/search?q=%23" + mt.Result("$2") + """>#" + mt.Result("$2") + "</a>")
                SyncLock LockObj
                    _hashList.Add("#" + mt.Result("$2"))
                End SyncLock
            End If
        Next
        retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9/&])([#＃])([0-9a-zA-Z_]*[a-zA-Z_]+[a-zA-Z0-9_\xc0-\xd6\xd8-\xf6\xf8-\xff]*)", "$1<a href=""" & _protocol & "twitter.com/search?q=%23$3"">$2$3</a>")

        retStr = Regex.Replace(retStr, "(^|[^a-zA-Z0-9_/&#＃@＠>=.])(sm|nm)([0-9]{1,10})", "$1<a href=""http://www.nicovideo.jp/watch/$2$3"">$2$3</a>")

        retStr = retStr.Replace("<<<<<tweenだいなり>>>>>", "&gt;").Replace("<<<<<tweenしょうなり>>>>>", "&lt;")

        retStr = AdjustHtml(ShortUrl.Resolve(PreProcessUrl(retStr))) 'IDN置換、短縮Uri解決、@リンクを相対→絶対にしてtarget属性付与
        Return retStr
    End Function

    Private Function AutoLinkUrl(ByVal m As Match) As String
        Dim sb As New StringBuilder(m.Result("${before}<a href="""))
        If m.Result("${protocol}").StartsWith("w", StringComparison.OrdinalIgnoreCase) Then
            sb.Append("http://")
        End If
        sb.Append(m.Result("${url}"">")).Append(m.Result("${url}")).Append("</a>")
        Return sb.ToString
    End Function

    Public ReadOnly Property RemainCountApi() As Integer
        Get
            If twCon.RemainCountApi <> -1 Then
                _infoapi.Initialize()
                Return twCon.RemainCountApi
            Else
                If _infoapi.RemainCount <> -1 Then
                    Return _infoapi.RemainCount
                Else
                    Return -1
                End If
            End If
        End Get
    End Property

    Public ReadOnly Property UpperCountApi() As Integer
        Get
            If twCon.UpperCountApi <> -1 Then
                _infoapi.Initialize()
                Return twCon.UpperCountApi
            Else
                If _infoapi.MaxCount <> -1 Then
                    Return _infoapi.MaxCount
                Else
                    Return -1
                End If
            End If
        End Get
    End Property

    Public ReadOnly Property ResetTimeApi() As DateTime
        Get
            If twCon.ResetTimeApi.ToBinary <> 0 Then
                _infoapi.Initialize()
                Return twCon.ResetTimeApi
            Else
                If _infoapi.ResetTime.ToBinary <> 0 Then
                    Return _infoapi.ResetTime
                Else
                    Return New DateTime
                End If
            End If
        End Get
    End Property

    Public Class ApiInfo
        Public MaxCount As Integer = -1
        Public RemainCount As Integer = -1
        Public ResetTime As New DateTime
        Public ResetTimeInSeconds As Integer = -1
        Public UsingCount As Integer = -1

        Public Sub Initialize()
            Me.MaxCount = -1
            Me.RemainCount = -1
            Me.ResetTime = New DateTime
            Me.ResetTimeInSeconds = -1
            'UsingCountは初期化対象外
        End Sub

        Public Function ConvertResetTimeInSecondsToResetTime(ByVal seconds As Integer) As DateTime
            If seconds >= 0 Then
                Return System.TimeZone.CurrentTimeZone.ToLocalTime((New DateTime(1970, 1, 1, 0, 0, 0)).AddSeconds(seconds))
            Else
                Return New DateTime
            End If
        End Function
    End Class

    Private _infoapi As New ApiInfo

    Public Function GetInfoApi(ByVal info As ApiInfo) As Boolean
        If Twitter.AccountState <> ACCOUNT_STATE.Valid Then Return True

        If _endingFlag Then Return True

        Dim res As HttpStatusCode
        Dim content As String = ""
        Try
            res = twCon.RateLimitStatus(content)
        Catch ex As Exception
            _infoapi.Initialize()
            info.Initialize()
            Return False
        End Try

        If res <> HttpStatusCode.OK Then Return False

        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(content)
            info.MaxCount = Integer.Parse(xdoc.SelectSingleNode("/hash/hourly-limit").InnerText)
            info.RemainCount = Integer.Parse(xdoc.SelectSingleNode("/hash/remaining-hits").InnerText)
            info.ResetTime = DateTime.Parse(xdoc.SelectSingleNode("/hash/reset-time").InnerText)
            info.ResetTimeInSeconds = Integer.Parse(xdoc.SelectSingleNode("/hash/reset-time-in-seconds").InnerText)

            _infoapi.MaxCount = info.MaxCount
            _infoapi.RemainCount = info.RemainCount
            _infoapi.ResetTime = info.ResetTime
            _infoapi.ResetTimeInSeconds = info.ResetTimeInSeconds
            Return True
        Catch ex As Exception
            _infoapi.Initialize()
            info.Initialize()
            Return False
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

    Public Property UserIdNo As String

End Class
