Imports System.Xml

Public Class ShowUserInfo

    Private userInfoXml As String = ""
    Private _info As UserInfo

    Private Structure UserInfo
        Dim Name As String
        Dim ScreenName As String
        Dim Location As String
        Dim Description As String
        Dim ImageUrl As Uri
        Dim Url As String
        Dim Protect As Boolean
        Dim FriendsCount As Integer
        Dim FollowersCount As Integer
        Dim FavoriteCount As Integer
        Dim CreatedAt As DateTime
        Dim StatusesCount As Integer
        Dim Verified As Boolean
    End Structure


    Private Function AnalizeUserInfo(ByVal xmlData As String) As Boolean
        If xmlData Is Nothing Then Return False
        Dim xdoc As New XmlDocument
        Try
            xdoc.LoadXml(xmlData)
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
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Private Sub ShowUserInfo_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        If Not AnalizeUserInfo(userInfoXml) Then
            MessageBox.Show(My.Resources.ShowUserInfo1)
            Me.Close()
            Return
        Else
            Me.Text = Me.Text.Insert(0, _info.ScreenName + " ")
            LabelName.Text = _info.Name
            LabelLocation.Text = _info.Location
            LinkLabelWeb.Text = _info.Url

            LabelDescription.Text = _info.Description

            LinkLabelFollowing.Text = _info.FriendsCount.ToString
            LinkLabelFollowers.Text = _info.FollowersCount.ToString
            LinkLabelFav.Text = _info.FavoriteCount.ToString
            LinkLabelTweet.Text = _info.StatusesCount.ToString

            LabelCreatedAt.Text = _info.CreatedAt.ToString
            LabelIsProtected.Text = DirectCast(IIf(_info.Protect, My.Resources.Yes, My.Resources.No), String)

            Dim isFollowing As Boolean = False
            Dim isFollowed As Boolean = False
            Dim ret As String = TweenMain.TwitterInstance.GetFriendshipInfo(_info.ScreenName, isFollowing, isFollowed)
            If ret = "" Then
                If isFollowing Then
                    LabelIsFollowing.Text = My.Resources.GetFriendshipInfo1
                Else
                    LabelIsFollowing.Text = My.Resources.GetFriendshipInfo2
                End If
                ButtonFollow.Enabled = Not isFollowed
                If isFollowed Then
                    LabelIsFollowed.Text = My.Resources.GetFriendshipInfo3
                Else
                    LabelIsFollowed.Text = My.Resources.GetFriendshipInfo4
                End If
                ButtonUnFollow.Enabled = Not isFollowed
            Else
                MessageBox.Show(ret)
            End If
        End If
    End Sub

    Private Sub ButtonOK_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonClose.Click
        Me.Close()
    End Sub

    Public WriteOnly Property XmlData() As String
        Set(ByVal value As String)
            userInfoXml = value
        End Set
    End Property

    Private Sub LinkLabelWeb_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabelWeb.LinkClicked
        If _info.Url IsNot Nothing Then
            TweenMain.OpenUriAsync(_info.Url)
        End If
    End Sub

    Private Sub LinkLabelFollowing_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabelFollowing.LinkClicked
        TweenMain.OpenUriAsync("http://twitter.com/" + _info.ScreenName + "/following")
    End Sub

    Private Sub LinkLabelFollowers_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabelFollowers.LinkClicked
        TweenMain.OpenUriAsync("http://twitter.com/" + _info.ScreenName + "/followers")
    End Sub

    Private Sub LinkLabelTweet_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabelTweet.LinkClicked
        TweenMain.OpenUriAsync("http://twitter.com/" + _info.ScreenName)
    End Sub

    Private Sub LinkLabelFav_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabelFav.LinkClicked
        TweenMain.OpenUriAsync("http://twitter.com/" + _info.ScreenName + "/favorites")
    End Sub

    Private Sub ButtonFollow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonFollow.Click
        Dim ret As String = TweenMain.TwitterInstance.PostFollowCommand(_info.ScreenName)
        If Not String.IsNullOrEmpty(ret) Then
            MessageBox.Show(My.Resources.FRMessage2 + ret)
        Else
            MessageBox.Show(My.Resources.FRMessage3)
        End If
    End Sub

    Private Sub ButtonUnFollow_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonUnFollow.Click
        Dim ret As String = TweenMain.TwitterInstance.PostRemoveCommand(_info.ScreenName)
        If Not String.IsNullOrEmpty(ret) Then
            MessageBox.Show(My.Resources.FRMessage2 + ret)
        Else
            MessageBox.Show(My.Resources.FRMessage3)
        End If
    End Sub
End Class