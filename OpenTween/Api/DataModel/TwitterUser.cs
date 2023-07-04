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

#nullable enable annotations

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween.Api.DataModel
{
    // 参照: https://developer.twitter.com/en/docs/tweets/data-dictionary/overview/user-object

    [DataContract]
    public class TwitterUser
    {
        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "description")]
        public string? Description { get; set; }

        [DataMember(Name = "default_profile")]
        public bool DefaultProfile { get; set; }

        [DataMember(Name = "default_profile_image")]
        public bool DefaultProfileImage { get; set; }

        [DataMember(Name = "entities", IsRequired = false)]
        public TwitterUser.TwitterUserEntity? Entities { get; set; }

        [DataContract]
        public class TwitterUserEntity
        {
            [DataMember(Name = "url", IsRequired = false)]
            public TwitterEntities? Url { get; set; }

            [DataMember(Name = "description", IsRequired = false)]
            public TwitterEntities? Description { get; set; }
        }

        [DataMember(Name = "favourites_count")]
        public int FavouritesCount { get; set; }

        [DataMember(Name = "followers_count")]
        public int FollowersCount { get; set; }

        [DataMember(Name = "friends_count")]
        public int FriendsCount { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "id_str")]
        public string IdStr { get; set; }

        [DataMember(Name = "listed_count")]
        public int? ListedCount { get; set; }

        [DataMember(Name = "location")]
        public string? Location { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "profile_banner_url")]
        public string ProfileBannerUrl { get; set; }

        [DataMember(Name = "profile_image_url_https")]
        public string ProfileImageUrlHttps { get; set; }

        [DataMember(Name = "protected")]
        public bool Protected { get; set; }

        [DataMember(Name = "screen_name")]
        public string ScreenName { get; set; }

        [DataMember(Name = "show_all_inline_media")]
        public bool ShowAllInlineMedia { get; set; }

        [DataMember(Name = "status", IsRequired = false)]
        public TwitterStatus? Status { get; set; }

        [DataMember(Name = "statuses_count")]
        public int StatusesCount { get; set; }

        [DataMember(Name = "url")]
        public string? Url { get; set; }

        [DataMember(Name = "verified")]
        public bool Verified { get; set; }

        /// <exception cref="SerializationException"/>
        public static TwitterUser ParseJson(string json)
            => MyCommon.CreateDataFromJson<TwitterUser>(json);

        public static TwitterUser CreateUnknownUser()
        {
            return new()
            {
                Id = 0L,
                IdStr = "0",
                ScreenName = "?????",
                Name = "Unknown User",
                ProfileImageUrlHttps = "",
            };
        }
    }
}
