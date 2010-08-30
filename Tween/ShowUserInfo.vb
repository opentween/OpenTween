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
Imports System.ComponentModel
Imports System.IO

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
            Dim nd As String = "/user"

            If xdoc.SelectSingleNode(nd) Is Nothing Then
                nd = "/status/user"
            End If

            _info.Id = Int64.Parse(xdoc.SelectSingleNode(nd + "/id").InnerText)
            _info.Name = xdoc.SelectSingleNode(nd + "/name").InnerText
            _info.ScreenName = xdoc.SelectSingleNode(nd + "/screen_name").InnerText
            _info.Location = xdoc.SelectSingleNode(nd + "/location").InnerText
            _info.Description = xdoc.SelectSingleNode(nd + "/description").InnerText
            _info.ImageUrl = New Uri(xdoc.SelectSingleNode(nd + "/profile_image_url").InnerText)

            _info.Url = xdoc.SelectSingleNode(nd + "/url").InnerText

            _info.Protect = Boolean.Parse(xdoc.SelectSingleNode(nd + "/protected").InnerText)
            _info.FriendsCount = Integer.Parse(xdoc.SelectSingleNode(nd + "/friends_count").InnerText)
            _info.FollowersCount = Integer.Parse(xdoc.SelectSingleNode(nd + "/followers_count").InnerText)
            _info.FavoriteCount = Integer.Parse(xdoc.SelectSingleNode(nd + "/favourites_count").InnerText)
            _info.CreatedAt = DateTime.ParseExact(xdoc.SelectSingleNode(nd + "/created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
            _info.StatusesCount = Integer.Parse(xdoc.SelectSingleNode(nd + "/statuses_count").InnerText)
            _info.Verified = Boolean.Parse(xdoc.SelectSingleNode(nd + "/verified").InnerText)

            ' 最終発言が取れないことがある
            Try
                If nd = "/user" Then
                    _info.RecentPost = xdoc.SelectSingleNode(nd + "/status/text").InnerText
                    _info.PostCreatedAt = DateTime.ParseExact(xdoc.SelectSingleNode(nd + "/status/created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
                    _info.PostSource = xdoc.SelectSingleNode(nd + "/status/source").InnerText
                Else
                    _info.RecentPost = xdoc.SelectSingleNode("/status/text").InnerText
                    _info.PostCreatedAt = DateTime.ParseExact(xdoc.SelectSingleNode("/status/created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
                    _info.PostSource = xdoc.SelectSingleNode("/status/source").InnerText
                End If
                If Not _info.PostSource.Contains("</a>") Then
                    _info.PostSource += "</a>"
                End If
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

    Private Sub SetLinklabelWeb(ByVal data As String)
        Dim webtext As String
        Dim jumpto As String
        webtext = MyOwner.TwitterInstance.PreProcessUrl("<a href=""" + data + """>Dummy</a>")
        webtext = ShortUrl.Resolve(webtext)
        jumpto = Regex.Match(webtext, "<a href=""(?<url>.*?)""").Groups.Item("url").Value
        ToolTip1.SetToolTip(LinkLabelWeb, jumpto)
        LinkLabelWeb.Tag = jumpto
        LinkLabelWeb.Text = data
    End Sub

    Private Function MakeDescriptionBrowserText(ByVal data As String) As String
        descriptionTxt = MyOwner.createDetailHtml( _
                                MyOwner.TwitterInstance.CreateHtmlAnchor(data, atlist))
        Return descriptionTxt
    End Function

    Private Sub ShowUserInfo_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        TweenMain.TopMost = Not TweenMain.TopMost
        TweenMain.TopMost = Not TweenMain.TopMost
    End Sub

    Private Sub ShowUserInfo_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        MyOwner = DirectCast(Me.Owner, TweenMain)
        If Not AnalizeUserInfo(userInfoXml) Then
            MessageBox.Show(My.Resources.ShowUserInfo1)
            Me.Close()
            Exit Sub
        End If

        'アイコンロード
        BackgroundWorkerImageLoader.RunWorkerAsync()

        InitPath()
        InitTooltip()
        Me.Text = Me.Text.Insert(0, _info.ScreenName + " ")
        LabelId.Text = _info.Id.ToString
        LabelScreenName.Text = _info.ScreenName
        LabelName.Text = _info.Name

        LabelLocation.Text = _info.Location

        SetLinklabelWeb(_info.Url)

        DescriptionBrowser.Visible = False
        MakeDescriptionBrowserText(_info.Description)

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

        If MyOwner.TwitterInstance.Username = _info.ScreenName Then
            ButtonEdit.Enabled = True
            ChangeIconToolStripMenuItem.Enabled = True
            ButtonBlock.Enabled = False
            ButtonReportSpam.Enabled = False
            ButtonBlockDestroy.Enabled = False
        Else
            ButtonEdit.Enabled = False
            ChangeIconToolStripMenuItem.Enabled = False
            ButtonBlock.Enabled = True
            ButtonReportSpam.Enabled = True
            ButtonBlockDestroy.Enabled = True
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
            MyOwner.OpenUriAsync(LinkLabelWeb.Text)
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
        If MessageBox.Show(_info.ScreenName + My.Resources.ButtonUnFollow_ClickText1, _
                           My.Resources.ButtonUnFollow_ClickText2, _
                           MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
            Dim ret As String = MyOwner.TwitterInstance.PostRemoveCommand(_info.ScreenName)
            If Not String.IsNullOrEmpty(ret) Then
                MessageBox.Show(My.Resources.FRMessage2 + ret)
            Else
                MessageBox.Show(My.Resources.FRMessage3)
                LabelIsFollowing.Text = My.Resources.GetFriendshipInfo2
                ButtonFollow.Enabled = True
                ButtonUnFollow.Enabled = False
            End If
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

    Private Sub BackgroundWorkerImageLoader_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorkerImageLoader.DoWork
        Dim name As String = _info.ImageUrl.ToString
        icondata = (New HttpVarious).GetImage(name.Replace("_normal", "_bigger"))
        If MyOwner.TwitterInstance.Username = _info.ScreenName Then Exit Sub

        _info.isFollowing = False
        _info.isFollowed = False
        FriendshipResult = MyOwner.TwitterInstance.GetFriendshipInfo(_info.ScreenName, _info.isFollowing, _info.isFollowed)
    End Sub

    Private Sub BackgroundWorkerImageLoader_RunWorkerCompleted(ByVal sender As System.Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles BackgroundWorkerImageLoader.RunWorkerCompleted
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
        Dim sc As WebBrowser = TryCast(ContextMenuRecentPostBrowser.SourceControl, WebBrowser)
        If sc IsNot Nothing Then
            sc.Document.ExecCommand("SelectAll", False, Nothing)
        End If
    End Sub

    Private Sub SelectionCopyToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SelectionCopyToolStripMenuItem.Click
        Dim sc As WebBrowser = TryCast(ContextMenuRecentPostBrowser.SourceControl, WebBrowser)
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

    Private Sub ContextMenuStrip1_Opening(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles ContextMenuRecentPostBrowser.Opening
        Dim sc As WebBrowser = TryCast(ContextMenuRecentPostBrowser.SourceControl, WebBrowser)
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
        MyOwner.OpenUriAsync("http://twitter.com/help/verified")
    End Sub

    Private Sub ButtonSearchPosts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonSearchPosts.Click
        MyOwner.AddNewTabForSearch("from:" + _info.ScreenName)
    End Sub

    Private Sub UserPicture_DoubleClick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserPicture.DoubleClick
        If UserPicture.Image IsNot Nothing Then
            Dim name As String = _info.ImageUrl.ToString
            MyOwner.OpenUriAsync(name.Remove(name.LastIndexOf("_normal"), 7))
        End If
    End Sub

    Private Sub UserPicture_MouseEnter(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserPicture.MouseEnter
        UserPicture.Cursor = Cursors.Hand
    End Sub

    Private Sub UserPicture_MouseLeave(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UserPicture.MouseLeave
        UserPicture.Cursor = Cursors.Default
    End Sub

    Private Class UpdateProfileArgs
        Public tw As Twitter
        Public name As String
        Public location As String
        Public url As String
        Public description As String
    End Class

    Private Sub UpdateProfile_Dowork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        Dim arg As UpdateProfileArgs = DirectCast(e.Argument, UpdateProfileArgs)
        e.Result = arg.tw.PostUpdateProfile(arg.name, _
                                            arg.url, _
                                            arg.location, _
                                            arg.description)
    End Sub

    Private Sub ButtonEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonEdit.Click
        Static IsEditing As Boolean = False
        Static ButtonEditText As String = ""

        ' 自分以外のプロフィールは変更できない
        If MyOwner.TwitterInstance.Username <> _info.ScreenName Then Exit Sub

        If Not IsEditing Then
            ButtonEditText = ButtonEdit.Text
            ButtonEdit.Text = My.Resources.UserInfoButtonEdit_ClickText1

            '座標初期化,プロパティ設定
            TextBoxName.Location = LabelName.Location
            TextBoxName.Height = LabelName.Height
            TextBoxName.Width = LabelName.Width
            TextBoxName.BackColor = MyOwner.InputBackColor
            TextBoxName.MaxLength = 20
            TextBoxName.Text = LabelName.Text
            TextBoxName.TabStop = True
            TextBoxName.Visible = True
            LabelName.Visible = False

            TextBoxLocation.Location = LabelLocation.Location
            TextBoxLocation.Height = LabelLocation.Height
            TextBoxLocation.Width = LabelLocation.Width
            TextBoxLocation.BackColor = MyOwner.InputBackColor
            TextBoxLocation.MaxLength = 30
            TextBoxLocation.Text = LabelLocation.Text
            TextBoxLocation.TabStop = True
            TextBoxLocation.Visible = True
            LabelLocation.Visible = False

            TextBoxWeb.Location = LinkLabelWeb.Location
            TextBoxWeb.Height = LinkLabelWeb.Height
            TextBoxWeb.Width = LinkLabelWeb.Width
            TextBoxWeb.BackColor = MyOwner.InputBackColor
            TextBoxWeb.MaxLength = 100
            TextBoxWeb.Text = _info.Url
            TextBoxWeb.TabStop = True
            TextBoxWeb.Visible = True
            LinkLabelWeb.Visible = False

            TextBoxDescription.Location = DescriptionBrowser.Location
            TextBoxDescription.Height = DescriptionBrowser.Height
            TextBoxDescription.Width = DescriptionBrowser.Width
            TextBoxDescription.BackColor = MyOwner.InputBackColor
            TextBoxDescription.MaxLength = 160
            TextBoxDescription.Text = _info.Description
            TextBoxDescription.Multiline = True
            TextBoxDescription.ScrollBars = ScrollBars.Vertical
            TextBoxDescription.TabStop = True
            TextBoxDescription.Visible = True
            DescriptionBrowser.Visible = False

            TextBoxName.Focus()
            TextBoxName.Select(TextBoxName.Text.Length, 0)

            IsEditing = True
        Else
            Dim arg As New UpdateProfileArgs

            If TextBoxName.Modified OrElse _
                TextBoxLocation.Modified OrElse _
                TextBoxWeb.Modified OrElse _
                TextBoxDescription.Modified Then

                arg.tw = MyOwner.TwitterInstance
                arg.name = TextBoxName.Text
                arg.url = TextBoxWeb.Text
                arg.location = TextBoxLocation.Text
                arg.description = TextBoxDescription.Text

                Using dlg As New FormInfo(Me, My.Resources.UserInfoButtonEdit_ClickText2, _
                                            AddressOf UpdateProfile_Dowork, _
                                            Nothing, _
                                            arg)
                    dlg.ShowDialog()
                    If Not String.IsNullOrEmpty(dlg.Result.ToString) Then
                        Exit Sub
                    End If
                End Using
            End If


            LabelName.Text = TextBoxName.Text
            _info.Name = LabelName.Text
            TextBoxName.TabStop = False
            TextBoxName.Visible = False
            LabelName.Visible = True

            LabelLocation.Text = TextBoxLocation.Text
            _info.Location = LabelLocation.Text
            TextBoxLocation.TabStop = False
            TextBoxLocation.Visible = False
            LabelLocation.Visible = True

            SetLinklabelWeb(TextBoxWeb.Text)
            _info.Url = TextBoxWeb.Text
            TextBoxWeb.TabStop = False
            TextBoxWeb.Visible = False
            LinkLabelWeb.Visible = True

            DescriptionBrowser.DocumentText = MakeDescriptionBrowserText(TextBoxDescription.Text)
            _info.Description = TextBoxDescription.Text
            TextBoxDescription.TabStop = False
            TextBoxDescription.Visible = False
            DescriptionBrowser.Visible = True

            ButtonEdit.Text = ButtonEditText

            IsEditing = False
        End If

    End Sub

    Class UpdateProfileImageArgs
        Public tw As Twitter
        Public FileName As String
    End Class

    Private Sub UpdateProfileImage_Dowork(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        Dim arg As UpdateProfileImageArgs = DirectCast(e.Argument, UpdateProfileImageArgs)
        e.Result = arg.tw.PostUpdateProfileImage(arg.FileName)
    End Sub

    Private Sub UpdateProfileImage_RunWorkerCompleted(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs)
        Dim res As String = ""
        Dim xdocbuf As String = ""

        If e.Result Is Nothing Then
            Exit Sub
        End If


        ' アイコンを取得してみる
        ' が、古いアイコンのユーザーデータが返ってくるため反映/判断できない

        res = MyOwner.TwitterInstance.GetUserInfo(_info.ScreenName, xdocbuf)

        Dim xdoc As New XmlDocument
        Dim img As Image
        Try
            xdoc.LoadXml(xdocbuf)
            _info.ImageUrl = New Uri(xdoc.SelectSingleNode("/user/profile_image_url").InnerText)
            img = (New HttpVarious).GetImage(_info.ImageUrl.ToString)
            If img IsNot Nothing Then
                UserPicture.Image = img
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub doChangeIcon(ByVal filename As String)
        Dim res As String = ""
        Dim arg As New UpdateProfileImageArgs With {.tw = MyOwner.TwitterInstance, .FileName = filename}

        Using dlg As New FormInfo(Me, My.Resources.ChangeIconToolStripMenuItem_ClickText3, _
                                  AddressOf UpdateProfileImage_Dowork, _
                                  AddressOf UpdateProfileImage_RunWorkerCompleted,
                                  arg)
            dlg.ShowDialog()
            res = TryCast(dlg.Result, String)
            If Not String.IsNullOrEmpty(res) Then
                ' "Err:"が付いたエラーメッセージが返ってくる
                MessageBox.Show(res + vbCrLf + My.Resources.ChangeIconToolStripMenuItem_ClickText4)
            Else
                MessageBox.Show(My.Resources.ChangeIconToolStripMenuItem_ClickText5)
            End If
        End Using
    End Sub


    Private Sub ChangeIconToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChangeIconToolStripMenuItem.Click
        OpenFileDialogIcon.Filter = My.Resources.ChangeIconToolStripMenuItem_ClickText1
        OpenFileDialogIcon.Title = My.Resources.ChangeIconToolStripMenuItem_ClickText2
        OpenFileDialogIcon.FileName = ""

        Dim rslt As Windows.Forms.DialogResult = OpenFileDialogIcon.ShowDialog

        If rslt <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If

        Dim fn As String = OpenFileDialogIcon.FileName
        If isValidIconFile(New FileInfo(fn)) Then
            doChangeIcon(fn)
        Else
            MessageBox.Show("ユーザーアイコンとして使用できないファイルです")
        End If
    End Sub

    Private Sub ButtonBlock_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonBlock.Click
        If MessageBox.Show(_info.ScreenName + My.Resources.ButtonBlock_ClickText1, _
                           My.Resources.ButtonBlock_ClickText2, _
                           MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
            Dim res As String = MyOwner.TwitterInstance.PostCreateBlock(_info.ScreenName)
            If Not String.IsNullOrEmpty(res) Then
                MessageBox.Show(res + Environment.NewLine + My.Resources.ButtonBlock_ClickText3)
            Else
                MessageBox.Show(My.Resources.ButtonBlock_ClickText4)
            End If
        End If
    End Sub

    Private Sub ButtonReportSpam_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonReportSpam.Click
        If MessageBox.Show(_info.ScreenName + My.Resources.ButtonReportSpam_ClickText1, _
                           My.Resources.ButtonReportSpam_ClickText2, _
                           MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
            Dim res As String = MyOwner.TwitterInstance.PostReportSpam(_info.ScreenName)
            If Not String.IsNullOrEmpty(res) Then
                MessageBox.Show(res + Environment.NewLine + My.Resources.ButtonReportSpam_ClickText3)
            Else
                MessageBox.Show(My.Resources.ButtonReportSpam_ClickText4)
            End If
        End If
    End Sub

    Private Sub ButtonBlockDestroy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonBlockDestroy.Click
        If MessageBox.Show(_info.ScreenName + My.Resources.ButtonBlockDestroy_ClickText1, _
                           My.Resources.ButtonBlockDestroy_ClickText2, _
                           MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) = Windows.Forms.DialogResult.Yes Then
            Dim res As String = MyOwner.TwitterInstance.PostDestroyBlock(_info.ScreenName)
            If Not String.IsNullOrEmpty(res) Then
                MessageBox.Show(res + Environment.NewLine + My.Resources.ButtonBlockDestroy_ClickText3)
            Else
                MessageBox.Show(My.Resources.ButtonBlockDestroy_ClickText4)
            End If
        End If
    End Sub

    Private Function isValidExtension(ByVal ext As String) As Boolean
        Return ext.Equals(".jpg") OrElse ext.Equals(".jpeg") OrElse ext.Equals(".png") OrElse ext.Equals(".gif")
    End Function

    Private Function isValidIconFile(ByVal info As FileInfo) As Boolean
        Dim ext As String = info.Extension.ToLower
        Return isValidExtension(ext) AndAlso info.Length < 700 * 1024 AndAlso Not IsAnimatedGif(info.FullName)
    End Function

    Private Sub ShowUserInfo_DragOver(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragOver
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim filename As String = CType(e.Data.GetData(DataFormats.FileDrop, False), String())(0)
            Dim fl As New FileInfo(filename)

            e.Effect = DragDropEffects.None
            If isValidIconFile(fl) Then
                e.Effect = DragDropEffects.Copy
            End If
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub ShowUserInfo_DragDrop(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles MyBase.DragDrop
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            Dim filename As String = CType(e.Data.GetData(DataFormats.FileDrop, False), String())(0)
            doChangeIcon(filename)
        End If
    End Sub
End Class