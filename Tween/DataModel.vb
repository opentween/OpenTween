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

Imports System.Runtime.InteropServices
Imports System.Runtime.Serialization

Public Class TwitterDataModel

    <DataContract()> _
    Public Class Annotations
        <DataMember(Name:="ConversationRole", IsRequired:=False)> Public ConversationRole As String
        <DataMember(Name:="FromUser", IsRequired:=False)> Public FromUser As String
    End Class

    <DataContract()> _
    Public Class SizeElement
        <DataMember(Name:="w")> Public w As Integer
        <DataMember(Name:="h")> Public h As Integer
        <DataMember(Name:="resize")> Public Resize As String
    End Class

    <DataContract()> _
    Public Class Sizes
        <DataMember(Name:="large")> Public Large As SizeElement
        <DataMember(Name:="medium")> Public Medium As SizeElement
        <DataMember(Name:="small")> Public Small As SizeElement
        <DataMember(Name:="thumb")> Public Thumb As SizeElement
    End Class

    <DataContract()> _
    Public Class Media
        <DataMember(Name:="id")> Public Id As Long
        <DataMember(Name:="media_url")> Public MediaUrl As String
        <DataMember(Name:="media_url_https")> Public MediaUrlHttps As String
        <DataMember(Name:="url")> Public Url As String
        <DataMember(Name:="display_url")> Public DisplayUrl As String
        <DataMember(Name:="expanded_url")> Public ExpandedUrl As String
        <DataMember(Name:="sizes")> Public Sizes As Sizes
        <DataMember(Name:="type")> Public Type As String
        <DataMember(Name:="indices")> Public Indices(2) As Integer
    End Class

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
        <DataMember(Name:="media", isRequired:=False)> Public Media() As Media
    End Class

    <DataContract()> _
    Public Class User
        <DataMember(Name:="statuses_count")> Public StatusesCount As Integer
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
        <DataMember(Name:="status", IsRequired:=False)> Public Status As Status
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
        <DataMember(Name:="annotations", IsRequired:=False)> Public Annotations As Annotations
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
        <DataMember(Name:="entities", isRequired:=False)> Public Entities As Entities
    End Class

    <DataContract()> _
    Public Class TargetObject
        Inherits Status
        <DataMember(Name:="mode")> Public Mode As String
        <DataMember(Name:="description")> Public Description As String
        <DataMember(Name:="slug")> Public Slug As String
        <DataMember(Name:="uri")> Public Uri As String
        <DataMember(Name:="member_count")> Public MemberCount As Integer
        <DataMember(Name:="full_name")> Public FullName As String
        <DataMember(Name:="subscriber_count")> Public SubscriberCount As Integer
        <DataMember(Name:="name")> Public Name As String
        <DataMember(Name:="following")> Public Following As Boolean
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
        <DataMember(Name:="target_object", isRequired:=False)> Public TargetObject As TargetObject
        <DataMember(Name:="created_at")> Public CreatedAt As String
        <DataMember(Name:="event")> Public [Event] As String
        <DataMember(Name:="source")> Public Source As User
    End Class

    <DataContract()> _
    Public Class RelatedTweet
        <DataMember(Name:="annotations")> Public Annotations As Annotations
        <DataMember(Name:="kind")> Public Kind As String
        <DataMember(Name:="score")> Public Score As Double
        <DataMember(Name:="value")> Public Status As Status
    End Class

    <DataContract()> _
    Public Class RelatedResult
        <DataMember(Name:="annotations")> Public Annotations As Annotations
        <DataMember(Name:="groupName")> Public GroupName As String
        <DataMember(Name:="resultType")> Public ResultType As String
        <DataMember(Name:="results")> Public Results As RelatedTweet()
        <DataMember(Name:="score")> Public Score As Double
    End Class

    <DataContract()> _
    Public Class RelationshipResult
        <DataMember(Name:="followed_by")> Public FollowedBy As Boolean
        <DataMember(Name:="following")> Public Following As Boolean
    End Class

    <DataContract()> _
    Public Class RelationshipUsers
        <DataMember(Name:="target")> Public Target As RelationshipResult
        <DataMember(Name:="source")> Public Source As RelationshipResult
    End Class

    <DataContract()> _
    Public Class Relationship
        <DataMember(Name:="relationship")> Public Relationship As RelationshipUsers
    End Class

    <DataContract()> _
    Public Class Ids
        <DataMember(Name:="ids")> Public Id As Long()
        <DataMember(Name:="next_cursor")> Public NextCursor As Long
        <DataMember(Name:="previous_cursor")> Public PreviousCursor As Long
    End Class

    <DataContract()> _
    Public Class RateLimitStatus
        <DataMember(Name:="reset_time_in_seconds")> Public RestTimeInSeconds As Integer
        <DataMember(Name:="remaining_hits")> Public RemainingHits As Integer
        <DataMember(Name:="reset_time")> Public RestTime As String
        <DataMember(Name:="hourly_limit")> Public HourlyLimit As Integer
    End Class

    <DataContract()> _
    Public Class ListElementData
        <DataMember(Name:="mode")> Public Mode As String
        <DataMember(Name:="uri")> Public Uri As String
        <DataMember(Name:="member_count")> Public MemberCount As Integer
        <DataMember(Name:="slug")> Public Slug As String
        <DataMember(Name:="full_name")> Public FullName As String
        <DataMember(Name:="user")> Public User As User
        <DataMember(Name:="following")> Public Following As Boolean
        <DataMember(Name:="subscriber_count")> Public SubscriberCount As Integer
        <DataMember(Name:="description")> Public Description As String
        <DataMember(Name:="name")> Public Name As String
        <DataMember(Name:="id")> Public Id As Long
    End Class

    <DataContract()> _
    Public Class Lists
        <DataMember(Name:="lists")> Public Lists As ListElementData()
        <DataMember(Name:="next_cursor")> Public NextCursor As Long
        <DataMember(Name:="previous_cursor")> Public PreviousCursor As Long
    End Class

    <DataContract()> _
    Public Class Users
        <DataMember(Name:="users")> Public users As User()
        <DataMember(Name:="next_cursor")> Public NextCursor As Long
        <DataMember(Name:="previous_cursor")> Public PreviousCursor As Long
    End Class

    <DataContract()> _
    Public Class ErrorResponse
        <DataMember(Name:="request")> Public Request As String
        <DataMember(Name:="error")> Public ErrMsg As String
    End Class

    <DataContract()> _
    Public Class SearchResult
        <DataMember(Name:="statuses")> Public Statuses As List(Of Status)
        <DataMember(Name:="next_page")> Public NextPage As String
        <DataMember(Name:="error")> Public ErrMsg As String
    End Class
End Class
