Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization

Public Class TwitterDataModel

    <DataContract()> _
    Public Class Urls
        <DataMember(Name:="urls")> Public Urls As String
        <DataMember(Name:="indices")> Public Indices(2) As Integer
    End Class

    <DataContract()> _
    Public Class Hashtags
        <DataMember(Name:="indices")> Public Indices(2) As Integer
        <DataMember(Name:="text")> Public Text As String
    End Class

    <DataContract()> _
    Public Class UserMentions
        <DataMember(Name:="indices")> Public Indices(2) As Integer
        <DataMember(Name:="screen_name")> Public ScreenName As String
        <DataMember(Name:="name")> Public Name As String
        <DataMember(Name:="id")> Public Id As Int64
    End Class

    <DataContract()> _
    Public Class Entities
        <DataMember(Name:="urls")> Public Urls() As Urls
        <DataMember(Name:="hashtags")> Public Hashtags() As Hashtags
        <DataMember(Name:="user_mentions")> Public UserMentions() As UserMentions
    End Class

    <DataContract()> _
    Public Class User
        <DataMember(Name:="statuses_count")> Public StatusesCount As Int64
        <DataMember(Name:="profile_sidebar_fill_color")> Public ProfileSidebarFillColor As String
        <DataMember(Name:="show_all_inline_media")> Public ShowAllInlineMedia As Boolean
        <DataMember(Name:="profile_use_background_image")> Public ProfileUseBackgroundImage As Boolean
        <DataMember(Name:="contributors_enabled")> Public ContributorsEnabled As Boolean
        <DataMember(Name:="profile_sidebar_border_color")> Public ProfileSidebarBorderColor As String
        <DataMember(Name:="location")> Public Location As String
        <DataMember(Name:="geo_enabled")> Public GeoEnabled As Boolean
        <DataMember(Name:="description")> Public Description As String
        <DataMember(Name:="friends_count")> Public FriendsCount As Integer
        <DataMember(Name:="verified")> Public Verified As Boolean
        <DataMember(Name:="favourites_count")> Public FavouritesCount As Integer
        <DataMember(Name:="created_at")> Public CreatedAt As String
        <DataMember(Name:="profile_background_color")> Public ProfileBackgroundColor As String
        <DataMember(Name:="follow_request_sent")> Public FollowRequestSent As String
        <DataMember(Name:="time_zone")> Public TimeZone As String
        <DataMember(Name:="followers_count")> Public FollowersCount As Integer
        <DataMember(Name:="url")> Public Url As String
        <DataMember(Name:="profile_image_url")> Public ProfileImageUrl As String
        <DataMember(Name:="notifications")> Public Notifications As String
        <DataMember(Name:="profile_text_color")> Public ProfileTextColor As String
        <DataMember(Name:="protected")> Public [Protected] As Boolean
        <DataMember(Name:="id_str")> Public IdStr As String
        <DataMember(Name:="lang")> Public Lang As String
        <DataMember(Name:="profile_background_image_url")> Public ProfileBackgroundImageUrl As String
        <DataMember(Name:="screen_name")> Public ScreenName As String
        <DataMember(Name:="name")> Public Name As String
        <DataMember(Name:="following")> Public Following As String
        <DataMember(Name:="profile_link_color")> Public ProfileLinkColor As String
        <DataMember(Name:="id")> Public Id As Int64
        <DataMember(Name:="listed_count")> Public ListedCount As Integer
        <DataMember(Name:="profile_background_tile")> Public ProfileBackgroundTile As Boolean
        <DataMember(Name:="utc_offset")> Public UtcOffset As String
        <DataMember(Name:="place", IsRequired:=False)> Public Place As Place
    End Class

    <DataContract()> _
    Public Class Coordinates
        <DataMember(Name:="type", IsRequired:=False)> Public Type As String
        <DataMember(Name:="coordinates", IsRequired:=False)> Public Coordinates(2) As Double
    End Class

    <DataContract()> _
    Public Class Geo
        <DataMember(Name:="type", IsRequired:=False)> Public Type As String
        <DataMember(Name:="coordinates", IsRequired:=False)> Public Coordinates(2) As Double
    End Class

    <DataContract()> _
    Public Class BoundingBox
        <DataMember(Name:="type", IsRequired:=False)> Public Type As String
        <DataMember(Name:="coordinates", IsRequired:=False)> Public Coordinates As Double()()()
    End Class

    <DataContract()> _
    Public Class Attributes
        <DataMember(Name:="street_address", IsRequired:=False)> Public StreetAddress As String
    End Class

    <DataContract()> _
    Public Class Place
        <DataMember(Name:="url")> Public Url As String
        <DataMember(Name:="bounding_box", IsRequired:=False)> Public BoundingBox As BoundingBox
        <DataMember(Name:="street_address", IsRequired:=False)> Public StreetAddress As String
        <DataMember(Name:="full_name")> Public FullName As String
        <DataMember(Name:="name")> Public Name As String
        '<DataMember(Name:="attributes", IsRequired:=False)> Public attributes As attributes
        <DataMember(Name:="country_code", IsRequired:=False)> Public CountryCode As String
        <DataMember(Name:="id")> Public Id As String
        <DataMember(Name:="country")> Public Country As String
        <DataMember(Name:="place_type")> Public PlaceType As String
    End Class

    <DataContract()> _
    Public Class RetweetedStatus
        <DataMember(Name:="coordinates", IsRequired:=False)> Public Coordinates As Coordinates
        <DataMember(Name:="geo", IsRequired:=False)> Public Geo As Geo
        <DataMember(Name:="in_reply_to_user_id")> Public InReplyToUserId As String
        <DataMember(Name:="source")> Public Source As String
        <DataMember(Name:="user")> Public User As User
        <DataMember(Name:="in_reply_to_screen_name")> Public InReplyToScreenName As String
        <DataMember(Name:="created_at")> Public CreatedAt As String
        <DataMember(Name:="contributors")> Public Contributors As Integer()
        <DataMember(Name:="favorited")> Public Favorited As Boolean
        <DataMember(Name:="truncated")> Public Truncated As Boolean
        <DataMember(Name:="id")> Public Id As Int64
        <DataMember(Name:="annotations", IsRequired:=False)> Public Annotations As String
        <DataMember(Name:="place", IsRequired:=False)> Public Place As Place
        <DataMember(Name:="in_reply_to_status_id")> Public InReplyToStatusId As String
        <DataMember(Name:="text")> Public Text As String
    End Class

    <DataContract()> _
    Public Class Status
        <DataMember(Name:="in_reply_to_status_id_str")> Public InReplyToStatusIdStr As String
        <DataMember(Name:="contributors", IsRequired:=False)> Public Contributors As Integer()
        <DataMember(Name:="in_reply_to_screen_name")> Public InReplyToScreenName As String
        <DataMember(Name:="in_reply_to_status_id")> Public InReplyToStatusId As String
        <DataMember(Name:="in_reply_to_user_id_str")> Public InReplyToUserIdStr As String
        <DataMember(Name:="retweet_count")> Public RetweetCount As String
        <DataMember(Name:="created_at")> Public CreatedAt As String
        <DataMember(Name:="geo", IsRequired:=False)> Public Geo As Geo
        <DataMember(Name:="retweeted")> Public Retweeted As Boolean
        <DataMember(Name:="in_reply_to_user_id")> Public InReplyToUserId As String
        <DataMember(Name:="source")> Public Source As String
        <DataMember(Name:="id_str")> Public IdStr As String
        <DataMember(Name:="coordinates", IsRequired:=False)> Public Coordinates As Coordinates
        <DataMember(Name:="truncated")> Public Truncated As Boolean
        <DataMember(Name:="place", IsRequired:=False)> Public Place As Place
        <DataMember(Name:="user")> Public User As User
        <DataMember(Name:="retweeted_status", IsRequired:=False)> Public RetweetedStatus As RetweetedStatus
        <DataMember(Name:="id")> Public Id As Int64
        <DataMember(Name:="favorited")> Public Favorited As Boolean
        <DataMember(Name:="text")> Public Text As String
    End Class

    <DataContract()> _
    Public Class Directmessage
        <DataMember(Name:="created_at")> Public CreatedAt As String
        <DataMember(Name:="sender_id")> Public SenderId As Int64
        <DataMember(Name:="sender_screen_name")> Public SenderScreenName As String
        <DataMember(Name:="sender")> Public Sender As User
        <DataMember(Name:="id_str")> Public IdStr As String
        <DataMember(Name:="recipient")> Public Recipient As User
        <DataMember(Name:="recipient_screen_name")> Public RecipientScreenName As String
        <DataMember(Name:="recipient_id")> Public RecipientId As Int64
        <DataMember(Name:="id")> Public Id As Int64
        <DataMember(Name:="text")> Public Text As String
    End Class

    <DataContract()> _
    Public Class Friendsevent
        <DataMember(Name:="friends")> Public Friends As Int64()
    End Class

    <DataContract()> _
    Public Class DeletedStatusContent
        <DataMember(Name:="id")> Public Id As Int64
        <DataMember(Name:="user_id")> Public UserId As Int64
    End Class

    <DataContract()> _
    Public Class DeletedStatus
        <DataMember(Name:="status")> Public Status As DeletedStatusContent
    End Class

    <DataContract()> _
    Public Class DeleteEvent
        <DataMember(Name:="delete")> Public [Event] As DeletedStatus
    End Class

    <DataContract()> _
    Public Class DeletedDirectmessage
        <DataMember(Name:="direct_message")> Public Directmessage As DeletedStatusContent
    End Class

    <DataContract()> _
    Public Class DeleteDirectmessageEvent
        <DataMember(Name:="delete")> Public [Event] As DeletedDirectmessage
    End Class
    <DataContract()> _
    Public Class DirectmessageEvent
        <DataMember(Name:="direct_message")> Public Directmessage As Directmessage
    End Class

    <DataContract()> _
    Public Class TrackCount
        <DataMember(Name:="track")> Public Track As Integer
    End Class

    <DataContract()> _
    Public Class LimitEvent
        <DataMember(Name:="limit")> Public Limit As TrackCount
    End Class

    <DataContract()> _
    Public Class EventData
        <DataMember(Name:="target")> Public Target As User
        <DataMember(Name:="target_object", isRequired:=False)> Public TargetObject As Status
        <DataMember(Name:="created_at")> Public CreatedAt As String
        <DataMember(Name:="event")> Public [Event] As String
        <DataMember(Name:="source")> Public Source As User
    End Class
End Class
