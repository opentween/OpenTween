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
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OpenTween.Api.DataModel
{
    // 参照: https://dev.twitter.com/docs/platform-objects/tweets

    [DataContract]
    public class TwitterStatus
    {
        [DataMember(Name = "coordinates", IsRequired = false)]
        public GeoJsonPoint Coordinates { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "display_text_range")]
        public int[] DisplayTextRange { get; set; }

        [DataMember(Name = "entities")]
        public TwitterEntities Entities { get; set; }

        [DataMember(Name = "extended_entities", IsRequired = false)]
        public TwitterEntities ExtendedEntities { get; set; }

        [DataMember(Name = "favorite_count")]
        public int? FavoriteCount { get; set; }

        [DataMember(Name = "favorited")]
        public bool? Favorited { get; set; }

        [DataMember(Name = "filter_level")]
        public string FilterLevel { get; set; }

        [DataMember(Name = "full_text")]
        public string FullText { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "id_str")]
        public string IdStr { get; set; }

        [DataMember(Name = "in_reply_to_screen_name")]
        public string InReplyToScreenName { get; set; } // Nullable

        [DataMember(Name = "in_reply_to_status_id")]
        public long? InReplyToStatusId { get; set; }

        [DataMember(Name = "in_reply_to_status_id_str")]
        public string InReplyToStatusIdStr { get; set; } // Nullable

        [DataMember(Name = "in_reply_to_user_id")]
        public long? InReplyToUserId { get; set; }

        [DataMember(Name = "in_reply_to_user_id_str")]
        public string InReplyToUserIdStr { get; set; } // Nullable

        [DataMember(Name = "lang")]
        public string Lang { get; set; } // Nullable

        [DataMember(Name = "place", IsRequired = false)]
        public TwitterPlace Place { get; set; }

        [DataMember(Name = "possibly_sensitive")]
        public bool? PossiblySensitive { get; set; }

        [DataMember(Name = "quoted_status_id", IsRequired = false)]
        public long? QuotedStatusId { get; set; }

        [DataMember(Name = "quoted_status_id_str", IsRequired = false)]
        public string QuotedStatusIdStr { get; set; }

        [DataMember(Name = "quoted_status", IsRequired = false)]
        public TwitterStatus QuotedStatus { get; set; }

        [DataMember(Name = "retweet_count")]
        public int RetweetCount { get; set; }

        [DataMember(Name = "retweeted")]
        public bool Retweeted { get; set; }

        [DataMember(Name = "retweeted_status", IsRequired = false)]
        public TwitterStatus RetweetedStatus { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }

        [DataMember(Name = "truncated")]
        public bool Truncated { get; set; }

        [DataMember(Name = "user")]
        public TwitterUser User { get; set; }

        [DataMember(Name = "withheld_copyright")]
        public bool WithheldCopyright { get; set; }

        [DataMember(Name = "withheld_in_countries")]
        public string[] WithheldInCountries { get; set; }

        [DataMember(Name = "withheld_scope")]
        public string WithheldScope { get; set; }

        /// <summary>
        /// Entities と ExtendedEntities をマージした状態のエンティティを返します
        /// </summary>
        [IgnoreDataMember]
        public TwitterEntities MergedEntities
        {
            get
            {
                if (this.ExtendedEntities == null)
                    return this.Entities;

                return new TwitterEntities
                {
                    Hashtags = this.ExtendedEntities.Hashtags ?? this.Entities.Hashtags,
                    Media = this.ExtendedEntities.Media ?? this.Entities.Media,
                    Urls = this.ExtendedEntities.Urls ?? this.Entities.Urls,
                    UserMentions = this.ExtendedEntities.UserMentions ?? this.Entities.UserMentions,
                };
            }
        }

        /// <exception cref="SerializationException"/>
        public static TwitterStatus ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterStatus>(json);
        }

        /// <exception cref="SerializationException"/>
        public static TwitterStatus[] ParseJsonArray(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterStatus[]>(json);
        }
    }

    /// <summary>
    /// Streaming API または tweet_mode=compat の REST API から返されるツイート (Compatibility mode)
    /// </summary>
    [DataContract]
    public class TwitterStatusCompat
    {
        [DataMember(Name = "coordinates", IsRequired = false)]
        public GeoJsonPoint Coordinates { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "entities")]
        public TwitterEntities Entities { get; set; }

        [DataMember(Name = "extended_entities", IsRequired = false)]
        public TwitterEntities ExtendedEntities { get; set; }

        [DataMember(Name = "extended_tweet", IsRequired = false)]
        public TwitterStatusCompat.Extended ExtendedTweet { get; set; }

        [DataContract]
        public class Extended
        {
            [DataMember(Name = "display_text_range")]
            public int[] DisplayTextRange { get; set; }

            [DataMember(Name = "entities")]
            public TwitterEntities Entities { get; set; }

            [DataMember(Name = "extended_entities", IsRequired = false)]
            public TwitterEntities ExtendedEntities { get; set; }

            [DataMember(Name = "full_text")]
            public string FullText { get; set; }
        }

        [DataMember(Name = "favorite_count")]
        public int? FavoriteCount { get; set; }

        [DataMember(Name = "favorited")]
        public bool? Favorited { get; set; }

        [DataMember(Name = "filter_level")]
        public string FilterLevel { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "id_str")]
        public string IdStr { get; set; }

        [DataMember(Name = "in_reply_to_screen_name")]
        public string InReplyToScreenName { get; set; } // Nullable

        [DataMember(Name = "in_reply_to_status_id")]
        public long? InReplyToStatusId { get; set; }

        [DataMember(Name = "in_reply_to_status_id_str")]
        public string InReplyToStatusIdStr { get; set; } // Nullable

        [DataMember(Name = "in_reply_to_user_id")]
        public long? InReplyToUserId { get; set; }

        [DataMember(Name = "in_reply_to_user_id_str")]
        public string InReplyToUserIdStr { get; set; } // Nullable

        [DataMember(Name = "lang")]
        public string Lang { get; set; } // Nullable

        [DataMember(Name = "place", IsRequired = false)]
        public TwitterPlace Place { get; set; }

        [DataMember(Name = "possibly_sensitive")]
        public bool? PossiblySensitive { get; set; }

        [DataMember(Name = "quoted_status_id", IsRequired = false)]
        public long? QuotedStatusId { get; set; }

        [DataMember(Name = "quoted_status_id_str", IsRequired = false)]
        public string QuotedStatusIdStr { get; set; }

        [DataMember(Name = "quoted_status", IsRequired = false)]
        public TwitterStatusCompat QuotedStatus { get; set; }

        [DataMember(Name = "retweet_count")]
        public int RetweetCount { get; set; }

        [DataMember(Name = "retweeted")]
        public bool Retweeted { get; set; }

        [DataMember(Name = "retweeted_status", IsRequired = false)]
        public TwitterStatusCompat RetweetedStatus { get; set; }

        [DataMember(Name = "source")]
        public string Source { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "truncated")]
        public bool Truncated { get; set; }

        [DataMember(Name = "user")]
        public TwitterUser User { get; set; }

        [DataMember(Name = "withheld_copyright")]
        public bool WithheldCopyright { get; set; }

        [DataMember(Name = "withheld_in_countries")]
        public string[] WithheldInCountries { get; set; }

        [DataMember(Name = "withheld_scope")]
        public string WithheldScope { get; set; }

        /// <summary>Compatibility Modeのツイートを<see cref="TwitterStatus"/>に変換します</summary>
        public TwitterStatus Normalize()
        {
            var normalized = new TwitterStatus
            {
                Coordinates = this.Coordinates,
                CreatedAt = this.CreatedAt,
                FavoriteCount = this.FavoriteCount,
                Favorited = this.Favorited,
                FilterLevel = this.FilterLevel,
                Id = this.Id,
                IdStr = this.IdStr,
                InReplyToScreenName = this.InReplyToScreenName,
                InReplyToStatusId = this.InReplyToStatusId,
                InReplyToStatusIdStr = this.InReplyToStatusIdStr,
                InReplyToUserId = this.InReplyToUserId,
                InReplyToUserIdStr = this.InReplyToUserIdStr,
                Lang = this.Lang,
                Place = this.Place,
                PossiblySensitive = this.PossiblySensitive,
                QuotedStatusId = this.QuotedStatusId,
                QuotedStatusIdStr = this.QuotedStatusIdStr,
                QuotedStatus = this.QuotedStatus?.Normalize(),
                RetweetCount = this.RetweetCount,
                Retweeted = this.Retweeted,
                RetweetedStatus = this.RetweetedStatus?.Normalize(),
                Source = this.Source,
                User = this.User,
                WithheldCopyright = this.WithheldCopyright,
                WithheldInCountries = this.WithheldInCountries,
                WithheldScope = this.WithheldScope,
            };

            if (this.ExtendedTweet != null)
            {
                // Extended Tweet
                normalized.DisplayTextRange = this.ExtendedTweet.DisplayTextRange;
                normalized.Entities = this.ExtendedTweet.Entities;
                normalized.ExtendedEntities = this.ExtendedTweet.ExtendedEntities;
                normalized.FullText = this.ExtendedTweet.FullText;
                normalized.Truncated = false;
            }
            else
            {
                // Classic Tweet
                normalized.DisplayTextRange = new[] { 0, this.GetCodePointCount(this.Text) };
                normalized.Entities = this.Entities;
                normalized.ExtendedEntities = this.ExtendedEntities;
                normalized.FullText = this.Text;
                normalized.Truncated = this.Truncated;
            }

            return normalized;
        }

        /// <summary>Unicodeコードポイント単位の文字数を返します</summary>
        private int GetCodePointCount(string text)
            => text.Length - text.Sum(x => char.IsHighSurrogate(x) ? 1 : 0);

        /// <exception cref="SerializationException"/>
        public static TwitterStatusCompat ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterStatusCompat>(json);
        }

        /// <exception cref="SerializationException"/>
        public static TwitterStatusCompat[] ParseJsonArray(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterStatusCompat[]>(json);
        }
    }

    [DataContract]
    public class TwitterDirectMessage
    {
        [DataMember(Name = "entities", IsRequired = false)]
        public TwitterEntities Entities { get; set; }

        [DataMember(Name = "created_at")]
        public string CreatedAt { get; set; }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "id_str")]
        public string IdStr { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "sender_id")]
        public long SenderId { get; set; }

        [DataMember(Name = "sender_id_str")]
        public string SenderIdStr { get; set; }

        [DataMember(Name = "sender_screen_name")]
        public string SenderScreenName { get; set; }

        [DataMember(Name = "sender", IsRequired = false)]
        public TwitterUser Sender { get; set; }

        [DataMember(Name = "recipient_id")]
        public long RecipientId { get; set; }

        [DataMember(Name = "recipient_id_str")]
        public string RecipientIdStr { get; set; }

        [DataMember(Name = "recipient_screen_name")]
        public string RecipientScreenName { get; set; }

        [DataMember(Name = "recipient", IsRequired = false)]
        public TwitterUser Recipient { get; set; }

        /// <exception cref="SerializationException"/>
        public static TwitterDirectMessage ParseJson(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterDirectMessage>(json);
        }

        /// <exception cref="SerializationException"/>
        public static TwitterDirectMessage[] ParseJsonArray(string json)
        {
            return MyCommon.CreateDataFromJson<TwitterDirectMessage[]>(json);
        }
    }
}
