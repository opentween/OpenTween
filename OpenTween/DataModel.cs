// OpenTween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
//           (c) 2011      Egtra (@egtra) <http://dev.activebasic.com/egtra/>
// All rights reserved.
//
// This file is part of OpenTween.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace OpenTween
{
    public class TwitterDataModel
    {
        [DataContract]
        public class Annotations
        {
            [DataMember(Name = "ConversationRole", IsRequired = false)] public string ConversationRole;
            [DataMember(Name = "FromUser", IsRequired = false)] public string FromUser;
        }

        [DataContract]
        public class SizeElement
        {
            [DataMember(Name = "w")] public int w;
            [DataMember(Name = "h")] public int h;
            [DataMember(Name = "resize")] public string Resize;
        }

        [DataContract]
        public class Sizes
        {
            [DataMember(Name = "large")] public SizeElement Large;
            [DataMember(Name = "medium")] public SizeElement Medium;
            [DataMember(Name = "small")] public SizeElement Small;
            [DataMember(Name = "thumb")] public SizeElement Thumb;
        }

        [DataContract]
        public class Media
        {
            [DataMember(Name = "id")] public long Id;
            [DataMember(Name = "media_url")] public string MediaUrl;
            [DataMember(Name = "media_url_https")] public string MediaUrlHttps;
            [DataMember(Name = "url")] public string Url;
            [DataMember(Name = "display_url")] public string DisplayUrl;
            [DataMember(Name = "expanded_url")] public string ExpandedUrl;
            [DataMember(Name = "sizes")] public Sizes Sizes;
            [DataMember(Name = "type")] public string Type;
            [DataMember(Name = "indices")] public int[] Indices = new int[3];
        }

        [DataContract]
        public class Urls
        {
            [DataMember(Name = "url")] public string Url;
            [DataMember(Name = "display_url")] public string DisplayUrl;
            [DataMember(Name = "expanded_url")] public string ExpandedUrl;
            [DataMember(Name = "indices")] public int[] Indices = new int[3];
        }

        [DataContract]
        public class Hashtags
        {
            [DataMember(Name = "indices")] public int[] Indices = new int[3];
            [DataMember(Name = "text")] public string Text;
        }

        [DataContract]
        public class UserMentions
        {
            [DataMember(Name = "indices")] public int[] Indices = new int[3];
            [DataMember(Name = "screen_name")] public string ScreenName;
            [DataMember(Name = "name")] public string Name;
            [DataMember(Name = "id")] public Int64 Id;
        }

        [DataContract]
        public class Entities
        {
            [DataMember(Name = "urls")] public Urls[] Urls;
            [DataMember(Name = "hashtags")] public Hashtags[] Hashtags;
            [DataMember(Name = "user_mentions")] public UserMentions[] UserMentions;
            [DataMember(Name = "media", IsRequired = false)] public Media[] Media;
        }

        [DataContract]
        public class User
        {
            [DataMember(Name = "statuses_count")] public int StatusesCount;
            [DataMember(Name = "profile_sidebar_fill_color")] public string ProfileSidebarFillColor;
            [DataMember(Name = "show_all_inline_media")] public bool ShowAllInlineMedia;
            [DataMember(Name = "profile_use_background_image")] public bool ProfileUseBackgroundImage;
            [DataMember(Name = "contributors_enabled")] public bool ContributorsEnabled;
            [DataMember(Name = "profile_sidebar_border_color")] public string ProfileSidebarBorderColor;
            [DataMember(Name = "location")] public string Location;
            [DataMember(Name = "geo_enabled")] public bool GeoEnabled;
            [DataMember(Name = "description")] public string Description;
            [DataMember(Name = "friends_count")] public int FriendsCount;
            [DataMember(Name = "verified")] public bool Verified;
            [DataMember(Name = "favourites_count")] public int FavouritesCount;
            [DataMember(Name = "created_at")] public string CreatedAt;
            [DataMember(Name = "profile_background_color")] public string ProfileBackgroundColor;
            [DataMember(Name = "follow_request_sent")] public string FollowRequestSent;
            [DataMember(Name = "time_zone")] public string TimeZone;
            [DataMember(Name = "followers_count")] public int FollowersCount;
            [DataMember(Name = "url")] public string Url;
            [DataMember(Name = "profile_image_url")] public string ProfileImageUrl;
            [DataMember(Name = "notifications")] public string Notifications;
            [DataMember(Name = "profile_text_color")] public string ProfileTextColor;
            [DataMember(Name = "protected")] public bool Protected;
            [DataMember(Name = "id_str")] public string IdStr;
            [DataMember(Name = "lang")] public string Lang;
            [DataMember(Name = "profile_background_image_url")] public string ProfileBackgroundImageUrl;
            [DataMember(Name = "screen_name")] public string ScreenName;
            [DataMember(Name = "name")] public string Name;
            [DataMember(Name = "following")] public string Following;
            [DataMember(Name = "profile_link_color")] public string ProfileLinkColor;
            [DataMember(Name = "id")] public Int64 Id;
            [DataMember(Name = "listed_count")] public int ListedCount;
            [DataMember(Name = "profile_background_tile")] public bool ProfileBackgroundTile;
            [DataMember(Name = "utc_offset")] public string UtcOffset;
            [DataMember(Name = "place", IsRequired = false)] public Place Place;
            [DataMember(Name = "status", IsRequired = false)] public Status Status;
        }

        [DataContract]
        public class Coordinates
        {
            [DataMember(Name = "type", IsRequired = false)] public string Type;
            [DataMember(Name = "coordinates", IsRequired = false)] public double[] coordinates = new double[3];
        }

        [DataContract]
        public class Geo
        {
            [DataMember(Name = "type", IsRequired = false)] public string Type;
            [DataMember(Name = "coordinates", IsRequired = false)] public double[] Coordinates = new double[3];
        }

        [DataContract]
        public class BoundingBox
        {
            [DataMember(Name = "type", IsRequired = false)] public string Type;
            [DataMember(Name = "coordinates", IsRequired = false)] public double[][][] Coordinates;
        }

        [DataContract]
        public class Attributes
        {
            [DataMember(Name = "street_address", IsRequired = false)] public string StreetAddress;
        }

        [DataContract]
        public class Place
        {
            [DataMember(Name = "url")] public string Url;
            [DataMember(Name = "bounding_box", IsRequired = false)] public BoundingBox BoundingBox;
            [DataMember(Name = "street_address", IsRequired = false)] public string StreetAddress;
            [DataMember(Name = "full_name")] public string FullName;
            [DataMember(Name = "name")] public string Name;
            //[DataMember(Name = "attributes", IsRequired = false)] public attributes attributes;
            [DataMember(Name = "country_code", IsRequired = false)] public string CountryCode;
            [DataMember(Name = "id")] public string Id;
            [DataMember(Name = "country")] public string Country;
            [DataMember(Name = "place_type")] public string PlaceType;
        }

        [DataContract]
        public class RetweetedStatus
        {
            [DataMember(Name = "coordinates", IsRequired = false)] public Coordinates Coordinates;
            [DataMember(Name = "geo", IsRequired = false)] public Geo Geo;
            [DataMember(Name = "in_reply_to_user_id")] public string InReplyToUserId;
            [DataMember(Name = "source")] public string Source;
            [DataMember(Name = "user")] public User User;
            [DataMember(Name = "in_reply_to_screen_name")] public string InReplyToScreenName;
            [DataMember(Name = "created_at")] public string CreatedAt;
            [DataMember(Name = "contributors")] public int[] Contributors;
            [DataMember(Name = "favorited")] public bool Favorited;
            [DataMember(Name = "truncated")] public string Truncated;
            [DataMember(Name = "id")] public Int64 Id;
            [DataMember(Name = "annotations", IsRequired = false)] public Annotations Annotations;
            [DataMember(Name = "place", IsRequired = false)] public Place Place;
            [DataMember(Name = "in_reply_to_status_id")] public string InReplyToStatusId;
            [DataMember(Name = "text")] public string Text;
            [DataMember(Name = "entities", IsRequired = false)] public Entities Entities;
        }

        [DataContract]
        public class Status
        {
            [DataMember(Name = "in_reply_to_status_id_str")] public string InReplyToStatusIdStr;
            [DataMember(Name = "contributors", IsRequired = false)] public int[] Contributors;
            [DataMember(Name = "in_reply_to_screen_name")] public string InReplyToScreenName;
            [DataMember(Name = "in_reply_to_status_id")] public string InReplyToStatusId;
            [DataMember(Name = "in_reply_to_user_id_str")] public string InReplyToUserIdStr;
            [DataMember(Name = "retweet_count")] public string RetweetCount;
            [DataMember(Name = "created_at")] public string CreatedAt;
            [DataMember(Name = "geo", IsRequired = false)] public Geo Geo;
            [DataMember(Name = "retweeted")] public bool Retweeted;
            [DataMember(Name = "in_reply_to_user_id")] public string InReplyToUserId;
            [DataMember(Name = "source")] public string Source;
            [DataMember(Name = "id_str")] public string IdStr;
            [DataMember(Name = "coordinates", IsRequired = false)] public Coordinates Coordinates;
            [DataMember(Name = "truncated")] public string Truncated;
            [DataMember(Name = "place", IsRequired = false)] public Place Place;
            [DataMember(Name = "user")] public User User;
            [DataMember(Name = "retweeted_status", IsRequired = false)] public RetweetedStatus RetweetedStatus;
            [DataMember(Name = "id")] public Int64 Id;
            [DataMember(Name = "favorited")] public bool Favorited;
            [DataMember(Name = "text")] public string Text;
            [DataMember(Name = "entities", IsRequired = false)] public Entities Entities;
        }

        [DataContract]
        public class TargetObject : Status
        {
            [DataMember(Name = "mode")] public string Mode;
            [DataMember(Name = "description")] public string Description;
            [DataMember(Name = "slug")] public string Slug;
            [DataMember(Name = "uri")] public string Uri;
            [DataMember(Name = "member_count")] public int MemberCount;
            [DataMember(Name = "full_name")] public string FullName;
            [DataMember(Name = "subscriber_count")] public int SubscriberCount;
            [DataMember(Name = "name")] public string Name;
            [DataMember(Name = "following")] public bool Following;
        }

        [DataContract]
        public class Directmessage
        {
            [DataMember(Name = "created_at")] public string CreatedAt;
            [DataMember(Name = "sender_id")] public Int64 SenderId;
            [DataMember(Name = "sender_screen_name")] public string SenderScreenName;
            [DataMember(Name = "sender")] public User Sender;
            [DataMember(Name = "id_str")] public string IdStr;
            [DataMember(Name = "recipient")] public User Recipient;
            [DataMember(Name = "recipient_screen_name")] public string RecipientScreenName;
            [DataMember(Name = "recipient_id")] public Int64 RecipientId;
            [DataMember(Name = "id")] public Int64 Id;
            [DataMember(Name = "text")] public string Text;
            [DataMember(Name = "entities", IsRequired = false)] public Entities Entities;
        }

        [DataContract]
        public class Friendsevent
        {
            [DataMember(Name = "friends")] public Int64[] Friends;
        }

        [DataContract]
        public class DeletedStatusContent
        {
            [DataMember(Name = "id")] public Int64 Id;
            [DataMember(Name = "user_id")] public Int64 UserId;
        }

        [DataContract]
        public class DeletedStatus
        {
            [DataMember(Name = "status")] public DeletedStatusContent Status;
        }

        [DataContract]
        public class DeleteEvent
        {
            [DataMember(Name = "delete")] public DeletedStatus Event;
        }

        [DataContract]
        public class DeletedDirectmessage
        {
            [DataMember(Name = "direct_message")] public DeletedStatusContent Directmessage;
        }

        [DataContract]
        public class DeleteDirectmessageEvent
        {
            [DataMember(Name = "delete")] public DeletedDirectmessage Event;
        }
        [DataContract]
        public class DirectmessageEvent
        {
            [DataMember(Name = "direct_message")] public Directmessage Directmessage;
        }

        [DataContract]
        public class TrackCount
        {
            [DataMember(Name = "track")] public int Track;
        }

        [DataContract]
        public class LimitEvent
        {
            [DataMember(Name = "limit")] public TrackCount Limit;
        }

        [DataContract]
        public class EventData
        {
            [DataMember(Name = "target")] public User Target;
            [DataMember(Name = "target_object", IsRequired = false)] public TargetObject TargetObject;
            [DataMember(Name = "created_at")] public string CreatedAt;
            [DataMember(Name = "event")] public string Event;
            [DataMember(Name = "source")] public User Source;
        }

        [DataContract]
        public class RelatedTweet
        {
            [DataMember(Name = "annotations")] public Annotations Annotations;
            [DataMember(Name = "kind")] public string Kind;
            [DataMember(Name = "score")] public double Score;
            [DataMember(Name = "value")] public Status Status;
        }

        [DataContract]
        public class RelatedResult
        {
            [DataMember(Name = "annotations")] public Annotations Annotations;
            [DataMember(Name = "groupName")] public string GroupName;
            [DataMember(Name = "resultType")] public string ResultType;
            [DataMember(Name = "results")] public RelatedTweet[] Results;
            [DataMember(Name = "score")] public double Score;
        }

        [DataContract]
        public class RelationshipResult
        {
            [DataMember(Name = "followed_by")] public bool FollowedBy;
            [DataMember(Name = "following")] public bool Following;
        }

        [DataContract]
        public class RelationshipUsers
        {
            [DataMember(Name = "target")] public RelationshipResult Target;
            [DataMember(Name = "source")] public RelationshipResult Source;
        }

        [DataContract]
        public class Relationship
        {
            [DataMember(Name = "relationship")] public RelationshipUsers relationship;
        }

        [DataContract]
        public class Ids
        {
            [DataMember(Name = "ids")] public long[] Id;
            [DataMember(Name = "next_cursor")] public long NextCursor;
            [DataMember(Name = "previous_cursor")] public long PreviousCursor;
        }

        [DataContract]
        public class RateLimitStatus
        {
            [DataMember(Name = "reset_time_in_seconds")] public int RestTimeInSeconds;
            [DataMember(Name = "remaining_hits")] public int RemainingHits;
            [DataMember(Name = "reset_time")] public string RestTime;
            [DataMember(Name = "hourly_limit")] public int HourlyLimit;
        }

        [DataContract]
        public class ListElementData
        {
            [DataMember(Name = "mode")] public string Mode;
            [DataMember(Name = "uri")] public string Uri;
            [DataMember(Name = "member_count")] public int MemberCount;
            [DataMember(Name = "slug")] public string Slug;
            [DataMember(Name = "full_name")] public string FullName;
            [DataMember(Name = "user")] public User User;
            [DataMember(Name = "following")] public bool Following;
            [DataMember(Name = "subscriber_count")] public int SubscriberCount;
            [DataMember(Name = "description")] public string Description;
            [DataMember(Name = "name")] public string Name;
            [DataMember(Name = "id")] public long Id;
        }

        [DataContract]
        public class Lists
        {
            [DataMember(Name = "lists")] public ListElementData[] lists;
            [DataMember(Name = "next_cursor")] public long NextCursor;
            [DataMember(Name = "previous_cursor")] public long PreviousCursor;
        }

        [DataContract]
        public class Users
        {
            [DataMember(Name = "users")] public User[] users;
            [DataMember(Name = "next_cursor")] public long NextCursor;
            [DataMember(Name = "previous_cursor")] public long PreviousCursor;
        }

        [DataContract]
        public class ErrorResponse
        {
            [DataMember(Name = "request")] public string Request;
            [DataMember(Name = "error")] public string ErrMsg;
        }

        [DataContract]
        public class SearchResultPhoenix
        {
            [DataMember(Name = "statuses")] public List<Status> Statuses;
            [DataMember(Name = "next_page")] public string NextPage;
            [DataMember(Name = "error")] public string ErrMsg;
        }

        [DataContract]
        public class SearchResult
        {
            [DataMember(Name = "completed_in")] public double CompletedIn;
            [DataMember(Name = "max_id")] public long MaxId;
            [DataMember(Name = "next_page")] public string NextPageQuery;
            [DataMember(Name = "page")] public int Page;
            [DataMember(Name = "query")] public string Query;
            [DataMember(Name = "refresh_url")] public string RefreshUrl;
            [DataMember(Name = "results")] public List<SearchResultData> Results;
            [DataMember(Name = "results_per_page")] public int ResultsPerPage;
            [DataMember(Name = "since_id")] public long SinceId;
        }

        [DataContract]
        public class SearchResultData
        {
            [DataMember(Name = "created_at")] public string CreatedAt;
            [DataMember(Name = "entities", IsRequired = false)] public Entities Entities;
            [DataMember(Name = "from_user")] public string FromUser;
            [DataMember(Name = "from_user_id")] public long FromUserId;
            [DataMember(Name = "from_user_name")] public string FromUserName;
            [DataMember(Name = "geo", IsRequired = false)] public Geo Geo;
            [DataMember(Name = "id")] public long Id;
            [DataMember(Name = "in_reply_to_status_id", IsRequired = false)] public long InReplyToStatusId = 0;
            [DataMember(Name = "iso_language_code")] public string IsoLanguageCode;
            [DataMember(Name = "meta_data")] public SearchMetaData MetaData;
            [DataMember(Name = "profile_image_url")] public string ProfileImageUrl;
            [DataMember(Name = "source")] public string Source;
            [DataMember(Name = "text")] public string Text;
            [DataMember(Name = "to_user")] public string ToUser;
            [DataMember(Name = "to_user_id")] public long? ToUserId;
            [DataMember(Name = "to_user_name")] public string ToUserName;
        }

        [DataContract]
        public class SearchMetaData
        {
            [DataMember(Name = "recent_retweets")] public int RecentRetweets;
            [DataMember(Name = "result_type")] public string ResultType;
        }

        [DataContract]
        public class PhotoSize
        {
            [DataMember(Name = "h")] public int Height;
            [DataMember(Name = "w")] public int Width;
            [DataMember(Name = "resize")] public string Resize;
        }

        [DataContract]
        public class PhotoType
        {
            [DataMember(Name = "large")] public PhotoSize LargeSize;
            [DataMember(Name = "medium")] public PhotoSize MediumSize;
            [DataMember(Name = "small")] public PhotoSize SmallSize;
            [DataMember(Name = "thumb")] public PhotoSize ThumbSize;
        }

        [DataContract]
        public class Configuration
        {
            [DataMember(Name = "characters_reserved_per_media")] public int CharactersReservedPerMedia = 20;
            [DataMember(Name = "photo_size_limit")] public int PhotoSizeLimit;
            [DataMember(Name = "photo_sizes")] public PhotoType PhotoSizes;
            [DataMember(Name = "non_username_paths")] public string[] NonUsernamePaths;
            [DataMember(Name = "short_url_length")] public int ShortUrlLength = 19;
            [DataMember(Name = "short_url_length_https")] public int ShortUrlLengthHttps = 20;
            [DataMember(Name = "max_media_per_upload")] public int MaxMediaPerUpload;
        }
    }
}
