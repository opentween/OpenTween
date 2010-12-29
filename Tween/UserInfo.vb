Public Class UserInfo
    Public Sub New()

    End Sub

    Public Sub New(ByVal user As TwitterDataModel.User)
        Me.Id = user.Id
        Me.Name = user.Name
        Me.ScreenName = user.ScreenName
        Me.Location = user.Location
        Me.Description = user.Description
        Try
            Me.ImageUrl = New Uri(user.ProfileImageUrl)
        Catch ex As Exception
            Me.ImageUrl = Nothing
        End Try
        Me.Url = user.Url
        Me.Protect = user.Protected
        Me.FriendsCount = user.FriendsCount
        Me.FollowersCount = user.FollowersCount
        Me.CreatedAt = DateTimeParse(user.CreatedAt)
        Me.StatusesCount = user.StatusesCount
        Me.Verified = user.Verified
        Me.isFollowing = Me.isFollowing
        If user.Status IsNot Nothing Then
            Me.RecentPost = user.Status.Text
            Me.PostCreatedAt = DateTimeParse(user.Status.CreatedAt)
            Me.PostSource = user.Status.Source
        End If
    End Sub

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

    Public Overrides Function ToString() As String
        Return Me.ScreenName + " / " + Me.Name
    End Function
End Class