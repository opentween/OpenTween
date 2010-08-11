Public Class UserInfo
    Public Sub New()

    End Sub

    Public Sub New(ByVal xmlNode As Xml.XmlNode)
        Me.Id = Long.Parse(xmlNode.Item("id").InnerText)
        Me.Name = xmlNode.Item("name").InnerText
        Me.ScreenName = xmlNode.Item("screen_name").InnerText
        Me.Location = xmlNode.Item("location").InnerText
        Me.Description = xmlNode.Item("description").InnerText
        Me.ImageUrl = New Uri(xmlNode.Item("profile_image_url").InnerText)
        Me.Url = xmlNode.Item("url").InnerText
        Me.Protect = Boolean.Parse(xmlNode.Item("protected").InnerText)
        'Me.FriendsCount = xmlNode.Item("").InnerText
        Me.FollowersCount = Integer.Parse(xmlNode.Item("followers_count").InnerText)
        Me.CreatedAt = DateTime.ParseExact(xmlNode.Item("created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
        Me.StatusesCount = Integer.Parse(xmlNode.Item("statuses_count").InnerText)
        Me.Verified = Boolean.Parse(xmlNode.Item("verified").InnerText)
        Me.isFollowing = Boolean.Parse(xmlNode.Item("following").InnerText)
        Dim postNode As Xml.XmlNode = xmlNode.Item("status")
        Me.RecentPost = postNode.Item("text").InnerText
        Me.PostCreatedAt = DateTime.ParseExact(postNode.Item("created_at").InnerText, "ddd MMM dd HH:mm:ss zzzz yyyy", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None)
        Me.PostSource = postNode.Item("source").InnerText
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