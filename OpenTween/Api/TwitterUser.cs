// OpenTween - Client of Twitter
// Copyright (c) 2014 kim_upsilon (@kim_upsilon) <https://upsilo.net/~upsilon/>
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween.Api
{
    // 参照: https://dev.twitter.com/docs/platform-objects/users

    [DataContract]
    public class TwitterUser
    {
        [DataMember(Name = "contributors_enabled")]
        public bool ContributorsEnabled { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; } // Nullable

        [DataMember(Name = "entities", IsRequired = false)]
        public TwitterUser.TwitterUserEntity Entities { get; set; }

        [DataContract]
        public class TwitterUserEntity
        {
            [DataMember(Name = "url", IsRequired = false)]
            public TwitterEntities Url { get; set; }

            [DataMember(Name = "description", IsRequired = false)]
            public TwitterEntities Description { get; set; }
        }

        [DataMember(Name = "favourites_count")]
        public int FavouritesCount { get; set; }

        [DataMember(Name = "follow_request_sent")]
        public bool? FollowRequestSent { get; set; }

        [DataMember(Name = "following")]
        public bool? Following { get; set; }

        [DataMember(Name = "followers_count")]
        public int FollowersCount { get; set; }

        [DataMember(Name = "friends_count")]
        public int FriendsCount { get; set; }

        [DataMember(Name = "geo_enabled")]
        public bool GeoEnabled { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "id_str")]
        public string IdStr { get; set; }

        [DataMember(Name = "lang")]
        public string Lang { get; set; }

        [DataMember(Name = "listed_count")]
        public int? ListedCount { get; set; }

        [DataMember(Name = "location")]
        public string Location { get; set; } // Nullable

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [Obsolete]
        [DataMember(Name = "notifications")]
        public bool? Notifications { get; set; } // Nullable

        [DataMember(Name = "profile_background_color")]
        public string ProfileBackgroundColor { get; set; }

        [DataMember(Name = "profile_background_image_url_https")]
        public string ProfileBackgroundImageUrlHttps { get; set; }

        [DataMember(Name = "profile_background_tile")]
        public bool ProfileBackgroundTile { get; set; }

        [DataMember(Name = "profile_image_url_https")]
        public string ProfileImageUrlHttps { get; set; }

        [DataMember(Name = "profile_link_color")]
        public string ProfileLinkColor { get; set; }

        [DataMember(Name = "profile_sidebar_border_color")]
        public string ProfileSidebarBorderColor { get; set; }

        [DataMember(Name = "profile_sidebar_fill_color")]
        public string ProfileSidebarFillColor { get; set; }

        [DataMember(Name = "profile_text_color")]
        public string ProfileTextColor { get; set; }

        [DataMember(Name = "profile_use_background_image")]
        public bool ProfileUseBackgroundImage { get; set; }

        [DataMember(Name = "protected")]
        public bool Protected { get; set; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        [DataMember(Name = "show_all_inline_media")]
        public bool ShowAllInlineMedia { get; set; }

        [DataMember(Name = "status", IsRequired = false)]
        public TwitterStatus Status { get; set; } // Nullable

        [DataMember(Name = "statuses_count")]
        public int StatusesCount { get; set; }

        [DataMember(Name = "time_zone")]
        public string TimeZone { get; set; } // Nullable

        [DataMember(Name = "url")]
        public string Url { get; set; } // Nullable

        [DataMember(Name = "utc_offset")]
        public int? UtcOffset { get; set; }

        [DataMember(Name = "verified")]
        public bool Verified { get; set; }

        /// <exception cref="SerializationException"/>
        public static TwitterUser ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterUser>(json);
        }
    }
}
