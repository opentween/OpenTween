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

Imports System.Xml
Imports System.Web
Imports System.Text.RegularExpressions

Public Class ShowUserInfo

    Private userInfoXml As String = ""
    Private _info As New UserInfo
    Private icondata As Image = Nothing
    Private atlist As New Generic.List(Of String)
    Private descriptionTxt As String
    Private recentPostTxt As String
    Private ToolTipWeb As String

    Private Const Mainpath As String = "http://twitter.com/"
    Private Const Followingpath As String = "/following"
    Private Const Followerspath As String = "/followers"
    Private Const Favpath As String = "/favorites"

    Private Home As String
    Private Following As String
    Private Followers As String
    Private Favorites As String
    Private MyOwner As TweenMain
    Private FriendshipResult As String = ""

    Private Class UserInfo
        Public Id As Int64 = 0
        Public Name As String = ""
        Public ScreenName As String = ""
        Public Location As String = ""
        Public Description As String = ""
        Public ImageUrl As Uri = Nothing
        Public Url As String = ""
        Public Protect As Boolean = False
        Public FriendsCount As Integer = 0
        Public FollowersCount As Integer = 0
        Public FavoriteCount As Integer = 0
        Public CreatedAt As New DateTime
        Public StatusesCount As Integer = 0
        Public Verified As Boolean = False
        Public RecentPost As String = ""
        Public PostCreatedAt As New DateTime
        Public PostSource As String = ""        ' html形式　"<a href="http://sourceforge.jp/projects/tween/wiki/FrontPage" rel="nofollow">Tween</a>"
        Public isFollowing As Boolean = False
        Public isFollowed As Boolean = False
    End Class

    Private Sub InitPath()
        Home = Mainpath + _info.ScreenName
        Following = Home + Followingpath
        Followers = Home + Followerspath
        Favorites = Home + Favpath
    End Sub

    Private Sub InitTooltip()
        ToolTip1.SetToolTip(LinkLabelTweet, Home)
        ToolTip1.SetToolTip(LinkLabelFollowing, Following)
        ToolTip1.SetToolTip(LinkLabelFollowers, Followers)
        ToolTip1.SetToolTip(LinkLabelFav, Favorites)
    End Sub

    Private Function AnalizeUserInfo(ByVal xmlData As String) As Boolean
        If xmlData Is Nothing Then Return False
        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(xmlData)
            _info.Id = Int64.Parse(xdoc.SelectSingleNode("/user/id").InnerText)
            _info.Name = xdoc.SelectSingleNode("/user/name").InnerText
            _info.ScreenName = xdoc.SelectSingleNode("/user/screen_name").InnerText
            _info.Location = xdoc.SelectSingleNode("/user/location").InnerText
            _info.Description = xdoc.SelectSingleNode("/user/description").InnerText
            _info.ImageUrl = New Uri(xdoc.SelectSingleNode("/user/profile_image_url").InnerText)

            _info.Url = xdoc.SelectSingleNode("/user/url").InnerText

            _info.Protect = Boolean.Parse(xdoc.SelectSingleNode("/user/protected").InnerText)
            _info.FriendsCount = Integer.Parse(xdoc.SelectSingleNode("/user/friends_count").InnerText)
            _info.FollowersCount = Integer.Parse(xdoc.SelectSingleNode("/user/followers_count").InnerText)
            _info.FavoriteCount = Integer.Parse(xdoc.SelectSingleNode("/user/favourites_count").InnerText)
            _info.CreatedAt = DateTime.ParseExact(xdoc.SelectSingleNode("/user/created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
            _info.StatusesCount = Integer.Parse(xdoc.SelectSingleNode("/user/statuses_count").InnerText)
            _info.Verified = Boolean.Parse(xdoc.SelectSingleNode("/user/verified").InnerText)

            ' 最終発言が取れないことがある
            Try
                _info.RecentPost = xdoc.SelectSingleNode("/user/status/text").InnerText
                _info.PostCreatedAt = DateTime.ParseExact(xdoc.SelectSingleNode("/user/status/created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
                _info.PostSource = xdoc.SelectSingleNode("/user/status/source").InnerText
            Catch ex As Exception
                _info.RecentPost = Nothing
                _info.PostCreatedAt = Nothing
                _info.PostSource = Nothing
            End Try
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Private Sub ShowUserInfo_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        MyOwner = DirectCast(Me.Owner, TweenMain)
        If Not AnalizeUserInfo(userInfoXml) Then
            MessageBox.Show(My.Resources.ShowUserInfo1)
            Me.Close()
            Exit Sub
        End If

        Dim webtext As String
        'アイコンロード
        BackgroundWorkerImageLoader.RunWorkerAsync()

        InitPath()
        InitTooltip()
        Me.Text = Me.Text.Insert(0, _info.ScreenName + " ")
        LabelId.Text = _info.Id.ToString
        LabelScreenName.Text = _info.ScreenName
        LabelName.Text = _info.Name

        LabelLocation.Text = _info.Location


        webtext = MyOwner.TwitterInstance.PreProcessUrl("<a href=""" + _info.Url + """>Dummy</a>")
        webtext = MyOwner.TwitterInstance.ShortUrlResolve(webtext)
        ToolTip1.SetToolTip(LinkLabelWeb, _
                            Regex.Match(webtext, "<a href=""(?<url>.*?)""").Groups.Item("url").Value)
        LinkLabelWeb.Text = _info.Url

        DescriptionBrowser.Visible = False
        descriptionTxt = MyOwner.createDetailHtml( _
                MyOwner.TwitterInstance.CreateHtmlAnchor(_info.Description, atlist))

        RecentPostBrowser.Visible = False
        If _info.RecentPost IsNot Nothing Then
            recentPostTxt = MyOwner.createDetailHtml( _
                MyOwner.TwitterInstance.CreateHtmlAnchor(_info.RecentPost, atlist) + _
                 " Posted at " + _info.PostCreatedAt.ToString + _
                 " via " + _info.PostSource)
        End If

        LinkLabelFollowing.Text = _info.FriendsCount.ToString
        LinkLabelFollowers.Text = _info.FollowersCount.ToString
        LinkLabelFav.Text = _info.FavoriteCount.ToString
        LinkLabelTweet.Text = _info.StatusesCount.ToString

        LabelCreatedAt.Text = _info.CreatedAt.ToString

        If _info.Protect Then
            LabelIsProtected.Text = My.Resources.Yes
        Else
            LabelIsProtected.Text = My.Resources.No
        End If

        If _info.Verified Then
            LabelIsVerified.Text = My.Resources.Yes
        Else
            LabelIsVerified.Text = My.Resources.No
        End If
    End Sub

    Private Sub ButtonClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonClose.Click
        Me.Close()
    End Sub

    Public WriteOnly Property XmlData() As String
        Set(ByVal value As String)
            userInfoXml = value
        End Set
    End Property

    Private Sub LinkLabelWeb_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabelWeb.LinkClicked
        If _info.Url IsNot Nothing Then
            MyOwner.OpenUriAsync(_info.Url)
        End If
    End Sub

    Private Sub LinkLabelFollowing_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabelFollowing.LinkClicked
        MyOwner.OpenUriAsync(Following)
    End Sub

    Private Sub LinkLabelFollowers_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabelFollowers.LinkClicked
        MyOwner.OpenUriAsync(Followers)
    End Sub

    Private Sub LinkLabelTweet_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabelTweet.LinkClicked
        MyOwner.OpenUriAsync(Home)
    End Sub

    Private Sub LinkLabelFav_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabelFav.LinkClicked
        MyOwner.OpenUriAsync(Favorites)
    End Sub

    Private Sub ButtonFollow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonFollow.Click
        Dim ret As String = MyOwner.TwitterInstance.PostFollowCommand(_info.ScreenName)
        If Not String.IsNullOrEmpty(ret) Then
            MessageBox.Show(My.Resources.FRMessage2 + ret)
        Else
            MessageBox.Show(My.Resources.FRMessage3)
            LabelIsFollowing.Text = My.Resources.GetFriendshipInfo1
            ButtonFollow.Enabled = False
            ButtonUnFollow.Enabled = True
        End If
    End Sub

    Private Sub ButtonUnFollow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonUnFollow.Click
        Dim ret As String = MyOwner.TwitterInstance.PostRemoveCommand(_info.ScreenName)
        If Not String.IsNullOrEmpty(ret) Then
            MessageBox.Show(My.Resources.FRMessage2 + ret)
        Else
            MessageBox.Show(My.Resources.FRMessage3)
            LabelIsFollowing.Text = My.Resources.GetFriendshipInfo2
            ButtonFollow.Enabled = True
            ButtonUnFollow.Enabled = False
        End If
    End Sub

    Private Sub ShowUserInfo_Activated(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Activated
        '画面が他画面の裏に隠れると、アイコン画像が再描画されない問題の対応
        If UserPicture.Image IsNot Nothing Then
            UserPicture.Invalidate(False)
        End If
    End Sub

    Private Sub ShowUserInfo_FormClosing(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles MyBase.FormClosing
        UserPicture.Image = Nothing
        If icondata IsNot Nothing Then
            icondata.Dispose()
        End If
    End Sub

    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorkerImageLoader.DoWork
        Try
            icondata = (New HttpVarious).GetImage(_info.ImageUrl.ToString())
        Catch ex As Exception
            icondata = Nothing
        End Try
        If MyOwner.TwitterInstance.Username = _info.ScreenName Then Exit Sub

        _info.isFollowing = False
        _info.isFollowed = False
        FriendshipResult = MyOwner.TwitterInstance.GetFriendshipInfo(_info.ScreenName, _info.isFollowing, _info.isFollowed)
    End Sub

    Private Sub BackgroundWorker1_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorkerImageLoader.RunWorkerCompleted
        Try
            If icondata IsNot Nothing Then
                UserPicture.Image = icondata
            End If
        Catch ex As Exception
            UserPicture.Image = Nothing
        End Try

        If MyOwner.TwitterInstance.Username = _info.ScreenName Then
            ' 自分の場合
            LabelIsFollowing.Text = ""
            LabelIsFollowed.Text = ""
            ButtonFollow.Enabled = False
            ButtonUnFollow.Enabled = False
        Else
            If FriendshipResult = "" Then
                If _info.isFollowing Then
                    LabelIsFollowing.Text = My.Resources.GetFriendshipInfo1
                Else
                    LabelIsFollowing.Text = My.Resources.GetFriendshipInfo2
                End If
                ButtonFollow.Enabled = Not _info.isFollowing
                If _info.isFollowed Then
                    LabelIsFollowed.Text = My.Resources.GetFriendshipInfo3
                Else
                    LabelIsFollowed.Text = My.Resources.GetFriendshipInfo4
                End If
                ButtonUnFollow.Enabled = _info.isFollowing
            Else
                MessageBox.Show(FriendshipResult)
                ButtonUnFollow.Enabled = False
                ButtonFollow.Enabled = False
                LabelIsFollowed.Text = My.Resources.GetFriendshipInfo6
                LabelIsFollowing.Text = My.Resources.GetFriendshipInfo6
            End If
        End If

    End Sub

    Private Sub ShowUserInfo_Shown(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Shown
        DescriptionBrowser.DocumentText = descriptionTxt
        DescriptionBrowser.Visible = True
        If _info.RecentPost IsNot Nothing Then
            RecentPostBrowser.DocumentText = recentPostTxt
            RecentPostBrowser.Visible = True
        Else
            LabelRecentPost.Text = My.Resources.ShowUserInfo2
        End If
        ButtonClose.Focus()
    End Sub

    Private Sub WebBrowser_Navigating(ByVal sender As System.Object, ByVal e As System.Windows.Forms.WebBrowserNavigatingEventArgs) Handles DescriptionBrowser.Navigating, RecentPostBrowser.Navigating
        If e.Url.AbsoluteUri <> "about:blank" Then
            e.Cancel = True

            If e.Url.AbsoluteUri.StartsWith("http://twitter.com/search?q=%23") OrElse _
               e.Url.AbsoluteUri.StartsWith("https://twitter.com/search?q=%23") Then
                'ハッシュタグの場合は、タブで開く
                Dim urlStr As String = HttpUtility.UrlDecode(e.Url.AbsoluteUri)
                Dim hash As String = urlStr.Substring(urlStr.IndexOf("#"))
                MyOwner.AddNewTabForSearch(hash)
                Exit Sub
            Else
                MyOwner.OpenUriAsync(e.Url.OriginalString)
            End If
        End If
    End Sub

    Private Sub WebBrowser_StatusTextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles DescriptionBrowser.StatusTextChanged, RecentPostBrowser.StatusTextChanged
        Dim ComponentInstance As WebBrowser = DirectCast(sender, WebBrowser)
        If ComponentInstance.StatusText.StartsWith("http") Then
            ToolTip1.Show(ComponentInstance.StatusText, Me, PointToClient(MousePosition))
        ElseIf DescriptionBrowser.StatusText = "" Then
            ToolTip1.Hide(Me)
        End If
    End Sub

    Private Sub SelectAllToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectAllToolStripMenuItem.Click
        Dim sc As WebBrowser = TryCast(ContextMenuStrip1.SourceControl, WebBrowser)
        If sc IsNot Nothing Then
            sc.Document.ExecCommand("SelectAll", False, Nothing)
        End If
    End Sub

    Private Sub SelectionCopyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectionCopyToolStripMenuItem.Click
        Dim sc As WebBrowser = TryCast(ContextMenuStrip1.SourceControl, WebBrowser)
        If sc IsNot Nothing Then
            Dim _selText As String = MyOwner.WebBrowser_GetSelectionText(sc)
            If _selText IsNot Nothing Then
                Try
                    Clipboard.SetDataObject(_selText, False, 5, 100)
                Catch ex As Exception
                    MessageBox.Show(ex.Message)
                End Try
            End If
        End If
    End Sub

    Private Sub ContextMenuStrip1_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening
        Dim sc As WebBrowser = TryCast(ContextMenuStrip1.SourceControl, WebBrowser)
        If sc IsNot Nothing Then
            Dim _selText As String = MyOwner.WebBrowser_GetSelectionText(sc)
            If _selText Is Nothing Then
                SelectionCopyToolStripMenuItem.Enabled = False
            Else
                SelectionCopyToolStripMenuItem.Enabled = True
            End If
        End If
    End Sub

    Private Sub ShowUserInfo_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.MouseEnter
        ToolTip1.Hide(Me)
    End Sub

    Private Sub LinkLabel1_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        TweenMain.OpenUriAsync("http://twitter.com/help/verified")
    End Sub

    Private Sub ButtonSearchPosts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonSearchPosts.Click
        MyOwner.AddNewTabForSearch("from:" + _info.ScreenName)
    End Sub
End Class